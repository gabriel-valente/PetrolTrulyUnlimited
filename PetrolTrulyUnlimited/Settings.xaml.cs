using PetrolTrulyUnlimited.Entity;
using System;
using System.ComponentModel;
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
                "\u2022 3000ms - 5500ms, between 3s and 5.5s a new vehicle will be generated and added to the queue.\n" +
                "\u2022 If the Spawn Rate is too high and multiple vehicles can't find an available pump, increase the values.",

        "The \"Wait Time Before Service\" is for how long the vehicle will wait in the queue to be served.\n" +
                "If it takes too long to enter in the pump area the vehicle will leave without fueling.\n" +
                "This time is expressed in milliseconds.\n" +
                "\u2022 Default Value: 7000ms - 10000ms.\n" +
                "\u2022 5000ms - 13000ms, the new created vehicle will wait between 5s and 13s until he leaves the queue.\n" +
                "\u2022 If multiple vehicles are leaving without fueling increase the values so they can wait for more.",

        "The \"Maximum Queue Lenght\" is how many vehicles will wait in the queue.\n" +
                "If the queue is full, new vehicles won't be created and any statistics will be changed.\n" +
                "This value is expressed in whole vehicles.\n" +
                "\u2022 Default Value: 5 Vehicles.\n" +
                "\u2022 10 Vehicles means that the queue will be receiving new vehicles until there is 10 vehicles waiting.\n" +
                "\u2022 If some vehicles goes to the pump or leaves the queue, new vehicles will arrive after the time specified.",

        "The \"Maximum Fueling Time\" is the maximum time the vehicle can be in the pump fueling.\n" +
                "If the vehicle would take more than this time, now he will take just the maximum time possible.\n" +
                "This time is expressed in milliseconds.\n" +
                "\u2022 Default Value: 18000ms.\n" +
                "\u2022 20000ms means if the car would thame more than 20s now he will be just for 20s and will leave with a not totally filled tank.\n" +
                "\u2022 This value combined with the \"Pump Dispensing velocity\" manages how long the vehicles will need to stay in the pump and how many leave without a full tank.\n" +
                "\u2022 If many vehicles are leaving without a full tank increase this value.",

        "The \"Pump Dispensing velocity\" is how much the pump dispenses every second.\n" +
                "If this value is too low vehicles will need more time to fuel the same amount of fuel.\n" +
                "This time is expressed in millisseconds.\n" +
                "\u2022 Default Value: 1.5l/s.\n" +
                "\u2022 2l/s means every seconds the pump dispenses 2 litres of fuel.\n" +
                "\u2022 This value combined with the \"Maximum Fueling Time\" manages how long the vehicles will need to stay in the pump and how many leave without a full tank.\n" +
                "\u2022 If the vehicles takes too long to fuel increase this value.",

        "The \"Swapping Animation Duration\" is how long the fade in the images take.\n" +
                "This value is individual for the queue and pump.\n" +
                "This time is expressed in milliseconds.\n" +
                "\u2022 Default Value: 150ms.\n" +
                "\u2022 100ms means the vehicle will take 100ms to leave the queue and 100ms to enter the pump.\n" +
                "\u2022 This value can impact the performance of the pump since during this time the vehicle is not in the pump therefore is not fueling.\n" +
                "\u2022 If this animation is not important for the pump logic this value can be 0.\n" +
                "\u2022 Please notice if the value is too high the vehicles will take too long to leave and enter the pump."
        };

        public Settings()
        {
            InitializeComponent();

            AssignValues();
        }

        private void AssignValues()
        {
            //Assign values to the textboxes
            Txt_MinSpawnTime.Text = Global.MIN_SPAWN_TIME.ToString();
            Txt_MaxSpawnTime.Text = Global.MAX_SPAWN_TIME.ToString();
            Txt_MinServiceTime.Text = Global.MIN_SERVICE_TIME.ToString();
            Txt_MaxServiceTime.Text = Global.MAX_SERVICE_TIME.ToString();
            Txt_MaxQueueSize.Text = Global.MAX_QUEUE_SIZE.ToString();
            Txt_MaxFuelingTime.Text = Global.MAX_FUELLING_TIME.ToString();
            Txt_PumpVelocity.Text = Global.PUMP_VELOCITY.ToString();
            Txt_AnimationTime.Text = Global.ANIMATION_TIME.ToString();
        }

        private new void GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox label = (TextBox)sender;
            int index = Convert.ToInt32(label.Tag.ToString());

            Txb_Tips.Text = tips[index];
        }

        private void Btn_Default_Click(object sender, RoutedEventArgs e)
        {
            StreamReader sr = new StreamReader(Global.FILELOCATION_DEFAULTVALUES);

            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();

                string[] data = line.Split(new char[] { ':' });

                if (typeof(Global).GetField(data[0].Trim()).FieldType.ToString() != "System.Windows.Media.Animation.DoubleAnimation" && !typeof(Global).GetField(data[0].Trim()).IsLiteral)
                {
                    typeof(Global).GetField(data[0].Trim()).SetValue(this, Convert.ChangeType(data[1].Trim(), Type.GetType(data[2].Trim())));
                }
            }

            sr.Close();

            AssignValues();
        }

        private void Btn_Back_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).Frm_Main.GoBack(); //References the MainWindow frame and navigates to the previous page
        }

        private void Btn_Save_Click(object sender, RoutedEventArgs e)
        {
            //Assigns every value to the specific variable
            Global.MIN_SPAWN_TIME = int.Parse(Txt_MinSpawnTime.Text);
            Global.MAX_SPAWN_TIME = int.Parse(Txt_MaxSpawnTime.Text);
            Global.MIN_SERVICE_TIME = int.Parse(Txt_MinServiceTime.Text);
            Global.MAX_SERVICE_TIME = int.Parse(Txt_MaxServiceTime.Text);
            Global.MAX_QUEUE_SIZE = int.Parse(Txt_MaxQueueSize.Text);
            Global.MAX_FUELLING_TIME = int.Parse(Txt_MaxFuelingTime.Text);
            Global.PUMP_VELOCITY = float.Parse(Txt_PumpVelocity.Text);
            Global.ANIMATION_TIME = int.Parse(Txt_AnimationTime.Text);

            Global.SetAnimationTime();

            

            StreamWriter sw = new StreamWriter(Global.FILELOCATION_CURRENTVALUES, false);

            foreach (FieldInfo field in typeof(Global).GetFields()) //https://stackoverflow.com/questions/6536163/how-to-list-all-variables-of-class
            {
                if (field.FieldType.ToString() != "System.Windows.Media.Animation.DoubleAnimation" && !field.IsLiteral)
                {
                    sw.WriteLine(field.Name + " : " + field.GetValue(this) + " : " + field.FieldType);
                }
            }

            sw.Close();

            ((MainWindow)Application.Current.MainWindow).Frm_Main.GoBack();
        }

        private void Int_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Key input = e.Key;
            Key[] range = { 
                Key.D0, Key.D1, Key.D2, Key.D3, Key.D4, Key.D5, Key.D6, Key.D7, Key.D8, Key.D9, 
                Key.NumPad0, Key.NumPad1, Key.NumPad2, Key.NumPad3, Key.NumPad4, Key.NumPad5, Key.NumPad6, Key.NumPad7, Key.NumPad8, Key.NumPad9,
                Key.Delete, Key.Back,
                Key.Left, Key.Right
            };

            if (!range.Contains(input))
            {
                e.Handled = true;
            }
        }

        private void Float_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Key input = e.Key;
            Key[] range = {
                Key.D0, Key.D1, Key.D2, Key.D3, Key.D4, Key.D5, Key.D6, Key.D7, Key.D8, Key.D9,
                Key.NumPad0, Key.NumPad1, Key.NumPad2, Key.NumPad3, Key.NumPad4, Key.NumPad5, Key.NumPad6, Key.NumPad7, Key.NumPad8, Key.NumPad9,
                Key.OemPeriod, Key.OemComma,
                Key.Delete, Key.Back,
                Key.Left, Key.Right
            };

            if (!range.Contains(input))
            {
                e.Handled = true;
            }
            else if (input == Key.OemPeriod || input == Key.OemComma)
            {
                TextBox textBox = (TextBox)sender;
                int index = textBox.SelectionStart;

                if (!textBox.Text.Contains(","))
                {
                textBox.Text = textBox.Text.Insert(index, ",");

                textBox.SelectionStart = ++index;
                }

                e.Handled = true;
            }
        }
    }
}
