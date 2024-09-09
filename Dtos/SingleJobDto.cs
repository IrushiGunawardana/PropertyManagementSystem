namespace PropertyManagementSystem.Dtos
{
    public class SingleJobDto
    {
        public Guid Id { get; set; }

        public SinglePropertyDtoResponse property { get; set; }
        public List<SingleOwnerDtoResponse> OwnerDetails { get; set; }

        public List<SingleTenantDtoResponse> TenantDetails { get; set; }

        public string Description { get; set; }
        public int JobNumber { get; set; }

        public JobTypeDtoResponse JobType { get; set; }

        public ProviderDtoResponse Provider { get; set; }


    }

    public class SinglePropertyDtoResponse
    {
        public Guid Id { get; set; }
        public string Address { get; set; }

    }
    public class SingleOwnerDtoResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

    }

    public class SingleTenantDtoResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

    }

    public class JobTypeDtoResponse
    {
        public Guid Id { get; set; }
        public string Name{ get; set; }

    }

    public class ProviderDtoResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string CompanyName { get; set; }
        public string Email { get; set; }

    }
}