// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Rig.TimeRewind
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Collections.Generic;
using GorillaLocomotion;
using Rexon_Menu.Core.Attributes;
using UnityEngine;
using UnityEngine.XR;

namespace Rexon_Menu.Core.Modules.Rig;

[Mod("Time Rewind [ALL BUTTONS + GRIPS]", "Rig", "Rewind back through your recent positions.", false, 33, ModType.Toggle, false)]
internal sealed class TimeRewind : MonoBehaviour
{
	private const int MaximumRecordedPositions = 50;
	private const float SampleIntervalSeconds = 0.2f;

	private readonly List<Vector3> _positions = new();
	private float _lastSampleAt;
	private bool _rewinding;
	private int _rewindOffset;
	private Rigidbody _playerBody;

	private void Start()
	{
		_playerBody = GTPlayer.Instance.GetComponent<Rigidbody>();
	}

	private void Update()
	{
		if (_playerBody == null)
		{
			return;
		}

		if (!_rewinding && Time.time > _lastSampleAt + SampleIntervalSeconds)
		{
			_lastSampleAt = Time.time;
			_positions.Add(GTPlayer.Instance.headCollider.transform.position);
			if (_positions.Count > MaximumRecordedPositions)
			{
				_positions.RemoveAt(0);
			}
		}

		bool rewindButtonsHeld = ControllerInputPoller.instance.leftGrab
			&& ControllerInputPoller.instance.rightGrab
			&& ControllerInputPoller.instance.leftControllerSecondaryButton
			&& ControllerInputPoller.instance.rightControllerSecondaryButton;
		if (!rewindButtonsHeld)
		{
			_rewinding = false;
			_rewindOffset = 0;
			return;
		}

		_rewinding = true;
		if (_positions.Count == 0)
		{
			return;
		}

		_rewindOffset++;
		if (_rewindOffset >= _positions.Count)
		{
			_rewindOffset = _positions.Count - 1;
		}

		int positionIndex = _positions.Count - 1 - Mathf.Min(_rewindOffset, _positions.Count - 1);
		GTPlayer.Instance.transform.position = _positions[positionIndex] - new Vector3(0f, 0.5f, 0f);
		_playerBody.velocity = Vector3.zero;
		GorillaTagger.Instance.DoVibration(XRNode.LeftHand, 0.2f, Time.deltaTime);
		GorillaTagger.Instance.DoVibration(XRNode.RightHand, 0.2f, Time.deltaTime);
	}

	private void ResetState()
	{
		_positions.Clear();
		_rewindOffset = 0;
		_rewinding = false;
	}

	private void OnDisable()
	{
		ResetState();
	}

	private void OnDestroy()
	{
		ResetState();
	}
}
