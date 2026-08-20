// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.GadgetCrashGun
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using Rexon_Menu.Core.Attributes;
using Rexon_Menu.Core.Patches;
using Rexon_Menu_Mat;
using UnityEngine;

using Object = UnityEngine.Object;

namespace Recovered.Obfuscated;

[Mod("Crash Gun", "Super Infection/Casual [MASTERCLIENT]", "Crashes target player.", false, 31, ModType.Toggle, false)]
internal class GadgetCrashGun : MonoBehaviour
{
	private const string GunId = "CrashGun";
	private static readonly Vector3 SpawnPosition = new Vector3(-61.9586f, 230.2118f, -61.7674f);

	private readonly HashSet<Player> _processedTargets = new HashSet<Player>();
	private Player _target;
	private GameObject _targetMarker;
	private float _lastTargetChangeTime;

	private void Update()
	{
		if (!PhotonNetwork.InRoom ||
			!GameNetworkUtilities.IsSuperInfectionMode() ||
			!PhotonNetwork.IsMasterClient)
		{
			ResetGun();
			return;
		}

		GameNetworkUtilities.CacheGameEntityTypeIds();
		GunController.GunResult gun = GunController.GetGunResult(
			GunId,
			targetPlayers: true,
			1f,
			allowSingleTargetLock: true);
		if (!gun.IsActive)
		{
			DestroyTargetMarker();
			return;
		}

		if (gun.IsShooting && gun.Target != null && Time.time > _lastTargetChangeTime + 1f)
		{
			_lastTargetChangeTime = Time.time;
			if (_target == gun.Target)
			{
				_target = null;
			}
			else if (!_processedTargets.Contains(gun.Target))
			{
				_target = gun.Target;
			}
		}
		if (_target != null && !PhotonNetwork.PlayerListOthers.Contains(_target))
		{
			_target = null;
		}

		if (_target == null)
		{
			DestroyTargetMarker();
			return;
		}

		UpdateTargetMarker();
		SpawnCrashEntities();
	}

	private void SpawnCrashEntities()
	{
		if (_target == null)
		{
			return;
		}

		List<GameEntityCreateData> entities = new List<GameEntityCreateData>();
		foreach (string entityName in GameNetworkUtilities.BuildableEntityNames)
		{
			if (!GameNetworkUtilities.EntityTypeIdsByName.TryGetValue(entityName, out int entityTypeId))
			{
				continue;
			}

			for (int entityIndex = 0; entityIndex < 100; entityIndex++)
			{
				entities.Add(new GameEntityCreateData
				{
					entityTypeId = entityTypeId,
					position = SpawnPosition,
					rotation = Quaternion.LookRotation(Vector3.down),
					createData = 0L
				});
			}
		}

		GameNetworkUtilities.SpawnGameEntities(entities, _target, destroyAfterCreation: true);
		_processedTargets.Add(_target);
		_target = null;
	}

	private void UpdateTargetMarker()
	{
		VRRig targetRig = MatBridge.GetVRRigFor(_target);
		if (targetRig == null)
		{
			return;
		}

		if (_targetMarker == null)
		{
			_targetMarker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			_targetMarker.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
			Object.Destroy(_targetMarker.GetComponent<Collider>());
			_targetMarker.GetComponent<Renderer>().material =
				ShaderPatch.CreateTransparentMaterial(Color.yellow);
		}
		_targetMarker.transform.position = targetRig.transform.position + Vector3.up * 0.3f;
	}

	private void DestroyTargetMarker()
	{
		if (_targetMarker != null)
		{
			Object.Destroy(_targetMarker);
			_targetMarker = null;
		}
	}

	private void ResetGun()
	{
		_target = null;
		_processedTargets.Clear();
		DestroyTargetMarker();
		GunController.Release(GunId);
	}

	private void OnDisable()
	{
		ResetGun();
	}
}
