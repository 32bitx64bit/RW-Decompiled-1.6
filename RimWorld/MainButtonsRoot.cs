using System.Collections.Generic;
using System.Linq;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace RimWorld;

public class MainButtonsRoot
{
	public MainTabsRoot tabs = new MainTabsRoot();

	private List<MainButtonDef> allButtonsInOrder;

	private int VisibleButtonsCount
	{
		get
		{
			int num = 0;
			for (int i = 0; i < allButtonsInOrder.Count; i++)
			{
				if (allButtonsInOrder[i].buttonVisible)
				{
					num++;
				}
			}
			return num;
		}
	}

	public MainButtonsRoot()
	{
		allButtonsInOrder = DefDatabase<MainButtonDef>.AllDefs.OrderBy((MainButtonDef x) => x.order).ToList();
	}

	public void MainButtonsOnGUI()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Invalid comparison between Unknown and I4
		if ((int)Event.current.type == 8)
		{
			return;
		}
		DoButtons();
		for (int i = 0; i < allButtonsInOrder.Count; i++)
		{
			if (!allButtonsInOrder[i].Worker.Disabled && allButtonsInOrder[i].hotKey != null && allButtonsInOrder[i].hotKey.KeyDownEvent)
			{
				Event.current.Use();
				allButtonsInOrder[i].Worker.InterfaceTryActivate();
				break;
			}
		}
	}

	public void HandleLowPriorityShortcuts()
	{
		tabs.HandleLowPriorityShortcuts();
		if (WorldRendererUtility.WorldSelected && Current.ProgramState == ProgramState.Playing && Find.CurrentMap != null && KeyBindingDefOf.Cancel.KeyDownEvent)
		{
			Event.current.Use();
			Find.World.renderer.wantedMode = WorldRenderMode.None;
		}
	}

	private void DoButtons()
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		for (int i = 0; i < allButtonsInOrder.Count; i++)
		{
			if (allButtonsInOrder[i].Worker.Visible)
			{
				num += (allButtonsInOrder[i].minimized ? 0.5f : 1f);
			}
		}
		GUI.color = Color.white;
		int num2 = (int)((float)UI.screenWidth / num);
		int num3 = num2 / 2;
		int num4 = allButtonsInOrder.FindLastIndex((MainButtonDef x) => x.Worker.Visible);
		int num5 = 0;
		Rect rect = default(Rect);
		for (int j = 0; j < allButtonsInOrder.Count; j++)
		{
			if (allButtonsInOrder[j].Worker.Visible)
			{
				int num6 = (allButtonsInOrder[j].minimized ? num3 : num2);
				if (j == num4)
				{
					num6 = UI.screenWidth - num5;
				}
				((Rect)(ref rect))._002Ector((float)num5, (float)(UI.screenHeight - 35), (float)num6, 36f);
				allButtonsInOrder[j].Worker.DoButton(rect);
				num5 += num6;
			}
		}
	}
}
