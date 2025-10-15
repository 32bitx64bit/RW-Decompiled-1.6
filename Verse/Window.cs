using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse.Sound;
using Verse.Steam;

namespace Verse;

public abstract class Window
{
	public WindowLayer layer = WindowLayer.Dialog;

	public string optionalTitle;

	public bool doCloseX;

	public bool doCloseButton;

	public bool closeOnAccept = true;

	public bool closeOnCancel = true;

	public bool forceCatchAcceptAndCancelEventEvenIfUnfocused;

	public bool closeOnClickedOutside;

	public bool forcePause;

	public bool preventCameraMotion = true;

	public bool preventDrawTutor;

	public bool doWindowBackground = true;

	public bool onlyOneOfTypeAllowed = true;

	public bool absorbInputAroundWindow;

	public bool resizeable;

	public bool draggable;

	public bool drawShadow = true;

	public bool focusWhenOpened = true;

	public float shadowAlpha = 1f;

	public SoundDef soundAppear;

	public SoundDef soundClose;

	public SoundDef soundAmbient;

	public bool silenceAmbientSound;

	public bool grayOutIfOtherDialogOpen;

	public Vector2 commonSearchWidgetOffset = new Vector2(0f, CloseButSize.y - QuickSearchSize.y) / 2f;

	public bool openMenuOnCancel;

	public bool preventSave;

	public bool drawInScreenshotMode = true;

	public bool onlyDrawInDevMode;

	public bool ignoreScreenFader;

	public const float StandardMargin = 18f;

	public const float FooterRowHeight = 55f;

	public static readonly Vector2 CloseButSize = new Vector2(120f, 40f);

	public static readonly Vector2 QuickSearchSize = new Vector2(240f, 24f);

	public int ID;

	public Rect windowRect;

	private Sustainer sustainerAmbient;

	private WindowResizer resizer;

	private bool resizeLater;

	private Rect resizeLaterRect;

	private IWindowDrawing windowDrawing;

	private string onGUIProfilerLabelCached;

	public string extraOnGUIProfilerLabelCached;

	private WindowFunction innerWindowOnGUICached;

	private Action notify_CommonSearchChangedCached;

	public virtual Vector2 InitialSize => new Vector2(500f, 500f);

	protected virtual float Margin => 18f;

	public virtual bool IsDebug => false;

	public bool IsOpen => Find.WindowStack.IsOpen(this);

	public virtual QuickSearchWidget CommonSearchWidget => null;

	public virtual string CloseButtonText => "CloseButton".Translate();

