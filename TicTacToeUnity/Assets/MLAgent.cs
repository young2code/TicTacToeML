using System.Collections.Generic;
using TicTacToe;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class MLAgent : Agent
{
    public Main Main;
    private Game.Player _mlPlayer;

    private void Update()
    {
        Game game = Main.GetGame();
        if (game.State == Game.GameState.Running)
        {
            if (game.CurrentPlayer == _mlPlayer)
            {
                RequestAction();
            }
            else
            {
                RandomPlay();
            }
        }
        else
        {
            FinishEpisode();
        }
    }

    public override void OnEpisodeBegin()
    {
        _mlPlayer = (Game.Player)Random.Range(0, 2);
        Game game = Main.GetGame();
        game.Start();
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
        // Actions, size = 2
        int row = (int)vectorAction[0];
        int col = (int)vectorAction[1];

        Main.Play(row, col);
    }

    private void RandomPlay()
    {
        Game game = Main.GetGame();

        List<(int row, int col)> potentialPlay = new List<(int row, int col)>();

        for (int row = 0; row < Game.MaxSize; ++row)
        {
            for (int col = 0; col < Game.MaxSize; ++col)
            {
                if (game.Board[row, col] == Game.CellType.Blank)
                {
                    potentialPlay.Add((row, col));
                }
            }
        }

        var randomPlay = potentialPlay[Random.Range(0, potentialPlay.Count)];

        Main.Play(randomPlay.row, randomPlay.col);
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
}
