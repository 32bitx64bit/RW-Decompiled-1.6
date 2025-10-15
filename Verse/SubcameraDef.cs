using UnityEngine;

namespace Verse;

public class SubcameraDef : Def
{
	[NoTranslate]
	public string layer;

	public int depth;

	public RenderTextureFormat format;

	public bool startEnabled = true;

	public bool doNotUpdate;

	public Color backgroundColor = new Color(0f, 0f, 0f, 0f);

	[Unsaved(false)]
	private int layerCached = -1;

	public int LayerId
	{
		get
		{
			if (layerCached == -1)
			{
				layerCached = LayerMask.NameToLayer(layer);
			}
			return layerCached;
		}
	}

	public RenderTextureFormat BestFormat
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Invalid comparison between Unknown and I4
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Invalid comparison between Unknown and I4
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Invalid comparison between Unknown and I4
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Invalid comparison between Unknown and I4
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Invalid comparison between Unknown and I4
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Invalid comparison between Unknown and I4
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Invalid comparison between Unknown and I4
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Invalid comparison between Unknown and I4
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Invalid comparison between Unknown and I4
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Invalid comparison between Unknown and I4
			if (SystemInfo.SupportsRenderTextureFormat(format))
			{
				return format;
			}
			if ((int)format == 16 && SystemInfo.SupportsRenderTextureFormat((RenderTextureFormat)25))
			{
				return (RenderTextureFormat)25;
			}
			if (((int)format == 16 || (int)format == 25) && SystemInfo.SupportsRenderTextureFormat((RenderTextureFormat)0))
			{
				return (RenderTextureFormat)0;
			}
			if (((int)format == 16 || (int)format == 15 || (int)format == 14) && SystemInfo.SupportsRenderTextureFormat((RenderTextureFormat)12))
			{
				return (RenderTextureFormat)12;
			}
			if (((int)format == 16 || (int)format == 15 || (int)format == 14 || (int)format == 12) && SystemInfo.SupportsRenderTextureFormat((RenderTextureFormat)11))
			{
				return (RenderTextureFormat)11;
			}
			return format;
		}
	}
}
