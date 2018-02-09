using System.Threading.Tasks;

namespace Snork.AspNet.DashboardBuilder
{
    public interface IDashboardDispatcher
    {
        Task Dispatch([NotNull] DashboardContext context);
    }
}