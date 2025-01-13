using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using QR_Code_Generator_RPTU.renderer;
using static System.Windows.Media.Colors;
using Color = System.Drawing.Color;

namespace QR_Code_Generator_RPTU.Pages;

public partial class HomePage : Page
{
    public HomePage()
    {
        InitializeComponent();
        ControlLabel.StatusLabel.Content = "Generiere einen QR-Code -->";
    }
}