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

        public void SetCell(int row, int col, Player player)
        {
            if (State != GameState.Running)
            {
                return;
            }

            if (player != CurrentPlayer)
            {
                return;
            }

            if (row < 0 || row >= MaxSize)
            {
                return;
            }

            if (col < 0 || col >= MaxSize)
            {
                return;
            }

            if (Board[row, col] != CellType.Blank)
            {
                CurrentPlayer = CurrentPlayer == Player.PlayerCircle ? Player.PlayerCross : Player.PlayerCircle;
                return;
            }

            CellType type = player == Player.PlayerCircle ? CellType.Circle : CellType.Cross;

            Board[row, col] = type;

            if (CheckWinner(row, col, type))
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
        }

        private bool CheckWinner(int rowMarked, int colMarked, CellType typeMarked)
        {
            if (CheckHorizontalLine(rowMarked, typeMarked))
            {
                return true;
            }

            if (CheckVerticalLine(colMarked, typeMarked))
            {
                return true;
            }

            if (CheckDiagonalLine(rowMarked, colMarked, typeMarked))
            {
                return true;
            }

            return false;
        }

        private bool CheckHorizontalLine(int rowMarked, CellType typeMarked)
        {
            for (int col = 0; col < MaxSize; ++col)
            {
                if (Board[rowMarked, col] != typeMarked)
                {
                    return false;
                }
            }

            return true;
        }

        private bool CheckVerticalLine(int colMarked, CellType typeMarked)
        {
            for (int row = 0; row < MaxSize; ++row)
            {
                if (Board[row, colMarked] != typeMarked)
                {
                    return false;
                }
            }

            return true;
        }

        private bool CheckDiagonalLine(int rowMarked, int colMarked, CellType typeMarked)
        {
            if (rowMarked == colMarked)
            {
                for (int index = 0; index < MaxSize; ++index)
                {
                    if (Board[index, index] != typeMarked)
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
                    if (Board[index, MaxSize - index - 1] != typeMarked)
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
