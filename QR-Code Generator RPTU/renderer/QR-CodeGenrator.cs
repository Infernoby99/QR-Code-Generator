using System.Printing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using QR_Code_Generator_RPTU.Data;


namespace QR_Code_Generator_RPTU.renderer;

public class QrCodeGenerator
{
    private object[,] QrCode { get; set; }
    private int[,] FreeBit { get; set; }
    // FreeBit number Code: 
    // Free Bit         0
    // Finders bit      1
    // alignment bit    2
    // timing bit       3
    // format bit       4
    private bool[,] MaskField { get; set; }
    private string Message { get; set; }
    private int Version { get; set; }

    private bool error { get; set; }
    
    private int levelSelected { get; set; }

    private readonly QrCodeValues _getValues = new();
    
    private Canvas _canvas;

    private int? information;
    
    public QrCodeGenerator(Canvas canvas, int info)
    {
        _canvas = canvas;
        Message = "Hallo Welt, wie Gehts.";
        information = info;
        Version = _getValues.Version(Message.Length, 6, 5);
        
        QrCode = new object[Version, Version];
        FreeBit = new int[Version, Version];
        
        for (int i = 0; i < Version; i++)
        {
            for (int j = 0; j < Version; j++)
            {
                FreeBit[i, j] = 0;
            }
        }
        MaskField = new bool[Version, Version];
        error = false;
    }

    public QrCodeGenerator(Canvas canvas, TextBox context)
    {
        _canvas = canvas;
        Message = context.Text;
        
        Version = _getValues.Version(Message.Length, 6, 5);
        
        QrCode = new object[Version, Version];
        FreeBit = new int[Version, Version];
        
        for (int i = 0; i < Version; i++)
        {
            for (int j = 0; j < Version; j++)
            {
                FreeBit[i, j] = 0;
            }
        }
        MaskField = new bool[Version, Version];
        error = false;
    }
    
    public QrCodeGenerator(Canvas canvas, TextBox context, int selectedVersion, int selectedLevel)
    {
        _canvas = canvas;
        Message = context.Text;
        
        Version = _getValues.Version(Message.Length, selectedVersion, selectedLevel);
        if (Version == -1)
        {
            Console.WriteLine("XXXXXXXXERRORXXXXXXXXX");
            error = true;
        }
        else
        {
            QrCode = new object[Version, Version];
            FreeBit = new int[Version, Version];

            for (int i = 0; i < Version; i++)
            {
                for (int j = 0; j < Version; j++)
                {
                    FreeBit[i, j] = 0;
                }
            }

            MaskField = new bool[Version, Version];
            error = false;
        }
    }

    public void setNewCanvas(Canvas newCanvas)
    {
        _canvas = newCanvas;
    }

    public bool ErrorCheck()
    {
        return error;
    }

    public QrCodeValues getValues()
    {
        return _getValues;
    }
    public void QrCodeField()
    {
        double bitSize = Math.Min(_canvas.ActualWidth, _canvas.ActualHeight) / Version;

        _canvas.Children.Clear();
        for (int i = 0; i < Version; i++)
        {
            for (int j = 0; j < Version; j++)
            {
                QrCodeBit bit = new QrCodeBit(bitSize);
                QrCode[i, j] = bit.Bit;

                Canvas.SetLeft(bit.Bit, j * bitSize);
                Canvas.SetTop(bit.Bit, i * bitSize);
                _canvas.Children.Add(bit.Bit);
                 
                //set.bit.Fill = (i + j) % 2 == 0 ? Brushes.Black : Brushes.White;
            }
        } 
    }

    public void FindersPattern()
    {
        int posX;
        int posY;
        for (int x = 0; x < 3; x++)
        {
            posX = x == 1 ? Version - 7 : 0;
            posY = x == 2 ? Version - 7 : 0;
            for (int i = posX; i < posX + 7; i++)
            {
                for (int j = posY; j < posY + 7; j++)
                {
                    Rectangle? bit = QrCode[i, j] as Rectangle;
                    FreeBit[i, j] = 1;
                    if(posX == i || posX + 6 == i || posY ==  j || posY + 6 == j ||
                       (i > posX + 1 && i < posX + 5 && j > posY + 1 && j < posY + 5))
                    {
                        if (bit != null && information != 1) bit.Fill = Brushes.Black;
                        else if (bit != null && information == 1) bit.Fill = Brushes.DarkBlue;
                    }
                    else if (bit != null && information != 1) bit.Fill = Brushes.White;
                    else if (bit != null && information == 1) bit.Fill = Brushes.LightBlue;
                }
            }
            
            posX = x == 1 ? Version - 8 : 0;
            posY = x == 2 ? Version - 8 : 0;
            for (int i = 0; i < posX + 8; i++)
            {
                for (int j = 0; j < posY + 8; j++)
                {
                    if (FreeBit[i, j] == 0 && (i < 8 || i >= posX) && (j < 8 || j >= posY))
                    {
                        Rectangle? bit = QrCode[i, j] as Rectangle;
                        FreeBit[i, j] = 1;
                        bit.Fill = Brushes.White;
                    }
                }
            }
        }
    }
    
