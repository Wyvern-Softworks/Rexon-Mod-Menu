// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Interface.LegacyMenu
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System;
using System.Collections.Generic;
using Photon.Pun;
using Recovered.Obfuscated;
using Rexon_Menu.Core.Attributes;
using Rexon_Menu.Core.Patches;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

using WindowFunction = UnityEngine.GUI.WindowFunction;

using Object = UnityEngine.Object;

namespace Rexon_Menu.Interface;

internal class LegacyMenu : MonoBehaviour
{
	internal struct ButtonData
	{
		public GameObject GameObject;

		public BoxCollider Collider;

		public string Action;

		public int ModIndex;

		public bool IsCategory;

		public string CategoryName;
	}

	internal class RoundClampColor : MonoBehaviour
	{
		public Renderer SourceRenderer;

		private Renderer TargetRenderer;

		private void Start()
		{
			TargetRenderer = this.GetComponent<Renderer>();
			Update();
		}

		private void Update()
		{
			if (TargetRenderer != null && SourceRenderer != null)
			{
				if (TargetRenderer.material.shader != SourceRenderer.material.shader)
				{
					TargetRenderer.material = new Material(SourceRenderer.material.shader);
				}
				TargetRenderer.material.color = SourceRenderer.material.color;
			}
		}
	}

	private static int MenuStyle;

	private static GameObject MenuRoot;

	private static GameObject CanvasObject;

	private static Text StatusText;

	private static bool ShowingCategories;

	private static string SelectedCategory;

	private static int PageIndex;

	private static int ReturnPageIndex;

	internal static bool DesktopMenuVisible;

	private static Rect WindowRect;

	private static Vector2 ScrollPosition;

	private static int SelectedDesktopCategoryIndex;

	private static List<string> Categories;

	private static float SmoothedFps;

	private static float DisplayedFps;

	private static float NextFpsUpdate;

	private static List<ButtonData> Buttons;

	private static Renderer BackgroundRenderer;

	private static GameObject HoveredButton;

	private void Update()
	{
		if (MenuStyle == 0)
		{
			return;
		}
		if (MenuStyle == 1 && StatusText != null && MenuRoot != null)
		{
			SmoothedFps = SmoothedFps * 0.95f + 1f / Time.unscaledDeltaTime * 0.05f;
			if (Time.time > NextFpsUpdate)
			{
				DisplayedFps = Mathf.Ceil(SmoothedFps);
				NextFpsUpdate = Time.time + 0.5f;
			}
			int pageCount = GetPageCount();
			StatusText.text = $"FPS: {Mathf.RoundToInt(DisplayedFps)}  Page: {PageIndex + 1}/{pageCount}";
		}
		if (MenuRoot == null || Buttons.Count <= 0)
		{
			return;
		}
		GameObject menuPointer = Main.MenuPointer;
		if (menuPointer == null)
		{
			return;
		}
		Vector3 position = menuPointer.transform.position;
		bool isHoveringButton = false;
		for (int i = 0; i < Buttons.Count; i++)
		{
			if (Buttons[i].Collider != null && ContainsPoint(Buttons[i].Collider, position))
			{
				isHoveringButton = true;
				if (HoveredButton != Buttons[i].GameObject)
				{
					HoveredButton = Buttons[i].GameObject;
					HandleButtonHover(Buttons[i]);
				}
				break;
			}
		}
		if (!isHoveringButton)
		{
			HoveredButton = null;
		}
	}

	private static bool ContainsPoint(BoxCollider collider, Vector3 point)
	{
		Vector3 localPoint = collider.transform.InverseTransformPoint(point);
		Vector3 minimum = collider.center - collider.size * 0.5f;
		Vector3 maximum = collider.center + collider.size * 0.5f;
		return localPoint.x >= minimum.x && localPoint.x <= maximum.x
			&& localPoint.y >= minimum.y && localPoint.y <= maximum.y
			&& localPoint.z >= minimum.z && localPoint.z <= maximum.z;
	}

	private static void HandleButtonHover(ButtonData buttonData)
	{
		if (buttonData.Action == "Leave")
		{
			VibrateMenuHand();
			PlayClickSound();
			ExecuteButtonAction(buttonData);
		}
		else if (BundleManager.TryAcquireButtonClickCooldown())
		{
			VibrateMenuHand();
			PlayClickSound();
			ExecuteButtonAction(buttonData);
		}
	}

	private static void VibrateMenuHand()
	{
		GorillaTagger.Instance.StartVibration(!MenuHandSetting.IsLeftHand, GorillaTagger.Instance.tagHapticStrength / 2f, GorillaTagger.Instance.tagHapticDuration / 2f);
	}

	private static void PlayClickSound()
	{
		if (MenuHandSetting.IsLeftHand)
		{
			GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(106, false, 0.4f);
		}
		else
		{
			GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(106, true, 0.4f);
		}
	}

