using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PetrolTrullyUnlimited
{
    /// <summary>
    /// Interação lógica para Main.xam
    /// </summary>
    public partial class Main : Page
    {
        private List<VehicleState> vehiclesQueue = new List<VehicleState>(); //List of Queued vehicles

        private int nextVehicleId = 0;

        public Main()
        {
            InitializeComponent();

            //var timer = new Timer(e => Functions.CreateVehicle(nextVehicleId++), null, -1, 1500);

            Timer timer2 = new Timer();
            timer2.Interval = 500;
            timer2.Elapsed += timer_Elapsed;
            timer2.AutoReset = true;
            timer2.Start();
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            CreateVehicle(nextVehicleId++);
        }

        /// <summary>
        /// Creates a new random vehicle
        /// </summary>
        /// <param name="id">Sequential id</param>
        /// <returns></returns>
        private void CreateVehicle(int id)
        {
            Vehicle[] vehicles = { Objects.car, Objects.van, Objects.lorry };
            VehicleState vehicle;

            Random random = new Random();

            byte iVehicle = (byte)random.Next(0, vehicles.Length);
            byte iFuel = (byte)random.Next(0, vehicles[iVehicle].fuel.Length);
            float remaining = (float)(random.NextDouble() * vehicles[iVehicle].capacity);


            vehicle = new VehicleState
            {
                id = id,
                type = vehicles[iVehicle].type,
                fuel = vehicles[iVehicle].fuel[iFuel],
                litres = remaining
            };

            vehiclesQueue.Add(vehicle);

            Dispatcher.Invoke(() =>
            {
                VehicleImage(vehicle.type);
            });
        }

        /// <summary>
        /// Creates the vehicle image in the GUI
        /// </summary>
        /// <param name="type">Type of vehicle to display</param>
        /// <returns></returns>
        public void VehicleImage(string type)
        {
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

            Wrp_Queue.Children.Add(image);
        }
    }
}
