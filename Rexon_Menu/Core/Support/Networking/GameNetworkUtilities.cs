// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.GameNetworkUtilities
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ExitGames.Client.Photon;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaTagScripts;
using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

using ProgressionData = SIPlayer.ProgressionData;
using ProjectileSource = RoomSystem.ProjectileSource;
using RaiseEventBatch = Photon.Pun.PhotonNetwork.RaiseEventBatch;
using SerializeViewBatch = Photon.Pun.PhotonNetwork.SerializeViewBatch;
using StatusEffects = RoomSystem.StatusEffects;
using ZoneState = GameEntityManager.ZoneState;
using Random = UnityEngine.Random;

namespace Recovered.Obfuscated;

internal static class GameNetworkUtilities
{
	public static class TentacleHelper
	{
		public static bool IsActive;
		public static int FrameCounter;

		public static void SendRigAtPosition(VRRig rig, Vector3 position, bool flushImmediately = false)
		{
			PhotonView view = RigUtilities.GetPhotonView(rig);
			if (view == null)
			{
				return;
			}

			Vector3 originalPosition = rig.transform.position;
			rig.transform.position = position;
			SendPhotonViewSerialization(view);
			rig.transform.position = originalPosition;
			rig.leftHandLink.grabbedLink = null;
			rig.leftHandLink.IsTentacleGrab = false;

			if (flushImmediately)
			{
				PhotonNetwork.SendAllOutgoingCommands();
			}
		}

		public static bool ShouldRunTentacleUpdate(bool force = false)
		{
			return force || FrameCounter % 3 == 0;
		}

		public static void ConfigureTentacleGrab(VRRig localRig, NetPlayer targetPlayer, VRRig targetRig, bool targetLeftHand)
		{
			TakeMyHand_HandLink localHand = localRig.leftHandLink;
			localHand.grabbedPlayer = targetPlayer;
			localHand.grabbedHandIsLeft = targetLeftHand;
			localHand.grabbedLink = targetLeftHand ? targetRig.leftHandLink : targetRig.rightHandLink;
			localHand.IsTentacleGrab = true;
			localHand.isGroundedHand = true;
			localHand.isGroundedButt = true;
			localRig.rightHandLink.isGroundedHand = true;
			localRig.rightHandLink.isGroundedButt = true;
		}

		public static bool ChooseLeftTargetHand(VRRig rig)
		{
			bool leftHandActive = rig.handSync / 10000 % 10 >= 5;
			bool rightHandActive = rig.handSync / 10 % 10 >= 5;
			return leftHandActive || !rightHandActive;
		}

		public static Vector3 OffsetPullPosition(Vector3 position)
		{
			return position + new Vector3(0.3f, 0f, 0.3f);
		}

		public static bool TryGetActiveHands(VRRig rig, out bool leftHandActive, out bool rightHandActive)
		{
			leftHandActive = rig.handSync / 10000 % 10 >= 5;
			rightHandActive = rig.handSync / 10 % 10 >= 5;
			return leftHandActive || rightHandActive;
		}

		public static bool IsAtLeastThreeMetersAway(float distance)
		{
			return distance >= 3f;
		}

		public static void ResetTentacleGrab(VRRig rig)
		{
			TakeMyHand_HandLink leftHand = rig.leftHandLink;
			leftHand.grabbedPlayer = null;
			leftHand.grabbedLink = null;
			leftHand.grabbedHandIsLeft = false;
			leftHand.IsTentacleGrab = false;
			leftHand.TentacleOffset = Vector3.zero;
			leftHand.isGroundedHand = false;
			leftHand.isGroundedButt = false;
			rig.rightHandLink.isGroundedHand = false;
			rig.rightHandLink.isGroundedButt = false;
		}
	}

	private const int EntityBatchSize = 25;

	internal static Shader XrayShader;

	public static int ProjectileColorIndex = 2;
	public static readonly string[] ProjectileColorNames =
	{
		"Black", "Blue", "Rainbow", "Cyan", "Gray", "Green", "Brown", "Magenta", "Red", "White", "Yellow"
	};

	public static int ProjectileSpeedIndex;
	public static readonly string[] ProjectileSpeedNames = { "Default", "Fast", "VeryFast", "Slow" };
	public static readonly float[] ProjectileSpeeds = { 50f, 200f, 10000f, 15f };

	public static int ImpactColorIndex;
	public static readonly string[] ImpactColorNames =
	{
		"Black", "Blue", "Rainbow", "Cyan", "Gray", "Green", "Brown", "Magenta", "Red", "White", "Yellow"
	};

	public static readonly Dictionary<string, int> EntityTypeIdsByName = new Dictionary<string, int>();
	public static readonly string[] BuildableEntityNames =
	{
		"SIGadgetDashYoyo", "StiltGadget Fixed", "StiltGadget FixedScaledShort", "StiltGadget FixedScaledLong",
		"StiltGadget Extendo", "PlatformDeployerGadget", "PlatformDeployerGadget_Bouncy", "SI_Resource_WeirdGear",
		"SI_Resource_StrangeWood", "SI_Resource_BouncySand", "SI_Resource_FloppyMetal", "SI_Resource_VibratingSpring",
		"SIMonkeIdol", "StiltGadget Turkey", "WristJetGadgetPropellor", "StiltGadget Motorized2",
		"StiltGadget Motorized3", "WeakBlasterGadget", "ChargeBlasterGadget", "MegaChargeBlasterGadget",
		"BlastLobberGadget", "LongBlasterGadget"
	};
	public static readonly string[] ResourceEntityNames =
	{
		"SI_Resource_WeirdGear", "SI_Resource_BouncySand", "SI_Resource_FloppyMetal", "SI_Resource_VibratingSpring"
	};

