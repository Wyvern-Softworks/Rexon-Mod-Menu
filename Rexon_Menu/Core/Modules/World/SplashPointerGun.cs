// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.SplashPointerGun
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Photon.Pun;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Splash Pointer Gun", "World", "Shoot splashes at aim point.", false, 14, ModType.Toggle, false)]
internal class SplashPointerGun : MonoBehaviour
{
	private const string GunId = "SplashPointerGun";
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
		if (gunResult.IsActive && gunResult.IsShooting && Time.time > _lastSplashTime + 0.1f)
		{
			_lastSplashTime = Time.time;
			Vector3 hitPoint = gunResult.Hit.point;
			Vector3 rigPosition = GorillaTagger.Instance.offlineVRRig.transform.position;
			if (hitPoint == Vector3.zero)
			{
				return;
			}
			if ((rigPosition - hitPoint).sqrMagnitude < 9f)
			{
				GorillaTagger.Instance.myVRRig.GetView.SendRpc(
					SplashRpc, RpcTarget.All, hitPoint, Quaternion.identity, 1f, 0.5f, false, true);
				GameNetworkUtilities.FlushAndReplayNetworkTraffic();
			}
		}
	}

	private void OnDisable()
	{
		ClearPointer();
	}

	private void ClearPointer()
	{
		GunController.Release(GunId);
		_lastSplashTime = 0f;
	}
}
