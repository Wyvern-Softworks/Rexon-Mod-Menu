// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: PublicRoomCreator
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System;
using System.Collections;
using System.Threading.Tasks;
using ExitGames.Client.Photon;
using GorillaGameModes;
using GorillaNetworking;
using GorillaTagScripts;
using Photon.Pun;
using Rexon_Menu.Core.Attributes;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using InternalState = NetworkSystemPUN.InternalState;
using Random = UnityEngine.Random;

[Mod("Create Public Room", "Overpowered", "Creates a public room.", true, 17, ModType.Toggle, false)]
internal class PublicRoomCreator : MonoBehaviour
{
	private const string RoomCodeCharacters = "ABCDEFGHIJKLMNPQRSTUVWXYZ123456789";

	private static readonly string[] PublicRoomTriggerPaths =
	{
		"Environment Objects/TriggerZonesPrefab/JoinRoomTriggersPrefab/JoinPublicRoom - Forest Tree Exit",
		"Environment Objects/TriggerZonesPrefab/JoinRoomTriggersPrefab/JoinPublicRoom - City Front",
		"Environment Objects/TriggerZonesPrefab/JoinRoomTriggersPrefab/JoinPublicRoom - Mountain For Computer",
		"Environment Objects/TriggerZonesPrefab/JoinRoomTriggersPrefab/JoinPublicRoom - Cave",
		"Environment Objects/TriggerZonesPrefab/JoinRoomTriggersPrefab/JoinPublicRoom - Canyon",
		"Environment Objects/TriggerZonesPrefab/JoinRoomTriggersPrefab/JoinPublicRoom - Beach from Forest",
		"Environment Objects/TriggerZonesPrefab/JoinRoomTriggersPrefab/JoinPublicRoom - Metropolis from City",
		"Environment Objects/TriggerZonesPrefab/JoinRoomTriggersPrefab/JoinPublicRoom - Clouds",
		"Environment Objects/TriggerZonesPrefab/JoinRoomTriggersPrefab/JoinPublicRoom - Rotating Map",
		"Environment Objects/TriggerZonesPrefab/JoinRoomTriggersPrefab/JoinPublicRoom - Basement For Computer",
		"Environment Objects/TriggerZonesPrefab/JoinRoomTriggersPrefab/JoinPublicRoom - Bayou",
		"Environment Objects/LocalObjectsPrefab/MallToMonkeBlocks/JoinPublicRoom - MonkeBlocks From Mall"
	};

	private void OnEnable()
	{
		CreatePublicRoom();
		UnityEngine.Object.Destroy(this);
	}

	private static string GenerateRoomCode()
	{
		string roomCode;
		do
		{
			char[] characters = new char[4];
			for (int index = 0; index < characters.Length; index++)
			{
				characters[index] = RoomCodeCharacters[Random.Range(0, RoomCodeCharacters.Length)];
			}

			roomCode = new string(characters);
		}
		while (GorillaComputer.instance.CheckAutoBanListForName(roomCode));

		return roomCode;
	}

	private static void CreatePublicRoom()
	{
		try
		{
			PhotonNetworkController networkController = PhotonNetworkController.Instance;
			string nearestTriggerPath = FindNearestPublicRoomTrigger();
			if (!string.IsNullOrEmpty(nearestTriggerPath))
			{
				string zoneName = GetZoneName(nearestTriggerPath);
				networkController.currentJoinTrigger = GorillaComputer.instance.GetJoinTriggerForZone(zoneName);
			}

			if (networkController.currentJoinTrigger == null)
			{
				networkController.currentJoinTrigger = GorillaComputer.instance.GetJoinTriggerForZone("forest");
			}

			Hashtable roomProperties = new Hashtable
			{
				{ "gameMode", networkController.currentJoinTrigger.GetFullDesiredGameModeString() },
				{ "platform", networkController.platformTag },
				{ "queueName", GorillaComputer.instance.currentQueue },
				{ "language", LocalisationManager.CurrentLanguage.ToString() },
				{ "fanclub", SubscriptionManager.IsLocalSubscribed() ? "true" : "false" }
			};

			RoomConfig roomConfig = new RoomConfig
			{
				isPublic = true,
				isJoinable = true,
				createIfMissing = true,
				MaxPlayers = RoomSystem.GetRoomSizeForCreate(
					networkController.currentJoinTrigger.zone,
					(GameModeType)Enum.Parse(
						typeof(GameModeType),
						GorillaComputer.instance.currentGameMode.Value,
						true),
					false,
					SubscriptionManager.IsLocalSubscribed()),
				CustomProps = roomProperties
			};

			if (NetworkSystem.Instance is NetworkSystemPUN punNetwork)
			{
				punNetwork.internalState = (InternalState)16;
			}

			_ = ForceCreateRoom(GenerateRoomCode(), roomConfig);
			GorillaTagger.Instance.StartCoroutine(MakeRoomVisibleCoroutine());
		}
		catch (Exception)
		{
		}
	}

	private static string FindNearestPublicRoomTrigger()
	{
		Vector3 playerPosition = GorillaTagger.Instance.transform.position;
		float nearestDistance = float.MaxValue;
		string nearestPath = null;

		foreach (string triggerPath in PublicRoomTriggerPaths)
		{
			GameObject triggerObject = GameObject.Find(triggerPath);
			if (triggerObject == null)
			{
				continue;
			}

			float distance = Vector3.Distance(playerPosition, triggerObject.transform.position);
			if (distance < nearestDistance)
			{
				nearestDistance = distance;
				nearestPath = triggerPath;
			}
		}

		return nearestPath;
	}

	private static string GetZoneName(string triggerPath)
	{
		return triggerPath
			.ToLowerInvariant()
			.Replace("environment objects/triggerzonesprefab/joinroomtriggersprefab/", string.Empty)
			.Replace("environment objects/localobjectsprefab/malltomonkeblocks/", string.Empty)
			.Replace("joinpublicroom - ", string.Empty)
			.Replace(",", string.Empty)
			.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0]
			.Trim();
	}

	public static async Task ForceCreateRoom(string roomCode, RoomConfig roomConfig)
	{
		if (NetworkSystem.Instance.InRoom)
		{
			await NetworkSystem.Instance.ReturnToSinglePlayer();
		}

		if (NetworkSystem.Instance is NetworkSystemPUN punNetwork)
		{
			await punNetwork.TryCreateRoom(roomCode, roomConfig);
		}
	}

	private static IEnumerator MakeRoomVisibleCoroutine()
	{
		const int maximumWaitSeconds = 20;
		int elapsedSeconds = 0;

		while (!PhotonNetwork.InRoom && elapsedSeconds < maximumWaitSeconds)
		{
			yield return new WaitForSeconds(1f);
			elapsedSeconds++;
		}

		if (!PhotonNetwork.InRoom)
		{
			yield break;
		}

		yield return new WaitForSeconds(4f);
		if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
		{
			PhotonNetwork.CurrentRoom.IsVisible = true;
			PhotonNetwork.CurrentRoom.IsOpen = true;
		}
	}
}
