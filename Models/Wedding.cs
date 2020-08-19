using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WeddingPlanner.Models
{
    public class Wedding
    {
        [Key]
        public int WeddingId {get;set;}

        [Required]
        [Display(Name = "Wedder One")]
        public string WedderOne {get;set;}

        [Required]
        [Display(Name = "Wedder Two")]
        public string WedderTwo {get;set;}

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Date {get;set;}

        [Required]
        public string Address {get;set;}
        
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;

        // Foreign Keys
        public int UserId {get;set;}

        // Navigation Properties
        public User Creator {get;set;}
        public List<Attend> WeddingAttenders {get;set;}
    }
}