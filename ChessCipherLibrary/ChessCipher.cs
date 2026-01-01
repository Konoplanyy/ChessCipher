using System.Numerics;
using System.Text;
using ChessCipherLibrary.Models;

namespace ChessCipherLibrary;

public static class ChessCipher
{
    public static string FromMatch(Match match)  
    {  
        List<int> radixes = new List<int>();  
        List<int> indices = new List<int>();  
  
        Match tempMatch = new Match();  
  
        for (int i = 0; i < match.MovesCount; i++)  
        {  
            var board = tempMatch.GetLastBoard();  
            var moves = board.GetAllMoves(i % 2 == 0);  
            moves.Sort();  
      
            var move = match.Moves[i];  
            int index = moves.FindIndex(x => x.From == move.From && x.To == move.To);  
          
            if (index == -1)  
                return "";  
      
            radixes.Add(moves.Count);  
            indices.Add(index);  
          
            tempMatch.AddMove(move);  
        }  
  
        // Конвертуємо в одне велике число  
        BigInteger bigNumber = 0;  
        BigInteger multiplier = 1;  
  
        for (int i = 0; i < indices.Count; i++)  
        {  
            bigNumber += indices[i] * multiplier;  
            multiplier *= radixes[i];  
        }  
  
        // Конвертуємо BigInteger в байти  
        byte[] bytes = bigNumber.ToByteArray();  
      
        // Конвертуємо в Base64  
        return Base64ToString(Convert.ToBase64String(bytes));  
    }
    
    public static string BinaryToText(string binary)  
    {  
        List<byte> bytes = new List<byte>();  
      
        for (int i = 0; i < binary.Length; i += 8)  
        {  
            int length = Math.Min(8, binary.Length - i);  
            string byteString = binary.Substring(i, length);  
          
            if (byteString.Length < 8)  
                byteString = byteString.PadLeft(8, '0');  
          
            bytes.Add((byte)Convert.ToInt32(byteString, 2));  
        }  
      
        return Encoding.UTF8.GetString(bytes.ToArray());  
    }
    
    private static string ConvertToBinary(BigInteger number)
    {
        if (number == 0) return "0";
    
        StringBuilder binary = new StringBuilder();
        while (number > 0)
        {
            binary.Insert(0, (number % 2).ToString());
            number /= 2;
        }
        return binary.ToString();
    }
    
    public static Match FromString(string text)  
    {  
        Match match = new Match();  
        
        
        // Конвертуємо з Base64 в байти  
        byte[] bytes = Convert.FromBase64String(StringToBase64(text));  
      
        // Конвертуємо байти в BigInteger  
        BigInteger bigNumber = new BigInteger(bytes);  
  
        // Декодуємо  
        int moveCount = 0;  
        while (bigNumber > 0 || moveCount == 0)  
        {  
            var board = match.GetLastBoard();  
            var moves = board.GetAllMoves(moveCount % 2 == 0);  
      
            if (moves.Count == 0)  
                break;  
          
            moves.Sort();  
            int radix = moves.Count;  
      
            int index = (int)(bigNumber % radix);  
            bigNumber /= radix;  
      
            match.AddMove(moves[index]);  
            moveCount++;  
          
            if (bigNumber == 0 && moveCount > 0)  
                break;  
        }  
  
        return match;  
    }
    
    public static int ReadOneNumber(int radix, ref string text)
    {
        if (string.IsNullOrEmpty(text) || radix <= 1)
            return 0;

        int bitsNeeded = (int)Math.Ceiling(Math.Log2(radix));
    
        if (bitsNeeded == 0 || text.Length < bitsNeeded)
            return 0;

        int bitsToTake = Math.Min(text.Length, bitsNeeded);
        string bitsToConvert = text.Substring(0, bitsToTake);
        text = text.Substring(bitsToTake);

        int decimalValue = Convert.ToInt32(bitsToConvert, 2);

        return decimalValue;
    }
    
    public static string StringToBase64(string text)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(text);
        return Convert.ToBase64String(bytes);
    }

    public static string Base64ToString(string base64)
    {
        byte[] bytes = Convert.FromBase64String(base64);
        return Encoding.UTF8.GetString(bytes);
    }
    
    public static string TextToBinary(string text)  
    {  
        byte[] bytes = Encoding.UTF8.GetBytes(text);  
        StringBuilder result = new StringBuilder();  
      
        foreach (byte b in bytes)  
        {  
            result.Append(Convert.ToString(b, 2).PadLeft(8, '0'));  
        }  
      
        return result.ToString();  
    }
}