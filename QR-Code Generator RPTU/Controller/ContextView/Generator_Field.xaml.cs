using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using QR_Code_Generator_RPTU.Pages;
using QR_Code_Generator_RPTU.renderer;
using static System.Windows.Media.Colors;

namespace QR_Code_Generator_RPTU.Controller;

public partial class Generator_Field : UserControl
{
    private QrCodeGenerator Generate { get; set; }

    private int level;
    private int version;
    private int mask;
    public Generator_Field()
    {
        InitializeComponent();
    }
    private void Message_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox tbox)
        {
            string content = tbox.Text;
            if (content.Length > 0)
            {
                Generator.IsEnabled = true;
                Placeholder.Visibility = Visibility.Hidden;
            }
            else
            {
                Generator.IsEnabled = false;
                Placeholder.Visibility = Visibility.Visible;
            }
        }
    }

    private void Generate_OnClick(object sender, RoutedEventArgs e)
    {
        level = ComboBox_CorrectionLevel.SelectedIndex;
        version = ComboBox_Version.SelectedIndex;
        mask = ComboBox_Mask.SelectedIndex;
        
        if(level == 4 && version == 6) Generate = new QrCodeGenerator(Canvas, Message);
        else Generate = new QrCodeGenerator(Canvas, Message, version, level);
        if(Generate.ErrorCheck())
        {
            StatusLabel.Content = "Versuchs Nochmal";
            StatusLabel.Foreground = new SolidColorBrush(Red);
            MessageBox.Show($"Text daten passen nicht! Wähle entweder eine höhere Version oder " +
                            $"niedrigeren Korrekturlevel.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        else
        {
            StatusLabel.Content = "Dein Generierter QR-Code";
            StatusLabel.Foreground = new SolidColorBrush(LimeGreen);
            Generate.QrCodeField();
            Generate.FindersPattern();
            Generate.AlignmentPattern();
            Generate.TimingPattern();
            Generate.MessageBits();
            Generate.MaskPattern(mask);
        }
    }

    
}