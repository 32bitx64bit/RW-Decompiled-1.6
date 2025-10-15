using System;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse.Sound;
using Verse.Steam;

namespace Verse;

[StaticConstructorOnStartup]
public class FloatMenuOption
{
	private string labelInt;

	public Action action;

	private MenuOptionPriority priorityInt = MenuOptionPriority.Default;

	public int orderInPriority;

	public bool autoTakeable;

	public float autoTakeablePriority;

	public Action<Rect> mouseoverGuiAction;

	public Thing revalidateClickTarget;

	public bool targetsDespawned;

	public WorldObject revalidateWorldClickTarget;

	public float extraPartWidth;

	public Func<Rect, bool> extraPartOnGUI;

	public string tutorTag;

	public ThingStyleDef thingStyle;

	public bool forceBasicStyle;

	public TipSignal? tooltip;

	public bool extraPartRightJustified;

	public int? graphicIndexOverride;

	public bool isGoto;

	private FloatMenuSizeMode sizeMode;

	private float cachedRequiredHeight;

	private float cachedRequiredWidth;

	private bool drawPlaceHolderIcon;

	private bool playSelectionSound = true;

	private ThingDef shownItem;

	public Thing iconThing;

	private Texture2D iconTex;

	public Rect iconTexCoords = new Rect(0f, 0f, 1f, 1f);

	private HorizontalJustification iconJustification;

	public Color iconColor = Color.white;

	public Color? forceThingColor;

	public const float MaxWidth = 300f;

	private const float TinyVerticalMargin = 1f;

	private const float NormalHorizontalMargin = 6f;

	private const float TinyHorizontalMargin = 3f;

	private const float MouseOverLabelShift = 4f;

	public static readonly Color ColorBGActive = new ColorInt(21, 25, 29).ToColor;

	public static readonly Color ColorBGActiveMouseover = new ColorInt(29, 45, 50).ToColor;

	public static readonly Color ColorBGDisabled = new ColorInt(40, 40, 40).ToColor;

	public static readonly Color ColorTextActive = Color.white;

	public static readonly Color ColorTextDisabled = new Color(0.9f, 0.9f, 0.9f);

	public const float ExtraPartHeight = 30f;

	private const float ItemIconSize = 27f;

	private const float ItemIconSizeTiny = 16f;

	private const float ItemIconMargin = 4f;

	private static float NormalVerticalMargin => SteamDeck.IsSteamDeck ? 10 : 4;

	public string Label
	{
		get
		{
			return labelInt;
		}
		set
		{
			if (value.NullOrEmpty())
			{
				value = "(missing label)";
			}
			labelInt = value.TrimEnd();
			SetSizeMode(sizeMode);
		}
	}

	private float VerticalMargin
	{
		get
		{
			if (sizeMode != FloatMenuSizeMode.Normal)
			{
				return 1f;
			}
			return NormalVerticalMargin;
		}
	}

	private float HorizontalMargin
	{
		get
		{
			if (sizeMode != FloatMenuSizeMode.Normal)
			{
				return 3f;
			}
			return 6f;
		}
	}

	private float IconOffset
	{
		get
		{
			if (shownItem == null && !drawPlaceHolderIcon && !((Object)(object)iconTex != (Object)null) && iconThing == null)
			{
				return 0f;
			}
			return CurIconSize;
		}
	}

	private GameFont CurrentFont
	{
		get
		{
			if (sizeMode != FloatMenuSizeMode.Normal)
			{
				return GameFont.Tiny;
			}
			return GameFont.Small;
		}
	}

	private float CurIconSize
	{
		get
		{
			if (sizeMode != FloatMenuSizeMode.Tiny)
			{
				return 27f;
			}
			return 16f;
		}
	}

	public bool Disabled
	{
		get
		{
			return action == null;
		}
		set
		{
			if (value)
			{
				action = null;
			}
		}
	}

	public float RequiredHeight => cachedRequiredHeight;

	public float RequiredWidth => cachedRequiredWidth;

	public MenuOptionPriority Priority
	{
		get
		{
			if (Disabled)
			{
				return MenuOptionPriority.DisabledOption;
			}
			return priorityInt;
		}
		set
		{
			if (Disabled)
			{
				Log.Error("Setting priority on disabled FloatMenuOption: " + Label);
			}
			priorityInt = value;
		}
	}

