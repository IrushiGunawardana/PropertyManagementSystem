using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PropertyManagementSystem.Models.Schema
{
    public class ServiceProvider
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string CompanyName { get; set; }
        public string Email { get; set; }


        [ForeignKey("UserId")]
        public User User { get; set; }
        public ICollection<ServiceProviderJobType> ServiceProviderJobTypes { get; set; }
        public ICollection<Job> Jobs { get; set; }


    }
}
