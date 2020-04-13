using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using PetrolTrulyUnlimited.Entity;

namespace PetrolTrulyUnlimited
{
    public partial class Main : Page
    {
        private List<Vehicle> vehiclesQueue = new List<Vehicle>(); //List of Queued vehicles
        private List<Vehicle> vehiclesPump = new List<Vehicle>(); //List of vehicles in the pump
        static public List<Receipt> receipts = new List<Receipt>(); //List of receipts of the pumps
        private List<Receipt> listReceipts = new List<Receipt>(); //List of receipts that is shown

        private QueueInformation queueInformation = new QueueInformation(); //Variable of information in queue popup
        static public PumpInformation[] pumpInformation = 
        {
            new PumpInformation(),
            new PumpInformation(),
            new PumpInformation(),
            new PumpInformation(),
            new PumpInformation(),
            new PumpInformation(),
            new PumpInformation(),
            new PumpInformation(),
            new PumpInformation()
        }; //Array of information in pump popup

        private int spawnInterval; //Seconds for each spawn

        private Random rnd = new Random(); //Get random time
        private Timer spawnTimer = new Timer(); //Declaration of timer
        private Timer checkTimer = new Timer(); //Declaration of timer

        string pumpPopupTag;
        string vehiclePopupTag;

        public Main()
        {
            InitializeComponent();

            Lst_Receipts.ItemsSource = listReceipts;

            //Spawn Timer Parameters
            spawnInterval = rnd.Next(Global.MIN_SPAWN_TIME, Global.MAX_SPAWN_TIME);
            spawnTimer.Interval = spawnInterval;
            spawnTimer.Elapsed += new ElapsedEventHandler(Spawner);
            spawnTimer.AutoReset = true;
            spawnTimer.Enabled = true;
            spawnTimer.Start();

            //Check Timer Parameters
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

            spawnTimer.Interval = spawnInterval; //Sets the time
            spawnTimer.Stop();
            CreateVehicle(); //Creates a new vehicle
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
                Dispatcher.Invoke(() =>
                {
                    vehiclesQueue.Find(_ => _.Id == vehicleId).ServiceTimer.Stop(); //Stops the service timer
                    vehiclesQueue.RemoveAt(vehiclesQueue.FindIndex(_ => _.Id == vehicleId)); //Removes vehicle from Queue
                    QueueLengthChecker(); //Checks id the timer should run
                    UpdateVehicleImage(vehicleId, "Queue"); //Updates the image of the vehicle in the queue
                });
            };
            serviceTimer.Enabled = true;

            vehicle = Vehicle.SetVehicle(vehicleId, vehicles[iVehicle], remaining, vehicles[iVehicle].Fuel[iFuel], serviceTimer); //Creates the vehicles fully set up

            vehiclesQueue.Add(vehicle); //Adds it to the queue

            Dispatcher.Invoke(() =>
            {
                AddVehicleImage(vehicle.Id, vehicle.Type, vehicle.FuelType.Type, "Queue"); //Adds vehicle image in queue
            });

            //AssignVehicleToPump(); //Checks if any vehicle can go in a pump
        }

