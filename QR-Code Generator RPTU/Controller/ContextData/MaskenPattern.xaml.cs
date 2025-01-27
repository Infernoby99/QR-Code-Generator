using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using QR_Code_Generator_RPTU.renderer;
using Brushes = System.Windows.Media.Brushes;
using Rectangle = System.Windows.Shapes.Rectangle;
using QR_Code_Generator_RPTU.Controller.ContextData;
using QR_Code_Generator_RPTU.Pages;
using QR_Code_Generator_RPTU.renderer;

namespace QR_Code_Generator_RPTU.Controller.ContextData;

public partial class MaskenPattern : UserControl
{
    private Canvas QrCanvas { get; set; }

    public void SetCanvas(Canvas _canvas)
    {
        QrCanvas = _canvas;
    }
    public MaskenPattern()
    {
        InitializeComponent();
    }
    
    private void GenerateMask_OnClick(object sender, RoutedEventArgs e)
    {
        object[,] Maskfield = new object[8,8];
        double bitSize = Math.Min(Maskexample.ActualWidth, Maskexample.ActualHeight) / 8;
        
        Maskexample.Children.Clear();
        int mask = ComboBox_Mask.SelectedIndex;
        
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                QrCodeBit bit = new QrCodeBit(bitSize);
                Maskfield[i, j] = bit.Bit;

                Canvas.SetLeft(bit.Bit, j * bitSize);
                Canvas.SetTop(bit.Bit, i * bitSize);

                Rectangle? maskBit = Maskfield[i, j] as Rectangle;
                //maskBit.Fill = (i + j) % 2 == 0 ? Brushes.White : Brushes.Black;
                switch (mask)
                {
                    case 0: maskBit.Fill = (i + j) % 2 != 0 ? Brushes.White : Brushes.Black; break;
                    case 1: maskBit.Fill =  i % 2 != 0  ? Brushes.White : Brushes.Black; break;
                    case 2: maskBit.Fill =  j % 3 != 0 ? Brushes.White : Brushes.Black; break;
                    case 3: maskBit.Fill =  (i + j) % 3 != 0 ? Brushes.White : Brushes.Black; break;
                    case 4: maskBit.Fill =  ((i / 2) + (j / 3)) % 2 != 0 ? Brushes.White : Brushes.Black; break;
                    case 5: maskBit.Fill =  ((i * j) % 2) + ((i * j) % 3) != 0 ? Brushes.White : Brushes.Black; break;
                    case 6: maskBit.Fill =  (((i * j) % 3 + i * j) % 2 != 0) ? Brushes.White : Brushes.Black; break;
                    case 7: maskBit.Fill =  (((i * j) % 3 + i + j) % 2 != 0) ? Brushes.White : Brushes.Black; break;
                    case 8: maskBit.Fill =  i % 2 != 0  ? Brushes.White : Brushes.Black; break;
                }
                Maskexample.Children.Add(bit.Bit);
            }

            QrCodeGenerator generatorMask = new QrCodeGenerator(QrCanvas, 6);
            generatorMask.QrCodeField();
            generatorMask.FindersPattern();
            generatorMask.TimingPattern();
            generatorMask.AlignmentPattern();
            generatorMask.MessageBits();
            generatorMask.MaskPattern(mask);
        }
    }   
}