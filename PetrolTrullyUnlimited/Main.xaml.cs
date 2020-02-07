using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
        private int nextVehicleId = 0;

        public Main()
        {
            InitializeComponent();

            //var timer = new Timer(e => Functions.CreateVehicle(nextVehicleId++), null, -1, 1500);

            System.Timers.Timer timer2 = new System.Timers.Timer();
            timer2.Interval = 1500;
            timer2.Elapsed += timer_Elapsed;
            timer2.AutoReset = true;
            timer2.Start();
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Vehicle[] vehicles = { Objects.car, Objects.van, Objects.lorry };
            VehicleState vehicle;

            Random random = new Random();

            byte iVehicle = (byte)random.Next(0, vehicles.Length);
            byte iFuel = (byte)random.Next(0, vehicles[iVehicle].fuel.Length);
            float remaining = (float)(random.NextDouble() * vehicles[iVehicle].capacity);


            vehicle = new VehicleState
            {
                id = nextVehicleId++,
                type = vehicles[iVehicle].type,
                fuel = vehicles[iVehicle].fuel[iFuel],
                litres = remaining
            };

            Objects.vehiclesQueue.Add(vehicle);

            foreach (VehicleState item in Objects.vehiclesQueue.ToArray())
            {
                _ = Functions.VehicleImage(item.type); //teste
                Wrp_Queue.Children.Add(Functions.VehicleImage(item.type));
            }
        }
    }
}
