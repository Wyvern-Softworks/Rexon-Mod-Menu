// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Overpowered.TracerDisk
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System;
using System.Collections.Generic;
using System.Reflection;
using GorillaLocomotion;
using Photon.Pun;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

using Object = UnityEngine.Object;

namespace Rexon_Menu.Core.Modules.Overpowered;

[Mod("Tracer Disk [RG] [BALLOON]", "Overpowered", "Disk orbit with balloon strings. Server sided. Needs balloon.", false, 35, ModType.Toggle, false)]
internal class TracerDisk : MonoBehaviour
{
	private const float CacheRefreshSeconds = 3f;
	private const float OrbitRadius = 100f;
	private const float OrbitHeight = 5f;
	private const float CollisionScaleFactor = 10000f;

	private static readonly FieldInfo BalloonDynamicsField = typeof(BalloonHoldable).GetField(
		"balloonDynamics",
		BindingFlags.Instance | BindingFlags.NonPublic);

	private readonly List<BalloonHoldable> _balloons = new List<BalloonHoldable>();
	private float _lastCacheRefresh;
	private Vector3 _savedPlayerPosition;
	private bool _worldCollisionReduced;

	private void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			return;
		}

		RefreshBalloonCache();
		if (!ControllerInputPoller.instance.rightGrab)
		{
			return;
		}

		foreach (BalloonHoldable balloon in _balloons)
		{
			if (balloon == null || balloon.ownerRig != GorillaTagger.Instance.offlineVRRig)
			{
				continue;
			}

			if (_savedPlayerPosition == Vector3.zero)
			{
				_savedPlayerPosition = GTPlayer.Instance.transform.position;
			}

			EnsureLocalOwnership(balloon);
			TryDisableDistanceConstraints(balloon);

			float angle = Time.time;
			float x = Mathf.Cos(angle) * OrbitRadius;
			float z = Mathf.Sin(angle) * OrbitRadius;
			balloon.transform.position = _savedPlayerPosition + new Vector3(x, OrbitHeight, z);

			if (!_worldCollisionReduced)
			{
				foreach (MeshCollider collider in Resources.FindObjectsOfTypeAll<MeshCollider>())
				{
					collider.transform.localScale /= CollisionScaleFactor;
				}

				_worldCollisionReduced = true;
			}

			GTPlayer.Instance.transform.position = _savedPlayerPosition + new Vector3(-x, OrbitHeight, -z);
		}
	}

	private void RefreshBalloonCache()
	{
		if (Time.time <= _lastCacheRefresh + CacheRefreshSeconds)
		{
			return;
		}

		_lastCacheRefresh = Time.time;
		foreach (Object candidate in Object.FindObjectsOfType(typeof(BalloonHoldable)))
		{
			BalloonHoldable balloon = (BalloonHoldable)candidate;
			if (!_balloons.Contains(balloon))
			{
				_balloons.Add(balloon);
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

	private static void TryDisableDistanceConstraints(BalloonHoldable balloon)
	{
		try
		{
			BalloonDynamics dynamics = BalloonDynamicsField?.GetValue(balloon) as BalloonDynamics;
			if (dynamics != null)
			{
				dynamics.EnableDistanceConstraints(false);
			}
		}
		catch (Exception)
		{
		}
	}

	private void RestorePlayerAndBalloons()
	{
		foreach (BalloonHoldable balloon in _balloons)
		{
			if (balloon == null || balloon.ownerRig != GorillaTagger.Instance.offlineVRRig)
			{
				continue;
			}

			TransferrableObject transferrable = balloon.GetComponent<TransferrableObject>();
			if (transferrable != null)
			{
				transferrable.currentState = TransferrableObject.PositionState.None;
			}
		}

		if (_savedPlayerPosition != Vector3.zero)
		{
			GTPlayer.Instance.transform.position = _savedPlayerPosition;
		}

		if (_worldCollisionReduced)
		{
			foreach (MeshCollider collider in Resources.FindObjectsOfTypeAll<MeshCollider>())
			{
				collider.transform.localScale *= CollisionScaleFactor;
			}

			_worldCollisionReduced = false;
		}

		_savedPlayerPosition = Vector3.zero;
	}

	private void OnDisable()
	{
		RestorePlayerAndBalloons();
	}

	private void OnDestroy()
	{
		RestorePlayerAndBalloons();
	}
}
