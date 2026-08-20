// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Tag.ForceTagLag
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using GorillaTagScripts;
using Photon.Pun;
using Recovered.Obfuscated;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.Tag;

[Mod("Force Tag Lag [MASTER]", "Tag", "Lags the tag system for the entire lobby. Requires master.", true, 7, ModType.Action, false)]
internal class ForceTagLag : MonoBehaviour
{
	private const string AddPartyMembersRpc = "AddPartyMembers";
	private const string InfectionMode = "Infection";

	private void OnEnable()
	{
		if (!PhotonNetwork.InRoom)
		{
			Object.Destroy(this);
			return;
		}
		FriendshipGroupDetection instance = FriendshipGroupDetection.Instance;
		PhotonView view = instance == null ? null : instance.GetComponent<PhotonView>();
		if (view == null)
		{
			Object.Destroy(this);
			return;
		}
		for (int i = 0; i < 3000; i++)
		{
			view.SendRpc(AddPartyMembersRpc, RpcTarget.MasterClient, InfectionMode, (short)12, null);
		}
		GameNetworkUtilities.FlushAndReplayNetworkTraffic();
		Object.Destroy(this);
	}
}
