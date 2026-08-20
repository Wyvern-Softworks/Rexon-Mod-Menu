// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Visuals.ShowPlatform
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Recovered.Obfuscated;
using Rexon_Menu.Core.Attributes;
using Rexon_Menu_Mat;
using TMPro;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.Visuals;

[Mod("Show Platform", "Visuals", "Shows PC or Quest above each player.", false, 42, ModType.Toggle, false)]
internal class ShowPlatform : MonoBehaviour
{
	private const string LabelPrefix = "Platform: ";
	private const string PcLabel = "PC/Steam";
	private const string QuestLabel = "Quest";

	private static readonly Vector3 LabelScale = new(0.5f, 0.5f, 0.5f);
	private static readonly Vector3 LabelOffset = new(0f, 0.5f, 0f);

	private readonly struct PlatformLabel
	{
		internal readonly Player Player;
		internal readonly string UserId;
		internal readonly TMP_Text Text;

		internal PlatformLabel(Player player, TMP_Text text)
		{
			Player = player;
			UserId = player.UserId;
			Text = text;
		}
	}

	private readonly List<PlatformLabel> _labels = new();

	private void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			ClearLabels();
			return;
		}

		RemoveStaleLabels();
		foreach (Player player in PhotonNetwork.PlayerListOthers)
		{
			VRRig rig = MatBridge.GetVRRigFor(player);
			if (rig == null || rig.isOfflineVRRig)
			{
				continue;
			}

			TMP_Text label = FindLabel(player.UserId);
			if (label == null)
			{
				label = CreateLabel(rig);
				if (label == null)
				{
					continue;
				}
				_labels.Add(new PlatformLabel(player, label));
			}

			label.transform.position = rig.transform.position + LabelOffset;
			if (Camera.main != null)
			{
				label.transform.LookAt(Camera.main.transform);
				label.transform.Rotate(0f, 180f, 0f);
			}

			bool isPcPlayer = GameNetworkUtilities.HasKnownModMarker(player);
			label.color = isPcPlayer ? Color.magenta : Color.green;
			label.text = LabelPrefix + (isPcPlayer ? PcLabel : QuestLabel);
		}
	}

	private void RemoveStaleLabels()
	{
		for (int index = _labels.Count - 1; index >= 0; index--)
		{
			PlatformLabel label = _labels[index];
			if (label.Text != null &&
				label.Player != null &&
				System.Array.IndexOf(PhotonNetwork.PlayerListOthers, label.Player) >= 0)
			{
				continue;
			}

			DestroyLabel(label.Text);
			_labels.RemoveAt(index);
		}
	}

	private TMP_Text FindLabel(string userId)
	{
		foreach (PlatformLabel label in _labels)
		{
			if (label.UserId == userId)
			{
				return label.Text;
			}
		}
		return null;
	}

	private static TMP_Text CreateLabel(VRRig rig)
	{
		if (rig.playerText1 == null)
		{
			return null;
		}

		TextMeshPro label = Object.Instantiate(rig.playerText1);
		label.fontSize = 2f;
		label.fontSizeMax = 10f;
		label.transform.localScale = LabelScale;
		RectTransform rect = label.GetComponent<RectTransform>();
		if (rect != null)
		{
			rect.sizeDelta = new Vector2(rect.sizeDelta.x, 10f);
		}
		return label;
	}

	private static void DestroyLabel(TMP_Text label)
	{
		if (label != null)
		{
			Object.Destroy(label.gameObject);
		}
	}

	private void ClearLabels()
	{
		foreach (PlatformLabel label in _labels)
		{
			DestroyLabel(label.Text);
		}
		_labels.Clear();
	}

	private void OnDisable()
	{
		ClearLabels();
	}

	private void OnDestroy()
	{
		ClearLabels();
	}
}
