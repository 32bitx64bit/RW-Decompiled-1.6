using System.Linq;
using UnityEngine;
using Verse;

namespace RimWorld;

public class Verb_CastPsycast : Verb_CastAbility
{
	private const float StatLabelOffsetY = 1f;

	public Psycast Psycast => ability as Psycast;

	public override bool IsApplicableTo(LocalTargetInfo target, bool showMessages = false)
	{
		if (!ModLister.CheckRoyalty("Psycast"))
		{
			return false;
		}
		if (!base.IsApplicableTo(target, showMessages))
		{
			return false;
		}
		if (!Psycast.def.HasAreaOfEffect && !Psycast.CanApplyPsycastTo(target))
		{
			if (showMessages)
			{
				Messages.Message(ability.def.LabelCap + ": " + "AbilityTargetPsychicallyDeaf".Translate(), target.ToTargetInfo(ability.pawn.Map), MessageTypeDefOf.RejectInput, historical: false);
			}
			return false;
		}
		return true;
	}

	public override void OrderForceTarget(LocalTargetInfo target)
	{
		if (IsApplicableTo(target))
		{
			base.OrderForceTarget(target);
		}
	}

	public override bool ValidateTarget(LocalTargetInfo target, bool showMessages = true)
	{
		if (!base.ValidateTarget(target, showMessages))
		{
			return false;
		}
		bool num = ability.EffectComps.All((CompAbilityEffect e) => e.Props.canTargetBosses);
		Pawn pawn = target.Pawn;
		if (!num && pawn != null && pawn.kindDef.isBoss)
		{
			Messages.Message("CommandPsycastInsanityImmune".Translate(), caster, MessageTypeDefOf.RejectInput, historical: false);
			return false;
		}
		if (CasterPawn.psychicEntropy.PsychicSensitivity < float.Epsilon)
		{
			Messages.Message("CommandPsycastZeroPsychicSensitivity".Translate(), caster, MessageTypeDefOf.RejectInput, historical: false);
			return false;
		}
		if (Psycast.def.EntropyGain > float.Epsilon && CasterPawn.psychicEntropy.WouldOverflowEntropy(Psycast.def.EntropyGain + PsycastUtility.TotalEntropyFromQueuedPsycasts(CasterPawn)))
		{
			Messages.Message("CommandPsycastWouldExceedEntropy".Translate(), caster, MessageTypeDefOf.RejectInput, historical: false);
			return false;
		}
		float num2 = Psycast.FinalPsyfocusCost(target);
		float num3 = PsycastUtility.TotalPsyfocusCostOfQueuedPsycasts(CasterPawn);
		float num4 = num2 + num3;
		if (num2 > float.Epsilon && num4 > CasterPawn.psychicEntropy.CurrentPsyfocus + 0.0005f)
		{
			Messages.Message("CommandPsycastNotEnoughPsyfocus".Translate(num4.ToStringPercent("0.#"), (CasterPawn.psychicEntropy.CurrentPsyfocus - num3).ToStringPercent("0.#"), Psycast.def.label.Named("PSYCASTNAME"), caster.Named("CASTERNAME")), caster, MessageTypeDefOf.RejectInput, historical: false);
			return false;
		}
		return true;
	}

	public override void OnGUI(LocalTargetInfo target)
	{
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		bool flag = ability.EffectComps.Any((CompAbilityEffect e) => e.Props.psychic);
		bool flag2 = ability.EffectComps.All((CompAbilityEffect e) => e.Props.canTargetBosses);
		Texture2D val = UIIcon;
		if (!Psycast.CanApplyPsycastTo(target))
		{
			val = TexCommand.CannotShoot;
			DrawIneffectiveWarning(target);
		}
		if (target.IsValid && CanHitTarget(target) && IsApplicableTo(target))
		{
			foreach (LocalTargetInfo affectedTarget in ability.GetAffectedTargets(target))
			{
				if (!flag2 && affectedTarget.Pawn.kindDef.isBoss)
				{
					DrawIneffectiveWarning(affectedTarget, "IneffectivePsychicImmune".Translate());
				}
				else if (flag)
				{
					if (Psycast.CanApplyPsycastTo(affectedTarget))
					{
						DrawSensitivityStat(affectedTarget);
					}
					else
					{
						DrawIneffectiveWarning(affectedTarget);
					}
				}
			}
			if (ability.EffectComps.Any((CompAbilityEffect e) => !e.Valid(target)))
			{
				val = TexCommand.CannotShoot;
			}
		}
		else
		{
			val = TexCommand.CannotShoot;
		}
		if (ThingRequiringRoyalPermissionUtility.IsViolatingRulesOfAnyFaction(HediffDefOf.PsychicAmplifier, CasterPawn, Psycast.def.level) && Psycast.def.DetectionChance > 0f)
		{
			TaggedString taggedString = ((string)"Illegal".Translate()).ToUpper() + "\n" + Psycast.def.DetectionChance.ToStringPercent() + " " + "DetectionChance".Translate();
			Text.Font = GameFont.Small;
			Texture2D iconTex = val;
			string text = taggedString;
			Color textBgColor = default(Color);
			((Color)(ref textBgColor))._002Ector(0.25f, 0f, 0f);
			GenUI.DrawMouseAttachment((Texture)(object)iconTex, text, 0f, default(Vector2), null, null, drawTextBackground: true, textBgColor);
		}
		else
		{
			GenUI.DrawMouseAttachment(val);
		}
		DrawAttachmentExtraLabel(target);
	}

	private void DrawIneffectiveWarning(LocalTargetInfo target, string text = null)
	{
		DrawIneffectiveWarningStatic(target, text);
	}

	public static void DrawIneffectiveWarningStatic(LocalTargetInfo target, string text = null)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		if (target.Pawn != null)
		{
			Vector3 drawPos = target.Pawn.DrawPos;
			drawPos.z += 1f;
			if (text == null)
			{
				text = "Ineffective".Translate();
			}
			GenMapUI.DrawText(new Vector2(drawPos.x, drawPos.z), text, Color.red);
		}
	}

	private void DrawSensitivityStat(LocalTargetInfo target)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		if (target.Pawn != null && !target.Pawn.IsHiddenFromPlayer())
		{
			Pawn pawn = target.Pawn;
			float statValue = pawn.GetStatValue(StatDefOf.PsychicSensitivity);
			Vector3 drawPos = pawn.DrawPos;
			drawPos.z += 1f;
			GenMapUI.DrawText(new Vector2(drawPos.x, drawPos.z), string.Concat(StatDefOf.PsychicSensitivity.LabelCap + ": ", statValue.ToString()), (statValue > float.Epsilon) ? Color.white : Color.red);
		}
	}
}
