// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Utilities.VRKeyboard
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Rexon_Menu.Core.Patches;
using Rexon_Menu.Interface;
using Rexon_Shader;
using TMPro;
using UnityEngine;

namespace Rexon_Menu.Core.Utilities;

public static class VRKeyboard
{
	private const float KeySize = 0.03f;
	private const float KeySpacing = 0.005f;

	private static readonly string[][] KeyRows =
	{
		new[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" },
		new[] { "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P" },
		new[] { "A", "S", "D", "F", "G", "H", "J", "K", "L" },
		new[] { "Z", "X", "C", "V", "B", "N", "M" }
	};

	private static GameObject KeyboardRoot;
	private static GameObject LeftPointer;
	private static GameObject RightPointer;
	private static TextMeshPro DisplayText;
	private static string InputText = string.Empty;
	private static bool ShiftEnabled;
	internal static bool Spawned;

	public static bool IsSpawned => Spawned;

	public static bool IsKeyboardPointer(Collider collider)
	{
		if (!Spawned || collider == null)
		{
			return false;
		}
		GameObject colliderObject = collider.gameObject;
		return (LeftPointer != null
				&& colliderObject == LeftPointer)
			|| (RightPointer != null
				&& colliderObject == RightPointer);
	}

	public static bool IsLeftPointer(Collider collider)
	{
		return Spawned
			&& collider != null
			&& LeftPointer != null
			&& collider.gameObject == LeftPointer;
	}

	public static void Spawn()
	{
		if (Spawned)
		{
			return;
		}

		Spawned = true;
		InputText = string.Empty;
		ShiftEnabled = false;
		float keyPitch = KeySize + KeySpacing;

		KeyboardRoot = new GameObject("RexonVRKeyboard");
		Transform playerBody = GorillaTagger.Instance.bodyCollider.transform;
		Vector3 forward = playerBody.forward;
		forward.y = 0f;
		forward.Normalize();
		KeyboardRoot.transform.position = playerBody.position + forward * 0.4f - Vector3.up * 0.15f;
		KeyboardRoot.transform.rotation =
			Quaternion.LookRotation(forward, Vector3.up) * Quaternion.Euler(55f, 0f, 0f);

		CreatePanel(
			"KeyboardBackground",
			new Vector3(0f, 0.01f, -0.002f),
			new Vector3(keyPitch * 11f, keyPitch * 7.5f, 0.002f),
			new Color(0.08f, 0f, 0.14f, 0.95f));

		float firstRowY = keyPitch * 3f;
		for (int rowIndex = 0; rowIndex < KeyRows.Length; rowIndex++)
		{
			string[] row = KeyRows[rowIndex];
			float rowY = firstRowY - rowIndex * keyPitch;
			float firstKeyX = -row.Length * keyPitch / 2f + KeySize / 2f;
			for (int keyIndex = 0; keyIndex < row.Length; keyIndex++)
			{
				CreateKey(row[keyIndex], new Vector3(firstKeyX + keyIndex * keyPitch, rowY, 0f), KeySize);
			}
		}

		float controlsY = firstRowY - 4f * keyPitch;
		CreateKey("Shift", new Vector3(-keyPitch * 3f, controlsY, 0f), KeySize, 2f);
		CreateKey("Space", new Vector3(0f, controlsY, 0f), KeySize, 3f);
		CreateKey("Del", new Vector3(keyPitch * 2.5f, controlsY, 0f), KeySize, 1.5f);
		CreateKey("Done", new Vector3(keyPitch * 4.2f, controlsY, 0f), KeySize, 1.5f);

		GameObject displayPanel = CreatePanel(
			"DisplayPanel",
			new Vector3(0f, firstRowY + keyPitch * 1.8f, 0f),
			new Vector3(keyPitch * 10f, keyPitch * 0.9f, 0.003f),
			new Color(0.05f, 0f, 0.1f, 1f));
		GameObject displayObject = new GameObject("InputDisplay");
		displayObject.transform.SetParent(displayPanel.transform, false);
		displayObject.transform.localPosition = new Vector3(0f, 0f, -0.6f);
		displayObject.transform.localRotation = Quaternion.identity;
		displayObject.transform.localScale = new Vector3(0.3f, 1f, 1f);
		DisplayText = displayObject.AddComponent<TextMeshPro>();
		DisplayText.fontSize = 3f;
		DisplayText.color = Color.white;
		DisplayText.alignment = (TextAlignmentOptions)514;
		DisplayText.text = "|";
		DisplayText.rectTransform.sizeDelta = new Vector2(10f, 1f);

		CreateHandPointers();
		UpdateDisplay();
	}

	private static GameObject CreatePanel(string name, Vector3 localPosition, Vector3 localScale, Color color)
	{
		GameObject panel = GameObject.CreatePrimitive((PrimitiveType)3);
		panel.name = name;
		panel.transform.SetParent(KeyboardRoot.transform, false);
		panel.transform.localPosition = localPosition;
		panel.transform.localScale = localScale;
		Object.Destroy(panel.GetComponent<BoxCollider>());
		Renderer renderer = panel.GetComponent<Renderer>();
		ShaderPatch.EnsureCached();
		renderer.material = new Material(ShaderBridge.Cached);
		renderer.material.color = color;
		return panel;
	}

	private static void CreateKey(string keyValue, Vector3 localPosition, float keySize, float width = 1f)
	{
		GameObject key = GameObject.CreatePrimitive((PrimitiveType)3);
		key.name = keyValue;
		key.transform.SetParent(KeyboardRoot.transform, false);
		key.transform.localPosition = localPosition;
		key.transform.localScale = new Vector3(keySize * width, keySize, keySize * 0.3f);
		BoxCollider collider = key.GetComponent<BoxCollider>();
		collider.isTrigger = true;
		collider.size = new Vector3(1f, 1f, 3f);
		Renderer renderer = key.GetComponent<Renderer>();
		renderer.material = new Material(ShaderBridge.Cached);
		renderer.material.color = new Color(0.15f, 0f, 0.26f, 1f);

		GameObject labelObject = new GameObject("Label");
		labelObject.transform.SetParent(key.transform, false);
		labelObject.transform.localPosition = new Vector3(0f, 0f, -0.6f);
		labelObject.transform.localRotation = Quaternion.identity;
		labelObject.transform.localScale = new Vector3(1f / width, 1f, 1f);
		TextMeshPro label = labelObject.AddComponent<TextMeshPro>();
		label.fontSize = 2.5f;
		label.color = Color.white;
		label.alignment = (TextAlignmentOptions)514;
		label.text = keyValue;
		label.rectTransform.sizeDelta = new Vector2(2f, 1f);
		key.AddComponent<VRKeyboardKey>().KeyValue = keyValue;
	}

	private static void CreateHandPointers()
	{
		LeftPointer = CreateHandPointer(GorillaTagger.Instance.leftHandTransform, out SphereCollider leftCollider);
		RightPointer = CreateHandPointer(GorillaTagger.Instance.rightHandTransform, out SphereCollider rightCollider);
		VRKeyboardKey.SetHandPointers(leftCollider, rightCollider);
	}

	private static GameObject CreateHandPointer(Transform hand, out SphereCollider collider)
	{
		GameObject pointer = GameObject.CreatePrimitive((PrimitiveType)0);
		pointer.transform.parent = hand;
		pointer.transform.localPosition = new Vector3(0f, -0.1f, 0f);
		pointer.transform.localScale = new Vector3(0.015f, 0.015f, 0.015f);
		Object.Destroy(pointer.GetComponent<Rigidbody>());
		pointer.GetComponent<Renderer>().material.color = new Color(0.75f, 0.53f, 1f, 0.8f);
		collider = pointer.GetComponent<SphereCollider>();
		collider.isTrigger = true;
		return pointer;
	}

	public static void Close()
	{
		if (!Spawned)
		{
			return;
		}
		Spawned = false;
		DestroyIfPresent(LeftPointer);
		DestroyIfPresent(RightPointer);
		DestroyIfPresent(KeyboardRoot);
		LeftPointer = null;
		RightPointer = null;
		KeyboardRoot = null;
		DisplayText = null;
	}

	private static void DestroyIfPresent(GameObject gameObject)
	{
		if (gameObject != null)
		{
			Object.Destroy(gameObject);
		}
	}

	public static void HandleKeyPress(string keyValue)
	{
		switch (keyValue)
		{
			case "Space":
				InputText += " ";
				break;
			case "Del":
				if (InputText.Length > 0)
				{
					InputText = InputText.Substring(0, InputText.Length - 1);
				}
				break;
			case "Shift":
				ShiftEnabled = !ShiftEnabled;
				break;
			case "Done":
				BundleManager.CloseSearch();
				return;
			default:
				if (keyValue.Length == 1)
				{
					InputText += ShiftEnabled ? keyValue.ToUpper() : keyValue.ToLower();
					ShiftEnabled = false;
				}
				break;
		}
		UpdateDisplay();
		BundleManager.UpdateSearchQuery(InputText);
	}

	private static void UpdateDisplay()
	{
		if (DisplayText != null)
		{
			string cursor = Time.frameCount / 30 % 2 == 0 ? "|" : " ";
			DisplayText.text = InputText + cursor;
		}
	}

	public static void RefreshDisplay()
	{
		if (Spawned && KeyboardRoot != null)
		{
			UpdateDisplay();
		}
	}
}