	public static void SetMenuStyle(int style)
	{
		if (MenuStyle != 0)
		{
			DestroyMenu();
			DesktopMenuVisible = false;
		}
		MenuStyle = style;
		Main.CloseMenus();
		if (style == 0)
		{
			bool desktopMode = !XRSettings.isDeviceActive;
			BundleManager.SetVrMenuVisible(visible: false);
			BundleManager.SetPcMenuVisible(desktopMode);
			if (desktopMode)
			{
				Main.SetDesktopMenuVisible(visible: true);
			}
			return;
		}
		BundleManager.SetVrMenuVisible(visible: false);
		BundleManager.SetPcMenuVisible(visible: false);
		Categories = new List<string>(BundleManager.Categories);
		int settingsCategoryIndex = Categories.IndexOf("Settings");
		if (settingsCategoryIndex >= 0)
		{
			SelectedDesktopCategoryIndex = settingsCategoryIndex;
		}
		ShowingCategories = false;
		SelectedCategory = "Settings";
		List<int> settingsMods = BundleManager.GetModIndexesForCategory("Settings");
		int menuStyleEntryIndex = -1;
		for (int i = 0; i < settingsMods.Count; i++)
		{
			if (BundleManager.GetModDisplayName(settingsMods[i]).Contains("Menu Style"))
			{
				menuStyleEntryIndex = i;
				break;
			}
		}
		int pageSize = style == 2 ? 4 : 5;
		PageIndex = menuStyleEntryIndex >= 0 ? menuStyleEntryIndex / pageSize : 0;
		ScrollPosition = Vector2.zero;
		if (!XRSettings.isDeviceActive)
		{
			DesktopMenuVisible = true;
			Main.SetDesktopMenuVisible(visible: true);
		}
	}

	public static void EnsureMenuCreated()
	{
		if (MenuRoot == null)
		{
			if (MenuStyle == 1)
			{
				BuildStyleOneMenu();
			}
			else if (MenuStyle == 2)
			{
				BuildStyleTwoMenu();
			}
		}
	}

	public static void UpdateVrMenuTransform()
	{
		if (MenuRoot != null)
		{
			if (MenuHandSetting.IsLeftHand)
			{
				MenuRoot.transform.position = GorillaTagger.Instance.leftHandTransform.position;
				MenuRoot.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation;
			}
			else
			{
				Transform rightHandTransform = GorillaTagger.Instance.rightHandTransform;
				MenuRoot.transform.position = rightHandTransform.position;
				MenuRoot.transform.rotation = rightHandTransform.rotation * Quaternion.Euler(0f, 180f, 0f) * Quaternion.Euler(180f, 0f, 0f);
			}
		}
	}

	private static Color ScaleThemeColor(float value, float amount)
	{
		Color themeColor = BundleManager.CurrentThemeColor;
		return new Color(
			Mathf.Clamp01(themeColor.r * value),
			Mathf.Clamp01(themeColor.g * value),
			Mathf.Clamp01(themeColor.b * value),
			amount);
	}

	private static void SetRendererColor(Renderer renderer, Color32 color)
	{
		renderer.material.color = color;
	}

	internal static void DestroyMenu()
	{
		if (MenuRoot != null)
		{
			Object.Destroy(MenuRoot);
			MenuRoot = null;
			CanvasObject = null;
			StatusText = null;
		}
		Buttons.Clear();
		BackgroundRenderer = null;
		HoveredButton = null;
	}

	private static void RebuildMenu()
	{
		Vector3 savedPosition = Vector3.zero;
		Quaternion savedRotation = Quaternion.identity;
		if (MenuRoot != null)
		{
			savedPosition = MenuRoot.transform.position;
			savedRotation = MenuRoot.transform.rotation;
		}
		DestroyMenu();
		if (MenuStyle == 1)
		{
			BuildStyleOneMenu();
		}
		else if (MenuStyle == 2)
		{
			BuildStyleTwoMenu();
		}
		if (MenuRoot != null && savedPosition != Vector3.zero)
		{
			MenuRoot.transform.position = savedPosition;
			MenuRoot.transform.rotation = savedRotation;
		}
	}

