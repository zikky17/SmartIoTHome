using CommunityToolkit.Mvvm.Input;
using SmartLight.ViewModels;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace SmartLight.Views
{
    /// <summary>
    /// Interaction logic for HomeView.xaml
    /// </summary>
    public partial class HomeView : UserControl
    {
        public HomeView()
        {
            InitializeComponent();
            StartAnimation();
        }

        public void StartAnimation()
        {
            BeginStoryboard storyboard = (BeginStoryboard)this.FindResource("glow-sb");
            storyboard.Storyboard.Begin();
        }
    }
}