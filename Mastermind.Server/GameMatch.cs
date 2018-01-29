using Mastermind.Shared.Types;
using System;
using System.Net.Sockets;

namespace Mastermind.Server
{
  internal class GameMatch
  {
    public enum GameStatus { Uninitialized, Running, Ended }
    public event EventHandler Win;
    public event EventHandler Lose;
    public void RaiseWin() => Win?.Invoke(this, new EventArgs());
    public void RaiseLose() => Lose?.Invoke(this, new EventArgs());

    private BoardController _boardController;
    private TcpClient _player1;
    private TcpClient _player2;
    private GameStatus _status;
    private GameMode _mode;

    internal BoardController BoardController { get => _boardController; }
    internal GameStatus Status { get => _status; set => _status = value; }
    public TcpClient Player1 { get => _player1; set => _player1 = value; }
    public TcpClient Player2 { get => _player2; set => _player2 = value; }
    public GameMode Mode { get => _mode; set => _mode = value; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GameMatch"/> class.
    /// </summary>
    public GameMatch()
    {
      _boardController = new BoardController();
      _boardController.NoMoreAttemps += _boardController_NoMoreAttemps;
      _boardController.CombinationCorrect += _boardController_CombinationCorrect;
      _status = GameStatus.Uninitialized;
    }

    /// <summary>
    /// Handles the CombinationCorrect event of the _boardController control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
    private void _boardController_CombinationCorrect(object sender, EventArgs e)
    {
      RaiseWin();
    }

    /// <summary>
    /// Handles the NoMoreAttemps event of the _boardController control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    private void _boardController_NoMoreAttemps(object sender, EventArgs e)
    {
      RaiseLose();
    }
  }
}
