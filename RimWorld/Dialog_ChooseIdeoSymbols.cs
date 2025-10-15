using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

public class Dialog_ChooseIdeoSymbols : Window
{
	private Ideo ideo;

	private string newName;

	private string newAdjective;

	private string newMemberName;

	private string newWorshipRoomLabel;

	private IdeoIconDef newIconDef;

	private ColorDef newColorDef;

	private Vector2 scrollPos;

	private float viewHeight;

	private static List<ColorDef> allColors;

	private const int IconSize = 40;

	private const int IconPadding = 5;

	private const int IconMargin = 5;

	private const int ColorSize = 22;

	private const int ColorPadding = 2;

	private static readonly Vector2 ButSize = new Vector2(150f, 38f);

	private static readonly Color IconColor = new Color(0.95f, 0.95f, 0.95f);

	private static readonly float EditFieldHeight = 30f;

	private static readonly float ResetButtonWidth = 120f;

	private static readonly Regex ValidSymbolRegex = new Regex("^[\\p{L}0-9 '\\-]*$");

	private const int MaxSymbolLength = 40;

	public override Vector2 InitialSize => new Vector2(740f, 700f);

	private static List<ColorDef> IdeoColorsSorted
	{
		get
		{
			if (allColors == null)
			{
				allColors = new List<ColorDef>();
				allColors.AddRange(DefDatabase<ColorDef>.AllDefsListForReading.Where((ColorDef x) => x.colorType == ColorType.Ideo));
				allColors.SortByColor((ColorDef x) => x.color);
			}
			return allColors;
		}
	}

	public Dialog_ChooseIdeoSymbols(Ideo ideo)
	{
		this.ideo = ideo;
		absorbInputAroundWindow = true;
		newName = ideo.name;
		newAdjective = ideo.adjective;
		newMemberName = ideo.memberName;
		newWorshipRoomLabel = ideo.WorshipRoomLabel;
		newIconDef = ideo.iconDef;
		newColorDef = ideo.colorDef;
	}

	public override void OnAcceptKeyPressed()
	{
		TryAccept();
		Event.current.Use();
	}

	public override void DoWindowContents(Rect rect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_038f: Unknown result type (might be due to invalid IL or missing references)
		Rect val = rect;
		((Rect)(ref val)).height = ((Rect)(ref val)).height - Window.CloseButSize.y;
		Text.Font = GameFont.Medium;
		Widgets.Label(new Rect(((Rect)(ref val)).x, ((Rect)(ref val)).y, ((Rect)(ref rect)).width, 35f), "EditSymbols".Translate());
		Text.Font = GameFont.Small;
		((Rect)(ref val)).yMin = ((Rect)(ref val)).yMin + 45f;
		float y = ((Rect)(ref val)).y;
		float num = ((Rect)(ref val)).x + ((Rect)(ref val)).width / 3f;
		float num2 = ((Rect)(ref val)).xMax - num - ResetButtonWidth - 10f;
		float curY = y;
		Widgets.Label(((Rect)(ref val)).x, ref curY, ((Rect)(ref val)).width, "Name".Translate());
		newName = Widgets.TextField(new Rect(num, y, num2, EditFieldHeight), newName, 40, ValidSymbolRegex);
		y += EditFieldHeight + 10f;
		float curY2 = y;
		Widgets.Label(((Rect)(ref val)).x, ref curY2, ((Rect)(ref val)).width, "Adjective".Translate());
		newAdjective = Widgets.TextField(new Rect(num, y, num2, EditFieldHeight), newAdjective, 40, ValidSymbolRegex);
		y += EditFieldHeight + 10f;
		float curY3 = y;
		Widgets.Label(((Rect)(ref val)).x, ref curY3, ((Rect)(ref val)).width, "IdeoMembers".Translate());
		newMemberName = Widgets.TextField(new Rect(num, y, num2, EditFieldHeight), newMemberName, 40, ValidSymbolRegex);
		y += EditFieldHeight + 10f;
		float curY4 = y;
		Widgets.Label(((Rect)(ref val)).x, ref curY4, ((Rect)(ref val)).width, "WorshipRoom".Translate());
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(num, y, num2, EditFieldHeight);
		Rect rect3 = new Rect(((Rect)(ref rect2)).xMax + 10f, y, ResetButtonWidth, EditFieldHeight);
		newWorshipRoomLabel = Widgets.TextField(rect2, newWorshipRoomLabel, 40, ValidSymbolRegex);
		if (Widgets.ButtonText(rect3, "Reset".Translate()))
		{
			SoundDefOf.Click.PlayOneShotOnCamera();
			ideo.WorshipRoomLabel = null;
			newWorshipRoomLabel = ideo.WorshipRoomLabel;
		}
		y += EditFieldHeight + 10f;
		Rect mainRect = val;
		((Rect)(ref mainRect)).yMax = ((Rect)(ref mainRect)).yMax - 4f;
		Widgets.Label(((Rect)(ref mainRect)).x, ref y, ((Rect)(ref mainRect)).width, "Icon".Translate());
		((Rect)(ref mainRect)).yMin = y;
		DoColorSelector(mainRect, ref y);
		((Rect)(ref mainRect)).yMin = y;
		DoIconSelector(mainRect);
		if (Widgets.ButtonText(new Rect(0f, ((Rect)(ref rect)).height - ButSize.y, ButSize.x, ButSize.y), "Back".Translate()))
		{
			Close();
		}
		if (Widgets.ButtonText(new Rect(((Rect)(ref rect)).width - ButSize.x, ((Rect)(ref rect)).height - ButSize.y, ButSize.x, ButSize.y), "DoneButton".Translate()))
		{
			TryAccept();
		}
	}

