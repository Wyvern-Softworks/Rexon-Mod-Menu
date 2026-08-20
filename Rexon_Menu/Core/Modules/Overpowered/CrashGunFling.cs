// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Overpowered.CrashGunFling
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Photon.Pun;
using Recovered.Obfuscated;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.Overpowered;

[Mod("Crash Gun [TARGET GRIP]", "Overpowered", "Flings target into quit box. [TARGET MUST GRIP]", false, 49, ModType.Toggle, false)]
internal class CrashGunFling : MonoBehaviour
{
	private const string GunId = "CrashGunFling";
	private static readonly Vector3 CrashDirection = new Vector3(-1f, -1f, -1f).normalized;

	private enum State
	{
		Idle,
		AcquireGrip,
		Launch,
		HoldBeforeLaunch,
		Complete
	}

	private State _state;

	private int _remainingFrames;

	private float _launchDelayUntil;

	private NetPlayer _targetPlayer;

	private VRRig _targetRig;

	private bool _targetLeftHand;

	private Vector3 _anchorPosition;

	private Vector3 _launchDirection;

	private void Update()
	{
		GameNetworkUtilities.TentacleHelper.FrameCounter++;
		if (!PhotonNetwork.InRoom)
		{
			ResetGun();
			return;
		}
		GunController.GunResult gunResult = GunController.GetGunResult(GunId, targetPlayers: true, 0f, allowSingleTargetLock: true);
		if (gunResult.LockedTarget == null)
		{
			if (_targetPlayer != null)
			{
				ResetTarget();
			}
			return;
		}
		NetPlayer selectedPlayer = NetPlayer.Get(gunResult.LockedTarget);
		if (selectedPlayer == null)
		{
			ResetTarget();
			return;
		}
		if (_targetPlayer == null)
		{
			_targetPlayer = selectedPlayer;
			_targetRig = RigUtilities.GetRig(_targetPlayer);
			if (_targetRig == null)
			{
				_targetPlayer = null;
				return;
			}
			GameNetworkUtilities.DisablePhotonRateLimitsAndFlush();
			GameNetworkUtilities.SetPhotonSerializeTickMultiplier(9999f);
			GameNetworkUtilities.TentacleHelper.IsActive = true;
			_anchorPosition = _targetRig.transform.position;
			bool targetIsDistant = GameNetworkUtilities.TentacleHelper.IsAtLeastThreeMetersAway(
				Vector3.Distance(GorillaTagger.Instance.offlineVRRig.transform.position, _targetRig.transform.position));
			_state = targetIsDistant ? State.AcquireGrip : State.Launch;
			_remainingFrames = targetIsDistant ? 1 : 0;
		}
		if (_targetPlayer == null || !_targetPlayer.InRoom || _targetRig == null)
		{
			ResetTarget();
			GunController.ClearLockedTarget(GunId);
			return;
		}
		VRRig localRig = GorillaTagger.Instance.offlineVRRig;
		switch (_state)
		{
		case State.Complete:
			break;
		case State.Idle:
			GameNetworkUtilities.TentacleHelper.ResetTentacleGrab(localRig);
			if (GameNetworkUtilities.TentacleHelper.ShouldRunTentacleUpdate())
			{
				GameNetworkUtilities.TentacleHelper.SendRigAtPosition(localRig, _anchorPosition);
			}
			if (--_remainingFrames <= 0)
			{
				_state = State.Launch;
			}
			break;
		case State.AcquireGrip:
			if (GameNetworkUtilities.TentacleHelper.TryGetActiveHands(_targetRig, out _, out _))
			{
				_targetLeftHand = GameNetworkUtilities.TentacleHelper.ChooseLeftTargetHand(_targetRig);
				_anchorPosition = _targetRig.transform.position;
				_launchDirection = CrashDirection;
				GameNetworkUtilities.TentacleHelper.ConfigureTentacleGrab(localRig, _targetPlayer, _targetRig, _targetLeftHand);
				GameNetworkUtilities.TentacleHelper.SendRigAtPosition(localRig, _anchorPosition, flushImmediately: true);
				_state = State.HoldBeforeLaunch;
				_remainingFrames = 15;
			}
			else if (GameNetworkUtilities.TentacleHelper.ShouldRunTentacleUpdate())
			{
				GameNetworkUtilities.TentacleHelper.ResetTentacleGrab(localRig);
				_anchorPosition = _targetRig.transform.position;
				GameNetworkUtilities.TentacleHelper.SendRigAtPosition(localRig, _anchorPosition);
			}
			break;
		case State.Launch:
			GameNetworkUtilities.TentacleHelper.ConfigureTentacleGrab(localRig, _targetPlayer, _targetRig, _targetLeftHand);
			GameNetworkUtilities.TentacleHelper.SendRigAtPosition(localRig, _anchorPosition + _launchDirection * 3500f, _remainingFrames <= 1);
			if (--_remainingFrames <= 0)
			{
				GameNetworkUtilities.TentacleHelper.ResetTentacleGrab(localRig);
				GameNetworkUtilities.TentacleHelper.SendRigAtPosition(localRig, _anchorPosition + _launchDirection * 3500f, flushImmediately: true);
				_launchDelayUntil = Time.time + 0.5f;
				_state = State.Complete;
			}
			break;
		case State.HoldBeforeLaunch:
			if (Time.time >= _launchDelayUntil)
			{
				_anchorPosition = _targetRig.transform.position;
				_state = State.Launch;
			}
			break;
		}
	}

	private void ResetTarget()
	{
		if (_targetPlayer != null)
		{
			VRRig localRig = GorillaTagger.Instance.offlineVRRig;
			GameNetworkUtilities.TentacleHelper.ResetTentacleGrab(localRig);
			GameNetworkUtilities.TentacleHelper.SendRigAtPosition(localRig, localRig.transform.position, flushImmediately: true);
			GameNetworkUtilities.SetPhotonSerializeTickMultiplier(1000f);
			GameNetworkUtilities.TentacleHelper.IsActive = false;
		}
		_targetPlayer = null;
		_targetRig = null;
		_state = State.Idle;
	}

	private void ResetGun()
	{
		ResetTarget();
		GunController.Release(GunId);
	}

	private void OnDisable()
	{
		ResetGun();
	}

	private void OnDestroy()
	{
		ResetGun();
	}

}
