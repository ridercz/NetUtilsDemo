using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Altairis.NetUtils.TraceServer.Pages {
    public class IndexModel : PageModel {
        private readonly NetUtilsDbContext dc;

        public IndexModel(NetUtilsDbContext dc) {
            this.dc = dc ?? throw new ArgumentNullException(nameof(dc));
        }

        [BindProperty]
        public string? Host { get; set; }

        public async Task<IActionResult> OnPostAsync() {
            // Redirect to self on empty host name
            if (string.IsNullOrWhiteSpace(this.Host)) return this.RedirectToPage();

            // Add new job
            var newJob = new TraceJob {
                Id = Guid.NewGuid(),
                DateCreated = DateTime.Now,
                Host = this.Host
            };
            await this.dc.TraceJobs.AddAsync(newJob);
            await this.dc.SaveChangesAsync();

            // Redirect to job status page
            return this.RedirectToPage("/Jobs/Detail", new { id = newJob.Id });
        }

    }
}