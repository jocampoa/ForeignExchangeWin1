namespace ForeignExchangeWin1.Infrastructure
{
    using ViewModels;
    public class IntanceLocator
    {
        public MainViewModel Main
        {
            get;
            set;
        }

        public IntanceLocator()
        {
            Main = new MainViewModel();
        }
    }
}
