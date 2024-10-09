using CSharpFunctionalExtensions;

namespace Poshta.Core.Models
{
    public class PostOffice
    {
        public const int MIN_SHIPMENTS_COUNT = 15;

        public const double MIN_LATITUDE = -90;
        public const double MAX_LATITUDE = 90;

        public const double MIN_LONGITUDE = -180;
        public const double MAX_LONGITUDE = 180;

        private readonly List<Shipment> shipments = [];

        private readonly List<Operator> operators = [];

        private PostOffice(
            Guid id,
            int number,
            string city, 
            string address, 
            int maxShipmentsCount, 
            double latitude,
            double longitude,
            PostOfficeType type)
        {
            Id = id;
            Number = number;
            City = city;
            Address = address;
            MaxShipmentsCount = maxShipmentsCount;
            Latitude = latitude;
            Longitude = longitude;
            Type = type;
        }

        public Guid Id { get; }

        public int Number { get; }

        public string City { get; } = string.Empty;

        public string Address { get; } = string.Empty;

        public int MaxShipmentsCount { get; }

        public double Latitude { get; }

        public double Longitude { get; }

        public PostOfficeType Type { get; }

        public IReadOnlyList<Shipment> Shipments => shipments;

        public IReadOnlyList<Operator> Operators => operators;

        public static Result<PostOffice> Create(
            Guid id,
            int number,
            string city,
            string address,
            int maxShipmentsCount,
            double latitude,
            double longitude,
            PostOfficeType type)
        {
            if (number <= 0)
                return Result.Failure<PostOffice>("post office number cannot be less or equel 0");

            if (string.IsNullOrEmpty(city))
                return Result.Failure<PostOffice>("city cannot be null or empty");

            if (string.IsNullOrEmpty(address))
                return Result.Failure<PostOffice>("address cannot be null or empty");

            if (maxShipmentsCount < MIN_SHIPMENTS_COUNT)
                return Result.Failure<PostOffice>($"max shipments count cannot be less then {MIN_SHIPMENTS_COUNT}");

            if (latitude < MIN_LATITUDE || latitude > MAX_LATITUDE)
                return Result.Failure<PostOffice>($"latitude cannot be less then {MIN_LATITUDE} or more than {MAX_LATITUDE}");

            if (longitude < MIN_LONGITUDE || longitude > MAX_LONGITUDE)
                return Result.Failure<PostOffice>($"longitude cannot be less then {MIN_LONGITUDE} or more than {MAX_LONGITUDE}");

            return new PostOffice(
                id,
                number,
                city,
                address,
                maxShipmentsCount,
                latitude,
                longitude,
                type);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, City, Address, Number, Latitude, Longitude, Type);
        }

        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = (PostOffice)obj;
            return Id == other.Id &&
                City == other.City &&
                Address == other.Address &&
                Number == other.Number &&
                Latitude == other.Latitude &&
                Longitude == other.Longitude &&
                Type == other.Type;
        }

        public override string ToString()
        {
            return $"City: {City}. Address: {Address}. Number: {Number}. Type: {Type}";
        }
    }
}
