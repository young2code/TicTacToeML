using System.Collections.Generic;
using TicTacToe;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class MLAgent : Agent
{
    public enum HeuristicType
    {
        Random,
        Basic,
    }

    public IMain Main;
    public HeuristicType CurrentHeuristicType;
    public Game.Player Player;

    public override void OnActionReceived(float[] vectorAction)
    {
        var mlPlay = GetRowCol((int)vectorAction[0]);
        Main.Play(mlPlay.row, mlPlay.col);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Game game = Main.GetGame();

        for (int row = 0; row < Game.MaxSize; ++row)
        {
            for (int col = 0; col < Game.MaxSize; ++col)
            {
                Game.CellType cell = game.Board[row, col];
                sensor.AddObservation(GetFeatureFromCell(cell));
            }
        }
    }

    public override void CollectDiscreteActionMasks(DiscreteActionMasker actionMasker)
    {
        actionMasker.SetMask(0, GetImpossiblePlays());
    }

    public override void Heuristic(float[] actionsOut)
    {
        switch (CurrentHeuristicType)
        {
            case HeuristicType.Random:
                actionsOut[0] = GetRandomPlayIndex();
                break;

            case HeuristicType.Basic:
                actionsOut[0] = GetBasicPlayIndex();
                break;
        }
    }

    public void FinishEpisode()
    {
        Game game = Main.GetGame();

        if (game.Winner == Player)
        {
            SetReward(1.0f);
        }
        else if (game.Winner == null)
        {
            SetReward(0.0f);
        }
        else
        {
            SetReward(-1.0f);
        }

        EndEpisode();
    }

    private int GetRandomPlayIndex()
    {
        List<int> possiblePlays = GetPossiblePlays();
        return possiblePlays[Random.Range(0, possiblePlays.Count)];
    }

    private int GetBasicPlayIndex()
    {
        List<int> possiblePlays = GetPossiblePlays();

        Game game = Main.GetGame();

        // Obvious Attack
        foreach (int playIndex in possiblePlays)
        {
            var play = GetRowCol(playIndex);
            Game.CellType[,] board = game.Board.Clone() as Game.CellType[,];
            board[play.row, play.col] = GetMyCellType();

            if (Game.CheckWinner(board, play.row, play.col, GetMyCellType()))
            {
                return playIndex;
            }
        }

        // Obvious Defense
        foreach (int playIndex in possiblePlays)
        {
            var play = GetRowCol(playIndex);
            Game.CellType[,] board = game.Board.Clone() as Game.CellType[,];
            board[play.row, play.col] = GetOpponentCellType();

            if (Game.CheckWinner(board, play.row, play.col, GetOpponentCellType()))
            {
                return playIndex;
            }
        }

        // Random
        return possiblePlays[Random.Range(0, possiblePlays.Count)];
    }

    private List<int> GetPossiblePlays()
    {
        Game game = Main.GetGame();

        List<int> possiblePlays = new List<int>();

        for (int row = 0; row < Game.MaxSize; ++row)
        {
            for (int col = 0; col < Game.MaxSize; ++col)
            {
                if (game.Board[row, col] == Game.CellType.Blank)
                {
                    possiblePlays.Add(GetIndex(row, col));
                }
            }
        }

        return possiblePlays;
    }

    private List<int> GetImpossiblePlays()
    {
        Game game = Main.GetGame();

        List<int> impossbilePlays = new List<int>();

        for (int row = 0; row < Game.MaxSize; ++row)
        {
            for (int col = 0; col < Game.MaxSize; ++col)
            {
                if (game.Board[row, col] != Game.CellType.Blank)
                {
                    impossbilePlays.Add(GetIndex(row, col));
                }
            }
        }

        return impossbilePlays;
    }

    private int GetIndex(int row, int col)
    {
        return row * Game.MaxSize + col;
    }

    private (int row, int col) GetRowCol(int index)
    {
        return (index / Game.MaxSize, index % Game.MaxSize);
    }

    private int GetFeatureFromCell(Game.CellType cell)
    {
        // Invert the board state for Cross player so that it plays as Circle player.
        // This will train the model to connect '1's and block '-1's.
        switch (cell)
        {
            case Game.CellType.Circle:
                return Player == Game.Player.PlayerCircle ? 1 : -1;
            case Game.CellType.Cross:
                return Player == Game.Player.PlayerCircle ? -1 : 1;
            case Game.CellType.Blank:
                return 0;
        }

        // This cannot be reached.
        Debug.Assert(true);
        return 0;
    }

    private Game.CellType GetMyCellType()
    {
        return Player == Game.Player.PlayerCircle ? Game.CellType.Circle : Game.CellType.Cross;
    }

    private Game.CellType GetOpponentCellType()
    {
        return Player == Game.Player.PlayerCircle ? Game.CellType.Cross : Game.CellType.Circle;
    }
}