	public FloatMenuOption(string label, Action action, MenuOptionPriority priority = MenuOptionPriority.Default, Action<Rect> mouseoverGuiAction = null, Thing revalidateClickTarget = null, float extraPartWidth = 0f, Func<Rect, bool> extraPartOnGUI = null, WorldObject revalidateWorldClickTarget = null, bool playSelectionSound = true, int orderInPriority = 0)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		Label = label;
		this.action = action;
		priorityInt = priority;
		this.revalidateClickTarget = revalidateClickTarget;
		this.mouseoverGuiAction = mouseoverGuiAction;
		this.extraPartWidth = extraPartWidth;
		this.extraPartOnGUI = extraPartOnGUI;
		this.revalidateWorldClickTarget = revalidateWorldClickTarget;
		this.playSelectionSound = playSelectionSound;
		this.orderInPriority = orderInPriority;
	}

	public FloatMenuOption(string label, Action action, ThingDef shownItemForIcon, ThingStyleDef thingStyle = null, bool forceBasicStyle = false, MenuOptionPriority priority = MenuOptionPriority.Default, Action<Rect> mouseoverGuiAction = null, Thing revalidateClickTarget = null, float extraPartWidth = 0f, Func<Rect, bool> extraPartOnGUI = null, WorldObject revalidateWorldClickTarget = null, bool playSelectionSound = true, int orderInPriority = 0, int? graphicIndexOverride = null)
		: this(label, action, priority, mouseoverGuiAction, revalidateClickTarget, extraPartWidth, extraPartOnGUI, revalidateWorldClickTarget, playSelectionSound, orderInPriority)
	{
		shownItem = shownItemForIcon;
		this.thingStyle = thingStyle;
		this.forceBasicStyle = forceBasicStyle;
		this.graphicIndexOverride = graphicIndexOverride;
		if (shownItemForIcon == null)
		{
			drawPlaceHolderIcon = true;
		}
	}

	public FloatMenuOption(string label, Action action, ThingDef shownItemForIcon, Texture2D iconTex, ThingStyleDef thingStyle = null, bool forceBasicStyle = false, MenuOptionPriority priority = MenuOptionPriority.Default, Action<Rect> mouseoverGuiAction = null, Thing revalidateClickTarget = null, float extraPartWidth = 0f, Func<Rect, bool> extraPartOnGUI = null, WorldObject revalidateWorldClickTarget = null, bool playSelectionSound = true, int orderInPriority = 0, int? graphicIndexOverride = null)
		: this(label, action, priority, mouseoverGuiAction, revalidateClickTarget, extraPartWidth, extraPartOnGUI, revalidateWorldClickTarget, playSelectionSound, orderInPriority)
	{
		this.iconTex = iconTex;
		shownItem = shownItemForIcon;
		this.thingStyle = thingStyle;
		this.forceBasicStyle = forceBasicStyle;
		this.graphicIndexOverride = graphicIndexOverride;
		if (shownItemForIcon == null && (Object)(object)iconTex == (Object)null)
		{
			drawPlaceHolderIcon = true;
		}
	}

	public FloatMenuOption(string label, Action action, Texture2D iconTex, Color iconColor, MenuOptionPriority priority = MenuOptionPriority.Default, Action<Rect> mouseoverGuiAction = null, Thing revalidateClickTarget = null, float extraPartWidth = 0f, Func<Rect, bool> extraPartOnGUI = null, WorldObject revalidateWorldClickTarget = null, bool playSelectionSound = true, int orderInPriority = 0, HorizontalJustification iconJustification = HorizontalJustification.Left, bool extraPartRightJustified = false)
		: this(label, action, priority, mouseoverGuiAction, revalidateClickTarget, extraPartWidth, extraPartOnGUI, revalidateWorldClickTarget, playSelectionSound, orderInPriority)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		this.iconTex = iconTex;
		this.iconColor = iconColor;
		this.iconJustification = iconJustification;
		this.extraPartRightJustified = extraPartRightJustified;
	}

	public FloatMenuOption(string label, Action action, Thing iconThing, Color iconColor, MenuOptionPriority priority = MenuOptionPriority.Default, Action<Rect> mouseoverGuiAction = null, Thing revalidateClickTarget = null, float extraPartWidth = 0f, Func<Rect, bool> extraPartOnGUI = null, WorldObject revalidateWorldClickTarget = null, bool playSelectionSound = true, int orderInPriority = 0)
		: this(label, action, priority, mouseoverGuiAction, revalidateClickTarget, extraPartWidth, extraPartOnGUI, revalidateWorldClickTarget, playSelectionSound, orderInPriority)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		this.iconThing = iconThing;
		this.iconColor = iconColor;
	}

	public static FloatMenuOption CheckboxLabeled(string label, Action checkboxStateChanged, bool currentState)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return new FloatMenuOption(label, checkboxStateChanged, Widgets.GetCheckboxTexture(currentState), Color.white, MenuOptionPriority.Default, null, null, 0f, null, null, playSelectionSound: true, 0, HorizontalJustification.Right);
	}

	public void SetSizeMode(FloatMenuSizeMode newSizeMode)
	{
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		sizeMode = newSizeMode;
		GameFont font = Text.Font;
		Text.Font = CurrentFont;
		float width = 300f - (2f * HorizontalMargin + 4f + extraPartWidth + IconOffset);
		cachedRequiredHeight = 2f * VerticalMargin + Text.CalcHeight(Label, width);
		cachedRequiredWidth = HorizontalMargin + 4f + Text.CalcSize(Label).x + extraPartWidth + HorizontalMargin + IconOffset + 4f;
		Text.Font = font;
	}

	public void Chosen(bool colonistOrdering, FloatMenu floatMenu)
	{
		floatMenu?.PreOptionChosen(this);
		if (!Disabled)
		{
			if (action != null)
			{
				if (colonistOrdering && playSelectionSound)
				{
					SoundDefOf.ColonistOrdered.PlayOneShotOnCamera();
				}
				action();
			}
		}
		else if (playSelectionSound)
		{
			SoundDefOf.ClickReject.PlayOneShotOnCamera();
		}
	}

	public virtual bool DoGUI(Rect rect, bool colonistOrdering, FloatMenu floatMenu)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_0440: Unknown result type (might be due to invalid IL or missing references)
		//IL_0456: Unknown result type (might be due to invalid IL or missing references)
		//IL_045c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_049f: Unknown result type (might be due to invalid IL or missing references)
		//IL_047b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0482: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		//IL_0419: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
		Rect val = rect;
		float height = ((Rect)(ref val)).height;
		((Rect)(ref val)).height = height - 1f;
		bool flag = !Disabled && Mouse.IsOver(val);
		bool flag2 = false;
		Text.Font = CurrentFont;
		if (tooltip.HasValue)
		{
			TooltipHandler.TipRegion(rect, tooltip.Value);
		}
		Rect val2 = rect;
		if (iconJustification == HorizontalJustification.Left)
		{
			((Rect)(ref val2)).xMin = ((Rect)(ref val2)).xMin + 4f;
			((Rect)(ref val2)).xMax = ((Rect)(ref rect)).x + CurIconSize;
			((Rect)(ref val2)).yMin = ((Rect)(ref val2)).yMin + 4f;
			((Rect)(ref val2)).yMax = ((Rect)(ref rect)).y + CurIconSize;
			if (flag)
			{
				((Rect)(ref val2)).x = ((Rect)(ref val2)).x + 4f;
			}
		}
		Rect rect2 = rect;
		((Rect)(ref rect2)).xMin = ((Rect)(ref rect2)).xMin + HorizontalMargin;
		((Rect)(ref rect2)).xMax = ((Rect)(ref rect2)).xMax - HorizontalMargin;
		((Rect)(ref rect2)).xMax = ((Rect)(ref rect2)).xMax - 4f;
		((Rect)(ref rect2)).xMax = ((Rect)(ref rect2)).xMax - (extraPartWidth + IconOffset);
		if (iconJustification == HorizontalJustification.Left)
		{
			((Rect)(ref rect2)).x = ((Rect)(ref rect2)).x + IconOffset;
		}
		if (flag)
		{
			((Rect)(ref rect2)).x = ((Rect)(ref rect2)).x + 4f;
		}
		float num = Mathf.Min(Text.CalcSize(Label).x, ((Rect)(ref rect2)).width - 4f);
		float num2 = ((Rect)(ref rect2)).xMin + num;
		if (iconJustification == HorizontalJustification.Right)
		{
			((Rect)(ref val2)).x = num2 + 4f;
			((Rect)(ref val2)).width = CurIconSize;
			((Rect)(ref val2)).yMin = ((Rect)(ref val2)).yMin + 4f;
			((Rect)(ref val2)).yMax = ((Rect)(ref rect)).y + CurIconSize;
			num2 += CurIconSize;
		}
		Rect val3 = default(Rect);
		if (extraPartWidth != 0f)
		{
			if (extraPartRightJustified)
			{
				num2 = ((Rect)(ref rect)).xMax - extraPartWidth;
			}
			((Rect)(ref val3))._002Ector(num2, ((Rect)(ref rect2)).yMin, extraPartWidth, 30f);
			flag2 = Mouse.IsOver(val3);
		}
		if (!Disabled)
		{
			MouseoverSounds.DoRegion(val);
		}
		Color color = GUI.color;
		if (Disabled)
		{
			GUI.color = ColorBGDisabled * color;
		}
		else if (flag && !flag2)
		{
			GUI.color = ColorBGActiveMouseover * color;
		}
		else
		{
			GUI.color = ColorBGActive * color;
		}
		GUI.DrawTexture(rect, (Texture)(object)BaseContent.WhiteTex);
		GUI.color = ((!Disabled) ? ColorTextActive : ColorTextDisabled) * color;
		if (sizeMode == FloatMenuSizeMode.Tiny)
		{
			((Rect)(ref rect2)).y = ((Rect)(ref rect2)).y + 1f;
		}
		Widgets.DrawAtlas(rect, TexUI.FloatMenuOptionBG);
		Text.Anchor = (TextAnchor)3;
		Widgets.Label(rect2, Label);
		Text.Anchor = (TextAnchor)0;
		GUI.color = new Color(iconColor.r, iconColor.g, iconColor.b, iconColor.a * GUI.color.a);
		if (shownItem != null || drawPlaceHolderIcon)
		{
			ThingStyleDef thingStyleDef = thingStyle ?? ((shownItem == null || Find.World == null) ? null : Faction.OfPlayer.ideos?.PrimaryIdeo?.GetStyleFor(shownItem));
			if (forceBasicStyle)
			{
				thingStyleDef = null;
			}
			Color value = (forceThingColor.HasValue ? forceThingColor.Value : ((shownItem == null) ? Color.white : (shownItem.MadeFromStuff ? shownItem.GetColorForStuff(GenStuff.DefaultStuffFor(shownItem)) : shownItem.uiIconColor)));
			value.a *= color.a;
			Widgets.DefIcon(val2, shownItem, null, 1f, thingStyleDef, drawPlaceHolderIcon, value, null, graphicIndexOverride);
		}
		else if (Object.op_Implicit((Object)(object)iconTex))
		{
			Widgets.DrawTextureFitted(val2, (Texture)(object)iconTex, 1f, new Vector2(1f, 1f), iconTexCoords);
		}
		else if (iconThing != null)
		{
			Widgets.ThingIcon(val2, iconThing, color.a);
		}
		GUI.color = color;
		if (extraPartOnGUI != null)
		{
			bool num3 = extraPartOnGUI(val3);
			GUI.color = color;
			if (num3)
			{
				return true;
			}
		}
		if (flag && mouseoverGuiAction != null)
		{
			mouseoverGuiAction(rect);
		}
		if (tutorTag != null)
		{
			UIHighlighter.HighlightOpportunity(rect, tutorTag);
		}
		if (Widgets.ButtonInvisible(val))
		{
			if (tutorTag != null && !TutorSystem.AllowAction(tutorTag))
			{
				return false;
			}
			Chosen(colonistOrdering, floatMenu);
			if (tutorTag != null)
			{
				TutorSystem.Notify_Event(tutorTag);
			}
			return true;
		}
		return false;
	}

	public override string ToString()
	{
		return "FloatMenuOption(" + Label + ", " + (Disabled ? "disabled" : "enabled") + ")";
	}
}
