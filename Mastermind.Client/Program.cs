using Mastermind.Shared.Types;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Linq;

namespace Mastermind.Client
{
  class Program
  {
    private static TcpClient _client;

    static void Main(string[] args)
    {
      _client = new TcpClient("127.0.0.1", 8000);
      if (_client.Connected == false) return;
      Console.WriteLine("==== Mastermind Client ====");
      while (true)
      {
        Message message = MessageHandler.ReadMessage(_client);
        ProcessMessage(message);
      }
    }

    /// <summary>
    /// Processes the message.
    /// </summary>
    /// <param name="message">The message.</param>
    private static void ProcessMessage(Message message)
    {
      MessageType msgType = 
        (MessageType) Enum.Parse(typeof(MessageType), message.GetElement<string>("type"));
      switch (msgType)
      {
        case MessageType.Default:
          OnDefault(message);
          break;
        case MessageType.AskGameMode:
          AskGameMode(message);
          break;
        case MessageType.AskInitialization:
          BoardInitialization(message);
          break;
        case MessageType.AskSecretCombination:
          SetBoardCombination(message);
          break;
        case MessageType.AskPlayerCombination:
          AskPlayerCombination(message);
          break;
        case MessageType.RightColorsStatus:
          OnRightColorStatus(message);
          break;
        case MessageType.GameWin:
          GameWin(message);
          break;
        case MessageType.GameEnd:
          GameEnd(message);
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// Asks the game mode.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <exception cref="NotImplementedException"></exception>
    private static void AskGameMode(Message message)
    {
      Console.WriteLine("1. Player vs Computer");
      Console.WriteLine("2. Player vs Player");
      Console.WriteLine();
      Console.Write("Please enter an option: ");
      GameMode gameMode = (GameMode)int.Parse(Console.ReadLine());
      Message gameModeMessage = new Message(MessageType.SetGameMode, string.Empty);
      gameModeMessage.SetElement("gamemode", gameMode);
      MessageHandler.SendMessage(_client, gameModeMessage);
    }

    /// <summary>
    /// Asks the player combination and send the combination to the server.
    /// </summary>
    /// <param name="message">The message.</param>
    private static void AskPlayerCombination(Message message)
    {
      Console.WriteLine(message.GetElement<string>("description"));
      Console.WriteLine();
      //public enum BallColor { Red = 1, Green = 2, Yellow = 3, Blue = 4, Black = 5, White = 6 }
      List<BallColor> ballColors = GetColorCombination();
      Message playerEnterCombination = new Message(MessageType.PlayerCombinationEntered, string.Empty);
      playerEnterCombination.SetElementCollection("combination", "color", ballColors);
      MessageHandler.SendMessage(_client, playerEnterCombination);
    }

    /// <summary>
    /// Display how many right colors have the board in the attempt
    /// </summary>
    /// <param name="message">The message.</param>
    private static void OnRightColorStatus(Message message)
    {
      List<BallColor> colors = message.GetElementCollection<BallColor>("combination");
      int reds = colors.Count(c => c == BallColor.Red);
      int whites = colors.Count(c => c == BallColor.White);
      ConsoleEx.WriteLine("");
      ConsoleEx.WriteLine("===============================");
      ConsoleEx.WriteLine($"You have {reds} colors in the right hole.");
      ConsoleEx.WriteLine($"You have {whites} colors that belongs to the right combination");
      ConsoleEx.WriteLine("===============================");
      ConsoleEx.WriteLine("");
    }

    /// <summary>
    /// Games the end.
    /// </summary>
    /// <param name="message">The message.</param>
    private static void GameWin(Message message)
    {
      Console.WriteLine("You Win!");
      Console.ReadLine();
      Environment.Exit(0);
    }

    /// <summary>
    /// Games the end.
    /// </summary>
    /// <param name="message">The message.</param>
    private static void GameEnd(Message message)
    {
      Console.WriteLine("You Lose!");
      Console.ReadLine();
      Environment.Exit(0);
    }

    /// <summary>
    /// Called when [default].
    /// </summary>
    /// <param name="message">The message.</param>
    private static void OnDefault(Message message)
    {
      Console.WriteLine(message.GetElement<string>("description"));
    }

    /// <summary>
    /// Boards the initialization.
    /// </summary>
    private static void BoardInitialization(Message message)
    {
      Console.WriteLine("Please enter the Mastermind settings:");
      Console.WriteLine();
      Console.Write("Enter the number of holes (columns): ");
      int.TryParse(Console.ReadLine(), out int holes);
      Console.Write("Enter the number of attempts (height): ");
      int.TryParse(Console.ReadLine(), out int chances);
      Message msg = new Message(MessageType.SetInitialization, "Board Initialization");
      msg.SetElement("holes", holes);
      msg.SetElement("chances", chances);
      MessageHandler.SendMessage(_client, msg);
    }

    /// <summary>
    /// Sets the board combination.
    /// </summary>
    /// <param name="message">The message.</param>
    private static void SetBoardCombination(Message message)
    {
      int holes = message.GetElement<int>("holes");
      ConsoleEx.Write("Please, enter a secret combination of ");
      ConsoleEx.Write($"{holes} ", ConsoleColor.Green);
      ConsoleEx.WriteLine("colors");
      List<BallColor> colors = GetColorCombination();
      Message msg = new Message(MessageType.SetSecretCombination, "Board Combination Initialization");
      msg.SetElementCollection("combination", "color", colors);
      MessageHandler.SendMessage(_client, msg);
    }

    /// <summary>
    /// Colors the combination.
    /// </summary>
    /// <returns></returns>
    private static List<BallColor> GetColorCombination()
    {
      ConsoleEx.WriteLine("1. Red", ConsoleColor.Red);
      ConsoleEx.WriteLine("2. Green", ConsoleColor.Green);
      ConsoleEx.WriteLine("3. Yellow", ConsoleColor.Yellow);
      ConsoleEx.WriteLine("4. Blue", ConsoleColor.Blue);
      ConsoleEx.WriteLine("5. Black", ConsoleColor.DarkGray);
      ConsoleEx.WriteLine("6. White", ConsoleColor.White);
      Console.WriteLine();
      ConsoleEx.WriteLine("Example: 1,2,2,6");
      Console.WriteLine();
      Console.Write("Please, enter your combination: ");
      string strColors = Console.ReadLine();
      string[] colors = strColors.Split(',');
      List<BallColor> ballColors = new List<BallColor>();
      for (int i = 0; i < colors.Length; ++i)
      {
        ballColors.Add((BallColor)int.Parse(colors[i]));
      }
      return ballColors;
    }
  }
}
