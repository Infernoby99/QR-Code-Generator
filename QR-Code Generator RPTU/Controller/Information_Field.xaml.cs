using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using QR_Code_Generator_RPTU.Controller.ContextData;
using QR_Code_Generator_RPTU.Pages;
using QR_Code_Generator_RPTU.renderer;

namespace QR_Code_Generator_RPTU.Controller;

public partial class Information_Field : UserControl
{
    public Information_Field()
    {
        InitializeComponent();
        InfoTitle.Text = "Anatomie eines QR-Codes";
        AddInfo(0);
    }

    internal void AddInfo(int n)
    {
        switch (n)
        {
            case 0: 
                var introduction = new IntroductionInfo();
                ContextBorder.Child = introduction;
                break;
            case 1:
                var finders = new FindersPatternInfo();
                ContextBorder.Child = finders;
                break;
            case 2:
                var alignment = new AlignmentPatternInfo();
                ContextBorder.Child = alignment;
                break;
            case 3:
                var timing = new TimingPatternInfo();
                ContextBorder.Child = timing;
                break;
        }
    }
    private void Generate_OnClick(object sender, RoutedEventArgs e)
    {
        int numFunc = 0;
        if (sender is Button button && button.Tag is string tagString && int.TryParse(tagString, out int value))
        {
            numFunc = value;
        }
        
        QrCodeGenerator generate = new QrCodeGenerator(Canvas);
        generate.QrCodeField();
        Action[] actions =
        {
            generate.FindersPattern,
            generate.AlignmentPattern,
            generate.TimingPattern,
            generate.MessageBits,
        };

        switch (numFunc)
        {
            case 0: 
                InfoTitle.Text = "Anatomie eines QR-Codes"; 
                AddInfo(0);
                break;
            case 1: 
                InfoTitle.Text = "Finders Pattern"; 
                AddInfo(1);
                break;
            case 2: 
                InfoTitle.Text = "Alignment Pattern"; 
                AddInfo(2);
                break;
            case 3: 
                InfoTitle.Text = "Timing Pattern"; 
                AddInfo(3);
                break;
            case 4: InfoTitle.Text = "Nachrichten Bits"; break;
            case 5: InfoTitle.Text = "Masken Pattern"; break;
        }
        
        for (int i = 0; i < numFunc; i++)
        {
            if (i < 4) actions[i]();
            else generate.MaskPattern(8);
        }
    }

    private void ToHomePage_Onclick(object sender, RoutedEventArgs e)
    {
        NavigationService navigationService = NavigationService.GetNavigationService(this);
        if (navigationService != null)
        {
            navigationService.Navigate(new HomePage());
        }
    }
}