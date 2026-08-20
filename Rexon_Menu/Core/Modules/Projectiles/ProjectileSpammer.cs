// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.ProjectileSpammer
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using BepInEx;
using Photon.Pun;
using Rexon_Menu.Core.Attributes;
using UnityEngine;
using UnityEngine.XR;

namespace Recovered.Obfuscated;

[Mod("Projectile Spammer", "Projectiles", "Spam projectiles from hand.", false, 2, ModType.Toggle, false)]
internal class ProjectileSpammer : MonoBehaviour
{
	private float _lastSpawnTime;


	private void Update()
	{
		if (PhotonNetwork.InRoom)
		{
			bool isFiring = !XRSettings.isDeviceActive
				? UnityInput.Current.GetMouseButton(1)
				: ControllerInputPoller.GripFloat(XRNode.RightHand) > 0.2f;
			if (isFiring && Time.time > _lastSpawnTime + 0.4f)
			{
				_lastSpawnTime = Time.time;
				GameNetworkUtilities.LaunchNetworkedProjectile(GorillaTagger.Instance.rightHandTransform.position, Vector3.zero, GameNetworkUtilities.GetPaletteColor(GameNetworkUtilities.ProjectileColorIndex));
			}
		}
	}
}

