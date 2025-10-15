using UnityEngine;
using Verse;

namespace RimWorld;

[StaticConstructorOnStartup]
public class Listing_ResourceReadout : Listing_Tree
{
	private Map map;

	private static Texture2D SolidCategoryBG = SolidColorMaterials.NewSolidColorTexture(new Color(0.1f, 0.1f, 0.1f, 0.6f));

	protected override float LabelWidth => base.ColumnWidth;

	public Listing_ResourceReadout(Map map)
	{
		this.map = map;
	}

	public void DoCategory(TreeNode_ThingCategory node, int nestLevel, int openMask)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		int countIn = map.resourceCounter.GetCountIn(node.catDef);
		if (countIn != 0)
		{
			OpenCloseWidget(node, nestLevel, openMask);
			Rect val = default(Rect);
			((Rect)(ref val))._002Ector(0f, curY, LabelWidth, lineHeight);
			((Rect)(ref val)).xMin = XAtIndentLevel(nestLevel) + 18f;
			Rect val2 = val;
			((Rect)(ref val2)).width = 80f;
			((Rect)(ref val2)).yMax = ((Rect)(ref val2)).yMax - 3f;
			((Rect)(ref val2)).yMin = ((Rect)(ref val2)).yMin + 3f;
			GUI.DrawTexture(val2, (Texture)(object)SolidCategoryBG);
			if (Mouse.IsOver(val))
			{
				GUI.DrawTexture(val, (Texture)(object)TexUI.HighlightTex);
			}
			if (Mouse.IsOver(val))
			{
				TooltipHandler.TipRegion(val, new TipSignal(node.catDef.LabelCap, node.catDef.GetHashCode()));
			}
			Rect val3 = default(Rect);
			((Rect)(ref val3))._002Ector(val);
			float width = (((Rect)(ref val3)).height = 28f);
			((Rect)(ref val3)).width = width;
			((Rect)(ref val3)).y = ((Rect)(ref val)).y + ((Rect)(ref val)).height / 2f - ((Rect)(ref val3)).height / 2f;
			GUI.DrawTexture(val3, (Texture)(object)node.catDef.icon);
			Rect rect = default(Rect);
			((Rect)(ref rect))._002Ector(val);
			((Rect)(ref rect)).xMin = ((Rect)(ref val3)).xMax + 6f;
			Widgets.Label(rect, countIn.ToStringCached());
			EndLine();
			if (IsOpen(node, openMask))
			{
				DoCategoryChildren(node, nestLevel + 1, openMask);
			}
		}
	}

	public void DoCategoryChildren(TreeNode_ThingCategory node, int indentLevel, int openMask)
	{
		foreach (TreeNode_ThingCategory childCategoryNode in node.ChildCategoryNodes)
		{
			if (!childCategoryNode.catDef.resourceReadoutRoot)
			{
				DoCategory(childCategoryNode, indentLevel, openMask);
			}
		}
		foreach (ThingDef childThingDef in node.catDef.childThingDefs)
		{
			if (childThingDef.PlayerAcquirable)
			{
				DoThingDef(childThingDef, indentLevel + 1);
			}
		}
	}

	private void DoThingDef(ThingDef thingDef, int nestLevel)
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		int count = map.resourceCounter.GetCount(thingDef);
		if (count == 0)
		{
			return;
		}
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(0f, curY, LabelWidth, lineHeight);
		((Rect)(ref val)).xMin = XAtIndentLevel(nestLevel) + 18f;
		if (Mouse.IsOver(val))
		{
			GUI.DrawTexture(val, (Texture)(object)TexUI.HighlightTex);
		}
		if (Mouse.IsOver(val))
		{
			TooltipHandler.TipRegion(val, new TipSignal(() => thingDef.LabelCap + ": " + thingDef.description.CapitalizeFirst(), thingDef.shortHash));
		}
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(val);
		float width = (((Rect)(ref rect)).height = 28f);
		((Rect)(ref rect)).width = width;
		((Rect)(ref rect)).y = ((Rect)(ref val)).y + ((Rect)(ref val)).height / 2f - ((Rect)(ref rect)).height / 2f;
		Widgets.ThingIcon(rect, thingDef);
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(val);
		((Rect)(ref rect2)).xMin = ((Rect)(ref rect)).xMax + 6f;
		Widgets.Label(rect2, count.ToStringCached());
		EndLine();
	}
}
