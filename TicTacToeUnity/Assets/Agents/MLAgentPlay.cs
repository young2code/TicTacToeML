using System.Collections.Generic;
using TicTacToe;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class MLAgentPlay : Agent
{
    public enum HeuristicType
    {
        Random,
        Basic,
    }

    public IMain Main;
    public HeuristicType CurrentHeuristicType;

    private readonly Game.Player Player = Game.Player.PlayerCircle;

    private void FixedUpdate()
    {
        Game game = Main.GetGame();
        if (game.State == Game.GameState.Running)
        {
            if (game.CurrentPlayer == Player)
            {
                RequestDecision();
            }
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Game game = Main.GetGame();

        for (int row = 0; row < Game.MaxSize; ++row)
        {
            for (int col = 0; col < Game.MaxSize; ++col)
            {
                sensor.AddObservation((int)game.Board[row, col]);
            }
        }
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        var mlPlay = GetRowCol((int)vectorAction[0]);
        Main.Play(mlPlay.row, mlPlay.col);
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

    private int GetRandomPlayIndex()
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

        return possiblePlays[Random.Range(0, possiblePlays.Count)];
    }

    private int GetBasicPlayIndex()
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

        // Obvious Attack
        foreach (int playIndex in possiblePlays)
        {
            var play = GetRowCol(playIndex);
            Game.CellType[,] board = game.Board.Clone() as Game.CellType[,];
            board[play.row, play.col] = Game.CellType.Circle;

            if (Game.CheckWinner(board, play.row, play.col, Game.CellType.Circle))
            {
                return playIndex;
            }
        }

        // Obvious Defense
        foreach (int playIndex in possiblePlays)
        {
            var play = GetRowCol(playIndex);
            Game.CellType[,] board = game.Board.Clone() as Game.CellType[,];
            board[play.row, play.col] = Game.CellType.Cross;

            if (Game.CheckWinner(board, play.row, play.col, Game.CellType.Cross))
            {
                return playIndex;
            }
        }

        // Random
        return possiblePlays[Random.Range(0, possiblePlays.Count)];
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
}
