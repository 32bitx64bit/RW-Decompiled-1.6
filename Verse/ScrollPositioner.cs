using UnityEngine;

namespace Verse;

public class ScrollPositioner
{
	private Rect? interestRect;

	private bool armed;

	public void Arm(bool armed = true)
	{
		this.armed = armed;
	}

	public void ClearInterestRects()
	{
		interestRect = null;
	}

	public void RegisterInterestRect(Rect rect)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (interestRect.HasValue)
		{
			interestRect = rect.Union(interestRect.Value);
		}
		else
		{
			interestRect = rect;
		}
	}

	public void ScrollHorizontally(ref Vector2 scrollPos, Vector2 outRectSize)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		Scroll(ref scrollPos, outRectSize, scrollHorizontally: true, scrollVertically: false);
	}

	public void ScrollVertically(ref Vector2 scrollPos, Vector2 outRectSize)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		Scroll(ref scrollPos, outRectSize, scrollHorizontally: false);
	}

	public void Scroll(ref Vector2 scrollPos, Vector2 outRectSize, bool scrollHorizontally = true, bool scrollVertically = true)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Invalid comparison between Unknown and I4
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		if ((int)Event.current.type != 8 || !armed)
		{
			return;
		}
		armed = false;
		if (interestRect.HasValue)
		{
			Rect value;
			if (scrollHorizontally)
			{
				ref float x = ref scrollPos.x;
				float x2 = outRectSize.x;
				value = interestRect.Value;
				float xMin = ((Rect)(ref value)).xMin;
				value = interestRect.Value;
				ScrollInDimension(ref x, x2, xMin, ((Rect)(ref value)).xMax);
			}
			if (scrollVertically)
			{
				ref float y = ref scrollPos.y;
				float y2 = outRectSize.y;
				value = interestRect.Value;
				float yMin = ((Rect)(ref value)).yMin;
				value = interestRect.Value;
				ScrollInDimension(ref y, y2, yMin, ((Rect)(ref value)).yMax);
			}
		}
	}

	private void ScrollInDimension(ref float scrollPos, float scrollViewSize, float v0, float v1)
	{
		float num = v1 - v0;
		if (num <= scrollViewSize)
		{
			scrollPos = v0 + num / 2f - scrollViewSize / 2f;
		}
		else
		{
			scrollPos = v0;
		}
	}
}
