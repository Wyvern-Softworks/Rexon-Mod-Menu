// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.Poop
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using BepInEx;
using Photon.Pun;
using Rexon_Menu.Core.Attributes;
using UnityEngine;
using UnityEngine.XR;

namespace Recovered.Obfuscated;

[Mod("Poop", "Projectiles", "Drop brown projectiles.", false, 6, ModType.Toggle, false)]
internal class Poop : MonoBehaviour
{
	private float _lastSpawnTime;


	private void Update()
	{
		if (PhotonNetwork.InRoom)
		{
			bool isFiring = !XRSettings.isDeviceActive
				? UnityInput.Current.GetMouseButton(0)
				: ControllerInputPoller.TriggerFloat(XRNode.RightHand) > 0.2f;
			if (isFiring && Time.time > _lastSpawnTime + 0.4f)
			{
				_lastSpawnTime = Time.time;
				Vector3 launchPosition = GorillaTagger.Instance.offlineVRRig.transform.position - new Vector3(0f, 0.3f, 0f);
				Vector3 velocity = Vector3.down;
				Color brown = new Color(0.5f, 0.3f, 0.1f);
				GameNetworkUtilities.LaunchNetworkedProjectile(launchPosition, velocity, brown);
			}
		}
	}
}
