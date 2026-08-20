// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Visuals.RainbowESP
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

[Mod("Rainbow ESP", "Visuals", "Rainbow cycling boxes around all players.", false, 36, ModType.Toggle, false)]
internal class RainbowESP : MonoBehaviour
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

	private float _hue;


	private void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			ClearVisuals();
			return;
		}
		_hue += Time.deltaTime * 0.5f;
		if (_hue > 1f)
		{
			_hue = 0f;
		}
		Color color = Color.HSVToRGB(_hue, 1f, 1f);
		color.a = 0.2f;
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
				visual = GameObject.CreatePrimitive((PrimitiveType)3);
				visual.transform.localScale = new Vector3(0.5f, 1.1f, 0.5f);
				Object.Destroy(visual.GetComponent<Collider>());
				visual.GetComponent<Renderer>().material = ShaderPatch.CreateTransparentMaterial(color);
				_visuals.Add(new ESPData(player, player.UserId, visual));
			}
			visual.transform.position = rig.transform.position;
			visual.GetComponent<Renderer>().material.color = color;
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