        /// <summary>
        /// Checks if the timer should run or not. And if the auto vehicle assigner should run.
        /// </summary>
        private void QueueLengthChecker()
        {
            if (vehiclesQueue.Count < Global.MAX_QUEUE_SIZE)
            {
                spawnTimer.Start();
            }
            else
            {
                spawnTimer.Stop();
            }

            if (vehiclesQueue.Count > 0 && vehiclesQueue.Count <= Global.MAX_QUEUE_SIZE)
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
        private void AddVehicleImage(int id, string type, string fuel, string place, int? pump = null)
        {
            string name = string.Format("Img_{0}_{1}", place, id); //Variable with the image element name

            //Create image variable
            Image image = new Image
            {
                Name = name,
                MaxWidth = 180,
                Margin = new Thickness(20, 5, 20, 5),
                Tag = id
            };

            Panel.SetZIndex(image, 100);
            image.MouseEnter += new MouseEventHandler(Vehicle_MouseEnter);
            image.MouseLeave += new MouseEventHandler(Vehicle_MouseLeave);
            image.Source = new BitmapImage(new Uri(string.Format("Image/{0}_{1}.png", type, fuel), UriKind.Relative));

            if (place == "Queue")
            {
                Wrp_Queue.Children.Add(image); //Adds image to the queue

                queueInformation.TotalVehicles++; //Updates information about queue
            }
            else if (place == "Pump")
            {
                int x = (int)pump % 3;
                int y = (int)(pump / 3) * 2;

                Grid.SetColumn(image, x);
                Grid.SetRow(image, y);

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
        private void UpdateVehicleImage(int id, string place, string type = null, string fuel = null, byte? location = null)
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

                        AddVehicleImage(id, type, fuel, "Pump", location); //Adds the vehicle to the pump
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
        private void AssignVehicleToPump(object _ = null, EventArgs __ = null)
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
                    Dispatcher.Invoke(() =>
                    {
                        UpdateVehicleImage(vehicle.Id, "Queue", vehicle.Type, vehicle.FuelType.Type, bestAvailable);
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
            PumpInformation pumpTempInfo = pumpInformation[pumpId - 1];
            Vehicle vehicle = vehiclesPump.Find(_ => _.Id == vehicleId); //Finds the vehicle that is fuelling
            Pump pump = Station.Pumps.First(_ => _.Id == pumpId); //Finds the pump the car is in
            float toFill = vehicle.Capacity - vehicle.Litres; //Calculates how much litres needs to fill
            float fuelingTime = (toFill / Global.PUMP_VELOCITY) * 1000; //Calculates how much time it takes

            if (fuelingTime > Global.MAX_FUELLING_TIME) //If the time is higher than the maximum time
            {
                fuelingTime = Global.MAX_FUELLING_TIME; //sets the time to the maximum time
                pumpTempInfo.NotFullVehicle++;
            }

            //Creates a new receipt
            Receipt receipt = new Receipt() {
                Vehicle = vehicle,
                PumpId = pump.Id,
                Litres = Global.PUMP_VELOCITY * (fuelingTime / 1000),
                Fuel = vehicle.FuelType,
                Cost = (Global.PUMP_VELOCITY * (fuelingTime / 1000)) * vehicle.FuelType.Price,
                Time = fuelingTime
            };

            //Sets the temporary info for the pump
            if (vehicle.FuelType.Type == "Diesel")
            {
                pumpTempInfo.LitresDispensed[Global.DIESEL_INDEX] += receipt.Litres;
                pumpTempInfo.AmountWon[Global.DIESEL_INDEX] += receipt.Litres * new Diesel().Price;
                pumpTempInfo.VehiclesCounter[Global.DIESEL_INDEX]++;
            }
            else if (vehicle.FuelType.Type == "Gasoline")
            {
                pumpTempInfo.LitresDispensed[Global.GASOLINE_INDEX] += receipt.Litres;
                pumpTempInfo.AmountWon[Global.GASOLINE_INDEX] += receipt.Litres * new Gasoline().Price;
                pumpTempInfo.VehiclesCounter[Global.GASOLINE_INDEX]++;
            }
            else if (vehicle.FuelType.Type == "LPG")
            {
                pumpTempInfo.LitresDispensed[Global.LPG_INDEX] += receipt.Litres;
                pumpTempInfo.AmountWon[Global.LPG_INDEX] += receipt.Litres * new Lpg().Price;
                pumpTempInfo.VehiclesCounter[Global.LPG_INDEX]++;
            }

            float totalWon = 0;

            //Calculates the total amount won
            foreach (float item in pumpTempInfo.AmountWon)
            {
                totalWon += item;
            }

            //Calculates the comission and assigns the receipt
            pumpTempInfo.Commission = totalWon * 0.01f;
            pumpTempInfo.Receipt = receipt;

            pumpInformation[pumpId - 1] = pumpTempInfo; //Assigns the temp info to the pump

            PumpPopup(); //Updates the info if needed

            //Sets up the fuelling timer
            pump.FuelingTimer.Interval = fuelingTime;
            pump.FuelingTimer.Elapsed += (sender, args) => {
                pump.Available = true;
                Dispatcher.InvokeAsync(() =>
                {
                    UpdateVehicleImage(vehicleId, "Pump"); //Updates the UI

                    receipts.Insert(0, receipt); //Adds the last receipt to the begining of the "internal" receipt list
                    listReceipts.Insert(0, receipt); //Adds the last receipt to the begining of the "external" receipt list

                    //Checks if the external list has 200 items
                    if (listReceipts.Count > 200)
                    {
                        int amount = listReceipts.Count - 200; //Counts the amount of items to remove
                        listReceipts.RemoveRange(200, amount); //Removes from 200 and olders
                    }

                    UpdateReceipts(); //Updates the list

                    pumpInformation[pumpId - 1].Receipt = new Receipt();
                    PumpPopup();
                });
                pump.FuelingTimer.Close();
            };
            pump.FuelingTimer.AutoReset = false;
            pump.FuelingTimer.Enabled = true;

            Station.Pumps.First(_ => _.Id == pumpId).FuelingTimer = pump.FuelingTimer; //Assigns the created timer to the pump timer
            Station.Pumps.First(_ => _.Id == pumpId).FuelingTimer.Start(); //Starts the timer
        }

        /// <summary>
        /// Refreshes the UI of the receipts
        /// </summary>
        private void UpdateReceipts()
        {
            Lst_Receipts.Items.Refresh();
        }

        //Popup when mouse goes over the Queue label
        private void Queue_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!Queue_Popup.IsOpen) //Checks if the timer is open
            {
                Queue_Popup.IsOpen = true; //Opens timer
                Point mousePosition = Mouse.GetPosition(this); //Gets mouse position
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

                pumpPopupTag = ((Label)sender).Tag.ToString();

                Point mousePosition = Mouse.GetPosition(this); //Gets mouse position
                Pump_Popup.HorizontalOffset = mousePosition.X; //Sets the X axis
                Pump_Popup.VerticalOffset = mousePosition.Y; //Sets the Y axis

                PumpPopup(); //Updates the popup
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

        //Popup when mouse goes over the Vehicle image
        private void Vehicle_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!Vehicle_Popup.IsOpen) //Checks if popup is open
            {
                vehiclePopupTag = ((Image)sender).Tag.ToString();

                Vehicle_Popup.IsOpen = true; //Opens timer
                Point mousePosition = Mouse.GetPosition(this); //Gets mouse position
                Vehicle_Popup.HorizontalOffset = mousePosition.X; //Sets the X axis
                Vehicle_Popup.VerticalOffset = mousePosition.Y; //Sets the Y axis

                VehiclePopup(); //Updates the popup
            }
        }

        //Popup when mouse leaves the Vehicle image
        private void Vehicle_MouseLeave(object sender, MouseEventArgs e)
        {
            if (Vehicle_Popup.IsOpen) //Checks if the popup is open
            {
                Vehicle_Popup.IsOpen = false; //Closes popup
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
        private void PumpPopup()
        {
            if (Pump_Popup.IsOpen) //If popup is open
            {
                int pump = Convert.ToInt32(pumpPopupTag);
                int totalVehicles = 0;
                float totalLitres = 0;
                float totalAmount = 0;

                for (int i = 0; i < pumpInformation[pump].LitresDispensed.Length; i++)
                {
                    totalLitres += pumpInformation[pump].LitresDispensed[i];
                    totalAmount += pumpInformation[pump].AmountWon[i];
                    totalVehicles += pumpInformation[pump].VehiclesCounter[i];
                }

                //Updates labels in the popup
                Lbl_Pump_TotalLitres.Content = string.Format(CultureInfo.InvariantCulture, "{0:##0.00L}", totalLitres);
                Lbl_Pump_AmountWon.Content = string.Format(CultureInfo.InvariantCulture, "{0:##0.00£}", totalAmount);
                Lbl_Pump_Commission.Content = string.Format(CultureInfo.InvariantCulture, "{0:##0.00£}", pumpInformation[pump].Commission);
                Lbl_Pump_VehicleCounter.Content = totalVehicles;
                Lbl_Pump_NotFullVehicles.Content = pumpInformation[pump].NotFullVehicle;
                Lbl_Pump_VehicleType.Content = pumpInformation[pump].Receipt.Vehicle.Type;
                Lbl_Pump_Fuel.Content = pumpInformation[pump].Receipt.Fuel.Type.ToString();
                Lbl_Pump_Litres.Content = string.Format(CultureInfo.InvariantCulture, "{0:##0.00L}", pumpInformation[pump].Receipt.Litres);
                Lbl_Pump_Cost.Content = string.Format(CultureInfo.InvariantCulture, "{0:##0.00£}", pumpInformation[pump].Receipt.Cost);
            }
        }

        /// <summary>
        /// Updates the information in the vehicle popup.
        /// </summary>
        private void VehiclePopup()
        {
            if (Vehicle_Popup.IsOpen)
            {
                int id = Convert.ToInt32(vehiclePopupTag);
                Vehicle vehicle = new Vehicle();

                if (vehiclesQueue.Exists(_ => _.Id == id))
                {
                    vehicle = vehiclesQueue.Find(_ => _.Id == id);

                    Lbl_Vehicle_Time.Content = string.Format("{0} sec.", Math.Round(vehicle.ServiceTimer.Interval / 1000, 2));
                }
                else if (vehiclesPump.Exists(_ => _.Id == id))
                {
                    vehicle = vehiclesPump.Find(_ => _.Id == id);

                    float toFill = vehicle.Capacity - vehicle.Litres; //Calculates how much litres needs to fill
                    float fuelingTime = (toFill / Global.PUMP_VELOCITY); //Calculates how much time it takes

                    if (fuelingTime > (Global.MAX_FUELLING_TIME / 1000)) //If the time is higher than the maximum time
                    {
                        fuelingTime = (Global.MAX_FUELLING_TIME / 1000); //sets the time to the maximum time
                    }

                    Lbl_Vehicle_Time.Content = string.Format("{0} sec.", Math.Round(fuelingTime, 2));
                }
                else
                {
                    vehicle = new Vehicle();
                }

                Lbl_Vehicle_Type.Content = vehicle.Type;
                Lbl_Vehicle_Capacity.Content = string.Format(CultureInfo.InvariantCulture, "{0:##0.00L}", vehicle.Capacity);
                Lbl_Vehicle_Remaining.Content = string.Format(CultureInfo.InvariantCulture, "{0:##0.00L}", vehicle.Litres);
                Lbl_Vehicle_Fuel.Content = vehicle.FuelType.Type;
            }
        }

        /// <summary>
        /// Opens Settings page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Settings_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).Frm_Main.Content = new Settings(); //References the MainWindow frame and navigates to the settings page
        }
    }
}
