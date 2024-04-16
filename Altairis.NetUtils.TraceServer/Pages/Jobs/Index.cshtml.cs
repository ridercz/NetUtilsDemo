

namespace Altairis.NetUtils.TraceServer.Pages.Jobs;

public class IndexModel(NetUtilsDbContext dc) : PageModel {

    public IEnumerable<TraceJob> Jobs = [];

    public async Task OnGetAsync() {
        this.Jobs = await dc.TraceJobs.OrderByDescending(x => x.DateCreated).ToListAsync();
    }

}
