using System.Text;
using ChessCipherLibrary.Models;

namespace ChessCipherLibrary;

public static class ChessCipher
{
    public static Match FromString(string text)
    {
        Match match = new Match();

        string binText = TextToBinary(text);
        
        while (binText.Length > 0)
        {
            var board = match.GetLastBoard();
            var moves = board.GetAllMoves(match.MovesCount % 2 == 0);
            int radix = moves.Count;
            
            int numMove = ReadOneNumber(radix, ref binText);
            
            match.AddMove(moves[numMove]);
        }
        
        return match;
    }

    public static int ReadOneNumber(int radix, ref string text)
    {
        if (string.IsNullOrEmpty(text) || radix <= 0)
            return 0;

        // Кількість біт для представлення radix
        int bitsNeeded = (int)Math.Ceiling(Math.Log2(radix));
    
        int bitsToTake = Math.Min(text.Length, bitsNeeded);
        string bitsToConvert = text.Substring(0, bitsToTake);
        text = text.Substring(bitsToTake);
    
        int decimalValue = Convert.ToInt32(bitsToConvert, 2);
    
        // Обмеження до діапазону [0, radix)
        return decimalValue % radix;
    }

    // Визначення максимальної кількості біт для radix
    private static int GetMaxBitsForRadix(int radix)
    {
        // int.MaxValue = 2,147,483,647
        int maxBits = 0;
        long testValue = 1;
        long maxValue = int.MaxValue;
    
        while (testValue < maxValue / 2)
        {
            testValue *= 2;
            maxBits++;
        }
    
        return maxBits; // Повертає 30 біт для безпеки
    }
    
    public static string TextToBinary(string text)
    {
        if (string.IsNullOrEmpty(text))
            return "";
    
        StringBuilder binary = new StringBuilder();
    
        foreach (char c in text)
        {
            // Кожен символ UTF-16 = 16 біт
            string charBinary = Convert.ToString(c, 2).PadLeft(16, '0');
            binary.Append(charBinary);
        }
    
        return binary.ToString();
    }
}