using System;
using System.Collections.Generic;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

[StaticConstructorOnStartup]
public class WorldTargeter
{
	private Func<GlobalTargetInfo, bool> action;

	private bool canTargetTiles;

	private bool showCancelButton;

	private Texture2D mouseAttachment;

	public bool closeWorldTabWhenFinished;

	private PlanetTile originForClosest;

	private Action onUpdate;

	private Func<GlobalTargetInfo, TaggedString> extraLabelGetter;

	private Func<GlobalTargetInfo, bool> canSelectTarget;

	private PlanetTile closestLayerTile = PlanetTile.Invalid;

	private const float BaseFeedbackTexSize = 0.8f;

	private static readonly Vector2 ButtonSize = new Vector2(150f, 38f);

	private const int BottomPanelYOffset = -50;

	private const int Padding = 8;

	private PlanetLayer cachedLayer;

	private PlanetTile cachedOrigin;

	private PlanetTile cachedClosest;

	public bool IsTargeting => action != null;

	public PlanetTile ClosestLayerTile => closestLayerTile;

	public void BeginTargeting(Func<GlobalTargetInfo, bool> action, bool canTargetTiles, Texture2D mouseAttachment = null, bool closeWorldTabWhenFinished = false, Action onUpdate = null, Func<GlobalTargetInfo, TaggedString> extraLabelGetter = null, Func<GlobalTargetInfo, bool> canSelectTarget = null, PlanetTile? originForClosest = null, bool showCancelButton = false)
	{
		this.action = action;
		this.canTargetTiles = canTargetTiles;
		this.mouseAttachment = mouseAttachment;
		this.closeWorldTabWhenFinished = closeWorldTabWhenFinished;
		this.onUpdate = onUpdate;
		this.extraLabelGetter = extraLabelGetter;
		this.canSelectTarget = canSelectTarget;
		this.originForClosest = originForClosest ?? PlanetTile.Invalid;
		this.showCancelButton = showCancelButton;
	}

	public void StopTargeting()
	{
		if (closeWorldTabWhenFinished)
		{
			CameraJumper.TryHideWorld();
		}
		action = null;
		canTargetTiles = false;
		mouseAttachment = null;
		closeWorldTabWhenFinished = false;
		onUpdate = null;
		extraLabelGetter = null;
		originForClosest = PlanetTile.Invalid;
		closestLayerTile = PlanetTile.Invalid;
		showCancelButton = false;
		cachedLayer = null;
		cachedOrigin = PlanetTile.Invalid;
		cachedClosest = PlanetTile.Invalid;
	}

