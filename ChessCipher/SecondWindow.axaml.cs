using System;
using Avalonia.Controls;
using Avalonia.Media;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ChessCipherLibrary.Models;
using ChessCipherLibrary;

namespace ChessCipher
{
    public partial class SecondWindow : Window
    {
        private ObservableCollection<ChessSquare> _boardSquares = new();
        private ObservableCollection<MoveDisplayItem> _moveDisplayItems = new();
        private Match _match = new();
        private int _currentMoveIndex;

        public static string Text;

        public SecondWindow()
        {
            InitializeComponent();
            InitializeBoard();
            InitializeMoves();
            SetupEventHandlers();
        }

        private void InitializeBoard()
        {
            _currentMoveIndex = 0;
            var initialBoard = new Board();

            // Створюємо 64 клітинки
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    bool isLight = (row + col) % 2 == 0;
                    _boardSquares.Add(new ChessSquare
                    {
                        Row = row,
                        Col = col,
                        Background = new SolidColorBrush(isLight ? Color.Parse("#f0d9b5") : Color.Parse("#b58863")),
                        Piece = GetPieceSymbol(initialBoard._board[row, col]),
                        PieceColor = GetPieceColor(initialBoard._board[row, col])
                    });
                }
            }

            ChessBoard.ItemsSource = _boardSquares;
        }

        private void InitializeMoves()
        {
            MovesList.ItemsSource = _moveDisplayItems;
            
            // Приклад ходів
            AddExampleMoves();
            UpdateMovesList();
        }

        private void AddExampleMoves()
        {
            _match = ChessCipherLibrary.ChessCipher.FromString(Text);
            Console.WriteLine(ChessCipherLibrary.ChessCipher.FromMatch(_match));
        }

        private void UpdateMovesList()
        {
            _moveDisplayItems.Clear();
            
            for (int i = 0; i < _match.Moves.Count; i += 2)
            {
                var moveItem = new MoveDisplayItem
                {
                    MoveNumber = (i / 2) + 1,
                    WhiteMove = GetMoveNotation(_match.Moves[i]),
                    BlackMove = i + 1 < _match.Moves.Count ? GetMoveNotation(_match.Moves[i + 1]) : ""
                };
                _moveDisplayItems.Add(moveItem);
            }
        }

        private string GetMoveNotation(Move move)
        {
            // Point.X = колонка (a-h), Point.Y = рядок (0-7)
            char fromFile = (char)('a' + move.From.X);  // X для колонки
            int fromRank = 8 - move.From.Y;              // Y для рядка
            char toFile = (char)('a' + move.To.X);
            int toRank = 8 - move.To.Y;
    
            return $"{fromFile}{fromRank}{toFile}{toRank}";
        }

        private void SetupEventHandlers()
        {
            BtnFirst.Click += (s, e) => GoToMove(0);
            BtnPrev.Click += (s, e) => GoToMove(_currentMoveIndex - 1);
            BtnNext.Click += (s, e) => GoToMove(_currentMoveIndex + 1);
            BtnLast.Click += (s, e) => GoToMove(_match.Moves.Count);
            BtnReset.Click += (s, e) => ResetBoard();
            
            MovesList.SelectionChanged += (s, e) =>
            {
                if (MovesList.SelectedIndex >= 0)
                {
                    // Кожен елемент списку = 2 напівходи (білі + чорні)
                    int halfMove = MovesList.SelectedIndex * 2 + 1;
                    GoToMove(halfMove);
                }
            };
        }

        private void GoToMove(int moveIndex)
        {
            if (moveIndex < 0 || moveIndex > _match.Moves.Count) return;

            _currentMoveIndex = moveIndex;
            var board = _match.GetBoard(moveIndex);
            UpdateBoard(board);
            TxtCurrentMove.Text = $"Хід: {moveIndex}";
        }

        private void UpdateBoard(Board board)
        {
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    int index = row * 8 + col;
                    _boardSquares[index].Piece = GetPieceSymbol(board._board[row, col]);
                    _boardSquares[index].PieceColor = GetPieceColor(board._board[row, col]);
                }
            }
        }

        private void ResetBoard()
        {
            GoToMove(0);
        }

        private string GetPieceSymbol(byte piece)
        {
            return piece switch
            {
                11 => "♔", 9 => "♕", 3 => "♖", 7 => "♗", 5 => "♘", 1 => "♙",
                12 => "♚", 10 => "♛", 4 => "♜", 8 => "♝", 6 => "♞", 2 => "♟",
                _ => ""
            };
        }

        private IBrush GetPieceColor(byte piece)
        {
            if (piece == 0) return Brushes.Transparent;
            return piece % 2 != 0 ? Brushes.White : Brushes.Black;
        }
    }

    // Клас з підтримкою INotifyPropertyChanged
    public class ChessSquare : INotifyPropertyChanged
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public IBrush Background { get; set; } = Brushes.Transparent;

        private string _piece = "";
        public string Piece
        {
            get => _piece;
            set
            {
                if (_piece != value)
                {
                    _piece = value;
                    OnPropertyChanged();
                }
            }
        }

        private IBrush _pieceColor = Brushes.Transparent;
        public IBrush PieceColor
        {
            get => _pieceColor;
            set
            {
                if (_pieceColor != value)
                {
                    _pieceColor = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class MoveDisplayItem
    {
        public int MoveNumber { get; set; }
        public string WhiteMove { get; set; } = "";
        public string BlackMove { get; set; } = "";
        public string DisplayText => $"{MoveNumber}. {WhiteMove} {BlackMove}".Trim();
    }
}