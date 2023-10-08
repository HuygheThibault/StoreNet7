namespace Store.Web.Models
{
    public class Noticiation
    {
        public string Name { get; set; }

        public NoticiationType Sort { get; set; }

        public enum NoticiationType
        {
            Info,
            Warning,
            Danger,
            Success
        }
    }
}