	internal static readonly Dictionary<VRRig, int> OriginalRigMaterialIndexes = new Dictionary<VRRig, int>();
	internal static BuilderTable CachedBuilderTable;
	internal static BuilderTableNetworking CachedBuilderNetworking;

	private static readonly List<int> SpawnedEntityNetworkIds = new List<int>();
	private static readonly HashSet<string> KnownModPropertyKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
	{
		"genesis", "HP_Left", "GrateVersion", "void", "BANANAOS", "GC", "CarName", "6p72ly3j85pau2g9mda6ib8px",
		"FPS-Nametags for Zlothy", "cronos", "ORBIT", "Violet On Top", "MP25", "GorillaWatch", "InfoWatch",
		"BananaPhone", "Vivid", "RGBA", "cheese is gouda", "shirtversion", "gpronouns", "gfaces", "monkephone",
		"pmversion", "gtrials", "msp", "gorillastats", "using gorilladrift", "monkehavocversion", "tictactoe",
		"ccolor", "imposter", "spectapeversion", "cats", "made by biotest05 :3", "fys cool magic mod", "colour",
		"chainedtogether", "goofywalkversion", "void_menu_open", "violetpaiduser", "violetfree", "obsidianmc", "dark",
		"hidden menu", "oblivionuser", "hgrehngio889584739_hugb\n", "eyerock reborn", "asteroidlite", "elux",
		"cokecosmetics", "GFaces", "github.com/maroon-shadow/SimpleBoards", "ObsidianMC", "hgrehngio889584739_hugb",
		"GTrials", "github.com/ZlothY29IQ/GorillaMediaDisplay", "github.com/ZlothY29IQ/TooMuchInfo",
		"github.com/ZlothY29IQ/RoomUtils-IW", "github.com/ZlothY29IQ/MonkeClick", "github.com/ZlothY29IQ/MonkeClick-CI",
		"github.com/ZlothY29IQ/MonkeRealism", "MediaPad", "GorillaCinema", "ChainedTogetherActive", "GPronouns",
		"CSVersion", "github.com/ZlothY29IQ/Zloth-RecRoomRig", "ShirtProperties", "GorillaShirts", "GS",
		"6XpyykmrCthKhFeUfkYGxv7xnXpoe2", "Body Tracking", "Body Estimation", "Gorilla Track", "CustomMaterial",
		"I like cheese", "silliness", "emotewheel", "untitled"
	};

	private static readonly int TransparentFxLayer = LayerMask.NameToLayer("TransparentFX");
	private static readonly int IgnoreRaycastLayer = LayerMask.NameToLayer("Ignore Raycast");
	private static readonly int ZoneLayer = LayerMask.NameToLayer("Zone");
	private static readonly int GorillaTriggerLayer = LayerMask.NameToLayer("Gorilla Trigger");
	private static readonly int GorillaBoundaryLayer = LayerMask.NameToLayer("Gorilla Boundary");
	private static readonly int GorillaCosmeticsLayer = LayerMask.NameToLayer("GorillaCosmetics");
	private static readonly int GorillaParticleLayer = LayerMask.NameToLayer("GorillaParticle");

	private static byte[] CapturedOutgoingPacket;
	private static Dictionary<string, SnowballThrowable> SnowballsByAnchorName;
	private static Coroutine DisableSnowballHandle;
	private static float RoomJoinTime = -1f;
	private static bool RoomTimerStarted;
	private static bool ThrowablesInitialized;
	private static float ThrowablesReadyTime = -1f;
	private static float LastProgressionUpdateTime = -999f;
	private static bool EntityTypeCacheInitialized;
	private static BuilderSetManager CachedBuilderSetManager;

	public static Color ProjectileColor => GetPaletteColor(ProjectileColorIndex);
	public static float ProjectileSpeed => ProjectileSpeeds[ProjectileSpeedIndex];
	public static Color ImpactColor => GetPaletteColor(ImpactColorIndex);

	private static IEnumerator DisableSnowballCoroutine(SnowballThrowable snowball)
	{
		yield return new WaitForSeconds(0.3f);
		if (snowball != null)
		{
			snowball.SetSnowballActiveLocal(false);
		}
	}

	private static IEnumerator SendDestroyBatches(List<int> networkIds, bool waitBetweenBatches)
	{
		GameEntityManager manager = GameEntityManager.activeManager;
		if (manager == null)
		{
			yield break;
		}

		int batchCount = (networkIds.Count + EntityBatchSize - 1) / EntityBatchSize;
		for (int batchIndex = 0; batchIndex < batchCount; batchIndex++)
		{
			int[] batch = networkIds.Skip(batchIndex * EntityBatchSize).Take(EntityBatchSize).ToArray();
			manager.photonView.SendRpc("DestroyItemRPC", RpcTarget.All, batch);
			FlushAndReplayNetworkTraffic();
			yield return waitBetweenBatches ? new WaitForSeconds(0.3f) : null;
		}
	}

