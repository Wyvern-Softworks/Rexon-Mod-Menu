// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Overpowered.FlingAll
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Photon.Pun;
using Recovered.Obfuscated;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.Overpowered;

[Mod("Fling All [TARGET GRIP]", "Overpowered", "Launches all gripping players. [TARGET MUST GRIP]", false, 48, ModType.Toggle, false)]
internal class FlingAll : MonoBehaviour
{
	private const float ActiveSerializeTickMultiplier = 9999f;
	private const float DefaultSerializeTickMultiplier = 1000f;
	private const float FlingDistance = 1774f;
	private const int FlingFrameCount = 12;
	private const int CleanupFrameCount = 3;

	private static readonly Vector3 FlingDirection = (-Vector3.one).normalized;

	private bool _networkStateConfigured;
	private VRRig _targetRig;
	private NetPlayer _targetPlayer;
	private bool _targetLeftHand;
	private Vector3 _targetOrigin;
	private int _flingFramesRemaining;
	private int _cleanupFramesRemaining;

	private void Update()
	{
		GameNetworkUtilities.TentacleHelper.FrameCounter++;
		if (!PhotonNetwork.InRoom)
		{
			ResetFlingState();
			return;
		}

		EnsureNetworkStateConfigured();
		VRRig localRig = GorillaTagger.Instance.offlineVRRig;
		if (_cleanupFramesRemaining > 0)
		{
			GameNetworkUtilities.TentacleHelper.ResetTentacleGrab(localRig);
			GameNetworkUtilities.TentacleHelper.SendRigAtPosition(localRig, localRig.transform.position);
			_cleanupFramesRemaining--;
			return;
		}

		if (_flingFramesRemaining > 0 &&
			_targetRig != null &&
			_targetPlayer != null &&
			_targetPlayer.InRoom)
		{
			ContinueFling(localRig);
			return;
		}

		_flingFramesRemaining = 0;
		TryStartNextFling(localRig);
	}

	private void EnsureNetworkStateConfigured()
	{
		if (_networkStateConfigured)
		{
			return;
		}

		GameNetworkUtilities.SetPhotonSerializeTickMultiplier(ActiveSerializeTickMultiplier);
		GameNetworkUtilities.TentacleHelper.IsActive = true;
		_networkStateConfigured = true;
	}

	private void ContinueFling(VRRig localRig)
	{
		GameNetworkUtilities.TentacleHelper.ConfigureTentacleGrab(localRig, _targetPlayer, _targetRig, _targetLeftHand);
		Vector3 flingPosition = _targetOrigin + FlingDirection * FlingDistance;
		GameNetworkUtilities.TentacleHelper.SendRigAtPosition(localRig, flingPosition, _flingFramesRemaining <= 1);
		_flingFramesRemaining--;

		if (_flingFramesRemaining > 0)
		{
			return;
		}

		GameNetworkUtilities.TentacleHelper.ResetTentacleGrab(localRig);
		GameNetworkUtilities.TentacleHelper.SendRigAtPosition(localRig, flingPosition, flushImmediately: true);
		_targetRig = null;
		_targetPlayer = null;
		_cleanupFramesRemaining = CleanupFrameCount;
	}

	private void TryStartNextFling(VRRig localRig)
	{
		foreach (VRRig rig in VRRigCache.ActiveRigs)
		{
			if (rig.isLocal ||
				rig.OwningNetPlayer == null ||
				!GameNetworkUtilities.TentacleHelper.TryGetActiveHands(rig, out _, out _))
			{
				continue;
			}

			_targetRig = rig;
			_targetPlayer = rig.OwningNetPlayer;
			_targetLeftHand = GameNetworkUtilities.TentacleHelper.ChooseLeftTargetHand(rig);
			_targetOrigin = rig.transform.position;
			GameNetworkUtilities.TentacleHelper.ConfigureTentacleGrab(localRig, _targetPlayer, _targetRig, _targetLeftHand);
			GameNetworkUtilities.TentacleHelper.SendRigAtPosition(localRig, _targetOrigin, flushImmediately: true);
			_flingFramesRemaining = FlingFrameCount;
			break;
		}
	}

	private void ResetFlingState()
	{
		if (_networkStateConfigured)
		{
			VRRig localRig = GorillaTagger.Instance.offlineVRRig;
			GameNetworkUtilities.TentacleHelper.ResetTentacleGrab(localRig);
			GameNetworkUtilities.TentacleHelper.SendRigAtPosition(
				localRig,
				localRig.transform.position,
				flushImmediately: true);
			GameNetworkUtilities.SetPhotonSerializeTickMultiplier(DefaultSerializeTickMultiplier);
			GameNetworkUtilities.TentacleHelper.IsActive = false;
		}

		_networkStateConfigured = false;
		_flingFramesRemaining = 0;
		_cleanupFramesRemaining = 0;
		_targetRig = null;
		_targetPlayer = null;
	}

	private void OnDisable()
	{
		ResetFlingState();
	}

	private void OnDestroy()
	{
		ResetFlingState();
	}
}
