using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace Verse;

public class StaticTextureAtlas
{
	public readonly TextureAtlasGroupKey groupKey;

	private List<Texture2D> textures = new List<Texture2D>();

	private Dictionary<Texture2D, Texture2D> masks = new Dictionary<Texture2D, Texture2D>();

	private Dictionary<Texture, StaticTextureAtlasTile> tiles = new Dictionary<Texture, StaticTextureAtlasTile>();

	private Texture2D colorTexture;

	private Texture2D maskTexture;

	public const int MaxTextureSizeForTiles = 512;

	public const int TexturePadding = 8;

	public Texture2D ColorTexture => colorTexture;

	public Texture2D MaskTexture => maskTexture;

	public static int MaxPixelsPerAtlas => MaxAtlasSize / 2 * (MaxAtlasSize / 2);

	public static int MaxAtlasSize => SystemInfo.maxTextureSize;

	public StaticTextureAtlas(TextureAtlasGroupKey groupKey)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		this.groupKey = groupKey;
		colorTexture = new Texture2D(1, 1, (TextureFormat)5, false);
	}

	public void Insert(Texture2D texture, Texture2D mask = null)
	{
		if (groupKey.hasMask && (Object)(object)mask == (Object)null)
		{
			Log.Error("Tried to insert a mask-less texture into a static atlas which does have a mask atlas");
		}
		if (!groupKey.hasMask && (Object)(object)mask != (Object)null)
		{
			Log.Error("Tried to insert a mask texture into a static atlas which does not have a mask atlas");
		}
		textures.Add(texture);
		if ((Object)(object)mask != (Object)null && groupKey.hasMask)
		{
			masks.Add(texture, mask);
		}
	}

	public void Bake(bool rebake = false)
	{
		using (new DeepProfilerScope("StaticTextureAtlas.Bake()"))
		{
			if (rebake)
			{
				foreach (KeyValuePair<Texture, StaticTextureAtlasTile> tile in tiles)
				{
					Object.Destroy((Object)(object)tile.Value.mesh);
				}
				tiles.Clear();
			}
			Rect[] array = CalcRectsForAtlasNew();
			if (array.Length != textures.Count)
			{
				Log.Error("Texture packing failed! Clearing out atlas...");
				textures.Clear();
				return;
			}
			bool flag = !UnityData.ComputeShadersSupported;
			BlitTexturesToColorAtlas(array, flag);
			if (groupKey.hasMask)
			{
				BuildMaskAtlas(array, flag);
			}
			BuildMeshesForUvs(array);
			if (Prefs.TextureCompression)
			{
				ApplyTextureCompression(flag);
			}
			if (flag)
			{
				DeepProfiler.Start("Final Texture2D.Apply() for atlas textures");
				if ((Object)(object)colorTexture != (Object)null)
				{
					colorTexture.Apply(false, true);
				}
				if ((Object)(object)maskTexture != (Object)null)
				{
					maskTexture.Apply(false, true);
				}
				DeepProfiler.End();
			}
		}
	}

	private void ApplyTextureCompression(bool noGpuCompressionSupport)
	{
		DeepProfiler.Start("Compress atlas textures");
		if ((Object)(object)colorTexture != (Object)null)
		{
			if (noGpuCompressionSupport)
			{
				colorTexture.Compress(true);
			}
			else
			{
				string name = ((Object)colorTexture).name;
				colorTexture = FastCompressDXT(colorTexture, deleteOriginal: true);
				((Object)colorTexture).name = name;
			}
		}
		if ((Object)(object)maskTexture != (Object)null)
		{
			if (noGpuCompressionSupport)
			{
				maskTexture.Compress(true);
			}
			else
			{
				string name2 = ((Object)maskTexture).name;
				maskTexture = FastCompressDXT(maskTexture, deleteOriginal: true);
				((Object)maskTexture).name = name2;
			}
		}
		DeepProfiler.End();
	}

	private void BuildMeshesForUvs(Rect[] uvRects)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < textures.Count; i++)
		{
			Mesh val = TextureAtlasHelper.CreateMeshForUV(uvRects[i], 0.5f);
			((Object)val).name = "TextureAtlasMesh_" + groupKey.ToString() + "_" + ((Object)val).GetInstanceID();
			tiles.Add((Texture)(object)textures[i], new StaticTextureAtlasTile
			{
				atlas = this,
				mesh = val,
				uvRect = uvRects[i]
			});
		}
	}

	private Rect[] CalcRectsForAtlasNew(int divisor = 1, bool useIncreasedShortAxis = false)
	{
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_0352: Expected O, but got Unknown
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		DeepProfiler.Start("StaticTextureAtlas.CalcRectsForAtlasNew()");
		List<Texture2D> list = textures.OrderByDescending((Texture2D t) => ((Texture)t).height).ToList();
		int num = list.Sum((Texture2D t) => ((Texture)t).width * ((Texture)t).height);
		int num2 = Mathf.NextPowerOfTwo((int)((float)Mathf.CeilToInt(Mathf.Sqrt((float)num)) * 1.05f));
		num2 = Mathf.Min(num2, MaxAtlasSize);
		int num3 = Mathf.NextPowerOfTwo(Mathf.CeilToInt((float)num / (float)num2));
		if (useIncreasedShortAxis)
		{
			num3 *= 2;
		}
		num3 = Mathf.Min(num3, MaxAtlasSize);
		if (divisor > 1)
		{
			num3 /= divisor;
			int num4 = num3 * num2;
			int num5 = num / (divisor * divisor);
			if ((float)num4 < (float)num5 * 1.5f)
			{
				num3 *= divisor;
			}
		}
		List<Vector2> list2 = ((IEnumerable<Texture2D>)list).Select((Func<Texture2D, Vector2>)((Texture2D t) => new Vector2((float)Mathf.RoundToInt((float)((Texture)t).width / (float)divisor), (float)Mathf.RoundToInt((float)((Texture)t).height / (float)divisor)))).ToList();
		int num6 = 0;
		int num7 = 0;
		int num8 = 0;
		Rect[] array = (Rect[])(object)new Rect[list.Count];
		for (int i = 0; i < list.Count; i++)
		{
			Texture2D val = list[i];
			Vector2 val2 = list2[i];
			if ((float)num6 + val2.x > (float)num2)
			{
				num6 = 0;
				num7 += num8;
				num8 = 0;
			}
			if ((float)num7 + val2.y > (float)num3)
			{
				if (divisor == 1 && !useIncreasedShortAxis && num3 < MaxAtlasSize)
				{
					return CalcRectsForAtlasNew(divisor, useIncreasedShortAxis: true);
				}
				if (divisor < 4)
				{
					Log.Warning($"StaticTextureAtlas: Texture {((Object)val).name} does not fit in the atlas of size {num2}x{num3} (trying to place at y {num7} with height of {val2.y}). Retrying with divisor of {divisor * 2}.");
					return CalcRectsForAtlasNew(divisor * 2);
				}
				return CalcRectsForAtlas();
			}
			array[i] = new Rect((float)num6, (float)num7, val2.x, val2.y);
			num6 += Mathf.CeilToInt(val2.x);
			if (val2.y > (float)num8)
			{
				num8 = Mathf.CeilToInt(val2.y);
			}
		}
		Rect[] array2 = (Rect[])(object)new Rect[list.Count];
		for (int j = 0; j < textures.Count; j++)
		{
			Texture2D val3 = textures[j];
			int num9 = list.IndexOf(val3);
			if (num9 < 0)
			{
				Log.Error("Texture " + ((Object)val3).name + " not found in height sorted list, cannot calculate UV rect.");
				continue;
			}
			Rect val4 = array[num9];
			array2[j] = new Rect(((Rect)(ref val4)).x / (float)num2, ((Rect)(ref val4)).y / (float)num3, ((Rect)(ref val4)).width / (float)num2, ((Rect)(ref val4)).height / (float)num3);
		}
		Texture2D obj = colorTexture;
		int num10 = Mathf.FloorToInt(Mathf.Log((float)num2 / 512f, 2f)) + 1;
		colorTexture = new Texture2D(num2, num3, (GraphicsFormat)4, num10, (TextureCreationFlags)5);
		Object.DestroyImmediate((Object)(object)obj);
		DeepProfiler.End();
		return array2;
	}

	private Rect[] CalcRectsForAtlas()
	{
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Expected O, but got Unknown
		DeepProfiler.Start("Create dummy textures for atlas packing");
		Texture2D[] array = ((IEnumerable<Texture2D>)textures).Select((Func<Texture2D, Texture2D>)delegate(Texture2D t)
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Expected O, but got Unknown
			int width = ((Texture)t).width;
			int height = ((Texture)t).height;
			bool flag = width % 4 == 0 && height % 4 == 0;
			return new Texture2D(width, height, (TextureFormat)((!flag) ? 1 : 10), 1, false, true)
			{
				name = ((Object)t).name + "_DummyForAtlas",
				filterMode = ((Texture)t).filterMode,
				wrapMode = ((Texture)t).wrapMode,
				anisoLevel = ((Texture)t).anisoLevel,
				mipMapBias = ((Texture)t).mipMapBias,
				minimumMipmapLevel = t.minimumMipmapLevel
			};
		}).ToArray();
		DeepProfiler.End();
		DeepProfiler.Start("PackTextures() with dummy textures");
		Rect[] result = colorTexture.PackTextures(array, 8, MaxAtlasSize, false);
		Texture2D val = colorTexture;
		Texture2D[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			Object.DestroyImmediate((Object)(object)array2[i]);
		}
		int num = Mathf.FloorToInt(Mathf.Log((float)Mathf.Max(((Texture)val).width, ((Texture)val).height) / 512f, 2f)) + 1;
		colorTexture = new Texture2D(((Texture)val).width, ((Texture)val).height, (GraphicsFormat)4, num, (TextureCreationFlags)5);
		Object.DestroyImmediate((Object)(object)val);
		DeepProfiler.End();
		return result;
	}

	private void BlitTexturesToColorAtlas(Rect[] uvRects, bool noGpuCompressionSupport)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		RenderTexture val = new RenderTexture(((Texture)colorTexture).width, ((Texture)colorTexture).height, 0, (RenderTextureFormat)0, (RenderTextureReadWrite)1)
		{
			name = "StaticTextureAtlas_Bake_Temp"
		};
		val.Create();
		RenderTexture active = RenderTexture.active;
		Graphics.SetRenderTarget(val);
		GL.Clear(true, true, Color.clear);
		GL.PushMatrix();
		GL.LoadPixelMatrix(0f, (float)((Texture)val).width, (float)((Texture)val).height, 0f);
		DeepProfiler.Start("Render real textures to RT");
		Material val2 = new Material(Shader.Find("Custom/BlitExact"));
		try
		{
			for (int i = 0; i < textures.Count; i++)
			{
				Rect val3 = uvRects[i];
				Texture2D val4 = textures[i];
				int num = Mathf.RoundToInt(((Rect)(ref val3)).x * (float)((Texture)colorTexture).width);
				int num2 = Mathf.RoundToInt((1f - ((Rect)(ref val3)).y) * (float)((Texture)colorTexture).height);
				int num3 = Mathf.RoundToInt(((Rect)(ref val3)).width * (float)((Texture)colorTexture).width);
				int num4 = Mathf.RoundToInt(((Rect)(ref val3)).height * (float)((Texture)colorTexture).height);
				num2 -= num4;
				Graphics.DrawTexture(new Rect((float)num, (float)num2, (float)num3, (float)num4), (Texture)(object)val4, new Rect(0f, 0f, 1f, 1f), 0, 0, 0, 0, val2, -1);
			}
		}
		finally
		{
			Object.Destroy((Object)(object)val2);
		}
		GL.PopMatrix();
		DeepProfiler.End();
		if (noGpuCompressionSupport)
		{
			DeepProfiler.Start("GPU Readback from RenderTexture to Texture2D");
			RenderTexture.active = val;
			colorTexture.ReadPixels(new Rect(0f, 0f, (float)((Texture)colorTexture).width, (float)((Texture)colorTexture).height), 0, 0, false);
			DeepProfiler.End();
			DeepProfiler.Start("colorTexture.Apply() to generate mipmaps");
			colorTexture.Apply(true, false);
			DeepProfiler.End();
		}
		else
		{
			DeepProfiler.Start("CopyTexture from RenderTexture to Texture2D");
			Graphics.CopyTexture((Texture)(object)val, 0, 0, 0, 0, ((Texture)colorTexture).width, ((Texture)colorTexture).height, (Texture)(object)colorTexture, 0, 0, 0, 0);
			DeepProfiler.End();
			DeepProfiler.Start("Generate mipmaps with compute shader");
			GenerateMipmapsWithCompute(colorTexture);
			DeepProfiler.End();
		}
		RenderTexture.active = active;
		val.Release();
		Object.DestroyImmediate((Object)(object)val);
		((Object)colorTexture).name = "TextureAtlas_" + groupKey.ToString() + "_" + ((Object)colorTexture).GetInstanceID();
	}

	private void BuildMaskAtlas(Rect[] uvRects, bool noGpuCompressionSupport)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Expected O, but got Unknown
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		maskTexture = new Texture2D(((Texture)colorTexture).width, ((Texture)colorTexture).height, (TextureFormat)5, false);
		RenderTexture val = new RenderTexture(((Texture)maskTexture).width, ((Texture)maskTexture).height, 0, (RenderTextureFormat)0, (RenderTextureReadWrite)1)
		{
			name = "StaticTextureAtlas_MaskBake_Temp"
		};
		val.Create();
		RenderTexture active = RenderTexture.active;
		Graphics.SetRenderTarget(val);
		GL.Clear(true, true, Color.clear);
		GL.PushMatrix();
		GL.LoadPixelMatrix(0f, (float)((Texture)val).width, (float)((Texture)val).height, 0f);
		DeepProfiler.Start("Render mask textures to RT");
		Material val2 = new Material(Shader.Find("Custom/BlitExact"));
		try
		{
			for (int i = 0; i < textures.Count; i++)
			{
				Texture2D key = textures[i];
				if (masks.TryGetValue(key, out var value))
				{
					Rect val3 = uvRects[i];
					int num = Mathf.RoundToInt(((Rect)(ref val3)).x * (float)((Texture)maskTexture).width);
					int num2 = Mathf.RoundToInt((1f - ((Rect)(ref val3)).y) * (float)((Texture)maskTexture).height);
					int num3 = Mathf.RoundToInt(((Rect)(ref val3)).width * (float)((Texture)maskTexture).width);
					int num4 = Mathf.RoundToInt(((Rect)(ref val3)).height * (float)((Texture)maskTexture).height);
					num2 -= num4;
					Graphics.DrawTexture(new Rect((float)num, (float)num2, (float)num3, (float)num4), (Texture)(object)value, new Rect(0f, 0f, 1f, 1f), 0, 0, 0, 0, val2, -1);
				}
			}
		}
		finally
		{
			Object.Destroy((Object)(object)val2);
		}
		GL.PopMatrix();
		DeepProfiler.End();
		if (noGpuCompressionSupport)
		{
			DeepProfiler.Start("GPU Readback from RenderTexture to mask Texture2D");
			RenderTexture.active = val;
			maskTexture.ReadPixels(new Rect(0f, 0f, (float)((Texture)maskTexture).width, (float)((Texture)maskTexture).height), 0, 0, false);
			DeepProfiler.End();
			DeepProfiler.Start("maskTexture.Apply()");
			maskTexture.Apply(true, false);
			DeepProfiler.End();
		}
		else
		{
			DeepProfiler.Start("CopyTexture from RenderTexture to mask Texture2D");
			Graphics.CopyTexture((Texture)(object)val, 0, 0, 0, 0, ((Texture)maskTexture).width, ((Texture)maskTexture).height, (Texture)(object)maskTexture, 0, 0, 0, 0);
			DeepProfiler.End();
		}
		RenderTexture.active = active;
		val.Release();
		Object.DestroyImmediate((Object)(object)val);
		((Object)maskTexture).name = "Mask_" + ((Object)colorTexture).name;
	}

	public bool TryGetTile(Texture texture, out StaticTextureAtlasTile tile)
	{
		return tiles.TryGetValue(texture, out tile);
	}

	private void GenerateMipmapsWithCompute(Texture2D baseTexture)
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Expected O, but got Unknown
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Expected O, but got Unknown
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Expected O, but got Unknown
		ComputeShader val = Resources.Load<ComputeShader>("Materials/Misc/Mipmapper");
		int num = val.FindKernel("GenerateMip");
		int num2 = Mathf.FloorToInt(Mathf.Log((float)Mathf.Max(((Texture)baseTexture).width, ((Texture)baseTexture).height) / 512f, 2f));
		if (num2 >= 1)
		{
			RenderTexture active = RenderTexture.active;
			RenderTexture val2 = new RenderTexture(((Texture)baseTexture).width, ((Texture)baseTexture).height, 0, (RenderTextureFormat)0);
			val2.Create();
			Material val3 = new Material(Shader.Find("Custom/BlitExact"));
			Graphics.Blit((Texture)(object)baseTexture, val2, val3);
			Object.Destroy((Object)(object)val3);
			RenderTexture[] array = (RenderTexture[])(object)new RenderTexture[num2];
			RenderTexture val4 = val2;
			for (int i = 0; i < num2; i++)
			{
				int num3 = Mathf.Max(1, ((Texture)val4).width / 2);
				int num4 = Mathf.Max(1, ((Texture)val4).height / 2);
				array[i] = new RenderTexture(num3, num4, 0, (RenderTextureFormat)0)
				{
					enableRandomWrite = true,
					name = $"GenerateMipmapsWithCompute_Mip{i + 1}"
				};
				array[i].Create();
				val.SetTexture(num, "InputTexture", (Texture)(object)val4);
				val.SetTexture(num, "OutputMip", (Texture)(object)array[i]);
				val.SetInts("InputSize", new int[2]
				{
					((Texture)val4).width,
					((Texture)val4).height
				});
				val.SetInts("OutputSize", new int[2] { num3, num4 });
				val.Dispatch(num, Mathf.CeilToInt((float)num3 / 8f), Mathf.CeilToInt((float)num4 / 8f), 1);
				val4 = array[i];
			}
			for (int j = 0; j < num2; j++)
			{
				Graphics.CopyTexture((Texture)(object)array[j], 0, 0, (Texture)(object)baseTexture, 0, j + 1);
				array[j].Release();
				Object.DestroyImmediate((Object)(object)array[j]);
			}
			RenderTexture.active = active;
			val2.Release();
			Object.DestroyImmediate((Object)(object)val2);
		}
	}

	public static Texture2D FastCompressDXT(Texture2D texture, bool deleteOriginal = false)
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Expected O, but got Unknown
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Expected O, but got Unknown
		int num = Mathf.Min(((Texture)texture).width, ((Texture)texture).height);
		if (num <= 16)
		{
			return texture;
		}
		ComputeShader val = Resources.Load<ComputeShader>("Materials/Misc/EncodeBCn");
		int num2 = val.FindKernel("EncodeBC3_AMD");
		int val2 = Mathf.FloorToInt(Mathf.Log((float)num / 16f, 2f)) + 1;
		val2 = Math.Min(val2, CalculateMaxMipmapsForDxtSupport(texture));
		val2 = Mathf.Min(val2, ((Texture)texture).mipmapCount);
		val2 = Mathf.Max(val2, 1);
		Texture2D val3 = new Texture2D(((Texture)texture).width, ((Texture)texture).height, (GraphicsFormat)101, val2, (TextureCreationFlags)1028);
		val3.Apply(false, true);
		val.SetFloat("_Quality", 0.9f);
		for (int i = 0; i < val2; i++)
		{
			int num3 = Mathf.Max(1, ((Texture)texture).width >> i);
			int num4 = Mathf.Max(1, ((Texture)texture).height >> i);
			RenderTexture val4 = new RenderTexture(num3 / 4, num4 / 4, 24)
			{
				graphicsFormat = (GraphicsFormat)44,
				enableRandomWrite = true,
				name = $"FastCompressDXT_OutputRT_Mip{i}"
			};
			val4.Create();
			val.SetTexture(num2, "_Target", (Texture)(object)val4);
			val.SetTexture(num2, "_Source", (Texture)(object)texture, i);
			val.SetInt("_mipLevel", i);
			val.Dispatch(num2, num3 / 8, num4 / 8, 1);
			Graphics.CopyTexture((Texture)(object)val4, 0, 0, 0, 0, ((Texture)val4).width, ((Texture)val4).height, (Texture)(object)val3, 0, i, 0, 0);
			val4.Release();
			Object.DestroyImmediate((Object)(object)val4);
		}
		if (deleteOriginal)
		{
			Object.DestroyImmediate((Object)(object)texture);
		}
		return val3;
	}

	public void Destroy()
	{
		Object.Destroy((Object)(object)colorTexture);
		Object.Destroy((Object)(object)maskTexture);
		foreach (KeyValuePair<Texture, StaticTextureAtlasTile> tile in tiles)
		{
			Object.Destroy((Object)(object)tile.Value.mesh);
		}
		textures.Clear();
		tiles.Clear();
	}

	public static int CalculateMaxMipmapsForDxtSupport(Texture2D tex)
	{
		int num = 0;
		int num2 = ((Texture)tex).width;
		int num3 = ((Texture)tex).height;
		while (num2 >= 4 && num3 >= 4 && num2 % 4 == 0 && num3 % 4 == 0)
		{
			num++;
			num2 >>= 1;
			num3 >>= 1;
		}
		if (num == 0)
		{
			num = 1;
		}
		return num;
	}
}