	public void ProcessInputEvents()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		if ((int)Event.current.type == 0)
		{
			if (Event.current.button == 0 && IsTargeting)
			{
				GlobalTargetInfo arg = CurrentTargetUnderMouse();
				if ((canSelectTarget == null || canSelectTarget(arg)) && action(arg))
				{
					SoundDefOf.Tick_High.PlayOneShotOnCamera();
					StopTargeting();
				}
				Event.current.Use();
			}
			if (Event.current.button == 1 && IsTargeting)
			{
				SoundDefOf.CancelMode.PlayOneShotOnCamera();
				StopTargeting();
				Event.current.Use();
			}
		}
		if (KeyBindingDefOf.Cancel.KeyDownEvent && IsTargeting)
		{
			SoundDefOf.CancelMode.PlayOneShotOnCamera();
			StopTargeting();
			Event.current.Use();
		}
	}

	public void TargeterOnGUI()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		if (!IsTargeting || Mouse.IsInputBlockedNow)
		{
			return;
		}
		Vector2 mousePosition = Event.current.mousePosition;
		Texture2D val = (Object.op_Implicit((Object)(object)mouseAttachment) ? mouseAttachment : TexCommand.Attack);
		Rect val2 = default(Rect);
		((Rect)(ref val2))._002Ector(mousePosition.x + 8f, mousePosition.y + 8f, 32f, 32f);
		GUI.DrawTexture(val2, (Texture)(object)val);
		if (originForClosest.Valid && originForClosest.Layer != PlanetLayer.Selected)
		{
			if (cachedLayer != PlanetLayer.Selected || cachedOrigin != originForClosest)
			{
				cachedLayer = PlanetLayer.Selected;
				cachedOrigin = originForClosest;
				closestLayerTile = (cachedClosest = PlanetLayer.Selected.GetClosestTile_NewTemp(originForClosest));
			}
			else
			{
				closestLayerTile = cachedClosest;
			}
		}
		else
		{
			closestLayerTile = PlanetTile.Invalid;
		}
		if (extraLabelGetter == null)
		{
			return;
		}
		GUI.color = Color.white;
		TaggedString taggedString = extraLabelGetter(CurrentTargetUnderMouse());
		if (!taggedString.NullOrEmpty())
		{
			Color color = GUI.color;
			GUI.color = Color.white;
			Rect rect = default(Rect);
			((Rect)(ref rect))._002Ector(((Rect)(ref val2)).xMax, ((Rect)(ref val2)).y, 9999f, 100f);
			Vector2 val3 = Text.CalcSize(taggedString);
			GUI.DrawTexture(new Rect(((Rect)(ref rect)).x - val3.x * 0.1f, ((Rect)(ref rect)).y, val3.x * 1.2f, val3.y), (Texture)(object)TexUI.GrayTextBG);
			GUI.color = color;
			Widgets.Label(rect, taggedString);
		}
		GUI.color = Color.white;
		if (showCancelButton)
		{
			Rect rect2 = new Rect((float)UI.screenWidth / 2f - ButtonSize.x / 2f - 8f, (float)UI.screenHeight - (ButtonSize.y + 8f) + -50f, ButtonSize.x + 16f, ButtonSize.y + 16f);
			Rect rect3 = GenUI.ContractedBy(rect2, 8f);
			Widgets.DrawWindowBackground(rect2);
			if (Widgets.ButtonText(rect3, "Cancel".Translate()))
			{
				SoundDefOf.Click.PlayOneShotOnCamera();
				StopTargeting();
			}
		}
	}

	public void TargeterUpdate()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		if (IsTargeting)
		{
			Vector3 pos = Vector3.zero;
			GlobalTargetInfo arg = CurrentTargetUnderMouse();
			if (arg.HasWorldObject)
			{
				pos = arg.WorldObject.DrawPos;
			}
			else if (arg.Tile.Valid)
			{
				pos = Find.WorldGrid.GetTileCenter(arg.Tile);
			}
			if (arg.IsValid && !Mouse.IsInputBlockedNow && (canSelectTarget == null || canSelectTarget(arg)))
			{
				WorldRendererUtility.DrawQuadTangentialToPlanet(pos, 0.8f * Find.WorldGrid.AverageTileSize, 0.05f, WorldMaterials.CurTargetingMat);
			}
			if (onUpdate != null)
			{
				onUpdate();
			}
		}
	}

	public bool IsTargetedNow(WorldObject o, List<WorldObject> worldObjectsUnderMouse = null)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if (!IsTargeting)
		{
			return false;
		}
		if (worldObjectsUnderMouse == null)
		{
			worldObjectsUnderMouse = GenWorldUI.WorldObjectsUnderMouse(UI.MousePositionOnUI);
		}
		if (worldObjectsUnderMouse.Any())
		{
			return o == worldObjectsUnderMouse[0];
		}
		return false;
	}

	private GlobalTargetInfo CurrentTargetUnderMouse()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (!IsTargeting)
		{
			return GlobalTargetInfo.Invalid;
		}
		List<WorldObject> list = GenWorldUI.WorldObjectsUnderMouse(UI.MousePositionOnUI);
		if (list.Any())
		{
			return list[0];
		}
		if (canTargetTiles)
		{
			PlanetTile tile = GenWorld.MouseTile();
			if (tile.Valid)
			{
				return new GlobalTargetInfo(tile);
			}
			return GlobalTargetInfo.Invalid;
		}
		return GlobalTargetInfo.Invalid;
	}
}
