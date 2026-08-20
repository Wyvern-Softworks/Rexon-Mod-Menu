// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.World.SplashStormGun
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Photon.Pun;
using Photon.Realtime;
using Recovered.Obfuscated;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.World;

[Mod("Teleport Splash Storm Gun [TARGET PLAYER]", "World", "Storm splashes around target from any distance.", false, 17, ModType.Toggle, false)]
internal class SplashStormGun : MonoBehaviour
{
	private const string GunId = "SplashStormGun";
	private const string SplashRpc = "RPC_PlaySplashEffect";

	private Vector3 _originalPosition;

	private bool _isRigSpoofed;

	private void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			RestoreLocalRig();
			GunController.Release(GunId);
			return;
		}
		GunController.GunResult gunResult = GunController.GetGunResult(GunId, targetPlayers: true, 0.15f, allowSingleTargetLock: true);
		if (!gunResult.IsActive && gunResult.LockedTarget == null)
		{
			RestoreLocalRig();
			return;
		}
		Player player = gunResult.LockedTarget;
		if (player == null)
		{
			RestoreLocalRig();
			return;
		}
		VRRig targetRig = RigUtilities.GetRig(player);
		if (targetRig == null)
		{
			RestoreLocalRig();
			return;
		}
		if (!_isRigSpoofed)
		{
			_originalPosition = GorillaTagger.Instance.offlineVRRig.transform.position;
			GorillaTagger.Instance.offlineVRRig.enabled = false;
			_isRigSpoofed = true;
		}
		if (gunResult.CanFire)
		{
			GunController.MarkFired(GunId);
			for (int i = 0; i < 3; i++)
			{
				Vector3 splashPosition = targetRig.transform.position + Random.insideUnitSphere * 3f;
				float splashScale = Random.Range(1f, 3f);
				GorillaTagger.Instance.offlineVRRig.transform.position = splashPosition;
				PhotonNetwork.SendAllOutgoingCommands();
				GorillaTagger.Instance.myVRRig.GetView.SendRpc(
					SplashRpc, RpcTarget.All, splashPosition, Quaternion.identity, splashScale, 0.4f, true, true);
				GameNetworkUtilities.FlushAndReplayNetworkTraffic();
			}
		}
	}

	private void RestoreLocalRig()
	{
		if (_isRigSpoofed)
		{
			GorillaTagger.Instance.offlineVRRig.transform.position = _originalPosition;
			GorillaTagger.Instance.offlineVRRig.enabled = true;
			_isRigSpoofed = false;
		}
	}

	private void OnDisable()
	{
		RestoreLocalRig();
		GunController.Release(GunId);
	}

	private void OnDestroy()
	{
		RestoreLocalRig();
		GunController.Release(GunId);
	}
}
