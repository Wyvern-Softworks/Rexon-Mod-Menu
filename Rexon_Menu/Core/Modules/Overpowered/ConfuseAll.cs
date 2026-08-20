// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Overpowered.ConfuseAll
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using Recovered.Obfuscated;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.Overpowered;

[Mod("Confuse All [FOREST]", "Overpowered", "Makes all players see you in random spots. Forest only.", false, 42, ModType.Toggle, false)]
internal class ConfuseAll : MonoBehaviour
{
	private static readonly Vector3[] ConfusionPositions =
	{
		new Vector3(-66.3784f, 11.8747f, -78.9161f),
		new Vector3(-49.6311f, 13.0581f, -58.1247f),
		new Vector3(-46.6669f, 14.1478f, -64.9965f),
		new Vector3(-51.8117f, 14.1579f, -51.3141f),
		new Vector3(-42.6003f, 14.9949f, -60.7968f),
		new Vector3(-46.1976f, 16.3465f, -77.6586f),
		new Vector3(-68.3912f, 21.8313f, -81.8292f),
		new Vector3(-68.6524f, 21.4049f, -60.8881f),
		new Vector3(-61.4034f, 16.5802f, -41.6979f)
	};

	private readonly Dictionary<int, int> _positionIndexByActor = new Dictionary<int, int>();
	private float _lastUpdateTime;

	private void Update()
	{
		if (!PhotonNetwork.InRoom || Time.time < _lastUpdateTime + 5f)
		{
			return;
		}

		_lastUpdateTime = Time.time;
		Player[] remotePlayers = PhotonNetwork.PlayerListOthers;
		AssignPositions(remotePlayers);
		RemoveDepartedPlayers(remotePlayers);
		SetPhotonSerializeDeadline(9999f);

		foreach (KeyValuePair<int, int> assignment in _positionIndexByActor)
		{
			GameNetworkUtilities.SendSpoofedRigSerialization(
				replacePosition: true,
				ConfusionPositions[assignment.Value],
				new[] { assignment.Key },
				Vector3.zero,
				Vector3.zero);
			PhotonNetwork.SendAllOutgoingCommands();
		}
	}

	private void AssignPositions(IEnumerable<Player> players)
	{
		HashSet<int> usedIndexes = new HashSet<int>(_positionIndexByActor.Values);
		foreach (Player player in players)
		{
			if (_positionIndexByActor.ContainsKey(player.ActorNumber))
			{
				continue;
			}

			for (int positionIndex = 0; positionIndex < ConfusionPositions.Length; positionIndex++)
			{
				if (usedIndexes.Add(positionIndex))
				{
					_positionIndexByActor[player.ActorNumber] = positionIndex;
					break;
				}
			}
		}
	}

	private void RemoveDepartedPlayers(IReadOnlyCollection<Player> players)
	{
		foreach (int actorNumber in _positionIndexByActor.Keys
			.Where(actor => players.All(player => player.ActorNumber != actor))
			.ToList())
		{
			_positionIndexByActor.Remove(actorNumber);
		}
	}

	private static void SetPhotonSerializeDeadline(float multiplier)
	{
		GameObject photonObject = GameObject.Find("PhotonMono");
		PhotonHandler photonHandler = photonObject != null ? photonObject.GetComponent<PhotonHandler>() : null;
		if (photonHandler != null)
		{
			Traverse.Create(photonHandler)
				.Field("nextSendTickCountOnSerialize")
				.SetValue((int)(Time.realtimeSinceStartup * multiplier));
		}
	}

	private void OnDisable()
	{
		_positionIndexByActor.Clear();
		SetPhotonSerializeDeadline(1000f);
	}

	private void OnDestroy()
	{
		_positionIndexByActor.Clear();
	}
}
