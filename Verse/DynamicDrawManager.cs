using System;
using System.Collections.Generic;
using Gilzoide.ManagedJobs;
using LudeonTK;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Verse;

public sealed class DynamicDrawManager
{
	private struct ThingCullDetails
	{
		public IntVec3 cell;

		public CellRect coarseBounds;

		public bool seeThroughFog;

		public float hideAtSnowOrSandDepth;

		public bool drawOffscreen;

		public Vector3 pos;

		public Vector2 drawSize;

		public bool drawSilhouette;

		public bool hasSunShadows;

		public Matrix4x4 trs;

		public bool shouldDraw;

		public bool shouldDrawShadow;
	}

	[BurstCompile]
	private struct CullJob : IJobParallelFor
	{
		[ReadOnly]
		public CellRect viewRect;

		[ReadOnly]
		public CellRect shadowViewRect;

		[ReadOnly]
		public CellIndices indices;

		[ReadOnly]
		public bool checkShadows;

		[ReadOnly]
		public NativeBitArray fogGrid;

		[ReadOnly]
		public NativeArray<float> depthGrid;

		public NativeArray<ThingCullDetails> details;

		[BurstCompile]
		public void Execute(int index)
		{
			ThingCullDetails thingCullDetails = details[index];
			int num = indices[thingCullDetails.cell];
			if (!indices.Contains(num) || (!thingCullDetails.seeThroughFog && ((NativeBitArray)(ref fogGrid)).IsSet(num)) || (thingCullDetails.hideAtSnowOrSandDepth < 1f && depthGrid[num] > thingCullDetails.hideAtSnowOrSandDepth))
			{
				return;
			}
			if (!thingCullDetails.drawOffscreen && !viewRect.Overlaps(thingCullDetails.coarseBounds))
			{
				if (checkShadows && thingCullDetails.hasSunShadows)
				{
					thingCullDetails.shouldDrawShadow = shadowViewRect.Contains(thingCullDetails.cell);
				}
			}
			else
			{
				thingCullDetails.shouldDraw = true;
				details[index] = thingCullDetails;
			}
		}
	}

	[BurstCompile]
	private struct ComputeSilhouetteMatricesJob : IJobParallelFor
	{
		public Vector3 inverseFovScale;

		public float altitude;

		public NativeArray<ThingCullDetails> details;

