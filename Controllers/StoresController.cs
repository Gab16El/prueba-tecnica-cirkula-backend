using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CirkulaApi.Data;
using CirkulaApi.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace CirkulaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoresController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public StoresController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;

        }

        [HttpGet]
        public async Task<IActionResult> GetStores([FromQuery] double latitude, [FromQuery] double longitude)
        {
            var stores = await _context.Stores.ToListAsync();

            var result = stores.Select(store => new
            {
                store.Id,
                store.Name,
                store.BannerUrl,
                store.Latitude,
                store.Longitude,
                store.OpenTime,
                store.CloseTime,
                distanceInKm = CalculateDistance(latitude, longitude, store.Latitude, store.Longitude),
                isOpen = IsStoreOpen(store.OpenTime, store.CloseTime)
            });

            return Ok(new { stores = result });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStoreById(int id)
        {
            var store = await _context.Stores.FindAsync(id);
            if (store == null) return NotFound();
            return Ok(store);
        }

        [HttpPost]
        public async Task<IActionResult> CreateStore([FromForm] string name, [FromForm] double latitude, [FromForm] double longitude, [FromForm] string openTime, [FromForm] string closeTime, IFormFile? banner)
        {
            string bannerUrl = "";
            if (banner != null)
            {
                var cloudinary = new Cloudinary(new Account(
                    _configuration["Cloudinary:CloudName"],
                    _configuration["Cloudinary:ApiKey"],
                    _configuration["Cloudinary:ApiSecret"]
                ));

                using var stream = banner.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(banner.FileName, stream),
                    Folder = "cirkula/stores"
                };

                var uploadResult = await cloudinary.UploadAsync(uploadParams);
                bannerUrl = uploadResult.SecureUrl.ToString();
            }

            var store = new Store
            {
                Name = name,
                BannerUrl = bannerUrl,
                Latitude = latitude,
                Longitude = longitude,
                OpenTime = openTime,
                CloseTime = closeTime
            };

            _context.Stores.Add(store);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetStoreById), new { id = store.Id }, store);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStore(int id, [FromBody] Store store)
        {
            if (id != store.Id) return BadRequest();
            _context.Entry(store).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStore(int id)
        {
            var store = await _context.Stores.FindAsync(id);
            if (store == null) return NotFound();
            _context.Stores.Remove(store);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371;
            var dLat = ToRad(lat2 - lat1);
            var dLon = ToRad(lon2 - lon1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return Math.Round(R * c, 2);
        }

        private static double ToRad(double deg) => deg * (Math.PI / 180);

        private static bool IsStoreOpen(string openTime, string closeTime)
        {
            var now = DateTime.Now.TimeOfDay;
            var open = DateTime.ParseExact(openTime, "hh:mm tt", null).TimeOfDay;
            var close = DateTime.ParseExact(closeTime, "hh:mm tt", null).TimeOfDay;
            return now >= open && now <= close;
        }
    }
}
