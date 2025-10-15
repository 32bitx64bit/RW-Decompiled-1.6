using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld;

[StaticConstructorOnStartup]
public static class PawnPortraitIconsDrawer
{
	private struct PawnPortraitIcon
	{
		public Color color;

		public Texture2D icon;

		public string tooltip;
	}

	private static Texture2D prisonerIcon = ContentFinder<Texture2D>.Get("UI/Icons/Prisoner");

	private static Texture2D sleepingIcon = ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/Sleeping");

	private static Texture2D slaveryIcon;

	private static List<PawnPortraitIcon> tmpPortraitIcons = new List<PawnPortraitIcon>();

	private static Texture2D SlaveryIcon
	{
		get
		{
			if ((Object)(object)slaveryIcon == (Object)null)
			{
				slaveryIcon = ContentFinder<Texture2D>.Get("UI/Icons/Slavery");
			}
			return slaveryIcon;
		}
	}

	private static void CalculatePawnPortraitIcons(Pawn pawn, bool required, bool showIdeoIcon)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		using (new ProfilerBlock("CalculatePawnPortraitIcons"))
		{
			tmpPortraitIcons.Clear();
			Ideo ideo = pawn.Ideo;
			if (required)
			{
				tmpPortraitIcons.Add(new PawnPortraitIcon
				{
					color = Color.white,
					icon = IdeoUIUtility.LockedTex,
					tooltip = "Required".Translate()
				});
			}
			if (!ModsConfig.IdeologyActive || ideo == null)
			{
				return;
			}
			if (!Find.IdeoManager.classicMode && showIdeoIcon)
			{
				tmpPortraitIcons.Add(new PawnPortraitIcon
				{
					color = ideo.Color,
					icon = ideo.Icon,
					tooltip = ideo.memberName
				});
				Precept_Role role = ideo.GetRole(pawn);
				if (role != null)
				{
					tmpPortraitIcons.Add(new PawnPortraitIcon
					{
						color = ideo.Color,
						icon = role.Icon,
						tooltip = role.TipLabel
					});
				}
				GUI.color = Color.white;
			}
			Faction homeFaction = pawn.HomeFaction;
			if (homeFaction != null && !homeFaction.IsPlayer && !homeFaction.Hidden)
			{
				tmpPortraitIcons.Add(new PawnPortraitIcon
				{
					color = homeFaction.Color,
					icon = homeFaction.def.FactionIcon,
					tooltip = "Faction".Translate() + ": " + homeFaction.Name + "\n" + homeFaction.def.LabelCap
				});
			}
			if (pawn.IsSlave)
			{
				tmpPortraitIcons.Add(new PawnPortraitIcon
				{
					color = Color.white,
					icon = SlaveryIcon,
					tooltip = "RitualBeginSlaveDesc".Translate()
				});
			}
			if (pawn.IsPrisoner)
			{
				tmpPortraitIcons.Add(new PawnPortraitIcon
				{
					color = Color.white,
					icon = prisonerIcon,
					tooltip = null
				});
			}
			if (!pawn.Awake())
			{
				tmpPortraitIcons.Add(new PawnPortraitIcon
				{
					color = Color.white,
					icon = sleepingIcon,
					tooltip = "RitualBeginSleepingDesc".Translate(pawn)
				});
			}
		}
	}

	public static void DrawPawnPortraitIcons(Rect portraitRect, Pawn p, bool required, bool grayedOut, ref float curX, ref float curY, float iconSize, bool showIdeoIcon, out bool tooltipActive)
	{
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		CalculatePawnPortraitIcons(p, required, showIdeoIcon);
		tooltipActive = false;
		Rect val = default(Rect);
		foreach (PawnPortraitIcon tmpPortraitIcon in tmpPortraitIcons)
		{
			PawnPortraitIcon localIcon = tmpPortraitIcon;
			((Rect)(ref val))._002Ector(curX - iconSize, curY - iconSize, iconSize, iconSize);
			curX -= iconSize;
			if (curX - iconSize < ((Rect)(ref portraitRect)).x)
			{
				curX = ((Rect)(ref portraitRect)).xMax;
				curY -= iconSize + 2f;
			}
			GUI.color = (grayedOut ? tmpPortraitIcon.color.SaturationChanged(0f) : tmpPortraitIcon.color);
			Widgets.DrawTextureFitted(val, (Texture)(object)tmpPortraitIcon.icon, 1f);
			GUI.color = Color.white;
			if (tmpPortraitIcon.tooltip != null)
			{
				if (Mouse.IsOver(val))
				{
					tooltipActive = true;
				}
				TooltipHandler.TipRegion(val, () => localIcon.tooltip, ((Object)tmpPortraitIcon.icon).GetInstanceID() ^ p.thingIDNumber);
			}
		}
	}
}