    public void AlignmentPattern()
    {
        int posX = Version - 9;
        int posY = Version - 9;
        if (Version > 21)
        {
            for (int i = posX; i < posX + 5; i++)
            {
                for (int j = posY; j < posY + 5; j++)
                {
                    Rectangle? bit = QrCode[i, j] as Rectangle;
                    if (j == posY || j == posY + 4 || i == posX || i == posX + 4 || (j == posY + 2 && i == posX + 2))
                    {
                        if (bit != null)
                        {
                            bit.Fill = information != 2 ? Brushes.Black: Brushes.DarkGreen;
                            FreeBit[i, j] = 2;
                        }
                    }
                    else if (bit != null)
                    {
                        bit.Fill = information != 2 ? Brushes.White: Brushes.LightGreen;
                        FreeBit[i, j] = 2;
                    }
                }
            }
        }
    }

    public void TimingPattern()
    {
        int start = 6;

        for (int i = 0; i < Version - 16; i++)
        {
            Rectangle? row = QrCode[start + 2 + i, start] as Rectangle;
            Rectangle? col = QrCode[start, start + 2 + i] as Rectangle;
            
            if(information == 3)
            {
                row.Fill = i % 2 == 0 ? Brushes.DarkRed : Brushes.Red;
                col.Fill = i % 2 == 0 ? Brushes.DarkRed : Brushes.Red;
            }
            else
            {
                row.Fill = i % 2 == 0 ? Brushes.Black : Brushes.White;
                col.Fill = i % 2 == 0 ? Brushes.Black : Brushes.White;
            }

            FreeBit[(start + 2) + i, start] = 3;
            FreeBit[start, (start + 2 + i)] = 3;
        }
    }
    
