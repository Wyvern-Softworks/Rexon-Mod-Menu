// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.World.SplashOrbitGun
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Photon.Pun;
using Photon.Realtime;
using Recovered.Obfuscated;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.World;

[Mod("Splash Orbit Gun [TARGET PLAYER]", "World", "Orbit splashes tight around target.", false, 16, ModType.Toggle, false)]
internal class SplashOrbitGun : MonoBehaviour
{
	private const string GunId = "SplashOrbitGun";
	private const string SplashRpc = "RPC_PlaySplashEffect";
	private const float OrbitSpeedDegrees = 200f;
	private const float OrbitRadius = 0.8f;
	private const float MaximumSpoofDistance = 3f;

	private float _orbitAngleDegrees;
	private Vector3 _originalRigPosition;
	private bool _localRigIsSpoofed;

	private void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			RestoreLocalRig();
			GunController.Release(GunId);
			return;
		}

		GunController.GunResult gun = GunController.GetGunResult(
			GunId,
			targetPlayers: true,
			0f,
			allowSingleTargetLock: true);
		if (!gun.IsActive && gun.LockedTarget == null)
		{
			RestoreLocalRig();
			return;
		}

		Player targetPlayer = gun.LockedTarget;
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

		_orbitAngleDegrees += Time.deltaTime * OrbitSpeedDegrees;
		float angleRadians = _orbitAngleDegrees * Mathf.Deg2Rad;
		Vector3 orbitOffset = new(
			Mathf.Sin(angleRadians) * OrbitRadius,
			1f,
			Mathf.Cos(angleRadians) * OrbitRadius);
		Vector3 splashPosition = targetRig.transform.position + orbitOffset;
		Vector3 distanceOrigin = _localRigIsSpoofed
			? _originalRigPosition
			: GorillaTagger.Instance.offlineVRRig.transform.position;
		if (Vector3.Distance(distanceOrigin, splashPosition) > MaximumSpoofDistance)
		{
			return;
		}

		if (!_localRigIsSpoofed)
		{
			_originalRigPosition = GorillaTagger.Instance.offlineVRRig.transform.position;
			GorillaTagger.Instance.offlineVRRig.enabled = false;
			_localRigIsSpoofed = true;
		}

		GorillaTagger.Instance.offlineVRRig.transform.position = splashPosition;
		PhotonNetwork.SendAllOutgoingCommands();
		GorillaTagger.Instance.myVRRig.GetView.SendRpc(
			SplashRpc,
			RpcTarget.All,
			splashPosition,
			Quaternion.identity,
			1.5f,
			0.3f,
			false,
			true);
		GameNetworkUtilities.FlushAndReplayNetworkTraffic();
	}

	private void RestoreLocalRig()
	{
		if (!_localRigIsSpoofed)
		{
			return;
		}

		GorillaTagger.Instance.offlineVRRig.transform.position = _originalRigPosition;
		GorillaTagger.Instance.offlineVRRig.enabled = true;
		_localRigIsSpoofed = false;
	}

	private void OnDisable()
	{
		_orbitAngleDegrees = 0f;
		RestoreLocalRig();
		GunController.Release(GunId);
	}

	private void OnDestroy()
	{
		RestoreLocalRig();
		GunController.Release(GunId);
	}
}
