// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Interface.Main
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using GorillaLocomotion;
using Photon.Pun;
using Photon.Realtime;
using Recovered.Obfuscated;
using Rexon_Menu.Core.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR;

using Object = UnityEngine.Object;
using StatusEffects = RoomSystem.StatusEffects;

namespace Rexon_Menu.Interface;

internal sealed class Main : MonoBehaviour
{
	private bool _vrMenuOpen;
	private bool _desktopMenuVisible;
	private bool _desktopToggleWasPressed;
	private bool _isDraggingDesktopMenu;
	private Vector2 _previousMousePosition;
	private bool _initialized;

	private static float _lastPeriodicMaintenanceAt;
	private static Main _instance;

	private static readonly Dictionary<int, float> LastTargetUpdateAt = new();
	private static readonly Dictionary<int, float> LastTargetStatusAt = new();

	internal static GameObject MenuPointer { get; private set; }

	public static HashSet<Player> SelectedPlayers { get; } = new();

	private void Start()
	{
		_instance = this;
		StartCoroutine(Initialize());
	}

	private IEnumerator Initialize()
	{
		while (GorillaTagger.Instance == null)
		{
			yield return null;
		}

		while (GTPlayer.Instance == null)
		{
			yield return null;
		}

		while (GorillaTagger.Instance.offlineVRRig == null)
		{
			yield return null;
		}

		BundleManager.InitializeInterface();
		yield return null;

		bool desktopMode = !XRSettings.isDeviceActive;
		_vrMenuOpen = false;
		_desktopMenuVisible = desktopMode;
		BundleManager.SetVrMenuVisible(visible: false);
		BundleManager.SetPcMenuVisible(desktopMode);
		_initialized = true;

	}

	private void Update()
	{
		if (!_initialized)
		{
			return;
		}

		BundleManager.ResetButtonFlash();
		BundleManager.CloseSearchOnEscape();
		BundleManager.UpdateFpsDisplay();
		BundleManager.UpdateRainbowTheme();
		BundleManager.UpdateRainbowPlatformColor();

		UpdateRainbowTheme();

		BundleManager.PollMenuState();
		BundleManager.PollDelayedAuthorization();

		if (Time.time > _lastPeriodicMaintenanceAt + 45f)
		{
			_lastPeriodicMaintenanceAt = Time.time;
			BundleManager.PollThemeAndAuthorization();
		}

		UpdateVrMenu();
		UpdateDesktopMenu();
		MaintainSelectedPlayers();
	}

	private static void UpdateRainbowTheme()
	{
		if (!GunController.IsRainbow)
		{
			return;
		}

		GunController.RainbowHue += Time.deltaTime * 0.5f;
		if (GunController.RainbowHue > 1f)
		{
			GunController.RainbowHue = 0f;
		}

		float hue = GunController.RainbowHue;
		GunController.ColorIdle = Color.HSVToRGB(hue, 0.6f, 0.5f);
		GunController.ColorShooting = Color.HSVToRGB(hue, 0.8f, 0.9f);
		GunController.ColorLocked = Color.HSVToRGB(hue, 0.9f, 1f);
	}

	private void UpdateDesktopMenu()
	{
		if (MenuStyleSetting.CurrentStyle != 0)
		{
			UpdateLegacyDesktopToggle();
			return;
		}

		if (BundleManager.PcMenuPanel == null)
		{
			return;
		}

		bool togglePressed = InputManager.Instance.IsPressed(LogicalInput.RightSecondaryButton);
		if (togglePressed && !_desktopToggleWasPressed)
		{
			_desktopMenuVisible = !_desktopMenuVisible;
			BundleManager.SetPcMenuVisible(_desktopMenuVisible);
		}

		_desktopToggleWasPressed = togglePressed;
		if (_desktopMenuVisible && BundleManager.PcMenuPanel.activeSelf)
		{
			UpdateDesktopDrag();
		}
		else
		{
			_isDraggingDesktopMenu = false;
		}
	}

	private void UpdateLegacyDesktopToggle()
	{
		bool togglePressed = InputManager.Instance.IsPressed(LogicalInput.RightSecondaryButton);
		if (togglePressed && !_desktopToggleWasPressed)
		{
			_desktopMenuVisible = !_desktopMenuVisible;
			LegacyMenu.DesktopMenuVisible = _desktopMenuVisible;
		}

		_desktopToggleWasPressed = togglePressed;
	}

