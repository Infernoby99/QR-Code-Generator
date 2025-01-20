using System.Windows;

namespace QR_Code_Generator_RPTU;

public partial class DataBytes : Window
{
    public DataBytes()
    {
        InitializeComponent();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    public static void Show()
    {
        new DataBytes().ShowDialog();
        
    }
}