using UnityEngine;
using Verse;

namespace RimWorld;

public abstract class RitualVisualEffectComp : IExposable
{
	protected RitualVisualEffect parent;

	public CompProperties_RitualVisualEffect props;

	public const float minScaleForRoom = 0.35f;

	public static readonly IntRange roomDimensionRange = new IntRange(7, 18);

	protected virtual ThingDef MoteDef => props.moteDef;

	public virtual bool ShouldSpawnNow(LordJob_Ritual ritual)
	{
		return false;
	}

	protected abstract Vector3 SpawnPos(LordJob_Ritual ritual);

	public virtual Mote SpawnMote(LordJob_Ritual ritual, Vector3? forcedPos = null)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		Vector3? val = ScaledPos(ritual, forcedPos);
		if (!val.HasValue)
		{
			return null;
		}
		Mote mote = MoteMaker.MakeStaticMote(val.Value, ritual.Map, MoteDef);
		if (mote == null)
		{
			return null;
		}
		mote.exactRotation = props.rotation.RandomInRange;
		mote.Scale = props.scale.RandomInRange * (props.scaleWithRoom ? ScaleForRoom(ritual) : 1f);
		mote.rotationRate = props.rotationRate.RandomInRange;
		if (!props.overrideColors.NullOrEmpty())
		{
			mote.instanceColor = props.overrideColors.RandomElement();
		}
		if (mote.def.mote.needsMaintenance)
		{
			mote.Maintain();
		}
		return mote;
	}

	public virtual void SpawnFleck(LordJob_Ritual ritual, Vector3? forcedPos = null, float? exactRotation = null)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		Vector3? val = ScaledPos(ritual, forcedPos);
		if (val.HasValue)
		{
			FleckCreationData dataStatic = FleckMaker.GetDataStatic(val.Value, ritual.Map, props.fleckDef);
			if (exactRotation.HasValue)
			{
				dataStatic.rotation = exactRotation.Value;
			}
			else
			{
				dataStatic.rotation = props.rotation.RandomInRange;
			}
			dataStatic.scale = props.scale.RandomInRange * (props.scaleWithRoom ? ScaleForRoom(ritual) : 1f);
			dataStatic.rotationRate = props.rotationRate.RandomInRange;
			dataStatic.velocity = props.velocityDir * props.velocity.RandomInRange;
			if (!props.overrideColors.NullOrEmpty())
			{
				dataStatic.instanceColor = props.overrideColors.RandomElement();
			}
			else if (props.colorOverride.HasValue)
			{
				dataStatic.instanceColor = props.colorOverride;
			}
			ritual.Map.flecks.CreateFleck(dataStatic);
		}
	}

	public Vector3? ScaledPos(LordJob_Ritual ritual, Vector3? forcedPos = null)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = ritual.Spot.ToVector3Shifted();
		Vector3 val2 = (Vector3)(((_003F?)forcedPos) ?? SpawnPos(ritual)) + props.offset;
		if (props.scalePositionWithRoom)
		{
			float num = ScaleForRoom(ritual);
			Vector3 val3 = (val2 - val) * num;
			val2 = val + val3;
		}
		if (props.onlySpawnInSameRoom && (new IntVec3(val2) + props.roomCheckOffset).GetRoom(ritual.Map) != ritual.GetRoom)
		{
			return null;
		}
		return val2;
	}

	public virtual Effecter SpawnEffecter(LordJob_Ritual ritual, TargetInfo target, Vector3? forcedPos = null)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		TargetInfo a = (target.IsValid ? target : ritual.selectedTarget);
		Effecter effecter = props.effecterDef.Spawn(a.Cell, parent.ritual.Map, props.offset);
		effecter.Trigger(a, TargetInfo.Invalid);
		return effecter;
	}

	public virtual void OnSetup(RitualVisualEffect parent, LordJob_Ritual ritual, bool loading)
	{
		this.parent = parent;
	}

	public virtual void TickInterval(int delta)
	{
		if (!ShouldSpawnNow(parent.ritual))
		{
			return;
		}
		if (MoteDef != null)
		{
			Mote mote = SpawnMote(parent.ritual);
			if (mote != null && mote.def.mote.needsMaintenance)
			{
				parent.AddMoteToMaintain(mote);
			}
		}
		if (props.fleckDef != null)
		{
			SpawnFleck(parent.ritual);
		}
		if (props.effecterDef != null)
		{
			SpawnEffecter(parent.ritual, TargetInfo.Invalid);
		}
	}

	public float ScaleForRoom(LordJob_Ritual ritual)
	{
		Room getRoom = ritual.GetRoom;
		if (getRoom == null || getRoom.PsychologicallyOutdoors || !getRoom.ProperRoom || ritual.RoomBoundsCached.IsInvalid)
		{
			return 1f;
		}
		float num = (float)(ritual.RoomBoundsCached.x + ritual.RoomBoundsCached.z) / 2f;
		return Mathf.Lerp(0.35f, 1f, Mathf.Clamp01(Mathf.InverseLerp((float)roomDimensionRange.min, (float)roomDimensionRange.max, num)));
	}

	public virtual void ExposeData()
	{
	}
}
