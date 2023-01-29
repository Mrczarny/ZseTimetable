namespace TimetableLib.Models.ScrapperModels
{
    public class ScrapperOption
    {
        public const string Position = "Scrappers";
        public string Name { get; set; }
        public string Pattern { get; set; }
        public int RegexOptions { get; set; }
    }
}