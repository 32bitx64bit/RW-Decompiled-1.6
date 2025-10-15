using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse.Sound;

namespace Verse;

public class Dialog_NamePawn : Window
{
	private class NameContext
	{
		public string current;

		public TaggedString label;

		public float labelWidth;

		public int maximumNameLength;

		public float textboxWidth;

		public string textboxName;

		public bool editable;

		public int nameIndex;

		public List<string> suggestedNames;

		private List<FloatMenuOption> suggestedOptions;

		public NameContext(string label, int nameIndex, string currentName, int maximumNameLength, bool editable, List<string> suggestedNames)
		{
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			current = currentName;
			this.nameIndex = nameIndex;
			this.label = label.Translate().CapitalizeFirst() + ":";
			labelWidth = Mathf.Ceil(this.label.GetWidthCached());
			this.maximumNameLength = maximumNameLength;
			textboxWidth = Mathf.Ceil(Text.CalcSize(new string('W', maximumNameLength + 2)).x);
			textboxName = label;
			this.editable = editable;
			this.suggestedNames = suggestedNames;
			if (suggestedNames == null)
			{
				return;
			}
			suggestedOptions = new List<FloatMenuOption>(suggestedNames.Count);
			foreach (string suggestedName in suggestedNames)
			{
				suggestedOptions.Add(new FloatMenuOption(suggestedName, delegate
				{
					current = suggestedName;
				}));
			}
		}

