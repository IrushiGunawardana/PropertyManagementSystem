using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementSystem.Models.Schema
{
    public class ServiceProviderJobType
    {

        [Key]
        public Guid Id { get; set; }
        public Guid JobTypeId { get; set; }

        public Guid ServiceProviderUserId { get; set; }


        [ForeignKey("ServiceProviderUserId")]
        public ServiceProvider ServiceProvider { get; set; }

        [ForeignKey("JobTypeId")]
        public JobType JobType { get; set; }
    }
}
