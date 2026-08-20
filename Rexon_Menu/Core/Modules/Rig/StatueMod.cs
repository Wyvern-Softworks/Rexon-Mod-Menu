// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.StatueMod
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Collections.Generic;
using GorillaLocomotion;
using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Statue Mod", "Rig", "Ghost when being looked at.", false, 49, ModType.Toggle, false)]
internal class StatueMod : MonoBehaviour
{
	private Vector3 _positionWhenObserved = Vector3.zero;

	private readonly float _gazeAngleDegrees = 45f;

	private int? _visibilityLayerMask;

	private void Update()
	{
		bool isBeingObserved = false;
		Vector3 position = GorillaTagger.Instance.offlineVRRig.transform.position;
		foreach (VRRig rig in VRRigCache.ActiveRigs)
		{
			if (rig == GorillaTagger.Instance.offlineVRRig)
			{
				continue;
			}

			Transform head = rig.headMesh.transform;
			Vector3 directionToLocalPlayer = (position - head.position).normalized;
			if (Vector3.Angle(head.forward, directionToLocalPlayer) <= _gazeAngleDegrees &&
				Physics.Raycast(head.position, directionToLocalPlayer, out RaycastHit hit,
					Vector3.Distance(head.position, position), GetVisibilityLayerMask()) &&
				(hit.collider.gameObject == GorillaTagger.Instance.offlineVRRig.gameObject ||
				 hit.collider.transform.IsChildOf(GorillaTagger.Instance.offlineVRRig.transform)))
			{
				isBeingObserved = true;
				break;
			}
		}

		GorillaTagger.Instance.offlineVRRig.enabled = !isBeingObserved;
		if (isBeingObserved)
		{
			if (_positionWhenObserved == Vector3.zero)
			{
				_positionWhenObserved = GTPlayer.Instance.transform.position;
			}
		}
		else if (_positionWhenObserved != Vector3.zero)
		{
			_positionWhenObserved = Vector3.zero;
		}
	}

	private int GetVisibilityLayerMask()
	{
		if (!_visibilityLayerMask.HasValue)
		{
			_visibilityLayerMask = ~(
				(1 << LayerMask.NameToLayer("TransparentFX")) |
				(1 << LayerMask.NameToLayer("Ignore Raycast")) |
				(1 << LayerMask.NameToLayer("Zone")) |
				(1 << LayerMask.NameToLayer("Gorilla Trigger")) |
				(1 << LayerMask.NameToLayer("Gorilla Boundary")) |
				(1 << LayerMask.NameToLayer("GorillaCosmetics")) |
				(1 << LayerMask.NameToLayer("GorillaParticle")));
		}
		return _visibilityLayerMask ?? (int)GTPlayer.Instance.locomotionEnabledLayers;
	}

	private void OnDisable()
	{
		if (_positionWhenObserved != Vector3.zero)
		{
			_positionWhenObserved = Vector3.zero;
		}
		GorillaTagger.Instance.offlineVRRig.enabled = true;
	}
}
