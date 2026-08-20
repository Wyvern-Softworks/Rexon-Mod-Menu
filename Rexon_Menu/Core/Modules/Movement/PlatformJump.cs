// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Movement.PlatformJump
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

using Object = UnityEngine.Object;

namespace Rexon_Menu.Core.Modules.Movement;

[Mod("Platform Jump", "Movement", "Right grip spawns a temporary platform below you.", false, 9, ModType.Toggle, false)]
internal class PlatformJump : MonoBehaviour
{
	private const float SpawnInterval = 0.5f;
	private const float PlatformLifetime = 1f;

	private float _lastSpawnTime;
	private GameObject _platform;

	private void Update()
	{
		if (!ControllerInputPoller.instance.rightGrab || Time.time < _lastSpawnTime + SpawnInterval)
		{
			return;
		}

		_lastSpawnTime = Time.time;
		try
		{
			DestroyPlatform();
			_platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
			_platform.transform.position = GorillaTagger.Instance.bodyCollider.transform.position - Vector3.up * 0.5f;
			_platform.transform.localScale = new Vector3(0.3f, 0.05f, 0.3f);
			_platform.GetComponent<Renderer>().enabled = false;

			Rigidbody rigidbody = _platform.GetComponent<Rigidbody>();
			if (rigidbody != null)
			{
				Object.Destroy(rigidbody);
			}

			Physics.IgnoreCollision(_platform.GetComponent<Collider>(), GorillaTagger.Instance.headCollider);
			Object.Destroy(_platform, PlatformLifetime);
		}
		catch (Exception)
		{
		}
	}

	private void DestroyPlatform()
	{
		if (_platform != null)
		{
			Object.Destroy(_platform);
			_platform = null;
		}
	}

	private void OnDisable()
	{
		DestroyPlatform();
	}
}
