using System.ComponentModel.DataAnnotations;

namespace WeddingPlanner.Models
{
    public class Attend
    {
        [Key]
        public int AttendId {get;set;}

        // Foreign Keys
        public int UserId {get;set;}
        public int WeddingId { get; set; }

        // Navigation Properties
        public User User {get;set;}
        public Wedding Wedding {get;set;}
    }
}