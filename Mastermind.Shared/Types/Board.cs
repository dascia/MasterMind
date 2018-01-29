using System.Net.Sockets;

namespace Mastermind.Shared.Types
{
  // TODO: This class must not be shared, it must be present just in the server
  public class Board
  {
    private BallColor[] _secretCombination;
    private int _holes;
    private int _chances;

    public BallColor[] SecretCombination { get => _secretCombination; set => _secretCombination = value; }
    public int Holes { get => _holes; set => _holes = value; }
    public int Chances { get => _chances; set => _chances = value; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Board"/> class.
    /// </summary>
    public Board()
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Board"/> class.
    /// </summary>
    /// <param name="holes">The holes.</param>
    /// <param name="chances">The chances.</param>
    /// <param name="secretCombination">The secret combination.</param>
    /// <param name="player1">The player1.</param>
    /// <param name="player2">The player2.</param>
    public Board(int holes, int chances, BallColor[] secretCombination)
    {
      _secretCombination = secretCombination;
      _holes = holes;
      _chances = chances;
    }
  }
}
