// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.DataESP
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using Rexon_Menu.Core.Attributes;
using Rexon_Menu_Mat;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Recovered.Obfuscated;

[Mod("Data ESP", "Visuals", "Shows platform, distance and velocity above players.", false, 27, ModType.Toggle, false)]
internal class DataESP : MonoBehaviour
{
	private const string NumberFormat = "F1";

	internal struct DataEntry
	{
		public Player Player;

		public string UserId;

		public TMP_Text Label;

		public DataEntry(Player player, string userId, TMP_Text text)
		{
			Player = player;
			UserId = userId;
			Label = text;
		}
	}

	private static readonly List<DataEntry> Entries = new List<DataEntry>();


	private void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			ClearPlayerLabels();
			return;
		}
		for (int i = Entries.Count - 1; i >= 0; i--)
		{
			DataEntry dataEntry = Entries[i];
			if (dataEntry.Label == null || dataEntry.Player == null || !PhotonNetwork.PlayerListOthers.Contains(dataEntry.Player))
			{
				if (dataEntry.Label != null)
				{
					Object.Destroy(dataEntry.Label.gameObject);
				}
				Entries.RemoveAt(i);
			}
		}
		foreach (Player player in PhotonNetwork.PlayerListOthers)
		{
			VRRig rig = MatBridge.GetVRRigFor(player);
			if (rig == null || rig.isOfflineVRRig)
			{
				continue;
			}
			TMP_Text label = Entries.FirstOrDefault(entry => entry.UserId == player.UserId).Label;
			if (label == null)
			{
				if (rig.playerText1 == null)
				{
					continue;
				}
				TextMeshPro textMesh = Object.Instantiate<TextMeshPro>(rig.playerText1);
				textMesh.fontSize = 2f;
				textMesh.fontSizeMax = 10f;
				textMesh.color = Color.white;
				textMesh.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
				RectTransform component = textMesh.GetComponent<RectTransform>();
				if (component != null)
				{
					component.sizeDelta = new Vector2(component.sizeDelta.x, 10f);
				}
				label = textMesh;
				Entries.Add(new DataEntry(player, player.UserId, label));
			}
			label.transform.position = rig.transform.position + new Vector3(0f, 0.55f, 0f);
			if (Camera.main != null)
			{
				label.transform.LookAt(Camera.main.transform);
				label.transform.Rotate(0f, 180f, 0f);
			}
			float distance = Vector3.Distance(GorillaTagger.Instance.offlineVRRig.transform.position, rig.transform.position);
			float speed = rig.LatestVelocity().magnitude;
			string platform = GameNetworkUtilities.HasKnownModMarker(player) ? "PC" : "Quest";
			label.text = $"{platform} | {distance.ToString(NumberFormat)}m | {speed.ToString(NumberFormat)}m/s";
		}
	}

	private void ClearPlayerLabels()
	{
		for (int i = 0; i < Entries.Count; i++)
		{
			if (Entries[i].Label != null)
			{
				Object.Destroy(Entries[i].Label.gameObject);
			}
		}
		Entries.Clear();
	}

	private void OnDisable()
	{
		ClearPlayerLabels();
	}
}
