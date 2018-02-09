using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace Snork.AspNet.DashboardBuilder
{
    internal sealed class OwinDashboardRequest : DashboardRequest
    {
        private readonly IOwinContext _context;

        public OwinDashboardRequest([NotNull] IDictionary<string, object> environment)
        {
            if (environment == null) throw new ArgumentNullException(nameof(environment));
            _context = new OwinContext(environment);
        }

        public override string Method => _context.Request.Method;
        public override string Path => _context.Request.Path.Value;
        public override string PathBase => _context.Request.PathBase.Value;
        public override string LocalIpAddress => _context.Request.LocalIpAddress;
        public override string RemoteIpAddress => _context.Request.RemoteIpAddress;

        public override string GetQuery(string key)
        {
            return _context.Request.Query[key];
        }

        public override async Task<IList<string>> GetFormValuesAsync(string key)
        {
            var form = await _context.ReadFormSafeAsync();
            return form.GetValues(key) ?? new List<string>();
        }
    }
}