	private static void ExecuteButtonAction(ButtonData buttonData)
	{
		if (buttonData.Action == "Leave")
		{
			PhotonNetwork.Disconnect();
		}
		else if (buttonData.Action == "NextPage")
		{
			int lastPage = GetPageCount() - 1;
			PageIndex = PageIndex < lastPage ? PageIndex + 1 : 0;
			RebuildMenu();
		}
		else if (buttonData.Action == "PreviousPage")
		{
			int lastPage = GetPageCount() - 1;
			PageIndex = PageIndex > 0 ? PageIndex - 1 : lastPage;
			RebuildMenu();
		}
		else if (buttonData.Action == "Home")
		{
			ShowingCategories = true;
			PageIndex = ReturnPageIndex;
			SelectedCategory = null;
			RebuildMenu();
		}
		else if (buttonData.IsCategory)
		{
			ReturnPageIndex = PageIndex;
			SelectedCategory = buttonData.CategoryName;
			ShowingCategories = false;
			PageIndex = 0;
			RebuildMenu();
		}
		else if (buttonData.ModIndex >= 0)
		{
			BundleManager.ToggleMod(buttonData.ModIndex, null, useLegacyToggle: false);
			RebuildMenu();
		}
	}

	private static int GetPageCount()
	{
		if (ShowingCategories)
		{
			return Mathf.Max(1, (BundleManager.Categories.Count + 4) / 5);
		}
		int modCount = BundleManager.GetModIndexesForCategory(SelectedCategory).Count;
		int pageSize = MenuStyle == 2 ? 4 : 5;
		return Mathf.Max(1, (modCount + pageSize - 1) / pageSize);
	}

	private static GameObject CreateMenuCube(
		Vector3 scale,
		Vector3 position,
		Color32 color,
		bool interactive,
		bool useWorldPosition = false)
	{
		GameObject cube = GameObject.CreatePrimitive((PrimitiveType)3);
		Object.Destroy(cube.GetComponent<Rigidbody>());
		BoxCollider collider = cube.GetComponent<BoxCollider>();
		if (interactive)
		{
			((Collider)collider).isTrigger = true;
		}
		else
		{
			Object.Destroy(collider);
		}
		cube.transform.parent = MenuRoot.transform;
		cube.transform.rotation = Quaternion.identity;
		cube.transform.localScale = scale;
		if (useWorldPosition)
		{
			cube.transform.position = position;
		}
		else
		{
			cube.transform.localPosition = position;
		}
		SetRendererColor(cube.GetComponent<Renderer>(), color);
		return cube;
	}

	private static GameObject CreateInteractiveCube(string action, Vector3 scale, Vector3 localPosition, Color32 color)
	{
		GameObject button = CreateMenuCube(scale, localPosition, color, interactive: true);
		Buttons.Add(new ButtonData
		{
			GameObject = button,
			Collider = button.GetComponent<BoxCollider>(),
			Action = action,
			ModIndex = -1
		});
		return button;
	}

	private static void CreateMenuCanvas(float pixelsPerUnit)
	{
		CanvasObject = new GameObject("LegacyCanvas");
		CanvasObject.transform.parent = MenuRoot.transform;
		Canvas canvas = CanvasObject.AddComponent<Canvas>();
		CanvasScaler scaler = CanvasObject.AddComponent<CanvasScaler>();
		CanvasObject.AddComponent<GraphicRaycaster>();
		canvas.renderMode = (RenderMode)2;
		scaler.dynamicPixelsPerUnit = pixelsPerUnit;
		scaler.referencePixelsPerUnit = pixelsPerUnit;
	}

