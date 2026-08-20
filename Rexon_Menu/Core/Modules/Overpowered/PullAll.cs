// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Overpowered.PullAll
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Collections.Generic;
using GorillaLocomotion;
using Photon.Pun;
using Recovered.Obfuscated;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

using Object = UnityEngine.Object;

namespace Rexon_Menu.Core.Modules.Overpowered;

[Mod("Pull All [TARGET GRIP]", "Overpowered", "Pulls all gripping players towards you. [TARGET MUST GRIP]", false, 52, ModType.Toggle, false)]
internal class PullAll : MonoBehaviour
{
	private readonly List<VRRig> _candidateRigs = new List<VRRig>();

	private bool _initialized;
	private float _targetScanCooldown;
	private bool _isPulling;
	private int _framesRemaining;
	private VRRig _targetRig;
	private NetPlayer _targetPlayer;
	private bool _targetLeftHand;
	private Vector3 _pullOrigin;
	private float _pullDistance;
	private int _selectionIndex;

	private void Update()
	{
		GameNetworkUtilities.TentacleHelper.FrameCounter++;
		if (!PhotonNetwork.InRoom)
		{
			ResetPullState();
			return;
		}
		if (!_initialized)
		{
			GameNetworkUtilities.SetPhotonSerializeTickMultiplier(9999f);
			GameNetworkUtilities.TentacleHelper.IsActive = true;
			_initialized = true;
			_targetScanCooldown = 0f;
		}

		VRRig localRig = GorillaTagger.Instance.offlineVRRig;
		Vector3 localPosition = GTPlayer.Instance.transform.position;
		if (_isPulling
			&& _targetRig != null
			&& _targetPlayer != null
			&& _targetPlayer.InRoom)
		{
			UpdateActivePull(localRig, localPosition);
			return;
		}

		_isPulling = false;
		GameNetworkUtilities.TentacleHelper.ResetTentacleGrab(localRig);
		_targetScanCooldown -= Time.deltaTime;
		if (_targetScanCooldown > 0f)
		{
			return;
		}
		_targetScanCooldown = 0.5f;
		SelectNextTarget(localRig);
	}

	private void UpdateActivePull(VRRig localRig, Vector3 localPosition)
	{
		if (!GameNetworkUtilities.TentacleHelper.ShouldRunTentacleUpdate())
		{
			return;
		}

		GameNetworkUtilities.TentacleHelper.ConfigureTentacleGrab(
			localRig, _targetPlayer, _targetRig, _targetLeftHand);
		Vector3 pullVector = localPosition - _targetRig.transform.position;
		float distanceToLocalPlayer = pullVector.magnitude;
		_pullDistance = Mathf.Min(
			_pullDistance + 8f * Time.deltaTime,
			Mathf.Max(distanceToLocalPlayer - 0.5f, 0f));
		Vector3 direction = distanceToLocalPlayer > 0.1f
			? pullVector / distanceToLocalPlayer
			: Vector3.forward;
		GameNetworkUtilities.TentacleHelper.SendRigAtPosition(
			localRig, _pullOrigin + direction * _pullDistance);

		_framesRemaining--;
		if (_framesRemaining > 0
			&& GameNetworkUtilities.TentacleHelper.TryGetActiveHands(_targetRig, out _, out _))
		{
			return;
		}

		GameNetworkUtilities.TentacleHelper.ResetTentacleGrab(localRig);
		GameNetworkUtilities.TentacleHelper.SendRigAtPosition(localRig, _pullOrigin);
		_isPulling = false;
		_targetRig = null;
		_targetPlayer = null;
	}

	private void SelectNextTarget(VRRig localRig)
	{
		_candidateRigs.Clear();
		foreach (VRRig rig in VRRigCache.ActiveRigs)
		{
			if (!rig.isLocal
				&& rig.OwningNetPlayer != null
				&& GameNetworkUtilities.TentacleHelper.TryGetActiveHands(rig, out _, out _))
			{
				_candidateRigs.Add(rig);
			}
		}
		if (_candidateRigs.Count == 0)
		{
			return;
		}

		_selectionIndex = (_selectionIndex + 1) % _candidateRigs.Count;
		VRRig selectedRig = _candidateRigs[_selectionIndex];
		if (selectedRig.OwningNetPlayer == null)
		{
			return;
		}

		_targetRig = selectedRig;
		_targetPlayer = selectedRig.OwningNetPlayer;
		_targetLeftHand = GameNetworkUtilities.TentacleHelper.ChooseLeftTargetHand(selectedRig);
		_pullOrigin = GameNetworkUtilities.TentacleHelper.OffsetPullPosition(selectedRig.transform.position);
		_pullDistance = 0f;
		GameNetworkUtilities.TentacleHelper.ConfigureTentacleGrab(
			localRig, _targetPlayer, _targetRig, _targetLeftHand);
		GameNetworkUtilities.TentacleHelper.SendRigAtPosition(localRig, _pullOrigin, flushImmediately: true);
		_isPulling = true;
		_framesRemaining = 30;
	}

	private void ResetPullState()
	{
		if (_initialized)
		{
			VRRig localRig = GorillaTagger.Instance.offlineVRRig;
			GameNetworkUtilities.TentacleHelper.ResetTentacleGrab(localRig);
			GameNetworkUtilities.TentacleHelper.SendRigAtPosition(
				localRig, localRig.transform.position, flushImmediately: true);
			GameNetworkUtilities.SetPhotonSerializeTickMultiplier(1000f);
			GameNetworkUtilities.TentacleHelper.IsActive = false;
		}
		_initialized = false;
		_isPulling = false;
		_targetRig = null;
		_targetPlayer = null;
		_candidateRigs.Clear();
	}

	private void OnDisable()
	{
		ResetPullState();
	}

	private void OnDestroy()
	{
		ResetPullState();
	}
}
