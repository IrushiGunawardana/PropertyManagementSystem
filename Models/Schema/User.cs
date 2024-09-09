using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementSystem.Models.Schema
{
    public class User : IdentityUser<Guid>
    {

        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public ICollection<PropertyManager> PropertyManagers { get; set; }
        public ICollection<PropertyOwner> PropertyOwners { get; set; }
        public ICollection<PropertyTenant> PropertyTenants { get; set; }
        public ICollection<ServiceProvider> ServiceProviders { get; set; }
        public ICollection<Job> PostedJobs { get; set; }


    }
}

