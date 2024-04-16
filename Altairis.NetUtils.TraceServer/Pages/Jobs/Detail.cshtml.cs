namespace Altairis.NetUtils.TraceServer.Pages.Jobs;

public class DetailModel(NetUtilsDbContext dc) : PageModel {
    public TraceJob Job { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(Guid id) {
        var job = await dc.TraceJobs.SingleOrDefaultAsync(x => x.Id == id);
        if (job == null) return this.NotFound();

        // Set autorefresh for non-final statuses
        if (job.Status == TraceJobStatus.Waiting || job.Status == TraceJobStatus.Processing) this.HttpContext.Response.Headers.Append("Refresh", "5");

        // Display result page
        this.Job = job;
        return this.Page();
    }
}
