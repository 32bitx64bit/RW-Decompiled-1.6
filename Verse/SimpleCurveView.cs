using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Verse;

public class SimpleCurveView
{
	public Rect rect;

	private Dictionary<object, float> debugInputValues = new Dictionary<object, float>();

	private const float ResetZoomBuffer = 0.1f;

	private static Rect identityRect = new Rect(0f, 0f, 1f, 1f);

	public IEnumerable<float> DebugInputValues
	{
		get
		{
			if (debugInputValues == null)
			{
				yield break;
			}
			foreach (float value in debugInputValues.Values)
			{
				yield return value;
			}
		}
	}

	public void SetDebugInput(object key, float value)
	{
		debugInputValues[key] = value;
	}

	public void ClearDebugInputFrom(object key)
	{
		if (debugInputValues.ContainsKey(key))
		{
			debugInputValues.Remove(key);
		}
	}

	public void SetViewRectAround(SimpleCurve curve)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (curve.PointsCount == 0)
		{
			rect = identityRect;
			return;
		}
		((Rect)(ref rect)).xMin = curve.Points.Select((CurvePoint pt) => pt.Loc.x).Min();
		((Rect)(ref rect)).xMax = curve.Points.Select((CurvePoint pt) => pt.Loc.x).Max();
		((Rect)(ref rect)).yMin = curve.Points.Select((CurvePoint pt) => pt.Loc.y).Min();
		((Rect)(ref rect)).yMax = curve.Points.Select((CurvePoint pt) => pt.Loc.y).Max();
		if (Mathf.Approximately(((Rect)(ref rect)).width, 0f))
		{
			((Rect)(ref rect)).width = ((Rect)(ref rect)).xMin * 2f;
		}
		if (Mathf.Approximately(((Rect)(ref rect)).height, 0f))
		{
			((Rect)(ref rect)).height = ((Rect)(ref rect)).yMin * 2f;
		}
		if (Mathf.Approximately(((Rect)(ref rect)).width, 0f))
		{
			((Rect)(ref rect)).width = 1f;
		}
		if (Mathf.Approximately(((Rect)(ref rect)).height, 0f))
		{
			((Rect)(ref rect)).height = 1f;
		}
		float width = ((Rect)(ref rect)).width;
		float height = ((Rect)(ref rect)).height;
		ref Rect reference = ref rect;
		((Rect)(ref reference)).xMin = ((Rect)(ref reference)).xMin - width * 0.1f;
		ref Rect reference2 = ref rect;
		((Rect)(ref reference2)).xMax = ((Rect)(ref reference2)).xMax + width * 0.1f;
		ref Rect reference3 = ref rect;
		((Rect)(ref reference3)).yMin = ((Rect)(ref reference3)).yMin - height * 0.1f;
		ref Rect reference4 = ref rect;
		((Rect)(ref reference4)).yMax = ((Rect)(ref reference4)).yMax + height * 0.1f;
	}
}
