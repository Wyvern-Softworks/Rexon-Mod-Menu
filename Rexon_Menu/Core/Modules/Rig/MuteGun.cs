// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.MuteGun
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Linq;
using Photon.Pun;
using Rexon_Menu.Core.Attributes;
using UnityEngine;
using ButtonType = GorillaPlayerLineButton.ButtonType;

namespace Recovered.Obfuscated;

[Mod("Mute Gun (CS)", "Rig", "Client-side mute via gun.", false, 44, ModType.Toggle, false)]
internal class MuteGun : MonoBehaviour
{
	private const string GunId = "MuteGunCS";

	private void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			GunController.Release(GunId);
			return;
		}

		GunController.GunResult gunResult = GunController.GetGunResult(
			GunId, targetPlayers: true, 0.5f, allowSingleTargetLock: true);
		if (!gunResult.IsActive || !gunResult.IsShooting || !gunResult.CanFire || gunResult.Target == null)
		{
			return;
		}

		NetworkSystem network = Object.FindObjectOfType<NetworkSystem>();
		if (network == null)
		{
			return;
		}

		NetPlayer target = network.GetNetPlayerByID(gunResult.Target.ActorNumber);
		if (target == null)
		{
			return;
		}

		GorillaPlayerScoreboardLine[] targetLines = Object
			.FindObjectsOfType<GorillaPlayerScoreboardLine>()
			.Where(line => line.linePlayer == target)
			.ToArray();
		if (targetLines.Length == 0)
		{
			return;
		}

		targetLines[0].PressButton(true, (ButtonType)3);
		foreach (GorillaPlayerScoreboardLine line in targetLines)
		{
			line.muteButton.isOn = true;
			line.muteButton.UpdateColor();
		}
	}

	private void OnDisable() => GunController.Release(GunId);
}
