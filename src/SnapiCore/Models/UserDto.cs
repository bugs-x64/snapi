namespace SnapiCore.Models
{
    public class UserDto
    {
        public string Name { get; set; }
        public int SubscribersCount { get; set; }
        public string[] Subscribers { get; set; }
    }
}