namespace Store.Web.Models
{
    public class Noticiation
    {
        public string Name { get; set; } = string.Empty;

        public NoticiationLevel Level { get; set; } = NoticiationLevel.Info;

        public enum NoticiationLevel
        {
            Info,
            Warning,
            Danger,
            Success
        }
    }
}
