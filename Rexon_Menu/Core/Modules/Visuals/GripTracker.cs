// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Visuals.GripTracker
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using Rexon_Menu.Core.Attributes;
using Rexon_Menu_Mat;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.Visuals;

[Mod("Grip Tracker", "Visuals", "Colors players by grip tendency. Green=gripping, Red=never.", false, 45, ModType.Toggle, false)]
internal class GripTracker : MonoBehaviour
{
	private const float SampleInterval = 0.1f;
	private const string BodyMaterialName = "gorilla_body";
	private const string OverlayShaderName = "GUI/Text Shader";

	internal struct GripData
	{
		public Player Player;
		public string UserId;
		public VRRig Rig;
		public Shader OriginalShader;
		public int GripSamples;
		public int TotalSamples;

		public GripData(Player player, string userId, VRRig rig, Shader originalShader)
		{
			Player = player;
			UserId = userId;
			Rig = rig;
			OriginalShader = originalShader;
			GripSamples = 0;
			TotalSamples = 0;
		}
	}

	private readonly List<GripData> _players = new List<GripData>();
	private float _lastSampleTime;

	private void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			RestorePlayerMaterials();
			return;
		}

		RemoveDepartedPlayers();
		bool shouldSample = Time.time - _lastSampleTime >= SampleInterval;
		foreach (Player player in PhotonNetwork.PlayerListOthers)
		{
			VRRig rig = MatBridge.GetVRRigFor(player);
			if (rig == null || rig.isOfflineVRRig || rig.mainSkin == null)
			{
				continue;
			}

			int index = _players.FindIndex(entry => entry.UserId == player.UserId);
			if (index < 0)
			{
				_players.Add(new GripData(player, player.UserId, rig, ((Renderer)rig.mainSkin).material.shader));
				index = _players.Count - 1;
			}

			GripData data = _players[index];
			if (shouldSample)
			{
				data.TotalSamples++;
				if (rig.leftHandLink.isReadyForGrabbing || rig.rightHandLink.isReadyForGrabbing)
				{
					data.GripSamples++;
				}
				_players[index] = data;
			}

			float gripRatio = data.TotalSamples > 0 ? (float)data.GripSamples / data.TotalSamples : 0f;
			Color color = GetGripColor(gripRatio);
			color.a = 0.6f;
			((Renderer)rig.mainSkin).material.shader = Shader.Find(OverlayShaderName);
			((Renderer)rig.mainSkin).material.color = color;
		}

		if (shouldSample)
		{
			_lastSampleTime = Time.time;
		}
	}

	private void RemoveDepartedPlayers()
	{
		for (int index = _players.Count - 1; index >= 0; index--)
		{
			GripData data = _players[index];
			if (data.Rig != null && PhotonNetwork.PlayerListOthers.Contains(data.Player))
			{
				continue;
			}
			RestorePlayerMaterial(data);
			_players.RemoveAt(index);
		}
	}

	private Color GetGripColor(float gripRatio)
	{
		float minimum = 1f;
		float maximum = 0f;
		foreach (GripData data in _players)
		{
			if (data.TotalSamples < 5)
			{
				continue;
			}
			float ratio = (float)data.GripSamples / data.TotalSamples;
			minimum = Mathf.Min(minimum, ratio);
			maximum = Mathf.Max(maximum, ratio);
		}
		float range = maximum - minimum;
		float normalized = range > 0.001f ? (gripRatio - minimum) / range : gripRatio;
		return Color.HSVToRGB(Mathf.Clamp01(normalized) * 0.33f, 1f, 1f);
	}

	private void RestorePlayerMaterials()
	{
		foreach (GripData data in _players)
		{
			RestorePlayerMaterial(data);
		}
		_players.Clear();
	}

	private static void RestorePlayerMaterial(GripData data)
	{
		if (data.Rig == null || data.Rig.mainSkin == null)
		{
			return;
		}
		Material material = ((Renderer)data.Rig.mainSkin).material;
		material.shader = data.OriginalShader;
		if (material.name.Contains(BodyMaterialName))
		{
			material.color = data.Rig.playerColor;
		}
	}

	private void OnDisable() => RestorePlayerMaterials();
	private void OnDestroy() => RestorePlayerMaterials();
}
