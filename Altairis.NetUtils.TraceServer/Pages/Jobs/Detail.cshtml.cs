using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Altairis.NetUtils.TraceServer.Pages.Jobs {
    public class DetailModel : PageModel
    {
        private readonly NetUtilsDbContext dc;

        public DetailModel(NetUtilsDbContext dc) {
            this.dc = dc ?? throw new ArgumentNullException(nameof(dc));
        }

        public TraceJob Job { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(Guid id) {
            var job = await this.dc.TraceJobs.SingleOrDefaultAsync(x => x.Id == id);
            if (job == null) return this.NotFound();

            // Set autorefresh for non-final statuses
            if (job.Status == TraceJobStatus.Waiting || job.Status == TraceJobStatus.Processing) this.HttpContext.Response.Headers.Add("Refresh", "5");

            // Display result page
            this.Job = job;
            return this.Page();
        }
    }
}
