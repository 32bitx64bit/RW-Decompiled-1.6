using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Verse;

public static class DirectXmlSaveFormatter
{
	public static void AddWhitespaceFromRoot(XElement root)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Expected O, but got Unknown
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Expected O, but got Unknown
		if (!((XContainer)root).Elements().Any())
		{
			return;
		}
		foreach (XElement item in ((XContainer)root).Elements().ToList())
		{
			XText val = new XText("\n");
			((XNode)item).AddAfterSelf((object)val);
		}
		((XNode)((XContainer)root).Elements().First()).AddBeforeSelf((object)new XText("\n"));
		((XNode)((XContainer)root).Elements().Last()).AddAfterSelf((object)new XText("\n"));
		foreach (XElement item2 in ((XContainer)root).Elements().ToList())
		{
			IndentXml(item2, 1);
		}
	}

	private static void IndentXml(XElement element, int depth)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		((XNode)element).AddBeforeSelf((object)new XText(IndentString(depth, startWithNewline: true)));
		bool startWithNewline = ((XNode)element).NextNode == null;
		((XNode)element).AddAfterSelf((object)new XText(IndentString(depth - 1, startWithNewline)));
		foreach (XElement item in ((XContainer)element).Elements().ToList())
		{
			IndentXml(item, depth + 1);
		}
	}

	private static string IndentString(int depth, bool startWithNewline)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (startWithNewline)
		{
			stringBuilder.Append("\n");
		}
		for (int i = 0; i < depth; i++)
		{
			stringBuilder.Append("  ");
		}
		return stringBuilder.ToString();
	}
}
