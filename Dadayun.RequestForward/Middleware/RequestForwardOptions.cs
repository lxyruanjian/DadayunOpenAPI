namespace Dadayun.RequestForward
{
    public class RequestForwardOptions
    {
        public string DownstreamScheme { get; set; }
        public string DownstreamHost { get; set; }
        public int DownstreamPort { get; set; }
        public string UpstreamPath { get; set; } = "/bpm";
    }
}
