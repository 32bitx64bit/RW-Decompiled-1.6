using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;

namespace Verse;

public class Graphic
{
	private struct AtlasReplacementInfoCacheKey : IEquatable<AtlasReplacementInfoCacheKey>
	{
		public readonly Material mat;

		public readonly TextureAtlasGroup group;

		public readonly bool flipUv;

		public readonly bool vertexColors;

		private readonly int hash;

		public AtlasReplacementInfoCacheKey(Material mat, TextureAtlasGroup group, bool flipUv, bool vertexColors)
		{
			this.mat = mat;
			this.group = group;
			this.flipUv = flipUv;
			this.vertexColors = vertexColors;
			hash = Gen.HashCombine(((object)mat).GetHashCode(), group.GetHashCode());
			if (flipUv)
			{
				hash = ~hash;
			}
			if (vertexColors)
			{
				hash ^= 123893723;
			}
		}

		public bool Equals(AtlasReplacementInfoCacheKey other)
		{
			if (mat == other.mat && group == other.group && flipUv == other.flipUv)
			{
				return vertexColors == other.vertexColors;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return hash;
		}
	}

	private struct CachedAtlasReplacementInfo
	{
		public Material material;

		public Vector2[] uvs;

		public Color32 vertexColor;
	}

	public GraphicData data;

	public string path;

	public string maskPath;

	public Color color = Color.white;

	public Color colorTwo = Color.white;

	public Vector2 drawSize = Vector2.one;

	private Graphic_Shadow cachedShadowGraphicInt;

	private Graphic cachedShadowlessGraphicInt;

	private static Dictionary<AtlasReplacementInfoCacheKey, CachedAtlasReplacementInfo> replacementInfoCache = new Dictionary<AtlasReplacementInfoCacheKey, CachedAtlasReplacementInfo>();

	public Shader Shader
	{
		get
		{
			Material matSingle = MatSingle;
			if ((Object)(object)matSingle != (Object)null)
			{
				return matSingle.shader;
			}
			return ShaderDatabase.Cutout;
		}
	}

	public Graphic_Shadow ShadowGraphic
	{
		get
		{
			if (cachedShadowGraphicInt == null && data != null && data.shadowData != null)
			{
				cachedShadowGraphicInt = new Graphic_Shadow(data.shadowData);
			}
			return cachedShadowGraphicInt;
		}
		set
		{
			cachedShadowGraphicInt = value;
		}
	}

	public Color Color => color;

	public Color ColorTwo => colorTwo;

	public virtual Material MatSingle => BaseContent.BadMat;

	public virtual Material MatWest => MatSingle;

	public virtual Material MatSouth => MatSingle;

	public virtual Material MatEast => MatSingle;

	public virtual Material MatNorth => MatSingle;

	public virtual bool WestFlipped
	{
		get
		{
			if (DataAllowsFlip)
			{
				return !ShouldDrawRotated;
			}
			return false;
		}
	}

	public virtual bool EastFlipped => false;

	public virtual bool ShouldDrawRotated => false;

	public virtual float DrawRotatedExtraAngleOffset => 0f;

	public virtual bool UseSameGraphicForGhost => false;

	protected bool DataAllowsFlip
	{
		get
		{
			if (data != null)
			{
				return data.allowFlip;
			}
			return true;
		}
	}

	public static bool TryGetTextureAtlasReplacementInfo(Material mat, TextureAtlasGroup group, bool flipUv, bool vertexColors, out Material material, out Vector2[] uvs, out Color32 vertexColor)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		material = mat;
		uvs = null;
		vertexColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
		AtlasReplacementInfoCacheKey key = new AtlasReplacementInfoCacheKey(mat, group, flipUv, vertexColors);
		if (replacementInfoCache.TryGetValue(key, out var value))
		{
			material = value.material;
			uvs = value.uvs;
			if (vertexColors)
			{
				vertexColor = value.vertexColor;
			}
			return true;
		}
		if (GlobalTextureAtlasManager.TryGetStaticTile(group, (Texture2D)mat.mainTexture, out var tile))
		{
			if (!MaterialPool.TryGetRequestForMat(mat, out var request))
			{
				Log.Error("Tried getting texture atlas replacement info for a material that was not created by MaterialPool!");
				return false;
			}
			uvs = (Vector2[])(object)new Vector2[4];
			Printer_Plane.GetUVs(tile.uvRect, out uvs[0], out uvs[1], out uvs[2], out uvs[3], flipUv);
			request.mainTex = (Texture)(object)tile.atlas.ColorTexture;
			if (vertexColors)
			{
				vertexColor = Color32.op_Implicit(request.color);
				request.color = Color.white;
			}
			if ((Object)(object)request.maskTex != (Object)null)
			{
				request.maskTex = tile.atlas.MaskTexture;
			}
			material = MaterialPool.MatFrom(request);
			replacementInfoCache.Add(key, new CachedAtlasReplacementInfo
			{
				material = material,
				uvs = uvs,
				vertexColor = vertexColor
			});
			return true;
		}
		return false;
	}

	public virtual void TryInsertIntoAtlas(TextureAtlasGroup groupKey)
	{
	}

	public virtual void Init(GraphicRequest req)
	{
		Log.ErrorOnce($"Cannot init Graphic of class {GetType()}", 658928);
	}

	public virtual Material NodeGetMat(PawnDrawParms parms)
	{
		return MatAt(parms.facing, parms.pawn);
	}

