// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Visuals.FullPlayerInfo
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using Recovered.Obfuscated;
using Rexon_Menu.Core.Attributes;
using Rexon_Menu_Mat;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rexon_Menu.Core.Modules.Visuals;

[Mod("Full Player Info", "Visuals", "Shows platform, distance and velocity above players.", false, 43, ModType.Toggle, false)]
internal class FullPlayerInfo : MonoBehaviour
{
	private const string NumberFormat = "F1";

	internal struct InfoData
	{
		public Player Player;
		public string UserId;
		public TMP_Text Text;

		public InfoData(Player player, string userId, TMP_Text text)
		{
			Player = player;
			UserId = userId;
			Text = text;
		}
	}
	private List<InfoData> _entries = new List<InfoData>();

	private void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			ClearInfoLabels();
			return;
		}
		Vector3 localPosition = GorillaTagger.Instance.offlineVRRig.transform.position;
		for (int i = _entries.Count - 1; i >= 0; i--)
		{
			InfoData infoData = _entries[i];
			if (infoData.Text == null || infoData.Player == null || !PhotonNetwork.PlayerListOthers.Contains(infoData.Player))
			{
				if (infoData.Text != null)
				{
					Object.Destroy(infoData.Text.gameObject);
				}
				_entries.RemoveAt(i);
			}
		}
		foreach (Player player in PhotonNetwork.PlayerListOthers)
		{
			VRRig rig = MatBridge.GetVRRigFor(player);
			if (rig == null || rig.isOfflineVRRig)
			{
				continue;
			}
			TMP_Text label = _entries.FirstOrDefault(entry => entry.UserId == player.UserId).Text;
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
				_entries.Add(new InfoData(player, player.UserId, label));
			}
			label.transform.position = rig.transform.position + new Vector3(0f, 0.55f, 0f);
			if (Camera.main != null)
			{
				label.transform.LookAt(Camera.main.transform);
				label.transform.Rotate(0f, 180f, 0f);
			}
			float distance = Vector3.Distance(localPosition, rig.transform.position);
			float speed = rig.LatestVelocity().magnitude;
			string platform = GameNetworkUtilities.HasKnownModMarker(player) ? "PC" : "Quest";
			label.text = $"{platform} | {distance.ToString(NumberFormat)}m | {speed.ToString(NumberFormat)}m/s";
		}
	}

	private void ClearInfoLabels()
	{
		for (int i = 0; i < _entries.Count; i++)
		{
			if (_entries[i].Text != null)
			{
				Object.Destroy(_entries[i].Text.gameObject);
			}
		}
		_entries.Clear();
	}

	private void OnDisable()
	{
		ClearInfoLabels();
	}

	private void OnDestroy()
	{
		ClearInfoLabels();
	}
}
