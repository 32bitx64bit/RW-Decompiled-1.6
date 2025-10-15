using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

[StaticConstructorOnStartup]
public class Building_HoldingPlatform : Building, IThingHolderWithDrawnPawn, IThingHolder, IRoofCollapseAlert, ISearchableContents
{
	public struct Chain
	{
		public Vector3 from;

		public Vector3 to;

		public Graphic graphic;

		public Graphic baseFastenerGraphic;

		public Graphic targetFastenerGraphic;

		public float rotation;
	}

	public ThingOwner innerContainer;

	private int lastDamaged;

	private Graphic chainsUntetheredGraphic;

	private List<Chain> chains;

	private CompAffectedByFacilities facilitiesComp;

	private CompAttachPoints attachPointsComp;

	private AttachPointTracker targetPoints;

	private List<Chain> defaultPointMapping;

	[Unsaved(false)]
	private int debugEscapeTick = -1;

	private int heldPawnStartTick = -1;

	private const float ChainsUntetheredYOffset = 0.05f;

	private const float ChainsTetheredYOffset = 0.13658537f;

	private const float LurchMTBTicks = 100f;

	private const float DamageMTBDays = 2f;

	private static readonly FloatRange Damage = new FloatRange(1f, 3f);

	private const float LungeAnimationChance = 0.25f;

	private Dictionary<AttachPointType, Vector3> platformPoints;

	public float HeldPawnDrawPos_Y => DrawPos.y + 0.03658537f;

	public float HeldPawnBodyAngle => base.Rotation.AsAngle;

	public PawnPosture HeldPawnPosture => PawnPosture.LayingOnGroundFaceUp;

	public Rot4 HeldPawnRotation => base.Rotation;

	public Vector3 PawnDrawOffset => new Vector3(0f, 0f, 0.15f);

	public Pawn HeldPawn => innerContainer.FirstOrDefault((Thing x) => x is Pawn) as Pawn;

	public bool Occupied => HeldPawn != null;

	public float AnimationAlpha => Mathf.Clamp01((float)(Find.TickManager.TicksGame - heldPawnStartTick) / 20f);

	private CompAffectedByFacilities FacilitiesComp => facilitiesComp ?? (facilitiesComp = GetComp<CompAffectedByFacilities>());

	private CompAttachPoints AttachPointsComp => attachPointsComp ?? (attachPointsComp = GetComp<CompAttachPoints>());

	public ThingOwner SearchableContents => innerContainer;

	private AttachPointTracker TargetPawnAttachPoints
	{
		get
		{
			if (targetPoints != null && targetPoints.ThingId != HeldPawn.ThingID)
			{
				targetPoints = null;
			}
			bool num = targetPoints == null;
			targetPoints = targetPoints ?? HeldPawn.TryGetComp<CompAttachPoints>()?.points;
			if (num)
			{
				foreach (HediffComp_AttachPoints hediffComp in HeldPawn.health.hediffSet.GetHediffComps<HediffComp_AttachPoints>())
				{
					if (hediffComp.Points != null)
					{
						if (targetPoints == null)
						{
							targetPoints = hediffComp.Points;
						}
						else
						{
							targetPoints.Add(hediffComp.Points);
						}
					}
				}
			}
			return targetPoints;
		}
	}

	public bool HasAttachedElectroharvester
	{
		get
		{
			foreach (Thing item in FacilitiesComp.LinkedFacilitiesListForReading)
			{
				CompPowerPlantElectroharvester compPowerPlantElectroharvester = item.TryGetComp<CompPowerPlantElectroharvester>();
				if (compPowerPlantElectroharvester != null && compPowerPlantElectroharvester.PowerOn)
				{
					return true;
				}
			}
			return false;
		}
	}

	public bool HasAttachedBioferriteHarvester
	{
		get
		{
			foreach (Thing item in FacilitiesComp.LinkedFacilitiesListForReading)
			{
				if (item is Building_BioferriteHarvester building_BioferriteHarvester && building_BioferriteHarvester.Power.PowerOn)
				{
					return true;
				}
			}
			return false;
		}
	}

