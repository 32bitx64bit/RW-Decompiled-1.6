using System.Collections.Generic;
using UnityEngine;

namespace Verse;

[StaticConstructorOnStartup]
public static class SimpleCurveDrawer
{
	private const float PointSize = 10f;

	private static readonly Color AxisLineColor = new Color(0.2f, 0.5f, 1f, 1f);

	private static readonly Color MajorLineColor = new Color(0.2f, 0.4f, 1f, 0.6f);

	private static readonly Color MinorLineColor = new Color(0.2f, 0.3f, 1f, 0.19f);

	private const float MeasureWidth = 60f;

	private const float MeasureHeight = 30f;

	private const float MeasureLinePeekOut = 5f;

	private const float LegendCellWidth = 140f;

	private const float LegendCellHeight = 20f;

	private static readonly Texture2D CurvePoint = ContentFinder<Texture2D>.Get("UI/Widgets/Dev/CurvePoint");

	public static void DrawCurve(Rect rect, SimpleCurve curve, SimpleCurveDrawerStyle style = null, List<CurveMark> marks = null, Rect legendScreenRect = default(Rect))
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		SimpleCurveDrawInfo simpleCurveDrawInfo = new SimpleCurveDrawInfo();
		simpleCurveDrawInfo.curve = curve;
		DrawCurve(rect, simpleCurveDrawInfo, style, marks, legendScreenRect);
	}

	public static void DrawCurve(Rect rect, SimpleCurveDrawInfo curve, SimpleCurveDrawerStyle style = null, List<CurveMark> marks = null, Rect legendScreenRect = default(Rect))
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (curve.curve != null)
		{
			List<SimpleCurveDrawInfo> list = new List<SimpleCurveDrawInfo>();
			list.Add(curve);
			DrawCurves(rect, list, style, marks, legendScreenRect);
		}
	}

	public static void DrawCurves(Rect rect, List<SimpleCurveDrawInfo> curves, SimpleCurveDrawerStyle style = null, List<CurveMark> marks = null, Rect legendRect = default(Rect))
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Invalid comparison between Unknown and I4
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		if ((int)Event.current.type != 7)
		{
			return;
		}
		if (style == null)
		{
			style = new SimpleCurveDrawerStyle();
		}
		if (curves.Count == 0)
		{
			return;
		}
		bool flag = true;
		Rect viewRect = default(Rect);
		for (int i = 0; i < curves.Count; i++)
		{
			SimpleCurveDrawInfo simpleCurveDrawInfo = curves[i];
			if (simpleCurveDrawInfo.curve != null)
			{
				if (flag)
				{
					flag = false;
					viewRect = simpleCurveDrawInfo.curve.View.rect;
					continue;
				}
				((Rect)(ref viewRect)).xMin = Mathf.Min(((Rect)(ref viewRect)).xMin, ((Rect)(ref simpleCurveDrawInfo.curve.View.rect)).xMin);
				((Rect)(ref viewRect)).xMax = Mathf.Max(((Rect)(ref viewRect)).xMax, ((Rect)(ref simpleCurveDrawInfo.curve.View.rect)).xMax);
				((Rect)(ref viewRect)).yMin = Mathf.Min(((Rect)(ref viewRect)).yMin, ((Rect)(ref simpleCurveDrawInfo.curve.View.rect)).yMin);
				((Rect)(ref viewRect)).yMax = Mathf.Max(((Rect)(ref viewRect)).yMax, ((Rect)(ref simpleCurveDrawInfo.curve.View.rect)).yMax);
			}
		}
		if (style.UseFixedScale)
		{
			((Rect)(ref viewRect)).yMin = style.FixedScale.x;
			((Rect)(ref viewRect)).yMax = style.FixedScale.y;
		}
		if (style.OnlyPositiveValues)
		{
			if (((Rect)(ref viewRect)).xMin < 0f)
			{
				((Rect)(ref viewRect)).xMin = 0f;
			}
			if (((Rect)(ref viewRect)).yMin < 0f)
			{
				((Rect)(ref viewRect)).yMin = 0f;
			}
		}
		if (style.UseFixedSection)
		{
			((Rect)(ref viewRect)).xMin = style.FixedSection.min;
			((Rect)(ref viewRect)).xMax = style.FixedSection.max;
		}
		if (Mathf.Approximately(((Rect)(ref viewRect)).width, 0f) || Mathf.Approximately(((Rect)(ref viewRect)).height, 0f))
		{
			return;
		}
		Rect val = rect;
		if (style.DrawMeasures)
		{
			((Rect)(ref val)).xMin = ((Rect)(ref val)).xMin + 60f;
			((Rect)(ref val)).yMax = ((Rect)(ref val)).yMax - 30f;
		}
		if (marks != null)
		{
			Rect rect2 = val;
			((Rect)(ref rect2)).height = 15f;
			DrawCurveMarks(rect2, viewRect, marks);
			((Rect)(ref val)).yMin = ((Rect)(ref rect2)).yMax;
		}
		if (style.DrawBackground)
		{
			GUI.color = new Color(0.302f, 0.318f, 0.365f);
			GUI.DrawTexture(val, (Texture)(object)BaseContent.WhiteTex);
		}
		if (style.DrawBackgroundLines)
		{
			DrawGraphBackgroundLines(val, viewRect);
		}
		if (style.DrawMeasures)
		{
			DrawCurveMeasures(rect, viewRect, val, style.MeasureLabelsXCount, style.MeasureLabelsYCount, style.XIntegersOnly, style.YIntegersOnly);
		}
		foreach (SimpleCurveDrawInfo curf in curves)
		{
			DrawCurveLines(val, curf, style.DrawPoints, viewRect, style.UseAntiAliasedLines, style.PointsRemoveOptimization);
		}
		if (style.DrawLegend)
		{
			DrawCurvesLegend(legendRect, curves);
		}
		if (style.DrawCurveMousePoint)
		{
			DrawCurveMousePoint(curves, val, viewRect, style.LabelX);
		}
	}

	public static void DrawCurveLines(Rect rect, SimpleCurveDrawInfo curve, bool drawPoints, Rect viewRect, bool useAALines, bool pointsRemoveOptimization)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Invalid comparison between Unknown and I4
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		if (curve.curve == null || curve.curve.PointsCount == 0)
		{
			return;
		}
		Rect rect2 = rect;
		((Rect)(ref rect2)).yMin = ((Rect)(ref rect2)).yMin - 1f;
		((Rect)(ref rect2)).yMax = ((Rect)(ref rect2)).yMax + 1f;
		Widgets.BeginGroup(rect2);
		if ((int)Event.current.type == 7)
		{
			if (useAALines)
			{
				bool flag = true;
				Vector2 val = default(Vector2);
				Vector2 curvePoint = default(Vector2);
				int num = curve.curve.Points.Count((CurvePoint x) => x.x >= ((Rect)(ref viewRect)).xMin && x.x <= ((Rect)(ref viewRect)).xMax);
				int num2 = RemovePointsOptimizationFreq(num);
				for (int i = 0; i < curve.curve.PointsCount; i++)
				{
					CurvePoint curvePoint2 = curve.curve[i];
					if (!pointsRemoveOptimization || i % num2 != 0 || i == 0 || i == num - 1)
					{
						curvePoint.x = curvePoint2.x;
						curvePoint.y = curvePoint2.y;
						Vector2 val2 = CurveToScreenCoordsInsideScreenRect(rect, viewRect, curvePoint);
						if (flag)
						{
							flag = false;
						}
						else if ((val.x >= 0f && val.x <= ((Rect)(ref rect)).width) || (val2.x >= 0f && val2.x <= ((Rect)(ref rect)).width))
						{
							Widgets.DrawLine(val, val2, curve.color, 1f);
						}
						val = val2;
					}
				}
				Vector2 val3 = CurveToScreenCoordsInsideScreenRect(rect, viewRect, curve.curve[0]);
				Vector2 val4 = CurveToScreenCoordsInsideScreenRect(rect, viewRect, curve.curve[curve.curve.PointsCount - 1]);
				Widgets.DrawLine(val3, new Vector2(0f, val3.y), curve.color, 1f);
				Widgets.DrawLine(val4, new Vector2(((Rect)(ref rect)).width, val4.y), curve.color, 1f);
			}
			else
			{
				GUI.color = curve.color;
				float num3 = ((Rect)(ref viewRect)).x;
				float num4 = ((Rect)(ref rect)).width / 1f;
				float num5 = ((Rect)(ref viewRect)).width / num4;
				while (num3 < ((Rect)(ref viewRect)).xMax)
				{
					num3 += num5;
					Vector2 val5 = CurveToScreenCoordsInsideScreenRect(rect, viewRect, new Vector2(num3, curve.curve.Evaluate(num3)));
					GUI.DrawTexture(new Rect(val5.x, val5.y, 1f, 1f), (Texture)(object)BaseContent.WhiteTex);
				}
			}
			GUI.color = Color.white;
		}
		if (drawPoints)
		{
			for (int j = 0; j < curve.curve.PointsCount; j++)
			{
				CurvePoint curvePoint3 = curve.curve[j];
				DrawPoint(CurveToScreenCoordsInsideScreenRect(rect, viewRect, curvePoint3.Loc));
			}
		}
		Vector2 curvePoint4 = default(Vector2);
		foreach (float debugInputValue in curve.curve.View.DebugInputValues)
		{
			GUI.color = new Color(0f, 1f, 0f, 0.25f);
			DrawInfiniteVerticalLine(rect, viewRect, debugInputValue);
			float num6 = curve.curve.Evaluate(debugInputValue);
			((Vector2)(ref curvePoint4))._002Ector(debugInputValue, num6);
			Vector2 screenPoint = CurveToScreenCoordsInsideScreenRect(rect, viewRect, curvePoint4);
			GUI.color = Color.green;
			DrawPoint(screenPoint);
			GUI.color = Color.white;
		}
		Widgets.EndGroup();
	}

	public static void DrawCurveMeasures(Rect rect, Rect viewRect, Rect graphRect, int xLabelsCount, int yLabelsCount, bool xIntegersOnly, bool yIntegersOnly)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Small;
		Color color = default(Color);
		((Color)(ref color))._002Ector(0.45f, 0.45f, 0.45f);
		Color color2 = default(Color);
		((Color)(ref color2))._002Ector(0.7f, 0.7f, 0.7f);
		Widgets.BeginGroup(rect);
		CalculateMeasureStartAndInc(out var start, out var inc, out var count, ((Rect)(ref viewRect)).xMin, ((Rect)(ref viewRect)).xMax, xLabelsCount, xIntegersOnly);
		Text.Anchor = (TextAnchor)1;
		string text = string.Empty;
		for (int i = 0; i < count; i++)
		{
			float num = start + inc * (float)i;
			string text2 = num.ToString("F0");
			if (!(text2 == text))
			{
				text = text2;
				float num2 = CurveToScreenCoordsInsideScreenRect(graphRect, viewRect, new Vector2(num, 0f)).x + 60f;
				float num3 = ((Rect)(ref rect)).height - 30f;
				GUI.color = color;
				Widgets.DrawLineVertical(num2, num3, 5f);
				GUI.color = color2;
				Rect rect2 = new Rect(num2 - 31f, num3 + 2f, 60f, 30f);
				Text.Font = GameFont.Tiny;
				Widgets.Label(rect2, text2);
				Text.Font = GameFont.Small;
			}
		}
		CalculateMeasureStartAndInc(out var start2, out var inc2, out var count2, ((Rect)(ref viewRect)).yMin, ((Rect)(ref viewRect)).yMax, yLabelsCount, yIntegersOnly);
		string text3 = string.Empty;
		Text.Anchor = (TextAnchor)2;
		for (int j = 0; j < count2; j++)
		{
			float num4 = start2 + inc2 * (float)j;
			string text4 = num4.ToString("F0");
			if (!(text4 == text3))
			{
				text3 = text4;
				float num5 = CurveToScreenCoordsInsideScreenRect(graphRect, viewRect, new Vector2(0f, num4)).y + (((Rect)(ref graphRect)).y - ((Rect)(ref rect)).y);
				GUI.color = color;
				Widgets.DrawLineHorizontal(55f, num5, 5f + ((Rect)(ref graphRect)).width);
				GUI.color = color2;
				Rect rect3 = new Rect(0f, num5 - 10f, 55f, 20f);
				Text.Font = GameFont.Tiny;
				Widgets.Label(rect3, text4);
				Text.Font = GameFont.Small;
			}
		}
		Widgets.EndGroup();
		GUI.color = new Color(1f, 1f, 1f);
		Text.Anchor = (TextAnchor)0;
	}

	private static void CalculateMeasureStartAndInc(out float start, out float inc, out int count, float min, float max, int wantedCount, bool integersOnly)
	{
		if (integersOnly && GenMath.AnyIntegerInRange(min, max))
		{
			int num = Mathf.CeilToInt(min);
			int num2 = Mathf.FloorToInt(max);
			start = num;
			inc = Mathf.CeilToInt((float)(num2 - num + 1) / (float)wantedCount);
			count = (num2 - num) / (int)inc + 1;
		}
		else
		{
			start = min;
			inc = (max - min) / (float)wantedCount;
			count = wantedCount;
		}
	}

	public static void DrawCurvesLegend(Rect rect, List<SimpleCurveDrawInfo> curves)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		Text.Anchor = (TextAnchor)0;
		Text.Font = GameFont.Small;
		Text.WordWrap = false;
		Widgets.BeginGroup(rect);
		float num = 0f;
		float num2 = 0f;
		int num3 = (int)(((Rect)(ref rect)).width / 140f);
		int num4 = 0;
		foreach (SimpleCurveDrawInfo curf in curves)
		{
			GUI.color = curf.color;
			GUI.DrawTexture(new Rect(num, num2 + 2f, 15f, 15f), (Texture)(object)BaseContent.WhiteTex);
			GUI.color = Color.white;
			num += 20f;
			if (curf.label != null)
			{
				Widgets.Label(new Rect(num, num2, 140f, 100f), curf.label);
			}
			num4++;
			if (num4 == num3)
			{
				num4 = 0;
				num = 0f;
				num2 += 20f;
			}
			else
			{
				num += 140f;
			}
		}
		Widgets.EndGroup();
		GUI.color = Color.white;
		Text.WordWrap = true;
	}

	public static void DrawCurveMousePoint(List<SimpleCurveDrawInfo> curves, Rect screenRect, Rect viewRect, string labelX)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		if (curves.Count == 0 || !Mouse.IsOver(screenRect))
		{
			return;
		}
		Widgets.BeginGroup(screenRect);
		Vector2 mousePosition = Event.current.mousePosition;
		Vector2 val = default(Vector2);
		Vector2 val2 = default(Vector2);
		SimpleCurveDrawInfo simpleCurveDrawInfo = null;
		bool flag = false;
		foreach (SimpleCurveDrawInfo curf in curves)
		{
			if (curf.curve.PointsCount != 0)
			{
				Vector2 val3 = ScreenToCurveCoords(screenRect, viewRect, mousePosition);
				val3.y = curf.curve.Evaluate(val3.x);
				Vector2 val4 = CurveToScreenCoordsInsideScreenRect(screenRect, viewRect, val3);
				if (!flag || Vector2.Distance(val4, mousePosition) < Vector2.Distance(val2, mousePosition))
				{
					flag = true;
					val = val3;
					val2 = val4;
					simpleCurveDrawInfo = curf;
				}
			}
		}
		if (flag)
		{
			DrawPoint(val2);
			Rect rect = default(Rect);
			((Rect)(ref rect))._002Ector(val2.x, val2.y, 120f, 60f);
			Text.Anchor = (TextAnchor)0;
			if (((Rect)(ref rect)).x + ((Rect)(ref rect)).width > ((Rect)(ref screenRect)).width)
			{
				((Rect)(ref rect)).x = ((Rect)(ref rect)).x - ((Rect)(ref rect)).width;
				Text.Anchor = (TextAnchor)2;
			}
			if (((Rect)(ref rect)).y + ((Rect)(ref rect)).height > ((Rect)(ref screenRect)).height)
			{
				((Rect)(ref rect)).y = ((Rect)(ref rect)).y - ((Rect)(ref rect)).height;
				if ((int)Text.Anchor == 0)
				{
					Text.Anchor = (TextAnchor)6;
				}
				else
				{
					Text.Anchor = (TextAnchor)8;
				}
			}
			string text = ((!simpleCurveDrawInfo.valueFormat.NullOrEmpty()) ? string.Format(simpleCurveDrawInfo.valueFormat, val.y.ToString("0.##")) : val.y.ToString("0.##"));
			Widgets.Label(rect, simpleCurveDrawInfo.label + "\n" + labelX + " " + val.x.ToString("0.##") + "\n" + text);
			Text.Anchor = (TextAnchor)0;
		}
		Widgets.EndGroup();
	}

	public static void DrawCurveMarks(Rect rect, Rect viewRect, List<CurveMark> marks)
	{
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		float x = ((Rect)(ref viewRect)).x;
		float num = ((Rect)(ref viewRect)).x + ((Rect)(ref viewRect)).width;
		float num2 = ((Rect)(ref rect)).y + 5f;
		_ = ((Rect)(ref rect)).yMax;
		Vector2 val = default(Vector2);
		Rect rect2 = default(Rect);
		for (int i = 0; i < marks.Count; i++)
		{
			CurveMark curveMark = marks[i];
			if (curveMark.X >= x && curveMark.X <= num)
			{
				GUI.color = curveMark.Color;
				((Vector2)(ref val))._002Ector(((Rect)(ref rect)).x + (curveMark.X - x) / (num - x) * ((Rect)(ref rect)).width, num2);
				DrawPoint(val);
				((Rect)(ref rect2))._002Ector(val.x - 5f, val.y - 5f, 10f, 10f);
				if (Mouse.IsOver(rect2))
				{
					TooltipHandler.TipRegion(rect2, new TipSignal(curveMark.Message));
				}
			}
		}
		GUI.color = Color.white;
	}

	private static void DrawPoint(Vector2 screenPoint)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		GUI.DrawTexture(new Rect(screenPoint.x - 5f, screenPoint.y - 5f, 10f, 10f), (Texture)(object)CurvePoint);
	}

	private static void DrawInfiniteVerticalLine(Rect rect, Rect viewRect, float curveX)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		Widgets.DrawLineVertical(CurveToScreenCoordsInsideScreenRect(rect, viewRect, new Vector2(curveX, 0f)).x, -999f, 9999f);
	}

	private static void DrawInfiniteHorizontalLine(Rect rect, Rect viewRect, float curveY)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = CurveToScreenCoordsInsideScreenRect(rect, viewRect, new Vector2(0f, curveY));
		Widgets.DrawLineHorizontal(-999f, val.y, 9999f);
	}

	public static Vector2 CurveToScreenCoordsInsideScreenRect(Rect rect, Rect viewRect, Vector2 curvePoint)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = curvePoint;
		val.x -= ((Rect)(ref viewRect)).x;
		val.y -= ((Rect)(ref viewRect)).y;
		val.x *= ((Rect)(ref rect)).width / ((Rect)(ref viewRect)).width;
		val.y *= ((Rect)(ref rect)).height / ((Rect)(ref viewRect)).height;
		val.y = ((Rect)(ref rect)).height - val.y;
		return val;
	}

	public static Vector2 ScreenToCurveCoords(Rect rect, Rect viewRect, Vector2 screenPoint)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = screenPoint;
		val.y = ((Rect)(ref rect)).height - val.y;
		val.x /= ((Rect)(ref rect)).width / ((Rect)(ref viewRect)).width;
		val.y /= ((Rect)(ref rect)).height / ((Rect)(ref viewRect)).height;
		val.x += ((Rect)(ref viewRect)).x;
		val.y += ((Rect)(ref viewRect)).y;
		return new CurvePoint(val);
	}

	public static void DrawGraphBackgroundLines(Rect rect, Rect viewRect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		Widgets.BeginGroup(rect);
		float num = 0.01f;
		while (((Rect)(ref viewRect)).width / (num * 10f) > 4f)
		{
			num *= 10f;
		}
		for (float num2 = (float)Mathf.FloorToInt(((Rect)(ref viewRect)).x / num) * num; num2 < ((Rect)(ref viewRect)).xMax; num2 += num)
		{
			if (Mathf.Abs(num2 % (10f * num)) < 0.001f)
			{
				GUI.color = MajorLineColor;
			}
			else
			{
				GUI.color = MinorLineColor;
			}
			DrawInfiniteVerticalLine(rect, viewRect, num2);
		}
		float num3 = 0.01f;
		while (((Rect)(ref viewRect)).height / (num3 * 10f) > 4f)
		{
			num3 *= 10f;
		}
		for (float num4 = (float)Mathf.FloorToInt(((Rect)(ref viewRect)).y / num3) * num3; num4 < ((Rect)(ref viewRect)).yMax; num4 += num3)
		{
			if (Mathf.Abs(num4 % (10f * num3)) < 0.001f)
			{
				GUI.color = MajorLineColor;
			}
			else
			{
				GUI.color = MinorLineColor;
			}
			DrawInfiniteHorizontalLine(rect, viewRect, num4);
		}
		GUI.color = AxisLineColor;
		DrawInfiniteHorizontalLine(rect, viewRect, 0f);
		DrawInfiniteVerticalLine(rect, viewRect, 0f);
		GUI.color = Color.white;
		Widgets.EndGroup();
	}

	private static int RemovePointsOptimizationFreq(int count)
	{
		int result = count + 1;
		if (count > 1000)
		{
			result = 5;
		}
		if (count > 1200)
		{
			result = 4;
		}
		if (count > 1400)
		{
			result = 3;
		}
		if (count > 1900)
		{
			result = 2;
		}
		return result;
	}
}
