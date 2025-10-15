using UnityEngine;

namespace Verse;

public class Graphic_MoteWithAgeSecs : Graphic_Mote
{
	protected override bool ForcePropertyBlock => true;

	public override void DrawWorker(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, float extraRotation)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		Graphic_Mote.propertyBlock.SetColor(ShaderPropertyIDs.Color, color);
		if (thing == null)
		{
			bool valueOrDefault = (thingDef?.mote?.realTime).GetValueOrDefault();
			float valueOrDefault2 = ((float?)Find.TickManager?.TicksGame / 60f).GetValueOrDefault();
			Graphic_Mote.propertyBlock.SetFloat(ShaderPropertyIDs.AgeSecs, valueOrDefault ? Time.realtimeSinceStartup : valueOrDefault2);
			Graphic_Mote.propertyBlock.SetFloat(ShaderPropertyIDs.AgeSecsPausable, valueOrDefault2);
			Graphic_Mote.DrawMote(data, MatSingle, loc, 0f, 0, forcePropertyBlock: true);
			return;
		}
		Mote mote = (Mote)thing;
		Graphic_Mote.propertyBlock.SetFloat(ShaderPropertyIDs.AgeSecs, mote.AgeSecs);
		Graphic_Mote.propertyBlock.SetFloat(ShaderPropertyIDs.AgeSecsPausable, mote.AgeSecsPausable);
		MaterialPropertyBlock obj = Graphic_Mote.propertyBlock;
		int randomPerObject = ShaderPropertyIDs.RandomPerObject;
		int spawnTick = mote.spawnTick;
		Vector3 drawPos = mote.DrawPos;
		obj.SetFloat(randomPerObject, (float)Gen.HashCombineInt(spawnTick, ((object)(Vector3)(ref drawPos)).GetHashCode()));
		Graphic_Mote.propertyBlock.SetFloat(ShaderPropertyIDs.RandomPerObjectOffsetRandom, (float)Gen.HashCombineInt(mote.spawnTick, mote.offsetRandom));
		DrawMoteInternal(loc, rot, thingDef, thing, 0);
	}

	public override string ToString()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		string[] obj = new string[7]
		{
			"Graphic_MoteWithAgeSecs(path=",
			path,
			", shader=",
			((object)base.Shader)?.ToString(),
			", color=",
			null,
			null
		};
		Color val = color;
		obj[5] = ((object)(Color)(ref val)).ToString();
		obj[6] = ", colorTwo=unsupported)";
		return string.Concat(obj);
	}
}
