namespace SnapiCore.Dto
{
    public class UserSubscriptionDto
    {
        public string Name { get; set; }
        public int SubscribersCount { get; set; }
        public string[] Subscribers { get; set; }
    }
}