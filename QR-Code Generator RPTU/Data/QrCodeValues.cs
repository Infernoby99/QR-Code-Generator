using System.Windows;
namespace QR_Code_Generator_RPTU.Data;

public class QrCodeValues
{
    private readonly int[,] _levelCapacity =
    {
        { 17, 32, 53, 78, 106, 134 }, // Low (L)
        { 14, 26, 42, 62, 84, 106 }, // Medium (M)
        { 11, 20, 32, 46, 60, 74 }, // Quartile (Q)
        { 7, 14, 24, 34, 44, 58 } // High (H)
    };

    private readonly int[,] _correctionBytes =
    {
        { 7, 10, 15, 20, 26, 18 }, // Low (L) - Anzahl Korrekturbytes für jede Version
        { 10, 16, 26, 18, 24, 16 }, // Medium (M)
        { 13, 22, 18, 26, 18, 24 }, // Quartile (Q)
        { 17, 28, 22, 16, 22, 18 } // High (H)
    };

    private readonly int[,] _correctionBytesBlockGroup1 =
    {
        { 1, 1, 1, 1, 1, 1 }, // Low (L) - Anzahl Blöcke für jede Version
        { 1, 1, 1, 2, 2, 2 }, // Medium (M)
        { 1, 1, 2, 2, 2, 4 }, // Quartile (Q)
        { 1, 1, 2, 4, 2, 4 } // High (H)
    };

    private readonly int[,] _correctionBytesBlockGroup2 =
    {
        { 0, 0, 0, 0, 0, 0 }, // Low (L) - Anzahl Blöcke für jede Version
        { 0, 0, 0, 0, 0, 0 }, // Medium (M)
        { 0, 0, 0, 0, 2, 0 }, // Quartile (Q)
        { 0, 0, 0, 0, 2, 0 } // High (H)
    };

    private readonly string[] _formatPatternLevel =["L", "M", "Q", "H"];

    private readonly int[] _versionSize = [21, 25, 29, 33, 37, 41];
    private string MessageBin { set; get; }
    private int EccLevel { set; get; }
    private int MaxCharaSize { set; get; }
    private int ModuleSize { set; get; }
    private int BlockNumber1 { set; get; }
    private int BlockNumber2 { set; get; }

    private readonly int[] _log = new int[256];

    private readonly int[] _antiLog = new int[256];

    readonly string[,] _formatPatterns = new string[4, 8]
    {
        {
            "111011111000100", "111001011110011", "111110110101010", "111100010011101",
            "110011000101111", "110001100011000", "110110001000001", "110100101110110"
        },
        {
            "101010000010010", "101000100100101", "101111001111100", "101101101001011",
            "100010111111001", "100000011001110", "100111110010111", "100101010100000"
        },
        {
            "011010101011111", "011000001101000", "011111100110001", "011101000000110",
            "010010010110100", "010000110000011", "010111011011010", "010101111101101"
        },
        {
            "001011010001001", "001001110111110", "001110011100111", "001100111010000",
            "000011101100010", "000001001010101", "000110100001100", "000100000111011"
        }
    };
    private int NumberOfCorrectionBytes { set; get; }
    
    public QrCodeValues()
    {
        MessageBin = "";
        BuildAntiLog();
        BuildLog();
    }

    private void BuildAntiLog()
    {
        int calc = 1;
        for (int i = 0; i < 256; i++)
        {
            _antiLog[i] = calc;
            calc *= 2;

            if (calc > 255) calc ^= 285;
        }
    }

    private void BuildLog()
    {
        _log[0] = -1;
        for (int i = 0; i < 255; i++)
        {
            _log[_antiLog[i]] = i;
        }
    }