	private static IEnumerator SendCreateBatches(
		List<GameEntityCreateData> entities,
		int batchSize,
		GameEntityManager manager,
		Player targetPlayer,
		bool destroyAfterCreation)
	{
		UnlockSuperInfectionProgression();
		int batchCount = (entities.Count + batchSize - 1) / batchSize;

		for (int batchIndex = 0; batchIndex < batchCount; batchIndex++)
		{
			List<GameEntityCreateData> batch = entities.Skip(batchIndex * batchSize).Take(batchSize).ToList();
			int[] networkIds = new int[batch.Count];
			int[] entityTypeIds = new int[batch.Count];
			long[] packedPositions = new long[batch.Count];
			int[] packedRotations = new int[batch.Count];
			long[] createData = new long[batch.Count];
			int[] parentNetworkIds = new int[batch.Count];
			int invalidParentId = manager.GetNetIdFromEntityId(GameEntityId.Invalid);

			for (int entityIndex = 0; entityIndex < batch.Count; entityIndex++)
			{
				GameEntityCreateData entity = batch[entityIndex];
				int indexWithinType = 1 + manager.FactoryGetBuiltInEntityCountById(entity.entityTypeId);
				int networkId = manager.CreateNetId(indexWithinType);
				SpawnedEntityNetworkIds.Add(networkId);
				networkIds[entityIndex] = networkId;
				entityTypeIds[entityIndex] = entity.entityTypeId;
				packedPositions[entityIndex] = BitPackUtils.PackWorldPosForNetwork(entity.position);
				packedRotations[entityIndex] = BitPackUtils.PackQuaternionForNetwork(entity.rotation);
				createData[entityIndex] = GetGameEntityCreateData(entity.entityTypeId);
				parentNetworkIds[entityIndex] = invalidParentId;
			}

			object[] rpcArguments = { networkIds, entityTypeIds, packedPositions, packedRotations, createData, parentNetworkIds };
			if (targetPlayer != null)
			{
				manager.photonView.SendRpc("CreateItemRPC", targetPlayer, rpcArguments);
			}
			else
			{
				manager.photonView.SendRpc("CreateItemRPC", destroyAfterCreation ? RpcTarget.Others : RpcTarget.All, rpcArguments);
			}
			yield return new WaitForSeconds(0.1f);
		}

		if (!destroyAfterCreation)
		{
			yield break;
		}
		yield return new WaitForSeconds(5f);
		if (GameEntityManager.activeManager != null && NetworkSystem.Instance.IsMasterClient)
		{
			GorillaTagger.Instance.StartCoroutine(SendDestroyBatches(SpawnedEntityNetworkIds, waitBetweenBatches: false));
		}
	}

	public static bool IsSuperInfectionMode()
	{
		if (!PhotonNetwork.InRoom || PhotonNetwork.CurrentRoom?.CustomProperties == null)
		{
			return false;
		}
		if (!PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("gameMode", out object gameMode) || gameMode == null)
		{
			return false;
		}
		string mode = gameMode.ToString();
		return mode.Contains("SuperInfect") || mode.Contains("SuperCasual");
	}

	public static void CacheGameEntityTypeIds()
	{
		if (EntityTypeCacheInitialized)
		{
			return;
		}
		SuperInfectionManager superInfection = SuperInfectionManager.activeSuperInfectionManager;
		GameEntityManager manager = superInfection == null ? null : superInfection.gameEntityManager;
		if (manager == null)
		{
			return;
		}
		foreach (KeyValuePair<int, GameObject> prefab in manager.itemPrefabFactory)
		{
			if (prefab.Value != null && !EntityTypeIdsByName.ContainsKey(prefab.Value.name))
			{
				EntityTypeIdsByName[prefab.Value.name] = prefab.Key;
			}
		}
		EntityTypeCacheInitialized = EntityTypeIdsByName.Count > 0;
	}

	public static GorillaTagManager GetTagManager()
	{
		return GorillaGameManager.instance as GorillaTagManager;
	}

	public static long GetGameEntityCreateData(int entityTypeId)
	{
		SIPlayer localPlayer = SIPlayer.LocalPlayer;
		if (localPlayer != null && SIPlayer.progressionSO != null &&
			SIPlayer.progressionSO.TryGetUpgradeTypeByEntityTypeId(entityTypeId, out SIUpgradeType upgradeType))
		{
			SIUpgradeSet upgrades = localPlayer.GetUpgrades((SITechTreePageId)SIUpgradeTypeSystem.GetPageId(upgradeType));
			return upgrades.GetCreateData(localPlayer);
		}
		return localPlayer == null ? 0L : localPlayer.ActorNr;
	}

