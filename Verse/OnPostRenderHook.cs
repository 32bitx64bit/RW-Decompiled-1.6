using System;
using System.Collections.Generic;
using UnityEngine;

namespace Verse;

public static class OnPostRenderHook
{
	private struct Callbacks
	{
		public bool preRenderCalled;

		public Action postRender;
	}

	private static Dictionary<Camera, Callbacks> hooks;

	static OnPostRenderHook()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected O, but got Unknown
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Expected O, but got Unknown
		hooks = new Dictionary<Camera, Callbacks>();
		Camera.onPreRender = (CameraCallback)Delegate.Combine((Delegate?)(object)Camera.onPreRender, (Delegate?)new CameraCallback(OnPreRender));
		Camera.onPostRender = (CameraCallback)Delegate.Combine((Delegate?)(object)Camera.onPostRender, (Delegate?)new CameraCallback(OnPostRender));
	}

	public static void HookOnce(Camera camera, Action postRender)
	{
		hooks.Add(camera, new Callbacks
		{
			postRender = postRender,
			preRenderCalled = false
		});
	}

	private static void OnPreRender(Camera camera)
	{
		if (hooks.TryGetValue(camera, out var value))
		{
			hooks[camera] = new Callbacks
			{
				postRender = value.postRender,
				preRenderCalled = true
			};
		}
	}

	private static void OnPostRender(Camera camera)
	{
		if (hooks.TryGetValue(camera, out var value) && value.preRenderCalled)
		{
			hooks.Remove(camera);
			value.postRender();
		}
	}
}
