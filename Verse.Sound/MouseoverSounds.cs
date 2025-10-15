using System.Collections.Generic;
using RimWorld;
using UnityEngine;

namespace Verse.Sound;

public static class MouseoverSounds
{
	private struct MouseoverRegionCall
	{
		public bool mouseIsOver;

		public Rect rect;

		public SoundDef sound;

		public bool IsValid => ((Rect)(ref rect)).x >= 0f;

		public static MouseoverRegionCall Invalid
		{
			get
			{
				//IL_001e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0023: Unknown result type (might be due to invalid IL or missing references)
				MouseoverRegionCall result = default(MouseoverRegionCall);
				result.rect = new Rect(-1000f, -1000f, 0f, 0f);
				return result;
			}
		}

		public bool Matches(MouseoverRegionCall other)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			return ((Rect)(ref rect)).Equals(other.rect);
		}

		public override string ToString()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			if (!IsValid)
			{
				return "(Invalid)";
			}
			Rect val = rect;
			return "(rect=" + ((object)(Rect)(ref val)).ToString() + (mouseIsOver ? "mouseIsOver" : "") + ")";
		}
	}

	private static List<MouseoverRegionCall> frameCalls = new List<MouseoverRegionCall>();

	private static int lastUsedCallInd = -1;

	private static MouseoverRegionCall lastUsedCall;

	private static int forceSilenceUntilFrame = -1;

	public static void SilenceForNextFrame()
	{
		forceSilenceUntilFrame = Time.frameCount + 1;
	}

	public static void DoRegion(Rect rect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		DoRegion(rect, SoundDefOf.Mouseover_Standard);
	}

	public static void DoRegion(Rect rect, SoundDef sound)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Invalid comparison between Unknown and I4
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		if (sound != null && (int)Event.current.type == 7)
		{
			Rect rect2 = default(Rect);
			((Rect)(ref rect2))._002Ector(GUIUtility.GUIToScreenPoint(((Rect)(ref rect)).position), ((Rect)(ref rect)).size);
			MouseoverRegionCall item = default(MouseoverRegionCall);
			item.rect = rect2;
			item.sound = sound;
			item.mouseIsOver = Mouse.IsOver(rect);
			frameCalls.Add(item);
		}
	}

	public static void ResolveFrame()
	{
		for (int i = 0; i < frameCalls.Count; i++)
		{
			if (frameCalls[i].mouseIsOver)
			{
				if (lastUsedCallInd != i && !frameCalls[i].Matches(lastUsedCall) && forceSilenceUntilFrame < Time.frameCount)
				{
					frameCalls[i].sound.PlayOneShotOnCamera();
				}
				lastUsedCallInd = i;
				lastUsedCall = frameCalls[i];
				frameCalls.Clear();
				return;
			}
		}
		lastUsedCall = MouseoverRegionCall.Invalid;
		lastUsedCallInd = -1;
		frameCalls.Clear();
	}
}
