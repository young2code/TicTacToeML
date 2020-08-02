﻿using System.Collections.Generic;
using TicTacToe;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class MLAgent : Agent
{
    public enum Mode
    {
        Train,
        Play,
    }

    public Main Main;
    public Mode CurrentMode;
    public Game.Player Player;

    private void FixedUpdate()
    {
        Game game = Main.GetGame();
        if (game.State == Game.GameState.Running)
        {
            if (game.CurrentPlayer == Player)
            {
                RequestDecision();
            }
            else if (CurrentMode == Mode.Train)
            {
                RandomPlay();
            }
        }
        else if (game.State == Game.GameState.Ended)
        {
            FinishEpisode();
        }
    }

    public override void OnEpisodeBegin()
    {
        if (CurrentMode == Mode.Play)
        {
            return;
        }

        Player = Game.Player.PlayerCircle;

        Game game = Main.GetGame();
        if (game.State != Game.GameState.Running)
        {
            Main.StartNewGame();
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

    private void RandomPlay()
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

        int randomPlayIndex =  possiblePlays[Random.Range(0, possiblePlays.Count)];
        var randomPlay = GetRowCol(randomPlayIndex);
        Main.Play(randomPlay.row, randomPlay.col);
    }

    private void FinishEpisode()
    {
        if (CurrentMode == Mode.Play)
        {
            return;
        }
        
        Game game = Main.GetGame();

        // Rewards
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

    private int GetIndex(int row, int col)
    {
        return row * Game.MaxSize + col;
    }

    private (int row, int col) GetRowCol(int index)
    {
        return (index / Game.MaxSize, index % Game.MaxSize);
    }
}
