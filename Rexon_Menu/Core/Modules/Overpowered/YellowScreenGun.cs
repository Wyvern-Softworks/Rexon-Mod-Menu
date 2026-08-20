// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.YellowScreenGun
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Linq;
using System.Reflection;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Rexon_Menu.Core.Attributes;
using Rexon_Menu.Core.Patches;
using Rexon_Menu_Mat;
using UnityEngine;

using Object = UnityEngine.Object;

namespace Recovered.Obfuscated;

[Mod("Yellow Screen Gun", "Overpowered", "Gives target a yellow screen. [BE INFRONT OF THEM]", false, 13, ModType.Toggle, false)]
internal class YellowScreenGun : MonoBehaviour
{
	private const string GunId = "YellowScreenGun";

	private Player _target;
	private GameObject _targetMarker;
	private float _lastTargetChangeTime;
	private float _lastProjectileTime;

	private void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			ResetGun();
			return;
		}

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
			_target = _target == gun.Target ? null : gun.Target;
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
		SendYellowProjectile();
	}

	private void SendYellowProjectile()
	{
		if (_target == null || Time.time < _lastProjectileTime + 0.8f)
		{
			return;
		}

		VRRig targetRig = MatBridge.GetVRRigFor(_target);
		if (targetRig == null)
		{
			return;
		}

		Vector3 origin = GorillaTagger.Instance.rightHandTransform.position;
		if (Vector3.Distance(GorillaTagger.Instance.offlineVRRig.transform.position, origin) > 4f)
		{
			return;
		}

		_lastProjectileTime = Time.time;
		Vector3 targetPosition = targetRig.headMesh.transform.position + new Vector3(0f, 0.1f, 0f);
		Vector3 velocity = (targetPosition - origin).normalized * 50f;
		Color32 yellow = new Color32(byte.MaxValue, byte.MaxValue, 0, byte.MaxValue);
		GorillaTagger.Instance.offlineVRRig.RightThrowableProjectileIndex = 12;

		object[] projectileData =
		{
			origin,
			velocity,
			2,
			IncrementProjectileCount(),
			true,
			yellow.r,
			yellow.g,
			yellow.b,
			yellow.a
		};
		object[] eventPayload =
		{
			NetworkSystem.Instance.ServerTimestamp,
			(byte)0,
			projectileData
		};

		RaiseEventOptions options = new RaiseEventOptions
		{
			TargetActors = new[] { _target.ActorNumber }
		};
		PhotonNetwork.RaiseEvent(3, eventPayload, options, SendOptions.SendUnreliable);
		GameNetworkUtilities.FlushAndReplayNetworkTraffic();
	}

	private static int IncrementProjectileCount()
	{
		FieldInfo projectileCount = typeof(VRRig).GetField(
			"projectileCount",
			BindingFlags.Instance | BindingFlags.NonPublic);
		if (projectileCount == null)
		{
			return Random.Range(1, 9999);
		}

		VRRig localRig = GorillaTagger.Instance.offlineVRRig;
		int nextCount = (int)projectileCount.GetValue(localRig) + 1;
		projectileCount.SetValue(localRig, nextCount);
		return nextCount;
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
		DestroyTargetMarker();
		GunController.Release(GunId);
	}

	private void OnDisable()
	{
		ResetGun();
	}
}
