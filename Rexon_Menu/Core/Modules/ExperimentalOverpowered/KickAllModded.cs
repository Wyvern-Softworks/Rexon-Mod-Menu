// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.ExperimentalOverpowered.KickAllModded
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Collections.Generic;
using ExitGames.Client.Photon;
using GorillaGameModes;
using Photon.Pun;
using Photon.Realtime;
using Recovered.Obfuscated;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Rexon_Menu.Core.Modules.ExperimentalOverpowered;

[Mod("Kick All [MODDED ONLY]", "Experimental Overpowered [D?]", "Flings everyone out of a modded lobby. Only works in modded.", false, 10, ModType.Toggle, false)]
internal class KickAllModded : MonoBehaviour
{
	private const string GameModeProperty = "gameMode";
	private const string ModdedModeMarker = "modded";
	private const string ForcedGuardianMode = "forestDEFAULTGuardian";
	private const float GameModeUpdateInterval = 15f;
	private const float LaunchInterval = 0.1f;
	private const float LaunchForce = 100f;
	private const float MaximumLaunchSpeed = 20f;

	private static readonly Vector3 ExitPosition = new(-66.7583f, 12.5176f, -78.9244f);

	private float _lastGameModeUpdateTime;
	private float _lastLaunchTime;

	private void Update()
	{
		if (!PhotonNetwork.InRoom || !IsModdedRoom())
		{
			return;
		}

		if (!PhotonNetwork.IsMasterClient)
		{
			PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
			return;
		}

		UpdateGameModeProperty();
		GorillaGameManager activeGameMode = GameMode.ActiveGameMode;
		if (activeGameMode == null ||
			(int)activeGameMode.GameType() != 8 ||
			Time.time <= _lastLaunchTime + LaunchInterval)
		{
			return;
		}

		_lastLaunchTime = Time.time;
		foreach (Player player in PhotonNetwork.PlayerListOthers)
		{
			VRRig rig = RigUtilities.GetRig(player);
			if (rig == null)
			{
				continue;
			}

			Vector3 direction = (ExitPosition - rig.transform.position).normalized;
			Vector3 velocity = Vector3.ClampMagnitude(LaunchForce * direction, MaximumLaunchSpeed);
			RoomSystem.LaunchPlayer(NetworkSystem.Instance.GetNetPlayerByID(player.ActorNumber), velocity);
		}
	}

	private static bool IsModdedRoom()
	{
		Photon.Realtime.Room room = PhotonNetwork.CurrentRoom;
		if (room == null || ((RoomInfo)room).CustomProperties == null)
		{
			return false;
		}

		Dictionary<object, object> properties = (Dictionary<object, object>)(object)((RoomInfo)room).CustomProperties;
		return properties.TryGetValue(GameModeProperty, out object value) &&
			value != null &&
			value.ToString().ToLowerInvariant().Contains(ModdedModeMarker);
	}

	private void UpdateGameModeProperty()
	{
		if (Time.time <= _lastGameModeUpdateTime + GameModeUpdateInterval)
		{
			return;
		}

		_lastGameModeUpdateTime = Time.time;
		Hashtable properties = new();
		((Dictionary<object, object>)(object)properties).Add(GameModeProperty, ForcedGuardianMode);
		PhotonNetwork.CurrentRoom.SetCustomProperties(properties, null, null);
	}
}
