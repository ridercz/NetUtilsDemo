namespace Altairis.NetUtils.Backend {
    public class NetUtilsDbContext : DbContext {

        public NetUtilsDbContext(DbContextOptions<NetUtilsDbContext> options) : base(options) { }

        public DbSet<TraceJob> TraceJobs => this.Set<TraceJob>();

    }
}
