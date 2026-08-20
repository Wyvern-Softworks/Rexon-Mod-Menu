// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Utilities.InputManager
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Collections.Generic;
using BepInEx;
using Rexon_Menu.Interface;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using CommonUsages = UnityEngine.XR.CommonUsages;
using InputDevice = UnityEngine.XR.InputDevice;

namespace Rexon_Menu.Core.Utilities;

public sealed class InputManager
{
	private static InputManager s_instance;

	private InputDevice? _leftController;

	private InputDevice? _rightController;

	private readonly Dictionary<LogicalInput, KeyCode> _desktopBindings = new Dictionary<LogicalInput, KeyCode>
	{
		{ LogicalInput.LeftGrip, KeyCode.Mouse0 },
		{ LogicalInput.RightGrip, KeyCode.Mouse1 },
		{ LogicalInput.LeftTrigger, KeyCode.Q },
		{ LogicalInput.RightTrigger, KeyCode.E },
		{ LogicalInput.LeftPrimaryButton, KeyCode.F },
		{ LogicalInput.RightSecondaryButton, KeyCode.G }
	};

	public static InputManager Instance
	{
		get
		{
			if (s_instance == null)
			{
				s_instance = new InputManager();
			}

			return s_instance;
		}
	}

	public bool IsVrActive => IsValid(_leftController) || IsValid(_rightController);

	private InputManager()
	{
		RefreshControllers();
		InputDevices.deviceConnected += OnDeviceConnected;
		InputDevices.deviceDisconnected += OnDeviceDisconnected;
	}

	public bool IsPressed(LogicalInput input)
	{
		return IsVrActive ? IsVrButtonPressed(input) : IsDesktopButtonPressed(input);
	}

	private bool IsDesktopButtonPressed(LogicalInput input)
	{
		if (BundleManager.IsSearchInputFocused && input != LogicalInput.LeftGrip && input != LogicalInput.RightGrip)
		{
			return false;
		}

		if (input == LogicalInput.LeftGrip)
		{
			return Mouse.current != null && Mouse.current.leftButton.isPressed;
		}

		if (input == LogicalInput.RightGrip)
		{
			return Mouse.current != null && Mouse.current.rightButton.isPressed;
		}

		return _desktopBindings.TryGetValue(input, out KeyCode key) && UnityInput.Current.GetKey(key);
	}

	private bool IsVrButtonPressed(LogicalInput input)
	{
		return input switch
		{
			LogicalInput.LeftGrip => ReadButton(_leftController, CommonUsages.gripButton),
			LogicalInput.RightGrip => ReadButton(_rightController, CommonUsages.gripButton),
			LogicalInput.LeftPrimaryButton => ReadButton(_leftController, CommonUsages.primaryButton),
			LogicalInput.RightSecondaryButton => ReadButton(_rightController, CommonUsages.secondaryButton),
			_ => false
		};
	}

	private static bool ReadButton(InputDevice? controller, InputFeatureUsage<bool> usage)
	{
		if (!controller.HasValue || !controller.Value.isValid)
		{
			return false;
		}

		return controller.Value.TryGetFeatureValue(usage, out bool pressed) && pressed;
	}

	private void OnDeviceConnected(InputDevice device)
	{
		if ((device.characteristics & InputDeviceCharacteristics.Left) != 0)
		{
			_leftController = device;
		}

		if ((device.characteristics & InputDeviceCharacteristics.Right) != 0)
		{
			_rightController = device;
		}
	}

	private void OnDeviceDisconnected(InputDevice device)
	{
		if ((device.characteristics & InputDeviceCharacteristics.Left) != 0)
		{
			_leftController = null;
		}

		if ((device.characteristics & InputDeviceCharacteristics.Right) != 0)
		{
			_rightController = null;
		}
	}

	private void RefreshControllers()
	{
		_leftController = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
		_rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
	}

	private static bool IsValid(InputDevice? device)
	{
		return device.HasValue && device.Value.isValid;
	}
}
