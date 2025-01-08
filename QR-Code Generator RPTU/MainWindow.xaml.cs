using System.Windows;
using System.Windows.Controls;
using QR_Code_Generator_RPTU.Data;
using QR_Code_Generator_RPTU.Pages;
using QR_Code_Generator_RPTU.renderer;

namespace QR_Code_Generator_RPTU;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow 
{
    public MainWindow()
    {
        InitializeComponent();
        MainFrame.Navigate(new HomePage());
    }

   
}