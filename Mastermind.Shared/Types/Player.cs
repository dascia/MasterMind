using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Mastermind.Shared.Types
{
  public class Player
  {
    private TcpClient _tcpClient;


    /// <summary>
    /// Initializes a new instance of the <see cref="Player"/> class.
    /// </summary>
    /// <param name="tcpClient">The TCP client.</param>
    public Player(TcpClient tcpClient)
    {
      _tcpClient = tcpClient;
    }
  }
}
