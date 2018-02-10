namespace Snork.AspNet.DashboardBuilder
{
    public class StaticResource<T>
    {
        public StaticResource(T value, string contentType)
        {
            Value = value;
            ContentType = contentType;
        }

        public T Value { get; }
        public string ContentType { get; }
    }
}