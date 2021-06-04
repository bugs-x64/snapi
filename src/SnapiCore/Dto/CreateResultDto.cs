using System;

namespace SnapiCore.Dto
{
    public class CreateUserDto
    {
        public DateTime Created { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
    }
}