	private static void BuildStyleOneMenu()
	{
		ShaderPatch.Skip = true;
		MenuRoot = GameObject.CreatePrimitive((PrimitiveType)3);
		Object.Destroy(MenuRoot.GetComponent<Rigidbody>());
		Object.Destroy(MenuRoot.GetComponent<BoxCollider>());
		Object.Destroy(MenuRoot.GetComponent<Renderer>());
		MenuRoot.transform.localScale = new Vector3(0.11f, 0.29f, 0.4f);

		GameObject background = CreateMenuCube(
			new Vector3(0.11f, 1f, 0.9f), new Vector3(0.05f, 0f, 0f),
			new Color32(70, 30, 120, byte.MaxValue), interactive: false, useWorldPosition: true);
		BackgroundRenderer = background.GetComponent<Renderer>();
		GameObject border = CreateMenuCube(
			new Vector3(0.115f, 1.01f, 0.91f), new Vector3(0.0495f, 0f, 0f),
			new Color32(180, 90, 230, byte.MaxValue), interactive: false, useWorldPosition: true);

		CreateMenuCanvas(2000f);
		RoundRendererCorners(background);
		RoundRendererCorners(border);
		CreateText("Menu Made By hamsterman", FontStyle.Bold, 3,
			new Vector2(0.3f, 0.13f), new Vector3(0.04f, 0f, 0.030784313f), new Vector3(0f, 90f, 90f));
		CreateText("Rexon Paid", FontStyle.BoldAndItalic, 2,
			new Vector2(0.2f, 0.05f), new Vector3(0.07f, 0f, 0.16f), new Vector3(180f, 90f, 90f), worldSpace: true);
		string status = $"FPS: {Mathf.RoundToInt(DisplayedFps)}  Page: {PageIndex + 1}/{GetPageCount()}";
		StatusText = CreateText(status, FontStyle.Bold, 1,
			new Vector2(0.2f, 0.03f), new Vector3(0.07f, 0f, 0.125f), new Vector3(180f, 90f, 90f), worldSpace: true);

		GameObject leaveButton = CreateInteractiveCube("Leave",
			new Vector3(0.09f, 0.22f, 0.06f), new Vector3(0.52f, 0.565f, 0.525f),
			new Color32(200, 50, 50, byte.MaxValue));
		GameObject leaveBorder = CreateMenuCube(
			new Vector3(0.095f, 0.225f, 0.065f), new Vector3(0.515f, 0.565f, 0.525f),
			new Color32(byte.MaxValue, 200, 200, byte.MaxValue), interactive: false);
		RoundRendererCorners(leaveButton);
		RoundRendererCorners(leaveBorder);
		CreateText("Leave", FontStyle.Bold, 1,
			new Vector2(0.05f, 0.03f), new Vector3(0.064f, 0.164f, 0.21f), new Vector3(180f, 90f, 90f));

		GameObject nextButton = CreateInteractiveCube("NextPage",
			new Vector3(0.05f, 0.26f, 0.08f), new Vector3(0.57f, -0.31f, -0.38f),
			new Color32(120, 40, 180, byte.MaxValue));
		GameObject nextBorder = CreateMenuCube(
			new Vector3(0.045f, 0.27f, 0.085f), new Vector3(0.565f, -0.31f, -0.38f),
			new Color32(180, 90, 230, byte.MaxValue), interactive: false);
		RoundRendererCorners(nextButton);
		RoundRendererCorners(nextBorder);
		CreateText(">", FontStyle.Bold, 2,
			new Vector2(0.08f, 0.03f), new Vector3(0.067f, -0.113f, -0.15f), new Vector3(180f, 90f, 90f));

		GameObject previousButton = CreateInteractiveCube("PreviousPage",
			new Vector3(0.05f, 0.26f, 0.08f), new Vector3(0.57f, 0.31f, -0.38f),
			new Color32(120, 40, 180, byte.MaxValue));
		GameObject previousBorder = CreateMenuCube(
			new Vector3(0.045f, 0.27f, 0.085f), new Vector3(0.565f, 0.31f, -0.38f),
			new Color32(180, 90, 230, byte.MaxValue), interactive: false);
		RoundRendererCorners(previousButton);
		RoundRendererCorners(previousBorder);
		CreateText("<", FontStyle.Bold, 2,
			new Vector2(0.08f, 0.03f), new Vector3(0.067f, 0.113f, -0.15f), new Vector3(180f, 90f, 90f));

		GameObject homeButton = CreateInteractiveCube("Home",
			new Vector3(0.05f, 0.18f, 0.08f), new Vector3(0.57f, 0f, -0.38f),
			new Color32(140, 50, 190, byte.MaxValue));
		GameObject homeBorder = CreateMenuCube(
			new Vector3(0.045f, 0.185f, 0.085f), new Vector3(0.565f, 0f, -0.38f),
			new Color32(180, 90, 230, byte.MaxValue), interactive: false);
		RoundRendererCorners(homeButton);
		RoundRendererCorners(homeBorder);
		CreateText("⌂", FontStyle.Bold, 3,
			new Vector2(0.08f, 0.03f), new Vector3(0.067f, 0f, -0.15f), new Vector3(180f, 90f, 90f));

		PopulateStyleOneButtons();
		ShaderPatch.Skip = false;
	}

	private static void PopulateStyleOneButtons()
	{
		const int pageSize = 5;
		int firstItem = PageIndex * pageSize;
		if (ShowingCategories)
		{
			List<string> categories = new List<string>(BundleManager.Categories);
			for (int slot = 0; slot < pageSize && firstItem + slot < categories.Count; slot++)
			{
				string category = categories[firstItem + slot];
				CreateStyleOneButton(slot * 0.117f + 0.09f, category,
					new Color32(60, 25, 100, byte.MaxValue), isCategory: true, category, -1);
			}
			return;
		}

		List<int> modIndexes = BundleManager.GetModIndexesForCategory(SelectedCategory);
		for (int slot = 0; slot < pageSize && firstItem + slot < modIndexes.Count; slot++)
		{
			int modIndex = modIndexes[firstItem + slot];
			bool enabled = BundleManager.IsModEnabled(modIndex);
			bool isAction = BundleManager.GetModType(modIndex) == ModType.Action;
			Color32 color = enabled && !isAction
				? new Color32(120, 40, 180, byte.MaxValue)
				: new Color32(60, 25, 100, byte.MaxValue);
			CreateStyleOneButton(slot * 0.117f + 0.09f, BundleManager.GetModDisplayName(modIndex),
				color, isCategory: false, null, modIndex);
		}
	}

