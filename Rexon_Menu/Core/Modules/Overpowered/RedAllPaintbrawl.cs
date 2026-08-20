// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Overpowered.RedAllPaintbrawl
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Photon.Pun;
using Photon.Realtime;
using Recovered.Obfuscated;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.Overpowered;

[Mod("Red All [PAINTBRAWL]", "Tag", "Eliminates all players in paintbrawl.", false, 44, ModType.Toggle, false)]
internal class RedAllPaintbrawl : MonoBehaviour
{
	private const string GameModePath = "Player Objects/RigCache/Network Parent/GameMode(Clone)";
	private const string ReportHitRpc = "RPC_ReportSlingshotHit";

	private float _lastUpdateTime;

	private void Update()
	{
		if (!PhotonNetwork.InRoom || Time.time < _lastUpdateTime + 0.2f)
		{
			return;
		}
		_lastUpdateTime = Time.time;
		GameObject gameModeObject = GameObject.Find(GameModePath);
		if (gameModeObject == null)
		{
			return;
		}
		PhotonView view = gameModeObject.GetComponent<PhotonView>();
		if (view != null)
		{
			foreach (Player player in PhotonNetwork.PlayerListOthers)
			{
				view.SendRpc(ReportHitRpc, RpcTarget.MasterClient, player, Vector3.zero, 1);
			}
		}
	}
}
