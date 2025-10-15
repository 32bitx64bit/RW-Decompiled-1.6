using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse.Sound;

namespace Verse;

public class FloatMenuGrid : Window
{
	private List<FloatMenuGridOption> options;

	private int calculatedSquareSize;

	private Color baseColor = Color.white;

	public Action onCloseCallback;

	private static readonly Vector2 OptionSize = new Vector2(34f, 34f);

	public override Vector2 InitialSize => new Vector2(TotalWidth, TotalHeight);

	public float TotalWidth => (float)calculatedSquareSize * (OptionSize.x - 1f);

	public float TotalHeight => (float)calculatedSquareSize * (OptionSize.y - 1f);

	protected override float Margin => 0f;

	public FloatMenuGrid(List<FloatMenuGridOption> options)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		this.options = options.OrderByDescending((FloatMenuGridOption op) => op.Priority).ToList();
		layer = WindowLayer.Super;
		closeOnClickedOutside = true;
		doWindowBackground = false;
		drawShadow = false;
		preventCameraMotion = false;
		calculatedSquareSize = Mathf.RoundToInt(Mathf.Sqrt(Mathf.Pow(Mathf.Round(Mathf.Sqrt((float)options.Count)), 2f)));
		SoundDefOf.FloatMenu_Open.PlayOneShotOnCamera();
	}

	protected override void SetInitialSizeAndPosition()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		Vector2 mousePositionOnUIInverted = UI.MousePositionOnUIInverted;
		if (mousePositionOnUIInverted.x + InitialSize.x > (float)UI.screenWidth)
		{
			mousePositionOnUIInverted.x = (float)UI.screenWidth - InitialSize.x;
		}
		if (mousePositionOnUIInverted.y > (float)UI.screenHeight)
		{
			mousePositionOnUIInverted.y = UI.screenHeight;
		}
		windowRect = new Rect(mousePositionOnUIInverted.x, mousePositionOnUIInverted.y - InitialSize.y, InitialSize.x, InitialSize.y);
	}

	public override void DoWindowContents(Rect rect)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		UpdateBaseColor();
		GUI.color = baseColor;
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < options.Count; i++)
		{
			FloatMenuGridOption floatMenuGridOption = options[i];
			float num3 = (float)num * OptionSize.x;
			float num4 = (float)num2 * OptionSize.y;
			if (num3 > 0f)
			{
				num3 -= (float)num;
			}
			if (num4 > 0f)
			{
				num4 -= (float)num2;
			}
			if (floatMenuGridOption.OnGUI(new Rect(num3, num4, OptionSize.x, OptionSize.y)))
			{
				Find.WindowStack.TryRemove(this);
				break;
			}
			num++;
			if (num >= calculatedSquareSize)
			{
				num = 0;
				num2++;
			}
		}
		GUI.color = Color.white;
	}

	private void UpdateBaseColor()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		baseColor = Color.white;
		Rect r = GenUI.ExpandedBy(new Rect(0f, 0f, TotalWidth, TotalHeight), 5f);
		if (!((Rect)(ref r)).Contains(Event.current.mousePosition))
		{
			float num = GenUI.DistFromRect(r, Event.current.mousePosition);
			baseColor = new Color(1f, 1f, 1f, 1f - num / 95f);
			if (num > 95f)
			{
				Close(doCloseSound: false);
				SoundDefOf.FloatMenu_Cancel.PlayOneShotOnCamera();
				Find.WindowStack.TryRemove(this);
			}
		}
	}

	public override void PostClose()
	{
		base.PostClose();
		onCloseCallback?.Invoke();
	}
}
