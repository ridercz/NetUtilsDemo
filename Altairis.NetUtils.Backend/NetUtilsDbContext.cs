namespace Altairis.NetUtils.Backend; 
public class NetUtilsDbContext(DbContextOptions<NetUtilsDbContext> options) : DbContext(options) {

    public DbSet<TraceJob> TraceJobs => this.Set<TraceJob>();

}
