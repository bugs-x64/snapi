using SnapiCore.Data.Models;
using SnapiCore.Dto;

namespace SnapiCore.Services
{
    public static class DtoConverter
    {
        public static CreateUserDto ToDto(User user)
        {
            return new ()
            {
                Id = user.Id,
                Name = user.Name,
                Created = user.Created
            };
        }
        
        
        public static SubscribeResultDto ToDto(SubscriberLink user)
        {
            return new ()
            {
                Id = user.Id,
                Created = user.Created,
                FromId = user.FromId,
                ToId = user.ToId
            };
        }
    }
}