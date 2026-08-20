// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.ShowVelocity
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

[Mod("Show Velocity", "Visuals", "Shows speed of each player.", false, 26, ModType.Toggle, false)]
internal class ShowVelocity : MonoBehaviour
{
	private const string NumberFormat = "F1";

	internal struct VelocityData
	{
		public Player Player;

		public string UserId;

		public TMP_Text Label;

		public VelocityData(Player player, string userId, TMP_Text text)
		{
			Player = player;
			UserId = userId;
			Label = text;
		}
	}

	private static readonly List<VelocityData> VelocityLabels = new List<VelocityData>();

	private void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			ClearLabels();
			return;
		}
		for (int i = VelocityLabels.Count - 1; i >= 0; i--)
		{
			VelocityData velocityData = VelocityLabels[i];
			if (velocityData.Label == null || velocityData.Player == null || !PhotonNetwork.PlayerListOthers.Contains(velocityData.Player))
			{
				if (velocityData.Label != null)
				{
					Object.Destroy(velocityData.Label.gameObject);
				}
				VelocityLabels.RemoveAt(i);
			}
		}
		foreach (Player player in PhotonNetwork.PlayerListOthers)
		{
			VRRig rig = MatBridge.GetVRRigFor(player);
			if (rig == null || rig.isOfflineVRRig)
			{
				continue;
			}
			TMP_Text label = VelocityLabels.FirstOrDefault(entry => entry.UserId == player.UserId).Label;
			if (label == null)
			{
				label = Object.Instantiate<TextMeshPro>(rig.playerText1);
				label.fontSize = 2f;
				label.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
				VelocityLabels.Add(new VelocityData(player, player.UserId, label));
			}
			float speed = rig.LatestVelocity().magnitude;
			label.text = "Speed: " + speed.ToString(NumberFormat) + " m/s";
			label.color = speed > 10f ? Color.red : Color.cyan;
			label.transform.position = rig.transform.position + new Vector3(0f, 0.35f, 0f);
			label.transform.LookAt(Camera.main.transform);
			label.transform.Rotate(0f, 180f, 0f);
		}
	}

	private void ClearLabels()
	{
		for (int i = 0; i < VelocityLabels.Count; i++)
		{
			if (VelocityLabels[i].Label != null)
			{
				Object.Destroy(VelocityLabels[i].Label.gameObject);
			}
		}
		VelocityLabels.Clear();
	}

	private void OnDisable()
	{
		ClearLabels();
	}
}
