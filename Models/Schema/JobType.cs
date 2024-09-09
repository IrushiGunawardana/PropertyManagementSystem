using System.ComponentModel.DataAnnotations;

namespace PropertyManagementSystem.Models.Schema
{
    public class JobType
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }

        public ICollection<ServiceProviderJobType> ServiceProviderJobTypes { get; set; }
        public ICollection<Job> Jobs { get; set; }
    }
}