    public int Version(int length)
    {
        for (int i = 0; i < _levelCapacity.GetLength(1); i++)
        {
            for (int j = 3; j >= 0; j--)
            {
                if (length <= _levelCapacity[j, i])
                {
                    Console.WriteLine($"_______________________________________________________________________________________________________________________________");
                    Console.WriteLine($"_______________________________________________________________________________________________________________________________");
                    Console.WriteLine($"Message Length \t\t\t|==> \t{length}");
                    MaxCharaSize = _levelCapacity[j, i];
                    Console.WriteLine($"Max Character length \t\t|==> \t{MaxCharaSize}");
                    NumberOfCorrectionBytes = _correctionBytes[j, i];
                    Console.WriteLine($"Number of Correction bytes \t|==> \t{NumberOfCorrectionBytes}");
                    BlockNumber1 = _correctionBytesBlockGroup1[j, i];
                    Console.WriteLine($"Block Group size 1 \t\t|==> \t{BlockNumber1}");
                    BlockNumber2 = _correctionBytesBlockGroup2[j, i];
                    Console.WriteLine($"Block Group size 2 \t\t|==> \t{BlockNumber2}");
                    ModuleSize = _versionSize[i];
                    Console.WriteLine($"Version size \t\t\t|==> \t{ModuleSize}");
                    Console.WriteLine($"Version level \t\t\t|==> \t{i + 1}");
                    Console.WriteLine($"Correction level \t\t|==> \t{_formatPatternLevel[j]}");
                    EccLevel = j;
                    
                    return ModuleSize;
                }
            }
        }
        return 0;
    }

    public string FormatPattern(int mask)
    {
        return _formatPatterns[EccLevel, mask];
    }
    public string MessageBytes(string message)
    {
        MessageBin = "0100";
        int temp = message.Length;
        string bin = "";
        bool change = false;
        // decode byte Length
        for (int i = 0; i < 8; i++)
        {
            bin += temp % 2 == 0 ? "0" : "1";
            temp /= 2;
        }
        MessageBin += ReverseString(bin);
        // decode Message
        for (int i = 0; i <= message.Length; i++)
        {
            bin = "";
            if (i < message.Length)
            {
                // message bits
                temp = message[i];
                for (int j = 0; j < 8; j++)
                {
                    bin += temp % 2 == 0 ? "0" : "1";
                    temp /= 2;
                }
            }
            else if (i == message.Length)
            {
                // terminate bits 
                MessageBin += "0000";
            }
            if(bin != "") MessageBin += ReverseString(bin);
            
        }

        while (MessageBin.Length / 8 < MaxCharaSize + 2)
        {
            MessageBin += !change ? "11101100" : "00010001";
            change = !change;
        }
        // correction bits
        MessageBin = CalculatingCorrectionBytes();
        PrintHexadecimal();
        return MessageBin;
    }
    
    private static string ReverseString(string s)
    {
        char[] charArray = s.ToArray();
        Array.Reverse(charArray);
        return new string(charArray);
    }

    private void PrintHexadecimal()
    {
        int[] Hexa = new int[MessageBin.Length / 8];
        for (int i = 0; i < MessageBin.Length / 8; i++)
        {
            int counter = 0;
            
            string bin = MessageBin.Substring(i * 8, 8);
            for (int j = bin.Length - 1; j > 0; j--)
            {
                
            }
        }
    }

    private string  CalculatingCorrectionBytes()
    {
        string message = MessageBin;
        string[] correctionBytes = new string[BlockNumber1 + BlockNumber2];
        int[] blocksizes = GetBlockSizes();
        string[] blockDatalines = new string[BlockNumber1 + BlockNumber2];
        int index = 0;
        int currentIndex = 0; // Start position for Substring
        
        for (int j = 0; j < BlockNumber1; j++)
        {
            string blockData = message.Substring(currentIndex, blocksizes[0] * 8);
            blockDatalines[index] = blockData;
            int[] blockDataDecimal = GetMessagePolynomial(blockData);
            int[] generator = BuildGeneratorPolynomial();
            correctionBytes[index++] += CalculatedCorrectionBytes(blockDataDecimal, generator);
            currentIndex += blocksizes[0] * 8;
        }

        for (int j = 0; j < BlockNumber2; j++)
        {
            string blockData = message.Substring(currentIndex, blocksizes[1] * 8);
            blockDatalines[index] = blockData;
            int[] blockDataDecimal = GetMessagePolynomial(blockData);
            int[] generator = BuildGeneratorPolynomial();
            correctionBytes[index++] += CalculatedCorrectionBytes(blockDataDecimal, generator);
            currentIndex += blocksizes[1] * 8;
        }
        
        for (int i = 0; i < blocksizes[1]; i++)
        {
            if (i < blocksizes[0])
            {
                for (int j = 0; j < BlockNumber1; j++)
                {
                    string line = blockDatalines[j];
                    message += line.Substring(i * 8, 8);
                }
            }

            for (int j = BlockNumber1; j < BlockNumber1 + BlockNumber2; j++)
            {
                string line = blockDatalines[j];
                message += line.Substring(i * 8, 8);
            }
        }

        for (int i = 0; i < NumberOfCorrectionBytes; i++)
        {
            for (int j = 0; j < BlockNumber1 + BlockNumber2; j++)
            {
                string line = correctionBytes[j];
                message += line.Substring(i * 8, 8);
            }
        }

        return message;
    }
    
