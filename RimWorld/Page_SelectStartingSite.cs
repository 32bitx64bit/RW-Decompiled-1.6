using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

public class Page_SelectStartingSite : Page
{
	private const float GapBetweenBottomButtons = 10f;

	private const float UseTwoRowsIfScreenWidthBelowBase = 540f;

	private static readonly List<Vector3> TmpTileVertices = new List<Vector3>();

	private PlanetTile? tutorialStartTilePatch;

	public override string PageTitle => "SelectStartingSite".TranslateWithBackup("SelectLandingSite");

	public override Vector2 InitialSize => Vector2.zero;

	protected override float Margin => 0f;

	public Page_SelectStartingSite()
	{
		absorbInputAroundWindow = false;
		shadowAlpha = 0f;
		preventCameraMotion = false;
	}

	public override void PreOpen()
	{
		base.PreOpen();
		Find.World.renderer.wantedMode = WorldRenderMode.Planet;
		Find.WorldInterface.Reset();
		((MainButtonWorker_ToggleWorld)MainButtonDefOf.World.Worker).resetViewNextTime = true;
	}

	public override void PostOpen()
	{
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		base.PostOpen();
		Find.GameInitData.ChooseRandomStartingTile();
		LessonAutoActivator.TeachOpportunity(ConceptDefOf.WorldCameraMovement, OpportunityType.Important);
		TutorSystem.Notify_Event("PageStart-SelectStartingSite");
		tutorialStartTilePatch = null;
		if (!TutorSystem.TutorialMode || Find.Tutor.activeLesson == null || Find.Tutor.activeLesson.Current == null || Find.Tutor.activeLesson.Current.Instruction != InstructionDefOf.ChooseLandingSite)
		{
			return;
		}
		Find.WorldCameraDriver.ResetAltitude();
		Find.WorldCameraDriver.Update();
		List<PlanetTile> list = new List<PlanetTile>();
		PlanetLayer planetLayer = Find.GameInitData.startingTile.Layer;
		float[] array = new float[planetLayer.TilesCount];
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector((float)Screen.width / 2f, (float)Screen.height / 2f);
		float num = Vector2.Distance(val, Vector2.zero);
		for (int i = 0; i < planetLayer.TilesCount; i++)
		{
			if (TutorSystem.AllowAction(GetActionStringForChoosingTile(planetLayer[i])))
			{
				TmpTileVertices.Clear();
				planetLayer.GetTileVertices(i, TmpTileVertices);
				Vector3 val2 = Vector3.zero;
				for (int j = 0; j < TmpTileVertices.Count; j++)
				{
					val2 += TmpTileVertices[j];
				}
				val2 /= (float)TmpTileVertices.Count;
				Vector3 val3 = Find.WorldCamera.WorldToScreenPoint(val2) / Prefs.UIScale;
				val3.y = (float)UI.screenHeight - val3.y;
				val3.x = Mathf.Clamp(val3.x, 0f, (float)UI.screenWidth);
				val3.y = Mathf.Clamp(val3.y, 0f, (float)UI.screenHeight);
				float num2 = 1f - Vector2.Distance(val, Vector2.op_Implicit(val3)) / num;
				Transform transform = ((Component)Find.WorldCamera).transform;
				Vector3 val4 = val2 - transform.position;
				Vector3 normalized = ((Vector3)(ref val4)).normalized;
				float num3 = Vector3.Dot(transform.forward, normalized);
				array[i] = num2 * num3;
			}
			else
			{
				array[i] = float.NegativeInfinity;
			}
		}
		for (int k = 0; k < 16; k++)
		{
			for (int l = 0; l < array.Length; l++)
			{
				list.Clear();
				planetLayer.GetTileNeighbors(l, list);
				float num4 = array[l];
				if (num4 < 0f)
				{
					continue;
				}
				for (int m = 0; m < list.Count; m++)
				{
					float num5 = array[list[m].tileId];
					if (!(num5 < 0f))
					{
						num4 += num5;
					}
				}
				array[l] = num4 / (float)list.Count;
			}
		}
		float num6 = float.NegativeInfinity;
		PlanetTile value = PlanetTile.Invalid;
		for (int n = 0; n < array.Length; n++)
		{
			if (array[n] > 0f && num6 < array[n])
			{
				num6 = array[n];
				value = new PlanetTile(n, planetLayer);
			}
		}
		if (value.Valid)
		{
			tutorialStartTilePatch = value;
		}
	}

	private static string GetActionStringForChoosingTile(Tile tile)
	{
		StringBuilder stringBuilder = new StringBuilder("ChooseBiome");
		if (tile.Biomes.Count() > 1)
		{
			stringBuilder.Append('-').Append(string.Join('-', tile.Biomes.Select((BiomeDef x) => x.defName)));
		}
		else
		{
			stringBuilder.Append('-').Append(tile.PrimaryBiome.defName);
		}
		stringBuilder.Append('-').Append(tile.hilliness.ToString()).Append('-')
			.Append(tile.Landmark?.def.defName ?? "None");
		return stringBuilder.ToString();
	}

	public override void PostClose()
	{
		base.PostClose();
		Find.World.renderer.wantedMode = WorldRenderMode.None;
	}

	public override void DoWindowContents(Rect rect)
	{
		if (Find.WorldInterface.SelectedTile.Valid)
		{
			Find.GameInitData.startingTile = Find.WorldInterface.SelectedTile;
		}
		else if (Find.WorldSelector.FirstSelectedObject != null)
		{
			Find.GameInitData.startingTile = Find.WorldSelector.FirstSelectedObject.Tile;
		}
	}