	private void DoIconSelector(Rect mainRect)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		int num = 50;
		Rect viewRect = default(Rect);
		((Rect)(ref viewRect))._002Ector(0f, 0f, ((Rect)(ref mainRect)).width - 16f, viewHeight);
		Widgets.BeginScrollView(mainRect, ref scrollPos, viewRect);
		IEnumerable<IdeoIconDef> allDefs = DefDatabase<IdeoIconDef>.AllDefs;
		int num2 = Mathf.FloorToInt(((Rect)(ref viewRect)).width / (float)(num + 5));
		int num3 = allDefs.Count();
		int num4 = 0;
		Rect val = default(Rect);
		foreach (IdeoIconDef item in allDefs)
		{
			int num5 = num4 / num2;
			int num6 = num4 % num2;
			int num7 = ((num4 >= num3 - num3 % num2) ? (num3 % num2) : num2);
			float num8 = (((Rect)(ref viewRect)).width - (float)(num7 * num) - (float)((num7 - 1) * 5)) / 2f;
			((Rect)(ref val))._002Ector(num8 + (float)(num6 * num) + (float)(num6 * 5), (float)(num5 * num + num5 * 5), (float)num, (float)num);
			Widgets.DrawLightHighlight(val);
			Widgets.DrawHighlightIfMouseover(val);
			if (item == newIconDef)
			{
				Widgets.DrawBox(val);
			}
			GUI.color = newColorDef.color;
			GUI.DrawTexture(new Rect(((Rect)(ref val)).x + 5f, ((Rect)(ref val)).y + 5f, 40f, 40f), (Texture)(object)item.Icon);
			GUI.color = Color.white;
			if (Widgets.ButtonInvisible(val))
			{
				newIconDef = item;
				SoundDefOf.Tick_High.PlayOneShotOnCamera();
			}
			viewHeight = Mathf.Max(viewHeight, ((Rect)(ref val)).yMax);
			num4++;
		}
		GUI.color = Color.white;
		Widgets.EndScrollView();
	}

	private void DoColorSelector(Rect mainRect, ref float curY)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		int num = 26;
		float num2 = 98f;
		int num3 = Mathf.FloorToInt((((Rect)(ref mainRect)).width - num2) / (float)(num + 2));
		int num4 = Mathf.CeilToInt((float)IdeoColorsSorted.Count / (float)num3);
		Widgets.BeginGroup(mainRect);
		GUI.color = newColorDef.color;
		GUI.DrawTexture(new Rect(5f, 5f, 88f, 88f), (Texture)(object)newIconDef.Icon);
		GUI.color = Color.white;
		curY += num2;
		int num5 = 0;
		Rect val = default(Rect);
		foreach (ColorDef item in IdeoColorsSorted)
		{
			int num6 = num5 / num3;
			int num7 = num5 % num3;
			float num8 = (num2 - (float)(num * num4) - 2f) / 2f;
			((Rect)(ref val))._002Ector(num2 + (float)(num7 * num) + (float)(num7 * 2), num8 + (float)(num6 * num) + (float)(num6 * 2), (float)num, (float)num);
			Widgets.DrawLightHighlight(val);
			Widgets.DrawHighlightIfMouseover(val);
			if (newColorDef == item)
			{
				Widgets.DrawBox(val);
			}
			Widgets.DrawBoxSolid(new Rect(((Rect)(ref val)).x + 2f, ((Rect)(ref val)).y + 2f, 22f, 22f), item.color);
			if (Widgets.ButtonInvisible(val))
			{
				newColorDef = item;
				SoundDefOf.Tick_High.PlayOneShotOnCamera();
			}
			curY = Mathf.Max(curY, ((Rect)(ref mainRect)).yMin + ((Rect)(ref val)).yMax);
			num5++;
		}
		Widgets.EndGroup();
		curY += 4f;
	}

	private void TryAccept()
	{
		if (!newName.NullOrEmpty())
		{
			newName = newName.Trim();
		}
		if (!newAdjective.NullOrEmpty())
		{
			newAdjective = newAdjective.Trim();
		}
		if (!newMemberName.NullOrEmpty())
		{
			newMemberName = newMemberName.Trim();
		}
		if (!newWorshipRoomLabel.NullOrEmpty())
		{
			newWorshipRoomLabel = newWorshipRoomLabel.Trim();
		}
		bool num = ideo.name != newName || ideo.adjective != newAdjective || ideo.memberName != newMemberName;
		if (!newName.NullOrEmpty())
		{
			ideo.name = newName;
		}
		if (!newAdjective.NullOrEmpty())
		{
			ideo.adjective = newAdjective;
		}
		if (!newMemberName.NullOrEmpty())
		{
			ideo.memberName = newMemberName;
		}
		if (ideo.WorshipRoomLabel != newWorshipRoomLabel && !newWorshipRoomLabel.NullOrEmpty())
		{
			ideo.WorshipRoomLabel = newWorshipRoomLabel;
		}
		ideo.SetIcon(newIconDef, newColorDef, newColorDef != ideo.colorDef);
		if (num)
		{
			ideo.MakeMemeberNamePluralDirty();
			ideo.RegenerateAllPreceptNames();
		}
		Close();
	}
}
