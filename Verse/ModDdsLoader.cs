using System;
using System.Buffers;
using System.IO;
using RimWorld.IO;
using UnityEngine;

namespace Verse;

public static class ModDdsLoader
{
	public static Texture2D TryLoadDds(VirtualFile file)
	{
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Invalid comparison between Unknown and I4
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		using MemoryMappedFileSpanWrapper memoryMappedFileSpanWrapper = new MemoryMappedFileSpanWrapper(new FileInfo(((file as FilesystemFile) ?? throw new NotSupportedException("ModDdsLoader only supports FilesystemFile types.")).FullPath), suppressExistsCheck: true);
		DdsHeader header = memoryMappedFileSpanWrapper.Read<DdsHeader>(0L);
		if (header.Magic != 542327876)
		{
			throw new InvalidDataException($"Invalid DDS magic number: {header.Magic:X8}. Expected: {542327876u:X8}");
		}
		if (header.Size != 124)
		{
			throw new InvalidDataException($"Invalid DDS header size: {header.Size}. Expected: {124u}");
		}
		if (header.PixelFormat.Size != 32)
		{
			throw new InvalidDataException($"Invalid DDS pixel format size: {header.PixelFormat.Size}. Expected: {32u}");
		}
		TextureFormat val = header.PixelFormat.ToTextureFormat();
		int num = (((int)val == 25) ? 148 : 128);
		if (header.PixelFormat.IsBgr888 && !header.PixelFormat.IsCompressed)
		{
			Span<byte> span = memoryMappedFileSpanWrapper.GetSpan(num);
			byte[] array = ArrayPool<byte>.Shared.Rent(span.Length);
			Span<byte> val2 = default(Span<byte>);
			val2._002Ector(array, 0, span.Length);
			span.CopyTo(val2);
			int num2 = (int)header.PixelFormat.RGBBitCount / 8;
			for (int i = 0; i < span.Length; i += num2)
			{
				ref byte reference = ref val2[i];
				ref byte reference2 = ref val2[i + 2];
				byte b = val2[i + 2];
				byte b2 = val2[i];
				reference = b;
				reference2 = b2;
			}
			Texture2D result = CreateTexture(file, header, val, val2);
			ArrayPool<byte>.Shared.Return(array, false);
			return result;
		}
		return CreateTexture(file, header, val, memoryMappedFileSpanWrapper.GetSpan(num));
	}

	private unsafe static Texture2D CreateTexture(VirtualFile file, DdsHeader header, TextureFormat format, Span<byte> data)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Expected O, but got Unknown
		bool flag = (header.Flags & DdsHeaderFlags.MipMapCount) != 0 && header.MipMapCount > 1;
		int num = (int)((!flag) ? 1 : header.MipMapCount);
		Texture2D val = new Texture2D((int)header.Width, (int)header.Height, format, num, false, true);
		fixed (byte* ptr = data)
		{
			val.LoadRawTextureData((IntPtr)ptr, data.Length);
		}
		((Object)val).name = Path.GetFileNameWithoutExtension(file.Name);
		((Texture)val).filterMode = (FilterMode)2;
		((Texture)val).anisoLevel = 0;
		val.Apply(!flag, true);
		return val;
	}
}
