using System.ComponentModel.DataAnnotations;

namespace Altairis.NetUtils.Backend {
    public class TraceJob {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public TraceJobStatus Status { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }

        public DateTime? DateStarted { get; set; }

        public DateTime? DateCompleted { get; set; }

        [Required]
        public string Host { get; set; } = string.Empty;

        public string? Result { get; set; }

    }

    public enum TraceJobStatus {
        Waiting = 0,
        Processing = 1,
        Completed = 2,
        Error = 3,
    }

}
