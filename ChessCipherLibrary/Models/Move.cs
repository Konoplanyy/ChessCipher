using System.Drawing;

namespace ChessCipherLibrary.Models;

public class Move: IComparable
{
    public Point From { get; set; }
    public Point To { get; set; }
    public bool IsCastling { get; set; } = false;
    public int CompareTo(object? obj)  
    {  
        if (obj == null) return 1;  
          
        Move? other = obj as Move;  
        if (other == null)  
            throw new ArgumentException("Object is not a Move");  
          
        // Спочатку порівнюємо From.X  
        int fromXComparison = this.From.X.CompareTo(other.From.X);  
        if (fromXComparison != 0) return fromXComparison;  
          
        // Потім From.Y  
        int fromYComparison = this.From.Y.CompareTo(other.From.Y);  
        if (fromYComparison != 0) return fromYComparison;  
          
        // Потім To.X  
        int toXComparison = this.To.X.CompareTo(other.To.X);  
        if (toXComparison != 0) return toXComparison;  
          
        // Потім To.Y  
        int toYComparison = this.To.Y.CompareTo(other.To.Y);  
        if (toYComparison != 0) return toYComparison;  
          
        // І нарешті IsCastling (false < true)  
        return this.IsCastling.CompareTo(other.IsCastling);  
    }
    
    public override bool Equals(object obj)  
    {  
        if (obj is Move other)  
        {  
            // Порівнюй поля, наприклад:  
            return this.From == other.From && this.To == other.To;  
        }  
        return false;  
    }
    
    public override int GetHashCode()  
    {  
        return HashCode.Combine(From, To);  
    }
}