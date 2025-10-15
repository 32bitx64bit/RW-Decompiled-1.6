using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RimWorld;

public static class PawnHairColors
{
	public static readonly Color DarkBlack = new Color(0.2f, 0.2f, 0.2f);

	public static readonly Color MidBlack = new Color(0.31f, 0.28f, 0.26f);

	public static readonly Color DarkReddish = new Color(0.25f, 0.2f, 0.15f);

	public static readonly Color DarkSaturatedReddish = new Color(0.3f, 0.2f, 0.1f);

	public static readonly Color DarkBrown = new Color(0.3529412f, 0.22745098f, 0.1254902f);

	public static readonly Color ReddishBrown = new Color(44f / 85f, 0.3254902f, 0.18431373f);

	public static readonly Color SandyBlonde = new Color(0.75686276f, 0.57254905f, 1f / 3f);

	public static readonly Color Blonde = new Color(79f / 85f, 0.7921569f, 52f / 85f);

	private static List<GeneDef> cachedHairColorGenes;

	private static List<GeneDef> HairColorGenes
	{
		get
		{
			if (cachedHairColorGenes == null)
			{
				cachedHairColorGenes = DefDatabase<GeneDef>.AllDefs.Where((GeneDef x) => x.hairColorOverride.HasValue).ToList();
			}
			return cachedHairColorGenes;
		}
	}

	public static void ResetStaticData()
	{
		cachedHairColorGenes = null;
	}

	public static Color RandomHairColor(Pawn pawn, Color skinColor, int ageYears)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		if (Rand.Value < 0.02f)
		{
			return new Color(Rand.Value, Rand.Value, Rand.Value);
		}
		if (HasGreyHair(pawn, ageYears))
		{
			return RandomGreyHairColor();
		}
		if (PawnSkinColors.IsDarkSkin(skinColor) || Rand.Value < 0.5f)
		{
			float value = Rand.Value;
			if (value < 0.25f)
			{
				return DarkBlack;
			}
			if (value < 0.5f)
			{
				return MidBlack;
			}
			if (value < 0.75f)
			{
				return DarkReddish;
			}
			return DarkSaturatedReddish;
		}
		float value2 = Rand.Value;
		if (value2 < 0.25f)
		{
			return DarkBrown;
		}
		if (value2 < 0.5f)
		{
			return ReddishBrown;
		}
		if (value2 < 0.75f)
		{
			return SandyBlonde;
		}
		return Blonde;
	}

	public static bool HasGreyHair(Pawn pawn, int ageYears)
	{
		if (ageYears <= 40)
		{
			return false;
		}
		if (ModsConfig.BiotechActive && pawn.genes != null)
		{
			foreach (Gene item in pawn.genes.GenesListForReading)
			{
				if (item.def.neverGrayHair)
				{
					return false;
				}
			}
		}
		float num = GenMath.SmootherStep(40f, 75f, ageYears);
		return Rand.Value < num;
	}

	public static Color RandomGreyHairColor()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		float num = Rand.Range(0.65f, 0.85f);
		return new Color(num, num, num);
	}

	public static GeneDef RandomHairColorGeneFor(Pawn pawn)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return RandomHairColorGene(pawn.story.SkinColorBase, ModsConfig.AnomalyActive && pawn.Faction == Faction.OfHoraxCult);
	}

	private static GeneDef RandomHairColorGene(Color skinColor, bool cultist = false)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (HairColorGenes.TryRandomElementByWeight(delegate(GeneDef g)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			float num = g.selectionWeight;
			if (PawnSkinColors.IsDarkSkin(skinColor))
			{
				num *= g.selectionWeightFactorDarkSkin;
			}
			if (cultist)
			{
				num *= g.selectionWeightCultist;
			}
			return num;
		}, out var result))
		{
			return result;
		}
		return null;
	}

	public static GeneDef ClosestHairColorGene(Color hairColor, Color skinColor)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		foreach (GeneDef hairColorGene in HairColorGenes)
		{
			if (hairColor.IndistinguishableFrom(hairColorGene.hairColorOverride.Value))
			{
				return hairColorGene;
			}
		}
		return RandomHairColorGene(skinColor);
	}
}
