namespace DataService.Infrastructure.Entities
{
    public class Notification : BaseEntity
    {
        public DateTime CreateAt { get; set; }

        public required string Message { get; set; }
        
        public required short Type { get; set; }

        public required short Action { get; set; }

        public bool IsRead { get; set; }
    }
}
