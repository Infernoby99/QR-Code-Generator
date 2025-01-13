using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using QR_Code_Generator_RPTU.renderer;

namespace QR_Code_Generator_RPTU.Controller;

public partial class Information_Field : UserControl
{
    public Information_Field()
    {
        InitializeComponent();
    }
    
    private void Home_OnClick(object sender, RoutedEventArgs e)
    {
        NavigationService navigationService = NavigationService.GetNavigationService(this);
        if (navigationService != null)
        {
            navigationService.Navigate(new Uri("Pages/HomePage.xaml", UriKind.Relative));
        }
    }
}