    public void MessageBits()
    {
        int col = Version - 1;
        int row = Version - 1;
        int turn = Version - 4;
        bool changeDirection = false;

        string messageByte = _getValues.MessageBytes(Message);

        for (int x = 0; x <= messageByte.Length; x++)
        {
            if (col < 0) col++;
            while (FreeBit[row, col] == 0 && x == messageByte.Length && row < Version - 8)
            {
                Rectangle? bit = QrCode[row, col] as Rectangle;
                bit.Fill = Brushes.White;
                if ((col % 2 == 1 && col > 5) || (col < 5 && col % 2 == 0))
                {
                    col++;
                    row++;
                }
                else col--;
            }

            if (FreeBit[row, col] == 0 && x < messageByte.Length)
            {
                Rectangle? bit = QrCode[row, col] as Rectangle;
                if(information != 4) bit.Fill = messageByte[x] == '0' ? Brushes.White : Brushes.Black;
                else if (information == 4)
                {
                    if(x < 4)bit.Fill = messageByte[x] == '0' ? Brushes.DodgerBlue : Brushes.MediumBlue;
                    else if(x < 12) bit.Fill = messageByte[x] == '0' ? Brushes.Lime : Brushes.DarkGreen;
                    else if(x < 22 * 8) bit.Fill = messageByte[x] == '0' ? Brushes.CornflowerBlue : Brushes.DarkSlateBlue;
                    else if(x < 22 * 8 + 4) bit.Fill = Brushes.DarkOrange;
                    else if(x < 27 * 8) bit.Fill = messageByte[x] == '0' ? Brushes.PaleGreen : Brushes.DarkOliveGreen;
                    else bit.Fill = messageByte[x] == '0' ? Brushes.Red : Brushes.DarkRed;
                }

                // Jump to end sector
                if (col == 9 && row == Version - 1)
                {
                    col--;
                    row -= 8;
                    turn -= 2;
                    changeDirection = !changeDirection;
                }
                // Jump over Timing Pattern
                else if (col == 7 && row == 9)
                {
                    col -= 2;
                    turn--;
                }
                else if (col % 2 == 1 && row == 7 && !changeDirection)
                {
                    col++;
                    row -= 2;
                }
                else if (col % 2 == 1 && row == 5 && changeDirection)
                {
                    col++;
                    row += 2;
                }
                // move around alignment Pattern
                else if (row < Version - 1 && row > 0 &&
                         col % 2 == 1 &&
                         FreeBit[col + 1, row - 1] == 2 &&
                         FreeBit[col + 1, row - 2] == 2 &&
                         FreeBit[col, row - 1] == 0)
                {
                    row--;
                }
                else if (row < Version - 1 && row > 0 && col % 2 == 1 && FreeBit[col + 1, row] == 2)
                {
                    if (FreeBit[col + 1, row - 1] == 0) col++;
                    row--;
                }

                // jump over alignment Pattern
                else if (row < Version - 1 && row > 0 && FreeBit[col, row + 1] == 2 && FreeBit[col + 1, row + 1] == 2 &&
                         changeDirection && col % 2 == 1)
                {
                    col++;
                    row += 6;
                }
                else if (row < Version - 1 && row > 0 && FreeBit[col, row - 1] == 2 && FreeBit[col + 1, row - 1] == 2 &&
                         !changeDirection && col % 2 == 1)
                {
                    col++;
                    row -= 6;
                }

                // turn at lower border 
                else if (col < Version - 10 && col > 8 && row == Version - 1 && col != turn && changeDirection) col--;
                else if (col < Version - 10 && col > 8 && row == Version - 1 && col == turn && changeDirection)
                {
                    changeDirection = !changeDirection;
                    col++;
                    row--;
                    turn -= 2;
                }
                else if (((col > Version - 10 && row == Version - 1) || (col < 8 && row == Version - 9)) &&
                         col != turn && changeDirection) col--;
                else if (((col >= Version - 10 && row == Version - 1) || (col < 8 && row == Version - 9)) &&
                         col == turn && changeDirection)
                {
                    changeDirection = !changeDirection;
                    col++;
                    row--;
                    turn -= 2;
                }

                // Turn at upper border 
                else if (col <= Version - 9 && col > 8 && row == 0 && col != turn && !changeDirection) col--;
                else if (col < Version - 9 && col > 8 && row == 0 && col == turn && !changeDirection)
                {
                    changeDirection = !changeDirection;
                    col++;
                    row++;
                    turn -= 2;
                }
                else if ((col > Version - 10 || col < 8) && row == 9 && col != turn && !changeDirection) col--;
                else if ((col >= Version - 10 || col < 8) && row == 9 && col == turn && !changeDirection)
                {
                    changeDirection = !changeDirection;
                    col++;
                    row++;
                    turn -= 2;
                }

                // Normal generate muster
                else if ((col % 2 == 1 && col > 5) || (col < 5 && col % 2 == 0))
                {
                    col++;
                    row = !changeDirection ? row - 1 : row + 1;
                }
                else col--;
            }
        }
        
    }
    
    public void MaskPattern(int selectedMask)
    {

        int mask = selectedMask < 8 ? selectedMask : CalculateMask();
        Console.WriteLine($"Chosen Mask Number:\t\t|==>\t{mask}");
        FormatPatterns(mask);
        
        for (int i = 0; i < Version; i++)
        {
            for (int j = 0; j < Version; j++)
            {
                switch (mask)
                {
                    case 0:
                        if ((i + j) % 2 == 0 && FreeBit[i, j] == 0) MaskField[i, j] = true;
                        else MaskField[i, j] = false;
                        break;
                    case 1:
                        if (i % 2 == 0 && FreeBit[i, j] == 0) MaskField[i, j] = true;
                        else MaskField[i, j] = false;
                        break;
                    case 2:
                        if (j % 3 == 0 && FreeBit[i, j] == 0) MaskField[i, j] = true;
                        else MaskField[i, j] = false;
                        break;
                    case 3:
                        if ((i + j) % 3 == 0 && FreeBit[i, j] == 0) MaskField[i, j] = true;
                        else MaskField[i, j] = false;
                        break;
                    case 4:
                        if (((i / 2) + (j / 3)) % 2 == 0 && FreeBit[i, j] == 0) MaskField[i, j] = true;
                        else MaskField[i, j] = false;
                        break;
                    case 5:
                        if (((i * j) % 2) + ((i * j) % 3) == 0 && FreeBit[i, j] == 0) MaskField[i, j] = true;
                        else MaskField[i, j] = false;
                        break;
                    case 6:
                        if (((i * j) % 3 + i * j) % 2 == 0 && FreeBit[i, j] == 0) MaskField[i, j] = true;
                        else MaskField[i, j] = false;
                        break;
                    case 7:
                        if (((i * j) % 3 + i + j) % 2 == 0 && FreeBit[i, j] == 0) MaskField[i, j] = true;
                        else MaskField[i, j] = false;
                        break;
                }

                Rectangle? rect = QrCode[i, j] as Rectangle;

                if (FreeBit[i, j] != 0) continue;

                if ((rect.Fill == Brushes.Black && MaskField[i, j]) ||
                    (rect.Fill == Brushes.White && !MaskField[i, j])) rect.Fill = Brushes.White;
                else rect.Fill = Brushes.Black;
            }
        }
    }

