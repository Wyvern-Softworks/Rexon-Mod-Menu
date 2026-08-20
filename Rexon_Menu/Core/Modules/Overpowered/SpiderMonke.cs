// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Overpowered.SpiderMonke
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System;
using System.Collections.Generic;
using System.Reflection;
using GorillaLocomotion;
using Photon.Pun;
using Recovered.Obfuscated;
using Rexon_Menu.Core.Attributes;
using UnityEngine;
using Object = UnityEngine.Object;
using PositionState = TransferrableObject.PositionState;

namespace Rexon_Menu.Core.Modules.Overpowered;

[Mod("Spider Monke [GRIPS] [BALLOON]", "Overpowered", "Grapple to surfaces with balloon strings. Server sided. Needs balloon.", false, 36, ModType.Toggle, false)]
internal class SpiderMonke : MonoBehaviour
{
	private const float BalloonRefreshIntervalSeconds = 3f;
	private const float MaximumRayDistance = 100f;
	private const float MaximumAimAngle = 30f;
	private const float MinimumGrappleDistance = 2f;
	private const float GrappleLaunchVelocity = 12f;

	private readonly List<BalloonHoldable> _balloons = new List<BalloonHoldable>();

	private float _lastBalloonRefresh;

	private bool _leftGrappleAttached;

	private bool _rightGrappleAttached;

	private Vector3 _leftAnchor;

	private Vector3 _rightAnchor;

	private SpringJoint _leftSpring;

	private SpringJoint _rightSpring;

	private Vector3 _velocityBeforeGrapple;

	private bool _releaseHandled = true;

	private void OnDisable()
	{
		ResetGrapples();
	}

	private void OnDestroy()
	{
		ResetGrapples();
	}

