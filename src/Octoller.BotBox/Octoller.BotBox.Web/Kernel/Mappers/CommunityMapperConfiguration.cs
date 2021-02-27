using Octoller.BotBox.Web.Data.Models;
using Octoller.BotBox.Web.Kernel.Mappers.Base;
using Octoller.BotBox.Web.ViewModels;

namespace Octoller.BotBox.Web.Kernel.Mappers
{
    public class CommunityMapperConfiguration : MapperConfigurationBase
    {
        public CommunityMapperConfiguration()
        {
            CreateMap<Community, CommunityViewModel>()
                .ForMember(sour => sour.Name, o => o.MapFrom(s => s.Name))
                .ForMember(sour => sour.Photo, o => o.MapFrom(s => s.Photo))
                .ForMember(sour => sour.Connected, o => o.MapFrom(s => s.Connected));
        }
    }
}
