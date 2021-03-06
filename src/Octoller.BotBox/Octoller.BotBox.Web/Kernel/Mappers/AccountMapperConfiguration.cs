﻿using Octoller.BotBox.Web.Data.Models;
using Octoller.BotBox.Web.Kernel.Mappers.Base;
using Octoller.BotBox.Web.ViewModels;

namespace Octoller.BotBox.Web.Kernel.Mappers
{
    public class AccountMapperConfiguration : MapperConfigurationBase
    {
        public AccountMapperConfiguration()
        {
            CreateMap<Account, AccountViewModel>()
                .ForMember(dest => dest.Name, o => o.MapFrom(s => s.Name))
                .ForMember(dest => dest.Avatar, o => o.MapFrom(s => s.Photo))
                .ForMember(dest => dest.IsAccountConnected, o => o.MapFrom(s => s.VkId != null))
                .ForMember(dest => dest.CommunityConnectedCount, o => o.Ignore())
                .ForMember(dest => dest.CountTemplate, o => o.Ignore());
        }
    }
}
