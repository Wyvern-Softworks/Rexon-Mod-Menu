// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Utilities.NotificationManager
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System;
using System.Collections.Generic;
using System.Linq;
using GorillaLocomotion;
using Rexon_Menu.Core.Patches;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

using Object = UnityEngine.Object;

namespace Rexon_Menu.Core.Utilities;

public class NotificationManager : MonoBehaviour
{
	private static readonly List<Coroutine> ActiveClearCoroutines = new List<Coroutine>();

	private static NotificationManager _instance;
	private GameObject _canvasObject;
	private Transform _cameraTransform;
	private TextMeshProUGUI _notificationText;
	private bool _initialized;
	private bool _isVr;

	public static float NotificationDuration = 3f;
	public static bool ScaleWithPlayer = true;

	public static NotificationManager Instance => _instance;

	private void Awake()
	{
		if (_instance != null
			&& _instance != this)
		{
			Object.Destroy(gameObject);
			return;
		}
		_instance = this;
		Object.DontDestroyOnLoad(gameObject);
	}

	private void Update()
	{
		if (!_initialized && Camera.main != null)
		{
			_isVr = XRSettings.isDeviceActive;
			CreateNotificationCanvas();
			_initialized = true;
		}
		if (!_initialized || _canvasObject == null || !_isVr)
		{
			return;
		}

		_canvasObject.transform.position = _cameraTransform.TransformPoint(0f, 0f, 1.5f);
		_canvasObject.transform.rotation = _cameraTransform.rotation;
		float playerScale = 1f;
		if (ScaleWithPlayer && GTPlayer.Instance != null)
		{
			playerScale = GTPlayer.Instance.scale;
		}
		_canvasObject.transform.localScale = Vector3.one * playerScale;
	}

	private void CreateNotificationCanvas()
	{
		_cameraTransform = Camera.main.transform;
		_canvasObject = new GameObject("Rexon_NotifCanvas");
		Canvas canvas = _canvasObject.AddComponent<Canvas>();
		CanvasScaler scaler = _canvasObject.AddComponent<CanvasScaler>();

		GameObject textObject = new GameObject("NotifText");
		textObject.transform.SetParent(_canvasObject.transform, false);
		_notificationText = textObject.AddComponent<TextMeshProUGUI>();
		_notificationText.alignment = (TextAlignmentOptions)1025;
		_notificationText.overflowMode = (TextOverflowModes)0;
		_notificationText.richText = true;

		if (_isVr)
		{
			canvas.renderMode = (RenderMode)2;
			canvas.worldCamera = Camera.main;
			_canvasObject.GetComponent<RectTransform>().sizeDelta = new Vector2(5f, 5f);
			_notificationText.fontSize = 28f;
			_notificationText.material = ShaderPatch.CreateTransparentMaterial(Color.white);
			_notificationText.rectTransform.sizeDelta = new Vector2(500f, 250f);
			_notificationText.rectTransform.localScale = new Vector3(0.003f, 0.003f, 0.3f);
			_notificationText.rectTransform.localPosition = new Vector3(0f, -0.2f, 0.5f);
		}
		else
		{
			canvas.renderMode = (RenderMode)0;
			canvas.sortingOrder = 9999;
			scaler.uiScaleMode = (CanvasScaler.ScaleMode)1;
			scaler.referenceResolution = new Vector2(1920f, 1080f);
			scaler.matchWidthOrHeight = 0.5f;
			_notificationText.fontSize = 24f;
			_notificationText.rectTransform.anchorMin = Vector2.zero;
			_notificationText.rectTransform.anchorMax = Vector2.zero;
			_notificationText.rectTransform.pivot = Vector2.zero;
			_notificationText.rectTransform.sizeDelta = new Vector2(600f, 300f);
			_notificationText.rectTransform.anchoredPosition = new Vector2(20f, 20f);
		}
	}

	internal void RemoveOldestNotification()
	{
		if (_notificationText == null
			|| string.IsNullOrEmpty(_notificationText.text))
		{
			return;
		}

		string[] notifications = _notificationText.text.Split(
			new[] { Environment.NewLine },
			StringSplitOptions.RemoveEmptyEntries);
		_notificationText.text = notifications.Length <= 1
			? string.Empty
			: string.Join(Environment.NewLine, notifications.Skip(1).ToArray());
		if (ActiveClearCoroutines.Count > 0)
		{
			ActiveClearCoroutines.RemoveAt(0);
		}
	}

	private void OnDestroy()
	{
		if (_canvasObject != null)
		{
			Object.Destroy(_canvasObject);
		}
	}
}
