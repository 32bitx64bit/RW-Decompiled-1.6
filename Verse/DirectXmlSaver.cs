using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace Verse;

public static class DirectXmlSaver
{
	public static bool IsSimpleTextType(Type type)
	{
		if (!(type == typeof(float)) && !(type == typeof(double)) && !(type == typeof(long)) && !(type == typeof(ulong)) && !(type == typeof(char)) && !(type == typeof(byte)) && !(type == typeof(sbyte)) && !(type == typeof(int)) && !(type == typeof(uint)) && !(type == typeof(bool)) && !(type == typeof(short)) && !(type == typeof(ushort)) && !(type == typeof(string)))
		{
			return type.IsEnum;
		}
		return true;
	}

	public static void SaveDataObject(object obj, string filePath)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			XDocument val = new XDocument();
			XElement val2 = XElementFromObject(obj, obj.GetType());
			((XContainer)val).Add((object)val2);
			val.Save(filePath);
		}
		catch (Exception ex)
		{
			Log.Error("Exception saving data object " + obj.ToStringSafe() + ": " + ex);
			GenUI.ErrorDialog("ProblemSavingFile".Translate(filePath, ex.ToString()));
		}
	}

	public static XElement XElementFromObject(object obj, Type expectedClass)
	{
		return XElementFromObject(obj, expectedClass, expectedClass.Name);
	}

	public static XElement XElementFromObject(object obj, Type expectedType, string nodeName, FieldInfo owningField = null, bool saveDefsAsRefs = false)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Expected O, but got Unknown
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected O, but got Unknown
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected O, but got Unknown
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Expected O, but got Unknown
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Expected O, but got Unknown
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Expected O, but got Unknown
		if (owningField != null && owningField.TryGetAttribute<DefaultValueAttribute>(out var customAttribute) && customAttribute.ObjIsDefault(obj))
		{
			return null;
		}
		if (obj == null)
		{
			XElement val = new XElement(XName.op_Implicit(nodeName));
			val.SetAttributeValue(XName.op_Implicit("IsNull"), (object)"True");
			return val;
		}
		Type type = obj.GetType();
		XElement val2 = new XElement(XName.op_Implicit(nodeName));
		if (IsSimpleTextType(type))
		{
			((XContainer)val2).Add((object)new XText(obj.ToString()));
		}
		else if (saveDefsAsRefs && GenTypes.IsDef(type))
		{
			string defName = ((Def)obj).defName;
			((XContainer)val2).Add((object)new XText(defName));
		}
		else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
		{
			Type expectedType2 = type.GetGenericArguments()[0];
			int num = (int)type.GetProperty("Count").GetValue(obj, null);
			for (int i = 0; i < num; i++)
			{
				object[] index = new object[1] { i };
				XNode val3 = (XNode)(object)XElementFromObject(type.GetProperty("Item").GetValue(obj, index), expectedType2, "li", null, saveDefsAsRefs: true);
				((XContainer)val2).Add((object)val3);
			}
		}
		else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<, >))
		{
			Type expectedType3 = type.GetGenericArguments()[0];
			Type expectedType4 = type.GetGenericArguments()[1];
			foreach (object item in obj as IEnumerable)
			{
				object value = item.GetType().GetProperty("Key").GetValue(item, null);
				object value2 = item.GetType().GetProperty("Value").GetValue(item, null);
				XElement val4 = new XElement(XName.op_Implicit("li"));
				((XContainer)val4).Add((object)XElementFromObject(value, expectedType3, "key", null, saveDefsAsRefs: true));
				((XContainer)val4).Add((object)XElementFromObject(value2, expectedType4, "value", null, saveDefsAsRefs: true));
				((XContainer)val2).Add((object)val4);
			}
		}
		else
		{
			if (type != expectedType)
			{
				XAttribute val5 = new XAttribute(XName.op_Implicit("Class"), (object)GenTypes.GetTypeNameWithoutIgnoredNamespaces(obj.GetType()));
				((XContainer)val2).Add((object)val5);
			}
			foreach (FieldInfo item2 in from f in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				orderby f.MetadataToken
				select f)
			{
				try
				{
					XElement val6 = XElementFromField(item2, obj);
					if (val6 != null)
					{
						((XContainer)val2).Add((object)val6);
					}
				}
				catch
				{
					throw;
				}
			}
		}
		return val2;
	}

	private static XElement XElementFromField(FieldInfo fi, object owningObj)
	{
		if (Attribute.IsDefined(fi, typeof(UnsavedAttribute)))
		{
			return null;
		}
		return XElementFromObject(fi.GetValue(owningObj), fi.FieldType, fi.Name, fi);
	}
}
