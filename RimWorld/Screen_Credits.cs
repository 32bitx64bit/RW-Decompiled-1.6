using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RimWorld;

public class Screen_Credits : Window
{
	private List<CreditsEntry> creds;

	public bool wonGame;

	public SongDef endCreditsSong;

	public bool exitToMainMenu;

	public float songStartDelay = 5f;

	private float timeUntilAutoScroll;

	private float scrollPosition;

	private bool playedMusic;

	private float victoryTextHeight;

	private float closeDelay = -1f;

	public static bool creditsShowing;

	public static float creationRealtime = -1f;

	private const int ColumnWidth = 800;

	private const float InitialAutoScrollDelay = 1f;

	private const float InitialAutoScrollDelayWonGame = 10f;

	private const float AutoScrollDelayAfterManualScroll = 3f;

	private const float VictoryTextScrollSpeed = 20f;

	private const float ScrollSpeedLerpHeight = 200f;

	private const GameFont Font = GameFont.Medium;

	public const float DefaultSongStartDelay = 5f;

	private const int CloseTransitionSeconds = 3;

	public override Vector2 InitialSize => new Vector2((float)UI.screenWidth, (float)UI.screenHeight);

	protected override float Margin => 0f;

	private float ViewWidth => 800f;

	private float ViewHeight
	{
		get
		{
			GameFont font = Text.Font;
			Text.Font = GameFont.Medium;
			float result = creds.Sum((CreditsEntry c) => c.DrawHeight(ViewWidth)) + 20f;
			Text.Font = font;
			return result;
		}
	}

	private float MaxScrollPosition => Mathf.Max(ViewHeight - (float)UI.screenHeight / 2f, 0f);

	private float AutoScrollRate
	{
		get
		{
			if (wonGame)
			{
				if (scrollPosition < victoryTextHeight - 200f)
				{
					return 20f;
				}
				float num = 130f;
				if (EndCreditsSong != null)
				{
					num = EndCreditsSong.clip.length + songStartDelay - 10f - victoryTextHeight / 20f;
				}
				float num2 = (scrollPosition - victoryTextHeight) / 200f;
				return Mathf.Lerp(20f, MaxScrollPosition / num, num2);
			}
			return 30f;
		}
	}

	private SongDef EndCreditsSong => endCreditsSong;

	public Screen_Credits()
		: this("")
	{
	}

	public Screen_Credits(string preCreditsMessage, int preCreditsSpace = 50)
	{
		doWindowBackground = false;
		doCloseButton = false;
		doCloseX = false;
		forcePause = true;
		closeOnCancel = false;
		creds = CreditsAssembler.AllCredits().ToList();
		creds.Insert(0, new CreditRecord_Space(100f));
		if (!preCreditsMessage.NullOrEmpty())
		{
			creds.Insert(1, new CreditRecord_Space(200f));
			creds.Insert(2, new CreditRecord_Text(preCreditsMessage, (TextAnchor)0));
			creds.Insert(3, new CreditRecord_Space(preCreditsSpace));
			Text.Font = GameFont.Medium;
			victoryTextHeight = creds.Take(4).Sum((CreditsEntry c) => c.DrawHeight(ViewWidth));
		}
		creds.Add(new CreditRecord_Space(300f));
		creds.Add(new CreditRecord_Text("ThanksForPlaying".Translate(), (TextAnchor)1));
		string text = string.Empty;
		foreach (CreditsEntry cred in creds)
		{
			if (!(cred is CreditRecord_Role creditRecord_Role))
			{
				text = string.Empty;
				continue;
			}
			creditRecord_Role.displayKey = text.NullOrEmpty() || creditRecord_Role.roleKey != text;
			text = creditRecord_Role.roleKey;
		}
	}

	public override void PreOpen()
	{
		base.PreOpen();
		creationRealtime = Time.realtimeSinceStartup;
		creditsShowing = true;
		if (wonGame)
		{
			timeUntilAutoScroll = 10f;
		}
		else
		{
			timeUntilAutoScroll = 1f;
		}
	}

