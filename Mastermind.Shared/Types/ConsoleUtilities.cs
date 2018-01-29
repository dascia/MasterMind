using System;

namespace Mastermind.Shared.Types
{
  public class ConsoleEx
  {
    /// <summary>
    /// Write to the console a formatted text
    /// </summary>
    /// <param name="color">The color.</param>
    /// <param name="message">The message.</param>
    /// <param name="alignment">The alignment.</param>
    public static void WriteLine(string message, ConsoleColor color = ConsoleColor.White, 
      Alignment alignment = Alignment.Left)
    {
      if (alignment == Alignment.Center)
        Console.SetCursorPosition((Console.WindowWidth - message.Length) /2  , Console.CursorTop);
      Console.ForegroundColor = color;
      Console.WriteLine(message);
      Console.ResetColor();
    }

    /// <summary>
    /// Write to the console a formatted text
    /// </summary>
    /// <param name="color">The color.</param>
    /// <param name="message">The message.</param>
    /// <param name="alignment">The alignment.</param>
    public static void Write(string message, ConsoleColor color = ConsoleColor.White,
      Alignment alignment = Alignment.Left)
    {
      if (alignment == Alignment.Center)
        Console.SetCursorPosition((Console.WindowWidth - message.Length) / 2, Console.CursorTop);
      Console.ForegroundColor = color;
      Console.Write(message);
      Console.ResetColor();
    }

    /// <summary>
    /// Text alignment
    /// </summary>
    public enum Alignment { Left, Center }
  }
}
