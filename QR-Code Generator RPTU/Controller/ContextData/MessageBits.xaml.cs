using System.Windows;
using System.Windows.Controls;
using QR_Code_Generator_RPTU.CustomMessageBoxes;

namespace QR_Code_Generator_RPTU.Controller.ContextData;

public partial class MessageBits : UserControl
{
    public MessageBits()
    {
        InitializeComponent();
    }

    private void MsgStructure_Onclick(object sender, RoutedEventArgs e)
    {
       DataBytes.Show();
    }

    private void PatternGenerator_Onclick(object sender, RoutedEventArgs e)
    {
        infoPatternGenerator.Show();
    }
}