	private static void CreateStyleOneButton(
		float verticalOffset,
		string label,
		Color32 color,
		bool isCategory,
		string categoryName,
		int modIndex)
	{
		GameObject button = CreateMenuCube(
			new Vector3(0.085f, 0.88f, 0.107f),
			new Vector3(0.57f, 0f, 0.295f - verticalOffset),
			color,
			interactive: true);
		Buttons.Add(new ButtonData
		{
			GameObject = button,
			Collider = button.GetComponent<BoxCollider>(),
			Action = string.Empty,
			ModIndex = modIndex,
			IsCategory = isCategory,
			CategoryName = categoryName
		});

		GameObject border = CreateMenuCube(
			new Vector3(0.09f, 0.885f, 0.112f),
			new Vector3(0.565f, 0f, 0.295f - verticalOffset),
			new Color32(180, 90, 230, byte.MaxValue),
			interactive: false);
		RoundRendererCorners(button);
		RoundRendererCorners(border);
		CreateText(label, FontStyle.Bold, 0,
			new Vector2(0.21f, 0.0275f),
			new Vector3(0.07f, 0f, 0.119f - verticalOffset / 2.48f),
			new Vector3(180f, 90f, 90f));
	}

	private static void RoundRendererCorners(GameObject gameObject, float radius = 0.02f)
	{
		Renderer sourceRenderer = gameObject.GetComponent<Renderer>();
		Vector3 sourceScale = gameObject.transform.localScale;
		Vector3 sourcePosition = gameObject.transform.localPosition;

		GameObject verticalFill = GameObject.CreatePrimitive((PrimitiveType)3);
		verticalFill.GetComponent<Renderer>().enabled = sourceRenderer.enabled;
		Object.Destroy(verticalFill.GetComponent<Collider>());
		verticalFill.transform.parent = MenuRoot.transform;
		verticalFill.transform.rotation = Quaternion.identity;
		verticalFill.transform.localPosition = sourcePosition;
		verticalFill.transform.localScale = sourceScale + new Vector3(0f, radius * -2.55f, 0f);

		GameObject horizontalFill = GameObject.CreatePrimitive((PrimitiveType)3);
		horizontalFill.GetComponent<Renderer>().enabled = sourceRenderer.enabled;
		Object.Destroy(horizontalFill.GetComponent<Collider>());
		horizontalFill.transform.parent = MenuRoot.transform;
		horizontalFill.transform.rotation = Quaternion.identity;
		horizontalFill.transform.localPosition = sourcePosition;
		horizontalFill.transform.localScale = sourceScale + new Vector3(0f, 0f, -radius * 2f);

		float[] ySigns = { 1f, -1f, 1f, -1f };
		float[] zSigns = { 1f, 1f, -1f, -1f };
		GameObject[] corners = new GameObject[4];
		for (int cornerIndex = 0; cornerIndex < corners.Length; cornerIndex++)
		{
			GameObject corner = GameObject.CreatePrimitive((PrimitiveType)2);
			corner.GetComponent<Renderer>().enabled = sourceRenderer.enabled;
			Object.Destroy(corner.GetComponent<Collider>());
			corner.transform.parent = MenuRoot.transform;
			corner.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
			corner.transform.localPosition = sourcePosition + new Vector3(
				0f,
				ySigns[cornerIndex] * (sourceScale.y / 2f - radius * 1.275f),
				zSigns[cornerIndex] * (sourceScale.z / 2f - radius));
			corner.transform.localScale = new Vector3(radius * 2.55f, sourceScale.x / 2f, radius * 2f);
			corners[cornerIndex] = corner;
		}

		GameObject[] roundedParts =
		{
			verticalFill,
			horizontalFill,
			corners[0],
			corners[1],
			corners[2],
			corners[3]
		};
		foreach (GameObject part in roundedParts)
		{
			part.AddComponent<RoundClampColor>().SourceRenderer = sourceRenderer;
		}
		sourceRenderer.enabled = false;
	}

