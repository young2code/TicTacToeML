using TicTacToe;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MainTrain : MonoBehaviour, IMain
{
    public GameObject CellPrefab;
    public Canvas Canvas;
    public Text StateText;
    public MLAgent Agent0;
    public MLAgent Agent1;
    public bool StopRendering;

    private Game _game;
    private Button[,] _board = new Button[Game.MaxSize, Game.MaxSize];

    // Start is called before the first frame update
    private void Start()
    {
        Agent0.Main = this;
        Agent1.Main = this;

        _game = new Game();

        CreateCells();

        UpdateStateText();

        StartNewGame();
    }

    private void FixedUpdate()
    {
        if (_game.State == Game.GameState.Running)
        {
            if (Agent0.Player == _game.CurrentPlayer)
            {
                Agent0.RequestDecision();
            }
            else
            {
                Agent1.RequestDecision();
            }
        }
        else if (_game.State == Game.GameState.Ended)
        {
            Agent0.FinishEpisode();
            Agent1.FinishEpisode();

            StartNewGame();
        }
    }

    public void StartNewGame()
    {
        _game.Start();

        ResetCells();

        UpdateStateText();
    }

    public Game GetGame()
    {
        return _game;
    }

    private void CreateCells()
    {
        for (int row = 0; row < Game.MaxSize; ++row)
        {
            for (int col = 0; col < Game.MaxSize; ++col)
            {
                CreateCell(row, col);
            }
        }

        void CreateCell(int row, int col)
        {
            GameObject newGameObject = Instantiate(CellPrefab, Canvas.transform);
            Button button = newGameObject.GetComponent<Button>();
            button.onClick.AddListener(GetCellOnClick(row, col));

            _board[row, col] = button;
            button.transform.localPosition = GetCellPosition(row, col);
        }
    }

    private void ResetCells()
    {
        for (int row = 0; row < Game.MaxSize; ++row)
        {
            for (int col = 0; col < Game.MaxSize; ++col)
            {
                UpdateCell(row, col, Game.CellType.Blank);
            }
        }
    }

    private void UpdateCell(int row, int col, Game.CellType cell)
    {
        if (StopRendering)
        {
            return;
        }

        Button button = _board[row, col];

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(GetCellOnClick(row, col));

        Text text = button.GetComponentInChildren<Text>();
        switch (cell)
        {
            case Game.CellType.Blank:
                text.text = string.Empty;
                break;
            case Game.CellType.Circle:
                text.text = "O";
                break;
            case Game.CellType.Cross:
                text.text = "X";
                break;
        }
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
        if (!_game.SetCell(row, col, _game.CurrentPlayer))
        {
            return;
        }

        UpdateCell(row, col, _game.Board[row, col]);

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
}