	public static void DestroyAllGameEntities()
	{
		GameEntityManager manager = GameEntityManager.activeManager;
		if (manager == null || manager.entities.Count == 0 || !NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		List<int> networkIds = manager.entities.Where(entity => entity != null)
			.Select(entity => manager.GetNetIdFromEntityId(entity.id)).ToList();
		GorillaTagger.Instance.StartCoroutine(SendDestroyBatches(networkIds, waitBetweenBatches: true));
	}

	public static bool AreBuilderComponentsAvailable()
	{
		CacheBuilderComponents();
		return CachedBuilderTable != null && CachedBuilderNetworking != null;
	}

	public static void FlushAndReplayNetworkTraffic()
	{
		ApplyUnlimitedNetworkLimits();
		try
		{
			ReplayCapturedPacketBurst();
		}
		catch (Exception)
		{
		}
		PhotonNetwork.SendAllOutgoingCommands();
	}

	public static void VibrateHand(bool isLeftHand, float strengthScale = 0.5f, float durationScale = 0.05f)
	{
		GorillaTagger.Instance.StartVibration(isLeftHand,
			GorillaTagger.Instance.tagHapticStrength * strengthScale,
			GorillaTagger.Instance.tagHapticDuration * durationScale);
	}

	public static void SendSpoofedRigSerialization(bool replacePosition, Vector3 bodyPosition, int[] targetActors,
		Vector3 rightHandPosition, Vector3 leftHandPosition, int timestamp = -1)
	{
		int networkTimestamp = timestamp == -1 ? PhotonNetwork.ServerTimestamp : timestamp;
		Action<List<object>> fullBatchMutator = null;
		Action<List<object>> remainingBatchMutator = null;
		if (replacePosition)
		{
			Vector3 originalBody = GorillaTagger.Instance.offlineVRRig.transform.position;
			Vector3 originalRightHand = GorillaTagger.Instance.offlineVRRig.rightHandTransform.position;
			Vector3 originalLeftHand = GorillaTagger.Instance.offlineVRRig.leftHandTransform.position;
			List<KeyValuePair<Vector3, long>> allReplacements = new List<KeyValuePair<Vector3, long>>
			{
				new KeyValuePair<Vector3, long>(originalBody, BitPackUtils.PackWorldPosForNetwork(bodyPosition))
			};
			if (rightHandPosition != Vector3.zero)
			{
				allReplacements.Add(new KeyValuePair<Vector3, long>(originalRightHand, BitPackUtils.PackWorldPosForNetwork(rightHandPosition)));
			}
			if (leftHandPosition != Vector3.zero)
			{
				allReplacements.Add(new KeyValuePair<Vector3, long>(originalLeftHand, BitPackUtils.PackWorldPosForNetwork(leftHandPosition)));
			}
			List<KeyValuePair<Vector3, long>> bodyReplacement = new List<KeyValuePair<Vector3, long>> { allReplacements[0] };
			fullBatchMutator = payload => ReplacePackedPositions(payload, allReplacements);
			remainingBatchMutator = payload => ReplacePackedPositions(payload, bodyReplacement);
		}
		SendSerializedViews(targetActors, networkTimestamp, fullBatchMutator, remainingBatchMutator);
	}

	private static void ReplayCapturedPacketBurst()
	{
		LoadBalancingPeer peer = PhotonNetwork.NetworkingClient.LoadBalancingPeer;
		Type peerType = peer.GetType();
		MethodInfo sendReliable = peerType.GetMethod("SendReliable", BindingFlags.Instance | BindingFlags.NonPublic);
		MethodInfo sendUnreliable = peerType.GetMethod("SendUnreliable", BindingFlags.Instance | BindingFlags.NonPublic);
		if (CapturedOutgoingPacket != null)
		{
			MethodInfo sender = sendReliable ?? sendUnreliable;
			if (sender != null)
			{
				for (int i = 0; i < 1300; i++)
				{
					sender.Invoke(peer, new object[] { CapturedOutgoingPacket });
				}
			}
		}
		FieldInfo queueField = peerType.GetField("outgoingStreamQueue", BindingFlags.Instance | BindingFlags.NonPublic);
		if (queueField?.GetValue(peer) is IList queue && queue.Count > 0)
		{
			CapturedOutgoingPacket = queue[queue.Count - 1] as byte[];
		}
	}

	public static bool CanLocalPlayerTag(Player player)
	{
		return GorillaGameManager.instance.LocalCanTag((NetPlayer)PhotonNetwork.LocalPlayer, (NetPlayer)player);
	}

	internal static Color GetPaletteColor(int index)
	{
		switch (index)
		{
			case 0: return Color.black;
			case 1: return Color.blue;
			case 2: return Color.HSVToRGB(Time.time * 0.5f % 1f, 1f, 1f);
			case 3: return Color.cyan;
			case 4: return Color.gray;
			case 5: return Color.green;
			case 6: return new Color(0.5f, 0.3f, 0.1f);
			case 7: return Color.magenta;
			case 8: return Color.red;
			case 9: return Color.white;
			case 10: return Color.yellow;
			default: return Color.black;
		}
	}

	public static bool CanPlayerTag(Player source, Player target)
	{
		return GorillaGameManager.instance.LocalCanTag((NetPlayer)source, (NetPlayer)target);
	}

	public static Rigidbody GetPlayerRigidbody()
	{
		return GTPlayer.Instance.bodyCollider.attachedRigidbody;
	}

	public static void SendStatusEffect(StatusEffects statusEffect, RaiseEventOptions options)
	{
		PhotonNetwork.RaiseEvent(3, new object[]
		{
			NetworkSystem.Instance.ServerTimestamp, (byte)2, new object[] { (int)statusEffect }
		}, options, SendOptions.SendUnreliable);
		FlushAndReplayNetworkTraffic();
	}

	public static void CacheBuilderComponents()
	{
		if (CachedBuilderTable == null)
		{
			BuilderTable.TryGetBuilderTableForZone((GTZone)25, out CachedBuilderTable);
		}
		if (CachedBuilderNetworking == null && CachedBuilderTable != null)
		{
			CachedBuilderNetworking = CachedBuilderTable.builderNetworking;
		}
		if (CachedBuilderSetManager == null)
		{
			GameObject managerObject = GameObject.Find("Networking Scripts/BuilderSetManager");
			if (managerObject != null)
			{
				CachedBuilderSetManager = managerObject.GetComponent<BuilderSetManager>();
			}
		}
	}

	public static bool EnqueueRawPhotonEvent(byte eventCode, object payload, RaiseEventOptions options, SendOptions sendOptions)
	{
		int actorNumber = PhotonNetwork.IsMasterClient ? PhotonNetwork.PlayerListOthers[0].ActorNumber : PhotonNetwork.MasterClient.ActorNumber;
		ParameterDictionary parameters = new ParameterDictionary
		{
			[(byte)254] = actorNumber,
			[(byte)67] = float.NaN,
			[(byte)244] = eventCode
		};
		if (options != null)
		{
			parameters[(byte)247] = (byte)options.CachingOption;
			if (options.TargetActors != null)
			{
				parameters[(byte)252] = options.TargetActors;
			}
			else if (options.InterestGroup > 0)
			{
				parameters[(byte)240] = options.InterestGroup;
			}
			else if ((int)options.Receivers > 0)
			{
				parameters[(byte)246] = (byte)options.Receivers;
			}
			if (options.Flags.HttpForward)
			{
				parameters[(byte)234] = (byte)4;
			}
		}
		if (payload != null)
		{
			parameters[(byte)245] = payload;
		}
		PeerBase peerBase = ((PhotonPeer)PhotonNetwork.NetworkingClient.LoadBalancingPeer).peerBase;
		StreamBuffer message = peerBase.SerializeOperationToMessage(253, parameters, (EgMessageType)2, false);
		return peerBase.EnqueuePhotonMessage(message, sendOptions);
	}

	public static void RequestMonkeBallGameState(Dictionary<int, int> teamsByActorNumber)
	{
		MonkeBallGame game = MonkeBallGame.Instance;
		if (game == null)
		{
			return;
		}
		IEnumerable<NetPlayer> players = NetworkSystem.Instance.AllNetPlayers.Where(player => teamsByActorNumber.ContainsKey(player.ActorNumber));
		game.photonView.SendRpc("RequestSetGameStateRPC", RpcTarget.All,
			2,
			PhotonNetwork.Time + game.gameDuration - 1f,
			players.Select(player => player.ActorNumber).ToArray(),
			players.Select(player => teamsByActorNumber[player.ActorNumber]).ToArray(),
			new int[game.team.Count],
			game.startingBalls.Select(ball => BitPackUtils.PackHandPosRotForNetwork(ball.transform.position, ball.transform.rotation)).ToArray(),
			game.startingBalls.Select(ball => BitPackUtils.PackWorldPosForNetwork(ball.gameBall.GetVelocity())).ToArray());
	}

	public static void UnlockSuperInfectionProgression()
	{
		if (Time.time < LastProgressionUpdateTime + 2f)
		{
			return;
		}
		SuperInfectionManager manager = SuperInfectionManager.activeSuperInfectionManager;
		if (manager == null || NetworkSystem.Instance == null || !NetworkSystem.Instance.InRoom || SIPlayer.LocalPlayer == null)
		{
			return;
		}
		ProgressionData progression = SIPlayer.LocalPlayer.CurrentProgression;
		if (progression.techTreeData == null)
		{
			return;
		}
		LastProgressionUpdateTime = Time.time;
		for (int pageIndex = 0; pageIndex < progression.techTreeData.Length; pageIndex++)
		{
			bool[] page = progression.techTreeData[pageIndex];
			if (page == null)
			{
				continue;
			}
			for (int upgradeIndex = 0; upgradeIndex < page.Length; upgradeIndex++)
			{
				page[upgradeIndex] = true;
			}
		}
		object[] progressionData =
		{
			progression.resourceArray, progression.limitedDepositTimeArray, progression.techTreeData,
			progression.stashedQuests, progression.stashedBonusPoints, progression.bonusProgress,
			progression.currentQuestIds, progression.currentQuestProgresses
		};
		manager.photonView.SendRpc("SIClientToClientRPC", RpcTarget.All, 0, progressionData);
	}

	public static bool HasKnownModMarker(Player player)
	{
		VRRig rig = RigUtilities.GetRig(player);
		if (rig == null)
		{
			return false;
		}
		if (rig._playerOwnedCosmetics.Contains("FIRST LOGIN"))
		{
			return true;
		}
		NetPlayer creator = rig.Creator;
		if (creator == null)
		{
			return false;
		}
		foreach (DictionaryEntry property in creator.GetPlayerRef().CustomProperties)
		{
			if (KnownModPropertyKeys.Contains(property.Key.ToString()))
			{
				return true;
			}
		}
		return false;
	}

	public static void SpawnGameEntity(int entityTypeId, Vector3 position, Quaternion rotation)
	{
		if (!NetworkSystem.Instance.IsMasterClient || GameEntityManager.activeManager == null)
		{
			return;
		}
		UnlockSuperInfectionProgression();
		InitializeSuperInfectionZone();
		GameEntityManager manager = GameEntityManager.activeManager;
		manager.SetZoneState((ZoneState)3);
		int indexWithinType = 1 + manager.FactoryGetBuiltInEntityCountById(entityTypeId);
		int networkId = manager.CreateNetId(indexWithinType);
		int invalidParentId = manager.GetNetIdFromEntityId(GameEntityId.Invalid);
		manager.photonView.SendRpc("CreateItemRPC", RpcTarget.All,
			new[] { networkId }, new[] { entityTypeId },
			new[] { BitPackUtils.PackWorldPosForNetwork(position) },
			new[] { BitPackUtils.PackQuaternionForNetwork(rotation) },
			new[] { GetGameEntityCreateData(entityTypeId) }, new[] { invalidParentId });
		FlushAndReplayNetworkTraffic();
	}

	public static void SendMalformedSplashEffect()
	{
		Vector3 position = GorillaTagger.Instance.offlineVRRig.transform.position;
		GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlaySplashEffect", RpcTarget.All,
			new object[] { position, Quaternion.identity, float.NaN, 0.5f, false, true });
		PhotonNetwork.SendAllOutgoingCommands();
	}

