using System.Drawing;

namespace ChessCipherLibrary.Models;

public class Board
{
    public byte[,] _board { get; set; }
    
    // Прапорці для рокіровки
    public bool WhiteKingMoved { get; set; } = false;
    public bool WhiteRookKingsideMoved { get; set; } = false;
    public bool WhiteRookQueensideMoved { get; set; } = false;
    public bool BlackKingMoved { get; set; } = false;
    public bool BlackRookKingsideMoved { get; set; } = false;
    public bool BlackRookQueensideMoved { get; set; } = false;

    public Board()
    {
        _board = new byte[,]
        {
            { 4, 6, 8, 10, 12, 8, 6, 4 },
            { 2, 2, 2, 2, 2, 2, 2, 2 },
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 1, 1, 1, 1, 1, 1, 1, 1 },
            { 3, 5, 7, 9, 11, 7, 5, 3 }
        };
    }

    public List<Move> GetAllMoves(bool isWhiteTurn)
    {
        List<Move> allMoves = new List<Move>();
        
        int rows = _board.GetLength(0);
        int cols = _board.GetLength(1);
        
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                byte piece = _board[row, col];
                
                if (piece == 0) continue;
                
                bool isPieceWhite = piece % 2 == 1;
                if (isPieceWhite != isWhiteTurn) continue;
                
                allMoves.AddRange(GetMovesForPiece(row, col, piece, isWhiteTurn));
            }
        }
        
        return allMoves;
    }

    private List<Move> GetMovesForPiece(int row, int col, byte piece, bool isWhite)
    {
        List<Move> moves = new List<Move>();
        
        int pieceType = (piece + 1) / 2;
        
        switch (pieceType)
        {
            case 1: // Пішак
                AddPawnMoves(moves, row, col, isWhite);
                break;
            case 2: // Тура
                AddRookMoves(moves, row, col, isWhite);
                break;
            case 3: // Кінь
                AddKnightMoves(moves, row, col, isWhite);
                break;
            case 4: // Слон
                AddBishopMoves(moves, row, col, isWhite);
                break;
            case 5: // Ферзь
                AddQueenMoves(moves, row, col, isWhite);
                break;
            case 6: // Король
                AddKingMoves(moves, row, col, isWhite);
                break;
        }
        
        return moves;
    }

    private void AddPawnMoves(List<Move> moves, int row, int col, bool isWhite)
    {
        int direction = isWhite ? -1 : 1;
        int startRow = isWhite ? 6 : 1;
        
        // Хід вперед на одну клітинку
        if (IsValidPosition(row + direction, col) && _board[row + direction, col] == 0)
        {
            moves.Add(new Move { From = new Point(col, row), To = new Point(col, row + direction) });
            
            // Хід вперед на дві клітинки з початкової позиції
            if (row == startRow && _board[row + 2 * direction, col] == 0)
            {
                moves.Add(new Move { From = new Point(col, row), To = new Point(col, row + 2 * direction) });
            }
        }
        
        // Взяття по діагоналі
        int[] dCol = { -1, 1 };
        foreach (int dc in dCol)
        {
            int newRow = row + direction;
            int newCol = col + dc;
            
            if (IsValidPosition(newRow, newCol) && _board[newRow, newCol] != 0)
            {
                bool targetIsWhite = _board[newRow, newCol] % 2 == 1;
                if (targetIsWhite != isWhite)
                {
                    moves.Add(new Move { From = new Point(col, row), To = new Point(newCol, newRow) });
                }
            }
        }
    }

    private void AddRookMoves(List<Move> moves, int row, int col, bool isWhite)
    {
        int[][] directions = { new[] { -1, 0 }, new[] { 1, 0 }, new[] { 0, -1 }, new[] { 0, 1 } };
        AddSlidingMoves(moves, row, col, isWhite, directions);
    }

    private void AddBishopMoves(List<Move> moves, int row, int col, bool isWhite)
    {
        int[][] directions = { new[] { -1, -1 }, new[] { -1, 1 }, new[] { 1, -1 }, new[] { 1, 1 } };
        AddSlidingMoves(moves, row, col, isWhite, directions);
    }

    private void AddQueenMoves(List<Move> moves, int row, int col, bool isWhite)
    {
        int[][] directions = { 
            new[] { -1, 0 }, new[] { 1, 0 }, new[] { 0, -1 }, new[] { 0, 1 },
            new[] { -1, -1 }, new[] { -1, 1 }, new[] { 1, -1 }, new[] { 1, 1 }
        };
        AddSlidingMoves(moves, row, col, isWhite, directions);
    }

    private void AddKnightMoves(List<Move> moves, int row, int col, bool isWhite)
    {
        int[][] knightMoves = {
            new[] { -2, -1 }, new[] { -2, 1 }, new[] { -1, -2 }, new[] { -1, 2 },
            new[] { 1, -2 }, new[] { 1, 2 }, new[] { 2, -1 }, new[] { 2, 1 }
        };
        
        foreach (var move in knightMoves)
        {
            int newRow = row + move[0];
            int newCol = col + move[1];
            
            if (IsValidPosition(newRow, newCol))
            {
                byte targetPiece = _board[newRow, newCol];
                // Порожня клітинка або ворожа фігура
                if (targetPiece == 0 || (targetPiece % 2 == 1) != isWhite)
                {
                    moves.Add(new Move { From = new Point(col, row), To = new Point(newCol, newRow) });
                }
            }
        }
    }

    private void AddKingMoves(List<Move> moves, int row, int col, bool isWhite)
    {
        int[][] directions = { 
            new[] { -1, -1 }, new[] { -1, 0 }, new[] { -1, 1 },
            new[] { 0, -1 }, new[] { 0, 1 },
            new[] { 1, -1 }, new[] { 1, 0 }, new[] { 1, 1 }
        };
        
        foreach (var dir in directions)
        {
            int newRow = row + dir[0];
            int newCol = col + dir[1];
            
            if (IsValidPosition(newRow, newCol))
            {
                byte targetPiece = _board[newRow, newCol];
                if (targetPiece == 0 || (targetPiece % 2 == 1) != isWhite)
                {
                    moves.Add(new Move { From = new Point(col, row), To = new Point(newCol, newRow) });
                }
            }
        }
        
        // Рокіровка
        AddCastlingMoves(moves, row, col, isWhite);
    }

    private void AddCastlingMoves(List<Move> moves, int row, int col, bool isWhite)
    {
        if (isWhite)
        {
            // Біла коротка рокіровка (kingside)
            if (!WhiteKingMoved && !WhiteRookKingsideMoved)
            {
                // Перевіряємо, чи клітинки між королем і турою порожні
                if (_board[7, 5] == 0 && _board[7, 6] == 0)
                {
                    // Перевіряємо, чи тура на місці
                    if (_board[7, 7] == 3) // Біла тура
                    {
                        moves.Add(new Move { From = new Point(4, 7), To = new Point(6, 7), IsCastling = true });
                    }
                }
            }
            
            // Біла довга рокіровка (queenside)
            if (!WhiteKingMoved && !WhiteRookQueensideMoved)
            {
                // Перевіряємо, чи клітинки між королем і турою порожні
                if (_board[7, 1] == 0 && _board[7, 2] == 0 && _board[7, 3] == 0)
                {
                    // Перевіряємо, чи тура на місці
                    if (_board[7, 0] == 3) // Біла тура
                    {
                        moves.Add(new Move { From = new Point(4, 7), To = new Point(2, 7), IsCastling = true });
                    }
                }
            }
        }
        else
        {
            // Чорна коротка рокіровка (kingside)
            if (!BlackKingMoved && !BlackRookKingsideMoved)
            {
                if (_board[0, 5] == 0 && _board[0, 6] == 0)
                {
                    if (_board[0, 7] == 4) // Чорна тура
                    {
                        moves.Add(new Move { From = new Point(4, 0), To = new Point(6, 0), IsCastling = true });
                    }
                }
            }
            
            // Чорна довга рокіровка (queenside)
            if (!BlackKingMoved && !BlackRookQueensideMoved)
            {
                if (_board[0, 1] == 0 && _board[0, 2] == 0 && _board[0, 3] == 0)
                {
                    if (_board[0, 0] == 4) // Чорна тура
                    {
                        moves.Add(new Move { From = new Point(4, 0), To = new Point(2, 0), IsCastling = true });
                    }
                }
            }
        }
    }

    private void AddSlidingMoves(List<Move> moves, int row, int col, bool isWhite, int[][] directions)
    {
        foreach (var dir in directions)
        {
            int newRow = row + dir[0];
            int newCol = col + dir[1];
            
            while (IsValidPosition(newRow, newCol))
            {
                byte targetPiece = _board[newRow, newCol];
                
                if (targetPiece == 0)
                {
                    // Порожня клітинка - можна йти далі
                    moves.Add(new Move { From = new Point(col, row), To = new Point(newCol, newRow) });
                }
                else
                {
                    // Зустріли фігуру
                    bool targetIsWhite = targetPiece % 2 == 1;
                    if (targetIsWhite != isWhite)
                    {
                        // Можна взяти ворожу фігуру
                        moves.Add(new Move { From = new Point(col, row), To = new Point(newCol, newRow) });
                    }
                    // Не можна перестрибувати фігури
                    break;
                }
                
                newRow += dir[0];
                newCol += dir[1];
            }
        }
    }

    private bool IsValidPosition(int row, int col)
    {
        return row >= 0 && row < _board.GetLength(0) && col >= 0 && col < _board.GetLength(1);
    }
    
    // Метод для виконання ходу (потрібен для оновлення прапорців рокіровки)
    public void MakeMove(Move move)
    {
        int fromRow = move.From.Y;
        int fromCol = move.From.X;
        int toRow = move.To.Y;
        int toCol = move.To.X;
        
        byte piece = _board[fromRow, fromCol];
        
        // Оновлюємо прапорці для рокіровки
        if (piece == 11) // Білий король
        {
            WhiteKingMoved = true;
        }
        else if (piece == 12) // Чорний король
        {
            BlackKingMoved = true;
        }
        else if (piece == 3) // Біла тура
        {
            if (fromRow == 7 && fromCol == 0)
                WhiteRookQueensideMoved = true;
            else if (fromRow == 7 && fromCol == 7)
                WhiteRookKingsideMoved = true;
        }
        else if (piece == 4) // Чорна тура
        {
            if (fromRow == 0 && fromCol == 0)
                BlackRookQueensideMoved = true;
            else if (fromRow == 0 && fromCol == 7)
                BlackRookKingsideMoved = true;
        }
        
        // Виконуємо хід
        _board[toRow, toCol] = piece;
        _board[fromRow, fromCol] = 0;
        
        // Якщо це рокіровка, переміщуємо також туру
        if (move.IsCastling)
        {
            if (toCol == 6) // Коротка рокіровка
            {
                _board[toRow, 5] = _board[toRow, 7];
                _board[toRow, 7] = 0;
            }
            else if (toCol == 2) // Довга рокіровка
            {
                _board[toRow, 3] = _board[toRow, 0];
                _board[toRow, 0] = 0;
            }
        }
    }
}