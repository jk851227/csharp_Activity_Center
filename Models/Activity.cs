using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ActivityCenter.Models
{
    public class Activity
    {
        [Key]
        public int ActivityId { get; set; }
        
        [Required]
        public string Title { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public DateTime Time { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        public string Duration { get; set; }
        [Required]
        public string DurationTime { get; set; }

        public string Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Foreign Key
        public int UserId { get; set; }

        // Navigation Property
        public User Coordinator { get; set; }
        public List<Participate> Participants { get; set; }

        [NotMapped]
        public string ActivityDuration
        {
            get
            {
                string activityDuration = $"{Duration} {DurationTime}";
                return activityDuration;
            }
        }

        [NotMapped]
        public string ActivityDateTime
        {
            get
            {
                string date = Date.ToString("MM/dd/yy");
                string time = Time.ToString("h:mm tt");
                return $"{date} @ {time}";
            }
        }
    }
}