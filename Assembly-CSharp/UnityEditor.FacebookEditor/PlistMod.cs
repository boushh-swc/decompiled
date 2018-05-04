using System;
using System.IO;
using System.Xml;
using UnityEngine;

namespace UnityEditor.FacebookEditor
{
	public class PlistMod
	{
		private static XmlNode FindPlistDictNode(XmlDocument doc)
		{
			for (XmlNode xmlNode = doc.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
			{
				if (xmlNode.Name.Equals("plist") && xmlNode.ChildNodes.Count == 1)
				{
					XmlNode firstChild = xmlNode.FirstChild;
					if (firstChild.Name.Equals("dict"))
					{
						return firstChild;
					}
				}
			}
			return null;
		}

		private static XmlElement AddChildElement(XmlDocument doc, XmlNode parent, string elementName, string innerText = null)
		{
			XmlElement xmlElement = doc.CreateElement(elementName);
			if (!string.IsNullOrEmpty(innerText))
			{
				xmlElement.InnerText = innerText;
			}
			parent.AppendChild(xmlElement);
			return xmlElement;
		}

		private static bool HasKey(XmlNode dict, string keyName)
		{
			for (XmlNode xmlNode = dict.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
			{
				if (xmlNode.Name.Equals("key") && xmlNode.InnerText.Equals(keyName))
				{
					return true;
				}
			}
			return false;
		}

		public static void UpdatePlist(string path, string appId, string[] allPossibleAppIds)
		{
			string text = Path.Combine(path, "Info.plist");
			if (string.IsNullOrEmpty(appId) || appId.Equals("0"))
			{
				Debug.LogError("You didn't specify a Facebook app ID.  Please add one using the Facebook menu in the main Unity editor.");
				return;
			}
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(text);
			XmlNode xmlNode = PlistMod.FindPlistDictNode(xmlDocument);
			if (xmlNode == null)
			{
				Debug.LogError("Error parsing " + text);
				return;
			}
			if (!PlistMod.HasKey(xmlNode, "FacebookAppID"))
			{
				PlistMod.AddChildElement(xmlDocument, xmlNode, "key", "FacebookAppID");
				PlistMod.AddChildElement(xmlDocument, xmlNode, "string", appId);
			}
			if (!PlistMod.HasKey(xmlNode, "CFBundleURLTypes"))
			{
				PlistMod.AddChildElement(xmlDocument, xmlNode, "key", "CFBundleURLTypes");
				XmlElement parent = PlistMod.AddChildElement(xmlDocument, xmlNode, "array", null);
				XmlElement parent2 = PlistMod.AddChildElement(xmlDocument, parent, "dict", null);
				PlistMod.AddChildElement(xmlDocument, parent2, "key", "CFBundleURLSchemes");
				XmlElement parent3 = PlistMod.AddChildElement(xmlDocument, parent2, "array", null);
				for (int i = 0; i < allPossibleAppIds.Length; i++)
				{
					string str = allPossibleAppIds[i];
					PlistMod.AddChildElement(xmlDocument, parent3, "string", "fb" + str);
				}
			}
			xmlDocument.Save(text);
			StreamReader streamReader = new StreamReader(text);
			string text2 = streamReader.ReadToEnd();
			streamReader.Close();
			int num = text2.IndexOf("<!DOCTYPE plist PUBLIC", StringComparison.Ordinal);
			if (num <= 0)
			{
				return;
			}
			int num2 = text2.IndexOf('>', num);
			if (num2 <= 0)
			{
				return;
			}
			string text3 = text2.Substring(0, num);
			text3 += "<!DOCTYPE plist PUBLIC \"-//Apple//DTD PLIST 1.0//EN\" \"http://www.apple.com/DTDs/PropertyList-1.0.dtd\">";
			text3 += text2.Substring(num2 + 1);
			StreamWriter streamWriter = new StreamWriter(text, false);
			streamWriter.Write(text3);
			streamWriter.Close();
		}
	}
}
