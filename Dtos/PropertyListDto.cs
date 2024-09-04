namespace PropertyManagementSystem.Dtos
{
    public class PropertyListDto
    {
        public Guid Id { get; set; }
        public string Address { get; set; }

        public List<OwnerDto> ownersDetails { get; set; }

        public List<TenantDto> tenantsDetails { get; set; }


    }
    public class OwnerDto
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

    }

    public class TenantDto
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

    }


}
