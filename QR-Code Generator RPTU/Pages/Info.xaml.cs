using System.CodeDom.Compiler;
using System.Windows.Controls;
using QR_Code_Generator_RPTU.renderer;

namespace QR_Code_Generator_RPTU.Pages;

public partial class Info : Page
{
    private QrCodeGenerator Generate;
    public Info( int level, int version, int mask, TextBox textBox)
    {
        InitializeComponent();
        ControlLabel.StatusLabel.Content = "Finders Pattern";
        Generate = new QrCodeGenerator(ControlLabel.Canvas, textBox);
        Generate.QrCodeField();
        Generate.FindersPattern();
    }
}