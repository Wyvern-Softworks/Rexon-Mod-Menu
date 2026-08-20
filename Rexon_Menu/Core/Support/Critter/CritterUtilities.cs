// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.CritterUtilities
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System;
using System.Linq;
using System.Reflection;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using AnimalType = CritterConfiguration.AnimalType;
using CreatureState = CrittersPawn.CreatureState;
using CrittersActorType = CrittersActor.CrittersActorType;
using CritterEvent = CrittersManager.CritterEvent;
using Random = UnityEngine.Random;

namespace Recovered.Obfuscated;

internal static class CritterUtilities
{
	private const string RemoteSpawnCreatureRpc = "RemoteSpawnCreature";
	private const string RemoteUpdateCritterDataRpc = "RemoteUpdateCritterData";
	private const string PhotonRunViewUpdateMethod = "RunViewUpdate";

	private static readonly FieldInfo LocalInZoneField = typeof(CrittersManager).GetField(
		"localInZone",
		BindingFlags.Instance | BindingFlags.NonPublic);

	private static float s_nextCritterSpawnTime;
	private static float s_nextFoodSpawnTime;
	private static float s_nextHoneySpawnTime;
	private static float s_nextCrashFoodSpawnTime;

	public static void SpawnCritter(
		Vector3 position,
		Vector3 velocity,
		float appearanceSize,
		CreatureState state,
		float cooldownSeconds = 0.1f)
	{
		if (Time.time < s_nextCritterSpawnTime || !IsLocalInCritterZone())
		{
			return;
		}

		s_nextCritterSpawnTime = Time.time + cooldownSeconds;
		bool movedRig = MoveRigNearIfNeeded(position);
		try
		{
			RetireCrittersAtCapacity();
			EnsureCritterAuthority();

			CrittersManager manager = (CrittersManager)CrittersManager.instance;
			CrittersPawn critter = (CrittersPawn)manager.SpawnActor(CrittersActorType.Creature, -1);
			if (critter == null)
			{
				return;
			}

			critter.SetTemplate(manager.creatureIndex.GetRandomCritterType(null));
			critter.currentState = (CreatureState)0;
			((CrittersActor)critter).MoveActor(position, Quaternion.identity, false, true, true);
			((CrittersActor)critter).localCanStore = true;
			critter.SetState(state);
			critter.currentState = state;
			((CrittersActor)critter).SetImpulseVelocity(velocity, velocity);
			((CrittersActor)critter).SetImpulse();
			critter.regionId = 1;

			AnimalType[] animalTypes = Enum.GetValues(typeof(AnimalType))
				.Cast<AnimalType>()
				.Where(type => (int)type != -1)
				.ToArray();
			critter.creatureConfiguration.animalType = animalTypes[Random.Range(0, animalTypes.Length)];
			critter.SetState(state);

			string hatName = critter.visuals.hats[Random.Range(1, critter.visuals.hats.Length)].name;
			critter.visuals.SetAppearance(new CritterAppearance(hatName, appearanceSize));

			CritterAppearance appearance = critter.visuals.Appearance;
			((NetworkView)manager).GetView.SendRpc(
				RemoteSpawnCreatureRpc,
				RpcTarget.Others,
				((CrittersActor)critter).actorId,
				critter.regionId,
				appearance.WriteToRPCData());

			GameNetworkUtilities.FlushAndReplayNetworkTraffic();
			((CrittersActor)critter).SetImpulseVelocity(velocity, velocity);
			((CrittersActor)critter).SetImpulse();
		}
		catch (Exception)
		{
		}
		finally
		{
			RestoreRigAfterMove(movedRig);
		}
	}

