// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.VibrateOthers
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Photon.Pun;
using Photon.Realtime;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

using StatusEffects = RoomSystem.StatusEffects;

namespace Recovered.Obfuscated;

[Mod("Vibrate Others", "Masterclient", "Vibrates all other players.", false, 1, ModType.Toggle, false)]
internal class VibrateOthers : MonoBehaviour
{
	private float _lastVibrationTime;

	private void Update()
	{
		if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient && Time.time > _lastVibrationTime + 0.5f)
		{
			_lastVibrationTime = Time.time;
			GameNetworkUtilities.SendStatusEffect((StatusEffects)1, new RaiseEventOptions
			{
				Receivers = (ReceiverGroup)0
			});
		}
	}
}

