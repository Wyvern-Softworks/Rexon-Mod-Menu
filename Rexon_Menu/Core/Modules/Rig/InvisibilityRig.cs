// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Rig.InvisibilityRig
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Recovered.Obfuscated;
using Rexon_Menu.Core.Attributes;
using Rexon_Menu.Interface;
using Rexon_Menu_Mat;
using UnityEngine;
using UnityEngine.XR;

using Object = UnityEngine.Object;

namespace Rexon_Menu.Core.Modules.Rig;

[Mod("Invis", "Rig", "Right secondary to go invisible.", false, 2, ModType.Toggle, false)]
internal sealed class InvisibilityRig : MonoBehaviour
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

	private bool _invisible;
	private bool _buttonWasPressed;
	private Vector3 _visibleRigPosition;
	private GameObject _rightHandMarker;
	private GameObject _leftHandMarker;
	private float _activatedAt;
	private bool _desktopMode;
	private float _rainbowHue;

	internal static bool IsActive { get; private set; }

	private void OnEnable()
	{
		_desktopMode = !XRSettings.isDeviceActive;
		if (_desktopMode)
		{
			BeginInvisibility();
		}
	}

	private void Update()
	{
		if (!_desktopMode)
		{
			bool buttonPressed = ControllerInputPoller.instance.rightControllerSecondaryButton;
			if (buttonPressed && !_buttonWasPressed)
			{
				_buttonWasPressed = true;
				_invisible = !_invisible;
				if (_invisible)
				{
					_visibleRigPosition = GorillaTagger.Instance.offlineVRRig.transform.position;
					_activatedAt = Time.time;
				}
			}
			else if (!buttonPressed)
			{
				_buttonWasPressed = false;
			}
		}

		IsActive = _invisible;
		if (_invisible)
		{
			MaintainInvisibility();
		}
		else
		{
			RestoreVisibleRig();
		}
	}

	private void BeginInvisibility()
	{
		_invisible = true;
		_visibleRigPosition = GorillaTagger.Instance.offlineVRRig.transform.position;
		_activatedAt = Time.time;
	}

	private void MaintainInvisibility()
	{
		bool initialSerializationWindow = Time.time < _activatedAt + 1f;
		if (initialSerializationWindow)
		{
			MatBridge.SetSerializationRateFor(1000);
		}

		GorillaTagger.Instance.offlineVRRig.enabled = false;
		GorillaTagger.Instance.offlineVRRig.transform.position = _visibleRigPosition - Vector3.up * 10f;
		if (GhostRig.IsActive && GhostRig.Instance != null)
		{
			GhostRig.Instance.ClearHandMarkers();
		}
		UpdateHandMarkers();

		if (initialSerializationWindow)
		{
			GameNetworkUtilities.FlushAndReplayNetworkTraffic();
		}
	}

	private void RestoreVisibleRig()
	{
		ClearHandMarkers();
		if (_visibleRigPosition != Vector3.zero)
		{
			GorillaTagger.Instance.offlineVRRig.transform.position = _visibleRigPosition;
			_visibleRigPosition = Vector3.zero;
		}

		if (!GorillaTagger.Instance.offlineVRRig.enabled && !GhostRig.IsActive)
		{
			GorillaTagger.Instance.offlineVRRig.enabled = true;
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

	private void ClearHandMarkers()
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
		_invisible = false;
		IsActive = false;
		if (GorillaTagger.Instance != null && GorillaTagger.Instance.offlineVRRig != null)
		{
			if (_visibleRigPosition != Vector3.zero)
			{
				GorillaTagger.Instance.offlineVRRig.transform.position = _visibleRigPosition;
			}
			if (!GhostRig.IsActive)
			{
				GorillaTagger.Instance.offlineVRRig.enabled = true;
			}
		}

		_visibleRigPosition = Vector3.zero;
		ClearHandMarkers();
	}
}