	public override void PostClose()
	{
		base.PostOpen();
		creditsShowing = false;
		if (exitToMainMenu)
		{
			GenScene.GoToMainMenu();
		}
	}

	public override void WindowUpdate()
	{
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		base.WindowUpdate();
		if (timeUntilAutoScroll > 0f)
		{
			timeUntilAutoScroll -= Time.deltaTime;
		}
		else
		{
			scrollPosition += AutoScrollRate * Time.deltaTime;
		}
		if (wonGame && EndCreditsSong != null && !playedMusic && Time.realtimeSinceStartup > creationRealtime + songStartDelay)
		{
			Find.MusicManagerPlay.ForcePlaySong(EndCreditsSong, ignorePrefsVolume: false);
			playedMusic = true;
		}
		if (!(closeDelay > 0f))
		{
			return;
		}
		closeDelay -= Time.deltaTime;
		if (closeDelay <= 0f)
		{
			ScreenFader.StartFade(Color.clear, 3f);
			Close();
			if (Current.ProgramState == ProgramState.Playing)
			{
				Find.TickManager.CurTimeSpeed = TimeSpeed.Normal;
			}
		}
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Invalid comparison between Unknown and I4
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Invalid comparison between Unknown and I4
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Invalid comparison between Unknown and I4
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Invalid comparison between Unknown and I4
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(0f, 0f, (float)UI.screenWidth, (float)UI.screenHeight);
		GUI.DrawTexture(val, (Texture)(object)BaseContent.BlackTex);
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(val);
		((Rect)(ref rect)).yMin = ((Rect)(ref rect)).yMin + 30f;
		((Rect)(ref rect)).yMax = ((Rect)(ref rect)).yMax - 30f;
		((Rect)(ref rect)).xMin = ((Rect)(ref val)).center.x - 400f;
		((Rect)(ref rect)).width = 800f;
		float viewWidth = ViewWidth;
		float viewHeight = ViewHeight;
		scrollPosition = Mathf.Clamp(scrollPosition, 0f, MaxScrollPosition);
		Widgets.BeginGroup(rect);
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(0f, 0f, viewWidth, viewHeight);
		((Rect)(ref rect2)).y = ((Rect)(ref rect2)).y - scrollPosition;
		Widgets.BeginGroup(rect2);
		Text.Font = GameFont.Medium;
		float num = 0f;
		Rect rect3 = default(Rect);
		foreach (CreditsEntry cred in creds)
		{
			float num2 = cred.DrawHeight(((Rect)(ref rect2)).width);
			((Rect)(ref rect3))._002Ector(0f, num, ((Rect)(ref rect2)).width, num2);
			cred.Draw(rect3);
			num += num2;
		}
		Widgets.EndGroup();
		Widgets.EndGroup();
		if (closeDelay < 0f && scrollPosition > 0f && Widgets.ButtonText(new Rect(((Rect)(ref val)).xMax - 200f, ((Rect)(ref val)).yMax - 100f, 150f, 50f), "SkipCredits".Translate()))
		{
			OnCancelKeyPressed();
		}
		if ((int)Event.current.type == 6)
		{
			Scroll(Event.current.delta.y * 25f);
			Event.current.Use();
		}
		if ((int)Event.current.type == 4)
		{
			if ((int)Event.current.keyCode == 274)
			{
				Scroll(250f);
				Event.current.Use();
			}
			if ((int)Event.current.keyCode == 273)
			{
				Scroll(-250f);
				Event.current.Use();
			}
		}
	}

	private void Scroll(float offset)
	{
		scrollPosition += offset;
		timeUntilAutoScroll = 3f;
	}

	public override void OnCancelKeyPressed()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		Event.current.Use();
		if (!(closeDelay > 0f))
		{
			closeDelay = 3f;
			ScreenFader.StartFade(Color.black, 3f);
		}
	}
}
