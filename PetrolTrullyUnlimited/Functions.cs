using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;


namespace PetrolTrullyUnlimited
{
    class Functions
    {
        /// <summary>
        /// Creates the vehicle image in the GUI
        /// </summary>
        /// <param name="type">Type of vehicle to display</param>
        /// <returns></returns>
        public static Image VehicleImage(string type)
        {

            //Problemas com este elemento
            Image image = new Image
            {
                MaxWidth = 180,
                Margin = new Thickness(20, 5, 20, 5)
            };

            if (type == "Car")
            {
                image.Source = new BitmapImage(new Uri("Images/car.png", UriKind.Relative));
            }
            else if (type == "Van")
            {
                image.Source = new BitmapImage(new Uri("Images/van.png", UriKind.Relative));
            }
            else if (type == "Lorry")
            {
                image.Source = new BitmapImage(new Uri("Images/lorry.png", UriKind.Relative));
            }

            return image;
        }

        /// <summary>
        /// Creates a new random vehicle
        /// </summary>
        /// <param name="id">Sequential id</param>
        /// <returns></returns>
        public static void CreateVehicle(int id)
        {
            Vehicle[] vehicles = { Objects.car, Objects.van, Objects.lorry };
            VehicleState vehicle;

            Random random = new Random();

            byte iVehicle = (byte)random.Next(0, vehicles.Length - 1);
            byte iFuel = (byte)random.Next(0, vehicles[iVehicle].fuel.Length - 1);
            float remaining = (float)(random.NextDouble() * vehicles[iVehicle].capacity);


            vehicle = new VehicleState {
                id = id,
                type = vehicles[iVehicle].type,
                fuel = vehicles[iVehicle].fuel[iFuel],
                litres = remaining
            };

            Console.WriteLine(vehicle.id);

            Objects.vehiclesQueue.Add(vehicle);
        }
    }
}
