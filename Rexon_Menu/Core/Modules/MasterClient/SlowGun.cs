// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.SlowGun
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Photon.Pun;
using Photon.Realtime;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

using StatusEffects = RoomSystem.StatusEffects;

namespace Recovered.Obfuscated;

[Mod("Slow Gun", "Masterclient", "Slows player you shoot.", false, 4, ModType.Toggle, false)]
internal class SlowGun : MonoBehaviour
{
	private const string GunId = "SlowGun";


	private void Update()
	{
		if (!PhotonNetwork.InRoom || !PhotonNetwork.IsMasterClient)
		{
			GunController.Release(GunId);
			return;
		}
		GunController.GunResult gunResult = GunController.GetGunResult(GunId, targetPlayers: true, 1f);
		if (gunResult.IsActive && gunResult.CanFire && gunResult.Target != null)
		{
			GunController.MarkFired(GunId);
			RaiseEventOptions options = new RaiseEventOptions
			{
				TargetActors = new[] { gunResult.Target.ActorNumber }
			};
			GameNetworkUtilities.SendStatusEffect((StatusEffects)0, options);
			GameNetworkUtilities.VibrateHand(isLeftHand: false, 0.5f, 0.2f);
		}
	}

	private void OnDisable()
	{
		GunController.Release(GunId);
	}
}
