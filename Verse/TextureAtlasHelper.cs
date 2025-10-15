using System.IO;
using UnityEngine;

namespace Verse;

public static class TextureAtlasHelper
{
	public static Mesh CreateMeshForUV(Rect uv, float scale = 1f)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		Mesh val = new Mesh();
		val.vertices = (Vector3[])(object)new Vector3[4]
		{
			new Vector3(-1f * scale, 0f, -1f * scale),
			new Vector3(-1f * scale, 0f, 1f * scale),
			new Vector3(1f * scale, 0f, 1f * scale),
			new Vector3(1f * scale, 0f, -1f * scale)
		};
		val.normals = (Vector3[])(object)new Vector3[4]
		{
			Vector3.up,
			Vector3.up,
			Vector3.up,
			Vector3.up
		};
		val.uv = (Vector2[])(object)new Vector2[4]
		{
			((Rect)(ref uv)).min,
			new Vector2(((Rect)(ref uv)).xMin, ((Rect)(ref uv)).yMax),
			((Rect)(ref uv)).max,
			new Vector2(((Rect)(ref uv)).xMax, ((Rect)(ref uv)).yMin)
		};
		val.triangles = new int[6] { 0, 1, 2, 2, 3, 0 };
		return val;
	}

	public static void WriteDebugPNG(RenderTexture atlas, string path)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		Texture2D val = new Texture2D(((Texture)atlas).width, ((Texture)atlas).height, (TextureFormat)5, false);
		RenderTexture.active = atlas;
		val.ReadPixels(new Rect(0f, 0f, (float)((Texture)atlas).width, (float)((Texture)atlas).height), 0, 0);
		RenderTexture.active = null;
		File.WriteAllBytes(path, ImageConversion.EncodeToPNG(val));
	}

	public static void WriteDebugPNG(Texture2D atlas, string path)
	{
		Texture2D val = (((Texture)atlas).isReadable ? atlas : MakeReadableTextureInstance(atlas));
		File.WriteAllBytes(path, ImageConversion.EncodeToPNG(val));
		if ((Object)(object)val != (Object)(object)atlas)
		{
			Object.Destroy((Object)(object)val);
		}
	}

	public static Texture2D MakeReadableTextureInstance(Texture2D source)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Expected O, but got Unknown
		DeepProfiler.Start("MakeReadableTextureInstance");
		RenderTexture temporary = RenderTexture.GetTemporary(((Texture)source).width, ((Texture)source).height, 0, (RenderTextureFormat)7, (RenderTextureReadWrite)1);
		((Object)temporary).name = "MakeReadableTexture_Temp";
		Graphics.Blit((Texture)(object)source, temporary);
		RenderTexture active = RenderTexture.active;
		RenderTexture.active = temporary;
		Texture2D val = new Texture2D(((Texture)source).width, ((Texture)source).height);
		val.ReadPixels(new Rect(0f, 0f, (float)((Texture)temporary).width, (float)((Texture)temporary).height), 0, 0);
		val.Apply();
		RenderTexture.active = active;
		RenderTexture.ReleaseTemporary(temporary);
		DeepProfiler.End();
		return val;
	}

	public static TextureAtlasGroup ToAtlasGroup(this ThingCategory category)
	{
		return category switch
		{
			ThingCategory.Building => TextureAtlasGroup.Building, 
			ThingCategory.Plant => TextureAtlasGroup.Plant, 
			ThingCategory.Item => TextureAtlasGroup.Item, 
			ThingCategory.Filth => TextureAtlasGroup.Filth, 
			_ => TextureAtlasGroup.Misc, 
		};
	}
}
