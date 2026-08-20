// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Visuals.RingESP
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using Rexon_Menu.Core.Attributes;
using Rexon_Menu.Core.Patches;
using Rexon_Menu_Mat;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.Visuals;

[Mod("Ring ESP", "Visuals", "Cyan rings at player feet.", false, 37, ModType.Toggle, false)]
internal class RingESP : MonoBehaviour
{
	internal struct ESPData
	{
		public Player Player;
		public string UserId;
		public GameObject Visual;

		public ESPData(Player player, string userId, GameObject visual)
		{
			Player = player;
			UserId = userId;
			Visual = visual;
		}
	}
	private List<ESPData> _visuals = new List<ESPData>();


	private void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			ClearVisuals();
			return;
		}
		for (int i = _visuals.Count - 1; i >= 0; i--)
		{
			ESPData entry = _visuals[i];
			if (entry.Visual == null || entry.Player == null || !PhotonNetwork.PlayerListOthers.Contains(entry.Player))
			{
				if (entry.Visual != null)
				{
					Object.Destroy(entry.Visual);
				}
				_visuals.RemoveAt(i);
			}
		}
		foreach (Player player in PhotonNetwork.PlayerListOthers)
		{
			VRRig rig = MatBridge.GetVRRigFor(player);
			if (rig == null || rig.isOfflineVRRig)
			{
				continue;
			}
			GameObject visual = _visuals.FirstOrDefault(entry => entry.UserId == player.UserId).Visual;
			if (visual == null)
			{
				visual = GameObject.CreatePrimitive((PrimitiveType)2);
				visual.transform.localScale = new Vector3(1.2f, 0.02f, 1.2f);
				Object.Destroy(visual.GetComponent<Collider>());
				visual.GetComponent<Renderer>().material = ShaderPatch.CreateTransparentMaterial(new Color(0f, 1f, 1f, 0.4f));
				_visuals.Add(new ESPData(player, player.UserId, visual));
			}
			visual.transform.position = rig.transform.position - Vector3.up * 0.3f;
		}
	}

	private void ClearVisuals()
	{
		for (int i = 0; i < _visuals.Count; i++)
		{
			if (_visuals[i].Visual != null)
			{
				Object.Destroy(_visuals[i].Visual);
			}
		}
		_visuals.Clear();
	}

	private void OnDisable()
	{
		ClearVisuals();
	}

	private void OnDestroy()
	{
		ClearVisuals();
	}
}
