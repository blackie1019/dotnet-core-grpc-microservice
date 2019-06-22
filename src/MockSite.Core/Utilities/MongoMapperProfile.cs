#region

using AutoMapper;
using MockSite.Core.DTOs;
using MockSite.Core.Entities;

#endregion

namespace MockSite.Core.Utilities
{
    public class MongoMapperProfile : Profile
    {
        public MongoMapperProfile()
        {
            CreateMap<UserDto, UserEntity>();
        }
    }
}