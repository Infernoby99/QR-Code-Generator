using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using QR_Code_Generator_RPTU.renderer;
using static System.Windows.Media.Colors;
using Color = System.Drawing.Color;

namespace QR_Code_Generator_RPTU.Pages;

public partial class HomePage : Page
{
    public HomePage()
    {
        InitializeComponent();
        StatusLabel.Content = "Generiere einen QR-Code -->";
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
            StatusLabel.Content = "Versuchs Nochmal";
            StatusLabel.Foreground = new SolidColorBrush(Red);
            MessageBox.Show($"Text daten passen nicht! Wähle entweder eine höhere Version oder " +
                            $"niedrigeren Korrekturlevel.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        else
        {
            StatusLabel.Content = "Dein Generierter QR-Code";
            StatusLabel.Foreground = new SolidColorBrush(LimeGreen);
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
                Placeholder.Visibility = Visibility.Hidden;
            }
            else
            {
                Generator.IsEnabled = false;
                Placeholder.Visibility = Visibility.Visible;
            }
        }
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        NavigationService.Navigate(new Info());
    }
}