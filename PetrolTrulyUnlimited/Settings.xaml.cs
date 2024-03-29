﻿using PetrolTrulyUnlimited.Entity;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PetrolTrulyUnlimited
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Page
    {
        //Tips for each value available in the settings
        private readonly string[] tips = {
        "The \"Vehicle Spawn Time\" is how often a new vehicle is created.\n" +
                "A new vehicle will appear between after some time that is between the minimum and the maximum time.\n" +
                "This time is expressed in milliseconds.\n" +
                "\u2022 Default Value: 1500ms - 2200ms.\n" +
                "\u2022 Minimum Value: 300ms - 300ms.\n" +
                "\u2022 3000ms - 5500ms, between 3s and 5.5s a new vehicle will be generated and added to the queue.\n" +
                "\u2022 If the Spawn Rate is too high and multiple vehicles can't find an available pump, increase the values.",

        "The \"Wait Time Before Service\" is for how long the vehicle will wait in the queue to be served.\n" +
                "If it takes too long to enter in the pump area the vehicle will leave without fueling.\n" +
                "This time is expressed in milliseconds.\n" +
                "\u2022 Default Value: 7000ms - 10000ms.\n" +
                "\u2022 Minimum Value: 300ms - 300ms.\n" +
                "\u2022 5000ms - 13000ms, the new created vehicle will wait between 5s and 13s until he leaves the queue.\n" +
                "\u2022 If multiple vehicles are leaving without fueling increase the values so they can wait for more.",

        "The \"Maximum Queue Lenght\" is how many vehicles will wait in the queue.\n" +
                "If the queue is full, new vehicles won't be created and any statistics will be changed.\n" +
                "This value is expressed in whole vehicles.\n" +
                "\u2022 Default Value: 5 Vehicles.\n" +
                "\u2022 Minimum Value: 1 Vehicle.\n" +
                "\u2022 10 Vehicles means that the queue will be receiving new vehicles until there is 10 vehicles waiting.\n" +
                "\u2022 If some vehicles goes to the pump or leaves the queue, new vehicles will arrive after the time specified.",

        "The \"Maximum Fueling Time\" is the maximum time the vehicle can be in the pump fueling.\n" +
                "If the vehicle would take more than this time, now he will take just the maximum time possible.\n" +
                "This time is expressed in milliseconds.\n" +
                "\u2022 Default Value: 18000ms.\n" +
                "\u2022 Minimum Value: 300ms.\n" +
                "\u2022 20000ms means if the car would thame more than 20s now he will be just for 20s and will leave with a not totally filled tank.\n" +
                "\u2022 This value combined with the \"Pump Dispensing velocity\" manages how long the vehicles will need to stay in the pump and how many leave without a full tank.\n" +
                "\u2022 If many vehicles are leaving without a full tank increase this value.",

        "The \"Pump Dispensing velocity\" is how much the pump dispenses every second.\n" +
                "If this value is too low vehicles will need more time to fuel the same amount of fuel.\n" +
                "This time is expressed in millisseconds.\n" +
                "\u2022 Default Value: 3.5l/s.\n" +
                "\u2022 Minimum Value: 0.1l/s.\n" +
                "\u2022 2l/s means every seconds the pump dispenses 2 litres of fuel.\n" +
                "\u2022 This value combined with the \"Maximum Fueling Time\" manages how long the vehicles will need to stay in the pump and how many leave without a full tank.\n" +
                "\u2022 If the vehicles takes too long to fuel increase this value.",

        "The \"Swapping Animation Duration\" is how long the fade in the images take.\n" +
                "This value is individual for the queue and pump.\n" +
                "This time is expressed in milliseconds.\n" +
                "\u2022 Default Value: 150ms.\n" +
                "\u2022 Minimum Value: 1ms.\n" +
                "\u2022 100ms means the vehicle will take 100ms to leave the queue and 100ms to enter the pump.\n" +
                "\u2022 This value can impact the performance of the pump since during this time the vehicle is not in the pump therefore is not fueling.\n" +
                "\u2022 If this animation is not important for the pump logic this value can be 1.\n" +
                "\u2022 Please notice if the value is too high the vehicles will take too long to leave and enter the pump."
        };
        //Minimum value for each variable available in the settings
        private readonly string[] minValues = {
            "300",
            "300",
            "1",
            "300",
            "0,1",
            "1"
        };

        public Settings()
        {
            InitializeComponent();

            AssignValues(); //Gets values from Global variables
            UpdateStats(); //Updates statistics
        }

        /// <summary>
        /// Assigns valus from Global file to labels.
        /// </summary>
        private void AssignValues()
        {
            //Reads the values from the global variables and assigns it to the TextBoxes
            Txt_MinSpawnTime.Text = Global.MIN_SPAWN_TIME.ToString();
            Txt_MaxSpawnTime.Text = Global.MAX_SPAWN_TIME.ToString();
            Txt_MinServiceTime.Text = Global.MIN_SERVICE_TIME.ToString();
            Txt_MaxServiceTime.Text = Global.MAX_SERVICE_TIME.ToString();
            Txt_MaxQueueSize.Text = Global.MAX_QUEUE_SIZE.ToString();
            Txt_MaxFuelingTime.Text = Global.MAX_FUELLING_TIME.ToString();
            Txt_PumpVelocity.Text = Global.PUMP_VELOCITY.ToString();
            Txt_AnimationTime.Text = Global.ANIMATION_TIME.ToString();
        }

        /// <summary>
        /// Updates statistics of each type.
        /// </summary>
        private void UpdateStats()
        {
            //Sets some variables with the running app information
            PumpInformation[] pumpInfo = Main.pumpInformation; //Gets and assigns each pump information
            Receipt[] receiptsInfo = Main.receipts.ToArray(); //Gets and assigns each receipt information
            //All fuels available (needs to be in the same order as it is assign everywhere else)
            Fuel[] fuels = new Fuel[] {
                new Diesel(),
                new Gasoline(),
                new Lpg()
            };
            //All vehicles available (needs to be in the same order as it is assign everywhere else)
            Vehicle[] vehicles =
            {
                new Car(),
                new Van(),
                new Lorry()
            };

            //Loads each type of data to display
            GetStats(pumpInfo, receiptsInfo, fuels, vehicles);
        }

        /// <summary>
        /// Gets avery statistic available to display.
        /// </summary>
        /// <param name="pumpInfo">Array with all data from each pump.</param>
        /// <param name="receiptInfo">Array with all receipts.</param>
        /// <param name="receiptInfo">Array with all types of fuels.</param>
        /// <param name="receiptInfo">Array with all types of vehicles.</param>
        private void GetStats(PumpInformation[] pumpInfo, Receipt[] receiptsInfo, Fuel[] fuels, Vehicle[] vehicles)
        {
            //Variables with data to display to the user
            float totalLitres = 0f;
            float averageLitres = 0f;
            float averageTime = 0f;
            float averageCarTime = 0f;
            float averageVanTime = 0f;
            float averageLorryTime = 0f;
            float amountWon = 0f;
            float commission = 0f;
            float sumCTime = 0f; //Sum car time
            float sumVTime = 0f; //Sum van time
            float sumLTime = 0f; //Sum lorry time
            float[] totalEachFuel = new float[] { 0f, 0f, 0f };
            float[] averageEachFuel = new float[] { 0f, 0f, 0f };
            float[,] fuelTime = new float[,] {
                { 0f, 0f, 0f },
                { 0f, 0f, 0f },
                { 0f, 0f, 0f }
            };

            int countVehicles = 0;
            int maxFuelIndex = 0;
            int minFuelIndex = 0;
            int mostCommonDiesel = 0;
            int mostCommonGasoline = 0;
            int mostCommonLpg = 0;
            int sumCCount = 0; //Sum car count
            int sumVCount = 0; //Sum van count
            int sumLCount = 0; //sum lorry count
            int[] countEachVehicles = new int[] { 0, 0, 0 };
            int[] pumpUsed = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            int[] mostCommonVehicle = new int[] { 0, 0 };
            //Each row is one type of vehicle and each collumn is one type of fuel
            int[,] fuelCount = new int[,] {
                { 0, 0, 0 },
                { 0, 0, 0 },
                { 0, 0, 0 }
            };

            //Goes through each pump information
            foreach (PumpInformation pump in pumpInfo)
            {
                //This try is to prevent errors if the user enters the settings page before any vehicle is in the pump
                try
                {
                    int vehicleIndex = Array.IndexOf(vehicles, vehicles.First(_ => _.Type == pump.Receipt.Vehicle.Type)); //Gets the vehicle index (from "vehicles")
                    int fuelIndex = Array.IndexOf(fuels, fuels.First(_ => _.Type == pump.Receipt.Fuel.Type)); //Gets the vehicle index (from "fuels")

                    fuelCount[vehicleIndex, fuelIndex]++; //Adds one to the counter of vehicle - fuel
                    fuelTime[vehicleIndex, fuelIndex] += pump.Receipt.Time; //Adds the time of vehicle - fuel

                    amountWon += pump.Receipt.Cost; //Adds the cost of the fuelling
                    commission += pump.Receipt.Cost * 0.01f; //Adds the amount of commission

                    //For each type of fuel in the pump
                    for (int i = 0; i < pump.LitresDispensed.Length; i++)
                    {
                        totalLitres += pump.LitresDispensed[i]; //Adds to the total amount of fuel dispensed
                        totalEachFuel[i] += pump.LitresDispensed[i]; //Adds to the fuel dispensed of that type

                        countVehicles += pump.VehiclesCounter[i]; //Adds one vehicle to the total vehicles served
                        countEachVehicles[i] += pump.VehiclesCounter[i]; //Adds one to the that type of vehicle served
                    }
                }
                catch (Exception)
                {
                }
            }

            averageLitres = totalLitres / countVehicles; //Calculates the Average litres dispensed

            //Goes in each type of fuel
            for (int i = 0; i < averageEachFuel.Length; i++)
            {
                //Checks if current type of fuel is used more than the other
                if (totalEachFuel[i] >= totalEachFuel[maxFuelIndex])
                {
                    maxFuelIndex = i; //Sets current type of fuel to be the most used
                }

                //Checks if current type of fuel is used less than the other
                if (totalEachFuel[i] <= totalEachFuel[minFuelIndex])
                {
                    minFuelIndex = i; //Sets current type of fuel to be the less used
                }

                averageEachFuel[i] = totalEachFuel[i] / countEachVehicles[i]; //Calculates the average for each type of fuel
            }

            //Goes through each receipt
            foreach (Receipt receipt in receiptsInfo)
            {
                //This try is to prevent errors if the user enters the settings page before any vehicle is finnished
                try
                {
                    //Pump index is one less than the pump id
                    pumpUsed[receipt.PumpId - 1]++; //Adds one to the specific pump

                    int vehicleIndex = Array.IndexOf(vehicles, vehicles.First(_ => _.Type == receipt.Vehicle.Type)); //Gets the vehicle index (from "vehicles")
                    int fuelIndex = Array.IndexOf(fuels, fuels.First(_ => _.Type == receipt.Fuel.Type)); //Gets the vehicle index (from "fuels")

                    fuelCount[vehicleIndex, fuelIndex]++; //Adds one to the counter of vehicle - fuel
                    fuelTime[vehicleIndex, fuelIndex] += receipt.Time; //Adds the time of vehicle - fuel

                    amountWon += receipt.Cost; //Adds the cost of the fuelling
                    commission += receipt.Cost * 0.01f; //Adds the amount of commission
                }
                catch (Exception)
                {
                }
            }

            //Goes in each index of the first dimension (vehicle)
            for (int i = 0; i < fuelCount.GetLength(0); i++)
            {
                //Goes in each index of the second dimension (fuel)
                for (int j = 0; j < fuelCount.GetLength(1); j++)
                {
                    //Checks if the count is higher to get the most common vehicle
                    if (fuelCount[i, j] > fuelCount[mostCommonVehicle[0], mostCommonVehicle[1]])
                    {
                        mostCommonVehicle = new int[] { i, j }; //Sets the index (x, y) of the most common vehicle
                    }
                }

                //Checks what is the most common diesel vehicle
                if (fuelCount[i, Global.DIESEL_INDEX] > fuelCount[mostCommonDiesel, Global.DIESEL_INDEX])
                {
                    mostCommonDiesel = i; //Sets the index of the most common diesel vehicle 
                }

                //Checks what is the most common gasoline vehicle
                if (fuelCount[i, Global.GASOLINE_INDEX] > fuelCount[mostCommonDiesel, Global.GASOLINE_INDEX])
                {
                    mostCommonGasoline = i; //Sets the index of the most common gasoline vehicle 
                }

                //Checks what is the most common LPG vehicle
                if (fuelCount[i, Global.LPG_INDEX] > fuelCount[mostCommonDiesel, Global.LPG_INDEX])
                {
                    mostCommonLpg = i; //Sets the index of the most common LPG vehicle 
                }
            }

            averageTime = fuelTime.Cast<float>().Sum() / fuelCount.Cast<int>().Sum(); //Calculates the average time of all vehicles

            //Goes through each time saved
            for (int i = 0; i < fuelTime.GetLength(0); i++)
            {
                sumCTime += fuelTime[0, i]; //Adds car fuelling time
                sumVTime += fuelTime[1, i]; //Adds van fuelling time
                sumLTime += fuelTime[2, i]; //Adds lorry fuelling time
                sumCCount += fuelCount[0, i]; //Adds car count
                sumVCount += fuelCount[1, i]; //Adds van count
                sumLCount += fuelCount[2, i]; //Adds lorry count
            }

            averageCarTime = sumCTime / sumCCount; //Calculates the car fueling time
            averageVanTime = sumVTime / sumVCount; //Calculates the van fueling time
            averageLorryTime = sumLTime / sumLCount; //Calculates the lorry fueling time

            //Assigns each data to the corresponding label
            Lbl_Stats_Pumps_Total.Content = string.Format(CultureInfo.InvariantCulture, "{0:###0.00L}", Math.Round(totalLitres, 2));
            Lbl_Stats_Pumps_TotalDiesel.Content = string.Format(CultureInfo.InvariantCulture, "{0:###0.00L}", Math.Round(totalEachFuel[Global.DIESEL_INDEX], 2));
            Lbl_Stats_Pumps_TotalGasoline.Content = string.Format(CultureInfo.InvariantCulture, "{0:###0.00L}", Math.Round(totalEachFuel[Global.GASOLINE_INDEX], 2));
            Lbl_Stats_Pumps_TotalLpg.Content = string.Format(CultureInfo.InvariantCulture, "{0:###0.00L}", Math.Round(totalEachFuel[Global.LPG_INDEX], 2));

            //This checks if the data is NaN or not, if it is NaN it displays 0 otherwise displays the corresponding data
            Lbl_Stats_Pumps_Average.Content = string.Format(CultureInfo.InvariantCulture, "{0:###0.00L}", !float.IsNaN(averageLitres) ? Math.Round(averageLitres, 2) : 0.0);
            Lbl_Stats_Pumps_AverageDiesel.Content = string.Format(CultureInfo.InvariantCulture, "{0:###0.00L}", !float.IsNaN(averageEachFuel[Global.DIESEL_INDEX]) ? Math.Round(averageEachFuel[Global.DIESEL_INDEX], 2) : 0.0);
            Lbl_Stats_Pumps_AverageGasoline.Content = string.Format(CultureInfo.InvariantCulture, "{0:###0.00L}", !float.IsNaN(averageEachFuel[Global.GASOLINE_INDEX]) ? Math.Round(averageEachFuel[Global.GASOLINE_INDEX], 2) : 0.0);
            Lbl_Stats_Pumps_AverageLpg.Content = string.Format(CultureInfo.InvariantCulture, "{0:###0.00L}", !float.IsNaN(averageEachFuel[Global.LPG_INDEX]) ? Math.Round(averageEachFuel[Global.LPG_INDEX], 2) : 0.0);

            Lbl_Stats_Pumps_MostUsedFuel.Content = fuels[maxFuelIndex].Type;
            Lbl_Stats_Pumps_LessUsedFuel.Content = fuels[minFuelIndex].Type;
            Lbl_Stats_Pumps_MostUsedPump.Content = string.Format("{0:Pump #}", Array.IndexOf(pumpUsed, pumpUsed.Max()) + 1); //Displays the id of the most used pump
            Lbl_Stats_Pumps_LessUsedPump.Content = string.Format("{0:Pump #}", Array.IndexOf(pumpUsed, pumpUsed.Min()) + 1); //Displays the id of the less used pump

            //Assigns each data to the corresponding label
            Lbl_Stats_Vehicle_MostCommonVehicle.Content = vehicles[mostCommonVehicle[0]].Type;
            Lbl_Stats_Vehicle_MostCommonDiesel.Content = vehicles[mostCommonDiesel].Type;
            Lbl_Stats_Vehicle_MostCommonGasoline.Content = vehicles[mostCommonGasoline].Type;
            Lbl_Stats_Vehicle_MostCommonLpg.Content = vehicles[mostCommonLpg].Type;
            //This checks if the data is NaN or not, if it is NaN it displays 0 otherwise displays the corresponding data
            Lbl_Stats_Vehicle_AverageTime.Content = string.Format("{0} sec.", !float.IsNaN(averageTime) ? Math.Round(averageTime / 1000, 2) : 0.0);
            Lbl_Stats_Vehicle_AverageTimeCar.Content = string.Format("{0} sec.", !float.IsNaN(averageCarTime) ? Math.Round(averageCarTime / 1000, 2) : 0.0);
            Lbl_Stats_Vehicle_AverageTimeVan.Content = string.Format("{0} sec.", !float.IsNaN(averageVanTime) ? Math.Round(averageVanTime / 1000, 2) : 0.0);
            Lbl_Stats_Vehicle_AverageTimeLorry.Content = string.Format("{0} sec.", !float.IsNaN(averageLorryTime) ? Math.Round(averageLorryTime / 1000, 2) : 0.0);

            //Assigns each data to the corresponding label
            Lbl_Stats_Finances_TotalWon.Content = string.Format(CultureInfo.InvariantCulture, "{0:##0.00£}", amountWon);
            Lbl_Stats_Finances_Commission.Content = string.Format(CultureInfo.InvariantCulture, "{0:##0.00£}", commission);
            Lbl_Stats_Finances_SalaryCommission.Content = string.Format(CultureInfo.InvariantCulture, "{0:##0.00£}", (2.49f * 8) + commission);
        }

        /// <summary>
        /// Refreshes the data on Statistics.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Refresh_Click(object sender, RoutedEventArgs e)
        {
            UpdateStats(); //Calls the function that will load again the stats.
        }

        /// <summary>
        /// Checks which of the textboxes is focused.
        /// </summary>
        /// <param name="sender">TextBox on focus.</param>
        /// <param name="e"></param>
        private new void GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textbox = (TextBox)sender; //Gets the Textbox on focus
            int index = Convert.ToInt32(textbox.Tag.ToString()); //Sets the variable with the tag on the textbox

            Txb_Tips.Text = tips[index]; //Displays the information that is on the array to the tips label
        }

        /// <summary>
        /// Checks if the value is above the minimum
        /// </summary>
        /// <param name="sender">TextBox that lost focus.</param>
        /// <param name="e"></param>
        private void Txt_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textbox = (TextBox)sender; //Gets the Textbox that lost the focus
            int index = Convert.ToInt32(textbox.Tag.ToString()); //Sets the variable with the tag on the textbox

            //Checks if the value writen is bigger than the minimum
            if (textbox.Text.Length == 0 || float.Parse(minValues[index]) > float.Parse(textbox.Text))
            {
                textbox.Text = minValues[index]; //If what is written is smaller than the minimum, the minumum will be set
            }
        }

        /// <summary>
        /// Sets the default values.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Default_Click(object sender, RoutedEventArgs e)
        {
            StreamReader sr = new StreamReader(Global.FILELOCATION_DEFAULTVALUES); //Opens the file writer on the default configuration file

            //Goes through each line of the file until reaches the end
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine(); //Assigns the line to a variable

                string[] data = line.Split(new char[] { ':' }); //Splits the line into the seperate values by the character that devides them

                //Checks if its not the animation variable
                if (typeof(Global).GetField(data[0].Trim()).FieldType.ToString() != "System.Windows.Media.Animation.DoubleAnimation" && !typeof(Global).GetField(data[0].Trim()).IsLiteral)
                {
                    typeof(Global).GetField(data[0].Trim()).SetValue(this, Convert.ChangeType(data[1].Trim(), Type.GetType(data[2].Trim()))); //Gets the variable from the Global file, and assigns the value with the correct data type
                }
            }

            sr.Close(); //Closes the file reader

            AssignValues(); //Assigns the values from the variables to the textboxe to edit
        }

        /// <summary>
        /// Goes to the previus page without saving.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Back_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).Frm_Main.GoBack(); //References the MainWindow frame and navigates to the previous page
        }

        /// <summary>
        /// Saves all the changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Save_Click(object sender, RoutedEventArgs e)
        {
            //Checks if the user wrote the longer time on the smaller time textbox
            if (int.Parse(Txt_MinSpawnTime.Text) > int.Parse(Txt_MaxSpawnTime.Text))
            {
                string temp = Txt_MaxSpawnTime.Text; //Saves the shorter time

                Txt_MaxSpawnTime.Text = Txt_MinSpawnTime.Text; //Swaps the values to the correct order
                Txt_MinSpawnTime.Text = temp; //Sets the shorter time
            }

            //Checks if the user wrote the longer time on the smaller time textbox
            if (int.Parse(Txt_MinServiceTime.Text) > int.Parse(Txt_MaxServiceTime.Text))
            {
                string temp = Txt_MaxServiceTime.Text; //Saves the shorter time

                Txt_MaxServiceTime.Text = Txt_MinServiceTime.Text; //Swaps the values to the correct order
                Txt_MinServiceTime.Text = temp; //Sets the shorter time
            }

            //Assigns every value to the specific variable
            Global.MIN_SPAWN_TIME = int.Parse(Txt_MinSpawnTime.Text);
            Global.MAX_SPAWN_TIME = int.Parse(Txt_MaxSpawnTime.Text);
            Global.MIN_SERVICE_TIME = int.Parse(Txt_MinServiceTime.Text);
            Global.MAX_SERVICE_TIME = int.Parse(Txt_MaxServiceTime.Text);
            Global.MAX_QUEUE_SIZE = int.Parse(Txt_MaxQueueSize.Text);
            Global.MAX_FUELLING_TIME = int.Parse(Txt_MaxFuelingTime.Text);
            Global.PUMP_VELOCITY = float.Parse(Txt_PumpVelocity.Text);
            Global.ANIMATION_TIME = int.Parse(Txt_AnimationTime.Text);

            Global.SetAnimationTime(); //Calls the function that sets the animation time

            StreamWriter sw = new StreamWriter(Global.FILELOCATION_CURRENTVALUES, false); //Opens the file writer on the current configuration file, with the append turned off

            //For each variable in the Global file
            foreach (FieldInfo field in typeof(Global).GetFields()) //https://stackoverflow.com/questions/6536163/how-to-list-all-variables-of-class
            {
                //Checks if its not the animation variable
                if (field.FieldType.ToString() != "System.Windows.Media.Animation.DoubleAnimation" && !field.IsLiteral)
                {
                    sw.WriteLine(field.Name + " : " + field.GetValue(this) + " : " + field.FieldType); //Writes a new line with the variable name, value, and type
                }
            }

            sw.Close(); //Closes the file writer

            ((MainWindow)Application.Current.MainWindow).Frm_Main.GoBack(); //References the MainWindow frame and navigates to the previous page
        }

        /// <summary>
        /// Opens a Windows File Explorer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Receipts_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Global.DIRECTORY_RECEIPTS); //Opens a Windows File Explorer window in the directory of the Receipts
        }

        /// <summary>
        /// For type of Int checks if only numeric keys are pressed.
        /// </summary>
        /// <param name="sender">TextBox in use.</param>
        /// <param name="e">Key arguments.</param>
        private void Int_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Key input = e.Key; //Key pressed
            //Possible key presses
            Key[] range = {
                Key.D0, Key.D1, Key.D2, Key.D3, Key.D4, Key.D5, Key.D6, Key.D7, Key.D8, Key.D9,
                Key.NumPad0, Key.NumPad1, Key.NumPad2, Key.NumPad3, Key.NumPad4, Key.NumPad5, Key.NumPad6, Key.NumPad7, Key.NumPad8, Key.NumPad9,
                Key.Delete, Key.Back,
                Key.Left, Key.Right
            };

            //Checks if the key pressed is in the range above
            if (!range.Contains(input))
            {
                e.Handled = true; //If the key is not in the range it will tell the that the key was already handled
            }
        }

        /// <summary>
        /// For type of Float checks if only numeric keys are pressed.
        /// </summary>
        /// <param name="sender">TextBox in use.</param>
        /// <param name="e">Key arguments.</param>
        private void Float_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Key input = e.Key; //Key pressed
            //Possible key presses
            Key[] range = {
                Key.D0, Key.D1, Key.D2, Key.D3, Key.D4, Key.D5, Key.D6, Key.D7, Key.D8, Key.D9,
                Key.NumPad0, Key.NumPad1, Key.NumPad2, Key.NumPad3, Key.NumPad4, Key.NumPad5, Key.NumPad6, Key.NumPad7, Key.NumPad8, Key.NumPad9,
                Key.OemPeriod, Key.OemComma,
                Key.Delete, Key.Back,
                Key.Left, Key.Right
            };

            //Checks if the key pressed is in the range above
            if (!range.Contains(input))
            {
                e.Handled = true; //If the key is not in the range it will tell the that the key was already handled
            }
            else if (input == Key.OemPeriod || input == Key.OemComma) //Checks if it's a comma or period.
            {
                TextBox textBox = (TextBox)sender; //Gets the current textbox
                int index = textBox.SelectionStart; //Gets the current index that the user is writting

                //Checks if the current data doesnt contain already a comma
                if (!textBox.Text.Contains(","))
                {
                    textBox.Text = textBox.Text.Insert(index, ","); //Adds a comma to the current index, its done like this because the user can type a period and its not valid

                    textBox.SelectionStart = ++index; //Increasses the selection start by one so the user can continue typing without any weirdness
                }

                e.Handled = true; //Tells that the key was already handled
            }
        }
    }
}