// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.ProjectileRain
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using GorillaLocomotion;
using Photon.Pun;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Projectile Rain", "Projectiles", "Rain projectiles from above.", false, 3, ModType.Toggle, false)]
internal class ProjectileRain : MonoBehaviour
{
	private float _lastSpawnTime;

	private void Update()
	{
		if (PhotonNetwork.InRoom && Time.time > _lastSpawnTime + 0.4f)
		{
			_lastSpawnTime = Time.time;
			Vector3 position = GTPlayer.Instance.transform.position;
			Vector3 spawnPosition = new Vector3(
				position.x + Random.Range(-2f, 2f),
				position.y + Random.Range(3f, 4f),
				position.z + Random.Range(-2f, 2f));
			Color color = GameNetworkUtilities.GetPaletteColor(GameNetworkUtilities.ProjectileColorIndex);
			GameNetworkUtilities.LaunchNetworkedProjectile(spawnPosition, Vector3.zero, color);
		}
	}
}

