using System.Collections.Generic;
using TicTacToe;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class MLAgentSelfPlay : Agent
{
    public MainSelf Main;
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
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Game game = Main.GetGame();

        for (int row = 0; row < Game.MaxSize; ++row)
        {
            for (int col = 0; col < Game.MaxSize; ++col)
            {
                Game.CellType cell = game.Board[row, col];

                // Inverse the cell so that it plays as if  it is a circle player.
                if (Player == Game.Player.PlayerCross)
                {
                    switch (cell)
                    {
                        case Game.CellType.Blank:
                            break;

                        case Game.CellType.Circle:
                            cell = Game.CellType.Cross;
                            break;

                        case Game.CellType.Cross:
                            cell = Game.CellType.Circle;
                            break;
                    }
                }

                sensor.AddObservation((int)cell);
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

    private int GetIndex(int row, int col)
    {
        return row * Game.MaxSize + col;
    }

    private (int row, int col) GetRowCol(int index)
    {
        return (index / Game.MaxSize, index % Game.MaxSize);
    }
}
