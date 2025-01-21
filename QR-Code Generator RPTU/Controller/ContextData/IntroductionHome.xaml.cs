using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using QR_Code_Generator_RPTU.Pages;

namespace QR_Code_Generator_RPTU.Controller.ContextData;

public partial class IntroductionHome : UserControl
{
    public IntroductionHome()
    {
        InitializeComponent();
    }
    
    private void ButtonBase_OnClick(object sender, RoutedEventArgs routedEventArgs)
    {
        NavigationService navigationService = NavigationService.GetNavigationService(this);
        if (navigationService != null)
        {
            navigationService.Navigate(new Info());
        }
    }
}