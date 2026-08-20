// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.ShowJoinDateAll
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Collections.Generic;
using Photon.Pun;
using Rexon_Menu.Core.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Recovered.Obfuscated;

[Mod("Show Join Date All", "Visuals", "Shows time since each player joined.", false, 8, ModType.Toggle, false)]
internal class ShowJoinDateAll : MonoBehaviour
{
	private const string JoinTimeFormat = "Joined: {0}m {1}s ago";

	private static readonly Vector3 LabelScale = new(0.5f, 0.5f, 0.5f);
	private static readonly Vector3 LabelOffset = new(0f, 0.5f, 0f);

	private readonly Dictionary<VRRig, TextMeshPro> _labelsByRig = new();
	private readonly Dictionary<VRRig, float> _joinTimesByRig = new();

	private void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			ClearLabels();
			return;
		}

		HashSet<VRRig> activeRemoteRigs = new();
		foreach (VRRig rig in VRRigCache.ActiveRigs)
		{
			if (rig == null ||
				rig == GorillaTagger.Instance.offlineVRRig)
			{
				continue;
			}

			activeRemoteRigs.Add(rig);
			if (!_joinTimesByRig.ContainsKey(rig))
			{
				_joinTimesByRig[rig] = Time.time;
			}

			if (!_labelsByRig.TryGetValue(rig, out TextMeshPro label))
			{
				label = CreateLabel(rig);
				_labelsByRig[rig] = label;
			}

			float elapsedSeconds = Time.time - _joinTimesByRig[rig];
			int minutes = (int)(elapsedSeconds / 60f);
			int seconds = (int)(elapsedSeconds % 60f);
			((TMP_Text)label).text = string.Format(JoinTimeFormat, minutes, seconds);

			Transform labelTransform = label.transform;
			labelTransform.position = rig.transform.position + LabelOffset;
			labelTransform.LookAt(Camera.main.transform);
			labelTransform.Rotate(0f, 180f, 0f);
		}

		RemoveStaleLabels(activeRemoteRigs);
	}

	private static TextMeshPro CreateLabel(VRRig rig)
	{
		TextMeshPro label = Object.Instantiate(rig.playerText1);
		((TMP_Text)label).fontSize = 2f;
		label.transform.localScale = LabelScale;
		((Graphic)label).color = Color.white;
		return label;
	}

	private void RemoveStaleLabels(HashSet<VRRig> activeRemoteRigs)
	{
		List<VRRig> staleRigs = new();
		foreach (KeyValuePair<VRRig, TextMeshPro> entry in _labelsByRig)
		{
			if (entry.Key != null && activeRemoteRigs.Contains(entry.Key))
			{
				continue;
			}

			if (entry.Value != null)
			{
				Object.Destroy(entry.Value.gameObject);
			}
			staleRigs.Add(entry.Key);
		}

		foreach (VRRig rig in staleRigs)
		{
			_labelsByRig.Remove(rig);
			_joinTimesByRig.Remove(rig);
		}
	}

	private void ClearLabels()
	{
		foreach (TextMeshPro label in _labelsByRig.Values)
		{
			if (label != null)
			{
				Object.Destroy(label.gameObject);
			}
		}

		_labelsByRig.Clear();
		_joinTimesByRig.Clear();
	}

	private void OnDisable()
	{
		ClearLabels();
	}
}
