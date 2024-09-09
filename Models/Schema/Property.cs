using System.ComponentModel.DataAnnotations;

namespace PropertyManagementSystem.Models.Schema
{
    public class Property
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Address { get; set; }

        public ICollection<PropertyManager> PropertyManagers { get; set; }
        public ICollection<PropertyOwner> PropertyOwners { get; set; }
        public ICollection<PropertyTenant> PropertyTenants { get; set; }
        public ICollection<Job> Jobs { get; set; }


    }

}

