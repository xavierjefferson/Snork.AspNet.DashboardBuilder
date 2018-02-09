using System;

namespace Snork.AspNet.DashboardBuilder
{
    public class UrlHelper
    {
        private readonly DashboardContext _context;

        public UrlHelper([NotNull] DashboardContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            _context = context;
        }

        public string To(string relativePath)
        {
            return
#if NETFULL
                _owinContext?.Request.PathBase.Value ??
#endif
                _context.Request.PathBase
                + relativePath;
        }

        public string Home()
        {
            return To("/");
        }
    }
}