	public static void CrashPlayerWithFood(Player targetPlayer)
	{
		if (Time.time < s_nextCrashFoodSpawnTime || !IsLocalInCritterZone())
		{
			return;
		}

		s_nextCrashFoodSpawnTime = Time.time + 0.1f;
		EnsureCritterAuthority();
		try
		{
			CrittersFood food = SpawnExtremeFoodActor();
			if (food != null)
			{
				((NetworkView)CrittersManager.instance).GetView.SendRpc(
					RemoteUpdateCritterDataRpc,
					targetPlayer,
					new object[1] { BuildFoodRpcData(food, 1000f) });
			}
		}
		catch (Exception)
		{
		}
	}

	public static void CrashAllWithFood()
	{
		if (Time.time < s_nextCrashFoodSpawnTime || !IsLocalInCritterZone())
		{
			return;
		}

		s_nextCrashFoodSpawnTime = Time.time + 0.1f;
		EnsureCritterAuthority();
		try
		{
			CrittersFood food = SpawnExtremeFoodActor();
			if (food != null)
			{
				((NetworkView)CrittersManager.instance).GetView.SendRpc(
					RemoteUpdateCritterDataRpc,
					RpcTarget.Others,
					new object[1] { BuildFoodRpcData(food, 1000f) });
			}
		}
		catch (Exception)
		{
		}
	}

	public static void SpawnHoney(Vector3 position, Quaternion rotation, float cooldownSeconds = 0.1f)
	{
		if (Time.time < s_nextHoneySpawnTime || !IsLocalInCritterZone())
		{
			return;
		}

		s_nextHoneySpawnTime = Time.time + cooldownSeconds;
		bool movedRig = MoveRigNearIfNeeded(position);
		try
		{
			((CrittersManager)CrittersManager.instance).guard.RequestOwnershipImmediately(NoOp);
			CrittersManager manager = (CrittersManager)CrittersManager.instance;
			CrittersStickyTrap trap = (CrittersStickyTrap)manager.SpawnActor(CrittersActorType.StickyTrap, -1);
			if (trap == null)
			{
				return;
			}

			CrittersStickyGoo goo = (CrittersStickyGoo)manager.SpawnActor(CrittersActorType.StickyGoo, -1);
			if (goo == null)
			{
				return;
			}

			manager.TriggerEvent((CritterEvent)2, ((CrittersActor)trap).actorId, position, rotation);
			((CrittersActor)goo).MoveActor(position, rotation, false, true, true);
			((CrittersActor)goo).SetImpulseVelocity(Vector3.zero, Vector3.zero);
			((CrittersActor)goo).UpdateImpulses(true, false);
		}
		catch (Exception)
		{
		}
		finally
		{
			RestoreRigAfterMove(movedRig);
		}
	}

	public static void SpawnFoodImmediate(
		Vector3 position,
		Quaternion rotation,
		Vector3 velocity,
		float foodValue)
	{
		if (!IsLocalInCritterZone())
		{
			return;
		}

		EnsureCritterAuthority();
		TrySpawnAndBroadcastFood(position, rotation, velocity, foodValue);
	}

	public static void SpawnFoodNear(
		Vector3 position,
		Quaternion rotation,
		Vector3 velocity,
		float foodValue,
		float cooldownSeconds = 0.1f)
	{
		if (Time.time < s_nextFoodSpawnTime || !IsLocalInCritterZone())
		{
			return;
		}

		s_nextFoodSpawnTime = Time.time + cooldownSeconds;
		bool movedRig = MoveRigNearIfNeeded(position);
		try
		{
			EnsureCritterAuthority();
			TrySpawnAndBroadcastFood(position, rotation, velocity, foodValue);
		}
		finally
		{
			RestoreRigAfterMove(movedRig);
		}
	}

