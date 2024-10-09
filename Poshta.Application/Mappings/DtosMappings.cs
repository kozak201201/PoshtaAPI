using AutoMapper;
using Poshta.Application.Dtos;
using Poshta.Core.Models;

namespace Poshta.Application.Mappings
{
    public class DtosMappings : Profile
    {
        public DtosMappings()
        {
            CreateMap<User, UserDetails>();
        }
    }
}
