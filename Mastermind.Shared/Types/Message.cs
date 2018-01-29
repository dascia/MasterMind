using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Mastermind.Shared.Types
{
  public class Message
  {
    private XDocument _doc;

    /// <summary>
    /// Initializes a new instance of the <see cref="Message"/> class.
    /// </summary>
    public Message() : this(MessageType.Default, string.Empty)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Message"/> class.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="description">The description.</param>
    public Message(MessageType type, string description)
    {
      _doc = new XDocument();
      XElement elementRoot = new XElement("message");
      XElement elementDescription = new XElement("description", description);
      XElement elementType = new XElement("type", type.ToString());
      elementRoot.Add(elementDescription);
      elementRoot.Add(elementType);
      _doc.Add(elementRoot);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Message"/> class.
    /// </summary>
    /// <param name="doc">The document.</param>
    public Message(string doc)
    {
      _doc = XDocument.Parse(doc);
    }

    /// <summary>
    /// Set the value of a field, if the field doesn't exists it will create it.
    /// </summary>
    /// <param name="name">The property.</param>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public void SetElement<T>(string name, T value)
    {
      XElement element = _doc.Element("message").Elements().FirstOrDefault(e => e.Name == name);
      if (element == null) element = new XElement(name);
      element.SetValue(value);
      _doc.Element("message").Add(element);
    }

    /// <summary>
    /// Gets the element.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="elementName">Name of the element.</param>
    /// <returns></returns>
    public T GetElement<T>(string elementName)
    {
      XElement element = _doc.Element("message").Elements().FirstOrDefault(e => e.Name == elementName);
      if (element == null) return default(T);
      T elementValue = default(T);
      if (typeof(T).IsEnum)
      {
        elementValue = (T)Enum.Parse(typeof(T), element.Value);
      }
      else
      {
        elementValue = (T)Convert.ChangeType(element.Value, typeof(T));
      }
      return elementValue;
    }

    /// <summary>
    /// Gets the collection of elements
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collectionName">Name of the collection.</param>
    /// <returns></returns>
    public List<T> GetElementCollection<T>(string collectionName)
    {
      XElement element = _doc.Element("message").Elements().FirstOrDefault(e => e.Name == collectionName);
      if (element == null) return null;
      List<T> elements = new List<T>();
      foreach (XElement children in element.Elements())
      {
        if (typeof(T).IsEnum)
        {
          elements.Add((T)Enum.Parse(typeof(T), children.Value));
        }
        else
        {
          elements.Add((T)Convert.ChangeType(children.Value, typeof(T)));
        }
      }
      return elements;
    }

    /// <summary>
    /// Set a element collection with a respective value for each of one.
    /// </summary>
    /// <param name="collectionName">Name of the collection.</param>
    /// <param name="elementName">Name of the element.</param>
    /// <param name="values">The values.</param>
    public void SetElementCollection<T>(string collectionName, string elementName, List<T> values)
    {
      XElement element = _doc.Element("message").Elements().FirstOrDefault(e => e.Name == collectionName);
      if (element == null) element = new XElement(collectionName);
      element.RemoveAll();
      foreach (T value in values)
      {
        XElement tempElement = new XElement(elementName, value);
        element.Add(tempElement);
      }
      _doc.Element("message").Add(element);
    }

    /// <summary>
    /// Serializes this instance.
    /// </summary>
    /// <returns></returns>
    public byte[] BuildPacket()
    {
      string xml = _doc.ToString();
      List<byte> packet = new List<byte>();
      byte[] data = Encoding.ASCII.GetBytes(xml);
      byte[] size = BitConverter.GetBytes(data.Length);
      packet.AddRange(size);
      packet.AddRange(data);
      return packet.ToArray();
    }
  }
}
