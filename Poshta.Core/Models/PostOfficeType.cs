using CSharpFunctionalExtensions;

namespace Poshta.Core.Models
{
    public class PostOfficeType
    {
        public Guid Id { get; }

        public string Name { get; } = string.Empty;

        public float MaxShipmentWeight { get; }

        public float MaxShipmentLength { get; }

        public float MaxShipmentWidth { get; }

        public float MaxShipmentHeight { get; }

        private PostOfficeType(
            Guid id,
            string name,
            float maxShipmentWeight,
            float maxShipmentLength,
            float maxShipmentWidth,
            float maxShipmentHeight)
        {
            Id = id;
            Name = name;
            MaxShipmentWeight = maxShipmentWeight;
            MaxShipmentLength = maxShipmentLength;
            MaxShipmentWidth = maxShipmentWidth;
            MaxShipmentHeight = maxShipmentHeight;
        }

        public static Result<PostOfficeType> Create(
            Guid id,
            string name,
            float maxShipmentWeight,
            float maxShipmentLength,
            float maxShipmentWidth,
            float maxShipmentHeight)
        {
            if (string.IsNullOrEmpty(name))
                return Result.Failure<PostOfficeType>("Name can't be null or empty string");

            if (maxShipmentWeight <= 0)
                return Result.Failure<PostOfficeType>("Weight can't be equel or less than 0");

            if (maxShipmentLength <= 0)
                return Result.Failure<PostOfficeType>("Length can't be equel or less than 0");

            if (maxShipmentWidth <= 0)
                return Result.Failure<PostOfficeType>("Width can't be equel or less than 0");

            if (maxShipmentHeight <= 0)
                return Result.Failure<PostOfficeType>("Height can't be equel or less than 0");

            return new PostOfficeType(
                id,
                name, maxShipmentWeight,
                maxShipmentLength,
                maxShipmentWidth,
                maxShipmentHeight);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                Id,
                Name,
                MaxShipmentWeight,
                MaxShipmentLength,
                MaxShipmentWidth,
                MaxShipmentHeight);
        }

        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = (PostOfficeType)obj;
            return Id == other.Id &&
                Name == other.Name &&
                MaxShipmentWeight == other.MaxShipmentWeight &&
                MaxShipmentLength == other.MaxShipmentLength &&
                MaxShipmentWidth == other.MaxShipmentWidth &&
                MaxShipmentHeight == other.MaxShipmentHeight;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
