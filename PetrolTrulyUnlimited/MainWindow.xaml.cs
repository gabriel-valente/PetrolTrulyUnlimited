using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;

namespace PetrolTrulyUnlimited
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //Checks if default file exists
            if (!File.Exists(Global.FILELOCATION_DEFAULTVALUES))
            {
                DefaultFile(); //Goes to default file creating function
            }

            //Checks if current configuration file exists
            if (File.Exists(Global.FILELOCATION_CURRENTVALUES))
            {
                GetValues(); //Goes to current configuration file reader
            }

            //Checks if receipt directory exists
            if (!Directory.Exists(Global.DIRECTORY_RECEIPTS))
            {
                Directory.CreateDirectory(Global.DIRECTORY_RECEIPTS); //Creates directory if doesn't exist
            }

            Frm_Main.Content = new Main(); //Opens main program window on <Frame>
        }

        /// <summary>
        /// Creates default configuration file.
        /// </summary>
        private void DefaultFile()
        {
            //Checks if configuration directory exists
            if (!Directory.Exists(Global.DIRECTORY))
            {
                Directory.CreateDirectory(Global.DIRECTORY); //Creates configuration directory
            }
            
            StreamWriter sw = new StreamWriter(Global.FILELOCATION_DEFAULTVALUES, false); //Opens file writer on default values file with append off.

            //Gets every variable from global file
            foreach (FieldInfo field in typeof(Global).GetFields()) //https://stackoverflow.com/questions/6536163/how-to-list-all-variables-of-class
            {
                //Check if it isnt the animation variable
                if (field.FieldType.ToString() != "System.Windows.Media.Animation.DoubleAnimation" && !field.IsLiteral)
                {
                    sw.WriteLine(field.Name + " : " + field.GetValue(this) + " : " + field.FieldType); //Writes Variable name, value and data type
                }
            }

            sw.Close(); //Closes file writer
        }

        /// <summary>
        /// Gets values from current configuration file.
        /// </summary>
        private void GetValues()
        {
            StreamReader sr = new StreamReader(Global.FILELOCATION_CURRENTVALUES); //Opens file reader on curent configuration file

            //Goes through each line until reaches the end of the file
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine(); //Assigns current line to a variable

                string[] data = line.Split(new char[] { ':' }); //Splits the data into 3 different fields by the separator

                //Finds corresponding variable in global file and checks if it's not the animation variable
                if (typeof(Global).GetField(data[0].Trim()).FieldType.ToString() != "System.Windows.Media.Animation.DoubleAnimation" && !typeof(Global).GetField(data[0].Trim()).IsLiteral)
                {
                    typeof(Global).GetField(data[0].Trim()).SetValue(this, Convert.ChangeType(data[1].Trim(), Type.GetType(data[2].Trim()))); //Assigns value to the corresponding variable with the correct data type
                }
            }

            Global.SetAnimationTime(); //Calls the function that sets the animation time

            sr.Close(); //Closes file reader
        }

        /// <summary>
        /// When the program is closing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Variable for receipt in receipts directory with the current date
            string receiptFile = string.Format("{0}\\{1}.csv", Global.DIRECTORY_RECEIPTS, string.Format("{0:yyy_MM_dd}", DateTime.Today));

            //Checks if the file already exists
            if (!File.Exists(receiptFile))
            {
                File.WriteAllText(receiptFile, "Vehicle Type,Pump Id,Litres,Cost\n"); //Creates a .csv file with the header as it is writen
            }

            StreamWriter sw = new StreamWriter(receiptFile, true, Encoding.Unicode); //Opens file writer in the today's receipt file with appending on and the encoding set to unicode (to prevent erros with currency sign)

            Console.WriteLine(Main.receipts.Count);

            //For each item in receipts variable
            Main.receipts.ForEach(item => {
                //Writes each value as a .csv file should be writen
                sw.WriteLine(
                    string.Format(
                        "{0},{1},{2},{3}",
                        item.Vehicle.Type,
                        item.PumpId,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "{0:##0.00L}",
                            item.Litres),
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "{0:##0.00£}",
                            item.Cost)
                        )
                    );
            });

            sw.Close(); //Closes file writer
        }
    }
}
