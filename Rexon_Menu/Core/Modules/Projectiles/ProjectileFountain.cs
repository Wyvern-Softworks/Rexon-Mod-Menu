// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.ProjectileFountain
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Photon.Pun;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Projectile Fountain", "Projectiles", "Fountain of projectiles.", false, 4, ModType.Toggle, false)]
internal class ProjectileFountain : MonoBehaviour
{
	private float _lastSpawnTime;

	private void Update()
	{
		if (PhotonNetwork.InRoom && Time.time > _lastSpawnTime + 0.4f)
		{
			_lastSpawnTime = Time.time;
			Vector3 launchPosition = GorillaTagger.Instance.offlineVRRig.transform.position;
			Vector3 velocity = new Vector3(
				Random.Range(-4f, 4f), Random.Range(21f, 23f), Random.Range(-4f, 4f));
			GameNetworkUtilities.LaunchNetworkedProjectile(
				launchPosition, velocity, GameNetworkUtilities.GetPaletteColor(GameNetworkUtilities.ProjectileColorIndex));
		}
	}
}
