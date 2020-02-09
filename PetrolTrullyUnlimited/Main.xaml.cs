using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PetrolTrullyUnlimited
{
    public partial class Main : Page
    {
        private List<VehicleState> vehiclesQueue = new List<VehicleState>(); //List of Queued vehicles
        private List<VehicleState> vehiclesPump = new List<VehicleState>(); //List of vehicles in the pump

        private int nextVehicleId = 0; //Identification of vehicle/counter
        private int spawnInterval; //Seconds for each spawn

        private Random rnd = new Random(); //Get random time
        private Timer spawnTimer = new Timer(); //Declaration of timer

        public Main()
        {
            InitializeComponent();

            //Timer Parameters
            spawnInterval = rnd.Next(Objects.MIN_SPAWN_TIME, Objects.MAX_SPAWN_TIME);
            spawnTimer.Interval = spawnInterval;
            spawnTimer.Elapsed += new ElapsedEventHandler(Spawner);
            spawnTimer.Enabled = true;
            spawnTimer.Start();
        }

        private void Spawner(object _, EventArgs __)
        {
            int spawnInterval = rnd.Next(Objects.MIN_SPAWN_TIME, Objects.MAX_SPAWN_TIME);

            CreateVehicle(nextVehicleId++);

            spawnTimer.Interval = spawnInterval;
        }

        /// <summary>
        /// Creates a new random vehicle
        /// </summary>
        /// <param name="id">Sequential id</param>
        /// <returns></returns>
        private void CreateVehicle(int id)
        {
            Vehicle[] vehicles =
            {
                Objects.car,
                Objects.van,
                Objects.lorry
            };

            VehicleState vehicle;

            Random random = new Random();

            byte iVehicle = (byte)random.Next(0, vehicles.Length); //Get Random type of vehicle
            byte iFuel = (byte)random.Next(0, vehicles[iVehicle].fuel.Length); //Get type of fuel depending on what kind they accept
            float remaining = (float)(random.NextDouble() * (0.25 * vehicles[iVehicle].capacity)); //Random remaining fuel remaining, below 1/4 of the tank


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
                AddVehicleImage(vehicle.id, vehicle.type, "Queue");
            });

            AssignVehicleToPump();
        }

        /// <summary>
        /// Creates the vehicle image in the UI
        /// </summary>
        /// <param name="type">Type of vehicle to display</param>
        /// <param name="place">If its in the Queue or Pump</param>
        /// <param name="pump">Pump number, can be null</param>
        private void AddVehicleImage(int id, string type, string place, int? pump = null)
        {
            string name = string.Format("Img_{0}_{1}", place, id);

            Image image = new Image
            {
                Name = name,
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

            if (place == "Queue")
            {
                Wrp_Queue.Children.Add(image);
            }
            else if (place == "Pump")
            {
                switch (pump)
                {
                    case 0:
                        Grid.SetRow(image, 0);
                        Grid.SetColumn(image, 0);
                        break;
                    case 1:
                        Grid.SetRow(image, 0);
                        Grid.SetColumn(image, 1);
                        break;
                    case 2:
                        Grid.SetRow(image, 0);
                        Grid.SetColumn(image, 2);
                        break;
                    case 3:
                        Grid.SetRow(image, 2);
                        Grid.SetColumn(image, 0);
                        break;
                    case 4:
                        Grid.SetRow(image, 2);
                        Grid.SetColumn(image, 1);
                        break;
                    case 5:
                        Grid.SetRow(image, 2);
                        Grid.SetColumn(image, 2);
                        break;
                    case 6:
                        Grid.SetRow(image, 4);
                        Grid.SetColumn(image, 0);
                        break;
                    case 7:
                        Grid.SetRow(image, 4);
                        Grid.SetColumn(image, 1);
                        break;
                    case 8:
                        Grid.SetRow(image, 4);
                        Grid.SetColumn(image, 2);
                        break;
                }
                
                Grd_Pumps.Children.Add(image);
            }
        }

        /// <summary>
        /// Deletes the vehicle image in the UI
        /// </summary>
        /// <param name="id">Id of the vehicle to delete</param>
        /// <param name="place">If its in the Queue or Pump</param>
        private void RemoveVehicleImage(int id, string place)
        {
            string name = string.Format("Img_{0}_{1}", place, id);
            UIElement element;
            if (place == "Queue") //https://www.codeproject.com/Questions/190258/Add-and-Remove-controls-in-a-WPF
            {
                element = Wrp_Queue.Children.Cast<FrameworkElement>().Where(_ => _.Name == name).First();


                Storyboard.SetTarget(Objects.fadeOut, (Image)element);
                Storyboard.SetTargetProperty(Objects.fadeOut, new PropertyPath(OpacityProperty));

                var sb = new Storyboard();
                sb.Children.Add(Objects.fadeOut);

                sb.Begin();

                Wrp_Queue.Children.Remove(element);
                Wrp_Queue.UpdateLayout();
            }
            else if (place == "Pump")
            {
                element = Grd_Pumps.Children.Cast<FrameworkElement>().Where(_ => _.Name == name).First();
                Grd_Pumps.Children.Remove(element);
                Grd_Pumps.UpdateLayout();
            }
        }

        /// <summary>
        /// Sends the car to the best available pump
        /// </summary>
        private void AssignVehicleToPump()
        {
            foreach (VehicleState vehicle in vehiclesQueue) //Get a pump for each vehicle in the line
            {
                byte bestAvailable = Objects.LOWEST_PRIORITY_PUMP; //Set default minimum value
                bool foundPlace = false; //Validation that got a place in the pump

                //Search in every pump for an available spot with the highest priority
                for (int i = 0; i < Objects.pumps.Length; i++)
                {
                    if (Objects.pumps[i].available) //Check is its available
                    {
                        foreach (Fuel fuel in Objects.pumps[i].fuel)
                        {
                            if (vehicle.fuel.type == fuel.type && Objects.pumps[i].priority >= Objects.pumps[bestAvailable].priority) //Check if it has the requested fuel type and if it is the highest priority available
                            {
                                //(i + 1) % 3 == 0 Check if it is the last pump in the line
                                //(i + 2) % 3 == 0 Check if it is the middle pump in the line
                                //(i + 3) % 3 == 0 Check if it is the first pump in the line
                                if ((i + 1) % 3 == 0 && Objects.pumps[i - 1].available && Objects.pumps[i - 2].available)
                                {
                                    bestAvailable = (byte)i;
                                    foundPlace = true;
                                }
                                else if ((i + 2) % 3 == 0 && Objects.pumps[i - 1].available)
                                {
                                    bestAvailable = (byte)i;
                                    foundPlace = true;
                                }
                                else if ((i + 3) % 3 == 0)
                                {
                                    bestAvailable = (byte)i;
                                    foundPlace = true;
                                }
                            }
                        }
                    }
                }

                if (foundPlace) //If found a place it will update the UI
                {
                    Objects.pumps[bestAvailable].available = false; //Sets the current pump in to occupied

                    vehiclesQueue.RemoveAt(vehiclesQueue.FindIndex(_ => _.id == vehicle.id)); //Removes vehicle from Queue
                    
                    vehiclesPump.Add(vehiclesQueue.Find(_ => _.id == vehicle.id)); //Adds current Queue vehicle to Pump 


                    //Update UI
                    Dispatcher.Invoke(() =>
                    {
                        RemoveVehicleImage(vehicle.id, "Queue");
                        AddVehicleImage(vehicle.id, vehicle.type, "Pump", bestAvailable);
                    });

                }
            }
        }
    }
}
