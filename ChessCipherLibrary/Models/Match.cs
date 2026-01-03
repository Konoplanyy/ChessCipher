namespace ChessCipherLibrary.Models;

public class Match
{
    public List<Move> Moves { get; set; }
    public int MovesCount { get { return Moves.Count; } }
    
    public Match()
    {
        Moves = new List<Move>();
    }
    
    public void AddMove(Move move)
    {
        Moves.Add(move);
    }

    public Board GetLastBoard()
    {
        return GetBoard(Moves.Count);
    }
    
    public Board GetBoard(int Move)
    {
        Board board = new Board();
        for (int i = 0; i < Moves.Count && i < Move; i++)
        {
            board = DoMove(Moves[i], board);
        }
        return board;
    }

    private static Board DoMove(Move move, Board board)
    {
        // Point: X = col, Y = row
        // board._board[row, col]
        int fromRow = move.From.Y;  // Y = row
        int fromCol = move.From.X;  // X = col
        int toRow = move.To.Y;
        int toCol = move.To.X;
    
        byte piece = board._board[fromRow, fromCol];
    
        // Виконуємо хід
        board._board[toRow, toCol] = piece;
        board._board[fromRow, fromCol] = 0;
    
        // Оновлюємо прапорці для рокіровки
        if (piece == 11) board.WhiteKingMoved = true;
        else if (piece == 12) board.BlackKingMoved = true;
        else if (piece == 3)
        {
            if (fromRow == 7 && fromCol == 0) board.WhiteRookQueensideMoved = true;
            else if (fromRow == 7 && fromCol == 7) board.WhiteRookKingsideMoved = true;
        }
        else if (piece == 4)
        {
            if (fromRow == 0 && fromCol == 0) board.BlackRookQueensideMoved = true;
            else if (fromRow == 0 && fromCol == 7) board.BlackRookKingsideMoved = true;
        }
    
        // Рокіровка
        if (move.IsCastling)
        {
            if (toCol == 6) // Коротка
            {
                board._board[toRow, 5] = board._board[toRow, 7];
                board._board[toRow, 7] = 0;
            }
            else if (toCol == 2) // Довга
            {
                board._board[toRow, 3] = board._board[toRow, 0];
                board._board[toRow, 0] = 0;
            }
        }
    
        return board;
    }
}