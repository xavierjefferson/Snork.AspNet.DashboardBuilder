using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace Snork.AspNet.DashboardBuilder
{
    internal sealed class OwinDashboardResponse : DashboardResponse
    {
        private readonly IOwinContext _context;

        public OwinDashboardResponse([NotNull] IDictionary<string, object> environment)
        {
            if (environment == null) throw new ArgumentNullException(nameof(environment));
            _context = new OwinContext(environment);
        }

        public override string ContentType
        {
            get => _context.Response.ContentType;
            set => _context.Response.ContentType = value;
        }

        public override int StatusCode
        {
            get => _context.Response.StatusCode;
            set => _context.Response.StatusCode = value;
        }

        public override Stream Body => _context.Response.Body;

        public override void SetExpire(DateTimeOffset? value)
        {
            _context.Response.Expires = value;
        }

        public override Task WriteAsync(string text)
        {
            return _context.Response.WriteAsync(text);
        }
    }
}