using Mastermind.Shared.Types;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Mastermind.Server
{
  class Program
  {
    private static List<GameMatch> _onlineGameMatches;

    static void Main(string[] args)
    {
      Console.WriteLine("Server started.");
      _onlineGameMatches = new List<GameMatch>();
      ListenerLoop();
      Console.ReadLine();
    }

    private static void ListenerLoop()
    {
      IPAddress localAddr = IPAddress.Parse("127.0.0.1");
      TcpListener listener = new TcpListener(localAddr, 8000);
      listener.Start();
      while (true)
      {
        TcpClient client = listener.AcceptTcpClient();
        // Request game mode
        Task.Run(() =>
        {
          Message message = new Message(MessageType.AskGameMode, string.Empty);
          MessageHandler.SendMessage(client, message);
          Message response = MessageHandler.ReadMessage(client);
          SetGameMode(response, client);
        }).ConfigureAwait(false);
      }
    }



    /// <summary>
    /// Games the loop.
    /// </summary>
    private static void GameLoop(GameMatch match)
    {
      while (true)
      {
        try
        {
          Message msgAskPlayer = new Message(MessageType.AskPlayerCombination,
            $"{match.BoardController.GetNumberOfHoles()} colors combination - you have {match.BoardController.RemainingChances} remaining chances.");
          MessageHandler.SendMessage(match.Player1, msgAskPlayer);
          Message response = MessageHandler.ReadMessage(match.Player1);
          ProcessMessage(response, null, match);
        }
        catch (Exception ex)
        { }
      }
    }

    /// <summary>
    /// Processes the message.
    /// </summary>
    /// <param name="message">The message.</param>
    private static void ProcessMessage(Message message, TcpClient client = null, GameMatch match = null)
    {
      MessageType msgType =
        (MessageType)Enum.Parse(typeof(MessageType), message.GetElement<string>("type"));
      switch (msgType)
      {
        case MessageType.Default:
          break;
        case MessageType.SetGameMode:
          SetGameMode(message, client);
          break;
        case MessageType.SetInitialization:
          SetInitialization(message, match);
          break;
        case MessageType.SetSecretCombination:
          SetCombination(message, match);
          break;
        case MessageType.PlayerCombinationEntered:
          EnterCombination(message, match);
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// Sets the game mode.
    /// </summary>
    /// <param name="message">The message.</param>
    private static void SetGameMode(Message message, TcpClient client)
    {
      // choose the right match
      GameMode mode = message.GetElement<GameMode>("gamemode");
      if (mode == GameMode.MultiPlayer)
      {
        GameMatch currentGame = null;
        if (_onlineGameMatches.Count == 0 ||
          _onlineGameMatches[_onlineGameMatches.Count - 1].Status != GameMatch.GameStatus.Uninitialized)
        {
          currentGame = new GameMatch();
          currentGame.Mode = GameMode.MultiPlayer;
          currentGame.Lose += BoardController_Win;
          currentGame.Win += BoardController_Lose;
          _onlineGameMatches.Add(currentGame);
        }
        else currentGame = _onlineGameMatches[_onlineGameMatches.Count - 1];

        // send initialization messages
        if (currentGame.Player1 == null)
        {
          currentGame.Player1 = client;
          try
          {
            AskGameInitialization(client, currentGame);
          }
          catch (Exception ex)
          {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
          }
        }
        else
        {
          currentGame.Player2 = client;
          AskGameSecretCombination(client, currentGame);
        }

        if (currentGame.Player1 != null && currentGame.Player2 != null)
        {
          currentGame.Status = GameMatch.GameStatus.Running;
          GameLoop(currentGame);
        }
      }
      else if (mode == GameMode.SinglePlayer)
      {
        GameMatch singleMatch = new GameMatch();
        singleMatch.Mode = GameMode.SinglePlayer;
        singleMatch.Win += BoardController_Win;
        singleMatch.Lose += BoardController_Lose;
        singleMatch.Player1 = client;
        AskGameInitialization(client, singleMatch);
        GenerateSecretCombination(singleMatch);
        singleMatch.Status = GameMatch.GameStatus.Running;
        GameLoop(singleMatch);
      }
    }

    /// <summary>
    /// Generates the secret combination.
    /// </summary>
    /// <param name="singleMatch">The single match.</param>
    /// <exception cref="NotImplementedException"></exception>
    private static void GenerateSecretCombination(GameMatch match)
    {
      Random rnd = new Random();
      List<BallColor> combination = new List<BallColor>();
      for (int i = 0; i < match.BoardController.GetNumberOfHoles(); i++)
      {
        BallColor color = (BallColor)rnd.Next(1,7);
        combination.Add(color);
      }
      match.BoardController.SetCombination(combination.ToArray());
    }

    /// <summary>
    /// Asks the game initialization.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="client">The client.</param>
    private static void AskGameInitialization(TcpClient client, GameMatch match)
    {
      Console.WriteLine("Asking to the player 1 for configuration...");
      // Initialize Board
      Message boardInitializationMessage = new Message(MessageType.AskInitialization, string.Empty);
      MessageHandler.SendMessage(client, boardInitializationMessage);
      Message response = MessageHandler.ReadMessage(client);
      ProcessMessage(response, null, match);
    }

    /// <summary>
    /// Asks the game secret combination.
    /// </summary>
    /// <param name="client">The client.</param>
    private static void AskGameSecretCombination(TcpClient client, GameMatch match)
    {
      Console.WriteLine("Asking to the player 2 for combination...");
      // Board Combination
      Message boardCombinationMessage = new Message(MessageType.AskSecretCombination, string.Empty);
      boardCombinationMessage.SetElement("holes", match.BoardController.GetNumberOfHoles());
      MessageHandler.SendMessage(client, boardCombinationMessage);
      Message response = MessageHandler.ReadMessage(client);
      ProcessMessage(response, null, match);
    }

    /// <summary>
    /// Enters the combination.
    /// </summary>
    /// <param name="message">The message.</param>
    private static void EnterCombination(Message message, GameMatch match)
    {
      // Notify the combiation status to the player 1
      List<BallColor> combinationEntered = message.GetElementCollection<BallColor>("combination");
      List<BallColor> statusColors = match.BoardController.EnterCombination(combinationEntered.ToArray());
      Message statusMessage = new Message(MessageType.RightColorsStatus, "Holes Status");
      statusMessage.SetElementCollection("combination", "color", statusColors);
      MessageHandler.SendMessage(match.Player1, statusMessage);

      // Notify to the player 2 the combination entered
      if (match.Mode == GameMode.MultiPlayer)
      {
        StringBuilder str = new StringBuilder();
        foreach (var item in combinationEntered) str.Append($"{item}, ");
        str.Remove(str.Length - 2, 2);
        Message msgCombinationEntered = new Message(MessageType.Default, $"Your oponene enter the combination : {str}");
        MessageHandler.SendMessage(match.Player2,  msgCombinationEntered);
      }
    }

    /// <summary>
    /// Sets the initialization.
    /// </summary>
    /// <param name="message">The message.</param>
    private static void SetInitialization(Message message, GameMatch match)
    {
      match.BoardController.SetNumberOfHoles(message.GetElement<int>("holes"));
      match.BoardController.SetNumberOfChances(message.GetElement<int>("chances"));
    }

    /// <summary>
    /// Sets the combination.
    /// </summary>
    /// <param name="message">The message.</param>
    private static void SetCombination(Message message, GameMatch match)
    {
      List<BallColor> colors = message.GetElementCollection<BallColor>("combination");
      match.BoardController.SetCombination(colors.ToArray());
    }

    /// <summary>
    /// Handles the Win event of the BoardController control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    /// <exception cref="System.NotImplementedException"></exception>
    private static void BoardController_Win(object sender, EventArgs e)
    {
      GameMatch match = (GameMatch)sender;
      Message message = new Message(MessageType.GameWin, string.Empty);
      Message messageEnd = new Message(MessageType.GameEnd, "You Win!");
      MessageHandler.SendMessage(match.Player1, message);
      if (match.Mode == GameMode.MultiPlayer)
      {
        MessageHandler.SendMessage(match.Player2, messageEnd);
      }
      ConsoleEx.WriteLine("The Game has Ended", ConsoleColor.Magenta);
    }


    /// <summary>
    /// Handles the Lose event of the BoardController control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    private static void BoardController_Lose(object sender, EventArgs e)
    {
      GameMatch match = (GameMatch)sender;
      Message message = new Message(MessageType.GameEnd, string.Empty);
      Message messageEnd = new Message(MessageType.GameEnd, "You Lose!");
      MessageHandler.SendMessage(match.Player1, messageEnd);
      if (match.Mode == GameMode.MultiPlayer)
      {
        MessageHandler.SendMessage(match.Player2, message);
      }
      ConsoleEx.WriteLine("The Game has Ended", ConsoleColor.Magenta);
    }
  }
}
