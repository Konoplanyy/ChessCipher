using System.Drawing;

namespace ChessCipherLibrary.Models;

public class Move
{
    public Point From { get; set; }
    public Point To { get; set; }
    public bool IsCastling { get; set; } = false;
}