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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PetrolTrulyUnlimited.Entity;

namespace PetrolTrulyUnlimited
{
    public partial class Main : Page
    {
        private List<Vehicle> vehiclesQueue = new List<Vehicle>(); //List of Queued vehicles
        private List<Vehicle> vehiclesPump = new List<Vehicle>(); //List of vehicles in the pump

        private int spawnInterval; //Seconds for each spawn

        private Random rnd = new Random(); //Get random time
        private Timer spawnTimer = new Timer(); //Declaration of timer
        private Timer checkTimer = new Timer(); //Declaration of timer

        public Main()
        {
            InitializeComponent();

            //Spawn Timer Parameters
            spawnInterval = rnd.Next(Global.MIN_SPAWN_TIME, Global.MAX_SPAWN_TIME);
            spawnTimer.Interval = spawnInterval;
            spawnTimer.Elapsed += new ElapsedEventHandler(Spawner);
            spawnTimer.Enabled = true;
            spawnTimer.Start();

            //Spawn Timer Parameters
            checkTimer.Interval = 100;
            checkTimer.Elapsed += new ElapsedEventHandler(AssignVehicleToPump);
            checkTimer.Enabled = true;
            checkTimer.Start();
        }

        private void Spawner(object _, EventArgs __)
        {
            int spawnInterval = rnd.Next(Global.MIN_SPAWN_TIME, Global.MAX_SPAWN_TIME);
            
            CreateVehicle();

            spawnTimer.Interval = spawnInterval;
        }

        /// <summary>
        /// Creates a new random vehicle
        /// </summary>
        private void CreateVehicle()
        {
            Vehicle[] vehicles =
            {
                new Car(),
                new Van(),
                new Lorry()
            };

            Vehicle vehicle;

            Random random = new Random();

            byte iVehicle = (byte)random.Next(0, vehicles.Length); //Get Random type of vehicle
            float remaining = (float)(random.NextDouble() * (0.25 * vehicles[iVehicle].Capacity)); //Random remaining fuel remaining, below 1/4 of the tank
            byte iFuel = (byte)random.Next(0, vehicles[iVehicle].Fuel.Length); //Get type of fuel depending on what kind they accept

            vehicle = Vehicle.SetVehicle(vehicles[iVehicle], remaining, vehicles[iVehicle].Fuel[iFuel]);

            vehiclesQueue.Add(vehicle);

            Dispatcher.Invoke(() =>
            {
                AddVehicleImage(vehicle.Id, vehicle.Type, "Queue");
            });

            AssignVehicleToPump((object)null, (EventArgs)null);
        }

