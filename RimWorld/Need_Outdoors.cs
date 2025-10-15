using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld;

public class Need_Outdoors : Need
{
	private const float Delta_IndoorsThickRoof = -0.45f;

	private const float Delta_OutdoorsThickRoof = -0.4f;

	private const float Delta_IndoorsThinRoof = -0.32f;

	private const float Minimum_IndoorsThinRoof = 0.2f;

	private const float Delta_OutdoorsThinRoof = 1f;

	private const float Delta_IndoorsNoRoof = 5f;

	private const float Delta_OutdoorsNoRoof = 8f;

	private const float DeltaFactor_InBed = 0.2f;

	private float lastEffectiveDelta;

	public override int GUIChangeArrow
	{
		get
		{
			if (IsFrozen)
			{
				return 0;
			}
			return Math.Sign(lastEffectiveDelta);
		}
	}

	public OutdoorsCategory CurCategory
	{
		get
		{
			if (CurLevel > 0.8f)
			{
				return OutdoorsCategory.Free;
			}
			if (CurLevel > 0.6f)
			{
				return OutdoorsCategory.NeedFreshAir;
			}
			if (CurLevel > 0.4f)
			{
				return OutdoorsCategory.CabinFeverLight;
			}
			if (CurLevel >= 0.2f)
			{
				return OutdoorsCategory.CabinFeverSevere;
			}
			if (CurLevel > 0.05f)
			{
				return OutdoorsCategory.Trapped;
			}
			return OutdoorsCategory.Entombed;
		}
	}

	public override bool ShowOnNeedList => !Disabled;

	private bool Disabled
	{
		get
		{
			if (!pawn.Dead)
			{
				return pawn.needs.PrefersIndoors;
			}
			return true;
		}
	}

	public Need_Outdoors(Pawn pawn)
		: base(pawn)
	{
		threshPercents = new List<float>();
		threshPercents.Add(0.8f);
		threshPercents.Add(0.6f);
		threshPercents.Add(0.4f);
		threshPercents.Add(0.2f);
		threshPercents.Add(0.05f);
	}

	public override void SetInitialLevel()
	{
		CurLevel = 1f;
	}

	public override void NeedInterval()
	{
		if (Disabled)
		{
			CurLevel = 1f;
		}
		else if (!IsFrozen)
		{
			float num = 0.2f;
			float num2 = 0f;
			bool num3 = !pawn.Spawned || pawn.Position.UsesOutdoorTemperature(pawn.Map);
			RoofDef roofDef = (pawn.Spawned ? pawn.Position.GetRoof(pawn.Map) : null);
			if (num3)
			{
				num2 = ((roofDef == null) ? 8f : ((!roofDef.isThickRoof) ? 1f : (-0.4f)));
			}
			else if (roofDef == null)
			{
				num2 = 5f;
			}
			else if (!roofDef.isThickRoof)
			{
				num2 = -0.32f;
			}
			else
			{
				num2 = -0.45f;
				num = 0f;
			}
			if (pawn.InBed() && num2 < 0f)
			{
				num2 *= 0.2f;
			}
			num2 *= 0.0025f;
			float curLevel = CurLevel;
			if (num2 < 0f)
			{
				CurLevel = Mathf.Min(CurLevel, Mathf.Max(CurLevel + num2, num));
			}
			else
			{
				CurLevel = Mathf.Min(CurLevel + num2, 1f);
			}
			lastEffectiveDelta = CurLevel - curLevel;
		}
	}
}
