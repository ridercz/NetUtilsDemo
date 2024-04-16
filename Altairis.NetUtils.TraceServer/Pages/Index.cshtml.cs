namespace Altairis.NetUtils.TraceServer.Pages;

public class IndexModel(NetUtilsDbContext dc) : PageModel {
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
        await dc.TraceJobs.AddAsync(newJob);
        await dc.SaveChangesAsync();

        // Redirect to job status page
        return this.RedirectToPage("/Jobs/Detail", new { id = newJob.Id });
    }

}