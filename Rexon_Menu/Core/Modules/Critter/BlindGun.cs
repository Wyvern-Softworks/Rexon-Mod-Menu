// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.BlindGun
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

using Object = UnityEngine.Object;

namespace Recovered.Obfuscated;

[Mod("Blind Gun [CRITTER] [MASTER]", "Critter", "Blind target with giant food.", false, 13, ModType.Toggle, false)]
internal class BlindGun : MonoBehaviour
{
	private const string GunId = "BlindGunCritter";
	private const float TargetToggleCooldown = 1f;
	private const float FoodSpawnInterval = 2f;
	private const float FoodDistance = 7f;
	private const float FoodHeight = 6f;
	private const float SpawnDelay = 0.04f;
	private const float FoodScale = 150f;

	private Player _targetPlayer;
	private float _lastTargetChangeTime;
	private float _lastFoodSpawnTime;
	private Coroutine _spawnCoroutine;

	private void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			_targetPlayer = null;
			GunController.Release(GunId);
			return;
		}

		GunController.GunResult gun = GunController.GetGunResult(
			GunId,
			targetPlayers: true,
			TargetToggleCooldown,
			allowSingleTargetLock: true);
		if (gun.IsShooting &&
			gun.IsActive &&
			gun.Target != null &&
			Time.time > _lastTargetChangeTime + TargetToggleCooldown)
		{
			_lastTargetChangeTime = Time.time;
			GunController.MarkFired(GunId);
			_targetPlayer = _targetPlayer == gun.Target ? null : gun.Target;
		}

		if (_targetPlayer == null)
		{
			return;
		}

		if (!Array.Exists(PhotonNetwork.PlayerListOthers, IsSelectedTarget))
		{
			_targetPlayer = null;
			return;
		}

		VRRig targetRig = GorillaGameManager.StaticFindRigForPlayer((NetPlayer)_targetPlayer);
		if (targetRig == null || Time.time <= _lastFoodSpawnTime + FoodSpawnInterval)
		{
			return;
		}

		_lastFoodSpawnTime = Time.time;
		if (_spawnCoroutine != null)
		{
			GorillaTagger.Instance.StopCoroutine(_spawnCoroutine);
		}
		_spawnCoroutine = GorillaTagger.Instance.StartCoroutine(
			SpawnSurroundingFood(targetRig.transform.position, FoodDistance));
	}

	private IEnumerator SpawnSurroundingFood(Vector3 center, float distance)
	{
		Vector3[] offsets =
		{
			new(distance, 0f, 0f),
			new(-distance, 0f, 0f),
			new(0f, 0f, distance),
			new(0f, 0f, -distance)
		};

		foreach (Vector3 offset in offsets)
		{
			Vector3 spawnPosition = center + offset + Vector3.up * FoodHeight;
			CritterUtilities.SpawnFoodImmediate(
				spawnPosition,
				Quaternion.identity,
				Vector3.zero,
				FoodScale);
			yield return new WaitForSeconds(SpawnDelay);
		}

		_spawnCoroutine = null;
	}

	private void OnDisable()
	{
		_targetPlayer = null;
		if (_spawnCoroutine != null)
		{
			GorillaTagger.Instance.StopCoroutine(_spawnCoroutine);
			_spawnCoroutine = null;
		}
		GunController.Release(GunId);
	}

	private bool IsSelectedTarget(Player player)
	{
		return player == _targetPlayer;
	}
}
