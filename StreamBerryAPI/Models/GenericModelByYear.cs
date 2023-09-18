namespace StreamBerryAPI.Models
{

    public class TotalByYear
    {
        public int Year { get; set; }
        public int TotalYear { get; set; }
    }

    public class GenericModelByYear<T>
    {

        public int FilterByYear { get; set; }
        public List<TotalByYear>? TotalYear {get;set;}

        public List<T>? Data { get; set; }

        public GenericModelByYear()
        {
            TotalYear = new List<TotalByYear>();
            Data = new List<T>();
        }
    }
}