	private static void BuildStyleTwoMenu()
	{
		ShaderPatch.Skip = true;
		MenuRoot = GameObject.CreatePrimitive((PrimitiveType)3);
		Object.Destroy(MenuRoot.GetComponent<Rigidbody>());
		Object.Destroy(MenuRoot.GetComponent<BoxCollider>());
		Object.Destroy(MenuRoot.GetComponent<Renderer>());
		MenuRoot.transform.localScale = new Vector3(0.11f, 0.29f, 0.4f);

		GameObject background = CreateMenuCube(
			new Vector3(0.11f, 1f, 0.9f), new Vector3(0.05f, 0f, 0f),
			new Color32(50, 25, 80, byte.MaxValue), interactive: false, useWorldPosition: true);
		BackgroundRenderer = background.GetComponent<Renderer>();
		CreateMenuCanvas(1000f);

		CreateText("Menu Made By hamsterman", FontStyle.Normal, 4,
			new Vector2(0.3f, 0.13f), new Vector3(0.04f, 0f, 0.030784313f), new Vector3(0f, 90f, 90f));
		string title = $"Rexon Paid [Page {PageIndex + 1}/{GetPageCount()}]";
		CreateText(title, FontStyle.BoldAndItalic, 1,
			new Vector2(0.28f, 0.05f), new Vector3(0.07f, 0f, 0.15f), new Vector3(180f, 90f, 90f), worldSpace: true);

		CreateInteractiveCube("PreviousPage",
			new Vector3(0.03f, 0.11f, 0.8f), new Vector3(0.56f, 0.57f, -0.028254911f),
			new Color32(80, 20, 120, byte.MaxValue));
		CreateMenuCube(
			new Vector3(0.02f, 0.08f, 0.7f), new Vector3(0.58f, 0.4f, -0.05325491f),
			new Color32(80, 20, 120, byte.MaxValue), interactive: false);
		CreateText("<<<<<<", FontStyle.Normal, 1,
			new Vector2(0.2f, 0.03f), new Vector3(0.064f, 0.165f, -0.028254911f), new Vector3(180f, 90f, 180f));

		CreateInteractiveCube("NextPage",
			new Vector3(0.03f, 0.11f, 0.8f), new Vector3(0.56f, -0.57f, -0.028254911f),
			new Color32(80, 20, 120, byte.MaxValue));
		CreateMenuCube(
			new Vector3(0.02f, 0.08f, 0.7f), new Vector3(0.58f, -0.4f, -0.05325491f),
			new Color32(80, 20, 120, byte.MaxValue), interactive: false);
		CreateText(">>>>>>", FontStyle.Normal, 1,
			new Vector2(0.2f, 0.03f), new Vector3(0.064f, -0.165f, -0.028254911f), new Vector3(180f, 90f, 180f));

		CreateInteractiveCube("Leave",
			new Vector3(0.07f, 0.8f, 0.12f), new Vector3(0.56f, 0f, 0.5427451f),
			new Color32(100, 30, 140, byte.MaxValue));
		CreateText("Disconnect", FontStyle.Normal, 2,
			new Vector2(0.2f, 0.03f), new Vector3(0.07f, 0f, 0.2207843f), new Vector3(180f, 90f, 90f));

		PopulateStyleTwoButtons();
		ShaderPatch.Skip = false;
	}

	private static void PopulateStyleTwoButtons()
	{
		int pageSize = ShowingCategories ? 5 : 4;
		int firstItem = PageIndex * pageSize;
		if (ShowingCategories)
		{
			List<string> categories = new List<string>(BundleManager.Categories);
			for (int slot = 0; slot < pageSize && firstItem + slot < categories.Count; slot++)
			{
				string category = categories[firstItem + slot];
				CreateStyleTwoButton(slot * 0.1f + 0.05f, category,
					new Color32(40, 15, 60, byte.MaxValue), isCategory: true, category, -1);
			}
			return;
		}

		List<int> modIndexes = BundleManager.GetModIndexesForCategory(SelectedCategory);
		for (int slot = 0; slot < pageSize && firstItem + slot < modIndexes.Count; slot++)
		{
			int modIndex = modIndexes[firstItem + slot];
			bool enabled = BundleManager.IsModEnabled(modIndex);
			bool isAction = BundleManager.GetModType(modIndex) == ModType.Action;
			Color32 color = enabled && !isAction
				? new Color32(100, 30, 140, byte.MaxValue)
				: new Color32(40, 15, 60, byte.MaxValue);
			CreateStyleTwoButton(slot * 0.1f + 0.05f, BundleManager.GetModDisplayName(modIndex),
				color, isCategory: false, null, modIndex);
		}
	}

	private static void CreateStyleTwoButton(
		float verticalOffset,
		string label,
		Color32 color,
		bool isCategory,
		string categoryName,
		int modIndex,
		string action = "")
	{
		GameObject button = CreateMenuCube(
			new Vector3(0.085f, 0.78f, 0.07f),
			new Vector3(0.57f, 0f, 0.295f - verticalOffset),
			color,
			interactive: true);
		Buttons.Add(new ButtonData
		{
			GameObject = button,
			Collider = button.GetComponent<BoxCollider>(),
			Action = action,
			ModIndex = modIndex,
			IsCategory = isCategory,
			CategoryName = categoryName
		});

		float textOffset = verticalOffset - 0.02f;
		CreateText(label, FontStyle.Normal, 0,
			new Vector2(0.21f, 0.0275f),
			new Vector3(0.07f, 0f, 0.111f - textOffset / 2.514f),
			new Vector3(180f, 90f, 90f));
	}

