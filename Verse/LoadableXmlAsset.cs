using System;
using System.IO;
using System.Text;
using System.Xml;

namespace Verse;

public class LoadableXmlAsset
{
	public readonly string name;

	public readonly string fullFolderPath;

	public readonly XmlDocument xmlDoc;

	public readonly ModContentPack mod;

	private const int BufferSize = 16000;

	private static readonly XmlReaderSettings Settings = new XmlReaderSettings
	{
		IgnoreComments = true,
		IgnoreWhitespace = true,
		CheckCharacters = false,
		Async = false
	};

	public string FullFilePath
	{
		get
		{
			string text = fullFolderPath;
			char directorySeparatorChar = Path.DirectorySeparatorChar;
			return text + directorySeparatorChar + name;
		}
	}

	public LoadableXmlAsset(string name, string text)
	{
		this.name = name;
		fullFolderPath = string.Empty;
		try
		{
			using StringReader input = new StringReader(text);
			using XmlReader reader = XmlReader.Create(input, Settings);
			xmlDoc = new XmlDocument();
			xmlDoc.Load(reader);
		}
		catch (Exception arg)
		{
			Log.Warning($"Exception reading {name} as XML: {arg}");
			xmlDoc = null;
		}
	}

	public unsafe LoadableXmlAsset(FileInfo file, ModContentPack mod)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		this.mod = mod;
		name = file.Name;
		fullFolderPath = file.Directory.FullName;
		try
		{
			using MemoryMappedFileSpanWrapper memoryMappedFileSpanWrapper = new MemoryMappedFileSpanWrapper(file);
			ReadOnlySpan<byte> val = memoryMappedFileSpanWrapper.GetReadOnlySpan(0L, memoryMappedFileSpanWrapper.FileSize);
			if (val.Length >= 3 && *(byte*)val[0] == 239 && *(byte*)val[1] == 187 && *(byte*)val[2] == 191)
			{
				ReadOnlySpan<byte> val2 = val;
				val = val2.Slice(3, val2.Length - 3);
			}
			using StringReader input = new StringReader(Encoding.UTF8.GetString(val));
			using XmlReader reader = XmlReader.Create(input, Settings);
			xmlDoc = new XmlDocument();
			xmlDoc.Load(reader);
		}
		catch (Exception arg)
		{
			Log.Warning($"Exception reading {name} as XML: {arg}");
			xmlDoc = null;
		}
	}

	public override string ToString()
	{
		return name;
	}
}
