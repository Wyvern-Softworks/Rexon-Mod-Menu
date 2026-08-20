// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.ButtonClickerSetting
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System;
using System.Collections.Generic;
using System.Reflection;
using GorillaNetworking;
using HarmonyLib;
using Rexon_Menu.Core.Attributes;
using UnityEngine;
using UnityEngine.InputSystem;

using Object = UnityEngine.Object;
using Pointer = UnityEngine.InputSystem.Pointer;

namespace Recovered.Obfuscated;

[Mod("Button Clicker", "Settings", "Click buttons with mouse", false, 12, ModType.Toggle, true)]
internal class ButtonClickerSetting : MonoBehaviour
{
	private const string ShoulderCameraName = "Shoulder Camera";
	private const string TriggerColliderPath = "Player Objects/Player VR Controller/GorillaPlayer/TurnParent/RightHandTriggerCollider";
	private const string TriggerMethodName = "OnTriggerEnter";
	private const string KeyboardBindingFieldName = "Binding";
	private const float KeyboardClickDelay = 0.1f;
	private const float MaximumRayDistance = 512f;

	private static readonly Dictionary<string, GameObject> GameObjectCache = new();

	private static Camera _shoulderCamera;
	private static float _nextKeyboardClickTime;

	private void Update()
	{
		if (_shoulderCamera == null)
		{
			_shoulderCamera = FindCachedGameObject(ShoulderCameraName).GetComponent<Camera>();
		}

		if (_shoulderCamera == null || !Mouse.current.leftButton.isPressed)
		{
			return;
		}

		Vector2 pointerPosition = ((InputControl<Vector2>)(object)((Pointer)Mouse.current).position).ReadValue();
		Physics.Raycast(
			_shoulderCamera.ScreenPointToRay(pointerPosition),
			out RaycastHit hit,
			MaximumRayDistance,
			GameNetworkUtilities.GetGameplayLayerMask());

		if (Time.time <= _nextKeyboardClickTime)
		{
			return;
		}

		Collider handCollider = FindCachedGameObject(TriggerColliderPath).GetComponent<Collider>();
		foreach (Component component in hit.collider.GetComponents<Component>())
		{
			Type componentType = component.GetType();
			string componentName = componentType.Name;

			if (componentName == "GorillaPressableButton" ||
				typeof(GorillaPressableButton).IsAssignableFrom(componentType) ||
				componentName == "GorillaPlayerLineButton")
			{
				InvokeTrigger(componentType, component, handCollider);
			}

			if (componentName == "CustomKeyboardKey")
			{
				_nextKeyboardClickTime = Time.time + KeyboardClickDelay;
				InvokeTrigger(componentType, component, handCollider);
			}

			if (componentName == "GorillaKeyboardButton")
			{
				_nextKeyboardClickTime = Time.time + KeyboardClickDelay;
				GorillaKeyboardBindings binding = Traverse.Create(component).Field(KeyboardBindingFieldName).GetValue<GorillaKeyboardBindings>();
				GameEvents.OnGorrillaKeyboardButtonPressedEvent.Invoke(binding);
			}
		}
	}

	private static void InvokeTrigger(Type componentType, Component component, Collider handCollider)
	{
		componentType
			.GetMethod(TriggerMethodName, BindingFlags.Instance | BindingFlags.NonPublic)
			.Invoke(component, new object[] { handCollider });
	}

	public static GameObject FindCachedGameObject(string path)
	{
		if (GameObjectCache.TryGetValue(path, out GameObject cachedObject))
		{
			return cachedObject;
		}

		GameObject foundObject = GameObject.Find(path);
		if (foundObject != null)
		{
			GameObjectCache.Add(path, foundObject);
		}

		return foundObject;
	}
}
