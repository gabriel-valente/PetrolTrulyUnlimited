using System;
using System.Collections.Generic;
using System.Globalization;
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
        private static readonly PumpInformation bluePrintPumpInfo = new PumpInformation()
        {
            LitresDispensed = new[]
            {
                0f,
                0f,
                0f
            },
            AmountWon = new[]
            {
                0f,
                0f,
                0f
            },
            Commission = 0f,
            VehiclesCounter = 0,
            NotFullVehicle = 0,
            Receipt = new Receipt()
        };
        private List<Vehicle> vehiclesQueue = new List<Vehicle>(); //List of Queued vehicles
        private List<Vehicle> vehiclesPump = new List<Vehicle>(); //List of vehicles in the pump
        private List<Receipt> receipts = new List<Receipt>(); //List of receipts of the pumps

        private QueueInformation queueInformation = new QueueInformation(); //Variable of information in queue popup
        private PumpInformation[] pumpInformation = new PumpInformation[9]; //Array of information in pump popup

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
        }

        /// <summary>
        /// Spawns a vehicle and prepares the timer for the next one.
        /// </summary>
        /// <param name="_">object timer.</param>
        /// <param name="__">EventArgs timer.</param>
        private void Spawner(object _, EventArgs __)
        {
            int spawnInterval = rnd.Next(Global.MIN_SPAWN_TIME, Global.MAX_SPAWN_TIME); //Generates a random time
            
            CreateVehicle(); //Creates a new vehicle

            spawnTimer.Interval = spawnInterval; //Sets the time

            QueueLengthChecker(); //Checks id the timer should run
        }

        /// <summary>
        /// Creates a new random vehicle
        /// </summary>
        private void CreateVehicle()
        {
            //Creates an array with the possible vehicles
            Vehicle[] vehicles =
            {
                new Car(),
                new Van(),
                new Lorry()
            };

            Vehicle vehicle; //Single variable for the spawning vehicle

            Random random = new Random();

            byte iVehicle = (byte)random.Next(0, vehicles.Length); //Get Random type of vehicle
            float remaining = (float)(random.NextDouble() * (0.25 * vehicles[iVehicle].Capacity)); //Random remaining fuel remaining, below 1/4 of the tank
            byte iFuel = (byte)random.Next(0, vehicles[iVehicle].Fuel.Length); //Get type of fuel depending on what kind they accept
            int time = random.Next(Global.MIN_SERVICE_TIME, Global.MAX_SERVICE_TIME); //Set the time the vehicle will wait until he leaves
            Timer serviceTimer = new Timer(); //Creates a new timer for the vehicle to leave if is not in the pump
            int vehicleId = Vehicle.GetId(); //Gets this vehicle Id

            //Sets the timer
            serviceTimer.Interval = time;
            serviceTimer.Elapsed += (sender, args) => {
                Dispatcher.InvokeAsync(() => {
                    vehiclesQueue.Find(_ => _.Id == vehicleId).ServiceTimer.Stop(); //Stops the service timer
                    vehiclesQueue.RemoveAt(vehiclesQueue.FindIndex(_ => _.Id == vehicleId)); //Removes vehicle from Queue
                    QueueLengthChecker(); //Checks id the timer should run
                    UpdateVehicleImage(vehicleId, "Queue"); //Updates the image of the vehicle in the queue
                });
            };
            serviceTimer.Enabled = true;

            vehicle = Vehicle.SetVehicle(vehicleId, vehicles[iVehicle], remaining, vehicles[iVehicle].Fuel[iFuel], serviceTimer); //Creates the vehicles fully set up

            vehiclesQueue.Add(vehicle); //Adds it to the queue

            Dispatcher.InvokeAsync(() =>
            {
                AddVehicleImage(vehicle.Id, vehicle.Type, "Queue"); //Adds vehicle image in queue
            });

            AssignVehicleToPump(null, null); //Checks if any vehicle can go in a pump
        }

        /// <summary>
        /// Checks if the timer should run or not. And if the auto vehicle assigner should run.
        /// </summary>
        private void QueueLengthChecker()
        {
            if (vehiclesQueue.Count < 5)
            {
                spawnTimer.Start();
            }
            else
            {
                spawnTimer.Stop();
            }

            if (vehiclesQueue.Count > 0 && vehiclesQueue.Count <= 5)
            {
                checkTimer.Start();
            }
            else if (vehiclesQueue.Count == 0)
            {
                checkTimer.Stop();
            }
        }

        /// <summary>
        /// Creates the vehicle image in the UI.
        /// </summary>
        /// <param name="id">Id of the vehicle.</param>
        /// <param name="type">Type of vehicle to display.</param>
        /// <param name="place">If its in the Queue or Pump.</param>
        /// <param name="pump">Pump number, can be null.</param>
        private void AddVehicleImage(int id, string type, string place, int? pump = null)
        {
            string name = string.Format("Img_{0}_{1}", place, id); //Variable with the image element name

            //Create image variable
            Image image = new Image
            {
                Name = name,
                MaxWidth = 180,
                Margin = new Thickness(20, 5, 20, 5)
            };

            if (type == "Car")
            {
                image.Source = new BitmapImage(new Uri("Image/car.png", UriKind.Relative)); //Car image
            }
            else if (type == "Van")
            {
                image.Source = new BitmapImage(new Uri("Image/van.png", UriKind.Relative)); //Van image
            }
            else if (type == "Lorry")
            {
                image.Source = new BitmapImage(new Uri("Image/lorry.png", UriKind.Relative)); //Lorry image
            }

            if (place == "Queue")
            {
                Wrp_Queue.Children.Add(image); //Adds image to the queue

                queueInformation.TotalVehicles++; //Updates information about queue
            }
            else if (place == "Pump")
            {
                switch (pump) //Selects place to put image in pump
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

                Grd_Pumps.Children.Add(image); //Adds image to pump

                //Transition
                Storyboard.SetTarget(Global.fadeIn, image);
                Storyboard.SetTargetProperty(Global.fadeIn, new PropertyPath(OpacityProperty));

                var sb = new Storyboard();
                sb.Completed += (sender, args) => {
                    Fuelling((byte)++pump, id); //When finishes the fuelling process starts

                    sb.Stop();
                };
                sb.Children.Add(Global.fadeIn);

                sb.Begin();
            }

            QueuePopup(); //Updates queue information popup
        }

        /// <summary>
        /// Deletes the vehicle image in the UI.
        /// </summary>
        /// <param name="id">Id of the vehicle to delete.</param>
        /// <param name="place">If it is in the Queue or Pump.</param>
        /// <param name="type">Type of vehicle to add.</param>
        /// <param name="location">Location in the pump.</param>
        private void UpdateVehicleImage(int id, string place, string type = null, byte? location = null)
        {
            string name = string.Format("Img_{0}_{1}", place, id); //Variable with the image element name
            UIElement element;

            if (place == "Queue") //https://www.codeproject.com/Questions/190258/Add-and-Remove-controls-in-a-WPF
            {
                element = Wrp_Queue.Children.Cast<FrameworkElement>().First(_ => _.Name == name); //Finds the element

                //Setting up the animation
                Storyboard.SetTarget(Global.fadeOut, (Image)element);
                Storyboard.SetTargetProperty(Global.fadeOut, new PropertyPath(OpacityProperty));

                //Creating a storyboard
                var sb = new Storyboard();
                //When the animation ends it does some functions
                sb.Completed += (sender, args) => {
                    Wrp_Queue.Children.Remove(element); //Removes the image from the queue
                    Wrp_Queue.UpdateLayout();

                    //Checks if has to move the image to the pump
                    if (!string.IsNullOrEmpty(type) || location != null)
                    {
                        vehiclesPump.Find(_ => _.Id == id).ServiceTimer.Stop(); //Finds the vehicle in the list

                        queueInformation.VehiclesEntered++; //Updates the information in queue information

                        AddVehicleImage(id, type, "Pump", location); //Adds the vehicle to the pump
                    }
                    else
                    {
                        queueInformation.VehiclesRejected++; //Updates the information in queue information
                    }

                    QueuePopup();//Updates queue information popup

                    sb.Stop();
                };
                sb.Children.Add(Global.fadeOut);

                sb.Begin();

                vehiclesQueue.Find(_ => _.Id == id).ServiceTimer.Stop(); //Stops timer in vehicle
            }
            else if (place == "Pump")
            {
                element = Grd_Pumps.Children.Cast<FrameworkElement>().First(_ => _.Name == name); //Finds the element

                //Setting up the animation
                Storyboard.SetTarget(Global.fadeOut, (Image)element);
                Storyboard.SetTargetProperty(Global.fadeOut, new PropertyPath(OpacityProperty));
                
                //Creating a storyboard
                var sb = new Storyboard();
                //When the animation ends it does some functions
                sb.Completed += (sender, args) => {
                    Grd_Pumps.Children.Remove(element); //Removes the image from the pump
                    Grd_Pumps.UpdateLayout();

                    sb.Stop();
                };
                sb.Children.Add(Global.fadeOut);

                sb.Begin();
            }
        }

        /// <summary>
        /// Sends the car to the best available pump.
        /// </summary>
        /// <param name="_">object timer.</param>
        /// <param name="__">EventArgs timer.</param>
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

            QueueLengthChecker();
        }
        
        /// <summary>
        /// The process of fueling starts to the specific car.
        /// </summary>
        /// <param name="pumpId">ID of the pump the vehicle is in.</param>
        /// <param name="vehicleId">ID of the vehicle.</param>
        private void Fuelling(byte pumpId, int vehicleId)
        {
            Vehicle vehicle = vehiclesPump.Find(_ => _.Id == vehicleId); //Finds the vehicle that is fuelling
            Pump pump = Station.Pumps.First(_ => _.Id == pumpId); //Finds the pump the car is in
            float toFill = vehicle.Capacity - vehicle.Litres; //Calculates how much litres needs to fill
            float fuelingTime = (toFill / Global.PUMP_VELOCITY) * 1000; //Calculates how much time it takes

            if (fuelingTime > Global.MAX_FUELLING_TIME) //If the time is higher than the maximum time
            {
                fuelingTime = Global.MAX_FUELLING_TIME; //sets the time to the maximum time
                pumpInformation[pumpId - 1].NotFullVehicle++;
            }

            Receipt receipt = new Receipt() {
                VehicleType = vehicle.Type,
                PumpId = pump.Id,
                Litres = Global.PUMP_VELOCITY * (fuelingTime / 1000),
                Cost = (Global.PUMP_VELOCITY * (fuelingTime / 1000)) * vehicle.FuelType.Price
            };

            if (vehicle.FuelType.Type == "Diesel")
            {
                Diesel diesel = new Diesel();

                pumpInformation[pumpId - 1].LitresDispensed[Global.DIESEL_INDEX] += receipt.Litres;
                pumpInformation[pumpId - 1].AmountWon[Global.DIESEL_INDEX] += receipt.Litres * diesel.Price;
            }
            else if (vehicle.FuelType.Type == "Gasoline")
            {
                Gasoline gasoline = new Gasoline();

                pumpInformation[pumpId - 1].LitresDispensed[Global.GASOLINE_INDEX] += receipt.Litres;
                pumpInformation[pumpId - 1].AmountWon[Global.GASOLINE_INDEX] += receipt.Litres * gasoline.Price;
            }
            else if (vehicle.FuelType.Type == "LPG")
            {
                Lpg lpg = new Lpg();

                pumpInformation[pumpId - 1].LitresDispensed[Global.LPG_INDEX] += receipt.Litres;
                pumpInformation[pumpId - 1].AmountWon[Global.LPG_INDEX] += receipt.Litres * lpg.Price;
            }

            float totalWon = 0;

            foreach (float item in pumpInformation[pumpId - 1].AmountWon)
            {
                totalWon += item;
            }

            pumpInformation[pumpId - 1].Commission = totalWon * 0.01f;
            pumpInformation[pumpId - 1].VehiclesCounter++;
            pumpInformation[pumpId - 1].Receipt = receipt;

            //Sets up the fuelling timer
            pump.FuelingTimer.Interval = fuelingTime;
            pump.FuelingTimer.Elapsed += (sender, args) => {
                pump.Available = true;
                Dispatcher.InvokeAsync(() =>
                {
                    UpdateVehicleImage(vehicleId, "Pump"); //Updates the UI
                });
                pump.FuelingTimer.Close();
            };
            pump.FuelingTimer.AutoReset = false;
            pump.FuelingTimer.Enabled = true;

            Station.Pumps.First(_ => _.Id == pumpId).FuelingTimer = pump.FuelingTimer; //Assigns the created timer to the pump timer
            Station.Pumps.First(_ => _.Id == pumpId).FuelingTimer.Start(); //Starts the timer
        }

        //Popup when mouse goes over the Queue label
        private void Queue_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!Queue_Popup.IsOpen) //Checks if the timer is open
            {
                Queue_Popup.IsOpen = true; //Opens timer
                var mousePosition = e.GetPosition(Grd_QueueName); //Gets mouse position
                Queue_Popup.HorizontalOffset = mousePosition.X; //Sets the X axis
                Queue_Popup.VerticalOffset = mousePosition.Y; //Sets the Y axis

                QueuePopup(); //Updates the popup
            }
        }

        //Popup when mouse leaves the Queue label
        private void Queue_MouseLeave(object sender, MouseEventArgs e)
        {
            if (Queue_Popup.IsOpen) //Checks if the popup is open
            {
                Queue_Popup.IsOpen = false; //Closes popup
            }
        }

        //Popup when mouse goes over the Pump label
        private void Pump_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!Pump_Popup.IsOpen) //Checks if popup is open
            {
                Pump_Popup.IsOpen = true;

                Label label = (Label)sender;

                var mousePosition = e.GetPosition(Grd_Pumps); //Gets mouse position
                Pump_Popup.HorizontalOffset = mousePosition.X; //Sets the X axis
                Pump_Popup.VerticalOffset = mousePosition.Y; //Sets the Y axis

                PumpPopup(Convert.ToInt32(label.Tag.ToString())); //Updates the popup
                Console.WriteLine(Convert.ToInt32(label.Tag.ToString()));
            }
        }

        //Popup when mouse leaves the Pump label
        private void Pump_MouseLeave(object sender, MouseEventArgs e)
        {
            if (Pump_Popup.IsOpen) //Checks if the popup is open
            {
                Pump_Popup.IsOpen = false; //Closes popup
            }
        }

        /// <summary>
        /// Updates the information in the queue popup.
        /// </summary>
        private void QueuePopup()
        {
            if (Queue_Popup.IsOpen) //If popup is open
            {
                //Updates labels in the popup
                Lbl_Queue_TotalVehicles.Content = queueInformation.TotalVehicles;
                Lbl_Queue_QueueLenght.Content = vehiclesQueue.Count;
                Lbl_Queue_VehiclesEntered.Content = queueInformation.VehiclesEntered;
                Lbl_Queue_VehiclesRejected.Content = queueInformation.VehiclesRejected;

                if (queueInformation.VehiclesEntered + queueInformation.VehiclesRejected > 0) //Prevent division by 0 exception
                {
                    int acceptedVehicles = queueInformation.VehiclesEntered;
                    int rejectedVehicles = queueInformation.VehiclesRejected;

                    Lbl_Queue_PercentageAccepted.Content = string.Format(CultureInfo.InvariantCulture, "{0:##0.00}%", Math.Round((decimal)acceptedVehicles * 100 / (acceptedVehicles + rejectedVehicles), 2)); //Calculates percentage of accepted vehicles
                    Lbl_Queue_PercentageRejected.Content = string.Format(CultureInfo.InvariantCulture, "{0:##0.00}%", Math.Round((decimal)rejectedVehicles * 100 / (acceptedVehicles + rejectedVehicles), 2)); //Calculates percentage of rejected vehicles
                }
                else //To have a default value
                {
                    Lbl_Queue_PercentageAccepted.Content = "0.00%";
                    Lbl_Queue_PercentageRejected.Content = "0.00%";
                }
            }
        }

        /// <summary>
        /// Updates the information in the pump popup.
        /// </summary>
        /// <param name="pump">Position of the pump in array.</param>
        private void PumpPopup(int pump)
        {
            if (Pump_Popup.IsOpen) //If popup is open
            {
                float totalLitres = 0;
                float totalAmount = 0;

                foreach (float item in pumpInformation[pump].LitresDispensed)
                {
                    totalLitres += item;
                }

                foreach (float item in pumpInformation[pump].AmountWon)
                {
                    totalAmount += item;
                }

                //Updates labels in the popup
                Lbl_Pump_TotalLitres.Content = string.Format(CultureInfo.InvariantCulture, "{0:##0.00L}", totalLitres);
                Lbl_Pump_AmountWon.Content = string.Format(CultureInfo.InvariantCulture, "{0:##0.00£}", totalAmount);
                Lbl_Pump_Commission.Content = string.Format(CultureInfo.InvariantCulture, "{0:##0.00£}", pumpInformation[pump].Commission);
                Lbl_Pump_VehicleCounter.Content = pumpInformation[pump].VehiclesCounter;
                Lbl_Pump_NotFullVehicles.Content = pumpInformation[pump].NotFullVehicle;
                Lbl_Pump_VehicleType.Content = pumpInformation[pump].Receipt.VehicleType;
                Lbl_Pump_Litres.Content = string.Format(CultureInfo.InvariantCulture, "{0:##0.00L}", pumpInformation[pump].Receipt.Litres);
                Lbl_Pump_Cost.Content = string.Format(CultureInfo.InvariantCulture, "{0:##0.00£}", pumpInformation[pump].Receipt.Cost);
            }
        }

    }
}
