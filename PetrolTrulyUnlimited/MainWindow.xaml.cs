using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
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

            if (!File.Exists(Global.FILELOCATION_DEFAULTVALUES))
            {
                DefaultFile();
            }

            if (File.Exists(Global.FILELOCATION_CURRENTVALUES))
            {
                GetValues();
            }

            if (!Directory.Exists(Global.DIRECTORY_RECEIPTS))
            {
                Directory.CreateDirectory(Global.DIRECTORY_RECEIPTS);
            }

            Frm_Main.Content = new Main();
        }

        private void DefaultFile()
        {
            if (!Directory.Exists(Global.DIRECTORY))
            {
                Directory.CreateDirectory(Global.DIRECTORY);
            }
            
            StreamWriter sw = new StreamWriter(Global.FILELOCATION_DEFAULTVALUES, false);

            foreach (FieldInfo field in typeof(Global).GetFields()) //https://stackoverflow.com/questions/6536163/how-to-list-all-variables-of-class
            {
                if (field.FieldType.ToString() != "System.Windows.Media.Animation.DoubleAnimation" && !field.IsLiteral)
                {
                    sw.WriteLine(field.Name + " : " + field.GetValue(this) + " : " + field.FieldType);
                }
            }

            sw.Close();
        }

        private void GetValues()
        {
            StreamReader sr = new StreamReader(Global.FILELOCATION_CURRENTVALUES);

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
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string receiptFile = string.Format("{0}\\{1}.csv", Global.DIRECTORY_RECEIPTS, string.Format("{0:yyy_MM_dd}", DateTime.Today));

            if (!File.Exists(receiptFile))
            {
                File.WriteAllText(receiptFile, "Vehicle Type,Pump Id,Litres,Cost\n");
            }

            StreamWriter sw = new StreamWriter(receiptFile, true, Encoding.Unicode);

            Console.WriteLine(Main.receipts.Count);

            Main.receipts.ForEach(item => {
                sw.WriteLine(string.Format("{0},{1},{2},{3}", item.Vehicle.Type, item.PumpId, string.Format(CultureInfo.InvariantCulture, "{0:##0.00L}", item.Litres), string.Format(CultureInfo.InvariantCulture, "{0:##0.00£}", item.Cost)));
            });

            sw.Close();
        }
    }
}
