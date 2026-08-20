// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.World.WaterSplashGun
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Photon.Pun;
using Recovered.Obfuscated;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.World;

[Mod("Water Splash Gun [RT]", "World", "Shoot water splashes where you aim.", false, 20, ModType.Toggle, false)]
internal class WaterSplashGun : MonoBehaviour
{
	private const string GunId = "WaterSplashGun";
	private const string SplashRpc = "RPC_PlaySplashEffect";

	private float _lastSplashTime;

	private void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			GunController.Release(GunId);
			return;
		}
		GunController.GunResult gunResult = GunController.GetGunResult(GunId, targetPlayers: false);
		if (!gunResult.IsActive)
		{
			return;
		}
		if (gunResult.IsShooting && Time.time > _lastSplashTime + 0.1f)
		{
			_lastSplashTime = Time.time;
			Vector3 splashPosition = gunResult.Hit.point;
			if (splashPosition != Vector3.zero)
			{
				GorillaTagger.Instance.offlineVRRig.enabled = false;
				Transform rigTransform = GorillaTagger.Instance.offlineVRRig.transform;
				rigTransform.position = splashPosition + Vector3.up;
				GorillaTagger.Instance.myVRRig.GetView.SendRpc(
					SplashRpc, RpcTarget.All, splashPosition, rigTransform.rotation, 1f, 900f, true, false);
				GameNetworkUtilities.FlushAndReplayNetworkTraffic();
			}
		}
		else if (!gunResult.IsShooting)
		{
			GorillaTagger.Instance.offlineVRRig.enabled = true;
		}
	}

	private void OnDisable()
	{
		ResetWaterGun();
	}

	private void OnDestroy()
	{
		ResetWaterGun();
	}

	private void ResetWaterGun()
	{
		GunController.Release(GunId);
		GorillaTagger.Instance.offlineVRRig.enabled = true;
		_lastSplashTime = 0f;
	}
}
