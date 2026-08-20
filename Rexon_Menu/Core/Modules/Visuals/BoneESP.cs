// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.BoneESP
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Rexon_Menu.Core.Attributes;
using Rexon_Menu.Core.Patches;
using Rexon_Menu_Mat;
using UnityEngine;

using Object = UnityEngine.Object;

namespace Recovered.Obfuscated;

[Mod("Bone ESP", "Visuals", "Skeleton lines on players.", false, 4, ModType.Toggle, false)]
internal class BoneESP : MonoBehaviour
{
	private static readonly Dictionary<string, List<LineRenderer>> LinesByUserId =
		new Dictionary<string, List<LineRenderer>>();

	private static readonly int[] BonePairs =
	{
		4, 3, 5, 4, 19, 18, 20, 19, 3, 18,
		21, 20, 22, 21, 25, 21, 29, 21, 31, 29,
		27, 25, 24, 22, 6, 5, 7, 6, 10, 6,
		14, 6, 16, 14, 12, 10, 9, 7
	};

	private void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			ClearLines();
			return;
		}

		RemoveDepartedPlayers();
		foreach (Player player in PhotonNetwork.PlayerListOthers)
		{
			VRRig rig = MatBridge.GetVRRigFor(player);
			if (rig == null || rig.isOfflineVRRig || player.UserId == null)
			{
				continue;
			}

			List<LineRenderer> lines = GetOrCreateLines(player.UserId);
			Color color = GetPlayerColor(player);
			lines[0].SetPosition(0, rig.headConstraint.position + new Vector3(0f, 0.16f, 0f));
			lines[0].SetPosition(1, rig.headConstraint.position - new Vector3(0f, 0.4f, 0f));
			SetLineColor(lines[0], color);

			for (int bonePairIndex = 0; bonePairIndex < BonePairs.Length / 2; bonePairIndex++)
			{
				int firstBone = BonePairs[bonePairIndex * 2];
				int secondBone = BonePairs[bonePairIndex * 2 + 1];
				LineRenderer line = lines[bonePairIndex + 1];
				line.SetPosition(0, rig.mainSkin.bones[firstBone].position);
				line.SetPosition(1, rig.mainSkin.bones[secondBone].position);
				SetLineColor(line, color);
			}
		}
	}

	private static void RemoveDepartedPlayers()
	{
		HashSet<string> activeUserIds = new HashSet<string>();
		foreach (Player player in PhotonNetwork.PlayerListOthers)
		{
			if (player.UserId != null)
			{
				activeUserIds.Add(player.UserId);
			}
		}

		List<string> departedUserIds = new List<string>();
		foreach (KeyValuePair<string, List<LineRenderer>> entry in LinesByUserId)
		{
			if (!activeUserIds.Contains(entry.Key))
			{
				DestroyLines(entry.Value);
				departedUserIds.Add(entry.Key);
			}
		}
		foreach (string userId in departedUserIds)
		{
			LinesByUserId.Remove(userId);
		}
	}

	private static List<LineRenderer> GetOrCreateLines(string userId)
	{
		if (LinesByUserId.TryGetValue(userId, out List<LineRenderer> lines))
		{
			return lines;
		}

		lines = new List<LineRenderer> { CreateLine("HeadLine_" + userId) };
		for (int lineIndex = 0; lineIndex < BonePairs.Length / 2; lineIndex++)
		{
			lines.Add(CreateLine("BoneLine_" + userId + "_" + lineIndex));
		}
		LinesByUserId[userId] = lines;
		return lines;
	}

	private static Color GetPlayerColor(Player player)
	{
		if (VisualThemeSetting.CurrentTheme != "Original")
		{
			return VisualThemeSetting.GetCurrentColor();
		}
		return MatBridge.IsInfected(player)
			? new Color(1f, 0.15f, 0.15f, 0.9f)
			: new Color(0.15f, 1f, 0.15f, 0.9f);
	}

	private static LineRenderer CreateLine(string name)
	{
		LineRenderer line = new GameObject(name).AddComponent<LineRenderer>();
		line.startWidth = 0.03f;
		line.endWidth = 0.03f;
		line.positionCount = 2;
		line.useWorldSpace = true;
		line.material = ShaderPatch.CreateTransparentMaterial(Color.white);
		line.numCapVertices = 8;
		line.numCornerVertices = 8;
		return line;
	}

	private static void SetLineColor(LineRenderer line, Color color)
	{
		line.material.color = color;
		line.startColor = color;
		line.endColor = color;
	}

	private static void DestroyLines(IEnumerable<LineRenderer> lines)
	{
		foreach (LineRenderer line in lines)
		{
			if (line != null)
			{
				Object.Destroy(line.gameObject);
			}
		}
	}

	private static void ClearLines()
	{
		foreach (List<LineRenderer> lines in LinesByUserId.Values)
		{
			DestroyLines(lines);
		}
		LinesByUserId.Clear();
	}

	private void OnDisable()
	{
		ClearLines();
	}
}
