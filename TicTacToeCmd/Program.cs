using Newtonsoft.Json;
using System;
using System.IO;
using TicTacToe;

namespace TicTacToeCmd
{
    class Program
    {
        private const string FilePath = "tictactoe.json";

        static void Main(string[] args)
        {
            Console.WriteLine("Tic-Tac-Toe!");

            Game game = Load() ?? new Game();

            if (game.State != Game.GameState.Running)
            {
                game.Start();
                Save(game);
            }

            while (game.State == Game.GameState.Running)
            {
                Render(game);
                ProcessInput(game);
            }

            Render(game);
            Console.WriteLine("Ended!");
        }

        static void Render(Game game)
        {
            for (int row = 0; row < Game.MaxSize; ++row)
            {
                for (int col = 0; col < Game.MaxSize; ++col)
                {
                    switch (game.Board[row, col])
                    {
                        case Game.CellType.Blank:
                            Console.Write("[ ]");
                            break;
                        case Game.CellType.Circle:
                            Console.Write("[O]");
                            break;
                        case Game.CellType.Cross:
                            Console.Write("[X]");
                            break;
                    }
                }

                Console.WriteLine("");
            }
        }

        static void ProcessInput(Game game)
        {
            Console.WriteLine($"{game.CurrentPlayer}'s turn. Enter (row, col):");
            string input = Console.ReadLine();
            string[] items = input.Split(",");

            if (items.Length != 2)
            {
                Console.WriteLine($"Invalid input. Valid example: 2,1");
                return;
            }

            string rowStr = items[0].Trim();
            string colStr = items[1].Trim();

            try
            {
                int row = Convert.ToInt32(rowStr);
                int col = Convert.ToInt32(colStr);

                game.SetCell(row, col, game.CurrentPlayer);

                Save(game);
            }
            catch
            {
                Console.WriteLine($"Invalid input. Valid example: 2,1");
            }
        }

        private static void Save(Game game)
        {
            string json = JsonConvert.SerializeObject(game);
            File.WriteAllText(FilePath, json);
        }

        private static Game Load()
        {
            if (File.Exists(FilePath))
            {
                string json = File.ReadAllText(FilePath);
                return JsonConvert.DeserializeObject<Game>(json);
            }

            return null;
        }
    }
}