	private Graphic ChainsUntetheredGraphic
	{
		get
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			if (chainsUntetheredGraphic == null)
			{
				chainsUntetheredGraphic = GraphicDatabase.Get<Graphic_Single>(PlatformProps.untetheredGraphicTexPath, ShaderDatabase.Cutout, def.graphicData.drawSize, Color.white);
			}
			return chainsUntetheredGraphic;
		}
	}

	private CompProperties_EntityHolderPlatform PlatformProps => GetComp<CompEntityHolderPlatform>().Props;

	public PawnDrawParms HeldPawnDrawParms
	{
		get
		{
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			PawnDrawParms result = default(PawnDrawParms);
			result.pawn = HeldPawn;
			result.facing = HeldPawn.Rotation;
			result.rotDrawMode = RotDrawMode.Fresh;
			result.posture = HeldPawn.GetPosture();
			result.flags = PawnRenderFlags.Headgear | PawnRenderFlags.Clothes;
			result.tint = Color.white;
			return result;
		}
	}

	public List<Chain> DefaultPointMapping
	{
		get
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			if (defaultPointMapping == null)
			{
				defaultPointMapping = new List<Chain>();
				Vector3 worldPos = AttachPointsComp.points.GetWorldPos(AttachPointType.PlatformRestraint0);
				Vector3 worldPos2 = AttachPointsComp.points.GetWorldPos(AttachPointType.PlatformRestraint1);
				Vector3 worldPos3 = AttachPointsComp.points.GetWorldPos(AttachPointType.PlatformRestraint2);
				Vector3 worldPos4 = AttachPointsComp.points.GetWorldPos(AttachPointType.PlatformRestraint3);
				Vector2 val = default(Vector2);
				((Vector2)(ref val))._002Ector(Vector3.Distance(worldPos, worldPos3), 1f);
				List<Chain> list = defaultPointMapping;
				Chain item = new Chain
				{
					from = worldPos,
					to = worldPos3,
					graphic = (GraphicDatabase.Get<Graphic_Tiling>(PlatformProps.tilingChainTexPath, ShaderTypeDefOf.Cutout.Shader, val, Color.white) as Graphic_Tiling).WithTiling(val),
					baseFastenerGraphic = GraphicDatabase.Get<Graphic_Single>(PlatformProps.baseChainFastenerTexPath, ShaderTypeDefOf.Cutout.Shader, Vector2.one, Color.white),
					targetFastenerGraphic = GraphicDatabase.Get<Graphic_Single>(PlatformProps.targetChainFastenerTexPath, ShaderTypeDefOf.Cutout.Shader, Vector2.one, Color.white)
				};
				Vector3 val2 = worldPos3.WithY(0f) - worldPos.WithY(0f);
				item.rotation = ((Vector3)(ref val2)).normalized.ToAngleFlat();
				list.Add(item);
				((Vector2)(ref val))._002Ector(Vector3.Distance(worldPos2, worldPos4), 1f);
				List<Chain> list2 = defaultPointMapping;
				item = new Chain
				{
					from = worldPos2,
					to = worldPos4,
					graphic = (GraphicDatabase.Get<Graphic_Tiling>(PlatformProps.tilingChainTexPath, ShaderTypeDefOf.Cutout.Shader, val, Color.white) as Graphic_Tiling).WithTiling(val),
					baseFastenerGraphic = GraphicDatabase.Get<Graphic_Single>(PlatformProps.baseChainFastenerTexPath, ShaderTypeDefOf.Cutout.Shader, Vector2.one, Color.white),
					targetFastenerGraphic = GraphicDatabase.Get<Graphic_Single>(PlatformProps.targetChainFastenerTexPath, ShaderTypeDefOf.Cutout.Shader, Vector2.one, Color.white)
				};
				val2 = worldPos4.WithY(0f) - worldPos2.WithY(0f);
				item.rotation = ((Vector3)(ref val2)).normalized.ToAngleFlat();
				list2.Add(item);
			}
			return defaultPointMapping;
		}
	}

	public Building_HoldingPlatform()
	{
		innerContainer = new ThingOwner<Thing>(this);
	}

	public override void SpawnSetup(Map map, bool respawningAfterLoad)
	{
		if (!ModLister.CheckAnomaly("Holding platform"))
		{
			Destroy();
			return;
		}
		base.SpawnSetup(map, respawningAfterLoad);
		Find.StudyManager.UpdateStudiableCache(this, base.Map);
		Find.Anomaly.hasBuiltHoldingPlatform = true;
	}

	public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
	{
		if (mode != DestroyMode.WillReplace)
		{
			EjectContents();
		}
		platformPoints = null;
		base.DeSpawn(mode);
	}

	public void GetChildHolders(List<IThingHolder> outChildren)
	{
		ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());
	}

	public ThingOwner GetDirectlyHeldThings()
	{
		return innerContainer;
	}

	private bool TryGetFirstColonistDirection(out Vector2 direction)
	{
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		foreach (Thing item in GenRadial.RadialDistinctThingsAround(base.Position, base.Map, 4f, useCenter: false))
		{
			if (item is Pawn { IsColonist: not false } pawn)
			{
				direction = pawn.Position.ToVector2() - base.Position.ToVector2();
				return true;
			}
		}
		direction = Vector2.zero;
		return false;
	}

	protected override void Tick()
	{
		base.Tick();
		if (Occupied && chains == null && AttachPointsComp != null)
		{
			chains = ((TargetPawnAttachPoints != null) ? BuildTargetPointMapping() : DefaultPointMapping);
		}
		if (!Occupied && chains != null)
		{
			chains = null;
		}
		if (Occupied && HasAttachedElectroharvester && Rand.MTBEventOccurs(2f, 60000f, 1f))
		{
			HeldPawn.TakeDamage(new DamageInfo(DamageDefOf.ElectricalBurn, Damage.RandomInRange));
		}
		if (Occupied && Rand.MTBEventOccurs(100f, 1f, 1f))
		{
			UpdateAnimation();
		}
		if (debugEscapeTick > 0 && Find.TickManager.TicksGame == debugEscapeTick && HeldPawn != null)
		{
			HeldPawn.TryGetComp<CompHoldingPlatformTarget>()?.Escape(initiator: false);
		}
		if (heldPawnStartTick == -1 && HeldPawn != null)
		{
			heldPawnStartTick = Find.TickManager.TicksGame;
		}
		else if (HeldPawn == null)
		{
			heldPawnStartTick = -1;
		}
	}

	private void UpdateAnimation()
	{
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		if (HeldPawn.TryGetComp<CompHoldingPlatformTarget>(out var comp) && (!comp.Props.hasAnimation || HeldPawn.health.Downed))
		{
			HeldPawn.Drawer.renderer.SetAnimation(null);
			return;
		}
		SoundDef soundDef = PlatformProps.entityLungeSoundLow;
		AnimationDef animationDef = AnimationDefOf.HoldingPlatformWiggleLight;
		if (TryGetFirstColonistDirection(out var direction))
		{
			if (TargetPawnAttachPoints != null && Rand.Chance(0.25f))
			{
				Vector2 val = ((Vector2)(ref direction)).normalized.Cardinalize();
				if (val == Vector2.up)
				{
					animationDef = AnimationDefOf.HoldingPlatformLungeUp;
				}
				if (val == Vector2.right)
				{
					animationDef = AnimationDefOf.HoldingPlatformLungeRight;
				}
				if (val == Vector2.left)
				{
					animationDef = AnimationDefOf.HoldingPlatformLungeLeft;
				}
				if (val == Vector2.down)
				{
					animationDef = AnimationDefOf.HoldingPlatformLungeDown;
				}
				soundDef = PlatformProps.entityLungeSoundHi;
			}
			else
			{
				animationDef = AnimationDefOf.HoldingPlatformWiggleIntense;
			}
		}
		if (HeldPawn.Drawer.renderer.CurAnimation != animationDef)
		{
			soundDef?.PlayOneShot(this);
			HeldPawn.Drawer.renderer.SetAnimation(animationDef);
		}
	}

	public List<Chain> BuildTargetPointMapping()
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		if (chains == null)
		{
			chains = new List<Chain>();
		}
		else
		{
			chains.Clear();
		}
		HeldPawn.Drawer.renderer.renderTree.GetRootTPRS(HeldPawnDrawParms, out var offset, out var _, out var rotation, out var _);
		Vector3 val = DrawPos + PawnDrawOffset;
		Dictionary<AttachPointType, Vector3> dictionary = new Dictionary<AttachPointType, Vector3>();
		int num = 5;
		int num2 = 8;
		foreach (AttachPointType item2 in TargetPawnAttachPoints.PointTypes(num, num2))
		{
			Vector3 value = val + rotation * (offset + TargetPawnAttachPoints.GetRotatedOffset(item2, base.Rotation));
			dictionary.Add(item2, value);
		}
		Vector2 val5 = default(Vector2);
		for (int i = num; i <= num2; i++)
		{
			Vector3 val2 = GetPlatformPoints()[(AttachPointType)i];
			Vector3 val3 = dictionary[(AttachPointType)i];
			Vector3 val4 = Vector3.Lerp(val2, val3, AnimationAlpha);
			float num3 = Vector3.Distance(val4, val2);
			((Vector2)(ref val5))._002Ector(num3, 1f);
			List<Chain> list = chains;
			Chain item = new Chain
			{
				from = val2,
				to = val4,
				graphic = (GraphicDatabase.Get<Graphic_Tiling>(PlatformProps.tilingChainTexPath, ShaderTypeDefOf.CutoutTiling.Shader, val5, Color.white) as Graphic_Tiling).WithTiling(val5),
				baseFastenerGraphic = GraphicDatabase.Get<Graphic_Single>(PlatformProps.baseChainFastenerTexPath, ShaderTypeDefOf.CutoutTiling.Shader, Vector2.one, Color.white),
				targetFastenerGraphic = GraphicDatabase.Get<Graphic_Single>(PlatformProps.targetChainFastenerTexPath, ShaderTypeDefOf.CutoutTiling.Shader, Vector2.one, Color.white)
			};
			Vector3 val6 = val3.WithY(0f) - val2.WithY(0f);
			item.rotation = ((Vector3)(ref val6)).normalized.ToAngleFlat();
			list.Add(item);
		}
		return chains;
	}

	private Dictionary<AttachPointType, Vector3> GetPlatformPoints()
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		if (platformPoints == null)
		{
			platformPoints = new Dictionary<AttachPointType, Vector3>();
			int min = 5;
			int max = 8;
			foreach (AttachPointType item in AttachPointsComp.points.PointTypes(min, max))
			{
				platformPoints.Add(item, AttachPointsComp.points.GetWorldPos(item));
			}
		}
		return platformPoints;
	}

	public override void DynamicDrawPhaseAt(DrawPhase phase, Vector3 drawLoc, bool flip = false)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		base.DynamicDrawPhaseAt(phase, drawLoc, flip);
		Pawn heldPawn = HeldPawn;
		if (heldPawn != null)
		{
			Rot4 value = Rot4.South;
			if (heldPawn.IsShambler && heldPawn.RaceProps.Animal)
			{
				value = Rot4.East;
			}
			heldPawn.Drawer.renderer.DynamicDrawPhaseAt(phase, DrawPos + PawnDrawOffset, value, neverAimWeapon: true);
		}
	}

	protected override void DrawAt(Vector3 drawLoc, bool flip = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		base.DrawAt(drawLoc, flip);
		if (HeldPawn != null)
		{
			DrawChains();
		}
		else
		{
			ChainsUntetheredGraphic.Draw(drawLoc + Vector3.up * 0.05f, base.Rotation, this);
		}
	}

	private void DrawChains()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		if (chains == null)
		{
			return;
		}
		chains = ((TargetPawnAttachPoints != null) ? BuildTargetPointMapping() : DefaultPointMapping);
		Vector3 val = Vector3.up * 0.13658537f;
		foreach (Chain chain in chains)
		{
			Vector3 v = (chain.from + chain.to) / 2f;
			chain.graphic.Draw(v.WithY(DrawPos.y) + val, base.Rotation, this, chain.rotation + 180f);
			chain.targetFastenerGraphic.Draw(chain.to + 2f * val, base.Rotation, this, chain.rotation + 90f);
			chain.baseFastenerGraphic.Draw(chain.from + 2f * val, base.Rotation, this, chain.rotation + 90f);
		}
	}

	public void EjectContents()
	{
		defaultPointMapping = null;
		chains = null;
		HeldPawn?.Drawer.renderer.SetAnimation(null);
		HeldPawn?.GetComp<CompHoldingPlatformTarget>()?.Notify_ReleasedFromPlatform();
		innerContainer.TryDropAll(base.Position, base.Map, ThingPlaceMode.Near);
	}

	public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn selPawn)
	{
		foreach (FloatMenuOption floatMenuOption in base.GetFloatMenuOptions(selPawn))
		{
			yield return floatMenuOption;
		}
		if (!Occupied)
		{
			yield break;
		}
		foreach (FloatMenuOption floatMenuOption2 in HeldPawn.GetFloatMenuOptions(selPawn))
		{
			yield return floatMenuOption2;
		}
	}

	public override string GetInspectString()
	{
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		string text = base.GetInspectString();
		if (!text.NullOrEmpty())
		{
			text += "\n";
		}
		Pawn heldPawn = HeldPawn;
		if (heldPawn != null)
		{
			TaggedString ts = "HoldingThing".Translate() + ": " + heldPawn.NameShortColored.CapitalizeFirst();
			bool flag = this.SafelyContains(heldPawn);
			if (!flag)
			{
				ts += " (" + "HoldingPlatformRequiresStrength".Translate(StatDefOf.MinimumContainmentStrength.Worker.ValueToString(heldPawn.GetStatValue(StatDefOf.MinimumContainmentStrength), finalized: false)) + ")";
			}
			text += ts.Colorize(flag ? Color.white : ColorLibrary.RedReadable);
		}
		else
		{
			text += "HoldingThing".Translate() + ": " + "Nothing".Translate().CapitalizeFirst();
		}
		if (heldPawn != null && heldPawn.def.IsStudiable)
		{
			string inspectStringExtraFor = CompStudiable.GetInspectStringExtraFor(heldPawn);
			if (!inspectStringExtraFor.NullOrEmpty())
			{
				text = text + "\n" + inspectStringExtraFor;
			}
		}
		if (heldPawn != null && heldPawn.TryGetComp<CompProducesBioferrite>(out var comp))
		{
			string text2 = comp.CompInspectStringExtra();
			if (!text2.NullOrEmpty())
			{
				text = text + "\n" + text2;
			}
		}
		return text;
	}

	public override IEnumerable<InspectTabBase> GetInspectTabs()
	{
		foreach (InspectTabBase inspectTab in base.GetInspectTabs())
		{
			yield return inspectTab;
		}
		if (HeldPawn != null && HeldPawn.def.inspectorTabs.Contains(typeof(ITab_StudyNotes)))
		{
			yield return HeldPawn.GetInspectTabs().FirstOrDefault((InspectTabBase tab) => tab is ITab_StudyNotes);
		}
	}

	public void Notify_PawnDied(Pawn pawn, DamageInfo? dinfo)
	{
		if (pawn == HeldPawn)
		{
			innerContainer.TryDropAll(base.Position, base.Map, ThingPlaceMode.Near);
			if (!dinfo.HasValue || !dinfo.Value.Def.execution)
			{
				Messages.Message("EntityDiedOnHoldingPlatform".Translate(pawn), pawn, MessageTypeDefOf.NegativeEvent);
			}
		}
	}

	public override IEnumerable<Gizmo> GetGizmos()
	{
		foreach (Gizmo gizmo2 in base.GetGizmos())
		{
			yield return gizmo2;
		}
		if (HeldPawn != null && HeldPawn.TryGetComp<CompActivity>(out var comp))
		{
			foreach (Gizmo item in comp.CompGetGizmosExtra())
			{
				yield return item;
			}
		}
		if (HeldPawn != null && HeldPawn.TryGetComp<CompStudiable>(out var comp2))
		{
			foreach (Gizmo item2 in comp2.CompGetGizmosExtra())
			{
				yield return item2;
			}
		}
		if (HeldPawn != null && HeldPawn.TryGetComp<CompHoldingPlatformTarget>(out var comp3))
		{
			foreach (Gizmo item3 in comp3.CompGetGizmosExtra())
			{
				yield return item3;
			}
		}
		foreach (Thing item4 in (IEnumerable<Thing>)innerContainer)
		{
			Gizmo gizmo = Building.SelectContainedItemGizmo(this, item4);
			if (gizmo != null)
			{
				yield return gizmo;
			}
		}
		if (!DebugSettings.ShowDevGizmos || HeldPawn == null)
		{
			yield break;
		}
		yield return new Command_Action
		{
			defaultLabel = "DEV: Timed escape",
			action = delegate
			{
				List<FloatMenuOption> list = new List<FloatMenuOption>();
				for (int i = 1; i < 21; i++)
				{
					int delay = i * 60;
					list.Add(new FloatMenuOption(delay.TicksToSeconds() + "s", delegate
					{
						debugEscapeTick = Find.TickManager.TicksGame + delay;
					}));
				}
				Find.WindowStack.Add(new FloatMenu(list));
			}
		};
	}

	public RoofCollapseResponse Notify_OnBeforeRoofCollapse()
	{
		if (!Occupied)
		{
			return RoofCollapseResponse.None;
		}
		if (HeldPawn is IRoofCollapseAlert roofCollapseAlert)
		{
			roofCollapseAlert.Notify_OnBeforeRoofCollapse();
		}
		foreach (IRoofCollapseAlert comp in HeldPawn.GetComps<IRoofCollapseAlert>())
		{
			comp.Notify_OnBeforeRoofCollapse();
		}
		return RoofCollapseResponse.None;
	}

	public override void Notify_DefsHotReloaded()
	{
		base.Notify_DefsHotReloaded();
		chains = null;
	}

	public override void ExposeData()
	{
		base.ExposeData();
		Scribe_Values.Look(ref lastDamaged, "lastDamaged", 0);
		Scribe_Deep.Look(ref innerContainer, "innerContainer", this);
		Scribe_Values.Look(ref heldPawnStartTick, "heldPawnStartTick", 0);
	}
}
