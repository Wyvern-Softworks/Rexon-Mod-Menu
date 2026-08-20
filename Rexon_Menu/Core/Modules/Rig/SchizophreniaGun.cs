// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Rig.SchizophreniaGun
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Collections.Generic;
using System.Linq;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using Recovered.Obfuscated;
using Rexon_Menu.Core.Attributes;
using Rexon_Menu.Core.Patches;
using Rexon_Shader;
using UnityEngine;

using Object = UnityEngine.Object;

namespace Rexon_Menu.Core.Modules.Rig;

[Mod("Schizophrenia Gun", "Rig", "Make players only see you.", false, 37, ModType.Toggle, false)]
internal sealed class SchizophreniaGun : MonoBehaviour
{
	private const string GunId = "SchizophreniaGun";

	private readonly List<Player> _targets = new();
	private readonly Dictionary<int, GameObject> _targetMarkers = new();
	private readonly Dictionary<int, float> _lastIsolationUpdateAt = new();
	private readonly Dictionary<int, float> _lastVisiblePoseAt = new();

	private float _lastTargetToggleAt;
	private float _lastIdleBroadcastAt;

	private void OnEnable()
	{
		PhotonNetworkController.Instance.disableAFKKick = true;
	}

	private void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			ClearMarkers();
			GunController.Release(GunId);
			return;
		}

		GunController.GunResult result = GunController.GetGunResult(
			GunId,
			targetPlayers: true,
			1f,
			allowSingleTargetLock: true);
		if (result.IsShooting
			&& result.IsActive
			&& result.Target != null
			&& Time.time > _lastTargetToggleAt + 1f)
		{
			_lastTargetToggleAt = Time.time;
			GunController.MarkFired(GunId);
			ToggleTarget(result.Target);
		}

		MaintainTargetVisibility();
	}

	private void ToggleTarget(Player player)
	{
		if (_targets.Contains(player))
		{
			_targets.Remove(player);
			RemoveTargetState(player.ActorNumber);
			GameNetworkUtilities.SetPhotonSerializeTickMultiplier(1000f);
		}
		else
		{
			_targets.Add(player);
		}
	}

	private void MaintainTargetVisibility()
	{
		if (_targets.Count == 0)
		{
			if (_targetMarkers.Count > 0)
			{
				ClearMarkers();
			}

			if (Time.time > _lastIdleBroadcastAt + 6f)
			{
				_lastIdleBroadcastAt = Time.time;
				foreach (Player player in PhotonNetwork.PlayerListOthers)
				{
					SendRigPoseTo(player, new Vector3(999999f, -999999f, 999999f));
				}
				PhotonNetwork.SendAllOutgoingCommands();
			}
			return;
		}

		GameNetworkUtilities.SetPhotonSerializeTickMultiplier(9999f);
		Player[] playersInRoom = PhotonNetwork.PlayerListOthers;
		for (int index = _targets.Count - 1; index >= 0; index--)
		{
			Player target = _targets[index];
			if (target == null || !playersInRoom.Contains(target))
			{
				_targets.RemoveAt(index);
				if (target != null)
				{
					RemoveTargetState(target.ActorNumber);
				}
				GameNetworkUtilities.SetPhotonSerializeTickMultiplier(1000f);
				continue;
			}

			VRRig targetRig = RigUtilities.GetRig(target);
			if (targetRig == null)
			{
				continue;
			}

			GameObject marker = GetOrCreateMarker(target.ActorNumber);
			marker.transform.position = targetRig.transform.position;

			if (!_lastIsolationUpdateAt.TryGetValue(target.ActorNumber, out float lastIsolation)
				|| Time.time > lastIsolation + 6f)
			{
				_lastIsolationUpdateAt[target.ActorNumber] = Time.time;
				foreach (Player nonTarget in playersInRoom)
				{
					if (!_targets.Contains(nonTarget))
					{
						SendRigPoseTo(nonTarget, new Vector3(1E+09f, 1E+09f, 1E+09f));
					}
				}
			}

			if (!_lastVisiblePoseAt.TryGetValue(target.ActorNumber, out float lastVisiblePose)
				|| Time.time > lastVisiblePose + 0.2f)
			{
				_lastVisiblePoseAt[target.ActorNumber] = Time.time;
				SendRigPoseTo(target, GorillaTagger.Instance.offlineVRRig.transform.position);
			}
		}

		PhotonNetwork.SendAllOutgoingCommands();
	}

	private static void SendRigPoseTo(Player recipient, Vector3 position)
	{
		GameNetworkUtilities.SendSpoofedRigSerialization(
			replacePosition: true,
			position,
			new[] { recipient.ActorNumber },
			Vector3.zero,
			Vector3.zero);
	}

	private GameObject GetOrCreateMarker(int actorNumber)
	{
		if (_targetMarkers.TryGetValue(actorNumber, out GameObject marker) && marker != null)
		{
			return marker;
		}

		marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		marker.transform.localScale = Vector3.one * 0.1f;
		Object.Destroy(marker.GetComponent<Rigidbody>());
		Object.Destroy(marker.GetComponent<Collider>());
		ShaderPatch.EnsureCached();
		marker.GetComponent<Renderer>().material.shader = ShaderBridge.Cached;
		marker.GetComponent<Renderer>().material.color = Color.cyan;
		_targetMarkers[actorNumber] = marker;
		return marker;
	}

	private void RemoveTargetState(int actorNumber)
	{
		if (_targetMarkers.TryGetValue(actorNumber, out GameObject marker) && marker != null)
		{
			Object.DestroyImmediate(marker);
		}

		_targetMarkers.Remove(actorNumber);
		_lastIsolationUpdateAt.Remove(actorNumber);
		_lastVisiblePoseAt.Remove(actorNumber);
	}

	private void ClearMarkers()
	{
		foreach (GameObject marker in _targetMarkers.Values)
		{
			if (marker != null)
			{
				Object.DestroyImmediate(marker);
			}
		}

		_targetMarkers.Clear();
		_lastIsolationUpdateAt.Clear();
		_lastVisiblePoseAt.Clear();
	}

	private void OnDisable()
	{
		_targets.Clear();
		ClearMarkers();
		GameNetworkUtilities.SetPhotonSerializeTickMultiplier(1000f);
		GunController.Release(GunId);
	}
}
