namespace CSharp_Calc
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;  // Make sure the viewmodel is used
        }

    }

}
