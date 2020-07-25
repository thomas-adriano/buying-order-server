using AutoMapper;
using buying_order_server.Data.Entity;
using buying_order_server.DTO.Response;
using buying_order_server.DTO.Request;

namespace buying_order_server.Infrastructure.Configs
{
    public class MappingProfileConfiguration : Profile
    {
        public MappingProfileConfiguration()
        {
            CreateMap<AppConfigurationEntity, AppConfigurationDTO>().ReverseMap();

            CreateMap<PostponedOrderEntity, PostponedOrderDTO>().ReverseMap();
        }
    }
}