	public virtual Material MatAt(Rot4 rot, Thing thing = null)
	{
		return (Material)(rot.AsInt switch
		{
			0 => MatNorth, 
			1 => MatEast, 
			2 => MatSouth, 
			3 => MatWest, 
			_ => BaseContent.BadMat, 
		});
	}

	public virtual Mesh MeshAt(Rot4 rot)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = drawSize;
		if (rot.IsHorizontal && !ShouldDrawRotated)
		{
			val = val.Rotated();
		}
		if ((rot == Rot4.West && WestFlipped) || (rot == Rot4.East && EastFlipped))
		{
			return MeshPool.GridPlaneFlip(val);
		}
		return MeshPool.GridPlane(val);
	}

	public virtual Material MatSingleFor(Thing thing)
	{
		return MatSingle;
	}

	public Vector3 DrawOffset(Rot4 rot)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (data == null)
		{
			return Vector3.zero;
		}
		return data.DrawOffsetForRot(rot);
	}

	public void Draw(Vector3 loc, Rot4 rot, Thing thing, float extraRotation = 0f)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		DrawWorker(loc, rot, thing.def, thing, extraRotation);
	}

	public void DrawFromDef(Vector3 loc, Rot4 rot, ThingDef thingDef, float extraRotation = 0f)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		DrawWorker(loc, rot, thingDef, null, extraRotation);
	}

	public virtual void DrawWorker(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, float extraRotation)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		Mesh mesh = MeshAt(rot);
		Quaternion val = QuatFromRot(rot);
		if (extraRotation != 0f)
		{
			val *= Quaternion.Euler(Vector3.up * extraRotation);
		}
		if (data != null && data.addTopAltitudeBias)
		{
			val *= Quaternion.Euler(Vector3.left * 2f);
		}
		loc += DrawOffset(rot);
		Material mat = MatAt(rot, thing);
		DrawMeshInt(mesh, loc, val, mat);
		if (ShadowGraphic != null)
		{
			ShadowGraphic.DrawWorker(loc, rot, thingDef, thing, extraRotation);
		}
	}

	protected virtual void DrawMeshInt(Mesh mesh, Vector3 loc, Quaternion quat, Material mat)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		Graphics.DrawMesh(mesh, loc, quat, mat, 0);
	}

	public virtual void Print(SectionLayer layer, Thing thing, float extraRotation)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val;
		bool flag;
		if (ShouldDrawRotated)
		{
			val = drawSize;
			flag = false;
		}
		else
		{
			val = (thing.Rotation.IsHorizontal ? drawSize.Rotated() : drawSize);
			flag = (thing.Rotation == Rot4.West && WestFlipped) || (thing.Rotation == Rot4.East && EastFlipped);
		}
		if (thing.MultipleItemsPerCellDrawn())
		{
			val *= 0.8f;
		}
		float num = AngleFromRot(thing.Rotation) + extraRotation;
		if (flag && data != null)
		{
			num += data.flipExtraRotation;
		}
		Vector3 center = thing.TrueCenter() + DrawOffset(thing.Rotation);
		Material material = MatAt(thing.Rotation, thing);
		TryGetTextureAtlasReplacementInfo(material, thing.def.category.ToAtlasGroup(), flag, vertexColors: true, out material, out var uvs, out var vertexColor);
		Printer_Plane.PrintPlane(layer, center, val, material, num, flag, uvs, (Color32[])(object)new Color32[4] { vertexColor, vertexColor, vertexColor, vertexColor });
		ShadowGraphic?.Print(layer, thing, 0f);
	}

	public virtual Graphic GetColoredVersion(Shader newShader, Color newColor, Color newColorTwo)
	{
		Log.ErrorOnce("CloneColored not implemented on this subclass of Graphic: " + GetType().ToString(), 66300);
		return BaseContent.BadGraphic;
	}

	[Obsolete("Will be removed in a future release")]
	public virtual Graphic GetCopy(Vector2 newDrawSize)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return GetCopy(newDrawSize, null);
	}

	public virtual Graphic GetCopy(Vector2 newDrawSize, Shader overrideShader)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		return GraphicDatabase.Get(GetType(), path, overrideShader ?? Shader, newDrawSize, color, colorTwo);
	}

	public virtual Graphic GetShadowlessGraphic()
	{
		if (data == null || data.shadowData == null)
		{
			return this;
		}
		if (cachedShadowlessGraphicInt == null)
		{
			GraphicData graphicData = new GraphicData();
			graphicData.CopyFrom(data);
			graphicData.shadowData = null;
			cachedShadowlessGraphicInt = graphicData.Graphic;
		}
		return cachedShadowlessGraphicInt;
	}

	protected float AngleFromRot(Rot4 rot)
	{
		if (ShouldDrawRotated)
		{
			float asAngle = rot.AsAngle;
			asAngle += DrawRotatedExtraAngleOffset;
			if ((rot == Rot4.West && WestFlipped) || (rot == Rot4.East && EastFlipped))
			{
				asAngle += 180f;
			}
			return asAngle;
		}
		return 0f;
	}

	public Quaternion QuatFromRot(Rot4 rot)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		float num = AngleFromRot(rot);
		if (num == 0f)
		{
			return Quaternion.identity;
		}
		return Quaternion.AngleAxis(num, Vector3.up);
	}
}