	private static Text CreateText(
		string text,
		FontStyle fontStyle,
		int fontSize,
		Vector2 size,
		Vector3 position,
		Vector3 eulerAngles,
		bool worldSpace = false)
	{
		GameObject textObject = new GameObject("LegacyText");
		textObject.transform.parent = CanvasObject.transform;
		Text label = textObject.AddComponent<Text>();
		label.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
		label.text = text;
		label.fontSize = fontSize;
		label.fontStyle = fontStyle;
		label.alignment = (TextAnchor)4;
		label.resizeTextForBestFit = true;
		label.resizeTextMinSize = 0;

		RectTransform rectTransform = label.GetComponent<RectTransform>();
		rectTransform.localPosition = Vector3.zero;
		rectTransform.sizeDelta = size;
		if (worldSpace)
		{
			rectTransform.position = position;
		}
		else
		{
			rectTransform.localPosition = position;
		}
		rectTransform.rotation = Quaternion.Euler(eulerAngles);
		return label;
	}

	private void OnGUI()
	{
		if (MenuStyle != 0 && DesktopMenuVisible)
		{
			if (MenuStyle == 1)
			{
				GUI.backgroundColor = ScaleThemeColor(0.7f, 0.92f);
				WindowRect = GUILayout.Window(9999, WindowRect, new WindowFunction(DrawDesktopStyleOne), "", Array.Empty<GUILayoutOption>());
			}
			else if (MenuStyle == 2)
			{
				GUI.backgroundColor = ScaleThemeColor(0.55f, 0.92f);
				WindowRect = GUILayout.Window(9999, WindowRect, new WindowFunction(DrawDesktopStyleTwo), "", Array.Empty<GUILayoutOption>());
			}
		}
	}

	private void DrawDesktopStyleOne(int index)
	{
		GUI.Box(new Rect(0f, 0f, WindowRect.width, WindowRect.height), GUIContent.none);
		GUI.backgroundColor = ScaleThemeColor(1.8f, 0.95f);
		GUI.Box(new Rect(10f, 10f, 170f, 39f), GUIContent.none);
		GUIStyle titleStyle = new GUIStyle(GUI.skin.label)
		{
			fontSize = 25,
			fontStyle = (FontStyle)1,
			alignment = (TextAnchor)4
		};
		titleStyle.normal.textColor = Color.white;
		GUI.Label(new Rect(10f, 10f, 170f, 39f), "Rexon Paid", titleStyle);
		GUILayout.Space(40f);
		GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		GUILayout.BeginVertical(new GUILayoutOption[1] { GUILayout.Width(175f) });
		if (Categories.Count == 0)
		{
			Categories = new List<string>(BundleManager.Categories);
		}
		for (int i = 0; i < Categories.Count; i++)
		{
			GUI.backgroundColor = ((SelectedDesktopCategoryIndex == i) ? ScaleThemeColor(2.8f, 0.9f) : ScaleThemeColor(1.1f, 0.9f));
			if (GUILayout.Button(Categories[i], new GUILayoutOption[1] { GUILayout.Height(25f) }))
			{
				SelectedDesktopCategoryIndex = i;
				ScrollPosition = Vector2.zero;
			}
		}
		GUILayout.EndVertical();
		GUILayout.BeginVertical(new GUILayoutOption[1] { GUILayout.Width(465f) });
		GUILayout.Space(7f);
		ScrollPosition = GUILayout.BeginScrollView(ScrollPosition, false, true, new GUILayoutOption[2]
		{
			GUILayout.Width(460f),
			GUILayout.Height(390f)
		});
		DrawDesktopModList(SelectedDesktopCategoryIndex, alternateStyle: false);
		GUILayout.EndScrollView();
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
		Rect disconnectArea = new Rect(WindowRect.width - 150f, WindowRect.height - 40f, 140f, 40f);
		GUILayout.BeginArea(disconnectArea);
		GUI.backgroundColor = Color.red;
		if (GUILayout.Button("Disconnect", new GUILayoutOption[1] { GUILayout.Height(30f) }))
		{
			PhotonNetwork.Disconnect();
		}
		GUILayout.EndArea();
		Rect dragArea = new Rect(0f, 0f, Screen.width, Screen.height);
		GUI.DragWindow(dragArea);
	}

