using System.CodeDom.Compiler;
using System.Windows.Controls;
using QR_Code_Generator_RPTU.renderer;

namespace QR_Code_Generator_RPTU.Pages;

public partial class Info : Page
{
    private QrCodeGenerator Generate;
    public Info()
    {
        InitializeComponent();
        ControlLabel.StatusLabel.Content = "Hier wird der QR-Code Generiert";
    }
}