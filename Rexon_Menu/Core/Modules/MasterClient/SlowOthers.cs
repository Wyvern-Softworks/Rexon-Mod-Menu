// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.SlowOthers
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Photon.Pun;
using Photon.Realtime;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

using StatusEffects = RoomSystem.StatusEffects;

namespace Recovered.Obfuscated;

[Mod("Slow Others", "Masterclient", "Slows all other players.", false, 3, ModType.Toggle, false)]
internal class SlowOthers : MonoBehaviour
{
	private float _lastUpdateTime;

	private void Update()
	{
		if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient && Time.time > _lastUpdateTime + 1f)
		{
			_lastUpdateTime = Time.time;
			GameNetworkUtilities.SendStatusEffect((StatusEffects)0, new RaiseEventOptions
			{
				Receivers = (ReceiverGroup)0
			});
		}
	}
}

