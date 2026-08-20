// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.NoClip
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Rexon_Menu.Core.Attributes;
using UnityEngine;
using UnityEngine.XR;

namespace Recovered.Obfuscated;

[Mod("NoClip", "Movement", "Left trigger to phase through walls.", false, 6, ModType.Toggle, false)]
internal class NoClip : MonoBehaviour
{
	private const float TriggerThreshold = 0.4f;
	private const float ColliderScaleFactor = 10000f;

	private bool _collidersAreShrunk;
	private bool _desktopMode;
	private MeshCollider[] _colliders;

	private void OnEnable()
	{
		_desktopMode = !XRSettings.isDeviceActive;
		_colliders = Resources.FindObjectsOfTypeAll<MeshCollider>();
		if (_desktopMode)
		{
			ShrinkColliders();
		}
	}

	private void Update()
	{
		if (_desktopMode)
		{
			return;
		}

		if (ControllerInputPoller.TriggerFloat(XRNode.LeftHand) > TriggerThreshold)
		{
			ShrinkColliders();
		}
		else
		{
			RestoreColliders();
		}
	}

	private void ShrinkColliders()
	{
		if (_collidersAreShrunk || _colliders == null)
		{
			return;
		}

		foreach (MeshCollider collider in _colliders)
		{
			if (collider != null)
			{
				collider.transform.localScale /= ColliderScaleFactor;
			}
		}
		_collidersAreShrunk = true;
	}

	private void RestoreColliders()
	{
		if (!_collidersAreShrunk || _colliders == null)
		{
			return;
		}

		foreach (MeshCollider collider in _colliders)
		{
			if (collider != null)
			{
				collider.transform.localScale *= ColliderScaleFactor;
			}
		}
		_collidersAreShrunk = false;
	}

	private void OnDisable()
	{
		RestoreColliders();
		_colliders = null;
	}
}
