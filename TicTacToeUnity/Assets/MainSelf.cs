﻿using Newtonsoft.Json;
using System.IO;
using TicTacToe;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MainSelf : MonoBehaviour
{
    private const string FileName = "tictactoe.json";

    public GameObject BlankCellPrefab;
    public GameObject CircleCellPrefab;
    public GameObject CrossCellPrefab;
    public Canvas Canvas;
    public Button NewGameButton;
    public Text StateText;
    public MLAgentSelfPlay Agent_0;
    public MLAgentSelfPlay Agent_1;
    public bool SkipRendering;

    private Game _game;
    private Button[,] _board = new Button[Game.MaxSize, Game.MaxSize];

    // Start is called before the first frame update
    private void Start()
    {
        NewGameButton.onClick.AddListener(StartNewGame);

        _game = Load() ?? new Game();

        CreateCells();

        UpdateStateText();
    }

    private void FixedUpdate()
    {
        if (Agent_0 != null && Agent_1 != null)
        {
            if (_game.State == Game.GameState.None)
            {
                StartNewGame();
            }
            else if (_game.State == Game.GameState.Ended)
            {
                FinishEpisode(Agent_0);
                FinishEpisode(Agent_1);
                StartNewGame();
            }
        }

        void FinishEpisode(MLAgentSelfPlay agent)
        {
            // Rewards
            if (_game.Winner == agent.Player)
            {
                agent.SetReward(1.0f);
            }
            else if (_game.Winner == null)
            {
                agent.SetReward(0.0f);
            }
            else
            {
                agent.SetReward(-1.0f);
            }

            agent.EndEpisode();
        }
    }

    public void StartNewGame()
    {
        _game.Start();
        Save(_game);

        CreateCells();

        UpdateStateText();
    }

    public Game GetGame()
    {
        return _game;
    }

    private void CreateCells()
    {
        if (SkipRendering)
        {
            return;
        }

        for (int row = 0; row < Game.MaxSize; ++row)
        {
            for (int col = 0; col < Game.MaxSize; ++col)
            {
                CreateCell(row, col);
            }
        }
    }

    private void CreateCell(int row, int col)
    {
        if (SkipRendering)
        {
            return;
        }

        GameObject newGameObject = Instantiate(GetCellPrefab(row, col), Canvas.transform);
        Button button = newGameObject.GetComponent<Button>();

        if (_board[row, col] != null)
        {
            Destroy(_board[row, col].gameObject);
        }

        _board[row, col] = button;
        button.onClick.AddListener(GetCellOnClick(row, col));
        button.transform.localPosition = GetCellPosition(row, col);
    }

    private GameObject GetCellPrefab(int row, int col)
    {
        switch (_game.Board[row, col])
        {
            case Game.CellType.Blank:
                return BlankCellPrefab;
            case Game.CellType.Circle:
                return CircleCellPrefab;
            case Game.CellType.Cross:
                return CrossCellPrefab;
        }

        return null;
    }

    private Vector3 GetCellPosition(int row, int col)
    {
        const int startX = -30;
        const int startY = 160;
        const int offsetX = 120;
        const int offsetY = 120;

        return new Vector3(startX + col * offsetX, startY - row * offsetY, 0);
    }

    private UnityAction GetCellOnClick(int row, int col)
    {
        switch (_game.Board[row, col])
        {
            case Game.CellType.Blank:
                return () => { Play(row, col); };

            case Game.CellType.Circle:
                return () => { };

            case Game.CellType.Cross:
                return () => { };
        }

        return null;
    }

    public void Play(int row, int col)
    {
        _game.SetCell(row, col, _game.CurrentPlayer);

        CreateCell(row, col);

        Save(_game);

        UpdateStateText();
    }

    private void UpdateStateText()
    {
        switch(_game.State)
        {
            case Game.GameState.Running:
                StateText.text = $"Turn: {_game.CurrentPlayer}";
                break;

            case Game.GameState.Ended:
                if (_game.Winner != null)
                {
                    StateText.text = $"Winner: {_game.Winner}";
                }
                else
                {
                    StateText.text = "Tied.";
                }
                break;

            default:
                StateText.text = string.Empty;
                break;
        }
    }

    private void Save(Game game)
    {
        if (SkipRendering)
        {
            return;
        }

        string json = JsonConvert.SerializeObject(game);
        string path = Path.Combine(Application.persistentDataPath, FileName);
        File.WriteAllText(path, json);
    }

    private Game Load()
    {
        if (SkipRendering)
        {
            return null;
        }

        string path = Path.Combine(Application.persistentDataPath, FileName);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<Game>(json);
        }

        return null;
    }
}