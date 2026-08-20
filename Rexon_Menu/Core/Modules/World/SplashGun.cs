// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.SplashGun
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System;
using Photon.Pun;
using Photon.Realtime;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

using Object = UnityEngine.Object;

using Random = UnityEngine.Random;

namespace Recovered.Obfuscated;

[Mod("Splash Gun [TARGET PLAYER]", "World", "Lock a player and splash them.", false, 15, ModType.Toggle, false)]
internal class SplashGun : MonoBehaviour
{
	private const string GunId = "SplashGun";
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
		GunController.GunResult gunResult = GunController.GetGunResult(GunId, targetPlayers: true, 0.1f, allowSingleTargetLock: true);
		if (!gunResult.IsActive && gunResult.LockedTarget == null)
		{
			RestoreLocalRig();
			return;
		}
		Player targetPlayer = gunResult.LockedTarget;
		if (targetPlayer == null)
		{
			RestoreLocalRig();
			return;
		}
		VRRig targetRig = RigUtilities.GetRig(targetPlayer);
		if (targetRig == null)
		{
			RestoreLocalRig();
			return;
		}
		Vector3 localPosition = _isRigSpoofed
			? _originalPosition
			: GorillaTagger.Instance.offlineVRRig.transform.position;
		if (Vector3.Distance(localPosition, targetRig.transform.position) <= 3f)
		{
			if (!_isRigSpoofed)
			{
				_originalPosition = GorillaTagger.Instance.offlineVRRig.transform.position;
				GorillaTagger.Instance.offlineVRRig.enabled = false;
				_isRigSpoofed = true;
			}
			if (gunResult.CanFire)
			{
				GunController.MarkFired(GunId);
				Vector3 splashPosition = targetRig.transform.position + Random.insideUnitSphere * 1.5f;
				GorillaTagger.Instance.offlineVRRig.transform.position = splashPosition;
				PhotonNetwork.SendAllOutgoingCommands();
				GorillaTagger.Instance.myVRRig.GetView.SendRpc(
					SplashRpc, RpcTarget.All, splashPosition, Quaternion.identity, 2f, 0.4f, true, true);
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
}
