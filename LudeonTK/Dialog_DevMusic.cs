using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace LudeonTK;

[StaticConstructorOnStartup]
public class Dialog_DevMusic : Window_Dev
{
	private Vector2 windowPosition;

	private const string Title = "Music Debugger";

	private const float ButtonHeight = 30f;

	public override bool IsDebug => true;

	protected override float Margin => 4f;

	public override Vector2 InitialSize => new Vector2(275f, 360f);

	public Dialog_DevMusic()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		draggable = true;
		focusWhenOpened = false;
		drawShadow = false;
		closeOnAccept = false;
		closeOnCancel = false;
		preventCameraMotion = false;
		drawInScreenshotMode = false;
		windowPosition = Prefs.DevPalettePosition;
		onlyDrawInDevMode = true;
		doCloseX = true;
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_040b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_050d: Unknown result type (might be due to invalid IL or missing references)
		//IL_044f: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Small;
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(((Rect)(ref inRect)).x, ((Rect)(ref inRect)).y, ((Rect)(ref inRect)).width, 24f);
		DevGUI.Label(rect, "Music Debugger");
		float y = ((Rect)(ref rect)).height + 6f;
		Text.Font = GameFont.Tiny;
		MusicManagerPlay musicManagerPlay = Find.MusicManagerPlay;
		PrintLabel($"State: {musicManagerPlay.State}", inRect, ref y);
		PrintLabel("", inRect, ref y);
		PrintLabel("Song: " + (musicManagerPlay.IsPlaying ? $"{musicManagerPlay.CurrentSong.defName} ({musicManagerPlay.SongTime:0}/{musicManagerPlay.SongDuration:0}s)" : "NA"), inRect, ref y);
		if (musicManagerPlay.State == MusicManagerPlay.MusicManagerState.Fadeout)
		{
			PrintLabel(string.Format("Fadeout Progress: {0} ({1:0.0}s)", musicManagerPlay.FadeoutPercent.ToStringPercent("0"), musicManagerPlay.FadeoutDuration), inRect, ref y);
		}
		else if (musicManagerPlay.IsPlaying)
		{
			PrintLabel("Volume: " + musicManagerPlay.CurSanitizedVolume.ToStringPercent("0"), inRect, ref y);
		}
		else
		{
			PrintLabel("Next Song: " + ((!musicManagerPlay.IsPlaying && musicManagerPlay.NextSongTimer > 0f) ? $"{musicManagerPlay.NextSongTimer:0.0}s" : "NA"), inRect, ref y);
		}
		PrintLabel("Pause Vol Factor: " + musicManagerPlay.PausedVolumeFactor.ToStringPercent("0"), inRect, ref y);
		Rect rect2 = inRect;
		((Rect)(ref rect2)).y = y;
		((Rect)(ref rect2)).height = 18f;
		((Rect)(ref rect2)).xMin = ((Rect)(ref rect2)).width * 0.65f;
		((Rect)(ref rect2)).xMax = ((Rect)(ref rect2)).xMax - 2f;
		PrintLabel($"Danger Mode: {musicManagerPlay.DangerMusicMode}", inRect, ref y);
		if (DevGUI.ButtonText(rect2, musicManagerPlay.OverrideDangerMode ? "Override On" : "Override Off"))
		{
			musicManagerPlay.OverrideDangerMode = !musicManagerPlay.OverrideDangerMode;
		}
		PrintLabel("", inRect, ref y);
		PrintLabel("Source Transition: " + ((musicManagerPlay.TriggeredTransition != null) ? musicManagerPlay.TriggeredTransition.def.defName : "NA"), inRect, ref y);
		PrintLabel("Current Sequence: " + ((musicManagerPlay.MusicSequenceWorker != null) ? musicManagerPlay.MusicSequenceWorker.def.defName : "NA"), inRect, ref y);
		MusicSequenceWorker musicSequenceWorker = musicManagerPlay.MusicSequenceWorker;
		if (musicSequenceWorker != null)
		{
			PrintLabel("   - Next Sequence: " + ((musicSequenceWorker.def.nextSequence != null) ? musicSequenceWorker.def.nextSequence.defName : "NA"), inRect, ref y);
			PrintLabel($"   - Loop: {musicSequenceWorker.ShouldLoop()}", inRect, ref y);
			PrintLabel($"   - Can Transition: {musicSequenceWorker.ShouldTransition()}", inRect, ref y);
			PrintLabel($"   - Interruptible: {musicSequenceWorker.CanBeInterrupted()}", inRect, ref y);
			PrintLabel($"   - Times Looped: {musicSequenceWorker.timesLooped}", inRect, ref y);
		}
		Rect rect3 = inRect;
		((Rect)(ref rect3)).yMin = ((Rect)(ref rect3)).yMax - 30f - 2f;
		((Rect)(ref rect3)).yMax = ((Rect)(ref rect3)).yMax - 2f;
		((Rect)(ref rect3)).xMin = 2f;
		((Rect)(ref rect3)).xMax = ((Rect)(ref inRect)).width / 2f - 2f;
		Rect rect4 = inRect;
		((Rect)(ref rect4)).yMin = ((Rect)(ref rect4)).yMax - 30f - 2f;
		((Rect)(ref rect4)).yMax = ((Rect)(ref rect4)).yMax - 2f;
		((Rect)(ref rect4)).xMin = ((Rect)(ref inRect)).width / 2f + 2f;
		((Rect)(ref rect4)).xMax = ((Rect)(ref rect4)).xMax - 2f;
		if (DevGUI.ButtonText(rect3, musicManagerPlay.IsPlaying ? "Stop" : "Start", (TextAnchor)4))
		{
			if (musicManagerPlay.IsPlaying)
			{
				musicManagerPlay.Stop();
			}
			else
			{
				musicManagerPlay.ScheduleNewSong();
			}
		}
		if (musicManagerPlay.MusicSequenceWorker == null)
		{
			if (!DevGUI.ButtonText(rect4, "Trigger Transition", (TextAnchor)4))
			{
				return;
			}
			List<FloatMenuOption> list = new List<FloatMenuOption>();
			foreach (MusicTransitionDef allDef in DefDatabase<MusicTransitionDef>.AllDefs)
			{
				MusicTransitionDef local = allDef;
				list.Add(new FloatMenuOption(local.defName, delegate
				{
					Find.MusicManagerPlay.ForceTriggerTransition(local);
				}));
			}
			FloatMenu window = new FloatMenu(list, "Select transition");
			Find.WindowStack.Add(window);
		}
		else
		{
			bool flag = musicManagerPlay.MusicSequenceWorker.def.nextSequence != null;
			if (DevGUI.ButtonText(rect4, flag ? "Next Sequence" : "Next Song", (TextAnchor)4))
			{
				musicManagerPlay.ForceTriggerNextSongOrSequence();
			}
		}
	}

	private void PrintLabel(string text, Rect container, ref float y)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		DevGUI.Label(new Rect(((Rect)(ref container)).x, y, ((Rect)(ref container)).width, 20f), text);
		y += 20f;
	}
}
