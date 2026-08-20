// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.AntiReport
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Collections.Generic;
using GorillaTagScripts;
using Photon.Pun;
using Photon.Realtime;
using Rexon_Menu.Core.Attributes;
using Rexon_Menu.Core.Patches;
using UnityEngine;

using Object = UnityEngine.Object;

namespace Recovered.Obfuscated;

[Mod("Anti Report", "Safety", "Disconnects if someone tries to report.", false, 1, ModType.Toggle, false)]
internal class AntiReport : MonoBehaviour
{
	public static float ReportDistance = 0.5f;
	public static bool ShowReportZones;
	public static int ResponseModeIndex;
	public static readonly string[] ResponseModeNames =
	{
		"Disconnect",
		"Fling [GUARDIAN]",
		"Stutter"
	};

	private static readonly Dictionary<Vector3, GameObject> ReportZones =
		new Dictionary<Vector3, GameObject>();

	private static Color reportZoneColor = new Color(0f, 1f, 0f, 0.5f);
	private float nextScanTime;

	private void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			ClearReportZones();
			return;
		}
		if (Time.time < nextScanTime)
		{
			return;
		}

		nextScanTime = Time.time + 0.05f;
		List<Vector3> reportButtonPositions = FindLocalReportButtonPositions();
		HashSet<Vector3> activeZoneKeys = new HashSet<Vector3>();
		foreach (Vector3 position in reportButtonPositions)
		{
			activeZoneKeys.Add(QuantizePosition(position));
		}

		RemoveStaleReportZones(activeZoneKeys);
		if (ShowReportZones)
		{
			UpdateReportZones(reportButtonPositions);
		}
		else
		{
			ClearReportZones();
		}

		VRRig nearestRig;
		float nearestDistance = FindNearestRemoteHand(reportButtonPositions, out nearestRig);
		if (ShowReportZones)
		{
			UpdateReportZoneColor(nearestDistance);
		}
		if (nearestRig != null && nearestDistance < ReportDistance)
		{
			RespondToReportAttempt(nearestRig);
		}
	}

	private static List<Vector3> FindLocalReportButtonPositions()
	{
		List<Vector3> positions = new List<Vector3>();
		Player localPlayer = PhotonNetwork.LocalPlayer;
		if (localPlayer == null)
		{
			return positions;
		}

		List<GorillaPlayerScoreboardLine> lines = GorillaScoreboardTotalUpdater.allScoreboardLines;
		if (lines == null)
		{
			return positions;
		}

		foreach (GorillaPlayerScoreboardLine line in lines)
		{
			if (line != null &&
				line.linePlayer != null &&
				line.linePlayer.ActorNumber == localPlayer.ActorNumber &&
				line.reportButton != null)
			{
				positions.Add(line.reportButton.transform.position);
			}
		}
		return positions;
	}

	private static Vector3 QuantizePosition(Vector3 position)
	{
		const float gridSize = 0.15f;
		return new Vector3(
			Mathf.Round(position.x / gridSize) * gridSize,
			Mathf.Round(position.y / gridSize) * gridSize,
			Mathf.Round(position.z / gridSize) * gridSize);
	}

	private static void RemoveStaleReportZones(HashSet<Vector3> activeZoneKeys)
	{
		List<Vector3> staleKeys = new List<Vector3>();
		foreach (KeyValuePair<Vector3, GameObject> zone in ReportZones)
		{
			if (!activeZoneKeys.Contains(zone.Key))
			{
				if (zone.Value != null)
				{
					Object.Destroy(zone.Value);
				}
				staleKeys.Add(zone.Key);
			}
		}

		foreach (Vector3 key in staleKeys)
		{
			ReportZones.Remove(key);
		}
	}

	private static void UpdateReportZones(IEnumerable<Vector3> reportButtonPositions)
	{
		foreach (Vector3 position in reportButtonPositions)
		{
			Vector3 key = QuantizePosition(position);
			if (!ReportZones.TryGetValue(key, out GameObject zone) || zone == null)
			{
				zone = CreateReportZone();
				ReportZones[key] = zone;
			}

			zone.transform.position = position;
			zone.transform.localScale = Vector3.one * ReportDistance * 2f;
		}
	}

	private static GameObject CreateReportZone()
	{
		GameObject zone = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		Collider collider = zone.GetComponent<Collider>();
		if (collider != null)
		{
			Object.Destroy(collider);
		}

		Renderer renderer = zone.GetComponent<Renderer>();
		if (renderer != null)
		{
			renderer.material = ShaderPatch.CreateTransparentMaterial(reportZoneColor);
		}
		return zone;
	}

	private static float FindNearestRemoteHand(IReadOnlyList<Vector3> positions, out VRRig nearestRig)
	{
		nearestRig = null;
		float nearestDistance = float.MaxValue;
		IReadOnlyList<VRRig> activeRigs = VRRigCache.ActiveRigs;
		foreach (Vector3 position in positions)
		{
			for (int rigIndex = 0; rigIndex < activeRigs.Count; rigIndex++)
			{
				VRRig rig = activeRigs[rigIndex];
				if (rig == null || rig.isOfflineVRRig)
				{
					continue;
				}

				float rightDistance = Vector3.Distance(rig.rightHandTransform.position, position);
				float leftDistance = Vector3.Distance(rig.leftHandTransform.position, position);
				float distance = Mathf.Min(rightDistance, leftDistance);
				if (distance < nearestDistance)
				{
					nearestDistance = distance;
					nearestRig = rig;
				}
			}
		}
		return nearestDistance;
	}

	private static void UpdateReportZoneColor(float nearestDistance)
	{
		float danger = Mathf.Clamp01(1f - (nearestDistance - ReportDistance) / 2f);
		reportZoneColor = new Color(danger, 1f - danger, 0f, 0.4f);
		foreach (GameObject zone in ReportZones.Values)
		{
			if (zone == null)
			{
				continue;
			}

			Renderer renderer = zone.GetComponent<Renderer>();
			if (renderer != null && renderer.material != null)
			{
				renderer.material.color = reportZoneColor;
			}
		}
	}

	private static void RespondToReportAttempt(VRRig reportingRig)
	{
		string mode = ResponseModeNames[Mathf.Clamp(ResponseModeIndex, 0, ResponseModeNames.Length - 1)];
		switch (mode)
		{
		case "Fling [GUARDIAN]":
			FlingReportingPlayer(reportingRig);
			break;
		case "Stutter":
			StutterReportingPlayer(reportingRig);
			break;
		default:
			PhotonNetwork.Disconnect();
			break;
		}
	}

	private static void FlingReportingPlayer(VRRig reportingRig)
	{
		GameObject guardianObject = GameObject.Find("GT Systems/GameModeSystem/Gorilla Guardian Manager");
		GorillaGuardianManager guardianManager =
			guardianObject != null ? guardianObject.GetComponent<GorillaGuardianManager>() : null;
		if (guardianManager == null || !guardianManager.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer))
		{
			PhotonNetwork.Disconnect();
			return;
		}

		Player target = reportingRig.Creator.GetPlayerRef();
		PhotonView localRigView = GorillaTagger.Instance.myVRRig.GetView;
		localRigView.SendRpc("GrabbedByPlayer", target, true, false, false);
		Vector3 launchVelocity = new Vector3(Random.Range(-5f, 5f), 10f, Random.Range(-5f, 5f));
		localRigView.SendRpc("DroppedByPlayer", target, launchVelocity);
		PhotonNetwork.SendAllOutgoingCommands();
	}

	private static void StutterReportingPlayer(VRRig reportingRig)
	{
		Player target = reportingRig.Creator.GetPlayerRef();
		FriendshipGroupDetection friendshipGroup = FriendshipGroupDetection.Instance;
		if (target == null || friendshipGroup == null)
		{
			PhotonNetwork.Disconnect();
			return;
		}

		for (int requestIndex = 0; requestIndex < Rexon_Menu.Core.Modules.Settings.LagPower.Power; requestIndex++)
		{
			friendshipGroup.photonView.SendRpc("AddPartyMembers", target, "Infection", (short)12, null);
		}
		PhotonNetwork.SendAllOutgoingCommands();
	}

	private static void ClearReportZones()
	{
		foreach (GameObject zone in ReportZones.Values)
		{
			if (zone != null)
			{
				Object.Destroy(zone);
			}
		}
		ReportZones.Clear();
	}

	private void OnDisable()
	{
		ClearReportZones();
	}
}
