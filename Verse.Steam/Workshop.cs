using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LudeonTK;
using RimWorld;
using Steamworks;

namespace Verse.Steam;

public static class Workshop
{
	private static WorkshopItemHook uploadingHook;

	private static UGCUpdateHandle_t curUpdateHandle;

	private static WorkshopInteractStage curStage = WorkshopInteractStage.None;

	private static Callback<RemoteStoragePublishedFileSubscribed_t> subscribedCallback;

	private static Callback<RemoteStoragePublishedFileUnsubscribed_t> unsubscribedCallback;

	private static Callback<ItemInstalled_t> installedCallback;

	private static CallResult<SubmitItemUpdateResult_t> submitResult;

	private static CallResult<CreateItemResult_t> createResult;

	private static CallResult<SteamUGCRequestUGCDetailsResult_t> requestDetailsResult;

	private static UGCQueryHandle_t detailsQueryHandle;

	private static int detailsQueryCount = -1;

	public const uint InstallInfoFolderNameMaxLength = 257u;

	public static WorkshopInteractStage CurStage => curStage;

	internal static void Init()
	{
		subscribedCallback = Callback<RemoteStoragePublishedFileSubscribed_t>.Create((DispatchDelegate<RemoteStoragePublishedFileSubscribed_t>)OnItemSubscribed);
		installedCallback = Callback<ItemInstalled_t>.Create((DispatchDelegate<ItemInstalled_t>)OnItemInstalled);
		unsubscribedCallback = Callback<RemoteStoragePublishedFileUnsubscribed_t>.Create((DispatchDelegate<RemoteStoragePublishedFileUnsubscribed_t>)OnItemUnsubscribed);
	}

