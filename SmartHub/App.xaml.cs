using SharedResources.Data;

namespace SmartHub
{
    public partial class App : Application
    {
        private readonly IDatabaseContext _databaseContext;


        public App(IDatabaseContext databaseContext)
        {
            InitializeComponent();
            _databaseContext = databaseContext;
            MainPage = new MainPage();
        }
    }
}