	public static void LaunchNetworkedProjectile(Vector3 position, Vector3 velocity, Color color)
	{
		if (!PhotonNetwork.InRoom)
		{
			RoomJoinTime = -1f;
			RoomTimerStarted = false;
			ThrowablesInitialized = false;
			return;
		}
		if (!RoomTimerStarted)
		{
			RoomJoinTime = Time.time;
			RoomTimerStarted = true;
		}
		if (RoomJoinTime < 0f || Time.time < RoomJoinTime + 5f)
		{
			return;
		}
		if (!ThrowablesInitialized)
		{
			if (InitializeThrowableCosmetics())
			{
				ThrowablesInitialized = true;
				ThrowablesReadyTime = Time.time;
				SnowballsByAnchorName = null;
			}
			return;
		}
		if (Time.time < ThrowablesReadyTime + 2f)
		{
			return;
		}

		velocity = GTExt.ClampMagnitudeSafe(velocity, 50f);
		color.a = 1f;
		EnsureSnowballCache();
		if (SnowballsByAnchorName == null || !SnowballsByAnchorName.TryGetValue("SnowballRightAnchor(Clone)", out SnowballThrowable snowball) || snowball == null)
		{
			SnowballsByAnchorName = null;
			return;
		}
		snowball.SetSnowballActiveLocal(true);
		if (DisableSnowballHandle != null)
		{
			GorillaTagger.Instance.StopCoroutine(DisableSnowballHandle);
		}
		DisableSnowballHandle = GorillaTagger.Instance.StartCoroutine(DisableSnowballCoroutine(snowball));
		bool originalRandomizeColor = snowball.randomizeColor;
		snowball.randomizeColor = true;
		VRRig.LocalRig.SetThrowableProjectileColor(false, (Color32)color);
		int[] targetActors = PhotonNetwork.PlayerListOthers.Select(player => player.ActorNumber).ToArray();
		SendSerializedViews(targetActors, PhotonNetwork.ServerTimestamp, null, null);
		PhotonNetwork.SendAllOutgoingCommands();
		float scale = snowball.transform.lossyScale.x;
		SlingshotProjectile projectile = snowball.LaunchSnowballLocal(position, velocity, scale, true, color);
		if (PhotonNetwork.InRoom)
		{
			Color32 packedColor = color;
			RoomSystem.SendLaunchProjectile(position, velocity, (ProjectileSource)2, projectile.myProjectileCount, true,
				packedColor.r, packedColor.g, packedColor.b, packedColor.a);
			PhotonNetwork.SendAllOutgoingCommands();
		}
		snowball.randomizeColor = originalRandomizeColor;
		FlushAndReplayNetworkTraffic();
	}