	internal static void Upload(WorkshopUploadable item)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		if (curStage != 0)
		{
			Messages.Message("UploadAlreadyInProgress".Translate(), MessageTypeDefOf.RejectInput, historical: false);
			return;
		}
		uploadingHook = item.GetWorkshopItemHook();
		if (uploadingHook.PublishedFileId != PublishedFileId_t.Invalid)
		{
			if (Prefs.LogVerbose)
			{
				string name = uploadingHook.Name;
				PublishedFileId_t publishedFileId = uploadingHook.PublishedFileId;
				Log.Message("Workshop: Starting item update for mod '" + name + "' with PublishedFileId " + ((object)(PublishedFileId_t)(ref publishedFileId)).ToString());
			}
			curStage = WorkshopInteractStage.SubmittingItem;
			curUpdateHandle = SteamUGC.StartItemUpdate(SteamUtils.GetAppID(), uploadingHook.PublishedFileId);
			SetWorkshopItemDataFrom(curUpdateHandle, uploadingHook, creating: false);
			SteamAPICall_t val = SteamUGC.SubmitItemUpdate(curUpdateHandle, "[Auto-generated text]: Update on " + DateTime.Now.ToString() + ".");
			submitResult = CallResult<SubmitItemUpdateResult_t>.Create((APIDispatchDelegate<SubmitItemUpdateResult_t>)OnItemSubmitted);
			submitResult.Set(val, (APIDispatchDelegate<SubmitItemUpdateResult_t>)null);
		}
		else
		{
			if (Prefs.LogVerbose)
			{
				Log.Message("Workshop: Starting item creation for mod '" + uploadingHook.Name + "'.");
			}
			curStage = WorkshopInteractStage.CreatingItem;
			SteamAPICall_t val2 = SteamUGC.CreateItem(SteamUtils.GetAppID(), (EWorkshopFileType)0);
			createResult = CallResult<CreateItemResult_t>.Create((APIDispatchDelegate<CreateItemResult_t>)OnItemCreated);
			createResult.Set(val2, (APIDispatchDelegate<CreateItemResult_t>)null);
		}
		Find.WindowStack.Add(new Dialog_WorkshopOperationInProgress());
	}

	internal static void Unsubscribe(PublishedFileId_t pfid)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		SteamUGC.UnsubscribeItem(pfid);
	}

	internal static void Unsubscribe(WorkshopUploadable item)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		SteamUGC.UnsubscribeItem(item.GetPublishedFileId());
	}

	internal static void RequestItemsDetails(PublishedFileId_t[] publishedFileIds)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		if (detailsQueryCount >= 0)
		{
			Log.Error("Requested Workshop item details while a details request was already pending.");
			return;
		}
		detailsQueryCount = publishedFileIds.Length;
		detailsQueryHandle = SteamUGC.CreateQueryUGCDetailsRequest(publishedFileIds, (uint)detailsQueryCount);
		SteamAPICall_t val = SteamUGC.SendQueryUGCRequest(detailsQueryHandle);
		requestDetailsResult = CallResult<SteamUGCRequestUGCDetailsResult_t>.Create((APIDispatchDelegate<SteamUGCRequestUGCDetailsResult_t>)OnGotItemDetails);
		requestDetailsResult.Set(val, (APIDispatchDelegate<SteamUGCRequestUGCDetailsResult_t>)null);
	}

	internal static void OnItemSubscribed(RemoteStoragePublishedFileSubscribed_t result)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		if (IsOurAppId(result.m_nAppID))
		{
			if (Prefs.LogVerbose)
			{
				PublishedFileId_t nPublishedFileId = result.m_nPublishedFileId;
				Log.Message("Workshop: Item subscribed: " + ((object)(PublishedFileId_t)(ref nPublishedFileId)).ToString());
			}
			Find.WindowStack.WindowOfType<Page_ModsConfig>()?.Notify_SteamItemSubscribed(result.m_nPublishedFileId);
			WorkshopItems.Notify_Subscribed(result.m_nPublishedFileId);
		}
	}

	internal static void OnItemInstalled(ItemInstalled_t result)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		if (IsOurAppId(result.m_unAppID))
		{
			if (Prefs.LogVerbose)
			{
				PublishedFileId_t nPublishedFileId = result.m_nPublishedFileId;
				Log.Message("Workshop: Item installed: " + ((object)(PublishedFileId_t)(ref nPublishedFileId)).ToString());
			}
			Find.WindowStack.WindowOfType<Page_ModsConfig>()?.Notify_SteamItemInstalled(result.m_nPublishedFileId);
			WorkshopItems.Notify_Installed(result.m_nPublishedFileId);
		}
	}

	internal static void OnItemUnsubscribed(RemoteStoragePublishedFileUnsubscribed_t result)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		if (IsOurAppId(result.m_nAppID))
		{
			if (Prefs.LogVerbose)
			{
				PublishedFileId_t nPublishedFileId = result.m_nPublishedFileId;
				Log.Message("Workshop: Item unsubscribed: " + ((object)(PublishedFileId_t)(ref nPublishedFileId)).ToString());
			}
			Find.WindowStack.WindowOfType<Page_ModsConfig>()?.Notify_SteamItemUnsubscribed(result.m_nPublishedFileId);
			Find.WindowStack.WindowOfType<Page_SelectScenario>()?.Notify_SteamItemUnsubscribed(result.m_nPublishedFileId);
			WorkshopItems.Notify_Unsubscribed(result.m_nPublishedFileId);
		}
	}

	private static void OnItemCreated(CreateItemResult_t result, bool IOFailure)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Invalid comparison between Unknown and I4
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		if (IOFailure || (int)result.m_eResult != 1)
		{
			uploadingHook = null;
			Dialog_WorkshopOperationInProgress.CloseAll();
			Log.Error("Workshop: OnItemCreated failure. Result: " + result.m_eResult.GetLabel());
			Find.WindowStack.Add(new Dialog_MessageBox("WorkshopSubmissionFailed".Translate(GenText.SplitCamelCase(result.m_eResult.GetLabel()))));
			return;
		}
		uploadingHook.PublishedFileId = result.m_nPublishedFileId;
		if (Prefs.LogVerbose)
		{
			PublishedFileId_t publishedFileId = uploadingHook.PublishedFileId;
			Log.Message("Workshop: Item created. PublishedFileId: " + ((object)(PublishedFileId_t)(ref publishedFileId)).ToString());
		}
		curUpdateHandle = SteamUGC.StartItemUpdate(SteamUtils.GetAppID(), uploadingHook.PublishedFileId);
		SetWorkshopItemDataFrom(curUpdateHandle, uploadingHook, creating: true);
		curStage = WorkshopInteractStage.SubmittingItem;
		if (Prefs.LogVerbose)
		{
			Log.Message("Workshop: Submitting item.");
		}
		SteamAPICall_t val = SteamUGC.SubmitItemUpdate(curUpdateHandle, "[Auto-generated text]: Initial upload.");
		submitResult = CallResult<SubmitItemUpdateResult_t>.Create((APIDispatchDelegate<SubmitItemUpdateResult_t>)OnItemSubmitted);
		submitResult.Set(val, (APIDispatchDelegate<SubmitItemUpdateResult_t>)null);
		createResult = null;
	}

	private static void OnItemSubmitted(SubmitItemUpdateResult_t result, bool IOFailure)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Invalid comparison between Unknown and I4
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		if (IOFailure || (int)result.m_eResult != 1)
		{
			uploadingHook = null;
			Dialog_WorkshopOperationInProgress.CloseAll();
			Log.Error("Workshop: OnItemSubmitted failure. Result: " + result.m_eResult.GetLabel());
			Find.WindowStack.Add(new Dialog_MessageBox("WorkshopSubmissionFailed".Translate(GenText.SplitCamelCase(result.m_eResult.GetLabel()))));
		}
		else
		{
			SteamUtility.OpenWorkshopPage(uploadingHook.PublishedFileId);
			Messages.Message("WorkshopUploadSucceeded".Translate(uploadingHook.Name), MessageTypeDefOf.TaskCompletion, historical: false);
			if (Prefs.LogVerbose)
			{
				Log.Message("Workshop: Item submit result: " + ((object)(EResult)(ref result.m_eResult)).ToString());
			}
		}
		curStage = WorkshopInteractStage.None;
		submitResult = null;
	}

	private static void OnGotItemDetails(SteamUGCRequestUGCDetailsResult_t result, bool IOFailure)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Invalid comparison between Unknown and I4
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		if (IOFailure)
		{
			Log.Error("Workshop: OnGotItemDetails IOFailure.");
			detailsQueryCount = -1;
			return;
		}
		if (detailsQueryCount < 0)
		{
			Log.Warning("Got unexpected Steam Workshop item details response.");
		}
		string text = "Steam Workshop Item details received:";
		SteamUGCDetails_t val = default(SteamUGCDetails_t);
		for (int i = 0; i < detailsQueryCount; i++)
		{
			SteamUGC.GetQueryUGCResult(detailsQueryHandle, (uint)i, ref val);
			if ((int)val.m_eResult != 1)
			{
				text = text + "\n  Query result: " + ((object)(EResult)(ref val.m_eResult)).ToString();
			}
			else
			{
				text = text + "\n  Title: " + ((SteamUGCDetails_t)(ref val)).m_rgchTitle;
				string text2 = text;
				PublishedFileId_t nPublishedFileId = val.m_nPublishedFileId;
				text = text2 + "\n  PublishedFileId: " + ((object)(PublishedFileId_t)(ref nPublishedFileId)).ToString();
				text = text + "\n  Created: " + DateTime.FromFileTimeUtc(val.m_rtimeCreated);
				text = text + "\n  Updated: " + DateTime.FromFileTimeUtc(val.m_rtimeUpdated);
				text = text + "\n  Added to list: " + DateTime.FromFileTimeUtc(val.m_rtimeAddedToUserList);
				text = text + "\n  File size: " + val.m_nFileSize.ToStringKilobytes();
				text = text + "\n  Preview size: " + val.m_nPreviewFileSize.ToStringKilobytes();
				text = text + "\n  File name: " + ((SteamUGCDetails_t)(ref val)).m_pchFileName;
				string text3 = text;
				AppId_t val2 = val.m_nCreatorAppID;
				text = text3 + "\n  CreatorAppID: " + ((object)(AppId_t)(ref val2)).ToString();
				string text4 = text;
				val2 = val.m_nConsumerAppID;
				text = text4 + "\n  ConsumerAppID: " + ((object)(AppId_t)(ref val2)).ToString();
				text = text + "\n  Visibiliy: " + ((object)(ERemoteStoragePublishedFileVisibility)(ref val.m_eVisibility)).ToString();
				text = text + "\n  FileType: " + ((object)(EWorkshopFileType)(ref val.m_eFileType)).ToString();
				text = text + "\n  Owner: " + val.m_ulSteamIDOwner;
			}
			text += "\n";
		}
		Log.Message(text.TrimEndNewlines());
		detailsQueryCount = -1;
	}

	public static void GetUpdateStatus(out EItemUpdateStatus updateStatus, out float progPercent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected I4, but got Unknown
		ulong num = default(ulong);
		ulong num2 = default(ulong);
		updateStatus = (EItemUpdateStatus)(int)SteamUGC.GetItemUpdateProgress(curUpdateHandle, ref num, ref num2);
		progPercent = (float)num / (float)num2;
	}

	public static string UploadButtonLabel(PublishedFileId_t pfid)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return (pfid != PublishedFileId_t.Invalid) ? "UpdateOnSteamWorkshop".Translate() : "UploadToSteamWorkshop".Translate();
	}

	private static void SetWorkshopItemDataFrom(UGCUpdateHandle_t updateHandle, WorkshopItemHook hook, bool creating)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		hook.PrepareForWorkshopUpload();
		SteamUGC.SetItemTitle(updateHandle, hook.Name);
		if (creating)
		{
			SteamUGC.SetItemDescription(updateHandle, hook.Description);
		}
		if (!File.Exists(hook.PreviewImagePath))
		{
			Log.Warning("Missing preview file at " + hook.PreviewImagePath);
		}
		else
		{
			SteamUGC.SetItemPreview(updateHandle, hook.PreviewImagePath);
		}
		IList<string> tags = hook.Tags;
		foreach (Version supportedVersion in hook.SupportedVersions)
		{
			tags.Add(supportedVersion.Major + "." + supportedVersion.Minor);
		}
		SteamUGC.SetItemTags(updateHandle, tags);
		SteamUGC.SetItemContent(updateHandle, hook.Directory.FullName);
	}

	internal static IEnumerable<PublishedFileId_t> AllSubscribedItems()
	{
		uint numSubscribedItems = SteamUGC.GetNumSubscribedItems();
		PublishedFileId_t[] subbedItems = (PublishedFileId_t[])(object)new PublishedFileId_t[numSubscribedItems];
		uint count = SteamUGC.GetSubscribedItems(subbedItems, numSubscribedItems);
		for (int i = 0; i < count; i++)
		{
			yield return subbedItems[i];
		}
	}

	[DebugOutput("System", false)]
	internal static void SteamWorkshopStatus()
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("All subscribed items (" + SteamUGC.GetNumSubscribedItems() + " total):");
		List<PublishedFileId_t> list = AllSubscribedItems().ToList();
		for (int i = 0; i < list.Count; i++)
		{
			stringBuilder.AppendLine("   " + ItemStatusString(list[i]));
		}
		stringBuilder.AppendLine("All installed mods:");
		foreach (ModMetaData allInstalledMod in ModLister.AllInstalledMods)
		{
			stringBuilder.AppendLine("   " + allInstalledMod.PackageIdPlayerFacing + ": " + ItemStatusString(allInstalledMod.GetPublishedFileId()));
		}
		Log.Message(stringBuilder.ToString());
		List<PublishedFileId_t> list2 = AllSubscribedItems().ToList();
		PublishedFileId_t[] array = (PublishedFileId_t[])(object)new PublishedFileId_t[list2.Count];
		for (int j = 0; j < list2.Count; j++)
		{
			array[j] = (PublishedFileId_t)list2[j].m_PublishedFileId;
		}
		RequestItemsDetails(array);
	}

	private static string ItemStatusString(PublishedFileId_t pfid)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (pfid == PublishedFileId_t.Invalid)
		{
			return "[unpublished]";
		}
		PublishedFileId_t val = pfid;
		string text = "[" + ((object)(PublishedFileId_t)(ref val)).ToString() + "] ";
		ulong num = default(ulong);
		string text2 = default(string);
		uint num2 = default(uint);
		if (SteamUGC.GetItemInstallInfo(pfid, ref num, ref text2, 257u, ref num2))
		{
			text += "\n      installed";
			text = text + "\n      folder=" + text2;
			return text + "\n      sizeOnDisk=" + ((float)num / 1024f).ToString("F2") + "Kb";
		}
		return text + "\n      not installed";
	}

	private static bool IsOurAppId(AppId_t appId)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (appId != SteamUtils.GetAppID())
		{
			return false;
		}
		return true;
	}
}
