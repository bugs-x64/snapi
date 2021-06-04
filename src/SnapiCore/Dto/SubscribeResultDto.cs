using System;

namespace SnapiCore.Dto
{
    public class SubscribeResultDto
    {
        
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public int FromId { get; set; }
        public int ToId { get; set; }
    }
}