// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.World.RopeOrbit
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Collections;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaLocomotion.Gameplay;
using Photon.Pun;
using Recovered.Obfuscated;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

using Object = UnityEngine.Object;

namespace Rexon_Menu.Core.Modules.World;

[Mod("Rope Orbit", "World", "Spins all ropes toward you.", false, 28, ModType.Toggle, false)]
internal class RopeOrbit : MonoBehaviour
{
	private Coroutine _coroutine;

	private GorillaRopeSwing[] _ropes;

	private GorillaRopeSwing _currentRope;

	private readonly List<GorillaRopeSwing> _recentRopes = new();

	private float _lastVelocityUpdateTime;

	private float _orbitAngleDegrees;


	private void OnEnable()
	{
		_ropes = null;
		_currentRope = null;
		_recentRopes.Clear();
		_orbitAngleDegrees = 0f;
		_coroutine = StartCoroutine(RopeCoroutine());
	}

	private void OnDisable()
	{
		if (_coroutine != null)
		{
			StopCoroutine(_coroutine);
			_coroutine = null;
		}
		_ropes = null;
		_currentRope = null;
		_recentRopes.Clear();
		if (GorillaTagger.Instance != null && GorillaTagger.Instance.offlineVRRig != null)
		{
			GorillaTagger.Instance.offlineVRRig.enabled = true;
		}
	}

	private IEnumerator RopeCoroutine()
	{
		while (true)
		{
			if (_ropes == null)
			{
				_ropes = Object.FindObjectsOfType<GorillaRopeSwing>();
			}
			if (_ropes.Length == 0)
			{
				yield return new WaitForSeconds(1f);
				continue;
			}
			if (_currentRope == null)
			{
				_currentRope = _ropes[0];
			}
			float cycleStart = Time.time;
			while (Time.time - cycleStart < 0.3f)
			{
				if (Time.time >= _lastVelocityUpdateTime + 0.02f)
				{
					_lastVelocityUpdateTime = Time.time;
					Transform bone = _currentRope.GetBone(_currentRope.nodes.Length);
					if (bone != null)
					{
						Vector3 towardPlayer = (GTPlayer.Instance.transform.position - bone.position).normalized;
						float angleRadians = _orbitAngleDegrees * Mathf.Deg2Rad;
						Vector3 orbitOffset = new Vector3(Mathf.Cos(angleRadians), 0.5f, Mathf.Sin(angleRadians));
						Vector3 velocity = (towardPlayer + orbitOffset.normalized * 0.5f).normalized * 100f;
						ApplyRopeVelocity(_currentRope, velocity);
					}
				}
				_orbitAngleDegrees += 15f;
				yield return null;
			}
			_recentRopes.Add(_currentRope);
			if (_recentRopes.Count > 7)
			{
				_recentRopes.RemoveAt(0);
			}
			GorillaRopeSwing nextRope = FindNearestUnvisitedRope();
			if (nextRope != null)
			{
				_currentRope = nextRope;
			}
			else
			{
				_recentRopes.Clear();
				_currentRope = _ropes[0];
			}
			yield return new WaitForSeconds(0.2f);
		}
	}

	internal void ApplyRopeVelocity(GorillaRopeSwing rope, Vector3 velocity)
	{
		if (RopeSwingManager.instance == null)
		{
			return;
		}
		GameNetworkUtilities.TakePhotonViewOwnership(((NetworkSceneObject)RopeSwingManager.instance).photonView);
		if (((NetworkSceneObject)RopeSwingManager.instance).photonView.Owner != PhotonNetwork.LocalPlayer)
		{
			return;
		}
		Transform bone = rope.GetBone(rope.nodes.Length);
		if (bone == null)
		{
			return;
		}

		GorillaTagger.Instance.offlineVRRig.enabled = false;
		Transform localRigTransform = GorillaTagger.Instance.offlineVRRig.transform;
		localRigTransform.position = bone.position + Vector3.forward;
		PhotonNetwork.SendAllOutgoingCommands();
		if (Vector3.Distance(bone.position, localRigTransform.position) > 5f)
		{
			return;
		}

		RopeSwingManager.instance.SendSetVelocity_RPC(rope.ropeId, rope.nodes.Length, velocity, true);
		PhotonNetwork.SendAllOutgoingCommands();
		GameNetworkUtilities.FlushAndReplayNetworkTraffic();
	}

	internal GorillaRopeSwing FindNearestUnvisitedRope()
	{
		GorillaRopeSwing nearestRope = null;
		float nearestDistance = float.MaxValue;
		foreach (GorillaRopeSwing candidate in _ropes)
		{
			if (_recentRopes.Contains(candidate) || candidate == _currentRope)
			{
				continue;
			}

			float distance = Vector3.Distance(
				_currentRope.transform.position,
				candidate.transform.position);
			if (distance < nearestDistance)
			{
				nearestRope = candidate;
				nearestDistance = distance;
			}
		}

		return nearestRope;
	}
}


