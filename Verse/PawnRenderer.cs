using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;

namespace Verse;

public class PawnRenderer
{
	private struct PreRenderResults
	{
		public bool valid;

		public bool draw;

		public bool useCached;

		public bool showBody;

		public float bodyAngle;

		public PawnPosture posture;

		public Vector3 bodyPos;

		public PawnDrawParms parms;
	}

	private readonly Pawn pawn;

	public PawnDownedWiggler wiggler;

	private PawnHeadOverlays statusOverlays;

	private PawnStatusEffecters effecters;

	private PawnWoundDrawer woundOverlays;

	private PawnFirefoamDrawer firefoamOverlays;

	private PawnShamblerScarDrawer shamblerScarDrawer;

	private PawnScariaSoreDrawer scariaSoreDrawer;

	private Graphic_Shadow shadowGraphic;

	public DamageFlasher flasher;

	public PawnRenderTree renderTree;

	private Graphic silhouetteGraphic;

	private Vector3 silhouettePos;

	private ThingDef statueStuff;

	private Color? statuePaintColor;

	private const float CarriedPawnDrawAngle = 70f;

	private const float CachedPawnTextureMinCameraZoom = 18f;

	private PreRenderResults results;

	private readonly Dictionary<Apparel, (Color, bool)> tmpOriginalColors = new Dictionary<Apparel, (Color, bool)>();

	public Graphic BodyGraphic => renderTree.BodyGraphic;

	public Graphic HeadGraphic => renderTree.HeadGraphic;

	public RotDrawMode CurRotDrawMode
	{
		get
		{
			if (pawn.IsMutant && pawn.mutant.Def.useCorpseGraphics)
			{
				return RottableUtility.GetRotDrawMode(pawn.mutant.rotStage);
			}
			if (pawn.Dead && pawn.Corpse != null)
			{
				return pawn.Corpse.CurRotDrawMode;
			}
			return RotDrawMode.Fresh;
		}
	}

	public PawnWoundDrawer WoundOverlays => woundOverlays;

	public PawnFirefoamDrawer FirefoamOverlays => firefoamOverlays;

	public PawnShamblerScarDrawer ShamblerScarDrawer => shamblerScarDrawer;

	public PawnScariaSoreDrawer ScariaSoreDrawer => scariaSoreDrawer;

	public PawnHeadOverlays StatusOverlays => statusOverlays;

	public AnimationDef CurAnimation => renderTree.currentAnimation;

	public bool HasAnimation => renderTree.currentAnimation != null;

	public Graphic SilhouetteGraphic => silhouetteGraphic;

	public Vector3 SilhouettePos => silhouettePos;

	public Material FlightShadowMaterial => MaterialPool.MatFrom("Things/Skyfaller/SkyfallerShadowCircle", ShaderDatabase.Transparent, new Color(1f, 1f, 1f, 0.5f));

	public Color? StatueColor
	{
		get
		{
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			if (statueStuff != null)
			{
				return statuePaintColor ?? statueStuff?.stuffProps?.color;
			}
			return null;
		}
	}

	private PawnRenderFlags DefaultRenderFlagsNow
	{
		get
		{
			PawnNeedsHediffMaterial(out var renderFlags);
			if (!pawn.health.hediffSet.HasHead)
			{
				renderFlags |= PawnRenderFlags.HeadStump;
			}
			if (statueStuff != null)
			{
				renderFlags |= PawnRenderFlags.Statue;
			}
			return renderFlags;
		}
	}

	public PawnRenderer(Pawn pawn)
	{
		this.pawn = pawn;
		wiggler = new PawnDownedWiggler(pawn);
		statusOverlays = new PawnHeadOverlays(pawn);
		woundOverlays = new PawnWoundDrawer(pawn);
		firefoamOverlays = new PawnFirefoamDrawer(pawn);
		shamblerScarDrawer = new PawnShamblerScarDrawer(pawn);
		scariaSoreDrawer = new PawnScariaSoreDrawer(pawn);
		effecters = new PawnStatusEffecters(pawn);
		renderTree = new PawnRenderTree(pawn);
		flasher = new DamageFlasher(pawn);
		if (pawn.RaceProps.startingAnimation != null)
		{
			SetAnimation(pawn.RaceProps.startingAnimation);
		}
	}

	public void SetStatue(ThingDef stuff)
	{
		statueStuff = stuff;
	}

