// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.HandESP
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using Rexon_Menu.Core.Attributes;
using Rexon_Menu.Core.Patches;
using Rexon_Menu_Mat;
using UnityEngine;

using Object = UnityEngine.Object;

namespace Recovered.Obfuscated;

[Mod("Hand ESP", "Visuals", "Colored spheres on player hands.", false, 18, ModType.Toggle, false)]
internal class HandESP : MonoBehaviour
{
	internal struct HandData
	{
		public Player Player;

		public string UserId;

		public GameObject LeftMarker;

		public GameObject RightMarker;

		public HandData(Player player, string userId, GameObject left, GameObject right)
		{
			Player = player;
			UserId = userId;
			LeftMarker = left;
			RightMarker = right;
		}
	}

	private static List<HandData> HandMarkers = new List<HandData>();

	private void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			ClearHandMarkers();
			return;
		}
		for (int i = HandMarkers.Count - 1; i >= 0; i--)
		{
			HandData handData = HandMarkers[i];
			if (handData.Player == null || !PhotonNetwork.PlayerListOthers.Contains(handData.Player))
			{
				if (handData.LeftMarker != null)
				{
					Object.Destroy(handData.LeftMarker);
				}
				if (handData.RightMarker != null)
				{
					Object.Destroy(handData.RightMarker);
				}
				HandMarkers.RemoveAt(i);
			}
		}
		foreach (Player player in PhotonNetwork.PlayerListOthers)
		{
			VRRig rig = MatBridge.GetVRRigFor(player);
			if (rig == null || rig.isOfflineVRRig)
			{
				continue;
			}
			HandData existing = HandMarkers.FirstOrDefault(entry => entry.UserId == player.UserId);
			GameObject leftMarker = existing.LeftMarker;
			GameObject rightMarker = existing.RightMarker;
			if (leftMarker == null)
			{
				leftMarker = GameObject.CreatePrimitive((PrimitiveType)0);
				leftMarker.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
				Object.Destroy(leftMarker.GetComponent<Collider>());
				leftMarker.GetComponent<Renderer>().material = ShaderPatch.CreateTransparentMaterial(new Color(0f, 0f, 1f, 0.5f));

				rightMarker = GameObject.CreatePrimitive((PrimitiveType)0);
				rightMarker.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
				Object.Destroy(rightMarker.GetComponent<Collider>());
				rightMarker.GetComponent<Renderer>().material = ShaderPatch.CreateTransparentMaterial(new Color(1f, 0f, 0f, 0.5f));
				HandMarkers.Add(new HandData(player, player.UserId, leftMarker, rightMarker));
			}
			leftMarker.transform.position = rig.leftHandTransform.position;
			rightMarker.transform.position = rig.rightHandTransform.position;
		}
	}

	private void ClearHandMarkers()
	{
		for (int i = 0; i < HandMarkers.Count; i++)
		{
			if (HandMarkers[i].LeftMarker != null)
			{
				Object.Destroy(HandMarkers[i].LeftMarker);
			}
			if (HandMarkers[i].RightMarker != null)
			{
				Object.Destroy(HandMarkers[i].RightMarker);
			}
		}
		HandMarkers.Clear();
	}

	private void OnDisable()
	{
		ClearHandMarkers();
	}
}

