// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Rig.PunchBoost
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using GorillaLocomotion;
using Rexon_Menu.Core.Attributes;
using UnityEngine;
using UnityEngine.XR;

namespace Rexon_Menu.Core.Modules.Rig;

[Mod("Punch Boost", "Rig", "Swing your hands fast to boost forward.", false, 23, ModType.Toggle, false)]
internal class PunchBoost : MonoBehaviour
{
	private Vector3 _previousLeftHandPosition;

	private Vector3 _previousRightHandPosition;

	private float _nextBoostTime;

	private Rigidbody _rigidbody;


	private void Start()
	{
		_rigidbody = GTPlayer.Instance.GetComponent<Rigidbody>();
		_previousLeftHandPosition = GorillaTagger.Instance.leftHandTransform.position;
		_previousRightHandPosition = GorillaTagger.Instance.rightHandTransform.position;
	}

	private void Update()
	{
		if (_rigidbody != null)
		{
			Vector3 leftHandVelocity = (GorillaTagger.Instance.leftHandTransform.position - _previousLeftHandPosition) / Time.deltaTime;
			Vector3 rightHandVelocity = (GorillaTagger.Instance.rightHandTransform.position - _previousRightHandPosition) / Time.deltaTime;
			if (leftHandVelocity.magnitude > 8f && Time.time > _nextBoostTime)
			{
				_nextBoostTime = Time.time + 0.3f;
				_rigidbody.velocity += leftHandVelocity.normalized * 15f;
				GorillaTagger.Instance.DoVibration(XRNode.LeftHand, 0.8f, 0.15f);
			}
			if (rightHandVelocity.magnitude > 8f && Time.time > _nextBoostTime)
			{
				_nextBoostTime = Time.time + 0.3f;
				_rigidbody.velocity += rightHandVelocity.normalized * 15f;
				GorillaTagger.Instance.DoVibration(XRNode.RightHand, 0.8f, 0.15f);
			}
			_previousLeftHandPosition = GorillaTagger.Instance.leftHandTransform.position;
			_previousRightHandPosition = GorillaTagger.Instance.rightHandTransform.position;
		}
	}

	private void OnDisable()
	{
		_nextBoostTime = 0f;
	}
}