	public void SetStatuePaintColor(Color? color)
	{
		statuePaintColor = color;
	}

	private Mesh GetBlitMeshUpdatedFrame(PawnTextureAtlasFrameSet frameSet, Rot4 rotation, PawnDrawMode drawMode)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		int index = frameSet.GetIndex(rotation, drawMode);
		if (frameSet.isDirty[index])
		{
			Find.PawnCacheCamera.rect = frameSet.uvRects[index];
			Find.PawnCacheRenderer.RenderPawn(pawn, frameSet.atlas, Vector3.zero, 1f, 0f, rotation);
			Find.PawnCacheCamera.rect = new Rect(0f, 0f, 1f, 1f);
			frameSet.isDirty[index] = false;
		}
		return frameSet.meshes[index];
	}

	private void DrawShadowInternal(Vector3 drawLoc)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		if (pawn.Flying)
		{
			Vector3 drawPos = pawn.DrawPos;
			drawPos.y = AltitudeLayer.Filth.AltitudeFor();
			drawPos -= new Vector3(0f, 0f, pawn.flight.PositionOffsetFactor);
			Matrix4x4 val = Matrix4x4.TRS(drawPos, Quaternion.identity, Vector3.one * 0.75f);
			Graphics.DrawMesh(MeshPool.plane10, val, FlightShadowMaterial, 0);
		}
		else
		{
			if (pawn.Swimming || pawn.DrawNonHumanlikeSwimmingGraphic)
			{
				return;
			}
			if (pawn.def.race.specialShadowData != null)
			{
				if (shadowGraphic == null)
				{
					shadowGraphic = new Graphic_Shadow(pawn.def.race.specialShadowData);
				}
				shadowGraphic.Draw(drawLoc, Rot4.North, pawn);
			}
			BodyGraphic?.ShadowGraphic?.Draw(drawLoc, Rot4.North, pawn);
		}
	}

	public void DynamicDrawPhaseAt(DrawPhase phase, Vector3 drawLoc, Rot4? rotOverride = null, bool neverAimWeapon = false)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		switch (phase)
		{
		case DrawPhase.EnsureInitialized:
			EnsureGraphicsInitialized();
			break;
		case DrawPhase.ParallelPreDraw:
			ParallelPreRenderPawnAt(drawLoc, rotOverride, neverAimWeapon);
			break;
		case DrawPhase.Draw:
			RenderPawnAt(drawLoc, rotOverride, neverAimWeapon);
			break;
		}
	}

	public void RenderPawnAt(Vector3 drawLoc, Rot4? rotOverride = null, bool neverAimWeapon = false)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		if (!results.valid)
		{
			EnsureGraphicsInitialized();
			ParallelPreRenderPawnAt(drawLoc, rotOverride, neverAimWeapon);
		}
		if (!results.draw)
		{
			return;
		}
		if (results.useCached)
		{
			if (GlobalTextureAtlasManager.TryGetPawnFrameSet(pawn, out var frameSet, out var _))
			{
				using (new ProfilerBlock("Draw Cached Mesh"))
				{
					Rot4 facing = results.parms.facing;
					Vector3 bodyPos = results.bodyPos;
					float bodyAngle = results.bodyAngle;
					PawnDrawMode drawMode = ((!results.showBody) ? PawnDrawMode.HeadOnly : PawnDrawMode.BodyAndHead);
					Material mat = OverrideMaterialIfNeeded(MaterialPool.MatFrom(new MaterialRequest((Texture)(object)frameSet.atlas, ShaderDatabase.Cutout)), PawnRenderFlags.None);
					GenDraw.DrawMeshNowOrLater(GetBlitMeshUpdatedFrame(frameSet, facing, drawMode), bodyPos, Quaternion.AngleAxis(bodyAngle, Vector3.up), mat, drawNow: false);
					Vector3 drawPos = bodyPos.WithYOffset(PawnRenderUtility.AltitudeForLayer((facing == Rot4.North) ? (-10f) : 90f));
					PawnRenderUtility.DrawEquipmentAndApparelExtras(pawn, drawPos, facing, results.parms.flags);
				}
			}
			else
			{
				Log.ErrorOnce($"Attempted to use a cached frame set for pawn {pawn.Name} but failed to get one.", Gen.HashCombine(pawn.GetHashCode(), 5938111));
			}
		}
		else
		{
			using (new ProfilerBlock("Render Pawn Internal"))
			{
				RenderPawnInternal(results.parms);
			}
		}
		if (results.posture == PawnPosture.Standing && !results.parms.flags.FlagSet(PawnRenderFlags.Invisible))
		{
			using (new ProfilerBlock("Draw Shadow Internal"))
			{
				DrawShadowInternal(drawLoc);
			}
		}
		if (pawn.Spawned && !pawn.Dead)
		{
			pawn.stances.StanceTrackerDraw();
			pawn.pather.PatherDraw();
			pawn.roping.RopingDraw();
		}
		Graphic graphic = (pawn.RaceProps.Humanlike ? pawn.ageTracker.CurLifeStage.silhouetteGraphicData.Graphic : ((pawn.ageTracker.CurKindLifeStage.silhouetteGraphicData == null) ? BodyGraphic : pawn.ageTracker.CurKindLifeStage.silhouetteGraphicData.Graphic));
		SetSilhouetteData(graphic, results.bodyPos);
		DrawDebug();
		results = default(PreRenderResults);
	}

	private void SetSilhouetteData(Graphic graphic, Vector3 pos)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		silhouetteGraphic = graphic;
		silhouettePos = pos;
	}

	public void RenderShadowOnlyAt(Vector3 drawLoc)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (pawn.GetPosture() == PawnPosture.Standing && !DefaultRenderFlagsNow.FlagSet(PawnRenderFlags.Invisible))
		{
			DrawShadowInternal(drawLoc);
		}
	}

	public void EnsureGraphicsInitialized()
	{
		renderTree.EnsureInitialized(DefaultRenderFlagsNow);
	}

	private void ParallelPreRenderPawnAt(Vector3 drawLoc, Rot4? rotOverride = null, bool neverAimWeapon = false)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		results = ParallelGetPreRenderResults(drawLoc, rotOverride, neverAimWeapon);
	}

	private PreRenderResults ParallelGetPreRenderResults(Vector3 drawLoc, Rot4? rotOverride = null, bool neverAimWeapon = false, bool disableCache = false)
	{
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		PreRenderResults preRenderResults = default(PreRenderResults);
		preRenderResults.valid = true;
		PreRenderResults result = preRenderResults;
		if (pawn.IsHiddenFromPlayer())
		{
			return result;
		}
		result.draw = true;
		if (!renderTree.Resolved)
		{
			renderTree.SetDirty();
		}
		PawnRenderFlags pawnRenderFlags = DefaultRenderFlagsNow | PawnRenderFlags.Clothes | PawnRenderFlags.Headgear;
		if (neverAimWeapon)
		{
			pawnRenderFlags |= PawnRenderFlags.NeverAimWeapon;
		}
		bool crawling = pawn.Crawling;
		bool swimming = pawn.Swimming;
		if (swimming)
		{
			pawnRenderFlags &= ~(PawnRenderFlags.Headgear | PawnRenderFlags.Clothes | PawnRenderFlags.NeverAimWeapon);
			pawnRenderFlags |= PawnRenderFlags.NoBody;
		}
		RotDrawMode curRotDrawMode = CurRotDrawMode;
		PawnPosture posture = pawn.GetPosture();
		bool showBody;
		Vector3 bodyPos = GetBodyPos(drawLoc, posture, out showBody);
		float num = ((posture == PawnPosture.Standing) ? 0f : BodyAngle(pawnRenderFlags));
		Rot4 bodyFacing = rotOverride ?? ((posture == PawnPosture.Standing || crawling) ? pawn.Rotation : LayingFacing());
		result.posture = posture;
		result.showBody = showBody;
		result.bodyPos = bodyPos;
		result.bodyAngle = num;
		result.useCached = !disableCache && pawn.RaceProps.Humanlike && Find.CameraDriver.ZoomRootSize > 18f && !pawnRenderFlags.FlagSet(PawnRenderFlags.Portrait) && !pawnRenderFlags.FlagSet(PawnRenderFlags.Statue) && curRotDrawMode != RotDrawMode.Dessicated && !crawling && !swimming && renderTree.currentAnimation == null && pawn.carryTracker?.CarriedThing == null && pawn.CarriedBy == null && !PawnNeedsHediffMaterial(out var _);
		result.parms = GetDrawParms(bodyPos, num, bodyFacing, curRotDrawMode, pawnRenderFlags);
		if (result.useCached)
		{
			return result;
		}
		renderTree.ParallelPreDraw(result.parms);
		return result;
	}

	public void RenderCache(Rot4 rotation, float angle, Vector3 positionOffset, bool renderHead, bool portrait, bool renderHeadgear, bool renderClothes, IReadOnlyDictionary<Apparel, Color> overrideApparelColor = null, Color? overrideHairColor = null, bool stylingStation = false)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		Vector3 zero = Vector3.zero;
		PawnRenderFlags pawnRenderFlags = DefaultRenderFlagsNow | PawnRenderFlags.Cache | PawnRenderFlags.DrawNow;
		if (portrait)
		{
			pawnRenderFlags |= PawnRenderFlags.Portrait;
		}
		if (!renderHead)
		{
			pawnRenderFlags |= PawnRenderFlags.HeadStump;
		}
		if (renderHeadgear)
		{
			pawnRenderFlags |= PawnRenderFlags.Headgear;
		}
		if (renderClothes)
		{
			pawnRenderFlags |= PawnRenderFlags.Clothes;
		}
		if (stylingStation)
		{
			pawnRenderFlags |= PawnRenderFlags.StylingStation;
		}
		tmpOriginalColors.Clear();
		try
		{
			if (overrideApparelColor != null)
			{
				foreach (KeyValuePair<Apparel, Color> item in overrideApparelColor)
				{
					item.Deconstruct(out var key, out var value);
					Apparel apparel = key;
					Color newColor = value;
					CompColorable compColorable = apparel.TryGetComp<CompColorable>();
					if (compColorable != null)
					{
						tmpOriginalColors.Add(apparel, (compColorable.Color, compColorable.Active));
						apparel.SetColor(newColor);
					}
				}
			}
			Color hairColor = Color.white;
			if (pawn.story != null)
			{
				hairColor = pawn.story.HairColor;
				if (overrideHairColor.HasValue)
				{
					pawn.story.HairColor = overrideHairColor.Value;
					pawn.Drawer.renderer.renderTree.SetDirty();
				}
			}
			PawnDrawParms drawParms = GetDrawParms(zero + positionOffset, angle, rotation, CurRotDrawMode, pawnRenderFlags);
			renderTree.EnsureInitialized(DefaultRenderFlagsNow);
			renderTree.ParallelPreDraw(drawParms);
			RenderPawnInternal(drawParms);
			foreach (KeyValuePair<Apparel, (Color, bool)> tmpOriginalColor in tmpOriginalColors)
			{
				if (!tmpOriginalColor.Value.Item2)
				{
					tmpOriginalColor.Key.TryGetComp<CompColorable>().Disable();
				}
				else
				{
					tmpOriginalColor.Key.SetColor(tmpOriginalColor.Value.Item1);
				}
			}
			if (pawn.story != null && overrideHairColor.HasValue)
			{
				pawn.story.HairColor = hairColor;
				pawn.Drawer.renderer.renderTree.SetDirty();
			}
		}
		catch (Exception ex)
		{
			Log.Error("Error rendering pawn portrait: " + ex);
		}
		finally
		{
			tmpOriginalColors.Clear();
		}
	}

	private Vector3 GetBodyPos(Vector3 drawLoc, PawnPosture posture, out bool showBody)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		if (posture == PawnPosture.Standing)
		{
			showBody = true;
			return drawLoc;
		}
		Building_Bed building_Bed = pawn.CurrentBed();
		Vector3 result;
		if (building_Bed != null && pawn.RaceProps.Humanlike)
		{
			showBody = building_Bed.def.building.bed_showSleeperBody;
			AltitudeLayer altLayer = (AltitudeLayer)Mathf.Max((int)building_Bed.def.altitudeLayer, 20);
			Vector3 val = pawn.Position.ToVector3ShiftedWithAltitude(altLayer);
			Rot4 rotation = building_Bed.Rotation;
			rotation.AsInt += 2;
			float num = BaseHeadOffsetAt(Rot4.South).z + pawn.story.bodyType.bedOffset + building_Bed.def.building.bed_pawnDrawOffset;
			Vector3 val2 = rotation.FacingCell.ToVector3();
			result = val - val2 * num;
		}
		else
		{
			showBody = true;
			result = drawLoc;
			if (pawn.ParentHolder is IThingHolderWithDrawnPawn thingHolderWithDrawnPawn)
			{
				result.y = thingHolderWithDrawnPawn.HeldPawnDrawPos_Y;
			}
			else if (pawn.ParentHolder.ParentHolder is IThingHolderWithDrawnPawn thingHolderWithDrawnPawn2)
			{
				result.y = thingHolderWithDrawnPawn2.HeldPawnDrawPos_Y;
			}
			else if (!pawn.Dead && pawn.CarriedBy == null && pawn.ParentHolder.Isnt<PawnFlyer>())
			{
				result.y = AltitudeLayer.LayingPawn.AltitudeFor();
			}
		}
		showBody = pawn.mindState?.duty?.def?.drawBodyOverride ?? showBody;
		return result;
	}

	private bool PawnNeedsHediffMaterial(out PawnRenderFlags renderFlags)
	{
		renderFlags = PawnRenderFlags.None;
		if (pawn.IsPsychologicallyInvisible())
		{
			renderFlags |= PawnRenderFlags.Invisible;
			return true;
		}
		return false;
	}

	private void RenderPawnInternal(PawnDrawParms parms)
	{
		renderTree.Draw(parms);
	}

	private PawnDrawParms GetDrawParms(Vector3 rootLoc, float angle, Rot4 bodyFacing, RotDrawMode bodyDrawType, PawnRenderFlags flags)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		PawnDrawParms result = default(PawnDrawParms);
		result.pawn = pawn;
		result.matrix = Matrix4x4.TRS(rootLoc + pawn.ageTracker.CurLifeStage.bodyDrawOffset, Quaternion.AngleAxis(angle, Vector3.up), Vector3.one);
		result.facing = bodyFacing;
		result.rotDrawMode = bodyDrawType;
		result.posture = pawn.GetPosture();
		result.flags = flags;
		result.tint = flasher.CurColor.ToTransparent(InvisibilityUtility.GetAlpha(pawn));
		result.bed = pawn.CurrentBed();
		result.coveredInFoam = FirefoamOverlays.coveredInFoam;
		result.carriedThing = pawn.carryTracker?.CarriedThing;
		result.dead = pawn.Dead;
		result.crawling = pawn.Crawling;
		result.swimming = pawn.Swimming;
		result.statueColor = StatueColor;
		return result;
	}

	private Material OverrideMaterialIfNeeded(Material original, PawnRenderFlags flags)
	{
		if (flags.FlagSet(PawnRenderFlags.Cache) || flags.FlagSet(PawnRenderFlags.Portrait))
		{
			return original;
		}
		if (pawn.IsPsychologicallyInvisible())
		{
			return InvisibilityMatPool.GetInvisibleMat(original);
		}
		return flasher.GetDamagedMat(original);
	}

	private Rot4 RotationForcedByJob()
	{
		if (pawn.jobs?.curDriver != null && pawn.jobs.curDriver.ForcedLayingRotation.IsValid)
		{
			return pawn.jobs.curDriver.ForcedLayingRotation;
		}
		return Rot4.Invalid;
	}

	public Rot4 LayingFacing()
	{
		Rot4 result = RotationForcedByJob();
		if (result.IsValid)
		{
			return result;
		}
		PawnPosture posture = pawn.GetPosture();
		if (posture == PawnPosture.LayingOnGroundFaceUp || pawn.Deathresting)
		{
			return Rot4.South;
		}
		if (pawn.RaceProps.Humanlike)
		{
			if (pawn.DevelopmentalStage.Baby() && pawn.ParentHolder is Pawn_CarryTracker pawn_CarryTracker)
			{
				if (!(pawn_CarryTracker.pawn.Rotation == Rot4.West) && !(pawn_CarryTracker.pawn.Rotation == Rot4.North))
				{
					return Rot4.West;
				}
				return Rot4.East;
			}
			if (posture.FaceUp() && pawn.CurrentBed() != null)
			{
				return Rot4.South;
			}
			switch (pawn.thingIDNumber % 4)
			{
			case 0:
				return Rot4.South;
			case 1:
				return Rot4.South;
			case 2:
				return Rot4.East;
			case 3:
				return Rot4.West;
			}
		}
		else
		{
			switch (pawn.thingIDNumber % 4)
			{
			case 0:
				return Rot4.South;
			case 1:
				return Rot4.East;
			case 2:
				return Rot4.West;
			case 3:
				return Rot4.West;
			}
		}
		return Rot4.Random;
	}

	public float BodyAngle(PawnRenderFlags flags)
	{
		if (pawn.GetPosture() == PawnPosture.Standing)
		{
			return 0f;
		}
		Building_Bed building_Bed = pawn.CurrentBed();
		if (building_Bed != null && pawn.RaceProps.Humanlike)
		{
			Rot4 rotation = building_Bed.Rotation;
			rotation.AsInt += 2;
			return rotation.AsAngle;
		}
		if (pawn.ParentHolder is IThingHolderWithDrawnPawn thingHolderWithDrawnPawn)
		{
			return thingHolderWithDrawnPawn.HeldPawnBodyAngle;
		}
		if (pawn.ParentHolder.ParentHolder is IThingHolderWithDrawnPawn thingHolderWithDrawnPawn2)
		{
			return thingHolderWithDrawnPawn2.HeldPawnBodyAngle;
		}
		if (pawn.ParentHolder is Pawn_CarryTracker pawn_CarryTracker)
		{
			Rot4 rotation2 = pawn_CarryTracker.pawn.Rotation;
			return ((rotation2 == Rot4.West || rotation2 == Rot4.North) ? 290f : 70f) + pawn_CarryTracker.pawn.Drawer.renderer.BodyAngle(flags);
		}
		if (pawn.Crawling && !flags.FlagSet(PawnRenderFlags.Portrait))
		{
			return PawnRenderUtility.CrawlingBodyAngle(pawn.Rotation);
		}
		if (pawn.Downed || pawn.Dead)
		{
			return wiggler.downedAngle;
		}
		if (pawn.RaceProps.Humanlike)
		{
			return LayingFacing().AsAngle;
		}
		if (RotationForcedByJob().IsValid)
		{
			return 0f;
		}
		return ((pawn.thingIDNumber % 2 == 0) ? Rot4.West : Rot4.East).AsAngle;
	}

	public Vector3 BaseHeadOffsetAt(Rot4 rotation)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = pawn.story.bodyType.headOffset * Mathf.Sqrt(pawn.ageTracker.CurLifeStage.bodySizeFactor);
		switch (rotation.AsInt)
		{
		case 0:
			return new Vector3(0f, 0f, val.y);
		case 1:
			return new Vector3(val.x, 0f, val.y);
		case 2:
			return new Vector3(0f, 0f, val.y);
		case 3:
			return new Vector3(0f - val.x, 0f, val.y);
		default:
			Log.Error("BaseHeadOffsetAt error in " + pawn);
			return Vector3.zero;
		}
	}

	public void SetAnimation(AnimationDef animation)
	{
		if (animation == null)
		{
			renderTree.currentAnimation = null;
			renderTree.animationStartTick = -99999;
		}
		else
		{
			renderTree.currentAnimation = animation;
			renderTree.animationStartTick = Find.TickManager.TicksGame;
			if (animation.startOnRandomTick)
			{
				renderTree.animationStartTick += Rand.RangeSeeded(0, animation.durationTicks, pawn.thingIDNumber);
			}
		}
		renderTree.SetDirty();
	}

	public void SetAllGraphicsDirty()
	{
		LongEventHandler.ExecuteWhenFinished(delegate
		{
			if (renderTree.Resolved)
			{
				renderTree.SetDirty();
				SilhouetteUtility.NotifyGraphicDirty(pawn);
				WoundOverlays.ClearCache();
				PortraitsCache.SetDirty(pawn);
				GlobalTextureAtlasManager.TryMarkPawnFrameSetDirty(pawn);
			}
		});
	}

	public void Notify_DamageApplied(DamageInfo dam)
	{
		flasher.Notify_DamageApplied(dam);
		wiggler.Notify_DamageApplied(dam);
	}

	public void ProcessPostTickVisuals(int ticksPassed)
	{
		wiggler.ProcessPostTickVisuals(ticksPassed);
		if (renderTree.AnimationFinished)
		{
			SetAnimation(null);
		}
	}

	public void EffectersTick(bool suspended)
	{
		effecters.EffectersTick(suspended);
	}

	private void DrawDebug()
	{
		if (DebugViewSettings.drawDuties && Find.Selector.IsSelected(pawn))
		{
			pawn.mindState?.duty?.DrawDebug(pawn);
		}
	}
}
