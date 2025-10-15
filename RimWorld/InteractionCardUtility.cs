using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld;

public static class InteractionCardUtility
{
	private static Vector2 logScrollPosition = Vector2.zero;

	public const float ImageSize = 26f;

	public const float ImagePadRight = 3f;

	public const float TextOffset = 29f;

	private static List<Pair<string, int>> logStrings = new List<Pair<string, int>>();

	public static void DrawInteractionsLog(Rect rect, Pawn pawn, List<LogEntry> entries, int maxEntries)
	{
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		float num = ((Rect)(ref rect)).width - 29f - 16f - 10f;
		logStrings.Clear();
		float num2 = 0f;
		int num3 = 0;
		for (int i = 0; i < entries.Count; i++)
		{
			if (entries[i].Concerns(pawn))
			{
				TaggedString taggedString = entries[i].ToGameStringFromPOV(pawn);
				logStrings.Add(new Pair<string, int>(taggedString, i));
				num2 += Mathf.Max(26f, Text.CalcHeight(taggedString, num + 1f));
				num3++;
				if (num3 >= maxEntries)
				{
					break;
				}
			}
		}
		Rect viewRect = default(Rect);
		((Rect)(ref viewRect))._002Ector(0f, 0f, ((Rect)(ref rect)).width - 16f, num2);
		Widgets.BeginScrollView(rect, ref logScrollPosition, viewRect);
		float num4 = 0f;
		Rect val2 = default(Rect);
		for (int j = 0; j < logStrings.Count; j++)
		{
			TaggedString taggedString2 = logStrings[j].First;
			LogEntry entry = entries[logStrings[j].Second];
			float num5 = Mathf.Max(26f, Text.CalcHeight(taggedString2, num + 1f));
			Texture2D val = entry.IconFromPOV(pawn);
			if ((Object)(object)val != (Object)null)
			{
				GUI.color = (Color)(((_003F?)entry.IconColorFromPOV(pawn)) ?? Color.white);
				GUI.DrawTexture(new Rect(0f, num4, 26f, 26f), (Texture)(object)val);
			}
			GUI.color = (Color)((entry.Age > 7500) ? new Color(1f, 1f, 1f, 0.5f) : Color.white);
			((Rect)(ref val2))._002Ector(29f, num4, num, num5);
			if (Mouse.IsOver(val2))
			{
				TooltipHandler.TipRegion(val2, () => entry.GetTipString(), 613261 + j * 611);
				Widgets.DrawHighlight(val2);
			}
			Widgets.Label(val2, taggedString2);
			if (Widgets.ButtonInvisible(val2, entry.CanBeClickedFromPOV(pawn)))
			{
				entry.ClickedFromPOV(pawn);
			}
			GUI.color = Color.white;
			num4 += num5;
		}
		Widgets.EndScrollView();
	}
}
