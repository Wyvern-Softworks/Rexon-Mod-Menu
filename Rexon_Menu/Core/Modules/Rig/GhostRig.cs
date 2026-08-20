// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Rig.GhostRig
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Recovered.Obfuscated;
using Rexon_Menu.Core.Attributes;
using Rexon_Menu.Interface;
using UnityEngine;
using UnityEngine.XR;

using Object = UnityEngine.Object;

namespace Rexon_Menu.Core.Modules.Rig;

[Mod("Ghost", "Rig", "Left secondary to freeze rig.", false, 1, ModType.Toggle, false)]
internal sealed class GhostRig : MonoBehaviour
{
	private static readonly Color[] MarkerColors =
	{
		new(0.196f, 0.051f, 0.357f, 1f),
		new(0.1f, 0.1f, 0.1f, 1f),
		new(0.5f, 0.08f, 0.08f, 1f),
		new(0.08f, 0.15f, 0.5f, 1f),
		new(0.08f, 0.35f, 0.08f, 1f),
		new(0.35f, 0.08f, 0.5f, 1f),
		new(0.5f, 0.25f, 0f, 1f),
		new(0f, 0.35f, 0.45f, 1f),
		new(0.5f, 0.15f, 0.35f, 1f),
		new(0.5f, 0.4f, 0f, 1f),
		new(0.7f, 0.7f, 0.7f, 1f)
	};

	private bool _ghostEnabled;
	private bool _buttonWasPressed;
	private GameObject _rightHandMarker;
	private GameObject _leftHandMarker;
	private bool _desktopMode;
	private float _rainbowHue;

	internal static bool IsActive { get; private set; }
	internal static GhostRig Instance { get; private set; }

	private void OnEnable()
	{
		Instance = this;
		_desktopMode = !XRSettings.isDeviceActive;
		if (_desktopMode)
		{
			_ghostEnabled = true;
		}
	}

	private void Update()
	{
		if (!_desktopMode)
		{
			bool buttonPressed = ControllerInputPoller.instance.leftControllerSecondaryButton;
			if (buttonPressed && !_buttonWasPressed)
			{
				_buttonWasPressed = true;
				_ghostEnabled = !_ghostEnabled;
			}
			else if (!buttonPressed)
			{
				_buttonWasPressed = false;
			}
		}

		IsActive = _ghostEnabled;
		if (_ghostEnabled)
		{
			GorillaTagger.Instance.offlineVRRig.enabled = false;
			if (InvisibilityRig.IsActive)
			{
				ClearHandMarkers();
			}
			else
			{
				UpdateHandMarkers();
			}
		}
		else
		{
			ClearHandMarkers();
			if (!GorillaTagger.Instance.offlineVRRig.enabled && !InvisibilityRig.IsActive)
			{
				GorillaTagger.Instance.offlineVRRig.enabled = true;
			}
		}
	}

	private Color GetMarkerColor()
	{
		if (ThemeSetting.GetCurrentThemeName() == "Rainbow")
		{
			_rainbowHue += Time.deltaTime * 0.2f;
			if (_rainbowHue > 1f)
			{
				_rainbowHue = 0f;
			}
			return Color.HSVToRGB(_rainbowHue, 0.8f, 1f);
		}

		int colorIndex = Mathf.Clamp(ThemeSetting.CurrentIndex, 0, MarkerColors.Length - 1);
		return MarkerColors[colorIndex];
	}

	private void UpdateHandMarkers()
	{
		Color color = GetMarkerColor();
		UpdateMarker(ref _rightHandMarker, GorillaTagger.Instance.rightHandTransform.position, color);
		UpdateMarker(ref _leftHandMarker, GorillaTagger.Instance.leftHandTransform.position, color);

		if (XRSettings.isDeviceActive && Main.IsVrMenuOpen())
		{
			_rightHandMarker.GetComponent<Renderer>().enabled = MenuHandSetting.IsLeftHand;
			_leftHandMarker.GetComponent<Renderer>().enabled = !MenuHandSetting.IsLeftHand;
		}
		else
		{
			_rightHandMarker.GetComponent<Renderer>().enabled = true;
			_leftHandMarker.GetComponent<Renderer>().enabled = true;
		}
	}

	private static void UpdateMarker(ref GameObject marker, Vector3 position, Color color)
	{
		if (marker == null)
		{
			marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			marker.transform.localScale = Vector3.one * 0.1f;
			Object.Destroy(marker.GetComponent<Collider>());
		}

		marker.transform.position = position;
		marker.GetComponent<Renderer>().material.color = color;
	}

	internal void ClearHandMarkers()
	{
		if (_rightHandMarker != null)
		{
			Object.DestroyImmediate(_rightHandMarker);
			_rightHandMarker = null;
		}
		if (_leftHandMarker != null)
		{
			Object.DestroyImmediate(_leftHandMarker);
			_leftHandMarker = null;
		}
	}

	private void OnDisable()
	{
		_ghostEnabled = false;
		IsActive = false;
		if (GorillaTagger.Instance != null
			&& GorillaTagger.Instance.offlineVRRig != null
			&& !InvisibilityRig.IsActive)
		{
			GorillaTagger.Instance.offlineVRRig.enabled = true;
		}
		ClearHandMarkers();
		if (Instance == this)
		{
			Instance = null;
		}
	}
}
