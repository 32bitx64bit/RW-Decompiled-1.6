using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld;

[StaticConstructorOnStartup]
public class Command_SetNeuralSuperchargerAutoUse : Command
{
	private static Texture2D autoUseForEveryone;

	private static Texture2D autoUseWithDesire;

	private static Texture2D noAutoUseTex;

	private readonly CompNeuralSupercharger comp;

	private static Texture2D AutoUseForEveryone
	{
		get
		{
			if ((Object)(object)autoUseForEveryone == (Object)null)
			{
				autoUseForEveryone = ContentFinder<Texture2D>.Get("UI/Gizmos/NeuralSupercharger_EveryoneAutoUse");
			}
			return autoUseForEveryone;
		}
	}

	private static Texture2D AutoUseWithDesire
	{
		get
		{
			if ((Object)(object)autoUseWithDesire == (Object)null)
			{
				autoUseWithDesire = ContentFinder<Texture2D>.Get("UI/Gizmos/NeuralSupercharger_AutoUseWithDesire");
			}
			return autoUseWithDesire;
		}
	}

	private static Texture2D NoAutoUseTex
	{
		get
		{
			if ((Object)(object)noAutoUseTex == (Object)null)
			{
				noAutoUseTex = ContentFinder<Texture2D>.Get("UI/Gizmos/NeuralSupercharger_NoAutoUse");
			}
			return noAutoUseTex;
		}
	}

	public Command_SetNeuralSuperchargerAutoUse(CompNeuralSupercharger comp)
	{
		this.comp = comp;
		switch (comp.autoUseMode)
		{
		case CompNeuralSupercharger.AutoUseMode.NoAutoUse:
			defaultLabel = "CommandNeuralSuperchargerNoAutoUse".Translate();
			defaultDesc = "CommandNeuralSuperchargerNoAutoUseDescription".Translate();
			icon = (Texture)(object)NoAutoUseTex;
			break;
		case CompNeuralSupercharger.AutoUseMode.AutoUseWithDesire:
			defaultLabel = "CommandNeuralSuperchargerAutoUseWithDesire".Translate();
			defaultDesc = "CommandNeuralSuperchargerAutoUseWithDesireDescription".Translate();
			icon = (Texture)(object)AutoUseWithDesire;
			break;
		case CompNeuralSupercharger.AutoUseMode.AutoUseForEveryone:
			defaultLabel = "CommandNeuralSuperchargerAutoForEveryone".Translate();
			defaultDesc = "CommandNeuralSuperchargerAutoForEveryoneDescription".Translate();
			icon = (Texture)(object)AutoUseForEveryone;
			break;
		default:
			Log.Error($"Unknown auto use mode: {comp.autoUseMode}");
			break;
		}
	}

	public override void ProcessInput(Event ev)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		base.ProcessInput(ev);
		List<FloatMenuOption> list = new List<FloatMenuOption>();
		list.Add(new FloatMenuOption("CommandNeuralSuperchargerNoAutoUse".Translate(), delegate
		{
			comp.autoUseMode = CompNeuralSupercharger.AutoUseMode.NoAutoUse;
		}, NoAutoUseTex, Color.white));
		list.Add(new FloatMenuOption("CommandNeuralSuperchargerAutoUseWithDesire".Translate(), delegate
		{
			comp.autoUseMode = CompNeuralSupercharger.AutoUseMode.AutoUseWithDesire;
		}, AutoUseWithDesire, Color.white));
		list.Add(new FloatMenuOption("CommandNeuralSuperchargerAutoForEveryone".Translate(), delegate
		{
			comp.autoUseMode = CompNeuralSupercharger.AutoUseMode.AutoUseForEveryone;
		}, AutoUseForEveryone, Color.white));
		Find.WindowStack.Add(new FloatMenu(list));
	}
}