    private int CalculateMask()
    {
        int mask = 0;
        int[] points = new int[8];
        for (int x = 0; x < 8; x++)
        {
            for (int i = 0; i < Version; i++)
            {
                for (int j = 0; j < Version; j++)
                {
                    switch (x)
                    {
                        case 0:
                            if ((i + j) % 2 == 0 && FreeBit[i, j] == 0) MaskField[i, j] = true;
                            else MaskField[i, j] = false;
                            break;
                        case 1:
                            if (i % 2 == 0 && FreeBit[i, j] == 0) MaskField[i, j] = true;
                            else MaskField[i, j] = false;
                            break;
                        case 2:
                            if (j % 3 == 0 && FreeBit[i, j] == 0) MaskField[i, j] = true;
                            else MaskField[i, j] = false;
                            break;
                        case 3:
                            if ((i + j) % 3 == 0 && FreeBit[i, j] == 0) MaskField[i, j] = true;
                            else MaskField[i, j] = false;
                            break;
                        case 4:
                            if (((i / 2) + (j / 3)) % 2 == 0 && FreeBit[i, j] == 0) MaskField[i, j] = true;
                            else MaskField[i, j] = false;
                            break;
                        case 5:
                            if (((i * j) % 2) + ((i * j) % 3) == 0 && FreeBit[i, j] == 0) MaskField[i, j] = true;
                            else MaskField[i, j] = false;
                            break;
                        case 6:
                            if (((i * j) % 3 + i * j) % 2 == 0 && FreeBit[i, j] == 0) MaskField[i, j] = true;
                            else MaskField[i, j] = false;
                            break;
                        case 7:
                            if (((i * j) % 3 + i + j) % 2 == 0 && FreeBit[i, j] == 0) MaskField[i, j] = true;
                            else MaskField[i, j] = false;
                            break;
                    }
                }
            }
    
            points[x] = 0;
            points[x] += Criteria1();
            points[x] += Criteria2();
            points[x] += Criteria3();
            points[x] += Criteria4();
        }

        int low = 10000;

        for (int i = 0; i < 8; i++)
        {
            if (points[i] < low)
            {
                mask = i;
                low = points[i];
            }
        }
        return mask;
    }

