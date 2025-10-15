using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Verse;

namespace LudeonTK;

internal class EditWindow_DefEditor : EditWindow
{
	public Def def;

	public TreeNode_Editor cleanCopy;

	private float viewHeight;

	private Vector2 scrollPosition;

	private float labelColumnWidth = 140f;

	private const float TopAreaHeight = 16f;

	private const float ExtraScrollHeight = 200f;

	public override Vector2 InitialSize => new Vector2(700f, 600f);

	public override bool IsDebug => true;

	public EditWindow_DefEditor(Def def)
	{
		this.def = def;
		cleanCopy = EditTreeNodeDatabase.RootOf(DeepClone(def));
		closeOnAccept = false;
		closeOnCancel = false;
		optionalTitle = def.ToString();
	}

	public void InstanceEffecter()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		Effecter effecter = ((EffecterDef)def).Spawn();
		IntVec3 cell = UI.UIToMapPosition(Vector2.op_Implicit(Find.Camera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f)))).ToIntVec3();
		effecter.Trigger(new TargetInfo(cell, Find.CurrentMap), new TargetInfo(cell, Find.CurrentMap));
		effecter.Cleanup();
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Invalid comparison between Unknown and I4
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Invalid comparison between Unknown and I4
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Invalid comparison between Unknown and I4
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Invalid comparison between Unknown and I4
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Invalid comparison between Unknown and I4
		if ((int)Event.current.type == 4 && ((int)Event.current.keyCode == 13 || (int)Event.current.keyCode == 271 || (int)Event.current.keyCode == 27))
		{
			UI.UnfocusCurrentControl();
		}
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(0f, 0f, ((Rect)(ref inRect)).width, 16f);
		labelColumnWidth = Widgets.HorizontalSlider(rect, labelColumnWidth, 0f, ((Rect)(ref inRect)).width);
		Rect outRect = inRect.AtZero();
		((Rect)(ref outRect)).yMin = ((Rect)(ref outRect)).yMin + 16f;
		if (def is EffecterDef)
		{
			float x = ((Rect)(ref inRect)).x;
			DoRowButton(ref x, ((Rect)(ref outRect)).yMin, "Spawn Effecter", "spawn the effecter in the world", InstanceEffecter);
			DoRowButton(ref x, ((Rect)(ref outRect)).yMin, "Log Diff", "Diff the in-memory Def with the original", PrintDirty);
			((Rect)(ref outRect)).yMin = ((Rect)(ref outRect)).yMin + 24f;
		}
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(0f, 0f, ((Rect)(ref outRect)).width - 16f, viewHeight);
		Widgets.BeginScrollView(outRect, ref scrollPosition, val);
		Listing_TreeDefs listing_TreeDefs = new Listing_TreeDefs(labelColumnWidth);
		listing_TreeDefs.Begin(val);
		TreeNode_Editor node = EditTreeNodeDatabase.RootOf(def);
		listing_TreeDefs.ContentLines(node, 0);
		listing_TreeDefs.End();
		if ((int)Event.current.type == 8)
		{
			viewHeight = listing_TreeDefs.CurHeight + 200f;
		}
		Widgets.EndScrollView();
	}

	public override void PreClose()
	{
		base.PreClose();
	}

	public void PrintDirty()
	{
		Dictionary<string, string> dictionary = Compare(cleanCopy, EditTreeNodeDatabase.RootOf(def));
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("DirtyValues For " + def.defName);
		foreach (KeyValuePair<string, string> item in dictionary)
		{
			stringBuilder.AppendLine(item.Key + ":" + item.Value);
		}
		Log.Message(stringBuilder.ToString());
	}

	private Dictionary<string, string> Compare(TreeNode_Editor fromTree, TreeNode_Editor toTree)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		List<TreeNode_Editor> list = Flatten(fromTree, new List<TreeNode>()).Cast<TreeNode_Editor>().ToList();
		List<TreeNode_Editor> list2 = Flatten(toTree, new List<TreeNode>()).Cast<TreeNode_Editor>().ToList();
		int num = ((list.Count < list2.Count) ? list.Count : list2.Count);
		for (int i = 0; i < num; i++)
		{
			TreeNode_Editor treeNode_Editor = list[i];
			TreeNode_Editor treeNode_Editor2 = list2[i];
			if (treeNode_Editor.HasValue && treeNode_Editor2.HasValue && treeNode_Editor.Value != null && treeNode_Editor2.Value != null && treeNode_Editor.Value.ToString() != treeNode_Editor2.Value.ToString())
			{
				dictionary[$"{treeNode_Editor.LabelText}_{treeNode_Editor.GetHashCode()}"] = $"{treeNode_Editor.Value} -> {treeNode_Editor2.Value}";
			}
		}
		return dictionary;
	}

	private List<TreeNode> Flatten(TreeNode tree, List<TreeNode> treeNodes)
	{
		treeNodes.Add(tree);
		if (tree.children != null)
		{
			foreach (TreeNode child in tree.children)
			{
				Flatten(child, treeNodes);
			}
		}
		return treeNodes;
	}

	public static object DeepClone(object objSource)
	{
		Type type = objSource.GetType();
		object obj = Activator.CreateInstance(type);
		PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		foreach (PropertyInfo propertyInfo in properties)
		{
			if (!propertyInfo.CanWrite)
			{
				continue;
			}
			if (propertyInfo.PropertyType.IsValueType || propertyInfo.PropertyType.IsEnum || propertyInfo.PropertyType == typeof(string))
			{
				propertyInfo.SetValue(obj, propertyInfo.GetValue(objSource, null), null);
				continue;
			}
			object value = propertyInfo.GetValue(objSource, null);
			if (value == null)
			{
				propertyInfo.SetValue(obj, null, null);
			}
			else
			{
				propertyInfo.SetValue(obj, DeepClone(value), null);
			}
		}
		return obj;
	}
}
