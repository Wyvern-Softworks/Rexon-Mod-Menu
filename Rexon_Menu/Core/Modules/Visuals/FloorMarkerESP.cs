// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.FloorMarkerESP
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using Rexon_Menu.Core.Attributes;
using Rexon_Menu.Core.Patches;
using Rexon_Menu_Mat;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Floor Marker ESP", "Visuals", "Flat markers on the ground below players.", false, 20, ModType.Toggle, false)]
internal class FloorMarkerESP : MonoBehaviour
{
	internal struct MarkerData
	{
		public Player Player;

		public string UserId;

		public GameObject Marker;

		public MarkerData(Player player, string userId, GameObject marker)
		{
			Player = player;
			UserId = userId;
			Marker = marker;
		}
	}

	private static List<MarkerData> Markers = new List<MarkerData>();


	private void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			ClearFloorMarkers();
			return;
		}
		for (int i = Markers.Count - 1; i >= 0; i--)
		{
			MarkerData markerData = Markers[i];
			if (markerData.Player == null || !PhotonNetwork.PlayerListOthers.Contains(markerData.Player))
			{
				if (markerData.Marker != null)
				{
					Object.Destroy(markerData.Marker);
				}
				Markers.RemoveAt(i);
			}
		}
		foreach (Player player in PhotonNetwork.PlayerListOthers)
		{
			VRRig rig = MatBridge.GetVRRigFor(player);
			if (rig == null || rig.isOfflineVRRig)
			{
				continue;
			}
			GameObject marker = Markers.FirstOrDefault(entry => entry.UserId == player.UserId).Marker;
			if (marker == null)
			{
				marker = GameObject.CreatePrimitive((PrimitiveType)3);
				marker.transform.localScale = new Vector3(0.8f, 0.01f, 0.8f);
				Object.Destroy(marker.GetComponent<Collider>());
				marker.GetComponent<Renderer>().material = ShaderPatch.CreateTransparentMaterial(new Color(1f, 0f, 0.5f, 0.5f));
				Markers.Add(new MarkerData(player, player.UserId, marker));
			}
			if (Physics.Raycast(rig.transform.position, Vector3.down, out RaycastHit hit, 100f))
			{
				marker.transform.position = hit.point + new Vector3(0f, 0.01f, 0f);
			}
			else
			{
				marker.transform.position = rig.transform.position + new Vector3(0f, -1f, 0f);
			}
		}
	}

	private void ClearFloorMarkers()
	{
		for (int i = 0; i < Markers.Count; i++)
		{
			if (Markers[i].Marker != null)
			{
				Object.Destroy(Markers[i].Marker);
			}
		}
		Markers.Clear();
	}

	private void OnDisable()
	{
		ClearFloorMarkers();
	}
}

