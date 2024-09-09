namespace PropertyManagementSystem.Dtos
{
    public class CreateJobRequestDto
    {
        public Guid PropertyId {  get; set; }
        public string Description { get; set; }

        public Guid Type { get; set; }

        public Guid ServiceProviderId { get; set; }

    }
}
