using System.Windows;
using System.Windows.Controls;
using QR_Code_Generator_RPTU.Data;
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
        int level = ComboBox_CorrectionLevel.SelectedIndex;
        int version = ComboBox_Version.SelectedIndex;
        int mask = ComboBox_Mask.SelectedIndex;
        QrCodeGenerator generate; 
        if(level == 4 && version == 6) generate = new QrCodeGenerator(Canvas, Message);
        else generate = new QrCodeGenerator(Canvas, Message, version, level);
        if(generate.ErrorCheck())
        {
           
            MessageBox.Show($"Text daten passen nicht! Wähle entweder eine höhere Version oder " +
                            $"niedrigeren Korrekturlevel.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        else
        {
            generate.QrCodeField();
            generate.FindersPattern();
            generate.AlignmentPattern();
            generate.TimingPattern();
            generate.MessageBits();
            generate.MaskPattern(mask);
        }
    }

    private void Message_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox tbox)
        {
            string content = tbox.Text;
            if (content.Length > 0)
            {
                Generator.IsEnabled = true;
            }
            else Generator.IsEnabled = false;
        }
    }
}