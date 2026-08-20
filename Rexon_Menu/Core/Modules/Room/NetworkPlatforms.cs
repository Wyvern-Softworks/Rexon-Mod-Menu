// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Room.NetworkPlatforms
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

using Object = UnityEngine.Object;

namespace Rexon_Menu.Core.Modules.Room;

[Mod("Network Platforms", "Room", "See other Rexon users' platforms.", false, 8, ModType.Toggle, false)]
internal class NetworkPlatforms : MonoBehaviour
{
	private const byte CreateRightPlatformEvent = 69;
	private const byte CreateLeftPlatformEvent = 70;
	private const byte DestroyRightPlatformEvent = 71;
	private const byte DestroyLeftPlatformEvent = 72;

	private static readonly Vector3 PlatformScale = new Vector3(0.0125f, 0.28f, 0.3825f);
	private static readonly Color PlatformColor = new Color32(83, 5, 185, byte.MaxValue);
	private readonly Dictionary<int, GameObject> _rightPlatforms = new Dictionary<int, GameObject>();
	private readonly Dictionary<int, GameObject> _leftPlatforms = new Dictionary<int, GameObject>();
	private bool _isSubscribed;

	private void OnEnable()
	{
		if (!_isSubscribed)
		{
			PhotonNetwork.NetworkingClient.EventReceived += OnPlatformEvent;
			_isSubscribed = true;
		}
	}

	private void OnDisable()
	{
		if (_isSubscribed)
		{
			PhotonNetwork.NetworkingClient.EventReceived -= OnPlatformEvent;
			_isSubscribed = false;
		}
		ClearAllPlatforms();
	}

	private void OnPlatformEvent(EventData eventData)
	{
		switch (eventData.Code)
		{
		case CreateRightPlatformEvent:
			CreateOrUpdatePlatform(_rightPlatforms, eventData.Sender, eventData.CustomData);
			break;
		case CreateLeftPlatformEvent:
			CreateOrUpdatePlatform(_leftPlatforms, eventData.Sender, eventData.CustomData);
			break;
		case DestroyRightPlatformEvent:
			DestroyPlatform(_rightPlatforms, eventData.Sender);
			break;
		case DestroyLeftPlatformEvent:
			DestroyPlatform(_leftPlatforms, eventData.Sender);
			break;
		}
	}

	private static void CreateOrUpdatePlatform(
		IDictionary<int, GameObject> platforms,
		int actorNumber,
		object customData)
	{
		if (!(customData is object[] payload) ||
			payload.Length < 2 ||
			!(payload[0] is Vector3 position) ||
			!(payload[1] is Quaternion rotation))
		{
			return;
		}

		DestroyPlatform(platforms, actorNumber);
		GameObject platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
		platform.transform.localScale = PlatformScale;
		platform.transform.position = position;
		platform.transform.rotation = rotation;
		Renderer renderer = platform.GetComponent<Renderer>();
		if (renderer != null)
		{
			renderer.material.color = PlatformColor;
		}
		platforms[actorNumber] = platform;
	}

	private static void DestroyPlatform(IDictionary<int, GameObject> platforms, int actorNumber)
	{
		if (platforms.TryGetValue(actorNumber, out GameObject platform))
		{
			if (platform != null)
			{
				Object.Destroy(platform);
			}
			platforms.Remove(actorNumber);
		}
	}

	private void ClearAllPlatforms()
	{
		DestroyPlatforms(_rightPlatforms);
		DestroyPlatforms(_leftPlatforms);
	}

	private static void DestroyPlatforms(IDictionary<int, GameObject> platforms)
	{
		foreach (GameObject platform in platforms.Values)
		{
			if (platform != null)
			{
				Object.Destroy(platform);
			}
		}
		platforms.Clear();
	}
}