	public static void SetPhotonSerializeTickMultiplier(float multiplier)
	{
		GameObject photonObject = GameObject.Find("PhotonMono");
		PhotonHandler handler = photonObject == null ? null : photonObject.GetComponent<PhotonHandler>();
		if (handler != null)
		{
			Traverse.Create(handler).Field("nextSendTickCountOnSerialize").SetValue((int)(Time.realtimeSinceStartup * multiplier));
		}
	}

	public static void SpawnGameEntities(List<GameEntityCreateData> entities, Player targetPlayer = null, bool destroyAfterCreation = false)
	{
		if (!NetworkSystem.Instance.IsMasterClient || GameEntityManager.activeManager == null)
		{
			return;
		}
		InitializeSuperInfectionZone();
		GameEntityManager.activeManager.SetZoneState((ZoneState)3);
		int batchSize = destroyAfterCreation ? 20 : 30;
		GorillaTagger.Instance.StartCoroutine(SendCreateBatches(entities, batchSize,
			GameEntityManager.activeManager, targetPlayer, destroyAfterCreation));
	}

	public static void SendSpoofedBodyPosition(Vector3 position, int[] targetActors)
	{
		long packedPosition = BitPackUtils.PackWorldPosForNetwork(position);
		Action<List<object>> replaceAll = payload => ReplaceEveryPackedPosition(payload, packedPosition);
		SendSerializedViews(targetActors, PhotonNetwork.ServerTimestamp, replaceAll, replaceAll);
	}