        /// <summary>
        /// Creates the vehicle image in the UI
        /// </summary>
        /// <param name="id">Id of the vehicle</param>
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
                image.Source = new BitmapImage(new Uri("Image/car.png", UriKind.Relative));
            }
            else if (type == "Van")
            {
                image.Source = new BitmapImage(new Uri("Image/van.png", UriKind.Relative));
            }
            else if (type == "Lorry")
            {
                image.Source = new BitmapImage(new Uri("Image/lorry.png", UriKind.Relative));
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
                Storyboard.SetTarget(Global.fadeIn, image);
                Storyboard.SetTargetProperty(Global.fadeIn, new PropertyPath(OpacityProperty));

                var sb = new Storyboard();
                sb.Completed += (sender, args) => {
                    Fueling((byte)++pump, id);

                    sb.Stop();
                };
                sb.Children.Add(Global.fadeIn);

                sb.Begin();
            }
        }

        /// <summary>
        /// Deletes the vehicle image in the UI
        /// </summary>
        /// <param name="id">Id of the vehicle to delete</param>
        /// <param name="place">If it is in the Queue or Pump</param>
        /// <param name="type">Type of vehicle to add</param>
        /// <param name="location">Location in the pump</param>
        private void UpdateVehicleImage(int id, string place, string type = null, byte? location = null)
        {
            string name = string.Format("Img_{0}_{1}", place, id);
            UIElement element;

            if (place == "Queue") //https://www.codeproject.com/Questions/190258/Add-and-Remove-controls-in-a-WPF
            {
                element = Wrp_Queue.Children.Cast<FrameworkElement>().First(_ => _.Name == name);

                //Setting up the animation
                Storyboard.SetTarget(Global.fadeOut, (Image)element);
                Storyboard.SetTargetProperty(Global.fadeOut, new PropertyPath(OpacityProperty));

                //Creating a storyboard
                var sb = new Storyboard();
                //When the animation ends it does some functions
                sb.Completed += (sender, args) => {
                    Wrp_Queue.Children.Remove(element);
                    Wrp_Queue.UpdateLayout();

                    AddVehicleImage(id, type, "Pump", location);

                    sb.Stop();
                };
                sb.Children.Add(Global.fadeOut);

                sb.Begin();        

            }
            else if (place == "Pump")
            {
                element = Grd_Pumps.Children.Cast<FrameworkElement>().First(_ => _.Name == name);

                Storyboard.SetTarget(Global.fadeOut, (Image)element);
                Storyboard.SetTargetProperty(Global.fadeOut, new PropertyPath(OpacityProperty));

                var sb = new Storyboard();
                //When the animation ends it does some functions
                sb.Completed += (sender, args) => {
                    Grd_Pumps.Children.Remove(element);
                    Grd_Pumps.UpdateLayout();

                    sb.Stop();
                };
                sb.Children.Add(Global.fadeOut);

                sb.Begin();
            }
        }

        /// <summary>
        /// Sends the car to the best available pump
        /// </summary>
        private void AssignVehicleToPump(object _, EventArgs __)
        {
            foreach (Vehicle vehicle in vehiclesQueue) //Get a pump for each vehicle in the line
            {
                byte bestAvailable = Global.LOWEST_PRIORITY_PUMP; //Set default minimum value
                bool foundPlace = false; //Validation that got a place in the pump

                //Search in every pump for an available spot with the highest priority
                for (int i = 0; i < Station.Pumps.Length; i++)
                {
                    if (Station.Pumps[i].Available) //Check is its available
                    {
                        foreach (Fuel fuel in Station.Pumps[i].Fuel)
                        {
                            if (vehicle.FuelType.Type == fuel.Type && Station.Pumps[i].Priority >= Station.Pumps[bestAvailable].Priority) //Check if it has the requested fuel type and if it is the highest priority available
                            {
                                //(i + 1) % 3 == 0 Check if it is the last pump in the line
                                //(i + 2) % 3 == 0 Check if it is the middle pump in the line
                                //(i + 3) % 3 == 0 Check if it is the first pump in the line

                                if ((i + 1) % 3 == 0 && Station.Pumps[i - 1].Available && Station.Pumps[i - 2].Available)
                                {
                                    bestAvailable = (byte)i;
                                    foundPlace = true;
                                }
                                else if ((i + 2) % 3 == 0 && Station.Pumps[i - 1].Available)
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
                    Station.Pumps[bestAvailable].Available = false; //Sets the current pump in to occupied

                    vehiclesPump.Add(vehiclesQueue.Find(_1 => _1.Id == vehicle.Id)); //Adds current Queue vehicle to Pump

                    vehiclesQueue.RemoveAt(vehiclesQueue.FindIndex(_1 => _1.Id == vehicle.Id)); //Removes vehicle from Queue

                    //Update UI
                    Dispatcher.InvokeAsync(() =>
                    {
                        UpdateVehicleImage(vehicle.Id, "Queue", vehicle.Type, bestAvailable);
                    });

                }
            }
        }
        
        private void Fueling(byte pumpId, int vehicleId)
        {
            Vehicle vehicle = vehiclesPump.Find(_ => _.Id == vehicleId);
            Pump pump = Station.Pumps.First(_ => _.Id == pumpId);
            float toFill = vehicle.Capacity - vehicle.Litres;
            float fuelingTime = (toFill / Global.PUMP_VELOCITY) * 1000;

            pump.FuelingTimer.Interval = fuelingTime;
            pump.FuelingTimer.Elapsed += (sender, args) => {
                pump.Available = true;
                Dispatcher.InvokeAsync(() =>
                {
                    UpdateVehicleImage(vehicleId, "Pump");
                });
                pump.FuelingTimer.Close();
            };
            pump.FuelingTimer.AutoReset = false;
            pump.FuelingTimer.Enabled = true;

            Station.Pumps.First(_ => _.Id == pumpId).FuelingTimer = pump.FuelingTimer;
            Station.Pumps.First(_ => _.Id == pumpId).FuelingTimer.Start();
        }
    }
}
