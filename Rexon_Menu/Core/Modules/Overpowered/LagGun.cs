// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.LagGun
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Lag Gun", "Overpowered", "Lags players you select.", false, 11, ModType.Toggle, false)]
internal class LagGun : MonoBehaviour
{
	private const string GunId = "LagGun";


	private void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			GunController.Release(GunId);
			return;
		}
		GunController.GunResult gunResult = GunController.GetGunResult(
			GunId, targetPlayers: true, 2.5f, allowSingleTargetLock: false, allowMultipleTargetLocks: true);
		if ((gunResult.IsActive || gunResult.LockedTargets.Length != 0) && gunResult.CanFire && gunResult.LockedTargets.Length != 0)
		{
			GunController.MarkFired(GunId);
			ApplyLagToPlayers(gunResult.LockedTargets);
		}
	}

	private void ApplyLagToPlayers(Player[] players)
	{
		int[] targetActorNumbers = new int[players.Length];
		for (int i = 0; i < players.Length; i++)
		{
			targetActorNumbers[i] = players[i].ActorNumber;
		}
		for (int j = 0; j < Rexon_Menu.Core.Modules.Settings.LagPower.Power; j++)
		{
			PhotonNetwork.NetworkingClient.OpRaiseEvent((byte)186, (object)new object[1] { float.NaN }, new RaiseEventOptions
			{
				TargetActors = targetActorNumbers
			}, SendOptions.SendUnreliable);
		}
		GameNetworkUtilities.FlushAndReplayNetworkTraffic();
	}

	private void OnDisable()
	{
		GunController.Release(GunId);
	}
}
