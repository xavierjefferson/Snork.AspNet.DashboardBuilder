namespace Snork.AspNet.DashboardBuilder
{
    public class StringResource : StaticResource<string>
    {
        public StringResource(string value, string contentType) : base(value, contentType)
        {
        }
    }
}