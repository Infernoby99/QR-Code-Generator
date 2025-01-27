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
    private  QrCodeGenerator generate { get; set; }
    public Information_Field()
    {
        InitializeComponent();
        InfoTitle.Text = "Anatomie eines QR-Codes";
        AddInfo(0);
    }

    private void AddInfo(int n)
    {
        switch (n)
        {
            case 0: ContextBorder.Child = new IntroductionInfo(); break;
            case 1: ContextBorder.Child = new FindersPatternInfo(); break;
            case 2: ContextBorder.Child = new AlignmentPatternInfo(); break;
            case 3: ContextBorder.Child = new TimingPatternInfo(); break;
            case 4: ContextBorder.Child = new MessageBits(); break;
            case 5: ContextBorder.Child = new MaskenPattern(); break;
        }
    }

    public void GenerateMask(int numFunc)
    {
        
        int mask = 8;
        
        
        generate = new QrCodeGenerator(Canvas, numFunc);
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
            case 4: 
                InfoTitle.Text = "Nachrichten Bits";
                AddInfo(4);
                break;
            case 5: 
                InfoTitle.Text = "Masken"; 
                AddInfo(5);
                break;
        }

        if (ContextBorder.Child is MaskenPattern maskenPattern)
        {
            maskenPattern.SetCanvas(Canvas);
        }
        
        for (int i = 0; i < numFunc; i++)
        {
            if (i < 4) actions[i]();
            else if (mask < 8) generate.MaskPattern(mask);
            else generate.MaskPattern(8);
        }
    }
    private void Generate_OnClick(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is string tagString && int.TryParse(tagString, out int value))
        {
            GenerateMask(value);
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