		public void MakeRow(Pawn pawn, float randomizeButtonWidth, TaggedString randomizeText, TaggedString suggestedText, ref RectDivider divider, ref string focusControlOverride)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			Widgets.Label(divider.NewCol(labelWidth), label);
			RectDivider rectDivider = divider.NewCol(textboxWidth);
			if (editable)
			{
				GUI.SetNextControlName(textboxName);
				CharacterCardUtility.DoNameInputRect(rectDivider, ref current, maximumNameLength);
			}
			else
			{
				Widgets.Label(rectDivider, current);
			}
			if (!editable || nameIndex < 0)
			{
				return;
			}
			Rect rect = divider.NewCol(randomizeButtonWidth);
			if (suggestedNames != null)
			{
				List<string> list = suggestedNames;
				if (list != null && list.Count > 0 && Widgets.ButtonText(rect, suggestedText))
				{
					Find.WindowStack.Add(new FloatMenu(suggestedOptions));
				}
			}
			else if (Widgets.ButtonText(rect, randomizeText))
			{
				SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
				Name name = PawnBioAndNameGenerator.GeneratePawnName(pawn, NameStyle.Full, null, forceNoNick: false, pawn.genes?.Xenotype);
				if (name is NameTriple nameTriple)
				{
					current = nameTriple[nameIndex];
				}
				else if (name is NameSingle nameSingle)
				{
					current = nameSingle.Name;
				}
			}
		}
	}

	private Pawn pawn;

	private List<NameContext> names = new List<NameContext>(4);

	private bool firstCall = true;

	private string focusControlOverride;

	private string currentControl;

	private TaggedString descriptionText;

	private float? descriptionHeight;

	private float randomizeButtonWidth;

	private Vector2 size = new Vector2(800f, 800f);

	private float? renameHeight;

	private Rot4 portraitDirection;

	private float cameraZoom = 1f;

	private float portraitSize = 128f;

	private float humanPortraitVerticalOffset = -18f;

	private TaggedString cancelText = "Cancel".Translate().CapitalizeFirst();

	private TaggedString acceptText = "Accept".Translate().CapitalizeFirst();

	private TaggedString randomizeText;

	private TaggedString suggestedText;

	private TaggedString renameText;

	private string genderText;

	private const float ButtonHeight = 35f;

	private const float NameFieldsHeight = 30f;

	private const int MaximumNumberOfNames = 4;

	private const float VerticalMargin = 4f;

	private const float HorizontalMargin = 17f;

	private const float PortraitSize = 128f;

	private const int ContextHash = 136098329;

	private string CurPawnNick
	{
		get
		{
			if (pawn.Name is NameTriple)
			{
				return names[1].current;
			}
			if (pawn.Name is NameSingle)
			{
				return names[0].current;
			}
			throw new InvalidOperationException();
		}
		set
		{
			int num = -1;
			if (pawn.Name is NameTriple)
			{
				num = 1;
			}
			else
			{
				if (!(pawn.Name is NameSingle))
				{
					throw new InvalidOperationException();
				}
				num = 0;
			}
			names[num].current = value;
		}
	}

	private string CurPawnTitle
	{
		get
		{
			if (!(names.Last().textboxName == "BackstoryTitle"))
			{
				return null;
			}
			return names.Last().current;
		}
	}

	public override Vector2 InitialSize => size;

	private Name BuildName()
	{
		if (pawn.Name is NameTriple)
		{
			return new NameTriple(names[0].current?.Trim(), names[1].current?.Trim(), names[2].current?.Trim());
		}
		if (pawn.Name is NameSingle)
		{
			return new NameSingle(names[0].current?.Trim());
		}
		throw new InvalidOperationException();
	}

	public Dialog_NamePawn(Pawn pawn, NameFilter visibleNames, NameFilter editableNames, Dictionary<NameFilter, List<string>> suggestedNames, string initialFirstNameOverride = null, string initialNickNameOverride = null, string initialLastNameOverride = null, string initialTitleOverride = null)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
		this.pawn = pawn;
		if (pawn.RaceProps.Humanlike)
		{
			descriptionText = "{0}: {1}\n{2}: {3}".Formatted("Mother".Translate().CapitalizeFirst(), DescribePawn(pawn.GetMother()), "Father".Translate().CapitalizeFirst(), DescribePawn(pawn.GetFather()));
			renameText = "RenamePerson".Translate().CapitalizeFirst();
			portraitDirection = Rot4.South;
			if (pawn.story?.bodyType == BodyTypeDefOf.Hulk)
			{
				cameraZoom = 0.9f;
				humanPortraitVerticalOffset -= 8f;
			}
		}
		else
		{
			descriptionText = pawn.KindLabelIndefinite().CapitalizeFirst();
			renameText = ((ModsConfig.BiotechActive && pawn.RaceProps.IsMechanoid) ? "RenameMech" : "RenameAnimal").Translate().CapitalizeFirst();
			portraitDirection = Rot4.East;
			Bounds bounds = pawn.Drawer.renderer.BodyGraphic.MeshAt(portraitDirection).bounds;
			Vector3 extents = ((Bounds)(ref bounds)).extents;
			float num = Math.Max(extents.x, extents.z);
			cameraZoom = 1f / num;
			portraitSize = Mathf.Min(128f, Mathf.Ceil(128f * num));
		}
		NameTriple nameTriple = pawn.Name as NameTriple;
		if (nameTriple != null && (visibleNames & NameFilter.First) > NameFilter.None)
		{
			names.Add(new NameContext("FirstName", 0, initialFirstNameOverride ?? nameTriple.First, 12, (editableNames & NameFilter.First) > NameFilter.None, suggestedNames?.GetWithFallback(NameFilter.First)));
		}
		if ((visibleNames & NameFilter.Nick) > NameFilter.None)
		{
			string text = ((nameTriple == null || nameTriple.NickSet || (editableNames & NameFilter.Nick) <= NameFilter.None) ? pawn.Name.ToStringShort : "");
			names.Add(new NameContext("NickName", 1, initialNickNameOverride ?? text, 16, (editableNames & NameFilter.Nick) > NameFilter.None, suggestedNames?.GetWithFallback(NameFilter.Nick)));
		}
		if (nameTriple != null && (visibleNames & NameFilter.Last) > NameFilter.None)
		{
			names.Add(new NameContext("LastName", 2, initialLastNameOverride ?? nameTriple.Last, 12, (editableNames & NameFilter.Last) > NameFilter.None, suggestedNames?.GetWithFallback(NameFilter.Last)));
		}
		if (pawn.story != null && (visibleNames & NameFilter.Title) > NameFilter.None)
		{
			names.Add(new NameContext("BackstoryTitle", -1, initialTitleOverride ?? pawn.story.title ?? "", 16, (editableNames & NameFilter.Title) > NameFilter.None, suggestedNames?.GetWithFallback(NameFilter.Title)));
		}
		float num2 = names.Max((NameContext name) => name.labelWidth);
		float num3 = names.Max((NameContext name) => name.textboxWidth);
		foreach (NameContext name in names)
		{
			name.labelWidth = num2;
			name.textboxWidth = num3;
		}
		randomizeText = "Randomize".Translate().CapitalizeFirst();
		suggestedText = "Suggested".Translate().CapitalizeFirst() + "...";
		randomizeButtonWidth = ButtonWidth(randomizeText.GetWidthCached());
		genderText = string.Format("{0}: {1}", "Gender".Translate().CapitalizeFirst(), pawn.GetGenderLabel().CapitalizeFirst());
		float num4 = 2f * Margin + num2 + num3 + randomizeButtonWidth + 34f;
		size = new Vector2(num4, size.y);
		forcePause = true;
		absorbInputAroundWindow = true;
		closeOnClickedOutside = true;
		closeOnAccept = false;
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Invalid comparison between Unknown and I4
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Invalid comparison between Unknown and I4
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Invalid comparison between Unknown and I4
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Invalid comparison between Unknown and I4
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Invalid comparison between Unknown and I4
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		//IL_061c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0621: Unknown result type (might be due to invalid IL or missing references)
		//IL_0638: Unknown result type (might be due to invalid IL or missing references)
		//IL_063d: Unknown result type (might be due to invalid IL or missing references)
		//IL_066c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0672: Invalid comparison between Unknown and I4
		bool flag = false;
		if ((int)Event.current.type == 4 && ((int)Event.current.keyCode == 13 || (int)Event.current.keyCode == 271))
		{
			flag = true;
			Event.current.Use();
		}
		bool flag2 = false;
		bool forward = true;
		if ((int)Event.current.type == 4 && (int)Event.current.keyCode == 9)
		{
			flag2 = true;
			forward = !Event.current.shift;
			Event.current.Use();
		}
		if (!firstCall && (int)Event.current.type == 8)
		{
			currentControl = GUI.GetNameOfFocusedControl();
		}
		RectAggregator rectAggregator = new RectAggregator(new Rect(((Rect)(ref inRect)).x, ((Rect)(ref inRect)).y, ((Rect)(ref inRect)).width, 0f), 136098329, (Vector2?)new Vector2(17f, 4f));
		if (!renameHeight.HasValue)
		{
			Text.Font = GameFont.Medium;
			renameHeight = Mathf.Ceil(renameText.RawText.GetHeightCached());
			Text.Font = GameFont.Small;
		}
		float? num = descriptionHeight;
		Rect rect;
		float value;
		if (!num.HasValue)
		{
			string text = descriptionText;
			rect = rectAggregator.Rect;
			value = Mathf.Ceil(Text.CalcHeight(text, ((Rect)(ref rect)).width - portraitSize - 17f));
		}
		else
		{
			value = num.GetValueOrDefault();
		}
		descriptionHeight = value;
		float num2 = renameHeight.Value + 4f + descriptionHeight.Value;
		if (!pawn.RaceProps.Humanlike && portraitSize > num2)
		{
			num2 = portraitSize;
		}
		RectDivider rectDivider = rectAggregator.NewRow(num2);
		Text.Font = GameFont.Medium;
		Pawn obj = pawn;
		Vector2 val = new Vector2(portraitSize, portraitSize);
		Rot4 rotation = portraitDirection;
		PawnHealthState? healthStateOverride = PawnHealthState.Mobile;
		float num3 = cameraZoom;
		RenderTexture val2 = PortraitsCache.Get(obj, val, rotation, default(Vector3), num3, supersample: true, compensateForUIScale: true, renderHeadgear: true, renderClothes: true, null, null, stylingStation: false, healthStateOverride);
		Rect val3 = rectDivider.NewCol(portraitSize);
		if (pawn.RaceProps.Humanlike)
		{
			((Rect)(ref val3)).y = ((Rect)(ref val3)).y + humanPortraitVerticalOffset;
		}
		((Rect)(ref val3)).height = portraitSize;
		GUI.DrawTexture(val3, (Texture)(object)val2);
		RectDivider rectDivider2 = rectDivider.NewRow(renameHeight.Value);
		Rect val4 = rectDivider2.NewCol(renameHeight.Value, HorizontalJustification.Right);
		GUI.DrawTexture(val4, (Texture)(object)pawn.gender.GetIcon());
		TooltipHandler.TipRegion(val4, genderText);
		Widgets.Label(rectDivider2, renameText);
		Text.Font = GameFont.Small;
		Widgets.Label(rectDivider.NewRow(descriptionHeight.Value), descriptionText);
		Text.Anchor = (TextAnchor)3;
		foreach (NameContext name2 in names)
		{
			RectDivider divider = rectAggregator.NewRow(30f);
			name2.MakeRow(pawn, randomizeButtonWidth, randomizeText, suggestedText, ref divider, ref focusControlOverride);
		}
		Text.Anchor = (TextAnchor)0;
		rectAggregator.NewRow(17.5f);
		RectDivider rectDivider3 = rectAggregator.NewRow(35f);
		rect = rectDivider3.Rect;
		float width = Mathf.Floor((((Rect)(ref rect)).width - 17f) / 2f);
		if (Widgets.ButtonText(rectDivider3.NewCol(width), cancelText))
		{
			Close();
		}
		if (Widgets.ButtonText(rectDivider3.NewCol(width), acceptText) || flag)
		{
			Name name = BuildName();
			if (!name.IsValid)
			{
				Messages.Message("NameInvalid".Translate(), pawn, MessageTypeDefOf.NeutralEvent, historical: false);
			}
			else
			{
				pawn.Name = name;
				if (pawn.story != null)
				{
					pawn.story.Title = CurPawnTitle;
				}
				Find.WindowStack.TryRemove(this);
				string text2 = (pawn.def.race.Animal ? ((string)"AnimalGainsName".Translate(CurPawnNick)) : (pawn.def.race.IsMechanoid ? ((string)"MechGainsName".Translate(CurPawnNick)) : ((!(name is NameTriple nameTriple)) ? ((string)"PawnGainsName".Translate(CurPawnNick, pawn.story.Title, pawn.Named("PAWN")).AdjustedFor(pawn)) : ((string)"PawnGainsName".Translate(nameTriple.Nick, pawn.story.Title, pawn.Named("PAWN")).AdjustedFor(pawn)))));
				Messages.Message(text2, pawn, MessageTypeDefOf.PositiveEvent, historical: false);
				pawn.babyNamingDeadline = -1;
			}
		}
		float x = size.x;
		float y = size.y;
		rect = rectAggregator.Rect;
		size = new Vector2(x, Mathf.Ceil(y + (((Rect)(ref rect)).height - ((Rect)(ref inRect)).height)));
		SetInitialSizeAndPosition();
		if (flag2 || firstCall)
		{
			FocusNextControl(currentControl, forward);
			firstCall = false;
		}
		if ((int)Event.current.type == 8 && !string.IsNullOrEmpty(focusControlOverride))
		{
			GUI.FocusControl(focusControlOverride);
			focusControlOverride = null;
		}
	}

	private void FocusNextControl(string currentControl, bool forward)
	{
		int num = names.FindIndex((NameContext name) => name.textboxName == currentControl);
		int num2 = -1;
		if (forward)
		{
			for (int i = 1; i <= names.Count; i++)
			{
				int num3 = (num + i) % names.Count;
				if (names[num3].editable)
				{
					num2 = num3;
					break;
				}
			}
		}
		else
		{
			for (int j = 1; j <= names.Count; j++)
			{
				int num4 = (names.Count + num - j) % names.Count;
				if (names[num4].editable)
				{
					num2 = num4;
					break;
				}
			}
		}
		if (num2 >= 0)
		{
			focusControlOverride = names[num2].textboxName;
		}
	}

	private TaggedString DescribePawn(Pawn pawn)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		if (pawn != null)
		{
			return pawn.FactionDesc(pawn.NameFullColored, extraFactionsInfo: false, pawn.NameFullColored, pawn.gender.GetLabel(pawn.RaceProps.Animal)).Resolve();
		}
		return "Unknown".Translate().Colorize(Color.gray);
	}

	private static float ButtonWidth(float textWidth)
	{
		return Math.Max(114f, textWidth + 35f);
	}
}
