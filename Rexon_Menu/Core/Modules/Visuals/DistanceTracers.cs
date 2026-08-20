// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.DistanceTracers
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using Rexon_Menu.Core.Attributes;
using Rexon_Menu.Core.Patches;
using Rexon_Menu_Mat;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Distance Tracers", "Visuals", "Color changes by distance to players.", false, 11, ModType.Toggle, false)]
internal class DistanceTracers : MonoBehaviour
{
	private struct TracerData
	{
		public Player Player;
		public string UserId;
		public LineRenderer Line;

		public TracerData(Player player, string userId, LineRenderer line)
		{
			Player = player;
			UserId = userId;
			Line = line;
		}
	}

	private static readonly List<TracerData> Tracers = new List<TracerData>();

	private void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			ClearTracers();
			return;
		}

		RemoveDepartedPlayers();
		Vector3 origin = GorillaTagger.Instance.rightHandTransform.position;
		foreach (Player player in PhotonNetwork.PlayerListOthers)
		{
			VRRig rig = MatBridge.GetVRRigFor(player);
			if (rig == null || rig.isOfflineVRRig)
			{
				continue;
			}

			LineRenderer line = FindTracer(player.UserId);
			if (line == null)
			{
				line = CreateTracer(player.UserId);
				Tracers.Add(new TracerData(player, player.UserId, line));
			}

			Vector3 target = rig.transform.position;
			Color color = Color.Lerp(Color.green, Color.red, Mathf.Clamp01(Vector3.Distance(origin, target) / 20f));
			((Renderer)line).material.color = color;
			line.startColor = color;
			line.endColor = color;
			line.SetPosition(0, origin);
			line.SetPosition(1, target);
		}
	}

	private static LineRenderer FindTracer(string userId)
	{
		for (int index = 0; index < Tracers.Count; index++)
		{
			if (Tracers[index].UserId == userId)
			{
				return Tracers[index].Line;
			}
		}
		return null;
	}

	private static LineRenderer CreateTracer(string userId)
	{
		LineRenderer line = new GameObject("DistanceTracer_" + userId).AddComponent<LineRenderer>();
		line.startWidth = 0.005f;
		line.endWidth = 0.005f;
		line.positionCount = 2;
		line.useWorldSpace = true;
		((Renderer)line).material = ShaderPatch.CreateTransparentMaterial(Color.white);
		return line;
	}

	private static void RemoveDepartedPlayers()
	{
		for (int index = Tracers.Count - 1; index >= 0; index--)
		{
			TracerData data = Tracers[index];
			if (data.Line != null &&
				data.Player != null && PhotonNetwork.PlayerListOthers.Contains(data.Player))
			{
				continue;
			}
			DestroyTracer(data.Line);
			Tracers.RemoveAt(index);
		}
	}

	private static void ClearTracers()
	{
		foreach (TracerData data in Tracers)
		{
			DestroyTracer(data.Line);
		}
		Tracers.Clear();
	}

	private static void DestroyTracer(LineRenderer line)
	{
		if (line != null)
		{
			Object.Destroy(line.gameObject);
		}
	}

	private void OnDisable() => ClearTracers();
}
