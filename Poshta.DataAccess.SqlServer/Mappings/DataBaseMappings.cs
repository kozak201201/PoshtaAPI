using AutoMapper;
using Poshta.Core.Models;
using Poshta.DataAccess.SqlServer.Entities;

namespace Poshta.DataAccess.SqlServer.Mappings
{
    public class DataBaseMappings : Profile
    {
        public DataBaseMappings()
        {
            CreateMap<User, UserEntity>().ReverseMap();
            CreateMap<ShipmentHistory, ShipmentHistoryEntity>().ReverseMap();
            CreateMap<ShipmentHistoryEntity, ShipmentHistory>()
                .ConstructUsing(src => ShipmentHistory.Create(src.Id, src.ShipmentId, src.Status, src.PostOfficeId, src.StatusDate, src.Description).Value)
                .ReverseMap();
            CreateMap<Shipment, ShipmentEntity>().ReverseMap();
            CreateMap<PostOffice, PostOfficeEntity>().ReverseMap();
            CreateMap<OperatorRatingEntity, OperatorRating>()
                .ConstructUsing(src => OperatorRating.Create(src.Id, src.OperatorId, src.UserId, src.Rating, src.Review, src.CreatedAt).Value)
                .ReverseMap();
            CreateMap<Operator, OperatorRating>().ReverseMap();
            CreateMap<Operator, OperatorEntity>().ReverseMap();
            CreateMap<PostOfficeType, PostOfficeTypeEntity>().ReverseMap();
        }
    }
}
