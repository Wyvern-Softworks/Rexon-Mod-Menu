// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.DoubleHandTracers
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

[Mod("Double Hand Tracers", "Visuals", "3-point tracers from both hands to players.", false, 9, ModType.Toggle, false)]
internal class DoubleHandTracers : MonoBehaviour
{
	internal struct TracerData
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

	private static List<TracerData> Tracers = new();


	private void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			ClearTracers();
			return;
		}
		Vector3 leftHandPosition = GorillaTagger.Instance.leftHandTransform.position;
		Vector3 rightHandPosition = GorillaTagger.Instance.rightHandTransform.position;
		for (int i = Tracers.Count - 1; i >= 0; i--)
		{
			TracerData tracerData = Tracers[i];
			if (tracerData.Line == null || tracerData.Player == null || !PhotonNetwork.PlayerListOthers.Contains(tracerData.Player))
			{
				if (tracerData.Line != null)
				{
					Object.Destroy(tracerData.Line.gameObject);
				}
				Tracers.RemoveAt(i);
			}
		}
		foreach (Player player in PhotonNetwork.PlayerListOthers)
		{
			VRRig rig = MatBridge.GetVRRigFor(player);
			if (rig == null || rig.isOfflineVRRig)
			{
				continue;
			}
			LineRenderer line = Tracers.FirstOrDefault(entry => entry.UserId == player.UserId).Line;
			if (line == null)
			{
				line = new GameObject("DoubleHandTracer_" + player.UserId).AddComponent<LineRenderer>();
				line.startWidth = 0.004f;
				line.endWidth = 0.004f;
				line.positionCount = 3;
				line.useWorldSpace = true;
				line.material = ShaderPatch.CreateTransparentMaterial(Color.white);
				Color color = new Color(1f, 0.5f, 0f, 0.8f);
				line.startColor = color;
				line.endColor = color;
				Tracers.Add(new TracerData(player, player.UserId, line));
			}
			line.SetPosition(0, leftHandPosition);
			line.SetPosition(1, rig.transform.position);
			line.SetPosition(2, rightHandPosition);
		}
	}

	private void ClearTracers()
	{
		for (int i = 0; i < Tracers.Count; i++)
		{
			if (Tracers[i].Line != null)
			{
				Object.Destroy(Tracers[i].Line.gameObject);
			}
		}
		Tracers.Clear();
	}

	private void OnDisable()
	{
		ClearTracers();
	}
}