    private void FormatPatterns(int mask)
    {
        try
        {
            string formatPattern = _getValues.FormatPattern(mask);
            Console.WriteLine(formatPattern);
            int x = 0;
            int y = 14;

            for (int i = 0; i < Version; i++)
            {
                Rectangle RowBits = QrCode[8, i] as Rectangle;
                Rectangle ColBits = QrCode[i, 8] as Rectangle;

                //  Row Pattern
                if (FreeBit[8, i] == 0 && (i < 8 || i > Version - 1 - 8) && x < 15)
                {
                    if(information == 5) RowBits.Fill = formatPattern[x] == '1' ? Brushes.DarkRed : Brushes.Red;
                    else RowBits.Fill = formatPattern[x] == '1' ? Brushes.Black : Brushes.White;
                    FreeBit[8, i] = 4;
                    x++;
                }
                //  Column Pattern
                if (FreeBit[i, 8] == 0 && (i < 9 || i > Version - 1 - 7) && y >= 0)
                {
                    if(information == 5) ColBits.Fill = formatPattern[y] == '1' ? Brushes.DarkBlue : Brushes.Blue;
                    else ColBits.Fill = formatPattern[y] == '1' ? Brushes.Black : Brushes.White;
                    FreeBit[i, 8] = 4;
                    y--;
                }
            }

            Rectangle rect = QrCode[Version - 1 - 7, 8] as Rectangle;
            FreeBit[Version - 1 - 7, 8] = 4;
            rect.Fill = information == 5 ? Brushes.Green : Brushes.Black;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Fehler: {ex.Message}  \n Methode: {ex.TargetSite} " +
                            $"\nSource: {ex.Source}\n Stacktrace: {ex.StackTrace}");
            throw;
        }
    }
    
    private int Criteria1()
    {
        int points = 0;
        bool[] HorBlackline = new bool[Version];
        bool[] VerBlackline = new bool[Version];
        bool[] HorWhiteline = new bool[Version];
        bool[] VerWhiteline = new bool[Version];
        for (int i = 0; i < Version; i++)
        {
            for (int j = 0; j < Version; j++)
            {
                Rectangle rect = QrCode[i, j] as Rectangle;
                if (FreeBit[i, j] != 0)
                {
                    HorBlackline[j] = false;
                    HorWhiteline[j] = false;
                    continue;
                }

                ;

                if ((rect.Fill == Brushes.Black && MaskField[i, j]) ||
                    (rect.Fill != Brushes.Black && !MaskField[i, j]))
                {
                    HorWhiteline[j] = true;
                    HorBlackline[j] = false;
                }
                else
                {
                    HorBlackline[j] = true;
                    HorWhiteline[j] = false;
                }
            }

            points += CheckLine(HorBlackline);
            points += CheckLine(HorWhiteline);
        }

        for (int i = 0; i < Version; i++)
        {
            for (int j = 0; j < Version; j++)
            {
                Rectangle rect = QrCode[j, i] as Rectangle;
                if (FreeBit[j, i] != 0)
                {
                    VerWhiteline[j] = false;
                    VerBlackline[j] = false;
                    continue;
                }

                ;

                if ((rect.Fill == Brushes.Black && MaskField[i, j]) ||
                    (rect.Fill != Brushes.Black && !MaskField[i, j]))
                {
                    VerWhiteline[j] = true;
                    VerBlackline[j] = false;
                }
                else
                {
                    VerWhiteline[j] = false;
                    VerBlackline[j] = true;
                }
            }

            points += CheckLine(VerWhiteline);
            points += CheckLine(VerBlackline);
        }

        return points;
    }

    private int CheckLine(bool[] line)
    {
        int points = 0;
        int count = 0;
        for (int i = 0; i < Version; i++)
        {
            while (i < Version && line[i])
            {
                count++;
                i++;
            }

            if (count >= 5) points += (count - 5) + 3;
            count = 0;
        }

        return points;
    }

    private int Criteria2()
    {
        int points = 0;
        bool[] clear = new bool[Version];
        for (int i = 0; i < Version - 1; i++)
        {
            for (int j = 0; j < Version - 1; j++)
            {
                Rectangle rect1 = QrCode[i, j] as Rectangle;
                Rectangle rect2 = QrCode[i, j + 1] as Rectangle;
                Rectangle rect3 = QrCode[i + 1, j] as Rectangle;
                Rectangle rect4 = QrCode[i + 1, j + 1] as Rectangle;

                if (FreeBit[i, j] != 0) continue;

                if (j <= Version
                    && i <= Version
                    && (MaskField[i, j] && rect1.Fill == Brushes.Black)
                    && (MaskField[i, j + 1] && rect2.Fill == Brushes.Black)
                    && (MaskField[i + 1, j] && rect3.Fill == Brushes.Black)
                    && (MaskField[i + 1, j + 1] && rect4.Fill == Brushes.Black)
                    //&& !clear[j] 
                    //&& !clear[j + 1]
                   )
                {
                    //clear[j] = true;
                    //clear[j + 1] = true;
                    points += 3;
                    j += 2;
                }
                else if (j <= Version
                         && i <= Version
                         && (!MaskField[i, j] && rect1.Fill == Brushes.White)
                         && (!MaskField[i, j + 1] && rect1.Fill == Brushes.White)
                         && (!MaskField[i + 1, j] && rect1.Fill == Brushes.White)
                         && (!MaskField[i + 1, j + 1] && rect1.Fill == Brushes.White)
                         //&& !clear[j] 
                         //&& !clear[j + 1]
                        )
                {
                    //clear[j] = true;
                    //clear[j + 1] = true;
                    points += 3;
                    j += 2;
                }
                else if (j <= Version
                         && i <= Version
                         && (!MaskField[i, j] && rect1.Fill == Brushes.Black)
                         && (!MaskField[i, j + 1] && rect2.Fill == Brushes.Black)
                         && (!MaskField[i + 1, j] && rect3.Fill == Brushes.Black)
                         && (!MaskField[i + 1, j + 1] && rect4.Fill == Brushes.Black)
                         //&& !clear[j]
                         //&& !clear[j + 1]
                        )
                {
                    //clear[j] = true;
                    //clear[j + 1] = true;
                    points += 3;
                    j += 2;
                }
                else if (j <= Version
                         && i <= Version
                         && (MaskField[i, j] && rect1.Fill == Brushes.White)
                         && (MaskField[i, j + 1] && rect1.Fill == Brushes.White)
                         && (MaskField[i + 1, j] && rect1.Fill == Brushes.White)
                         && (MaskField[i + 1, j + 1] && rect1.Fill == Brushes.White)
                         //&& !clear[j]
                         //&& !clear[j + 1]
                        )
                {
                    //clear[j] = true;
                    //clear[j + 1] = true;
                    points += 3;
                    j += 2;
                }
                else
                {
                    clear[j] = false;
                }
            }
        }

        return points;
    }

    private int Criteria3()
    {
        int points = 0;
        int Pattern = 1011101;
        int[] horCheckpattern = new int[7];
        int[] verCheckpattern = new int[7];

        for (int i = 0; i < Version; i++)
        {
            for (int j = 0; j < Version; j++)
            {
                Rectangle rect = QrCode[i, j] as Rectangle;
                if (FreeBit[i, j] == 0)
                {
                    int x = 0;
                    while ((x <= Version && x < 7 && j < Version - 6) && FreeBit[i, j + x] == 0)
                    {
                        if ((rect.Fill == Brushes.Black && MaskField[i, j + x])
                            || (rect.Fill == Brushes.White && !MaskField[i, j + x]))
                        {
                            horCheckpattern[x] = 1;
                        }
                        else
                        {
                            horCheckpattern[x] = 0;
                        }

                        x++;
                    }

                    while ((x <= 20 && x < 7 && i < 15) && FreeBit[i + x, j] == 0)
                    {
                        if ((rect.Fill == Brushes.Black && MaskField[i + x, j])
                            || (rect.Fill == Brushes.White && !MaskField[i + x, j]))
                        {
                            verCheckpattern[x] = 1;
                        }
                        else
                        {
                            verCheckpattern[x] = 0;
                        }

                        x++;
                    }

                    points += CheckPattern(horCheckpattern, Pattern);
                    points += CheckPattern(verCheckpattern, Pattern);
                }
            }
        }

        return points;
    }

    private int CheckPattern(int[] pattern, int Pattern)
    {
        int points = 0;
        int count = 0;

        for (int i = 0; i < pattern.Length; i++)
        {
            if (Pattern % 10 == pattern[i]) count++;
            Pattern /= 10;
        }

        if (count == 7) points = 40;
        return points;
    }

    private int Criteria4()
    {
        int points;
        double blackCount = 0;

        for (int i = 0; i < Version; i++)
        {
            for (int j = 0; j < Version; j++)
            {
                if (FreeBit[i, j] == 0)
                {
                    Rectangle rect = QrCode[i, j] as Rectangle;

                    if ((rect.Fill == Brushes.Black && !MaskField[i, j]) ||
                        (rect.Fill == Brushes.White && MaskField[i, j])) blackCount++;
                }
            }
        }

        double anzFreeBits = 0;

        for (int i = 0; i < Version; i++)
        {
            for (int j = 0; j < Version; j++)
            {
                if (FreeBit[i, j] == 0) anzFreeBits++;
            }
        }

        double half = 50;
        int zähler = 0;
        while (half < (blackCount * 100) / anzFreeBits)
        {
            if (half == 50 + (5 + zähler)) zähler += 5;
            half++;
        }

        while (half > (blackCount * 100) / anzFreeBits)
        {
            if (half == 50 - (5 + zähler)) zähler += 5;
            half--;
        }

        points = zähler;
        return points;
    }
}