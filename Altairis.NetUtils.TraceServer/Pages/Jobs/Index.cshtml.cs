using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Altairis.NetUtils.TraceServer.Pages.Jobs {
    public class IndexModel : PageModel {
        private readonly NetUtilsDbContext dc;

        public IndexModel(NetUtilsDbContext dc) {
            this.dc = dc ?? throw new ArgumentNullException(nameof(dc));
        }

        public IEnumerable<TraceJob> Jobs => this.dc.TraceJobs.OrderByDescending(x => x.DateCreated).ToList();

    }
}
