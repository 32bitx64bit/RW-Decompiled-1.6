using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace LudeonTK;

public abstract class Dialog_OptionLister : Window_Dev
{
	protected Vector2 scrollPosition;

	protected string filter = "";

	protected float totalOptionsHeight;

	protected bool focusOnFilterOnOpen = true;

	private bool focusFilter;

	protected float curY;

	protected float curX;

	private int boundingRectCachedForFrame = -1;

	private Rect? boundingRectCached;

	private Rect? boundingRect;

	public float verticalSpacing = 2f;

	protected const string FilterControlName = "DebugFilter";

	public override Vector2 InitialSize => new Vector2((float)UI.screenWidth, (float)UI.screenHeight);

	public override bool IsDebug => true;

	public Rect? BoundingRectCached
	{
		get
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			if (boundingRectCachedForFrame != Time.frameCount)
			{
				if (boundingRect.HasValue)
				{
					Rect value = boundingRect.Value;
					Vector2 val = scrollPosition;
					((Rect)(ref value)).x = ((Rect)(ref value)).x + val.x;
					((Rect)(ref value)).y = ((Rect)(ref value)).y + val.y;
					boundingRectCached = value;
				}
				boundingRectCachedForFrame = Time.frameCount;
			}
			return boundingRectCached;
		}
	}

	public Dialog_OptionLister()
	{
		doCloseX = true;
		onlyOneOfTypeAllowed = true;
		absorbInputAroundWindow = true;
	}

	public override void PostOpen()
	{
		base.PostOpen();
		if (focusOnFilterOnOpen)
		{
			focusFilter = true;
		}
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Invalid comparison between Unknown and I4
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Invalid comparison between Unknown and I4
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Invalid comparison between Unknown and I4
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Invalid comparison between Unknown and I4
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		GUI.SetNextControlName("DebugFilter");
		if ((int)Event.current.type != 4 || (!KeyBindingDefOf.Dev_ToggleDebugSettingsMenu.KeyDownEvent && !KeyBindingDefOf.Dev_ToggleDebugActionsMenu.KeyDownEvent))
		{
			filter = DevGUI.TextField(new Rect(0f, 0f, 200f, 30f), filter);
			if (((int)Event.current.type == 4 || (int)Event.current.type == 7) && focusFilter)
			{
				GUI.FocusControl("DebugFilter");
				filter = "";
				focusFilter = false;
			}
			if ((int)Event.current.type == 8)
			{
				totalOptionsHeight = 0f;
			}
			Rect outRect = default(Rect);
			((Rect)(ref outRect))._002Ector(inRect);
			((Rect)(ref outRect)).yMin = ((Rect)(ref outRect)).yMin + 35f;
			int num = (int)(InitialSize.x / 200f);
			float num2 = (totalOptionsHeight + 24f * (float)(num - 1)) / (float)num;
			if (num2 < ((Rect)(ref outRect)).height)
			{
				num2 = ((Rect)(ref outRect)).height;
			}
			curX = 0f;
			curY = 0f;
			Rect viewRect = default(Rect);
			((Rect)(ref viewRect))._002Ector(0f, 0f, ((Rect)(ref outRect)).width - 16f, num2);
			DevGUI.BeginScrollView(outRect, ref scrollPosition, viewRect);
			DevGUI.BeginGroup(inRect);
			float columnWidth = (((Rect)(ref viewRect)).width - 17f * (float)(num - 1)) / (float)num;
			DoListingItems(inRect.AtZero(), columnWidth);
			DevGUI.EndGroup();
			DevGUI.EndScrollView();
		}
	}

	public override void PostClose()
	{
		base.PostClose();
		UI.UnfocusCurrentControl();
	}

	protected abstract void DoListingItems(Rect inRect, float columnWidth);

	protected bool FilterAllows(string label)
	{
		if (filter.NullOrEmpty())
		{
			return true;
		}
		if (label.NullOrEmpty())
		{
			return true;
		}
		return label.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0;
	}
}
