using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

[StaticConstructorOnStartup]
public class ActivityGizmo : Gizmo_Slider
{
	private readonly ThingWithComps thing;

	private static bool draggingBar;

	private static readonly Texture2D IconTex = ContentFinder<Texture2D>.Get("UI/Icons/SuppressionToggle");

	private CompActivity Comp => thing.GetComp<CompActivity>();

	protected override string Title => "ActivityGizmo".Translate();

	protected override float ValuePercent => Comp.ActivityLevel;

	protected override bool IsDraggable => Comp.CanBeSuppressed;

	protected override FloatRange DragRange => new FloatRange(0.1f, 0.9f);

	protected override string HighlightTag => "ActivityGizmo";

	protected override bool DraggingBar
	{
		get
		{
			return draggingBar;
		}
		set
		{
			draggingBar = value;
		}
	}

	protected override float Target
	{
		get
		{
			return Comp.suppressIfAbove;
		}
		set
		{
			Comp.suppressIfAbove = value;
		}
	}

	public ActivityGizmo(ThingWithComps thing)
	{
		this.thing = thing;
	}

	protected override string GetTooltip()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("ActivitySuppressionTooltipTitle".Translate().CapitalizeFirst().Colorize(ColoredText.TipSectionTitleColor));
		stringBuilder.AppendLine();
		stringBuilder.Append("ActivitySuppressionTooltipDesc".Translate(thing.LabelNoParenthesis, Comp.suppressIfAbove.ToStringPercent("0").Colorize(ColoredText.TipSectionTitleColor).Named("LEVEL")).Resolve());
		Comp.Props.Worker.GetSummary(thing, stringBuilder);
		if (!Comp.suppressionEnabled)
		{
			stringBuilder.Append("\n\n" + "ActivitySuppressionTooltipDisabled".Translate(thing.LabelNoParenthesis).CapitalizeFirst().Colorize(ColoredText.FactionColor_Hostile));
		}
		foreach (IActivity comp in thing.GetComps<IActivity>())
		{
			if (!comp.ActivityTooltipExtra().NullOrEmpty())
			{
				stringBuilder.Append("\n\n" + comp.ActivityTooltipExtra());
			}
		}
		return stringBuilder.ToString();
	}

	protected override void DrawHeader(Rect headerRect, ref bool mouseOverElement)
	{
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		((Rect)(ref headerRect)).xMax = ((Rect)(ref headerRect)).xMax - 24f;
		if (Comp.CanBeSuppressed)
		{
			Rect val = default(Rect);
			((Rect)(ref val))._002Ector(((Rect)(ref headerRect)).xMax, ((Rect)(ref headerRect)).y, 24f, 24f);
			GUI.DrawTexture(val, (Texture)(object)IconTex);
			GUI.DrawTexture(new Rect(((Rect)(ref val)).center.x, ((Rect)(ref val)).y, ((Rect)(ref val)).width / 2f, ((Rect)(ref val)).height / 2f), (Texture)(object)(Comp.suppressionEnabled ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex));
			if (Widgets.ButtonInvisible(val))
			{
				Comp.suppressionEnabled = !Comp.suppressionEnabled;
				if (Comp.suppressionEnabled)
				{
					SoundDefOf.Tick_High.PlayOneShotOnCamera();
				}
				else
				{
					SoundDefOf.Tick_Low.PlayOneShotOnCamera();
				}
			}
			if (Mouse.IsOver(val))
			{
				Widgets.DrawHighlight(val);
				TooltipHandler.TipRegion(val, GetTooltipDesc, 828267373);
				mouseOverElement = true;
			}
		}
		base.DrawHeader(headerRect, ref mouseOverElement);
	}

	private string GetTooltipDesc()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		string arg = (Comp.suppressionEnabled ? "On" : "Off").Translate().ToString().UncapitalizeFirst();
		string arg2 = Comp.suppressIfAbove.ToStringPercent("0").Colorize(ColoredText.TipSectionTitleColor);
		return "ActivitySuppressionToggleTooltipDesc".Translate(thing.LabelNoParenthesis, arg2.Named("LEVEL"), arg.Named("ONOFF")).Resolve();
	}
}
