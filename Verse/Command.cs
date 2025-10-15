using RimWorld;
using UnityEngine;
using Verse.Sound;
using Verse.Steam;

namespace Verse;

[StaticConstructorOnStartup]
public abstract class Command : Gizmo
{
	public string defaultLabel;

	public string defaultDesc;

	public string defaultDescPostfix;

	public string mouseText;

	public Texture icon;

	public float iconAngle;

	public Vector2 iconProportions = Vector2.one;

	public Rect iconTexCoords = new Rect(0f, 0f, 1f, 1f);

	public float iconDrawScale = 1f;

	public Vector2 iconOffset;

	public Color defaultIconColor = Color.white;

	public KeyBindingDef hotKey;

	public SoundDef activateSound;

	public int groupKey = -1;

	public int groupKeyIgnoreContent = -1;

	public string tutorTag = "TutorTagNotSet";

	public bool shrinkable;

	public bool groupable = true;

	public bool hideMouseIcon;

	public Material overrideMaterial;

	public static readonly Texture2D BGTex = ContentFinder<Texture2D>.Get("UI/Widgets/DesButBG");

	public static readonly Texture2D BGTexShrunk = ContentFinder<Texture2D>.Get("UI/Widgets/DesButBG");

	public static readonly Color LowLightBgColor = new Color(0.8f, 0.8f, 0.7f, 0.5f);

	public static readonly Color LowLightIconColor = new Color(0.8f, 0.8f, 0.7f, 0.6f);

	public static readonly Color LowLightLabelColor = Color.white;

	public const float LowLightIconAlpha = 0.6f;

	protected const float InnerIconDrawScale = 0.85f;

	public virtual string Label => defaultLabel;

	public virtual string LabelCap => Label.CapitalizeFirst();

	public virtual string TopRightLabel => null;

	public virtual string Desc => defaultDesc;

	public virtual string DescPostfix => defaultDescPostfix;

	public virtual Color IconDrawColor => defaultIconColor;

	public virtual SoundDef CurActivateSound => activateSound;

	protected virtual bool DoTooltip => true;

	public virtual string HighlightTag => tutorTag;

	public virtual string TutorTagSelect => tutorTag;

	public virtual Texture2D BGTexture => BGTex;

	public virtual Texture2D BGTextureShrunk => BGTexShrunk;

	public float GetShrunkSize => 36f;

	public override float GetWidth(float maxWidth)
	{
		return 75f;
	}

