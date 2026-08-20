// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.SkeletonESPV2
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

[Mod("Skeleton ESP V2", "Visuals", "Simplified skeleton lines on players.", false, 22, ModType.Toggle, false)]
internal class SkeletonESPV2 : MonoBehaviour
{
	private struct SkeletonData
	{
		public Player Player;
		public string UserId;
		public LineRenderer[] Lines;

		public SkeletonData(Player player, string userId, LineRenderer[] lines)
		{
			Player = player;
			UserId = userId;
			Lines = lines;
		}
	}

	private static readonly List<SkeletonData> Skeletons = new List<SkeletonData>();

	private void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			ClearSkeletons();
			return;
		}

		RemoveDepartedPlayers();
		foreach (Player player in PhotonNetwork.PlayerListOthers)
		{
			VRRig rig = MatBridge.GetVRRigFor(player);
			if (rig == null || rig.isOfflineVRRig)
			{
				continue;
			}

			LineRenderer[] lines = FindLines(player.UserId);
			if (lines == null)
			{
				lines = CreateLines(player.UserId);
				Skeletons.Add(new SkeletonData(player, player.UserId, lines));
			}

			Vector3 head = rig.headMesh.transform.position;
			Vector3 body = rig.transform.position;
			Vector3 leftHand = rig.leftHandTransform.position;
			Vector3 rightHand = rig.rightHandTransform.position;
			SetSegment(lines[0], head, body);
			SetSegment(lines[1], body, leftHand);
			SetSegment(lines[2], body, rightHand);
			SetSegment(lines[3], leftHand, rightHand);
		}
	}

	private static LineRenderer[] FindLines(string userId)
	{
		for (int index = 0; index < Skeletons.Count; index++)
		{
			if (Skeletons[index].UserId == userId)
			{
				return Skeletons[index].Lines;
			}
		}
		return null;
	}

	private static LineRenderer[] CreateLines(string userId)
	{
		LineRenderer[] lines = new LineRenderer[4];
		for (int index = 0; index < lines.Length; index++)
		{
			lines[index] = new GameObject($"SkeletonESP_{userId}_{index}").AddComponent<LineRenderer>();
			lines[index].positionCount = 2;
			lines[index].useWorldSpace = true;
			lines[index].startWidth = 0.01f;
			lines[index].endWidth = 0.01f;
			((Renderer)lines[index]).material = ShaderPatch.CreateTransparentMaterial(Color.white);
		}
		return lines;
	}

	private static void SetSegment(LineRenderer line, Vector3 start, Vector3 end)
	{
		line.SetPosition(0, start);
		line.SetPosition(1, end);
	}

	private static void RemoveDepartedPlayers()
	{
		for (int index = Skeletons.Count - 1; index >= 0; index--)
		{
			SkeletonData data = Skeletons[index];
			if (data.Player != null && PhotonNetwork.PlayerListOthers.Contains(data.Player))
			{
				continue;
			}
			DestroyLines(data.Lines);
			Skeletons.RemoveAt(index);
		}
	}

	private static void ClearSkeletons()
	{
		foreach (SkeletonData data in Skeletons)
		{
			DestroyLines(data.Lines);
		}
		Skeletons.Clear();
	}

	private static void DestroyLines(LineRenderer[] lines)
	{
		if (lines == null)
		{
			return;
		}
		foreach (LineRenderer line in lines)
		{
			if (line != null)
			{
				Object.Destroy(line.gameObject);
			}
		}
	}

	private void OnDisable() => ClearSkeletons();
}
