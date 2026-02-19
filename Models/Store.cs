namespace CirkulaApi.Models
{
    public class Store
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string BannerUrl { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string OpenTime { get; set; } = string.Empty;
        public string CloseTime { get; set; } = string.Empty;

    }
}
