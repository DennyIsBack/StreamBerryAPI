namespace StreamBerryAPI.Models
{
    public class RetPaged<T>
    {
        public int TotalPage { get;set; }

        public int TotalData { get;set; }

        public List<T>? Data { get; set; }
    }
}
