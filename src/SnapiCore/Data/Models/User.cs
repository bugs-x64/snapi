using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SnapiCore.Data.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; }

        [Required]
        public string IndexName { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;

        public ICollection<SubscriberLink> Subscribers { get; set; }
        
        public ICollection<SubscriberLink> Subscriptions { get; set; }
    }
}