	public static Vector3 GetHeadForward()
	{
		return GorillaTagger.Instance.headCollider.transform.forward;
	}

	public static int GetGameplayLayerMask()
	{
		return ~((1 << TransparentFxLayer) | (1 << IgnoreRaycastLayer) | (1 << ZoneLayer) |
			(1 << GorillaTriggerLayer) | (1 << GorillaBoundaryLayer) | (1 << GorillaCosmeticsLayer) |
			(1 << GorillaParticleLayer));
	}

	public static void TakePhotonViewOwnership(PhotonView view)
	{
		if (view == null || view.AmOwner)
		{
			return;
		}
		view.OwnerActorNr = PhotonNetwork.LocalPlayer.ActorNumber;
		view.ControllerActorNr = PhotonNetwork.LocalPlayer.ActorNumber;
		view.RequestOwnership();
		view.TransferOwnership(PhotonNetwork.LocalPlayer);
		RequestableOwnershipGuard guard = view.GetComponent<RequestableOwnershipGuard>();
		if (guard != null)
		{
			guard.actualOwner = NetworkSystem.Instance.LocalPlayer;
			guard.currentOwner = NetworkSystem.Instance.LocalPlayer;
			guard.RequestTheCurrentOwnerFromAuthority();
			guard.TransferOwnership(NetworkSystem.Instance.LocalPlayer, "");
			guard.TransferOwnershipFromToRPC(PhotonNetwork.LocalPlayer, guard.ownershipRequestNonce, default(PhotonMessageInfo));
			guard.GetAuthoritativePlayer();
			guard.RequestTheCurrentOwnerFromAuthority();
			guard.giveCreatorAbsoluteAuthority = true;
		}
	}

	public static void SendPhotonViewSerialization(PhotonView view, RaiseEventOptions options = null, int timestampOffset = 0)
	{
		if (!PhotonNetwork.InRoom || view == null)
		{
			return;
		}
		List<object> serializedView = PhotonNetwork.OnSerializeWrite(view);
		if (serializedView == null)
		{
			return;
		}
		RaiseEventBatch batchKey = new RaiseEventBatch
		{
			Reliable = (int)view.Synchronization == 1 || view.mixedModeIsReliable,
			Group = view.Group
		};
		if (!PhotonNetwork.serializeViewBatches.TryGetValue(batchKey, out SerializeViewBatch batch))
		{
			batch = new SerializeViewBatch(batchKey, 2);
			PhotonNetwork.serializeViewBatches[batchKey] = batch;
		}
		batch.Add(serializedView);
		RaiseEventOptions defaultOptions = PhotonNetwork.serializeRaiseEvOptions;
		RaiseEventOptions sendOptions = options == null ? defaultOptions : new RaiseEventOptions
		{
			CachingOption = defaultOptions.CachingOption,
			Flags = defaultOptions.Flags,
			InterestGroup = defaultOptions.InterestGroup,
			TargetActors = options.TargetActors,
			Receivers = options.Receivers
		};
		batch.ObjectUpdates[0] = PhotonNetwork.ServerTimestamp + timestampOffset;
		batch.ObjectUpdates[1] = PhotonNetwork.currentLevelPrefix == 0 ? null : (object)(byte)PhotonNetwork.currentLevelPrefix;
		PhotonNetwork.NetworkingClient.OpRaiseEvent(batch.Batch.Reliable ? (byte)206 : (byte)201,
			batch.ObjectUpdates, sendOptions, batch.Batch.Reliable ? SendOptions.SendReliable : SendOptions.SendUnreliable);
		batch.Clear();
	}

	public static void DisablePhotonRateLimitsAndFlush()
	{
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		ApplyUnlimitedNetworkLimits();
		PhotonNetwork.SendAllOutgoingCommands();
	}

	private static void ApplyUnlimitedNetworkLimits()
	{
		try
		{
			MonkeAgent agent = MonkeAgent.instance;
			agent.rpcErrorMax = int.MaxValue;
			agent.rpcCallLimit = int.MaxValue;
			agent.logErrorMax = int.MaxValue;
			PhotonNetwork.MaxResendsBeforeDisconnect = int.MaxValue;
			PhotonNetwork.QuickResends = int.MaxValue;
		}
		catch (Exception)
		{
		}
	}

	private static void InitializeSuperInfectionZone()
	{
		SuperInfectionManager manager = SuperInfectionManager.activeSuperInfectionManager;
		if (manager != null && manager.zoneSuperInfection != null)
		{
			manager.zoneSuperInfection.OnZoneInit();
		}
	}

	private static bool InitializeThrowableCosmetics()
	{
		VRRig localRig = VRRig.LocalRig;
		if (localRig == null || localRig.cosmeticsObjectRegistry == null)
		{
			return false;
		}
		bool foundAny = false;
		for (int materialIndex = 1; materialIndex < 256; materialIndex++)
		{
			if (CosmeticsV2Spawner_Dirty.GetThrowableIDFromMaterialIndex(false, materialIndex, out string throwableId) &&
				!string.IsNullOrEmpty(throwableId) && throwableId != "null")
			{
				localRig.cosmeticsObjectRegistry.Cosmetic(throwableId);
				foundAny = true;
			}
			if (CosmeticsV2Spawner_Dirty.GetThrowableIDFromMaterialIndex(true, materialIndex, out throwableId) &&
				!string.IsNullOrEmpty(throwableId) && throwableId != "null")
			{
				localRig.cosmeticsObjectRegistry.Cosmetic(throwableId);
				foundAny = true;
			}
		}
		return foundAny;
	}

