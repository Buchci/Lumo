namespace Lumo.Models
{
    public class ThemeOptions
    {
        public ThemeColors Light { get; set; }
        public ThemeColors Dark { get; set; }
    }

    public class ThemeColors
    {
        public string MainColor { get; set; }
        public string SecondaryColor { get; set; }
        public string BackgroundColor { get; set; }
        public string TextColor { get; set; }
    }
}
