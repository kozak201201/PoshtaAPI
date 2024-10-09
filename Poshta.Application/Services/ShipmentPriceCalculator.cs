using Poshta.Core.Models;

namespace Poshta.Application.Services
{
    public static class ShipmentPriceCalculator
    {
        private const int PriceForStorage = 30;

        private const int ShipmentMiniWeight = 2;
        private const int ShipmentNormalWeight = 10;

        private const int PriceForShipmentUpTo2kgCity = 40;
        private const int PriceForShipmentUpTo10kgCity = 60;
        private const int PriceForShipmentUpTo30kgCity = 80;

        private const int PriceForShipmentUpTo2kgCountry = 70;
        private const int PriceForShipmentUpTo10kgCountry = 100;
        private const int PriceForShipmentUpTo30kgCountry = 140;

        private const int MAX_APPRAISED_VALUE_WITHOUT_COMMISSION = 500;
        private const int APPRAISED_VALUE_COMMISSION_PERCENT = 5;

        public static double CalculatePrice(
            float weight,
            double appraisedValue,
            PostOffice startPostOffice,
            PostOffice endPostOffice)
        {
            double price;

            // storage function
            if (startPostOffice.Id == endPostOffice.Id)
            {
                return PriceForStorage;
            }

            // local delivery (one city)
            if (startPostOffice.City == endPostOffice.City)
            {
                if (weight <= ShipmentMiniWeight)
                {
                    price = PriceForShipmentUpTo2kgCity;
                }
                else if (weight <= ShipmentNormalWeight)
                {
                    price = PriceForShipmentUpTo10kgCity;
                }
                else
                {
                    price = PriceForShipmentUpTo30kgCity;
                }
            }
            // delivery throughout the country
            else
            {
                if (weight <= ShipmentMiniWeight)
                {
                    price = PriceForShipmentUpTo2kgCountry;
                }
                else if (weight <= ShipmentNormalWeight)
                {
                    price = PriceForShipmentUpTo10kgCountry;
                }
                else
                {
                    price = PriceForShipmentUpTo30kgCountry;
                }
            }
            // Appraised value commission
            if (appraisedValue >= MAX_APPRAISED_VALUE_WITHOUT_COMMISSION)
            {
                price += appraisedValue * APPRAISED_VALUE_COMMISSION_PERCENT / 100;
            }

            return price;
        }
    }
}
