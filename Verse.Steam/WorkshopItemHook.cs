using System;
using System.Collections.Generic;
using System.IO;
using Steamworks;

namespace Verse.Steam;

public class WorkshopItemHook
{
	private WorkshopUploadable owner;

	private CSteamID steamAuthor = CSteamID.Nil;

	private CallResult<SteamUGCRequestUGCDetailsResult_t> queryResult;

	public PublishedFileId_t PublishedFileId
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return owner.GetPublishedFileId();
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			owner.SetPublishedFileId(value);
		}
	}

	public string Name => owner.GetWorkshopName();

	public string Description => owner.GetWorkshopDescription();

	public string PreviewImagePath => owner.GetWorkshopPreviewImagePath();

	public IList<string> Tags => owner.GetWorkshopTags();

	public DirectoryInfo Directory => owner.GetWorkshopUploadDirectory();

	public IEnumerable<Version> SupportedVersions => owner.SupportedVersions;

	public bool MayHaveAuthorNotCurrentUser
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			if (PublishedFileId == PublishedFileId_t.Invalid)
			{
				return false;
			}
			if (steamAuthor == CSteamID.Nil)
			{
				return true;
			}
			return steamAuthor != SteamUser.GetSteamID();
		}
	}

	public WorkshopItemHook(WorkshopUploadable owner)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		this.owner = owner;
		if (owner.GetPublishedFileId() != PublishedFileId_t.Invalid)
		{
			SendSteamDetailsQuery();
		}
	}

	public void PrepareForWorkshopUpload()
	{
		owner.PrepareForWorkshopUpload();
	}

	private void SendSteamDetailsQuery()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (SteamManager.Initialized)
		{
			SteamAPICall_t val = SteamUGC.RequestUGCDetails(PublishedFileId, 999999u);
			queryResult = CallResult<SteamUGCRequestUGCDetailsResult_t>.Create((APIDispatchDelegate<SteamUGCRequestUGCDetailsResult_t>)OnDetailsQueryReturned);
			queryResult.Set(val, (APIDispatchDelegate<SteamUGCRequestUGCDetailsResult_t>)null);
		}
	}

	private void OnDetailsQueryReturned(SteamUGCRequestUGCDetailsResult_t result, bool IOFailure)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		steamAuthor = (CSteamID)result.m_details.m_ulSteamIDOwner;
	}
}
