// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Overpowered.TracerSpaz
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System;
using System.Collections.Generic;
using System.Reflection;
using Photon.Pun;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

using Object = UnityEngine.Object;

namespace Rexon_Menu.Core.Modules.Overpowered;

[Mod("Tracer Spaz [RG] [BALLOON]", "Overpowered", "Spazzes balloon strings. Server sided. Needs balloon.", false, 33, ModType.Toggle, false)]
internal class TracerSpaz : MonoBehaviour
{
	private const float CacheRefreshSeconds = 3f;
	private const float RandomOffsetRange = 1000f;

	private static readonly FieldInfo BalloonDynamicsField = typeof(BalloonHoldable).GetField(
		"balloonDynamics",
		BindingFlags.Instance | BindingFlags.NonPublic);

	private readonly List<BalloonHoldable> _balloons = new List<BalloonHoldable>();
	private float _lastCacheRefresh;

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

			EnsureLocalOwnership(balloon);
			TryDisableDistanceConstraints(balloon);

			Vector3 randomOffset = new Vector3(
				UnityEngine.Random.Range(-RandomOffsetRange, RandomOffsetRange),
				UnityEngine.Random.Range(-RandomOffsetRange, RandomOffsetRange),
				UnityEngine.Random.Range(-RandomOffsetRange, RandomOffsetRange));
			balloon.transform.position = GorillaTagger.Instance.offlineVRRig.transform.position + randomOffset;
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

	private void ResetOwnedBalloons()
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
	}

	private void OnDisable()
	{
		ResetOwnedBalloons();
	}

	private void OnDestroy()
	{
		ResetOwnedBalloons();
	}
}
