using System;
using System.ComponentModel.DataAnnotations;

namespace SnapiCore.Data.Models
{
    public class SubscriberLink
    {
        [Key]
        public int Id { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public int FromId { get; set; }
        public int ToId { get; set; }
        public User From { get; set; }
        public User To { get; set; }
    }
}