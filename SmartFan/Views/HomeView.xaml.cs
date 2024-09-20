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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SmartFan.Views
{
    /// <summary>
    /// Interaction logic for HomeView.xaml
    /// </summary>
    public partial class HomeView : UserControl
    {
        public HomeView()
        {
            InitializeComponent();
        }

        private bool IsActive = false;

        public void StartDeviceAnimation()
        {
            if (!IsActive)
            {
                var storyBoard = (BeginStoryboard)TryFindResource("rotate-sb");
                IsActive = true;
                storyBoard.Storyboard.Begin();
                Console.WriteLine("Animation started");
            }
        }

        public void StopDeviceAnimation()
        {
            if (IsActive)
            {
                var storyBoard = (BeginStoryboard)TryFindResource("rotate-sb");
                IsActive = false;
                storyBoard.Storyboard.Stop();
                Console.WriteLine("Animation stopped");
            }
        }




    }
}