		[BurstCompile]
		public void Execute(int index)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			ThingCullDetails thingCullDetails = details[index];
			if (thingCullDetails.drawSilhouette)
			{
				Vector3 val = default(Vector3);
				((Vector3)(ref val))._002Ector(thingCullDetails.drawSize.x, 0f, thingCullDetails.drawSize.y);
				Vector3 val2 = inverseFovScale;
				if (val.x < 2.5f)
				{
					val2.x *= val.x + SilhouetteUtility.AdjustScale(val.x);
				}
				else
				{
					val2.x *= val.x;
				}
				if (val.z < 2.5f)
				{
					val2.z *= val.z + SilhouetteUtility.AdjustScale(val.z);
				}
				else
				{
					val2.z *= val.z;
				}
				Vector3 pos = thingCullDetails.pos;
				pos.y = altitude;
				thingCullDetails.trs = Matrix4x4.TRS(pos, Quaternion.AngleAxis(0f, Vector3.up), val2);
				details[index] = thingCullDetails;
			}
		}
	}

	private class PreDrawThings : IJobParallelFor
	{
		public NativeArray<ThingCullDetails> details;

		public List<Thing> things;

		public void Execute(int index)
		{
			Thing thing = things[index];
			if (details[index].shouldDraw)
			{
				thing.DynamicDrawPhase(DrawPhase.ParallelPreDraw);
			}
		}
	}

	private Map map;

	private readonly List<Thing> drawThings = new List<Thing>();

	private bool drawingNow;

	public IReadOnlyList<Thing> DrawThings => drawThings;

	public DynamicDrawManager(Map map)
	{
		this.map = map;
	}

	public void RegisterDrawable(Thing t)
	{
		if (t.def.drawerType != 0)
		{
			if (drawingNow)
			{
				Log.Warning($"Cannot register drawable {t} while drawing is in progress. Things shouldn't be spawned in Draw methods.");
			}
			drawThings.Add(t);
		}
	}

	public void DeRegisterDrawable(Thing t)
	{
		if (t.def.drawerType != 0)
		{
			if (drawingNow)
			{
				Log.Warning($"Cannot deregister drawable {t} while drawing is in progress. Things shouldn't be despawned in Draw methods.");
			}
			drawThings.Remove(t);
		}
	}

	public void DrawDynamicThings()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		if (!DebugViewSettings.drawThingsDynamic || map.Disposed)
		{
			return;
		}
		drawingNow = true;
		bool flag = SilhouetteUtility.CanHighlightAny();
		NativeArray<ThingCullDetails> details = default(NativeArray<ThingCullDetails>);
		details._002Ector(drawThings.Count, (Allocator)3, (NativeArrayOptions)1);
		ComputeCulledThings(details);
		if (!DebugViewSettings.singleThreadedDrawing)
		{
			using (new ProfilerBlock("Ensure Graphics Initialized"))
			{
				for (int i = 0; i < details.Length; i++)
				{
					if (details[i].shouldDraw)
					{
						drawThings[i].DynamicDrawPhase(DrawPhase.EnsureInitialized);
					}
				}
			}
			PreDrawVisibleThings(details);
		}
		try
		{
			using (new ProfilerBlock("Draw Visible"))
			{
				for (int j = 0; j < details.Length; j++)
				{
					if (!details[j].shouldDraw && !details[j].shouldDrawShadow)
					{
						continue;
					}
					try
					{
						if (details[j].shouldDraw)
						{
							drawThings[j].DynamicDrawPhase(DrawPhase.Draw);
						}
						else if (drawThings[j] is Pawn pawn)
						{
							pawn.DrawShadowAt(pawn.DrawPos);
						}
					}
					catch (Exception arg)
					{
						Log.Error($"Exception drawing {drawThings[j]}: {arg}");
					}
				}
			}
			if (flag)
			{
				DrawSilhouettes(details);
			}
		}
		catch (Exception arg2)
		{
			Log.Error($"Exception drawing dynamic things: {arg2}");
		}
		finally
		{
			details.Dispose();
		}
		drawingNow = false;
	}

	private void PreDrawVisibleThings(NativeArray<ThingCullDetails> details)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		using (new ProfilerBlock("Pre draw job"))
		{
			ManagedJobParallelFor managedJobParallelFor = new ManagedJobParallelFor((IJobParallelFor)(object)new PreDrawThings
			{
				details = details,
				things = drawThings
			});
			int length = details.Length;
			int idealBatchCount = UnityData.GetIdealBatchCount(details.Length);
			JobHandle dependsOn = default(JobHandle);
			dependsOn = managedJobParallelFor.Schedule(length, idealBatchCount, dependsOn);
			((JobHandle)(ref dependsOn)).Complete();
		}
	}

	private void ComputeCulledThings(NativeArray<ThingCullDetails> details)
	{
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		CellRect cellRect = Find.CameraDriver.CurrentViewRect.ExpandedBy(1);
		if (WorldComponent_GravshipController.GravshipRenderInProgess)
		{
			cellRect = cellRect.Encapsulate(WorldComponent_GravshipController.GravshipRenderBounds);
		}
		cellRect.ClipInsideMap(map);
		using (new ProfilerBlock("Prepare cull job"))
		{
			for (int i = 0; i < drawThings.Count; i++)
			{
				Thing thing = drawThings[i];
				ThingCullDetails thingCullDetails = default(ThingCullDetails);
				thingCullDetails.cell = ((thing is Pawn) ? thing.DrawPos.ToIntVec3() : thing.Position);
				thingCullDetails.coarseBounds = thing.OccupiedDrawRect();
				thingCullDetails.hideAtSnowOrSandDepth = thing.def.hideAtSnowOrSandDepth;
				thingCullDetails.seeThroughFog = thing.def.seeThroughFog;
				thingCullDetails.hasSunShadows = thing.def.HasSunShadows;
				thingCullDetails.drawOffscreen = thing.def.drawOffscreen;
				ThingCullDetails thingCullDetails2 = thingCullDetails;
				details[i] = thingCullDetails2;
			}
		}
		using (new ProfilerBlock("Cull job"))
		{
			CullJob cullJob = default(CullJob);
			cullJob.indices = map.cellIndices;
			cullJob.viewRect = cellRect;
			cullJob.fogGrid = map.fogGrid.FogGrid_Unsafe;
			cullJob.depthGrid = map.snowGrid.DepthGrid_Unsafe;
			cullJob.details = details;
			cullJob.checkShadows = MatBases.SunShadow.shader.isSupported;
			cullJob.shadowViewRect = SectionLayer_SunShadows.GetSunShadowsViewRect(map, cellRect);
			CullJob cullJob2 = cullJob;
			int length = details.Length;
			int idealBatchCount = UnityData.GetIdealBatchCount(details.Length);
			JobHandle val = default(JobHandle);
			val = IJobParallelForExtensions.Schedule<CullJob>(cullJob2, length, idealBatchCount, val);
			((JobHandle)(ref val)).Complete();
		}
	}

	private void DrawSilhouettes(NativeArray<ThingCullDetails> details)
	{
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		using (new ProfilerBlock("Prepare matrices job"))
		{
			for (int i = 0; i < details.Length; i++)
			{
				if (details[i].shouldDraw)
				{
					Thing thing = drawThings[i];
					if (SilhouetteUtility.ShouldDrawSilhouette(thing) && thing is Pawn pawn)
					{
						ThingCullDetails thingCullDetails = details[i];
						thingCullDetails.pos = pawn.Drawer.renderer.SilhouettePos;
						thingCullDetails.drawSize = pawn.Drawer.renderer.SilhouetteGraphic.drawSize;
						thingCullDetails.drawSilhouette = true;
						details[i] = thingCullDetails;
					}
				}
			}
		}
		using (new ProfilerBlock("Compute matrices"))
		{
			ComputeSilhouetteMatricesJob computeSilhouetteMatricesJob = default(ComputeSilhouetteMatricesJob);
			computeSilhouetteMatricesJob.inverseFovScale = Find.CameraDriver.InverseFovScale;
			computeSilhouetteMatricesJob.altitude = AltitudeLayer.Silhouettes.AltitudeFor();
			computeSilhouetteMatricesJob.details = details;
			ComputeSilhouetteMatricesJob computeSilhouetteMatricesJob2 = computeSilhouetteMatricesJob;
			int length = details.Length;
			int idealBatchCount = UnityData.GetIdealBatchCount(details.Length);
			JobHandle val = default(JobHandle);
			val = IJobParallelForExtensions.Schedule<ComputeSilhouetteMatricesJob>(computeSilhouetteMatricesJob2, length, idealBatchCount, val);
			((JobHandle)(ref val)).Complete();
		}
		using (new ProfilerBlock("Draw silhouettes"))
		{
			for (int j = 0; j < details.Length; j++)
			{
				if (details[j].drawSilhouette && drawThings[j] is Pawn thing2)
				{
					SilhouetteUtility.DrawSilhouetteJob(thing2, details[j].trs);
				}
			}
		}
	}

	public void LogDynamicDrawThings()
	{
		Log.Message(DebugLogsUtility.ThingListToUniqueCountString(drawThings));
	}
}