	private static void EnsureSnowballCache()
	{
		if (SnowballsByAnchorName != null)
		{
			return;
		}
		SnowballsByAnchorName = new Dictionary<string, SnowballThrowable>();
		SnowballMaker[] makers = { SnowballMaker.leftHandInstance, SnowballMaker.rightHandInstance };
		foreach (SnowballMaker maker in makers)
		{
			if (maker == null)
			{
				continue;
			}
			foreach (SnowballThrowable snowball in maker.snowballs)
			{
				string anchorName = snowball.transform.parent.gameObject.name;
				if (!SnowballsByAnchorName.ContainsKey(anchorName))
				{
					SnowballsByAnchorName.Add(anchorName, snowball);
				}
			}
		}
	}

	private static void SendSerializedViews(int[] targetActors, int timestamp,
		Action<List<object>> fullBatchMutator, Action<List<object>> remainingBatchMutator)
	{
		NonAllocDictionary<int, PhotonView>.PairIterator views = PhotonNetwork.photonViewList.GetEnumerator();
		while (views.MoveNext())
		{
			PhotonView view = views.Current.Value;
			if ((int)view.Synchronization == 0 || !view.IsMine || !view.isActiveAndEnabled || PhotonNetwork.blockedSendingGroups.Contains(view.Group))
			{
				continue;
			}
			List<object> serializedView = PhotonNetwork.OnSerializeWrite(view);
			if (serializedView == null)
			{
				continue;
			}
			RaiseEventBatch key = new RaiseEventBatch
			{
				Reliable = (int)view.Synchronization == 1 || view.mixedModeIsReliable,
				Group = view.Group
			};
			if (!PhotonNetwork.serializeViewBatches.TryGetValue(key, out SerializeViewBatch batch))
			{
				batch = new SerializeViewBatch(key, 2);
				PhotonNetwork.serializeViewBatches.Add(key, batch);
			}
			batch.Add(serializedView);
			if (batch.ObjectUpdates.Count == batch.ObjectUpdates.Capacity)
			{
				SendSerializationBatch(batch, targetActors, timestamp, fullBatchMutator);
			}
		}
		foreach (SerializeViewBatch batch in PhotonNetwork.serializeViewBatches.Values)
		{
			SendSerializationBatch(batch, targetActors, timestamp, remainingBatchMutator);
		}
		PhotonNetwork.SendAllOutgoingCommands();
	}

	private static void SendSerializationBatch(SerializeViewBatch batch, int[] targetActors, int timestamp,
		Action<List<object>> payloadMutator)
	{
		batch.ObjectUpdates[0] = timestamp;
		batch.ObjectUpdates[1] = PhotonNetwork.currentLevelPrefix == 0 ? null : (object)new byte?[] { PhotonNetwork.currentLevelPrefix };
		payloadMutator?.Invoke(batch.ObjectUpdates);
		RaiseEventOptions options = new RaiseEventOptions { TargetActors = targetActors };
		PhotonNetwork.RaiseEventInternal(batch.Batch.Reliable ? (byte)206 : (byte)201,
			batch.ObjectUpdates, options, batch.Batch.Reliable ? SendOptions.SendReliable : SendOptions.SendUnreliable);
		batch.Clear();
	}

	private static void ReplacePackedPositions(object payload, IList<KeyValuePair<Vector3, long>> replacements)
	{
		if (payload is IDictionary dictionary)
		{
			foreach (object key in dictionary.Keys.Cast<object>().ToList())
			{
				object value = dictionary[key];
				if (value is long packed)
				{
					dictionary[key] = ReplacePackedPosition(packed, replacements);
				}
				else
				{
					ReplacePackedPositions(value, replacements);
				}
			}
		}
		else if (payload is IList list)
		{
			for (int i = 0; i < list.Count; i++)
			{
				object value = list[i];
				if (value is long packed)
				{
					list[i] = ReplacePackedPosition(packed, replacements);
				}
				else
				{
					ReplacePackedPositions(value, replacements);
				}
			}
		}
	}

	private static long ReplacePackedPosition(long packed, IList<KeyValuePair<Vector3, long>> replacements)
	{
		Vector3 unpacked = BitPackUtils.UnpackWorldPosFromNetwork(packed);
		long result = packed;
		foreach (KeyValuePair<Vector3, long> replacement in replacements)
		{
			if (Vector3.Distance(unpacked, replacement.Key) <= 0.2f)
			{
				result = replacement.Value;
			}
		}
		return result;
	}

	private static void ReplaceEveryPackedPosition(object payload, long replacement)
	{
		if (payload is IDictionary dictionary)
		{
			foreach (object key in dictionary.Keys.Cast<object>().ToList())
			{
				object value = dictionary[key];
				if (value is long)
				{
					dictionary[key] = replacement;
				}
				else
				{
					ReplaceEveryPackedPosition(value, replacement);
				}
			}
		}
		else if (payload is IList list)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i] is long)
				{
					list[i] = replacement;
				}
				else
				{
					ReplaceEveryPackedPosition(list[i], replacement);
				}
			}
		}
	}
}
