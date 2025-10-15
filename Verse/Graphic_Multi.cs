using UnityEngine;

namespace Verse;

public class Graphic_Multi : Graphic
{
	private Material[] mats = (Material[])(object)new Material[4];

	private bool westFlipped;

	private bool eastFlipped;

	private float drawRotatedExtraAngleOffset;

	public const string NorthSuffix = "_north";

	public const string SouthSuffix = "_south";

	public const string EastSuffix = "_east";

	public const string WestSuffix = "_west";

	public string GraphicPath => path;

	public override Material MatSingle => MatSouth;

	public override Material MatWest => mats[3];

	public override Material MatSouth => mats[2];

	public override Material MatEast => mats[1];

	public override Material MatNorth => mats[0];

	public override bool WestFlipped => westFlipped;

	public override bool EastFlipped => eastFlipped;

	public override bool ShouldDrawRotated
	{
		get
		{
			if (data != null && !data.drawRotated)
			{
				return false;
			}
			if (!((Object)(object)MatEast == (Object)(object)MatNorth))
			{
				return (Object)(object)MatWest == (Object)(object)MatNorth;
			}
			return true;
		}
	}

	public override float DrawRotatedExtraAngleOffset => drawRotatedExtraAngleOffset;

	public override void TryInsertIntoAtlas(TextureAtlasGroup groupKey)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected O, but got Unknown
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected O, but got Unknown
		Material[] array = mats;
		foreach (Material val in array)
		{
			Texture2D mask = null;
			if (val.HasProperty(ShaderPropertyIDs.MaskTex))
			{
				mask = (Texture2D)val.GetTexture(ShaderPropertyIDs.MaskTex);
			}
			GlobalTextureAtlasManager.TryInsertStatic(groupKey, (Texture2D)val.mainTexture, mask);
		}
	}

	public override void Init(GraphicRequest req)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		data = req.graphicData;
		path = req.path;
		maskPath = req.maskPath;
		color = req.color;
		colorTwo = req.colorTwo;
		drawSize = req.drawSize;
		Texture2D[] array = (Texture2D[])(object)new Texture2D[mats.Length];
		array[0] = ContentFinder<Texture2D>.Get(req.path + "_north", reportFailure: false);
		array[1] = ContentFinder<Texture2D>.Get(req.path + "_east", reportFailure: false);
		array[2] = ContentFinder<Texture2D>.Get(req.path + "_south", reportFailure: false);
		array[3] = ContentFinder<Texture2D>.Get(req.path + "_west", reportFailure: false);
		if ((Object)(object)array[0] == (Object)null)
		{
			if ((Object)(object)array[2] != (Object)null)
			{
				array[0] = array[2];
				drawRotatedExtraAngleOffset = 180f;
			}
			else if ((Object)(object)array[1] != (Object)null)
			{
				array[0] = array[1];
				drawRotatedExtraAngleOffset = -90f;
			}
			else if ((Object)(object)array[3] != (Object)null)
			{
				array[0] = array[3];
				drawRotatedExtraAngleOffset = 90f;
			}
			else
			{
				array[0] = ContentFinder<Texture2D>.Get(req.path, reportFailure: false);
			}
		}
		if ((Object)(object)array[0] == (Object)null)
		{
			Log.Error("Failed to find any textures at " + req.path + " while constructing " + this.ToStringSafe());
			mats[0] = (mats[1] = (mats[2] = (mats[3] = BaseContent.BadMat)));
			return;
		}
		if ((Object)(object)array[2] == (Object)null)
		{
			array[2] = array[0];
		}
		if ((Object)(object)array[1] == (Object)null)
		{
			if ((Object)(object)array[3] != (Object)null)
			{
				array[1] = array[3];
				eastFlipped = base.DataAllowsFlip;
			}
			else
			{
				array[1] = array[0];
			}
		}
		if ((Object)(object)array[3] == (Object)null)
		{
			if ((Object)(object)array[1] != (Object)null)
			{
				array[3] = array[1];
				westFlipped = base.DataAllowsFlip;
			}
			else
			{
				array[3] = array[0];
			}
		}
		Texture2D[] array2 = (Texture2D[])(object)new Texture2D[mats.Length];
		if (req.shader.SupportsMaskTex())
		{
			string text = (maskPath.NullOrEmpty() ? path : maskPath);
			string text2 = (maskPath.NullOrEmpty() ? "m" : string.Empty);
			array2[0] = ContentFinder<Texture2D>.Get(text + "_north" + text2, reportFailure: false);
			array2[1] = ContentFinder<Texture2D>.Get(text + "_east" + text2, reportFailure: false);
			array2[2] = ContentFinder<Texture2D>.Get(text + "_south" + text2, reportFailure: false);
			array2[3] = ContentFinder<Texture2D>.Get(text + "_west" + text2, reportFailure: false);
			if ((Object)(object)array2[0] == (Object)null)
			{
				if ((Object)(object)array2[2] != (Object)null)
				{
					array2[0] = array2[2];
				}
				else if ((Object)(object)array2[1] != (Object)null)
				{
					array2[0] = array2[1];
				}
				else if ((Object)(object)array2[3] != (Object)null)
				{
					array2[0] = array2[3];
				}
			}
			if ((Object)(object)array2[2] == (Object)null)
			{
				array2[2] = array2[0];
			}
			if ((Object)(object)array2[1] == (Object)null)
			{
				if ((Object)(object)array2[3] != (Object)null)
				{
					array2[1] = array2[3];
				}
				else
				{
					array2[1] = array2[0];
				}
			}
			if ((Object)(object)array2[3] == (Object)null)
			{
				if ((Object)(object)array2[1] != (Object)null)
				{
					array2[3] = array2[1];
				}
				else
				{
					array2[3] = array2[0];
				}
			}
		}
		for (int i = 0; i < mats.Length; i++)
		{
			MaterialRequest req2 = default(MaterialRequest);
			req2.mainTex = (Texture)(object)array[i];
			req2.shader = req.shader;
			req2.color = color;
			req2.colorTwo = colorTwo;
			req2.maskTex = array2[i];
			req2.shaderParameters = req.shaderParameters;
			req2.renderQueue = req.renderQueue;
			mats[i] = MaterialPool.MatFrom(req2);
		}
	}

	public override Graphic GetColoredVersion(Shader newShader, Color newColor, Color newColorTwo)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		return GraphicDatabase.Get<Graphic_Multi>(path, newShader, drawSize, newColor, newColorTwo, data, maskPath);
	}

	public override string ToString()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		string[] obj = new string[7] { "Multi(initPath=", path, ", color=", null, null, null, null };
		Color val = color;
		obj[3] = ((object)(Color)(ref val)).ToString();
		obj[4] = ", colorTwo=";
		val = colorTwo;
		obj[5] = ((object)(Color)(ref val)).ToString();
		obj[6] = ")";
		return string.Concat(obj);
	}

	public override int GetHashCode()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		return Gen.HashCombineStruct<Color>(Gen.HashCombineStruct<Color>(Gen.HashCombine(0, path), color), colorTwo);
	}
}
