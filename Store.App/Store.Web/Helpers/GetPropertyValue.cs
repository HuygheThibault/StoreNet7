namespace Store.Web.Helpers
{
    public class GetPropertyValue
    {
        public static object GetPropValue(object src, string propName)
        {
            return src?.GetType()?.GetProperty(propName)?.GetValue(src, null);
        }
    }
}
