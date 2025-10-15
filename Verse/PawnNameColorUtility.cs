using System.Collections.Generic;
using RimWorld;
using UnityEngine;

namespace Verse;

public static class PawnNameColorUtility
{
	private static readonly List<Color> ColorsNeutral;

	private static readonly List<Color> ColorsHostile;

	private static readonly List<Color> ColorsPrisoner;

	private static readonly Color ColorBaseNeutral;

	public static readonly Color ColorBaseHostile;

	private static readonly Color ColorBasePrisoner;

	private static readonly Color ColorSlave;

	private static readonly Color ColorColony;

	private static readonly Color ColorWildMan;

	private static readonly Color ColorUncontrolledPlayerMech;

	private const int ColorShiftCount = 10;

	private static readonly List<Color> ColorShifts;

	static PawnNameColorUtility()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		ColorsNeutral = new List<Color>();
		ColorsHostile = new List<Color>();
		ColorsPrisoner = new List<Color>();
		ColorBaseNeutral = new Color(0.4f, 0.85f, 0.9f);
		ColorBaseHostile = new Color(0.9f, 0.2f, 0.2f);
		ColorBasePrisoner = new Color(1f, 0.85f, 0.5f);
		ColorSlave = Color32.op_Implicit(new Color32((byte)222, (byte)192, (byte)22, byte.MaxValue));
		ColorColony = new Color(0.9f, 0.9f, 0.9f);
		ColorWildMan = new Color(1f, 0.8f, 1f);
		ColorUncontrolledPlayerMech = new Color(0.9f, 0.2f, 0.2f);
		ColorShifts = new List<Color>
		{
			new Color(1f, 1f, 1f),
			new Color(0.8f, 1f, 1f),
			new Color(0.8f, 0.8f, 1f),
			new Color(0.8f, 0.8f, 0.8f),
			new Color(1.2f, 1f, 1f),
			new Color(0.8f, 1.2f, 1f),
			new Color(0.8f, 1.2f, 1.2f),
			new Color(1.2f, 1.2f, 1.2f),
			new Color(1f, 1.2f, 1f),
			new Color(1.2f, 1f, 0.8f)
		};
		for (int i = 0; i < 10; i++)
		{
			ColorsNeutral.Add(RandomShiftOf(ColorBaseNeutral, i));
			ColorsHostile.Add(RandomShiftOf(ColorBaseHostile, i));
			ColorsPrisoner.Add(RandomShiftOf(ColorBasePrisoner, i));
		}
	}

	private static Color RandomShiftOf(Color color, int i)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		return new Color(Mathf.Clamp01(color.r * ColorShifts[i].r), Mathf.Clamp01(color.g * ColorShifts[i].g), Mathf.Clamp01(color.b * ColorShifts[i].b), color.a);
	}

	public static Color PawnNameColorOf(Pawn pawn)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		if (pawn.MentalStateDef != null)
		{
			return pawn.MentalStateDef.nameColor;
		}
		int index = ((pawn.Faction != null) ? (pawn.Faction.randomKey % 10) : 0);
		if (pawn.IsPrisoner)
		{
			return ColorsPrisoner[index];
		}
		if (pawn.IsSlave && SlaveRebellionUtility.IsRebelling(pawn))
		{
			return ColorBaseHostile;
		}
		if (pawn.IsSlave)
		{
			return ColorSlave;
		}
		if (pawn.IsWildMan())
		{
			return ColorWildMan;
		}
		if (pawn.Faction == null)
		{
			return ColorsNeutral[index];
		}
		if (pawn.IsColonyMechRequiringMechanitor())
		{
			return ColorUncontrolledPlayerMech;
		}
		if (pawn.Faction == Faction.OfPlayer)
		{
			return ColorColony;
		}
		if (pawn.Faction.HostileTo(Faction.OfPlayer))
		{
			return ColorsHostile[index];
		}
		return ColorsNeutral[index];
	}
}
