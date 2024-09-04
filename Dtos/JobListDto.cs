namespace PropertyManagementSystem.Dtos
{
    public class JobListDto
    {
        public Guid id { get; set; }
        public int JobNumber { get; set; }
        public string Description { get; set; }
        public DateTime PostedDate { get; set; }
    }
}
