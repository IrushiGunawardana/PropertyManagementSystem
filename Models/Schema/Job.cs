using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementSystem.Models.Schema
{
    public class Job
    {
        [Key]
        public Guid Id { get; set; }
        public int JobNumber { get; set; }
        public Guid PropertyId { get; set; }
        public Guid PostedByUserId { get; set; }
        public DateTime PostedOn { get; set; }
        public string Description { get; set; }
        public Guid TypeId { get; set; }
        public Guid ServiceProviderId { get; set; }


        [ForeignKey("PropertyId")]
        public Property Property { get; set; }

        [ForeignKey("PostedByUserId")]
        public User PostedByUser { get; set; }

        [ForeignKey("TypeId")]
        public JobType JobType { get; set; }

        [ForeignKey("ServiceProviderId")]
        public ServiceProvider ServiceProvider { get; set; }
    }
}
