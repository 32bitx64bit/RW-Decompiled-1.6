using System;
using UnityEngine;

namespace Verse;

public static class SimpleColorExtension
{
	public static Color ToUnityColor(this SimpleColor color)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		return (Color)(color switch
		{
			SimpleColor.White => Color.white, 
			SimpleColor.Red => Color.red, 
			SimpleColor.Green => Color.green, 
			SimpleColor.Blue => Color.blue, 
			SimpleColor.Magenta => Color.magenta, 
			SimpleColor.Yellow => Color.yellow, 
			SimpleColor.Cyan => Color.cyan, 
			SimpleColor.Orange => ColorLibrary.Orange, 
			_ => throw new ArgumentException(), 
		});
	}
}
