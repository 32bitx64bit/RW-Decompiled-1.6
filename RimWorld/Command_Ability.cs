using System.Collections.Generic;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

[StaticConstructorOnStartup]
public class Command_Ability : Command
{
	protected Ability ability;

	private List<Command_Ability> groupedCasts;

	private string pawnLabel;

	private string originalLabel;

	private string pawnTooltip;

	public bool devGizmo;

	public new static readonly Texture2D BGTex = ContentFinder<Texture2D>.Get("UI/Widgets/AbilityButBG");

	public new static readonly Texture2D BGTexShrunk = ContentFinder<Texture2D>.Get("UI/Widgets/AbilityButBGShrunk");

	private static readonly Texture2D cooldownBarTex = SolidColorMaterials.NewSolidColorTexture(Color32.op_Implicit(new Color32((byte)9, (byte)203, (byte)4, (byte)64)));

	public Ability Ability => ability;

	public override Texture2D BGTexture => BGTex;

	public override Texture2D BGTextureShrunk => BGTexShrunk;

	public Pawn Pawn { get; private set; }

	public virtual string Tooltip => ability.Tooltip;

	public override string TopRightLabel => ability.GizmoExtraLabel;

	public override bool Disabled
	{
		get
		{
			DisabledCheck();
			return disabled;
		}
		set
		{
			disabled = value;
		}
	}

	public Command_Ability(Ability ability, Pawn pawn)
	{
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		this.ability = ability;
		Order = 5f + (((float?)ability.def.category?.displayOrder) ?? 0f) / 100f + (float)ability.def.displayOrder / 1000f + (float)ability.def.level / 10000f;
		defaultLabel = ability.def.LabelCap;
		hotKey = ability.def.hotKey;
		icon = (Texture)(object)ability.def.uiIcon;
		shrinkable = true;
		Pawn = pawn;
		originalLabel = defaultLabel;
		string text = LabelCap.Colorize(ColoredText.TipSectionTitleColor);
		if (pawn.Name != null)
		{
			string text2 = " (" + pawn.Name.ToStringShort + ")";
			pawnLabel = defaultLabel + text2;
			pawnTooltip = Tooltip.Insert(text.Length, text2);
		}
		else
		{
			pawnLabel = defaultLabel;
			pawnTooltip = Tooltip;
		}
	}

