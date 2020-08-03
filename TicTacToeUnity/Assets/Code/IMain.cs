using TicTacToe;

public interface IMain
{
    Game GetGame();
    void Play(int row, int col);
}