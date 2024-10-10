using SharedResources.Data;

namespace SmartHub
{
    public partial class App : Application
    {
        private readonly IDbContextMAUI _databaseContext;


        public App(IDbContextMAUI databaseContext)
        {
            InitializeComponent();
            _databaseContext = databaseContext;
            MainPage = new MainPage();
        }
    }
}
