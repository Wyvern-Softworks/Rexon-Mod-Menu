// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.VibrateGun
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Photon.Pun;
using Photon.Realtime;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

using StatusEffects = RoomSystem.StatusEffects;

namespace Recovered.Obfuscated;

[Mod("Vibrate Gun", "Masterclient", "Vibrates player you shoot.", false, 2, ModType.Toggle, false)]
internal class VibrateGun : MonoBehaviour
{
	private const string GunId = "VibrateGun";


	private void Update()
	{
		if (!PhotonNetwork.InRoom || !PhotonNetwork.IsMasterClient)
		{
			GunController.Release(GunId);
			return;
		}
		GunController.GunResult gunResult = GunController.GetGunResult(GunId, targetPlayers: true, 0.5f);
		if (gunResult.IsActive && gunResult.CanFire && gunResult.Target != null)
		{
			GunController.MarkFired(GunId);
			RaiseEventOptions options = new RaiseEventOptions
			{
				TargetActors = new[] { gunResult.Target.ActorNumber }
			};
			GameNetworkUtilities.SendStatusEffect((StatusEffects)1, options);
			GameNetworkUtilities.VibrateHand(isLeftHand: false, 0.5f, 0.2f);
		}
	}

	private void OnDisable()
	{
		GunController.Release(GunId);
	}
}
