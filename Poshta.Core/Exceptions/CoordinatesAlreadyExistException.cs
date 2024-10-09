namespace Poshta.Core.Exceptions
{
    public class CoordinatesAlreadyExistException : Exception
    {
        public CoordinatesAlreadyExistException(double latitude, double longitude)
            : base($"Post office with coordinates (latitude: {latitude}, longitude: {longitude}) already exist.") { }
    }
}
