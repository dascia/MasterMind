using Mastermind.Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mastermind.Server
{
  class BoardController
  {
    // event fired when the player set a right combination
    public event EventHandler CombinationCorrect;
    public event EventHandler NoMoreAttemps;
    public void RaiseCombinationCorrect() => CombinationCorrect?.Invoke(this, new EventArgs());
    public void RaiseNoMoreAttemps() => NoMoreAttemps?.Invoke(this, new EventArgs());

    // fields
    private Board _board;
    private int _remainingChances;
    public int RemainingChances { get => _remainingChances; }


    /// <summary>
    /// Initializes a new instance of the <see cref="BoardController"/> class.
    /// </summary>
    public BoardController()
    {
      _board = new Board(4, 8, new[] { BallColor.White, BallColor.White, BallColor.White, BallColor.White, });
      _remainingChances = 8;
    }

    /// <summary>
    /// Sets the number of chances.
    /// </summary>
    /// <param name="chances">The chances.</param>
    public void SetNumberOfChances(int chances)
    {
      _board.Chances = chances;
      _remainingChances = chances;
    }

    /// <summary>
    /// Sets the number of holes.
    /// </summary>
    /// <param name="holes">The holes.</param>
    public void SetNumberOfHoles(int holes)
    {
      _board.Holes = holes;
    }

    /// <summary>
    /// Gets the number of holes.
    /// </summary>
    /// <returns></returns>
    public int GetNumberOfHoles()
    {
      return _board.Holes;
    }

    /// <summary>
    /// Sets the combination.
    /// </summary>
    /// <param name="balls">The balls.</param>
    public void SetCombination(BallColor[] balls)
    {
      _board.SecretCombination = balls;
    }

    /// <summary>
    /// Enters the combination and return the board status.
    /// </summary>
    /// <param name="combination">The combination.</param>
    /// <param name="winStatus">if set to <c>true</c> [win status].</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentOutOfRangeException">Wrong number of balls</exception>
    public List<BallColor> EnterCombination(BallColor[] combination)
    {
      if (combination.Length > _board.SecretCombination.Length)
        throw new ArgumentOutOfRangeException("Wrong number of balls");
      List<BallColor> ballStatus = new List<BallColor>();
      for (int i = 0; i < _board.SecretCombination.Length; ++i)
      {
        if (_board.SecretCombination[i] == combination[i])
        {
          ballStatus.Add(BallColor.Red);
          continue;
        }
        if (_board.SecretCombination.Any(b => b == combination[i]))
        {
          ballStatus.Add(BallColor.White);
        }
      }
      if (ballStatus.Count(b => b == BallColor.Red) == _board.SecretCombination.Length) RaiseCombinationCorrect();
      if (--_remainingChances == 0) RaiseNoMoreAttemps();
      return ballStatus;
    }

    /// <summary>
    /// Returns a <see cref="System.String" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
      StringBuilder str = new StringBuilder();
      str.AppendLine($"Holes: {_board.Holes}");
      str.AppendLine($"Chances: {_board.Chances}");
      str.AppendLine($"Combination: ");
      foreach (var item in _board.SecretCombination) str.Append($"{item}, ");
      str.Remove(str.Length - 2, 2);
      return str.ToString();
    }
  }
}