	public static bool IsLocalInCritterZone()
	{
		try
		{
			return LocalInZoneField?.GetValue(CrittersManager.instance) is bool inZone && inZone;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public static void EnsureCritterAuthority()
	{
		try
		{
			CrittersManager manager = (CrittersManager)CrittersManager.instance;
			if (!manager.guard.PlayerHasAuthority((NetPlayer)PhotonNetwork.LocalPlayer))
			{
				manager.guard.RequestOwnershipImmediately(NoOp);
			}
		}
		catch (Exception)
		{
		}
	}

	private static bool MoveRigNearIfNeeded(Vector3 position)
	{
		if (Vector3.Distance(GorillaTagger.Instance.offlineVRRig.transform.position, position) < 3f)
		{
			return false;
		}

		GorillaTagger.Instance.offlineVRRig.enabled = false;
		GorillaTagger.Instance.offlineVRRig.transform.position = position;
		try
		{
			typeof(PhotonNetwork)
				.GetMethod(PhotonRunViewUpdateMethod, BindingFlags.Static | BindingFlags.NonPublic)?
				.Invoke(null, Array.Empty<object>());
			PhotonNetwork.SendAllOutgoingCommands();
		}
		catch (Exception)
		{
		}

		return true;
	}

	private static void RestoreRigAfterMove(bool movedRig)
	{
		if (movedRig)
		{
			GorillaTagger.Instance.offlineVRRig.enabled = true;
		}
	}

	private static void RetireCrittersAtCapacity()
	{
		CrittersManager manager = (CrittersManager)CrittersManager.instance;
		if (manager.crittersPawns.Count < 100)
		{
			return;
		}

		foreach (CrittersPawn critter in manager.crittersPawns)
		{
			critter.SetState((CreatureState)10);
		}
	}

	private static CrittersFood SpawnExtremeFoodActor()
	{
		CrittersFood food = (CrittersFood)((CrittersManager)CrittersManager.instance)
			.SpawnActor(CrittersActorType.Food, 0);
		if (food == null)
		{
			return null;
		}

		Vector3 handPosition = GorillaTagger.Instance.offlineVRRig.rightHandTransform.position;
		((CrittersActor)food).MoveActor(
			handPosition + new Vector3(0f, 5f, 0f),
			Quaternion.LookRotation(handPosition, handPosition),
			false,
			true,
			true);
		food.SpawnData(9999999f, 9999999f, 1000f);
		Vector3 extremeImpulse = new Vector3(20f, 20f, 20f);
		((CrittersActor)food).SetImpulseVelocity(extremeImpulse, extremeImpulse);
		((CrittersActor)food).UpdateImpulses(true, false);
		((CrittersActor)food).SetImpulse();
		return food;
	}

	private static void TrySpawnAndBroadcastFood(
		Vector3 position,
		Quaternion rotation,
		Vector3 velocity,
		float foodValue)
	{
		try
		{
			CrittersFood food = (CrittersFood)((CrittersManager)CrittersManager.instance)
				.SpawnActor(CrittersActorType.Food, 0);
			if (food == null)
			{
				return;
			}

			((CrittersActor)food).MoveActor(position, rotation, false, true, true);
			food.SpawnData(9999999f, 9999999f, foodValue);
			((CrittersActor)food).SetImpulseVelocity(velocity, velocity);
			((CrittersActor)food).UpdateImpulses(true, false);
			((CrittersActor)food).SetImpulse();
			((NetworkView)CrittersManager.instance).GetView.SendRpc(
				RemoteUpdateCritterDataRpc,
				RpcTarget.All,
				new object[1] { BuildFoodRpcData(food, foodValue) });
		}
		catch (Exception)
		{
		}
	}

	private static object[] BuildFoodRpcData(CrittersFood food, float foodValue)
	{
		CrittersActor actor = food;
		return new object[]
		{
			actor.actorId,
			actor.lastImpulseTime,
			actor.lastImpulsePosition,
			actor.lastImpulseVelocity,
			actor.lastImpulseAngularVelocity,
			actor.lastImpulseQuaternion,
			actor.parentActorId,
			actor.isEnabled,
			actor.subObjectIndex,
			9999999,
			9999999f,
			foodValue
		};
	}

	private static void NoOp()
	{
	}
}
