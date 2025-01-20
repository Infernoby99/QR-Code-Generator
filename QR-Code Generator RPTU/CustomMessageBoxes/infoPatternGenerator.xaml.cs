using System.Windows;

namespace QR_Code_Generator_RPTU.CustomMessageBoxes;

public partial class infoPatternGenerator : Window
{
    public infoPatternGenerator()
    {
        InitializeComponent();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    public static void Show()
    {
        new infoPatternGenerator().ShowDialog();
    }
}