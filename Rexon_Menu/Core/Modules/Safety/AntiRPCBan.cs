// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Safety.AntiRPCBan
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Photon.Pun;
using Recovered.Obfuscated;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.Safety;

[Mod("Anti RPC Ban", "Safety", "Prevents disconnects from using too many mods.", false, 15, ModType.Toggle, false)]
internal class AntiRPCBan : MonoBehaviour
{
	private float _lastResetTime;

	private void Update()
	{
		if (PhotonNetwork.InRoom && Time.time >= _lastResetTime + 0.7f)
		{
			_lastResetTime = Time.time;
			MonkeAgent[] agents = Object.FindObjectsOfType<MonkeAgent>();
			foreach (MonkeAgent agent in agents)
			{
				agent.rpcErrorMax = int.MaxValue;
				agent.logErrorMax = int.MaxValue;
				agent.lastCheck = 0f;
			}
			GameNetworkUtilities.FlushAndReplayNetworkTraffic();
		}
	}
}
