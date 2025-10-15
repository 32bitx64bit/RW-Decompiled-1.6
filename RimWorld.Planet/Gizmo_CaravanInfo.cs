using System.Text;
using UnityEngine;
using Verse;

namespace RimWorld.Planet;

public class Gizmo_CaravanInfo : Gizmo
{
	private Caravan caravan;

	public Gizmo_CaravanInfo(Caravan caravan)
	{
		this.caravan = caravan;
		Order = -100f;
	}

	public override float GetWidth(float maxWidth)
	{
		return Mathf.Min(520f, maxWidth);
	}

	public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		if (!caravan.Spawned)
		{
			return new GizmoResult(GizmoState.Clear);
		}
		Rect val = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
		Widgets.DrawWindowBackground(val);
		Widgets.BeginGroup(val);
		Rect rect = GenUI.AtZero(val);
		int? ticksToArrive = (caravan.pather.Moving ? new int?(CaravanArrivalTimeEstimator.EstimatedTicksToArrive(caravan, allowCaching: true)) : null);
		StringBuilder stringBuilder = new StringBuilder();
		CaravanUIUtility.DrawCaravanInfo(new CaravanUIUtility.CaravanInfo(tilesPerDay: TilesPerDayCalculator.ApproxTilesPerDay(caravan, stringBuilder), massUsage: caravan.MassUsage, massCapacity: caravan.MassCapacity, massCapacityExplanation: caravan.MassCapacityExplanation, tilesPerDayExplanation: stringBuilder.ToString(), daysWorthOfFood: caravan.DaysWorthOfFood, foragedFoodPerDay: caravan.forage.ForagedFoodPerDay, foragedFoodPerDayExplanation: caravan.forage.ForagedFoodPerDayExplanation, visibility: caravan.Visibility, visibilityExplanation: caravan.VisibilityExplanation), null, caravan.Tile, ticksToArrive, -9999f, rect, lerpMassColor: true, null, multiline: true);
		Widgets.EndGroup();
		GenUI.AbsorbClicksInRect(val);
		return new GizmoResult(GizmoState.Clear);
	}
}
