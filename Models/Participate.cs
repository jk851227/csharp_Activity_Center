using System;
using System.ComponentModel.DataAnnotations;

namespace ActivityCenter.Models
{
    public class Participate
    {
        [Key]
        public int ParticipateId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Foreign Keys
        public int UserId { get; set; }
        public int ActivityId { get; set; }

        // Navigation Property
        public User Participant { get; set; }
        public Activity Activity { get; set; }

        
    }
}