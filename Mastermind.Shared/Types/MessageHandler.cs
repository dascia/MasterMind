using System;
using System.Net.Sockets;
using System.Text;

namespace Mastermind.Shared.Types
{
  public class MessageHandler
  {
    /// <summary>
    /// Sends the message.
    /// </summary>
    /// <param name="msg">The MSG.</param>
    public static void SendMessage(TcpClient tcpClient, Message message)
    {
      NetworkStream ns = tcpClient.GetStream();
      byte[] data = message.BuildPacket();
      ns.Write(data, 0, data.Length);
    }

    /// <summary>
    /// Sends the text.
    /// </summary>
    /// <param name="message">The message.</param>
    public static void SendText(TcpClient tcpClient, string text)
    {
      Message message = new Message(MessageType.Default, text);
      byte[] data = message.BuildPacket();
      tcpClient.GetStream().Write(data, 0, data.Length);
    }

    /// <summary>
    /// Sends the text.
    /// </summary>
    /// <param name="message">The message.</param>
    public static string ReceiveText(TcpClient tcpClient)
    {
      NetworkStream ns = tcpClient.GetStream();
      byte[] bSize = new byte[4];
      ns.Read(bSize, 0, 4);
      int packetSize = BitConverter.ToInt32(bSize, 0);
      byte[] bMessage = new byte[packetSize];
      ns.Read(bMessage, 0, packetSize);
      string message = Encoding.ASCII.GetString(bMessage);
      return message;
    }

    /// <summary>
    /// Reads the message.
    /// </summary>
    public static Message ReadMessage(TcpClient tcpClient)
    {
      NetworkStream ns = tcpClient.GetStream();
      byte[] bSize = new byte[4];
      ns.Read(bSize, 0, 4);
      int packetSize = BitConverter.ToInt32(bSize, 0);
      byte[] bXml = new byte[packetSize];
      ns.Read(bXml, 0, packetSize);
      string xml = Encoding.ASCII.GetString(bXml);
      return new Message(xml);
    }
  }
}
