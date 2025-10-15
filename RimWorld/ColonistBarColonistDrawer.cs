using System.Collections.Generic;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

[StaticConstructorOnStartup]
public class ColonistBarColonistDrawer
{
	private struct IconDrawCall
	{
		public Texture2D texture;

		public string tooltip;

		public Color? color;

		public IconDrawCall(Texture2D texture, string tooltip = null, Color? color = null)
		{
			this.texture = texture;
			this.tooltip = tooltip;
			this.color = color;
		}
	}

	private Dictionary<string, string> pawnLabelsCache = new Dictionary<string, string>();

	private static readonly Texture2D MoodBGTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.4f, 0.47f, 0.53f, 0.44f));

	private static readonly Texture2D MoodAtlas = ContentFinder<Texture2D>.Get("UI/Widgets/SubtleGradient");

	private static readonly Texture2D DeadColonistTex = ContentFinder<Texture2D>.Get("UI/Misc/DeadColonist");

	private static readonly Texture2D Icon_FormingCaravan = ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/FormingCaravan");

	private static readonly Texture2D Icon_MentalStateNonAggro = ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/MentalStateNonAggro");

	private static readonly Texture2D Icon_MentalStateAggro = ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/MentalStateAggro");

	private static readonly Texture2D Icon_MedicalRest = ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/MedicalRest");

	private static readonly Texture2D Icon_Sleeping = ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/Sleeping");

	private static readonly Texture2D Icon_Fleeing = ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/Fleeing");

	private static readonly Texture2D Icon_Attacking = ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/Attacking");

	private static readonly Texture2D Icon_Idle = ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/Idle");

	private static readonly Texture2D Icon_Burning = ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/Burning");

	private static readonly Texture2D Icon_Inspired = ContentFinder<Texture2D>.Get("UI/Icons/ColonistBar/Inspired");

	private static readonly Texture2D MoodGradient = ContentFinder<Texture2D>.Get("UI/Widgets/MoodGradient");

	public static readonly Vector2 PawnTextureSize = new Vector2(ColonistBar.BaseSize.x - 2f, 75f);

	public static readonly Vector3 PawnTextureCameraOffset = new Vector3(0f, 0f, 0.3f);

	public const float PawnTextureCameraZoom = 1.28205f;

	private const float PawnTextureHorizontalPadding = 1f;

	private static readonly float BaseIconAreaWidth = PawnTextureSize.x;

	private static readonly float BaseIconMaxSize = 20f;

	private const float BaseGroupFrameMargin = 12f;

	public const float DoubleClickTime = 0.5f;

	public const float FactionIconSpacing = 2f;

	public const float IdeoRoleIconSpacing = 2f;

	public const float SlaveIconSpacing = 2f;

	private const float MoodGradientHeight = 35f;

	private static List<IconDrawCall> tmpIconsToDraw = new List<IconDrawCall>();

	private ColonistBar ColonistBar => Find.ColonistBar;

	public void DrawColonist(Rect rect, Pawn colonist, Map pawnMap, bool highlight, bool reordering)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		float alpha = ColonistBar.GetEntryRectAlpha(rect);
		bool num = Prefs.VisibleMood && colonist.needs?.mood != null && colonist.mindState.mentalBreaker.CanDoRandomMentalBreaks && !colonist.Dead && !colonist.Downed;
		MoodThreshold moodThreshold = MoodThresholdExtensions.CurrentMoodThresholdFor(colonist);
		Color color = moodThreshold.GetColor();
		color.a *= alpha;
		ApplyEntryInAnotherMapAlphaFactor(pawnMap, ref alpha);
		if (reordering)
		{
			alpha *= 0.5f;
		}
		Color color2 = default(Color);
		((Color)(ref color2))._002Ector(1f, 1f, 1f, alpha);
		GUI.color = color2;
		if (num && alpha >= 1f)
		{
			float num2 = moodThreshold.EdgeExpansion();
			if (num2 > 0f)
			{
				GUI.color = color;
				Widgets.DrawAtlas(rect.ExpandedBy(num2), MoodAtlas);
				GUI.color = color2;
			}
		}
		GUI.DrawTexture(rect, (Texture)(object)ColonistBar.BGTex);
		if (colonist.needs != null && colonist.needs.mood != null)
		{
			Rect val = rect.ContractedBy(2f);
			float num3 = ((Rect)(ref val)).height * colonist.needs.mood.CurLevelPercentage;
			((Rect)(ref val)).yMin = ((Rect)(ref val)).yMax - num3;
			((Rect)(ref val)).height = num3;
			GUI.DrawTexture(val, (Texture)(object)MoodBGTex);
		}
		if (num && alpha >= 1f)
		{
			float transparency = ((moodThreshold < MoodThreshold.Major) ? 0.1f : 0.15f);
			Widgets.DrawBoxSolid(rect, moodThreshold.GetColor().ToTransparent(transparency));
		}
		if (highlight)
		{
			int thickness = ((((Rect)(ref rect)).width <= 22f) ? 2 : 3);
			GUI.color = Color.white;
			Widgets.DrawBox(rect, thickness);
			GUI.color = color2;
		}
		Rect rect2 = rect.ContractedBy(-2f * ColonistBar.Scale);
		if ((colonist.Dead ? Find.Selector.SelectedObjects.Contains(colonist.Corpse) : Find.Selector.SelectedObjects.Contains(colonist)) && !WorldRendererUtility.WorldSelected)
		{
			DrawSelectionOverlayOnGUI(colonist, rect2);
		}
		else if (WorldRendererUtility.WorldSelected && colonist.IsCaravanMember() && Find.WorldSelector.IsSelected(colonist.GetCaravan()))
		{
			DrawCaravanSelectionOverlayOnGUI(colonist.GetCaravan(), rect2);
		}
		GUI.DrawTexture(GetPawnTextureRect(((Rect)(ref rect)).position), (Texture)(object)PortraitsCache.Get(colonist, PawnTextureSize, Rot4.South, PawnTextureCameraOffset, 1.28205f));
		if (num)
		{
			Rect rect3 = rect.ContractedBy(1f);
			Widgets.BeginGroup(rect3);
			Rect val2 = rect3.AtZero();
			((Rect)(ref val2)).yMin = ((Rect)(ref val2)).yMax - 35f;
			GUI.color = color;
			GUI.DrawTexture(val2, (Texture)(object)MoodGradient);
			GUI.color = color2;
			Widgets.EndGroup();
		}
		GUI.color = new Color(1f, 1f, 1f, alpha * 0.8f);
		DrawIcons(rect, colonist);
		GUI.color = color2;
		if (colonist.Dead)
		{
			GUI.DrawTexture(rect, (Texture)(object)DeadColonistTex);
		}
		float num4 = 4f * ColonistBar.Scale;
		Vector2 pos = default(Vector2);
		((Vector2)(ref pos))._002Ector(((Rect)(ref rect)).center.x, ((Rect)(ref rect)).yMax - num4);
		GenMapUI.DrawPawnLabel(colonist, pos, alpha, ((Rect)(ref rect)).width + ColonistBar.SpaceBetweenColonistsHorizontal - 2f, pawnLabelsCache);
		Text.Font = GameFont.Small;
		GUI.color = Color.white;
	}

	private Rect GroupFrameRect(int group)
	{
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		float num = 99999f;
		float num2 = 0f;
		float num3 = 0f;
		List<ColonistBar.Entry> entries = ColonistBar.Entries;
		List<Vector2> drawLocs = ColonistBar.DrawLocs;
		for (int i = 0; i < entries.Count; i++)
		{
			if (entries[i].group == group)
			{
				num = Mathf.Min(num, drawLocs[i].x);
				num2 = Mathf.Max(num2, drawLocs[i].x + ColonistBar.Size.x);
				num3 = Mathf.Max(num3, drawLocs[i].y + ColonistBar.Size.y);
			}
		}
		return GenUI.ContractedBy(new Rect(num, 0f, num2 - num, num3 - 0f), -12f * ColonistBar.Scale);
	}

	public void DrawGroupFrame(int group)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		Rect position = GroupFrameRect(group);
		Map map = ColonistBar.Entries.Find((ColonistBar.Entry x) => x.group == group).map;
		float num = ((map == null) ? ((!WorldRendererUtility.WorldSelected) ? 0.75f : 1f) : ((map == Find.CurrentMap && !WorldRendererUtility.WorldSelected) ? 1f : 0.75f));
		Widgets.DrawRectFast(position, new Color(0.5f, 0.5f, 0.5f, 0.4f * num));
	}

	private void ApplyEntryInAnotherMapAlphaFactor(Map map, ref float alpha)
	{
		if (map == null)
		{
			if (!WorldRendererUtility.WorldSelected)
			{
				alpha = Mathf.Min(alpha, 0.4f);
			}
		}
		else if (map != Find.CurrentMap || WorldRendererUtility.WorldSelected)
		{
			alpha = Mathf.Min(alpha, 0.4f);
		}
	}

	public void HandleClicks(Rect rect, Pawn colonist, int reorderableGroup, out bool reordering)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if ((int)Event.current.type == 0 && Event.current.button == 0 && Event.current.clickCount == 2 && Mouse.IsOver(rect))
		{
			Event.current.Use();
			CameraJumper.TryJump(colonist);
		}
		reordering = ReorderableWidget.Reorderable(reorderableGroup, rect, useRightButton: true);
		if ((int)Event.current.type == 0 && Event.current.button == 1 && Mouse.IsOver(rect))
		{
			Event.current.Use();
		}
	}

	public void HandleGroupFrameClicks(int group)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Invalid comparison between Unknown and I4
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = GroupFrameRect(group);
		if ((int)Event.current.type == 1 && Event.current.button == 0 && Mouse.IsOver(rect) && !ColonistBar.AnyColonistOrCorpseAt(UI.MousePositionOnUIInverted))
		{
			bool worldSelected = WorldRendererUtility.WorldSelected;
			if ((!worldSelected && !Find.Selector.dragBox.IsValidAndActive) || (worldSelected && !Find.WorldSelector.dragBox.IsValidAndActive))
			{
				Find.Selector.dragBox.active = false;
				Find.WorldSelector.dragBox.active = false;
				ColonistBar.Entry entry = ColonistBar.Entries.Find((ColonistBar.Entry x) => x.group == group);
				Map map = entry.map;
				if (map == null)
				{
					if (WorldRendererUtility.WorldSelected)
					{
						CameraJumper.TrySelect(entry.pawn);
					}
					else
					{
						CameraJumper.TryJumpAndSelect(entry.pawn);
					}
				}
				else
				{
					if (!CameraJumper.TryHideWorld() && Find.CurrentMap != map)
					{
						SoundDefOf.MapSelected.PlayOneShotOnCamera();
					}
					Current.Game.CurrentMap = map;
				}
			}
		}
		if ((int)Event.current.type == 0 && Event.current.button == 1 && Mouse.IsOver(rect))
		{
			Event.current.Use();
		}
	}

	public void Notify_RecachedEntries()
	{
		pawnLabelsCache.Clear();
	}

	public void ClearLabelCache()
	{
		pawnLabelsCache.Clear();
	}

	public Rect GetPawnTextureRect(Vector2 pos)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		float x = pos.x;
		float y = pos.y;
		Vector2 val = PawnTextureSize * ColonistBar.Scale;
		return GenUI.ContractedBy(new Rect(x + 1f, y - (val.y - ColonistBar.Size.y) - 1f, val.x, val.y), 1f);
	}

	private void DrawIcons(Rect rect, Pawn colonist)
	{
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_046e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0465: Unknown result type (might be due to invalid IL or missing references)
		//IL_048f: Unknown result type (might be due to invalid IL or missing references)
		if (colonist.Dead)
		{
			return;
		}
		tmpIconsToDraw.Clear();
		bool flag = false;
		if (colonist.CurJob != null)
		{
			JobDef def = colonist.CurJob.def;
			if (def == JobDefOf.AttackMelee || def == JobDefOf.AttackStatic)
			{
				flag = true;
			}
			else if (def == JobDefOf.Wait_Combat && colonist.stances.curStance is Stance_Busy stance_Busy && stance_Busy.focusTarg.IsValid)
			{
				flag = true;
			}
		}
		if (colonist.IsFormingCaravan())
		{
			tmpIconsToDraw.Add(new IconDrawCall(Icon_FormingCaravan, "ActivityIconFormingCaravan".Translate()));
		}
		if (colonist.InAggroMentalState)
		{
			tmpIconsToDraw.Add(new IconDrawCall(Icon_MentalStateAggro, colonist.MentalStateDef.LabelCap));
		}
		else if (colonist.InMentalState)
		{
			tmpIconsToDraw.Add(new IconDrawCall(Icon_MentalStateNonAggro, colonist.MentalStateDef.LabelCap));
		}
		else if (colonist.InBed() && colonist.CurrentBed().Medical)
		{
			tmpIconsToDraw.Add(new IconDrawCall(Icon_MedicalRest, "ActivityIconMedicalRest".Translate()));
		}
		else
		{
			if (colonist.CurJob != null && colonist.jobs.curDriver.asleep)
			{
				goto IL_01c5;
			}
			if (colonist.GetCaravan() != null)
			{
				Pawn_NeedsTracker needs = colonist.needs;
				if (needs != null && needs.rest?.Resting == true)
				{
					goto IL_01c5;
				}
			}
			if (colonist.CurJob != null && colonist.CurJob.def == JobDefOf.FleeAndCower)
			{
				tmpIconsToDraw.Add(new IconDrawCall(Icon_Fleeing, "ActivityIconFleeing".Translate()));
			}
			else if (flag)
			{
				tmpIconsToDraw.Add(new IconDrawCall(Icon_Attacking, "ActivityIconAttacking".Translate()));
			}
			else if (colonist.mindState.IsIdle && GenDate.DaysPassed >= 1)
			{
				tmpIconsToDraw.Add(new IconDrawCall(Icon_Idle, "ActivityIconIdle".Translate()));
			}
		}
		goto IL_02b4;
		IL_01c5:
		tmpIconsToDraw.Add(new IconDrawCall(Icon_Sleeping, "ActivityIconSleeping".Translate()));
		goto IL_02b4;
		IL_02b4:
		if (colonist.IsBurning())
		{
			tmpIconsToDraw.Add(new IconDrawCall(Icon_Burning, "ActivityIconBurning".Translate()));
		}
		if (colonist.Inspired)
		{
			tmpIconsToDraw.Add(new IconDrawCall(Icon_Inspired, colonist.InspirationDef.LabelCap));
		}
		if (colonist.IsSlaveOfColony)
		{
			tmpIconsToDraw.Add(new IconDrawCall(colonist.guest.GetIcon()));
		}
		else
		{
			bool flag2 = false;
			if (colonist.Ideo != null)
			{
				Ideo ideo = colonist.Ideo;
				Precept_Role role = ideo.GetRole(colonist);
				if (role != null)
				{
					tmpIconsToDraw.Add(new IconDrawCall(role.Icon, null, ideo.Color));
					flag2 = true;
				}
			}
			if (!flag2)
			{
				Faction faction = null;
				if (colonist.HasExtraMiniFaction())
				{
					faction = colonist.GetExtraMiniFaction();
				}
				else if (colonist.HasExtraHomeFaction())
				{
					faction = colonist.GetExtraHomeFaction();
				}
				if (faction != null)
				{
					tmpIconsToDraw.Add(new IconDrawCall(faction.def.FactionIcon, null, faction.Color));
				}
			}
		}
		float num = Mathf.Min(BaseIconAreaWidth / (float)tmpIconsToDraw.Count, BaseIconMaxSize) * ColonistBar.Scale;
		Vector2 pos = default(Vector2);
		((Vector2)(ref pos))._002Ector(((Rect)(ref rect)).x + 1f, ((Rect)(ref rect)).yMax - num - 1f);
		foreach (IconDrawCall item in tmpIconsToDraw)
		{
			GUI.color = (Color)(((_003F?)item.color) ?? Color.white);
			DrawIcon(item.texture, ref pos, num, item.tooltip);
			GUI.color = Color.white;
		}
	}

	private void DrawIcon(Texture2D icon, ref Vector2 pos, float iconSize, string tooltip = null)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(pos.x, pos.y, iconSize, iconSize);
		GUI.DrawTexture(val, (Texture)(object)icon);
		if (tooltip != null)
		{
			TooltipHandler.TipRegion(val, tooltip);
		}
		pos.x += iconSize;
	}

	private void DrawSelectionOverlayOnGUI(Pawn colonist, Rect rect)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		Thing target = colonist;
		if (colonist.Dead)
		{
			target = colonist.Corpse;
		}
		SelectionDrawerUtility.DrawSelectionOverlayOnGUI(target, rect, 0.4f * ColonistBar.Scale, 20f * ColonistBar.Scale);
	}

	private void DrawCaravanSelectionOverlayOnGUI(Caravan caravan, Rect rect)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		SelectionDrawerUtility.DrawSelectionOverlayOnGUI(caravan, rect, 0.4f * ColonistBar.Scale, 20f * ColonistBar.Scale);
	}
}