	public override void ExtraOnGUI()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		base.ExtraOnGUI();
		Text.Anchor = (TextAnchor)1;
		DrawPageTitle(new Rect(0f, 5f, (float)UI.screenWidth, 300f));
		Text.Anchor = (TextAnchor)0;
		DoCustomBottomButtons();
		if (tutorialStartTilePatch.HasValue)
		{
			TmpTileVertices.Clear();
			Find.WorldGrid.GetTileVertices(tutorialStartTilePatch.Value, TmpTileVertices);
			Vector3 val = Vector3.zero;
			for (int i = 0; i < TmpTileVertices.Count; i++)
			{
				val += TmpTileVertices[i];
			}
			Color color = GUI.color;
			GUI.color = Color.white;
			GenUI.DrawArrowPointingAtWorldspace(val / (float)TmpTileVertices.Count, Find.WorldCamera);
			GUI.color = color;
		}
	}

	protected override bool CanDoNext()
	{
		if (!base.CanDoNext())
		{
			return false;
		}
		PlanetTile selectedTile = Find.WorldInterface.SelectedTile;
		if (!selectedTile.Valid)
		{
			if (Prefs.DevMode && !Find.WorldInterface.selector.AnyObjectOrTileSelected)
			{
				Messages.Message("Tile has been randomly selected (debug mode only)", MessageTypeDefOf.SilentInput, historical: false);
				Find.WorldInterface.SelectedTile = TileFinder.RandomStartingTile();
				return true;
			}
			Messages.Message("MustSelectStartingSite".TranslateWithBackup("MustSelectLandingSite"), MessageTypeDefOf.RejectInput, historical: false);
			return false;
		}
		StringBuilder stringBuilder = new StringBuilder();
		if (!TileFinder.IsValidTileForNewSettlement(selectedTile, stringBuilder))
		{
			Messages.Message(stringBuilder.ToString(), MessageTypeDefOf.RejectInput, historical: false);
			return false;
		}
		if (!TutorSystem.AllowAction(GetActionStringForChoosingTile(selectedTile.Tile)))
		{
			return false;
		}
		return true;
	}

	protected override void DoNext()
	{
		PlanetTile selTile = Find.WorldInterface.SelectedTile;
		SettlementProximityGoodwillUtility.CheckConfirmSettle(selTile, delegate
		{
			Find.GameInitData.startingTile = selTile;
			base.DoNext();
		});
	}

	private void DoCustomBottomButtons()
	{
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		int num = 4;
		int num2 = ((num < 3 || !((float)UI.screenWidth < 540f + (float)num * (Page.BottomButSize.x + 10f))) ? 1 : 2);
		int num3 = Mathf.CeilToInt((float)num / (float)num2);
		float num4 = Page.BottomButSize.x * (float)num3 + 10f * (float)(num3 + 1);
		float num5 = (float)num2 * Page.BottomButSize.y + 10f * (float)(num2 + 1);
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(((float)UI.screenWidth - num4) / 2f, (float)UI.screenHeight - num5 - 4f, num4, num5);
		WorldInspectPane worldInspectPane = Find.WindowStack.WindowOfType<WorldInspectPane>();
		if (worldInspectPane != null && ((Rect)(ref val)).x < InspectPaneUtility.PaneWidthFor(worldInspectPane) + 4f)
		{
			((Rect)(ref val)).x = InspectPaneUtility.PaneWidthFor(worldInspectPane) + 4f;
		}
		Widgets.DrawWindowBackground(val);
		float num6 = ((Rect)(ref val)).xMin + 10f;
		float num7 = ((Rect)(ref val)).yMin + 10f;
		Text.Font = GameFont.Small;
		if ((Widgets.ButtonText(new Rect(num6, num7, Page.BottomButSize.x, Page.BottomButSize.y), "Back".Translate()) || KeyBindingDefOf.Cancel.KeyDownEvent) && CanDoBack())
		{
			DoBack();
		}
		num6 += Page.BottomButSize.x + 10f;
		if (Widgets.ButtonText(new Rect(num6, num7, Page.BottomButSize.x, Page.BottomButSize.y), "SelectRandomSite".Translate()))
		{
			SoundDefOf.Click.PlayOneShotOnCamera();
			if (ModsConfig.OdysseyActive && Rand.Bool)
			{
				Find.WorldInterface.SelectedTile = TileFinder.RandomSettlementTileFor(Find.WorldGrid.Surface, Faction.OfPlayer, mustBeAutoChoosable: true, (PlanetTile x) => x.Tile.Landmark != null);
			}
			else
			{
				Find.WorldInterface.SelectedTile = TileFinder.RandomStartingTile();
			}
			Find.WorldCameraDriver.JumpTo(Find.WorldGrid.GetTileCenter(Find.WorldInterface.SelectedTile));
		}
		num6 += Page.BottomButSize.x + 10f;
		if (num2 == 2)
		{
			num6 = ((Rect)(ref val)).xMin + 10f;
			num7 += Page.BottomButSize.y + 10f;
		}
		if (Widgets.ButtonText(new Rect(num6, num7, Page.BottomButSize.x, Page.BottomButSize.y), "WorldFactionsTab".Translate()))
		{
			Find.WindowStack.Add(new Dialog_FactionDuringLanding());
		}
		num6 += Page.BottomButSize.x + 10f;
		if (Widgets.ButtonText(new Rect(num6, num7, Page.BottomButSize.x, Page.BottomButSize.y), "Next".Translate()) && CanDoNext())
		{
			DoNext();
		}
		num6 += Page.BottomButSize.x + 10f;
		GenUI.AbsorbClicksInRect(val);
	}

	public override void OnAcceptKeyPressed()
	{
		if (CanDoNext())
		{
			DoNext();
		}
	}
}
