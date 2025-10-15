using System;
using System.Collections;
using UnityEngine;
using Verse;

public static class Delay
{
	private class CoroutineRunner : MonoBehaviour
	{
	}

	private static CoroutineRunner coroutineRunner;

	private static Coroutine StartCoroutine(IEnumerator coroutine)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)coroutineRunner))
		{
			coroutineRunner = new GameObject("CoroutineRunner").AddComponent<CoroutineRunner>();
		}
		return ((MonoBehaviour)coroutineRunner).StartCoroutine(coroutine);
	}

	public static void AfterNSeconds(float seconds, Action action)
	{
		StartCoroutine(Internal_AfterNSeconds(seconds, action));
	}

	private static IEnumerator Internal_AfterNSeconds(float seconds, Action action)
	{
		yield return (object)new WaitForSeconds(seconds);
		action?.Invoke();
	}

	public static void AfterNTicks(int ticks, Action action)
	{
		StartCoroutine(Internal_AfterNTicks(GenTicks.TicksGame + ticks, action));
	}

	private static IEnumerator Internal_AfterNTicks(int targetTick, Action action)
	{
		yield return (object)new WaitUntil((Func<bool>)(() => GenTicks.TicksGame >= targetTick));
		action?.Invoke();
	}
}
