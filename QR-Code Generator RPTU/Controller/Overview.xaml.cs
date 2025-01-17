using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using QR_Code_Generator_RPTU.Pages;

namespace QR_Code_Generator_RPTU.Controller;

public partial class Overview : UserControl
{
    public Overview()
    {
        InitializeComponent();
    }

    private void ToHomePage_Onclick(object sender, RoutedEventArgs e)
    {
        NavigationService navigationService = NavigationService.GetNavigationService(this);
        if (navigationService != null)
        {
            navigationService.Navigate(new HomePage());
        }
    }
}