	public Window(IWindowDrawing customWindowDrawing = null)
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Expected O, but got Unknown
		soundAppear = SoundDefOf.DialogBoxAppear;
		soundClose = SoundDefOf.Click;
		onGUIProfilerLabelCached = "WindowOnGUI: " + GetType().Name;
		extraOnGUIProfilerLabelCached = "ExtraOnGUI: " + GetType().Name;
		innerWindowOnGUICached = new WindowFunction(InnerWindowOnGUI);
		notify_CommonSearchChangedCached = Notify_CommonSearchChanged;
		windowDrawing = customWindowDrawing ?? new DefaultWindowDrawing();
	}

	public virtual void WindowUpdate()
	{
		if (sustainerAmbient != null)
		{
			sustainerAmbient.Maintain();
		}
	}

	public abstract void DoWindowContents(Rect inRect);

	public virtual void ExtraOnGUI()
	{
	}

	public virtual void PreOpen()
	{
		SetInitialSizeAndPosition();
		CommonSearchWidget?.Reset();
		if (layer == WindowLayer.Dialog)
		{
			if (Current.ProgramState == ProgramState.Playing)
			{
				Find.DesignatorManager.Dragger.EndDrag();
				Find.DesignatorManager.Deselect();
				Find.Selector.Notify_DialogOpened();
			}
			if (Find.World != null)
			{
				Find.WorldSelector.Notify_DialogOpened();
			}
		}
	}

	public virtual void PostOpen()
	{
		if (soundAppear != null)
		{
			soundAppear.PlayOneShotOnCamera();
		}
		if (soundAmbient != null)
		{
			sustainerAmbient = soundAmbient.TrySpawnSustainer(SoundInfo.OnCamera(MaintenanceType.PerFrame));
		}
	}

	public virtual bool OnCloseRequest()
	{
		return true;
	}

	public virtual void PreClose()
	{
	}

	public virtual void PostClose()
	{
	}

	public virtual void WindowOnGUI()
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		if ((!drawInScreenshotMode && Find.UIRoot.screenshotMode.Active) || (onlyDrawInDevMode && !Prefs.DevMode))
		{
			return;
		}
		if (resizeable)
		{
			if (resizer == null)
			{
				resizer = new WindowResizer();
			}
			if (resizeLater)
			{
				resizeLater = false;
				windowRect = resizeLaterRect;
			}
		}
		windowRect = windowRect.Rounded();
		windowRect = GUI.Window(ID, windowRect, innerWindowOnGUICached, "", windowDrawing.EmptyStyle);
	}

	private void InnerWindowOnGUI(int x)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Invalid comparison between Unknown and I4
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Invalid comparison between Unknown and I4
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Invalid comparison between Unknown and I4
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		UnityGUIBugsFixer.OnGUI();
		SteamDeck.WindowOnGUI();
		OriginalEventUtility.RecordOriginalEvent(Event.current);
		Rect val = windowRect.AtZero();
		Find.WindowStack.currentlyDrawnWindow = this;
		if (doWindowBackground)
		{
			windowDrawing.DoWindowBackground(val);
		}
		if (KeyBindingDefOf.Cancel.KeyDownEvent)
		{
			Find.WindowStack.Notify_PressedCancel();
		}
		if (KeyBindingDefOf.Accept.KeyDownEvent)
		{
			Find.WindowStack.Notify_PressedAccept();
		}
		if ((int)Event.current.type == 0)
		{
			Find.WindowStack.Notify_ClickedInsideWindow(this);
		}
		if ((int)Event.current.type == 4 && !Find.WindowStack.GetsInput(this))
		{
			Event.current.Use();
		}
		if (!optionalTitle.NullOrEmpty())
		{
			GUI.Label(new Rect(Margin, Margin, ((Rect)(ref windowRect)).width, 25f), optionalTitle);
		}
		if (doCloseX && windowDrawing.DoClostButtonSmall(val))
		{
			Close();
		}
		if (resizeable && (int)Event.current.type != 7)
		{
			Rect val2 = resizer.DoResizeControl(windowRect);
			if (val2 != windowRect)
			{
				resizeLater = true;
				resizeLaterRect = val2;
			}
		}
		Rect val3 = val.ContractedBy(Margin);
		if (!optionalTitle.NullOrEmpty())
		{
			((Rect)(ref val3)).yMin = ((Rect)(ref val3)).yMin + (Margin + 25f);
		}
		CommonSearchWidget?.OnGUI(QuickSearchWidgetRect(val, val3), notify_CommonSearchChangedCached);
		windowDrawing.BeginGroup(val3);
		try
		{
			DoWindowContents(val3.AtZero());
		}
		catch (Exception ex)
		{
			Log.Error("Exception filling window for " + GetType()?.ToString() + ": " + ex);
		}
		windowDrawing.EndGroup();
		LateWindowOnGUI(val3);
		if (grayOutIfOtherDialogOpen)
		{
			IList<Window> windows = Find.WindowStack.Windows;
			for (int i = 0; i < windows.Count; i++)
			{
				if (windows[i].layer == WindowLayer.Dialog && !(windows[i] is Page) && windows[i] != this)
				{
					windowDrawing.DoGrayOut(val);
					break;
				}
			}
		}
		if (resizeable && (int)Event.current.type == 7)
		{
			resizer.DoResizeControl(windowRect);
		}
		if (doCloseButton)
		{
			Text.Font = GameFont.Small;
			Rect rect = default(Rect);
			((Rect)(ref rect))._002Ector(((Rect)(ref val)).width / 2f - CloseButSize.x / 2f, ((Rect)(ref val)).height - 55f, CloseButSize.x, CloseButSize.y);
			if (windowDrawing.DoCloseButton(rect, CloseButtonText))
			{
				Close();
			}
		}
		if (KeyBindingDefOf.Cancel.KeyDownEvent && IsOpen)
		{
			OnCancelKeyPressed();
		}
		if (draggable)
		{
			GUI.DragWindow();
		}
		else if ((int)Event.current.type == 0)
		{
			Event.current.Use();
		}
		if (!ignoreScreenFader)
		{
			ScreenFader.OverlayOnGUI(((Rect)(ref val)).size);
		}
		Find.WindowStack.currentlyDrawnWindow = null;
		OriginalEventUtility.Reset();
	}

	protected virtual Rect QuickSearchWidgetRect(Rect winRect, Rect inRect)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = commonSearchWidgetOffset;
		return new Rect(((Rect)(ref winRect)).x + val.x, ((Rect)(ref winRect)).height - 55f + val.y, QuickSearchSize.x, QuickSearchSize.y);
	}

	protected virtual void LateWindowOnGUI(Rect inRect)
	{
	}

	public virtual void Close(bool doCloseSound = true)
	{
		Find.WindowStack.TryRemove(this, doCloseSound);
	}

	public virtual bool CausesMessageBackground()
	{
		return false;
	}

	protected virtual void SetInitialSizeAndPosition()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		Vector2 initialSize = InitialSize;
		windowRect = new Rect(((float)UI.screenWidth - initialSize.x) / 2f, ((float)UI.screenHeight - initialSize.y) / 2f, initialSize.x, initialSize.y);
		windowRect = windowRect.Rounded();
	}

	public virtual void OnCancelKeyPressed()
	{
		if (closeOnCancel)
		{
			Close();
			Event.current.Use();
		}
		if (openMenuOnCancel)
		{
			Find.MainTabsRoot.ToggleTab(MainButtonDefOf.Menu);
		}
	}

	public virtual void OnAcceptKeyPressed()
	{
		if (closeOnAccept)
		{
			Close();
			Event.current.Use();
		}
	}

	public virtual void Notify_ResolutionChanged()
	{
		SetInitialSizeAndPosition();
	}

	public virtual void Notify_CommonSearchChanged()
	{
	}

	public virtual void Notify_ClickOutsideWindow()
	{
		CommonSearchWidget?.Unfocus();
	}
}
