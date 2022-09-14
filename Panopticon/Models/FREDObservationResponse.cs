namespace Panopticon.Models
{
    public class FREDObservationResponse
    {
        public string realtime_start { get; set; }
        public string realtime_end { get; set; }
        public string observation_start { get; set; }
        public string observation_end { get; set; }
        public string units { get; set; }
        public int output_type { get; set; }
        public string file_type { get; set; }
        public string order_by { get; set; }
        public string sort_order { get; set; }
        public int count { get; set; }
        public int offset { get; set; }
        public int limit { get; set; }
        public Observation[] observations { get; set; }
    }

    public class Observation
    {
        public string realtime_start { get; set; }
        public string realtime_end { get; set; }
        public string date { get; set; }
        public string value { get; set; }
    }
}
