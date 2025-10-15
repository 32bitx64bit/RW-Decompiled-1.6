using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Verse;

public class SimpleCurve : IEnumerable<CurvePoint>, IEnumerable, IExposable
{
	private List<CurvePoint> points = new List<CurvePoint>();

	[Unsaved(false)]
	private SimpleCurveView view;

	private static Comparison<CurvePoint> CurvePointsComparer = delegate(CurvePoint a, CurvePoint b)
	{
		if (a.x < b.x)
		{
			return -1;
		}
		return (b.x < a.x) ? 1 : 0;
	};

	public int PointsCount => points.Count;

	public List<CurvePoint> Points => points;

	public bool HasView => view != null;

	public SimpleCurveView View
	{
		get
		{
			if (view == null)
			{
				view = new SimpleCurveView();
				view.SetViewRectAround(this);
			}
			return view;
		}
	}

	public float MinY => points.Min((CurvePoint point) => point.y);

	public float MaxY => points.Max((CurvePoint point) => point.y);

	public CurvePoint this[int i]
	{
		get
		{
			return points[i];
		}
		set
		{
			points[i] = value;
		}
	}

	public SimpleCurve(IEnumerable<CurvePoint> points)
	{
		SetPoints(points);
	}

	public SimpleCurve()
	{
	}

	public void ExposeData()
	{
		Scribe_Collections.Look(ref points, "points", LookMode.Value);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public IEnumerator<CurvePoint> GetEnumerator()
	{
		foreach (CurvePoint point in points)
		{
			yield return point;
		}
	}

	public void SetPoints(IEnumerable<CurvePoint> newPoints)
	{
		points.Clear();
		foreach (CurvePoint newPoint in newPoints)
		{
			points.Add(newPoint);
		}
		SortPoints();
	}

	public void Add(float x, float y, bool sort = true)
	{
		CurvePoint newPoint = new CurvePoint(x, y);
		Add(newPoint, sort);
	}

	public void Add(CurvePoint newPoint, bool sort = true)
	{
		points.Add(newPoint);
		if (sort)
		{
			SortPoints();
		}
	}

	public void SortPoints()
	{
		points.Sort(CurvePointsComparer);
	}

	public float ClampToCurve(float value)
	{
		if (points.Count == 0)
		{
			Log.Error("Clamping a value to an empty SimpleCurve.");
			return value;
		}
		return Mathf.Clamp(value, points[0].y, points[points.Count - 1].y);
	}

	public void RemovePointNear(CurvePoint point)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < points.Count; i++)
		{
			Vector2 val = points[i].Loc - point.Loc;
			if (((Vector2)(ref val)).sqrMagnitude < 0.001f)
			{
				points.RemoveAt(i);
				break;
			}
		}
	}

	public float Evaluate(float x)
	{
		if (points.Count == 0)
		{
			Log.Error("Evaluating a SimpleCurve with no points.");
			return 0f;
		}
		if (x <= points[0].x)
		{
			return points[0].y;
		}
		if (x >= points[points.Count - 1].x)
		{
			return points[points.Count - 1].y;
		}
		CurvePoint curvePoint = points[0];
		CurvePoint curvePoint2 = points[points.Count - 1];
		for (int i = 0; i < points.Count; i++)
		{
			if (x <= points[i].x)
			{
				curvePoint2 = points[i];
				if (i > 0)
				{
					curvePoint = points[i - 1];
				}
				break;
			}
		}
		float num = (x - curvePoint.x) / (curvePoint2.x - curvePoint.x);
		return Mathf.Lerp(curvePoint.y, curvePoint2.y, num);
	}

	public float EvaluateInverted(float y)
	{
		if (points.Count == 0)
		{
			Log.Error("Evaluating a SimpleCurve with no points.");
			return 0f;
		}
		if (points.Count == 1)
		{
			return points[0].x;
		}
		for (int i = 0; i < points.Count - 1; i++)
		{
			if ((y >= points[i].y && y <= points[i + 1].y) || (y <= points[i].y && y >= points[i + 1].y))
			{
				if (y == points[i].y)
				{
					return points[i].x;
				}
				if (y == points[i + 1].y)
				{
					return points[i + 1].x;
				}
				return GenMath.LerpDouble(points[i].y, points[i + 1].y, points[i].x, points[i + 1].x, y);
			}
		}
		if (y < points[0].y)
		{
			float result = 0f;
			float num = 0f;
			for (int j = 0; j < points.Count; j++)
			{
				if (j == 0 || points[j].y < num)
				{
					num = points[j].y;
					result = points[j].x;
				}
			}
			return result;
		}
		float result2 = 0f;
		float num2 = 0f;
		for (int k = 0; k < points.Count; k++)
		{
			if (k == 0 || points[k].y > num2)
			{
				num2 = points[k].y;
				result2 = points[k].x;
			}
		}
		return result2;
	}

	public float PeriodProbabilityFromCumulative(float startX, float span)
	{
		if (points.Count < 2)
		{
			return 0f;
		}
		if (points[0].y != 0f)
		{
			Log.Warning("PeriodProbabilityFromCumulative should only run on curves whose first point is 0.");
		}
		float num = Evaluate(startX + span) - Evaluate(startX);
		if (num < 0f)
		{
			Log.Error("PeriodicProbability got negative probability from " + this?.ToString() + ": slope should never be negative.");
			num = 0f;
		}
		if (num > 1f)
		{
			num = 1f;
		}
		return num;
	}

	public IEnumerable<string> ConfigErrors(string prefix)
	{
		for (int i = 0; i < points.Count - 1; i++)
		{
			if (points[i + 1].x < points[i].x)
			{
				yield return prefix + ": points are out of order";
				break;
			}
		}
	}

	public static SimpleCurve Empty()
	{
		return new SimpleCurve { new CurvePoint(0f, 0f) };
	}

	public AnimationCurve ToAnimationCurve()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		AnimationCurve val = new AnimationCurve();
		foreach (CurvePoint point in Points)
		{
			val.AddKey(point.x, point.y);
		}
		return val;
	}

	public static SimpleCurve FromAnimationCurve(AnimationCurve curve)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		SimpleCurve simpleCurve = new SimpleCurve();
		Keyframe[] keys = curve.keys;
		for (int i = 0; i < keys.Length; i++)
		{
			Keyframe val = keys[i];
			simpleCurve.Points.Add(new CurvePoint(((Keyframe)(ref val)).time, ((Keyframe)(ref val)).value));
		}
		return simpleCurve;
	}
}
