using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace RimWorld;

public abstract class MainTabWindow : Window
{
	public MainButtonDef def;

	public virtual Vector2 RequestedTabSize => new Vector2(1010f, 684f);

	public virtual MainTabWindowAnchor Anchor => MainTabWindowAnchor.Left;

	public override Vector2 InitialSize
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			Vector2 requestedTabSize = RequestedTabSize;
			if (requestedTabSize.y > (float)(UI.screenHeight - 35))
			{
				requestedTabSize.y = UI.screenHeight - 35;
			}
			if (requestedTabSize.x > (float)UI.screenWidth)
			{
				requestedTabSize.x = UI.screenWidth;
			}
			return requestedTabSize;
		}
	}

	public MainTabWindow()
	{
		layer = WindowLayer.GameUI;
		soundAppear = null;
		soundClose = SoundDefOf.TabClose;
		doCloseButton = false;
		doCloseX = false;
		preventCameraMotion = false;
	}

	protected override void SetInitialSizeAndPosition()
	{
		base.SetInitialSizeAndPosition();
		if (Anchor == MainTabWindowAnchor.Left)
		{
			((Rect)(ref windowRect)).x = 0f;
		}
		else
		{
			((Rect)(ref windowRect)).x = (float)UI.screenWidth - ((Rect)(ref windowRect)).width;
		}
		((Rect)(ref windowRect)).y = (float)(UI.screenHeight - 35) - ((Rect)(ref windowRect)).height;
	}

	public override void PostOpen()
	{
		base.PostOpen();
		if (def.closesWorldView)
		{
			Find.World.renderer.wantedMode = WorldRenderMode.None;
		}
	}
}
