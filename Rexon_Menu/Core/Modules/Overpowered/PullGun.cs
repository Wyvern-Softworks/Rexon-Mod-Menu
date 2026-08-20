// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Overpowered.PullGun
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using GorillaLocomotion;
using Photon.Pun;
using Recovered.Obfuscated;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.Overpowered;

[Mod("Pull Gun [TARGET GRIP]", "Overpowered", "Moves target towards you. [TARGET MUST GRIP]", false, 51, ModType.Toggle, false)]
internal class PullGun : MonoBehaviour
{
	private const string GunId = "PullGun";

	private enum State
	{
		SendAnchorPosition,
		ConfirmGrab,
		PullTarget,
		ResetGrab,
		Complete
	}

	private State _state;

	private int _remainingFrames;

	private NetPlayer _targetPlayer;

	private VRRig _targetRig;

	private bool _targetLeftHand;

	private Vector3 _anchorPosition;

	private float _pullDistance;

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
		if (_targetPlayer == null || _targetPlayer.ActorNumber != selectedPlayer.ActorNumber)
		{
			ResetTarget();
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
			_anchorPosition = GameNetworkUtilities.TentacleHelper.OffsetPullPosition(_targetRig.transform.position);
			_pullDistance = 0f;
			bool targetIsDistant = GameNetworkUtilities.TentacleHelper.IsAtLeastThreeMetersAway(
				Vector3.Distance(GorillaTagger.Instance.offlineVRRig.transform.position, _targetRig.transform.position));
			_state = targetIsDistant ? State.ConfirmGrab : State.SendAnchorPosition;
			_remainingFrames = targetIsDistant ? 1 : 0;
		}
		if (_targetPlayer == null || !_targetPlayer.InRoom || _targetRig == null)
		{
			ResetTarget();
			GunController.ClearLockedTarget(GunId);
		}
		else
		{
			if (!GameNetworkUtilities.TentacleHelper.ShouldRunTentacleUpdate())
			{
				return;
			}
			VRRig localRig = GorillaTagger.Instance.offlineVRRig;
			switch (_state)
			{
			case State.Complete:
				break;
			case State.SendAnchorPosition:
				GameNetworkUtilities.TentacleHelper.ResetTentacleGrab(localRig);
				GameNetworkUtilities.TentacleHelper.SendRigAtPosition(localRig, _anchorPosition);
				if (--_remainingFrames <= 0)
				{
					_state = State.SendAnchorPosition;
				}
				break;
			default:
				if (GameNetworkUtilities.TentacleHelper.TryGetActiveHands(_targetRig, out _, out _))
				{
					_targetLeftHand = GameNetworkUtilities.TentacleHelper.ChooseLeftTargetHand(_targetRig);
					_anchorPosition = GameNetworkUtilities.TentacleHelper.OffsetPullPosition(_targetRig.transform.position);
					GameNetworkUtilities.TentacleHelper.ConfigureTentacleGrab(localRig, _targetPlayer, _targetRig, _targetLeftHand);
					_state = State.PullTarget;
					_remainingFrames = 3;
					_pullDistance = 0f;
				}
				else
				{
					GameNetworkUtilities.TentacleHelper.ResetTentacleGrab(localRig);
					_anchorPosition = GameNetworkUtilities.TentacleHelper.OffsetPullPosition(_targetRig.transform.position);
					GameNetworkUtilities.TentacleHelper.SendRigAtPosition(localRig, _anchorPosition);
				}
				break;
			case State.ConfirmGrab:
				GameNetworkUtilities.TentacleHelper.ConfigureTentacleGrab(localRig, _targetPlayer, _targetRig, _targetLeftHand);
				GameNetworkUtilities.TentacleHelper.SendRigAtPosition(localRig, _anchorPosition, flushImmediately: true);
				if (--_remainingFrames <= 0)
				{
					_pullDistance = 0f;
					_state = State.ResetGrab;
				}
				break;
			case State.PullTarget:
			{
				if (!GameNetworkUtilities.TentacleHelper.TryGetActiveHands(_targetRig, out _, out _))
				{
					_state = State.Complete;
					_remainingFrames = 2;
					break;
				}
				GameNetworkUtilities.TentacleHelper.ConfigureTentacleGrab(localRig, _targetPlayer, _targetRig, _targetLeftHand);
				Vector3 targetPosition = _targetRig.transform.position;
				Vector3 pullVector = GTPlayer.Instance.transform.position - targetPosition;
				float distanceToLocalPlayer = pullVector.magnitude;
				_pullDistance += 8f * Time.deltaTime;
				_pullDistance = Mathf.Min(_pullDistance, Mathf.Max(distanceToLocalPlayer - 0.5f, 0f));
				Vector3 pullDirection = distanceToLocalPlayer > 0.1f
					? pullVector / distanceToLocalPlayer
					: Vector3.forward;
				GameNetworkUtilities.TentacleHelper.SendRigAtPosition(localRig, _anchorPosition + pullDirection * _pullDistance);
				break;
			}
			case State.ResetGrab:
				GameNetworkUtilities.TentacleHelper.ResetTentacleGrab(localRig);
				GameNetworkUtilities.TentacleHelper.SendRigAtPosition(localRig, _anchorPosition);
				if (--_remainingFrames <= 0)
				{
					_anchorPosition = GameNetworkUtilities.TentacleHelper.OffsetPullPosition(_targetRig.transform.position);
					_pullDistance = 0f;
					_state = State.SendAnchorPosition;
				}
				break;
			}
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
		_state = State.SendAnchorPosition;
		_pullDistance = 0f;
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
