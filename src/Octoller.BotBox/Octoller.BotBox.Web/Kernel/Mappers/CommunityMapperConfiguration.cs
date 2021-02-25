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
                .ForMember(sour => sour.Connected, o => o.MapFrom(s => s.Connected))
                .ForSourceMember(sour => sour.AccessToken, o => o.DoNotValidate())
                .ForSourceMember(sour => sour.CreatedAt, o => o.DoNotValidate())
                .ForSourceMember(sour => sour.CreatedBy, o => o.DoNotValidate())
                .ForSourceMember(sour => sour.Id, o => o.DoNotValidate())
                .ForSourceMember(sour => sour.TemplateBot, o => o.DoNotValidate())
                .ForSourceMember(sour => sour.UpdatedAt, o => o.DoNotValidate())
                .ForSourceMember(sour => sour.UpdatedBy, o => o.DoNotValidate())
                .ForSourceMember(sour => sour.User, o => o.DoNotValidate())
                .ForSourceMember(sour => sour.UserId, o => o.DoNotValidate())
                .ForSourceMember(sour => sour.VkId, o => o.DoNotValidate());
        }
        
    }
}
