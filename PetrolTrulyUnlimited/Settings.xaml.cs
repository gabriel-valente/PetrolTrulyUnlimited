using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace PetrolTrulyUnlimited
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Page
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void Btn_Back_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).Frm_Main.Content = new Main();
        }

        private void Btn_Save_Click(object sender, RoutedEventArgs e)
        {
            Global.MIN_SPAWN_TIME = int.Parse(Txt_MinSpawnTime.Text);
            Global.MAX_SPAWN_TIME = int.Parse(Txt_MaxSpawnTime.Text);
            Global.MIN_SERVICE_TIME = int.Parse(Txt_MinServiceTime.Text);
            Global.MAX_SERVICE_TIME = int.Parse(Txt_MaxServiceTime.Text);
            Global.MAX_QUEUE_SIZE = int.Parse(Txt_MaxQueueSize.Text);
            Global.MAX_FUELLING_TIME = int.Parse(Txt_MaxFuelingTime.Text);
            Global.PUMP_VELOCITY = int.Parse(Txt_PumpVelocity.Text);
            Global.ANIMATION_TIME = int.Parse(Txt_AnimationTime.Text);

            ((MainWindow)Application.Current.MainWindow).Frm_Main.Content = new Main();
        }
    }
}
