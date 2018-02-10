namespace Snork.AspNet.DashboardBuilder
{
    public class ByteResource : StaticResource<byte[]>
    {
        public ByteResource(byte[] value, string contentType = "application/octet-stream") : base(value, contentType)
        {
        }
    }
}