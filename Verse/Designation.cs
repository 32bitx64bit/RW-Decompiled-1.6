using RimWorld;
using UnityEngine;

namespace Verse;

public class Designation : IExposable
{
	public DesignationManager designationManager;

	public DesignationDef def;

	public LocalTargetInfo target;

	public ColorDef colorDef;

	[Unsaved(false)]
	private Material cachedMaterial;

	public const float ClaimedDesignationDrawAltitude = 15f;

	private Map Map => designationManager.map;

	public float DesignationDrawAltitude => AltitudeLayer.MetaOverlays.AltitudeFor();

	public Material IconMat
	{
		get
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Expected O, but got Unknown
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)cachedMaterial == (Object)null)
			{
				if (colorDef != null)
				{
					cachedMaterial = new Material(def.iconMat);
					cachedMaterial.color = colorDef.color;
				}
				else
				{
					cachedMaterial = def.iconMat;
				}
			}
			return cachedMaterial;
		}
	}

	public Designation()
	{
	}

	public Designation(LocalTargetInfo target, DesignationDef def, ColorDef colorDef = null)
	{
		this.target = target;
		this.def = def;
		this.colorDef = colorDef;
	}

	public void ExposeData()
	{
		Scribe_Defs.Look(ref def, "def");
		Scribe_TargetInfo.Look(ref target, "target");
		Scribe_Defs.Look(ref colorDef, "colorDef");
		if (Scribe.mode == LoadSaveMode.ResolvingCrossRefs && def == DesignationDefOf.Haul && !target.HasThing)
		{
			Log.Error("Haul designation has no target! Deleting.");
			Delete();
		}
	}

	public void Notify_Added()
	{
		if (def == DesignationDefOf.Haul)
		{
			Map.listerHaulables.HaulDesignationAdded(target.Thing);
		}
	}

	internal void Notify_Removing()
	{
		if (def == DesignationDefOf.Haul && target.HasThing)
		{
			Map.listerHaulables.HaulDesignationRemoved(target.Thing);
		}
	}

	public Vector3 DrawLoc()
	{
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		if (target.HasThing)
		{
			Vector3 val = target.Thing.DrawPos;
			val.y = DesignationDrawAltitude;
			if (target.Thing.def.building != null && target.Thing.def.building.isAttachment)
			{
				val += (target.Thing.Rotation.AsVector2 * 0.5f).ToVector3();
			}
			return val;
		}
		return target.Cell.ToVector3ShiftedWithAltitude(DesignationDrawAltitude);
	}

	public virtual void DesignationDraw()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (!target.HasThing || target.Thing.Spawned)
		{
			Graphics.DrawMesh(MeshPool.plane10, DrawLoc(), Quaternion.identity, IconMat, 0);
		}
	}

	public void Delete()
	{
		Map.designationManager.RemoveDesignation(this);
	}

	public override string ToString()
	{
		string[] obj = new string[5] { "(", def.defName, " target=", null, null };
		LocalTargetInfo localTargetInfo = target;
		obj[3] = localTargetInfo.ToString();
		obj[4] = ")";
		return string.Format(string.Concat(obj));
	}
}
