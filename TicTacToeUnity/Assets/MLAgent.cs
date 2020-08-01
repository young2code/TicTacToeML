using System.Collections.Generic;
using TicTacToe;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class MLAgent : Agent
{
    public Main Main;
    private Game.Player _mlPlayer;

    private void FixedUpdate()
    {
        Game game = Main.GetGame();
        if (game.State == Game.GameState.Running)
        {
            if (game.CurrentPlayer == _mlPlayer)
            {
                RequestDecision();
            }
            else
            {
                int randomPlay = RandomPlay();
                var play = GetRowCol(randomPlay);
                Main.Play(play.row, play.col);
            }
        }
        else if (game.State == Game.GameState.Ended)
        {
            FinishEpisode();
        }
    }

    public override void OnEpisodeBegin()
    {
        _mlPlayer = (Game.Player)Random.Range(0, 2);
        Main.StartNewGame();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Game game = Main.GetGame();

        sensor.AddObservation((int)_mlPlayer);

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
        // Actions, size = 0 [0~8]
        var play = GetRowCol((int)vectorAction[0]);

        Main.Play(play.row, play.col);
    }

    //public override void Heuristic(float[] actionsOut)
    //{
    //    actionsOut[0] = RandomPlay();
    //}

    public override void CollectDiscreteActionMasks(DiscreteActionMasker actionMasker)
    {
        actionMasker.SetMask(0, GetImpossiblePlays());
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

    private int RandomPlay()
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

    private void FinishEpisode()
    {
        Game game = Main.GetGame();

        // Rewards
        if (game.Winner == _mlPlayer)
        {
            SetReward(1.0f);
        }
        else if (game.Winner == null)
        {
            SetReward(0.5f);
        }
        else
        {
            SetReward(-1.0f);
        }

        EndEpisode();
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
