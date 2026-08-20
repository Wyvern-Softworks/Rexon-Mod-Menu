// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.ShowDistance
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

[Mod("Show Distance", "Visuals", "Shows distance to each player.", false, 25, ModType.Toggle, false)]
internal class ShowDistance : MonoBehaviour
{
	private const string NumberFormat = "F1";

	internal struct DistanceData
	{
		public Player Player;

		public string UserId;

		public TMP_Text Label;

		public DistanceData(Player player, string userId, TMP_Text text)
		{
			Player = player;
			UserId = userId;
			Label = text;
		}
	}

	private static readonly List<DistanceData> DistanceLabels = new List<DistanceData>();


	private void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			ClearLabels();
			return;
		}
		Vector3 localPosition = GorillaTagger.Instance.offlineVRRig.transform.position;
		for (int i = DistanceLabels.Count - 1; i >= 0; i--)
		{
			DistanceData distanceData = DistanceLabels[i];
			if (distanceData.Label == null || distanceData.Player == null || !PhotonNetwork.PlayerListOthers.Contains(distanceData.Player))
			{
				if (distanceData.Label != null)
				{
					Object.Destroy(distanceData.Label.gameObject);
				}
				DistanceLabels.RemoveAt(i);
			}
		}
		foreach (Player player in PhotonNetwork.PlayerListOthers)
		{
			VRRig rig = MatBridge.GetVRRigFor(player);
			if (rig == null || rig.isOfflineVRRig)
			{
				continue;
			}
			TMP_Text label = DistanceLabels.FirstOrDefault(entry => entry.UserId == player.UserId).Label;
			if (label == null)
			{
				label = Object.Instantiate<TextMeshPro>(rig.playerText1);
				label.fontSize = 2f;
				label.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
				DistanceLabels.Add(new DistanceData(player, player.UserId, label));
			}
			float distance = Vector3.Distance(localPosition, rig.transform.position);
			label.text = "Distance: " + distance.ToString(NumberFormat) + "m";
			label.color = Color.yellow;
			label.transform.position = rig.transform.position + new Vector3(0f, 0.45f, 0f);
			label.transform.LookAt(Camera.main.transform);
			label.transform.Rotate(0f, 180f, 0f);
		}
	}

	private void ClearLabels()
	{
		for (int i = 0; i < DistanceLabels.Count; i++)
		{
			if (DistanceLabels[i].Label != null)
			{
				Object.Destroy(DistanceLabels[i].Label.gameObject);
			}
		}
		DistanceLabels.Clear();
	}

	private void OnDisable()
	{
		ClearLabels();
	}
}
