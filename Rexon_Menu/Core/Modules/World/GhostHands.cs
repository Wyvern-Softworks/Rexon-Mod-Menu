// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.World.GhostHands
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Photon.Pun;
using Recovered.Obfuscated;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.World;

[Mod("Ghost Hands", "World", "Plays random hand tap sounds around you.", false, 32, ModType.Toggle, false)]
internal class GhostHands : MonoBehaviour
{
	private const string PlayHandTapRpc = "RPC_PlayHandTap";

	private float _lastSpawnTime;


	private void Update()
	{
		if (PhotonNetwork.InRoom && Time.time >= _lastSpawnTime + 0.08f)
		{
			_lastSpawnTime = Time.time;
			int soundIndex = Random.Range(0, 10);
			bool isLeftHand = Random.value > 0.5f;
			GorillaTagger.Instance.myVRRig.GetView.SendRpc(
				PlayHandTapRpc, RpcTarget.All, soundIndex, isLeftHand, 0.08f);
			GameNetworkUtilities.FlushAndReplayNetworkTraffic();
		}
	}
}