    private string CalculatedCorrectionBytes(int[] polynomial, int[] generator)
    {
        try
        {
            string cBytes = "";
            int[] polynomialTerms = new int[polynomial.Length + NumberOfCorrectionBytes]; 
            Array.Copy(polynomial, 0, polynomialTerms, 0, polynomial.Length);
            for (int i = 0; i < polynomial.Length; i++)
            {
                if (polynomialTerms[i] != 0)
                {
                    int leadTerm = polynomialTerms[i];
                    for (int j = 0; j < generator.Length; j++)
                    {
                        int logValue = (_log[leadTerm]+ _log[generator[j]]) % 255;
                        polynomialTerms[i + j] ^= _antiLog[logValue];
                    }
                }
            }
            
            int[] correctionBytes = new int[NumberOfCorrectionBytes];
            Array.Copy(polynomialTerms, polynomialTerms.Length - NumberOfCorrectionBytes, correctionBytes, 0, NumberOfCorrectionBytes);
            
            for (int i = 0; i < correctionBytes.Length; i++)
            {
                string bin = "";
                for (int j = 0; j < 8; j++)
                {
                    bin += correctionBytes[i] % 2 == 0 ? '0' : '1';
                    correctionBytes[i] /= 2;
                }

                cBytes += ReverseString(bin);
            }
            return cBytes;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Fehler: {ex.Message}  \n Methode: {ex.TargetSite} " +
                            $"\nSource: {ex.Source}\n Stacktrace: {ex.StackTrace}");
            throw;
        }
    }
    
    private void PrintArray(int[] array)
    {
        string arrayString = string.Join(", ", array);
        Console.WriteLine(arrayString);
    }

    private int[] GetBlockSizes()
    {
        if (ModuleSize == 33 && (MaxCharaSize + 2 == 62 || MaxCharaSize + 2 == 46))
        {
            if (MaxCharaSize + 2 == 62) return [15, 16];
            return [11, 12];
        }
        int size = (MaxCharaSize + 2) / BlockNumber1;
        return [size, 0];
    }
    
    private int[] GetMessagePolynomial(string blockData)
    {
        int[] polynomial = new int[blockData.Length / 8];
        int calc = 1;
        for (int i = 0; i < polynomial.Length; i++)
        {
            string messageByte = blockData.Substring(i * 8, 8);
            for (int j = 7; j >= 0; j--)
            {
                polynomial[i] += messageByte[j] == '1' ? calc : 0;
                
                calc *= 2;
            }
            calc = 1;
        }
        return polynomial;
    }
    
    private int[] BuildGeneratorPolynomial()
    {
        int[] generator = [1];

        for (int i = 0; i < NumberOfCorrectionBytes; i++)
        {
            int[] nextTerm = [1, _antiLog[i]];
            generator = MultiplyPolynomials(generator, nextTerm);
        }
        
        return generator;
    }
    
    private int[] MultiplyPolynomials(int[] term1, int[] term2)
    {
        int degree1 = term1.Length - 1;
        int degree2 = term2.Length - 1;

        int[] result = new int[degree1 + degree2 + 1];

        for (int i = 0; i <= degree1; i++)
        {
            for (int j = 0; j <= degree2; j++)
            {
                if (term1[i] != 0 && term2[j] != 0)
                {
                    int logSum = (_log[term1[i]] + _log[term2[j]]) % 255;
                    result[i + j] ^= _antiLog[logSum];
                }
            }
        }
        
        return result;
    }
}