	public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		defaultLabel = (parms.multipleSelected ? pawnLabel : originalLabel);
		if (devGizmo)
		{
			defaultLabel = "DEV: " + defaultLabel;
		}
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
		GizmoResult result = base.GizmoOnGUI(topLeft, maxWidth, parms);
		bool flag = (ability.Casting || KeyBindingDefOf.QueueOrder.IsDownEvent) && !ability.CanQueueCast;
		if (flag)
		{
			Widgets.FillableBar(val, 0f, cooldownBarTex, null, doBorder: false);
		}
		else if (ability.CooldownTicksRemaining > 0)
		{
			float num = Mathf.InverseLerp((float)ability.CooldownTicksTotal, 0f, (float)ability.CooldownTicksRemaining);
			Widgets.FillableBar(val, Mathf.Clamp01(num), cooldownBarTex, null, doBorder: false);
			if (ability.CooldownTicksRemaining > 0)
			{
				Text.Font = GameFont.Tiny;
				string text = ability.CooldownTicksRemaining.ToStringTicksToPeriod();
				Vector2 val2 = Text.CalcSize(text);
				val2.x += 2f;
				Rect rect = val;
				((Rect)(ref rect)).x = ((Rect)(ref val)).x + ((Rect)(ref val)).width / 2f - val2.x / 2f;
				((Rect)(ref rect)).width = val2.x;
				((Rect)(ref rect)).height = val2.y;
				Rect val3 = rect.ExpandedBy(8f, 0f);
				Text.Anchor = (TextAnchor)1;
				GUI.DrawTexture(val3, (Texture)(object)TexUI.GrayTextBG);
				Widgets.Label(rect, text);
				Text.Anchor = (TextAnchor)0;
			}
		}
		if (result.State == GizmoState.Interacted && (bool)ability.CanCast && !flag)
		{
			return result;
		}
		return new GizmoResult(result.State);
	}

	protected override GizmoResult GizmoOnGUIInt(Rect butRect, GizmoRenderParms parms)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (Mouse.IsOver(butRect))
		{
			if (parms.multipleSelected)
			{
				if (Pawn.Map != null)
				{
					GenUI.DrawArrowPointingAtWorldspace(Pawn.DrawPos, Find.Camera);
				}
				defaultDesc = pawnTooltip;
			}
			else
			{
				defaultDesc = Tooltip;
			}
		}
		DisabledCheck();
		return base.GizmoOnGUIInt(butRect, parms);
	}

	public override bool GroupsWith(Gizmo other)
	{
		if (ability.def.groupAbility && other is Command_Ability command_Ability)
		{
			return command_Ability.ability.def == ability.def;
		}
		return false;
	}

	public override bool ShowPawnDetailsWith(Gizmo other)
	{
		if (other is Command_Ability command_Ability)
		{
			return command_Ability.ability.def == ability.def;
		}
		return false;
	}

	public virtual void GroupAbilityCommands(List<Gizmo> group)
	{
		if (groupedCasts == null)
		{
			groupedCasts = new List<Command_Ability>();
		}
		groupedCasts.Clear();
		for (int i = 0; i < group.Count; i++)
		{
			if (group[i].GroupsWith(this))
			{
				groupedCasts.Add((Command_Ability)group[i]);
			}
		}
	}

	protected virtual void DisabledCheck()
	{
		disabled = ability.GizmoDisabled(out var reason);
		if (disabled)
		{
			DisableWithReason(reason.CapitalizeFirst());
		}
	}

	private ITargetingSource GetBetterTargetingSource(LocalTargetInfo t)
	{
		groupedCasts.SortBy((Command_Ability c) => c.ability.pawn.Position.DistanceToSquared(t.Cell));
		for (int i = 0; i < groupedCasts.Count; i++)
		{
			if (groupedCasts[i].ability.CanQueueCast && groupedCasts[i].ability.verb.ValidateTarget(t, showMessages: false))
			{
				return groupedCasts[i].ability.verb;
			}
		}
		return null;
	}

	public override void ProcessInput(Event ev)
	{
		base.ProcessInput(ev);
		SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
		if (ability.def.targetRequired)
		{
			Find.DesignatorManager.Deselect();
			if (!ability.def.targetWorldCell)
			{
				if (groupedCasts.NullOrEmpty())
				{
					Find.Targeter.BeginTargeting(ability.verb);
				}
				else
				{
					Find.Targeter.BeginTargeting(ability.verb, null, allowNonSelectedTargetingSource: false, GetBetterTargetingSource);
				}
				return;
			}
			CameraJumper.TryJump(CameraJumper.GetWorldTarget(ability.pawn));
			Find.WorldTargeter.BeginTargeting(delegate(GlobalTargetInfo t)
			{
				if (ability.ValidateGlobalTarget(t))
				{
					ability.QueueCastingJob(t);
					return true;
				}
				return false;
			}, canTargetTiles: true, ability.def.uiIcon, !ability.pawn.IsCaravanMember(), null, ability.WorldMapExtraLabel, ability.ValidateGlobalTarget);
		}
		else
		{
			ability.QueueCastingJob(ability.pawn, LocalTargetInfo.Invalid);
		}
	}

	public override void GizmoUpdateOnMouseover()
	{
		if (ability.verb is Verb_CastAbility verb_CastAbility)
		{
			verb_CastAbility.verbProps.DrawRadiusRing(verb_CastAbility.caster.Position, verb_CastAbility);
		}
		ability.OnGizmoUpdate();
	}

	protected void DisableWithReason(string reason)
	{
		disabledReason = reason;
		disabled = true;
	}
}
