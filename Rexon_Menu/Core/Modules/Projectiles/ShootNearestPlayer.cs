// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.ShootNearestPlayer
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Photon.Pun;
using Photon.Realtime;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Shoot Nearest Player", "Projectiles", "Shoots at nearest player.", false, 5, ModType.Toggle, false)]
internal class ShootNearestPlayer : MonoBehaviour
{
	private float _lastShotTime;


	private void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		VRRig nearestRig = null;
		float nearestDistance = float.MaxValue;
		VRRig localRig = GorillaGameManager.instance.FindPlayerVRRig((NetPlayer)PhotonNetwork.LocalPlayer);
		foreach (Player player in PhotonNetwork.PlayerListOthers)
		{
			VRRig rig = GorillaGameManager.instance.FindPlayerVRRig((NetPlayer)player);
			if (rig != null && localRig != null)
			{
				float distance = Vector3.Distance(localRig.transform.position, rig.transform.position);
				if (distance < nearestDistance)
				{
					nearestDistance = distance;
					nearestRig = rig;
				}
			}
		}
		if (nearestRig != null && Time.time > _lastShotTime + 0.4f)
		{
			_lastShotTime = Time.time;
			Vector3 launchPosition = GorillaTagger.Instance.rightHandTransform.position;
			Vector3 direction = nearestRig.transform.position - launchPosition;
			Vector3 velocity = direction.normalized * GameNetworkUtilities.ProjectileSpeeds[GameNetworkUtilities.ProjectileSpeedIndex];
			GameNetworkUtilities.LaunchNetworkedProjectile(
				launchPosition, velocity, GameNetworkUtilities.GetPaletteColor(GameNetworkUtilities.ProjectileColorIndex));
		}
	}
}
