// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.ProjectileMinigun
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Photon.Pun;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Projectile Minigun", "Projectiles", "Rapid fire projectiles.", false, 1, ModType.Toggle, false)]
internal class ProjectileMinigun : MonoBehaviour
{
	private const string GunId = "ProjectileMinigun";


	private void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			GunController.Release(GunId);
			return;
		}
		GunController.GunResult gunResult = GunController.GetGunResult(GunId, targetPlayers: false, 0.4f);
		Vector3 hitPoint = gunResult.Hit.point;
		if (gunResult.IsActive && gunResult.IsShooting && gunResult.CanFire && hitPoint != Vector3.zero)
		{
			GunController.MarkFired(GunId);
			Vector3 launchPosition = GorillaTagger.Instance.rightHandTransform.position;
			Vector3 direction = hitPoint - launchPosition;
			Vector3 velocity = direction.normalized * GameNetworkUtilities.ProjectileSpeeds[GameNetworkUtilities.ProjectileSpeedIndex];
			GameNetworkUtilities.LaunchNetworkedProjectile(
				launchPosition, velocity, GameNetworkUtilities.GetPaletteColor(GameNetworkUtilities.ProjectileColorIndex));
		}
	}

	private void OnDisable()
	{
		GunController.Release(GunId);
	}
}