	private void UpdateVrMenu()
	{
		if (MenuStyleSetting.CurrentStyle != 0)
		{
			UpdateLegacyVrMenu();
			return;
		}

		if (BundleManager.VrMenuPanel == null)
		{
			return;
		}

		bool menuButtonPressed;
		try
		{
			menuButtonPressed = MenuHandSetting.IsLeftHand
				? ControllerInputPoller.instance.rightControllerPrimaryButton
				: ControllerInputPoller.instance.leftControllerPrimaryButton;
		}
		catch (Exception)
		{
			return;
		}

		if (VRKeyboard.Spawned)
		{
			OpenCurrentVrMenu();
			PositionCurrentMenuInFrontOfHead();
			VRKeyboard.RefreshDisplay();
			BundleManager.AlignMenuPointer();
			return;
		}

		if (menuButtonPressed)
		{
			OpenCurrentVrMenu();
			PositionCurrentMenuAtHand();
			BundleManager.AlignMenuPointer();
		}
		else if (_vrMenuOpen)
		{
			_vrMenuOpen = false;
			BundleManager.SetVrMenuVisible(visible: false);
			DestroyPointer();
		}
	}

	private void UpdateLegacyVrMenu()
	{
		bool menuButtonPressed;
		try
		{
			menuButtonPressed = MenuHandSetting.IsLeftHand
				? ControllerInputPoller.instance.leftControllerPrimaryButton
				: ControllerInputPoller.instance.rightControllerPrimaryButton;
		}
		catch (Exception)
		{
			return;
		}

		if (menuButtonPressed)
		{
			if (!_vrMenuOpen)
			{
				_vrMenuOpen = true;
				EnsurePointer();
				LegacyMenu.EnsureMenuCreated();
			}

			LegacyMenu.UpdateVrMenuTransform();
		}
		else if (_vrMenuOpen)
		{
			_vrMenuOpen = false;
			DestroyPointer();
			LegacyMenu.DestroyMenu();
		}
	}

	private void OpenCurrentVrMenu()
	{
		if (!_vrMenuOpen)
		{
			_vrMenuOpen = true;
			BundleManager.SetVrMenuVisible(visible: true);
		}

		EnsurePointer();
	}

	private static void PositionCurrentMenuInFrontOfHead()
	{
		if (BundleManager.VrMenuPanel == null)
		{
			return;
		}

		Transform head = GorillaTagger.Instance.headCollider.transform;
		Vector3 horizontalForward = head.forward;
		horizontalForward.y = 0f;
		horizontalForward.Normalize();

		Transform menuTransform = BundleManager.VrMenuPanel.transform;
		menuTransform.position = head.position + horizontalForward * 0.45f + Vector3.up * -0.05f;
		menuTransform.rotation = Quaternion.LookRotation(horizontalForward, Vector3.up);
	}

	private static void PositionCurrentMenuAtHand()
	{
		if (BundleManager.VrMenuPanel == null)
		{
			return;
		}

		Transform menuTransform = BundleManager.VrMenuPanel.transform;
		if (MenuHandSetting.IsLeftHand)
		{
			Transform leftHand = GorillaTagger.Instance.leftHandTransform;
			menuTransform.position = leftHand.position + leftHand.right * 0.08f + leftHand.forward * 0.1f;
			menuTransform.rotation = leftHand.rotation * Quaternion.Euler(0f, -90f, -90f);
		}
		else
		{
			Transform rightHand = GorillaTagger.Instance.rightHandTransform;
			menuTransform.position = rightHand.position + rightHand.right * -0.08f + rightHand.forward * 0.1f;
			menuTransform.rotation = rightHand.rotation * Quaternion.Euler(0f, 90f, 90f);
		}
	}

	private static void EnsurePointer()
	{
		if (MenuPointer != null)
		{
			return;
		}

		MenuPointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		Object.Destroy(MenuPointer.GetComponent<Rigidbody>());
		MenuPointer.GetComponent<SphereCollider>().isTrigger = true;
		MenuPointer.GetComponent<Renderer>().material.color = Color.white;
		MenuPointer.transform.localScale = Vector3.one * 0.01f;

		MenuPointer.transform.parent = MenuHandSetting.IsLeftHand
			? GorillaTagger.Instance.rightHandTransform
			: GorillaTagger.Instance.leftHandTransform;
		MenuPointer.transform.localPosition = new Vector3(0f, -0.1f, 0f);
	}

	internal static void SetDesktopMenuVisible(bool visible)
	{
		if (_instance != null)
		{
			_instance._desktopMenuVisible = visible;
		}
	}

	internal static void CloseMenus()
	{
		if (_instance == null)
		{
			return;
		}

		_instance._vrMenuOpen = false;
		_instance._desktopMenuVisible = false;
		DestroyPointer();
	}

