using System;

namespace TicTacToe
{
    public class Game
    {
        public enum GameState
        {
            None,
            Running,
            Ended,
        }

        public enum Player
        {
            PlayerCircle,
            PlayerCross,
        }

        public enum CellType
        {
            Blank,
            Circle,
            Cross,
        }

        public const int MaxSize = 3;

        public GameState State { get; set; }
        public Player CurrentPlayer { get; set; }
        public Player? Winner { get; set; }
        public CellType[,] Board { get; set; } = new CellType[MaxSize, MaxSize];

        public void Reset()
        {
            State = GameState.None;

            Random rand = new Random();
            CurrentPlayer = (Game.Player)rand.Next(0, 2);

            Winner = null;

            for (int row = 0; row < MaxSize; ++row)
            {
                for (int col = 0; col < MaxSize; ++col)
                {
                    Board[row, col] = CellType.Blank;
                }
            }
        }

        public void Start()
        {
            Reset();

            State = GameState.Running;
        }

        public bool SetCell(int row, int col, Player player)
        {
            if (State != GameState.Running)
            {
                return false;
            }

            if (player != CurrentPlayer)
            {
                return false;
            }

            if (row < 0 || row >= MaxSize)
            {
                return false;
            }

            if (col < 0 || col >= MaxSize)
            {
                return false;
            }

            if (Board[row, col] != CellType.Blank)
            {
                return false;
            }

            CellType type = player == Player.PlayerCircle ? CellType.Circle : CellType.Cross;

            Board[row, col] = type;

            if (CheckWinner(Board, row, col, type))
            {
                Winner = player;
                State = GameState.Ended;
            }
            else if (CheckNoBlank())
            {
                Winner = null;
                State = GameState.Ended;
            }
            else
            {
                CurrentPlayer = CurrentPlayer == Player.PlayerCircle ? Player.PlayerCross : Player.PlayerCircle;
            }

            return true;
        }

        public static bool CheckWinner(CellType[,] board, int rowMarked, int colMarked, CellType typeMarked)
        {
            if (CheckHorizontalLine(board, rowMarked, typeMarked))
            {
                return true;
            }

            if (CheckVerticalLine(board, colMarked, typeMarked))
            {
                return true;
            }

            if (CheckDiagonalLine(board, rowMarked, colMarked, typeMarked))
            {
                return true;
            }

            return false;
        }

        private static bool CheckHorizontalLine(CellType[,] board, int rowMarked, CellType typeMarked)
        {
            for (int col = 0; col < MaxSize; ++col)
            {
                if (board[rowMarked, col] != typeMarked)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool CheckVerticalLine(CellType[,] board, int colMarked, CellType typeMarked)
        {
            for (int row = 0; row < MaxSize; ++row)
            {
                if (board[row, colMarked] != typeMarked)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool CheckDiagonalLine(CellType[,] board, int rowMarked, int colMarked, CellType typeMarked)
        {
            if (rowMarked == colMarked)
            {
                for (int index = 0; index < MaxSize; ++index)
                {
                    if (board[index, index] != typeMarked)
                    {
                        return false;
                    }
                }

                return true;
            }
            else if (rowMarked + colMarked == MaxSize - 1)
            {
                for (int index = 0; index < MaxSize; ++index)
                {
                    if (board[index, MaxSize - index - 1] != typeMarked)
                    {
                        return false;
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }


        private bool CheckNoBlank()
        {
            for (int row = 0; row < MaxSize; ++row)
            {
                for (int col = 0; col < MaxSize; ++col)
                {
                    if (Board[row, col] == CellType.Blank)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