	private void DrawDesktopStyleTwo(int index)
	{
		GUI.Box(new Rect(0f, 0f, WindowRect.width, WindowRect.height), GUIContent.none);
		GUI.backgroundColor = ScaleThemeColor(1.5f, 0.95f);
		GUI.Box(new Rect(10f, 10f, 170f, 39f), GUIContent.none);
		GUIStyle titleStyle = new GUIStyle(GUI.skin.label)
		{
			fontSize = 25,
			fontStyle = (FontStyle)1,
			alignment = (TextAnchor)4
		};
		titleStyle.normal.textColor = Color.white;
		GUI.Label(new Rect(10f, 10f, 170f, 39f), "Rexon Paid", titleStyle);
		GUILayout.Space(40f);
		GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		GUILayout.BeginVertical(new GUILayoutOption[1] { GUILayout.Width(175f) });
		if (Categories.Count == 0)
		{
			Categories = new List<string>(BundleManager.Categories);
		}
		for (int i = 0; i < Categories.Count; i++)
		{
			GUI.backgroundColor = ((SelectedDesktopCategoryIndex == i) ? ScaleThemeColor(2.3f, 0.9f) : ScaleThemeColor(0.65f, 0.9f));
			if (GUILayout.Button(Categories[i], new GUILayoutOption[1] { GUILayout.Height(25f) }))
			{
				SelectedDesktopCategoryIndex = i;
				ScrollPosition = Vector2.zero;
			}
		}
		GUILayout.EndVertical();
		GUILayout.BeginVertical(new GUILayoutOption[1] { GUILayout.Width(465f) });
		GUILayout.Space(7f);
		ScrollPosition = GUILayout.BeginScrollView(ScrollPosition, false, true, new GUILayoutOption[2]
		{
			GUILayout.Width(460f),
			GUILayout.Height(390f)
		});
		DrawDesktopModList(SelectedDesktopCategoryIndex, alternateStyle: true);
		GUILayout.EndScrollView();
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
		Rect disconnectArea = new Rect(WindowRect.width - 150f, WindowRect.height - 40f, 140f, 40f);
		GUILayout.BeginArea(disconnectArea);
		GUI.backgroundColor = Color.red;
		if (GUILayout.Button("Disconnect", new GUILayoutOption[1] { GUILayout.Height(30f) }))
		{
			PhotonNetwork.Disconnect();
		}
		GUILayout.EndArea();
		Rect dragArea = new Rect(0f, 0f, Screen.width, Screen.height);
		GUI.DragWindow(dragArea);
	}

	private void DrawDesktopModList(int index, bool alternateStyle)
	{
		if (Categories.Count == 0 || index < 0 || index >= Categories.Count)
		{
			return;
		}
		List<int> modIndexes = BundleManager.GetModIndexesForCategory(Categories[index]);
		Color enabledColor = alternateStyle ? ScaleThemeColor(2.3f, 1f) : ScaleThemeColor(2.8f, 1f);
		Color disabledColor = alternateStyle ? ScaleThemeColor(0.65f, 1f) : ScaleThemeColor(1.1f, 1f);
		for (int i = 0; i < modIndexes.Count; i++)
		{
			int modIndex = modIndexes[i];
			string displayName = BundleManager.GetModDisplayName(modIndex);
			bool enabled = BundleManager.IsModEnabled(modIndex);
			if (BundleManager.GetModType(modIndex) == ModType.Action)
			{
				GUI.backgroundColor = disabledColor;
			}
			else
			{
				GUI.backgroundColor = enabled ? enabledColor : disabledColor;
			}
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Space(8f);
			GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
			buttonStyle.fontSize = CalculateFittingFontSize(displayName, 430, 28);
			if (GUILayout.Button(displayName, buttonStyle, new GUILayoutOption[2]
			{
				GUILayout.Width(430f),
				GUILayout.Height(28f)
			}))
			{
				BundleManager.ToggleMod(modIndex, null, useLegacyToggle: false);
			}
			GUILayout.EndHorizontal();
		}
	}

	private int CalculateFittingFontSize(string text, int maxWidth, int maxHeight)
	{
		int candidateSize = maxHeight - 5;
		GUIStyle style = new GUIStyle(GUI.skin.button);
		while (candidateSize > 8)
		{
			style.fontSize = candidateSize;
			Vector2 contentSize = style.CalcSize(new GUIContent(text));
			if (contentSize.x <= maxWidth && contentSize.y <= maxHeight)
			{
				break;
			}
			candidateSize--;
		}
		return candidateSize;
	}

	static LegacyMenu()
	{
		MenuStyle = 0;
		ShowingCategories = true;
		SelectedCategory = null;
		PageIndex = 0;
		ReturnPageIndex = 0;
		DesktopMenuVisible = false;
		WindowRect = new Rect(200f, 200f, 680f, 390f);
		SelectedDesktopCategoryIndex = 0;
		Categories = new List<string>();
		SmoothedFps = 0f;
		DisplayedFps = 0f;
		NextFpsUpdate = 0f;
		Buttons = new List<ButtonData>();
	}
}