	internal static bool IsVrMenuOpen()
	{
		return _instance != null && _instance._vrMenuOpen;
	}

	internal static void CloseVrMenu()
	{
		DestroyPointer();
		if (_instance != null)
		{
			_instance._vrMenuOpen = false;
			BundleManager.SetVrMenuVisible(visible: false);
		}
	}

	private static void DestroyPointer()
	{
		if (MenuPointer != null)
		{
			Object.Destroy(MenuPointer);
			MenuPointer = null;
		}
	}

	private static bool IsPointerOverButton(Vector2 screenPosition)
	{
		if (EventSystem.current == null)
		{
			return false;
		}

		PointerEventData pointerData = new(EventSystem.current)
		{
			position = screenPosition
		};
		List<RaycastResult> results = new();
		EventSystem.current.RaycastAll(pointerData, results);
		return results.Any(result => result.gameObject.GetComponent<Button>() != null);
	}

	private static bool ContainsScreenPoint(RectTransform rectangle, Vector2 screenPosition)
	{
		if (rectangle == null)
		{
			return false;
		}

		Canvas canvas = BundleManager.PcMenuPanel.GetComponent<Canvas>();
		Camera camera = canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay
			? canvas.worldCamera
			: null;
		return RectTransformUtility.RectangleContainsScreenPoint(rectangle, screenPosition, camera);
	}

	private void UpdateDesktopDrag()
	{
		RectTransform menuRectangle = BundleManager.BackgroundRectPC;
		if (menuRectangle == null)
		{
			return;
		}

		Vector2 mousePosition = new(UnityInput.Current.mousePosition.x, UnityInput.Current.mousePosition.y);
		if (UnityInput.Current.GetMouseButtonDown(0)
			&& ContainsScreenPoint(menuRectangle, mousePosition)
			&& !IsPointerOverButton(mousePosition))
		{
			_isDraggingDesktopMenu = true;
			_previousMousePosition = mousePosition;
		}

		if (UnityInput.Current.GetMouseButtonUp(0))
		{
			_isDraggingDesktopMenu = false;
		}

		if (!_isDraggingDesktopMenu || !UnityInput.Current.GetMouseButton(0))
		{
			return;
		}

		Vector2 movement = mousePosition - _previousMousePosition;
		menuRectangle.anchoredPosition += movement;
		if (BundleManager.BorderRectPC != null)
		{
			BundleManager.BorderRectPC.anchoredPosition += movement;
		}

		_previousMousePosition = mousePosition;
	}

	private static void MaintainSelectedPlayers()
	{
		if (!PhotonNetwork.InRoom)
		{
			SelectedPlayers.Clear();
			LastTargetUpdateAt.Clear();
			LastTargetStatusAt.Clear();
			return;
		}

		string gameMode = PhotonNetwork.CurrentRoom.CustomProperties["gameMode"]?.ToString();
		if (SelectedPlayers.Count == 0 || gameMode == null || !gameMode.Contains("MODDED_"))
		{
			return;
		}

		foreach (Player departedPlayer in SelectedPlayers.Where(player => !PhotonNetwork.PlayerList.Contains(player)).ToList())
		{
			SelectedPlayers.Remove(departedPlayer);
			LastTargetUpdateAt.Remove(departedPlayer.ActorNumber);
			LastTargetStatusAt.Remove(departedPlayer.ActorNumber);
		}

		foreach (Player target in SelectedPlayers)
		{
			if (LastTargetUpdateAt.TryGetValue(target.ActorNumber, out float lastUpdate)
				&& Time.time < lastUpdate + 0.3f)
			{
				continue;
			}

			LastTargetUpdateAt[target.ActorNumber] = Time.time;
			if (!LastTargetStatusAt.TryGetValue(target.ActorNumber, out float lastStatus)
				|| Time.time >= lastStatus + 1f)
			{
				LastTargetStatusAt[target.ActorNumber] = Time.time;
				if (!PhotonNetwork.LocalPlayer.IsMasterClient)
				{
					PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
					PhotonNetwork.SendAllOutgoingCommands();
				}

				RaiseEventOptions targetOnly = new()
				{
					TargetActors = new[] { target.ActorNumber }
				};
				GameNetworkUtilities.SendStatusEffect((StatusEffects)0, targetOnly);
			}

			GameNetworkUtilities.FlushAndReplayNetworkTraffic();
		}
	}

	private void OnDestroy()
	{
		DestroyPointer();
		if (_instance == this)
		{
			_instance = null;
		}
	}
}
