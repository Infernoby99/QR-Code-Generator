using System.Windows;
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
    }

    private void Generate_OnClick(object sender, RoutedEventArgs e)
    {
        QrCodeGenerator generate = new QrCodeGenerator(Canvas, Message);
        generate.QrCodeField();
        generate.FindersPattern();
        generate.AlignmentPattern();
        generate.TimingPattern();
        generate.MessageBits();
        generate.MaskPattern();
    }
}