// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Settings.WasdFly
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using BepInEx;
using GorillaLocomotion;
using Rexon_Menu.Core.Attributes;
using Rexon_Menu.Interface;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

namespace Rexon_Menu.Core.Modules.Settings;

[Mod("WASD Fly", "Settings", "WASD keys to fly", false, 13, ModType.Toggle, true)]
internal sealed class WasdFly : MonoBehaviour
{
	private static float _baseFlySpeed = 10f;
	private static Vector3 _anchoredPosition = Vector3.zero;
	private static float _initialYaw = -1f;
	private static float _initialPitch = -1f;
	private static float _initialMouseX;
	private static float _initialMouseY;

	internal static float FlySpeed
	{
		get => _baseFlySpeed * GTPlayer.Instance.scale;
		set => _baseFlySpeed = value;
	}

	private void Update()
	{
		if (XRSettings.isDeviceActive)
		{
			return;
		}

		Rigidbody body = GorillaTagger.Instance.rigidbody;
		Transform bodyTransform = body.transform;
		if (BundleManager.IsSearchInputFocused)
		{
			body.linearVelocity = Vector3.zero;
			if (_anchoredPosition != Vector3.zero)
			{
				bodyTransform.position = _anchoredPosition;
			}
			return;
		}

		bool forwardPressed = UnityInput.Current.GetKey(KeyCode.W);
		bool leftPressed = UnityInput.Current.GetKey(KeyCode.A);
		bool backwardPressed = UnityInput.Current.GetKey(KeyCode.S);
		bool rightPressed = UnityInput.Current.GetKey(KeyCode.D);
		bool upPressed = UnityInput.Current.GetKey(KeyCode.Space);
		bool downPressed = UnityInput.Current.GetKey(KeyCode.LeftControl);
		bool movementPressed = forwardPressed || leftPressed || backwardPressed
			|| rightPressed || upPressed || downPressed;

		if (movementPressed)
		{
			body.linearVelocity = Vector3.zero;
		}

		UpdateMouseLook();

		float speed = FlySpeed;
		if (UnityInput.Current.GetKey(KeyCode.LeftShift))
		{
			speed *= 2f;
		}

		Transform cameraRig = GorillaTagger.Instance.rightHandTransform.parent;
		if (forwardPressed)
		{
			bodyTransform.position += cameraRig.forward * (Time.deltaTime * speed);
		}
		if (backwardPressed)
		{
			bodyTransform.position -= cameraRig.forward * (Time.deltaTime * speed);
		}
		if (leftPressed)
		{
			bodyTransform.position -= cameraRig.right * (Time.deltaTime * speed);
		}
		if (rightPressed)
		{
			bodyTransform.position += cameraRig.right * (Time.deltaTime * speed);
		}
		if (upPressed)
		{
			bodyTransform.position += Vector3.up * (Time.deltaTime * speed);
		}
		if (downPressed)
		{
			bodyTransform.position -= Vector3.up * (Time.deltaTime * speed);
		}

		VRRig.LocalRig.head.rigTarget.transform.rotation = GorillaTagger.Instance.headCollider.transform.rotation;

		if (movementPressed || _anchoredPosition == Vector3.zero)
		{
			_anchoredPosition = bodyTransform.position;
		}
		else
		{
			bodyTransform.position = _anchoredPosition;
		}
	}

	private static void UpdateMouseLook()
	{
		if (Mouse.current == null || !Mouse.current.rightButton.isPressed)
		{
			_initialYaw = -1f;
			_initialPitch = -1f;
			return;
		}

		Transform cameraRig = GorillaTagger.Instance.rightHandTransform.parent;
		Vector3 currentAngles = cameraRig.rotation.eulerAngles;
		Vector2 mousePosition = Mouse.current.position.ReadValue();
		float normalizedMouseX = mousePosition.x / Screen.width;
		float normalizedMouseY = mousePosition.y / Screen.height;

		if (_initialYaw < 0f)
		{
			_initialYaw = currentAngles.y;
			_initialMouseX = normalizedMouseX;
		}
		if (_initialPitch < 0f)
		{
			_initialPitch = currentAngles.x;
			_initialMouseY = normalizedMouseY;
		}

		float pitch = _initialPitch - (normalizedMouseY - _initialMouseY) * 360f * 1.33f;
		float yaw = _initialYaw + (normalizedMouseX - _initialMouseX) * 360f * 1.33f;
		if (pitch > 180f)
		{
			pitch -= 360f;
		}

		cameraRig.rotation = Quaternion.Euler(Mathf.Clamp(pitch, -90f, 90f), yaw, currentAngles.z);
	}
}
