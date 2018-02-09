namespace Snork.AspNet.DashboardBuilder
{
    public interface IDashboardAuthorizationFilter
    {
        bool Authorize([NotNull] DashboardContext context);
    }
}