	private void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			return;
		}

		try
		{
			LayerMask grappleMask = (LayerMask)GameNetworkUtilities.GetGameplayLayerMask();
			bool leftGrip = ControllerInputPoller.instance.leftGrab;
			bool rightGrip = ControllerInputPoller.instance.rightGrab;
			Rigidbody playerBody = GTPlayer.Instance.GetComponent<Rigidbody>();
			if (playerBody == null)
			{
				return;
			}

			if (!_leftGrappleAttached && !_rightGrappleAttached)
			{
				_velocityBeforeGrapple = playerBody.velocity;
			}

			RefreshBalloonCacheIfNeeded();

			if (leftGrip || rightGrip)
			{
				_releaseHandled = false;
				playerBody.useGravity = false;
				playerBody.AddForce(Physics.gravity * 0.5f * playerBody.mass, ForceMode.Force);

				if (leftGrip && !_leftGrappleAttached)
				{
					TryAttachGrapple(
						GorillaTagger.Instance.leftHandTransform,
						grappleMask,
						playerBody,
						ref _leftAnchor,
						ref _leftSpring,
						ref _leftGrappleAttached);
				}

				if (rightGrip && !_rightGrappleAttached)
				{
					TryAttachGrapple(
						GorillaTagger.Instance.rightHandTransform,
						grappleMask,
						playerBody,
						ref _rightAnchor,
						ref _rightSpring,
						ref _rightGrappleAttached);
				}

				ConfigureOwnedBalloonsForGrappling();
			}
			else
			{
				_leftAnchor = Vector3.zero;
				_rightAnchor = Vector3.zero;
				if (!_releaseHandled)
				{
					_releaseHandled = true;
					playerBody.useGravity = true;
					ResetOwnedBalloons();
				}
			}

			if (!leftGrip)
			{
				DetachGrapple(ref _leftSpring, ref _leftGrappleAttached);
			}

			if (!rightGrip)
			{
				DetachGrapple(ref _rightSpring, ref _rightGrappleAttached);
			}
		}
		catch (Exception)
		{
		}
	}

	private void RefreshBalloonCacheIfNeeded()
	{
		if (Time.time <= _lastBalloonRefresh + BalloonRefreshIntervalSeconds)
		{
			return;
		}

		_lastBalloonRefresh = Time.time;
		foreach (BalloonHoldable balloon in Object.FindObjectsOfType<BalloonHoldable>())
		{
			if (balloon != null && !_balloons.Contains(balloon))
			{
				_balloons.Add(balloon);
			}
		}
	}

	private void TryAttachGrapple(
		Transform hand,
		LayerMask grappleMask,
		Rigidbody playerBody,
		ref Vector3 anchor,
		ref SpringJoint spring,
		ref bool attached)
	{
		Vector3 aimDirection = -hand.up + hand.forward * 0.1f;
		if (!Physics.Raycast(hand.position, aimDirection, out RaycastHit hit, MaximumRayDistance, grappleMask))
		{
			return;
		}

		Vector3 directionToHit = (hit.point - hand.position).normalized;
		if (Vector3.Angle(aimDirection, directionToHit) > MaximumAimAngle)
		{
			return;
		}

		float distance = Vector3.Distance(GTPlayer.Instance.transform.position, hit.point);
		if (distance <= MinimumGrappleDistance)
		{
			return;
		}

		anchor = hit.point;
		spring = GorillaTagger.Instance.gameObject.AddComponent<SpringJoint>();
		spring.autoConfigureConnectedAnchor = false;
		spring.connectedAnchor = anchor;
		spring.maxDistance = distance * 0.3f;
		spring.minDistance = distance * 0.1f;
		spring.spring = 20f;
		spring.damper = 20f;
		spring.massScale = 12f;
		attached = true;

		playerBody.velocity = _velocityBeforeGrapple;
		playerBody.AddForce(
			(anchor - GTPlayer.Instance.transform.position).normalized * GrappleLaunchVelocity,
			ForceMode.VelocityChange);
	}

	private void ConfigureOwnedBalloonsForGrappling()
	{
		foreach (BalloonHoldable balloon in _balloons)
		{
			if (balloon == null || balloon.ownerRig != GorillaTagger.Instance.offlineVRRig)
			{
				continue;
			}

			EnsureLocalOwnership(balloon);
			try
			{
				FieldInfo dynamicsField = typeof(BalloonHoldable).GetField(
					"balloonDynamics",
					BindingFlags.Instance | BindingFlags.NonPublic);
				BalloonDynamics dynamics = dynamicsField?.GetValue(balloon) as BalloonDynamics;
				if (dynamics == null)
				{
					continue;
				}

				dynamics.EnableDistanceConstraints(false, 1f);
				dynamics.bouyancyForce = 0f;
				dynamics.bouyancyMaxHeight = 0f;
				dynamics.bouyancyMinHeight = 0f;
				dynamics.bopSpeed = 0f;
				dynamics.airResistance = 0f;
				dynamics.maximumVelocity = float.MaxValue;
				dynamics.stringLength = float.MaxValue;
				dynamics.stringStrength = float.MaxValue;
				dynamics.stringStretch = float.MaxValue;
				dynamics.varianceMaxheight = float.MaxValue;
			}
			catch (Exception)
			{
			}
		}
	}

	private void ResetOwnedBalloons()
	{
		foreach (BalloonHoldable balloon in _balloons)
		{
			if (balloon == null)
			{
				continue;
			}

			balloon.WorldShareableRequestOwnership();
			EnsureLocalOwnership(balloon);
			if (balloon.ownerRig != GorillaTagger.Instance.offlineVRRig)
			{
				continue;
			}

			try
			{
				TransferrableObject transferable = balloon.GetComponent<TransferrableObject>();
				if (transferable != null)
				{
					transferable.currentState = (PositionState)0;
				}
			}
			catch (Exception)
			{
			}
		}
	}

	private static void EnsureLocalOwnership(BalloonHoldable balloon)
	{
		PhotonView view = balloon.GetComponent<PhotonView>();
		if (view == null || view.IsMine || view.Owner == PhotonNetwork.LocalPlayer)
		{
			return;
		}

		view.RequestOwnership();
		view.OwnerActorNr = PhotonNetwork.LocalPlayer.ActorNumber;
		view.ControllerActorNr = PhotonNetwork.LocalPlayer.ActorNumber;
	}

	private static void DetachGrapple(ref SpringJoint spring, ref bool attached)
	{
		attached = false;
		if (spring != null)
		{
			Object.Destroy(spring);
			spring = null;
		}
	}

	private void ResetGrapples()
	{
		DetachGrapple(ref _leftSpring, ref _leftGrappleAttached);
		DetachGrapple(ref _rightSpring, ref _rightGrappleAttached);
		_leftAnchor = Vector3.zero;
		_rightAnchor = Vector3.zero;
		_releaseHandled = true;

		Rigidbody playerBody = GTPlayer.Instance?.GetComponent<Rigidbody>();
		if (playerBody != null)
		{
			playerBody.useGravity = true;
		}
	}
}