	public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		return GizmoOnGUIInt(new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f), parms);
	}

	public virtual GizmoResult GizmoOnGUIShrunk(Vector2 topLeft, float size, GizmoRenderParms parms)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		parms.shrunk = true;
		return GizmoOnGUIInt(new Rect(topLeft.x, topLeft.y, size, size), parms);
	}

	protected virtual GizmoResult GizmoOnGUIInt(Rect butRect, GizmoRenderParms parms)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Tiny;
		Color val = Color.white;
		bool flag = false;
		if (Mouse.IsOver(butRect))
		{
			flag = true;
			if (!disabled)
			{
				val = GenUI.MouseoverColor;
			}
		}
		MouseoverSounds.DoRegion(butRect, SoundDefOf.Mouseover_Command);
		if (parms.highLight)
		{
			Widgets.DrawStrongHighlight(butRect.ExpandedBy(4f));
		}
		if (disabled)
		{
			parms.lowLight = true;
		}
		Material val2 = (parms.lowLight ? TexUI.GrayscaleGUI : null);
		GUI.color = (parms.lowLight ? LowLightBgColor : val);
		GenUI.DrawTextureWithMaterial(butRect, (Texture)(object)(parms.shrunk ? BGTextureShrunk : BGTexture), val2);
		GUI.color = val;
		DrawIcon(butRect, val2, parms);
		bool flag2 = false;
		GUI.color = Color.white;
		if (parms.lowLight)
		{
			GUI.color = LowLightLabelColor;
		}
		Vector2 val3 = (parms.shrunk ? new Vector2(3f, 0f) : new Vector2(5f, 3f));
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(((Rect)(ref butRect)).x + val3.x, ((Rect)(ref butRect)).y + val3.y, ((Rect)(ref butRect)).width - 10f, Text.LineHeight);
		if (SteamDeck.IsSteamDeckInNonKeyboardMode)
		{
			if (parms.isFirst)
			{
				GUI.DrawTexture(new Rect(((Rect)(ref rect)).x, ((Rect)(ref rect)).y, 21f, 21f), (Texture)(object)TexUI.SteamDeck_ButtonA);
				if (KeyBindingDefOf.Accept.KeyDownEvent)
				{
					flag2 = true;
					Event.current.Use();
				}
			}
		}
		else
		{
			KeyCode val4 = (KeyCode)((hotKey != null) ? ((int)hotKey.MainKey) : 0);
			if ((int)val4 != 0 && !GizmoGridDrawer.drawnHotKeys.Contains(val4))
			{
				Widgets.Label(rect, val4.ToStringReadable());
				GizmoGridDrawer.drawnHotKeys.Add(val4);
				if (hotKey.KeyDownEvent)
				{
					flag2 = true;
					Event.current.Use();
				}
			}
		}
		if (GizmoGridDrawer.customActivator != null && GizmoGridDrawer.customActivator(this))
		{
			flag2 = true;
		}
		if (Widgets.ButtonInvisible(butRect))
		{
			flag2 = true;
		}
		if (!parms.shrunk)
		{
			string topRightLabel = TopRightLabel;
			if (!topRightLabel.NullOrEmpty())
			{
				Vector2 val5 = Text.CalcSize(topRightLabel);
				Rect val6;
				Rect rect2 = (val6 = new Rect(((Rect)(ref butRect)).xMax - val5.x - 2f, ((Rect)(ref butRect)).y + 3f, val5.x, val5.y));
				((Rect)(ref val6)).x = ((Rect)(ref val6)).x - 2f;
				((Rect)(ref val6)).width = ((Rect)(ref val6)).width + 3f;
				Text.Anchor = (TextAnchor)2;
				GUI.DrawTexture(val6, (Texture)(object)TexUI.GrayTextBG);
				Widgets.Label(rect2, topRightLabel);
				Text.Anchor = (TextAnchor)0;
			}
			string labelCap = LabelCap;
			if (!labelCap.NullOrEmpty())
			{
				float num = Text.CalcHeight(labelCap, ((Rect)(ref butRect)).width + 0.1f);
				Rect val7 = new Rect(((Rect)(ref butRect)).x, ((Rect)(ref butRect)).yMax - num + 12f, ((Rect)(ref butRect)).width, num);
				GUI.DrawTexture(val7, (Texture)(object)TexUI.GrayTextBG);
				Text.Anchor = (TextAnchor)1;
				Widgets.Label(val7, labelCap);
				Text.Anchor = (TextAnchor)0;
			}
			GUI.color = Color.white;
		}
		if (Mouse.IsOver(butRect) && DoTooltip)
		{
			TipSignal tip = Desc;
			if (disabled && !disabledReason.NullOrEmpty())
			{
				tip.text += ("\n\n" + "DisabledCommand".Translate() + ": " + disabledReason).Colorize(ColorLibrary.RedReadable);
			}
			tip.text += DescPostfix;
			TooltipHandler.TipRegion(butRect, tip);
		}
		if (!HighlightTag.NullOrEmpty() && (Find.WindowStack.FloatMenu == null || !((Rect)(ref Find.WindowStack.FloatMenu.windowRect)).Overlaps(butRect)))
		{
			UIHighlighter.HighlightOpportunity(butRect, HighlightTag);
		}
		Text.Font = GameFont.Small;
		if (flag2)
		{
			if (disabled)
			{
				if (!disabledReason.NullOrEmpty())
				{
					Messages.Message("DisabledCommand".Translate() + ": " + disabledReason, MessageTypeDefOf.RejectInput, historical: false);
				}
				return new GizmoResult(GizmoState.Mouseover, null);
			}
			GizmoResult result;
			if (Event.current.button == 1)
			{
				result = new GizmoResult(GizmoState.OpenedFloatMenu, Event.current);
			}
			else
			{
				if (!TutorSystem.AllowAction(TutorTagSelect))
				{
					return new GizmoResult(GizmoState.Mouseover, null);
				}
				result = new GizmoResult(GizmoState.Interacted, Event.current);
				TutorSystem.Notify_Event(TutorTagSelect);
			}
			return result;
		}
		if (flag)
		{
			return new GizmoResult(GizmoState.Mouseover, null);
		}
		return new GizmoResult(GizmoState.Clear, null);
	}

	public virtual void DrawIcon(Rect rect, Material buttonMat, GizmoRenderParms parms)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		Texture badTex = icon;
		if ((Object)(object)badTex == (Object)null)
		{
			badTex = (Texture)(object)BaseContent.BadTex;
		}
		((Rect)(ref rect)).position = ((Rect)(ref rect)).position + new Vector2(iconOffset.x * ((Rect)(ref rect)).size.x, iconOffset.y * ((Rect)(ref rect)).size.y);
		if (!disabled || parms.lowLight)
		{
			GUI.color = IconDrawColor;
		}
		else
		{
			GUI.color = IconDrawColor.SaturationChanged(0f);
		}
		if (parms.lowLight)
		{
			GUI.color = GUI.color.ToTransparent(0.6f);
		}
		Widgets.DrawTextureFitted(rect, badTex, iconDrawScale * 0.85f, iconProportions, iconTexCoords, iconAngle, overrideMaterial ?? buttonMat);
		GUI.color = Color.white;
	}

	public override bool GroupsWith(Gizmo other)
	{
		if (!groupable)
		{
			return false;
		}
		if (!(other is Command { groupable: not false } command))
		{
			return false;
		}
		if (hotKey == command.hotKey && Label == command.Label && (Object)(object)icon == (Object)(object)command.icon && groupKey == command.groupKey)
		{
			return true;
		}
		if (groupKeyIgnoreContent == -1 || command.groupKeyIgnoreContent == -1)
		{
			return false;
		}
		if (groupKeyIgnoreContent == command.groupKeyIgnoreContent)
		{
			return true;
		}
		return false;
	}

	public override void ProcessInput(Event ev)
	{
		if (CurActivateSound != null)
		{
			CurActivateSound.PlayOneShotOnCamera();
		}
	}

	public override string ToString()
	{
		return "Command(label=" + defaultLabel + ", defaultDesc=" + defaultDesc + ")";
	}
}
