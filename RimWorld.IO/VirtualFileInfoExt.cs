using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace RimWorld.IO;

public static class VirtualFileInfoExt
{
	public static XDocument LoadAsXDocument(this VirtualFile file)
	{
		using Stream input = file.CreateReadStream();
		return XDocument.Load(XmlReader.Create(input), (LoadOptions)4);
	}

	public static XDocument LoadAsXDocument(string fileContents)
	{
		using StringReader stringReader = new StringReader(fileContents);
		return XDocument.Load((TextReader)stringReader, (LoadOptions)4);
	}
}
