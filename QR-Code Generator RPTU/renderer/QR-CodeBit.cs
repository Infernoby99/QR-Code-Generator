using System.Windows.Shapes;

namespace QR_Code_Generator_RPTU.renderer;

public class QrCodeBit
{
    public Rectangle Bit { get; private set; }
    
    public QrCodeBit(double bitSize)
    {
        Bit = new Rectangle
        {
            Width = bitSize,
            Height =  bitSize,
        };
    }
}