using UnityEngine;

namespace Verse;

public static class DragSliderManager
{
	private static bool dragging = false;

	private static float rootX;

	private static float lastRateFactor = 1f;

	private static DragSliderCallback draggingUpdateMethod;

	private static DragSliderCallback completedMethod;

	public static void ForceStop()
	{
		dragging = false;
	}

	public static bool DragSlider(Rect rect, float rateFactor, DragSliderCallback newStartMethod, DragSliderCallback newDraggingUpdateMethod, DragSliderCallback newCompletedMethod)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if ((int)Event.current.type == 0 && Event.current.button == 0 && Mouse.IsOver(rect))
		{
			lastRateFactor = rateFactor;
			newStartMethod(0f, rateFactor);
			StartDragSliding(newDraggingUpdateMethod, newCompletedMethod);
			return true;
		}
		return false;
	}

	private static void StartDragSliding(DragSliderCallback newDraggingUpdateMethod, DragSliderCallback newCompletedMethod)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		dragging = true;
		draggingUpdateMethod = newDraggingUpdateMethod;
		completedMethod = newCompletedMethod;
		rootX = UI.MousePositionOnUI.x;
	}

	private static float CurMouseOffset()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return UI.MousePositionOnUI.x - rootX;
	}

	public static void DragSlidersOnGUI()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Invalid comparison between Unknown and I4
		if (dragging && (int)Event.current.type == 1 && Event.current.button == 0)
		{
			dragging = false;
			if (completedMethod != null)
			{
				completedMethod(CurMouseOffset(), lastRateFactor);
			}
		}
	}

	public static void DragSlidersUpdate()
	{
		if (dragging && draggingUpdateMethod != null)
		{
			draggingUpdateMethod(CurMouseOffset(), lastRateFactor);
		}
	}
}
