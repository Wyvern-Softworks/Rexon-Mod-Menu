// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Interface.BundleManager
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using Recovered.Obfuscated;
using Rexon_Menu.Core.Attributes;
using Rexon_Menu.Core.Modules.Overpowered;
using Rexon_Menu.Core.Modules.Room;
using Rexon_Menu.Core.Modules.World;
using Rexon_Menu.Core.Patches;
using Rexon_Menu.Core.Utilities;
using Rexon_Menu_Mat;
using Rexon_Shader;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;
using UnityEngine.XR;

using Object = UnityEngine.Object;

using Random = System.Random;

namespace Rexon_Menu.Interface;

public static class BundleManager
{
	public class OriginalColors : MonoBehaviour
	{
		private ColorBlock _originalColors;

		public void Store(ColorBlock colors)
		{
			_originalColors = colors;
		}

		public ColorBlock Get()
		{
			return _originalColors;
		}
	}

	internal struct ModInfo
	{
		public Type Type;

		public Mod Attribute;

		public ModInfo(Type type, Mod attribute)
		{
			Type = type;
			Attribute = attribute;
		}
	}

	private static readonly byte[] EmbeddedBundleEncryptionKey;

	private static readonly byte[] PayloadCipherTable =
	{
		95, 6, 48, 19, 140, 139, 83, 7, 77, 238, 70, 102, 122, 102, 148, 89,
		75, 182, 201, 102, 0, 123, 12, 172, 86, 133, 34, 204, 181, 44, 195, 195,
		148, 232, 68, 59, 36, 42, 128, 117, 134, 55, 94, 233, 128, 252, 212, 150,
		114, 158, 44, 93, 239, 169, 91, 32, 164, 207, 229, 180, 84, 137, 119, 142,
		86, 48, 70, 32, 130, 2, 110, 12, 49, 146, 252, 93, 163, 26, 144, 130,
		235, 180, 2, 33, 224, 55, 156, 60, 151, 39, 238, 247, 88, 252, 53, 77,
		247, 153, 36, 227, 225, 179, 206, 83, 214, 210, 118, 98, 189, 92, 12, 227,
		187, 42, 84, 154, 13, 215, 29, 204, 213, 104, 62, 42, 26, 44, 160, 128,
		38, 58, 223, 123, 84, 107, 164, 156, 70, 26, 226, 21, 198, 216, 222, 232,
		195, 133, 23, 159, 219, 167, 105, 19, 193, 25, 136, 196, 162, 42, 138, 147,
		25, 178, 175, 151, 241, 155, 255, 135, 230, 136, 143, 128, 209, 37, 99, 173,
		177, 155, 92, 13, 235, 149, 255, 246, 31, 213, 117, 15, 28, 218, 56, 165,
		106, 33, 84, 166, 213, 75, 82, 186, 225, 141, 59, 39, 66, 102, 116, 251,
		64, 54, 56, 129, 164, 173, 174, 113, 209, 92, 247, 167, 88, 114, 172, 61,
		249, 1, 138, 115, 233, 240, 120, 16, 214, 47, 143, 13, 136, 161, 56, 2,
		168, 87, 47, 97, 122, 203, 60, 114, 67, 147, 155, 0, 150, 118, 164, 112,
		111, 54, 141, 97, 134, 11, 67, 82, 40, 54, 205, 173, 155, 214, 157, 73,
		246, 24, 28, 48, 119, 227, 200, 77, 189, 63, 191, 55, 116, 241, 190, 180,
		42, 108, 55, 201, 81, 158, 173, 242, 145, 5, 3, 243, 18, 191, 49, 89,
		101, 86, 66, 6, 205, 63, 182, 42, 87, 7, 192, 213, 34, 71, 1, 177,
		239, 85, 165, 98, 77, 40, 161, 23, 107, 239, 86, 12, 185, 106, 230, 241,
		15, 73, 0, 136, 15, 153, 211, 155, 162, 89, 230, 21, 21, 214, 175, 100,
		232, 9, 137, 139, 184, 14, 152, 132, 129, 185, 236, 219, 64, 59, 28, 241,
		201, 94, 88, 91, 16, 217, 202, 154, 206, 37, 182, 63, 203, 67, 171, 153,
		218, 77, 164, 152, 158, 239, 122, 246, 83, 10, 131, 71, 185, 192, 163, 244,
		234, 24, 200, 207, 161, 34, 221, 94, 13, 41, 74, 26, 126, 14, 125, 186,
		131, 129, 192, 240, 182, 102, 170, 57, 20, 150, 223, 0, 7, 185, 34, 141,
		116, 183, 119, 160, 24, 235, 165, 86, 239, 106, 36, 28, 24, 111, 186, 237,
		74, 174, 158, 133, 48, 227, 59, 48, 255, 61, 200, 226, 131, 148, 61, 152,
		107, 72, 209, 28, 195, 106, 152, 156, 72, 67, 130, 9, 178, 201, 6, 220,
		166, 32, 95, 106, 159, 109, 59, 174, 179, 114, 195, 121, 48, 131, 254, 32,
		53, 119, 47, 63, 215, 7, 229, 245, 25, 130, 0, 60, 20, 66, 246, 164
	};


	internal static GameObject VrMenuPanel;
	internal static GameObject PcMenuPanel;
	private static RectTransform VrBackgroundRect;
	internal static RectTransform PcBackgroundRect;
	internal static RectTransform PcBorderRect;

	private static string GunTracerStatusText;
	private static string GunColorStatusText;
	private static string GunSoundStatusText;
	private static string AutoLoadStatusText;
	private static string AntiReportTypeStatusText;
	private static string AntiReportDistanceStatusText;
	private static string ProjectileColorStatusText;
	private static string ProjectileSpeedStatusText;
	private static string ImpactColorStatusText;
	private static string VisualThemeStatusText;
	private static string LagPowerStatusText;
	private static string SteamLongArmsStatusText;
	internal static string SoundboardHearSelfStatusText;
	private static string MenuHandStatusText;
	private static string SpeedBoostStatusText;
	private static string BackTrackDelayStatusText;
	private static string AudioIndexStatusText;
	private static string CritterSizeStatusText;
	private static string FoodSizeStatusText;
	private static string GravityStatusText;
	private static string MenuStyleStatusText;
	private static string KickTypeStatusText;
	private static string ThemeStatusText;
	private static string PlatformColorStatusText;

	private static List<ModInfo> AllMods;
	private static List<ModInfo> EnabledMods;
	private static GameObject[] VrMenuButtonObjects;
	private static GameObject[] PcMenuButtonObjects;
	internal static List<string> Categories;
	private static List<int> SearchResultIndexes;

	internal static int CurrentPageIndex;
	internal static int ReturnPageIndex;
	private static int PageBeforeSearch;
	internal static string CurrentCategory;
	private static string CategoryBeforeSearch;
	internal static bool ShowingCategories;
	private static bool ShowingCategoriesBeforeSearch;
	private static bool SearchModeActive;
	private static string SearchQuery;
	private static bool UseDesktopSearchInput;

	private static Transform VrBackgroundTransform;
	private static Transform PcBackgroundTransform;
	private static GameObject MenuPointerCollider;
	private static GameObject SearchInputPanel;
	private static TMP_InputField SearchInputField;
	private static Button VrLeaveButton;
	private static Button PcLeaveButton;
	internal static Image VrLeaveButtonImage;
	internal static Image PcLeaveButtonImage;
	private static Image VrBackgroundImage;
	private static Image PcBackgroundImage;
	private static TextMeshProUGUI VrFpsLabel;
	private static TextMeshProUGUI PcFpsLabel;

	private static float LastButtonClickTime;
	private static bool LeaveConfirmationArmed;
	private static float LeaveConfirmationStartedAt;
	private static float SmoothedFps;
	private static float DisplayedFps;
	private static float NextFpsUpdateTime;
	private static float RainbowThemeHue;
	private static float RainbowPlatformHue;
	private static float InterfaceInitializationTime;
	private static float LastMenuStatePollTime;
	private static float LastThemeAuthorizationPollTime;

	internal static readonly Color DefaultLeaveButtonColor;
	private static readonly Color LeaveConfirmationColor;
	private static readonly Color LeaveConfirmedColor;
	internal static Color CurrentThemeColor;
	internal static bool RainbowThemeEnabled;
	private static readonly string[] CategoryDisplayOrder;
	private static Random PayloadRandom;

	internal static bool PrimaryAuthorizationInProgress;
	internal static bool PrimaryAuthorizationSucceeded;
	internal static bool SecondaryAuthorizationInProgress;
	private static bool SecondaryAuthorizationScheduled;
	internal static string AuthorizationKey;
	internal static string DeviceFingerprint;
	public static GameObject UIPanelVR => VrMenuPanel;
	public static GameObject UIPanelPC => PcMenuPanel;
	public static RectTransform BackgroundRectVR => VrBackgroundRect;
	public static RectTransform BackgroundRectPC => PcBackgroundRect;
	public static RectTransform BorderRectPC => PcBorderRect;
	public static bool IsSearchInputFocused =>
		SearchInputField != null && SearchInputField.isFocused;

	private static void ConfigureMenuButtonCollider(
		GameObject buttonObject,
		bool isNavigationButton,
		string navigationAction)
	{
		BoxCollider existingCollider = buttonObject.GetComponent<BoxCollider>();
		if (existingCollider != null)
		{
			Object.Destroy(existingCollider);
		}

		BoxCollider collider = buttonObject.AddComponent<BoxCollider>();
		collider.isTrigger = true;
		RectTransform rectTransform = buttonObject.GetComponent<RectTransform>();
		collider.size = rectTransform != null
			? new Vector3(rectTransform.rect.width, rectTransform.rect.height, 50f)
			: new Vector3(100f, 100f, 50f);
		collider.center = Vector3.zero;

		MenuButtonHandler handler = buttonObject.GetComponent<MenuButtonHandler>();
		if (handler == null)
		{
			handler = buttonObject.AddComponent<MenuButtonHandler>();
		}
		handler.IsNavigationButton = isNavigationButton;
		handler.NavigationAction = navigationAction;
	}

	private static byte[] DecryptEmbeddedBundle(byte[] data)
	{
		byte[] initializationVector = new byte[16];
		Buffer.BlockCopy(data, 0, initializationVector, 0, initializationVector.Length);
		RijndaelManaged rijndaelManaged = new RijndaelManaged
		{
			Key = EmbeddedBundleEncryptionKey,
			IV = initializationVector,
			Mode = CipherMode.CBC,
			Padding = PaddingMode.PKCS7
		};
		ICryptoTransform cryptoTransform = rijndaelManaged.CreateDecryptor();
		return cryptoTransform.TransformFinalBlock(data, 16, data.Length - 16);
	}

	private static void CreateSearchInputPanel()
	{
		if (PcBackgroundTransform == null)
		{
			return;
		}

		SearchInputPanel = new GameObject("SearchInputPanel");
		SearchInputPanel.transform.SetParent(PcBackgroundTransform.parent, false);
		RectTransform panelRect = SearchInputPanel.AddComponent<RectTransform>();
		panelRect.anchorMin = new Vector2(0.5f, 0.5f);
		panelRect.anchorMax = new Vector2(0.5f, 0.5f);
		float menuTop = PcBackgroundRect != null
			? PcBackgroundRect.anchoredPosition.y
				+ PcBackgroundRect.sizeDelta.y * PcBackgroundRect.localScale.y / 2f
			: 340f;
		panelRect.anchoredPosition = new Vector2(0f, menuTop + 30f);
		panelRect.sizeDelta = new Vector2(670f, 50f);

		Image background = SearchInputPanel.AddComponent<Image>();
		background.color = new Color(0.153f, 0f, 0.263f, 1f);
		Outline outline = SearchInputPanel.AddComponent<Outline>();
		outline.effectColor = new Color(0.755f, 0.533f, 1f, 1f);
		outline.effectDistance = new Vector2(2f, -2f);

		GameObject textAreaObject = new GameObject("Text Area");
		textAreaObject.transform.SetParent(SearchInputPanel.transform, false);
		RectTransform textArea = textAreaObject.AddComponent<RectTransform>();
		textArea.anchorMin = Vector2.zero;
		textArea.anchorMax = Vector2.one;
		textArea.offsetMin = new Vector2(10f, 5f);
		textArea.offsetMax = new Vector2(-10f, -5f);
		textAreaObject.AddComponent<RectMask2D>();

		GameObject placeholderObject = new GameObject("Placeholder");
		placeholderObject.transform.SetParent(textAreaObject.transform, false);
		RectTransform placeholderRect = placeholderObject.AddComponent<RectTransform>();
		placeholderRect.anchorMin = Vector2.zero;
		placeholderRect.anchorMax = Vector2.one;
		placeholderRect.offsetMin = Vector2.zero;
		placeholderRect.offsetMax = Vector2.zero;
		TextMeshProUGUI placeholder = placeholderObject.AddComponent<TextMeshProUGUI>();
		placeholder.text = "Search mods...";
		placeholder.fontSize = 24f;
		placeholder.color = new Color(0.5f, 0.4f, 0.6f, 0.6f);
		placeholder.alignment = (TextAlignmentOptions)4097;

		GameObject inputTextObject = new GameObject("Text");
		inputTextObject.transform.SetParent(textAreaObject.transform, false);
		RectTransform inputTextRect = inputTextObject.AddComponent<RectTransform>();
		inputTextRect.anchorMin = Vector2.zero;
		inputTextRect.anchorMax = Vector2.one;
		inputTextRect.offsetMin = Vector2.zero;
		inputTextRect.offsetMax = Vector2.zero;
		TextMeshProUGUI inputText = inputTextObject.AddComponent<TextMeshProUGUI>();
		inputText.fontSize = 24f;
		inputText.color = Color.white;
		inputText.alignment = (TextAlignmentOptions)4097;

		SearchInputField = SearchInputPanel.AddComponent<TMP_InputField>();
		SearchInputField.textViewport = textArea;
		SearchInputField.textComponent = inputText;
		SearchInputField.placeholder = placeholder;
		SearchInputField.fontAsset = inputText.font;
		SearchInputField.pointSize = 24f;
		SearchInputField.caretColor = Color.white;
		SearchInputField.selectionColor = new Color(0.755f, 0.533f, 1f, 0.3f);
		SearchInputField.onValueChanged.AddListener(UpdateSearchQuery);
		SearchInputPanel.SetActive(false);
	}

	private static void CacheMenuButtonObjects()
	{
		VrMenuButtonObjects = GetMenuButtonObjects(VrBackgroundTransform);
		PcMenuButtonObjects = GetMenuButtonObjects(PcBackgroundTransform);
	}


	internal static void HideMenuContent()
	{
		if (VrBackgroundImage != null)
		{
			VrBackgroundImage.color = Color.clear;
		}
		if (PcBackgroundImage != null)
		{
			PcBackgroundImage.color = Color.clear;
		}

		foreach (GameObject buttonObject in VrMenuButtonObjects)
		{
			if (buttonObject != null)
			{
				buttonObject.SetActive(false);
			}
		}
		foreach (GameObject buttonObject in PcMenuButtonObjects)
		{
			if (buttonObject != null)
			{
				buttonObject.SetActive(false);
			}
		}
	}

	internal static Task PollRigColorBufferInternal()
	{
		return Task.CompletedTask;
	}

	public static void InitializeInterface()
	{
		LoadMenuBundles();
		CacheBackgroundRects(PcMenuPanel);
		CacheBackgroundRects(VrMenuPanel);
		ReplaceMenuShaders(VrMenuPanel);
		ReplaceMenuShaders(PcMenuPanel);
		CacheBackgroundLayout();
		InitializeMenuHierarchyAndSettings();
		LocalOnlyPolicy.EnsureDataDirectory();
		SoundboardAudioManager.InitializeAudioDirectory();
		InterfaceInitializationTime = Time.time;
		InitializeAuthorization();
	}

	private static void InitializeAuthorization()
	{
		PrimaryAuthorizationInProgress = false;
		PrimaryAuthorizationSucceeded = true;
		AuthorizationKey = LocalOnlyPolicy.LocalAuthorizationKey;
		DeviceFingerprint = LocalOnlyPolicy.LocalDeviceIdentity;
	}
	public static void AlignMenuPointer()
	{
		if (MenuPointerCollider != null && VrMenuPanel != null)
		{
			Transform background = VrMenuPanel.transform.Find("Background");
			if (background != null)
			{
				MenuPointerCollider.transform.position = background.position + background.forward * 0.001f;
				MenuPointerCollider.transform.rotation = background.rotation;
			}
		}
	}

public static void OpenCategory(string value)
	{
		ReturnPageIndex = CurrentPageIndex;
		CurrentCategory = value;
		CurrentPageIndex = 0;
		ShowingCategories = false;
		if (value == "Enabled")
		{
			RefreshEnabledMods();
		}
		RefreshMenu();
	}

internal static void CloseMenusAndReset()
	{
		SetVrMenuVisible(visible: false);
		SetPcMenuVisible(visible: false);
	}

	public static bool IsModEnabled(int index)
	{
		if (index < 0 || index >= AllMods.Count)
		{
			return false;
		}
		return FindActiveModComponent(AllMods[index].Type) != null;
	}

	private static void AutoLoadSavedMods()
	{
		const string ModsSection = "[MODS]";
		const string SettingsSection = "[SETTINGS]";
		ConfigurationManager.EnsureLoaded();

		List<string> savedModNames = new List<string>();
		if (ConfigurationManager.AutoLoadEnabled)
		{
			string savePath = ConfigurationManager.GetSavePath();
			if (File.Exists(savePath))
			{
				bool inModsSection = false;
				foreach (string rawLine in File.ReadAllLines(savePath).Skip(1))
				{
					string line = rawLine?.Trim();
					if (string.IsNullOrEmpty(line))
					{
						continue;
					}
					if (line == ModsSection)
					{
						inModsSection = true;
					}
					else if (line == SettingsSection)
					{
						inModsSection = false;
					}
					else if (inModsSection)
					{
						savedModNames.Add(line);
					}
				}
			}
		}

		if (savedModNames.Count == 0)
		{
			return;
		}
		foreach (string savedModName in savedModNames)
		{
			ModInfo mod = AllMods.FirstOrDefault(candidate => candidate.Attribute.Name == savedModName);
			if (mod.Type != null && FindActiveModComponent(mod.Type) == null)
			{
				ModManager.Instance.gameObject.AddComponent(mod.Type);
			}
		}

		ConfigurationManager.EnsureLoaded();
		if (ConfigurationManager.AutoLoadEnabled)
		{
			AutoLoadStatusText = "Auto Load: On";
		}
	}

	private static void ConfigureButtonText(TextMeshProUGUI text, string value, Vector2 position)
	{
		text.text = value;
		text.alignment = (TextAlignmentOptions)514;
		text.rectTransform.anchoredPosition = position;
		text.enableWordWrapping = false;
		text.overflowMode = (TextOverflowModes)0;
		text.rectTransform.sizeDelta = new Vector2(400f, text.rectTransform.sizeDelta.y);
	}

	private static void ConfigureModButtonHandler(GameObject buttonObject, ModInfo mod, int modIndex)
	{
		MenuButtonHandler handler = buttonObject.GetComponent<MenuButtonHandler>();
		if (handler == null)
		{
			return;
		}

		handler.IsCategoryButton = false;
		handler.CategoryName = null;
		handler.ModIndex = modIndex;
		handler.TargetModType = mod.Attribute.Type;
		handler.UseLegacyToggle = false;
		handler.IsSoundboardAudio = false;
		handler.SoundboardAudioName = string.Empty;
		handler.IsNavigationButton = false;
		handler.NavigationAction = string.Empty;
	}

	private static void PopulateEnabledModButtons(GameObject[] buttonObjects, bool includePcOnlyMods)
	{
		if (buttonObjects == null || buttonObjects.Length == 0)
		{
			return;
		}

		int firstModIndex = CurrentPageIndex * 6;
		List<ModInfo> visibleMods = includePcOnlyMods
			? EnabledMods
			: EnabledMods.Where(mod => !mod.Attribute.PCOnly).ToList();

		for (int buttonIndex = 0; buttonIndex < buttonObjects.Length; buttonIndex++)
		{
			GameObject buttonObject = buttonObjects[buttonIndex];
			int visibleIndex = firstModIndex + buttonIndex;
			if (visibleIndex >= visibleMods.Count)
			{
				buttonObject.SetActive(false);
				continue;
			}

			buttonObject.SetActive(true);
			ModInfo mod = visibleMods[visibleIndex];
			TextMeshProUGUI label = buttonObject.transform.Find("ButtonText")?.GetComponent<TextMeshProUGUI>();
			TextMeshProUGUI tooltip = buttonObject.transform.Find("Tooltip")?.GetComponent<TextMeshProUGUI>();
			if (label != null)
			{
				ConfigureButtonText(label, mod.Attribute.Name, Vector2.zero);
			}
			if (tooltip != null)
			{
				tooltip.text = string.Empty;
			}

			Button button = buttonObject.GetComponent<Button>();
			if (button == null)
			{
				continue;
			}

			int modIndex = GetModIndex(mod.Type);
			SetButtonEnabledVisual(button, true);
			ConfigureModButtonHandler(buttonObject, mod, modIndex);
			button.onClick.RemoveAllListeners();
			button.onClick.AddListener(() => DisableMod(modIndex));
		}
	}

	internal static string EncryptPayload(string plaintext)
	{
		byte[] plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
		byte[] nonce = new byte[16];
		PayloadRandom.NextBytes(nonce);
		byte[] authenticationTag = ComputePayloadTag(plaintextBytes, nonce);

		int tableOffset = nonce.Sum(value => value) % PayloadCipherTable.Length;
		byte[] ciphertext = new byte[plaintextBytes.Length];
		for (int index = 0; index < plaintextBytes.Length; index++)
		{
			int tableIndex = (tableOffset + index) % PayloadCipherTable.Length;
			ciphertext[index] = (byte)(
				plaintextBytes[index]
				^ PayloadCipherTable[tableIndex]
				^ nonce[index % nonce.Length]);
		}

		byte[] envelope = new byte[nonce.Length + authenticationTag.Length + ciphertext.Length];
		Array.Copy(nonce, 0, envelope, 0, nonce.Length);
		Array.Copy(authenticationTag, 0, envelope, nonce.Length, authenticationTag.Length);
		Array.Copy(ciphertext, 0, envelope, nonce.Length + authenticationTag.Length, ciphertext.Length);

		StringBuilder hexadecimal = new StringBuilder(envelope.Length * 2);
		foreach (byte value in envelope)
		{
			hexadecimal.Append(value.ToString("X2"));
		}
		return hexadecimal.ToString();
	}

	private static void ConfigureSearchBackButton(Transform root)
	{
		if (root == null)
		{
			return;
		}
		Transform searchRoot = root.Find("SearchBack");
		if (searchRoot != null)
		{
			Button button = searchRoot.GetComponent<Button>();
			if (button == null)
			{
				button = searchRoot.gameObject.AddComponent<Button>();
			}
			button.onClick.RemoveAllListeners();
			button.onClick.AddListener(ToggleSearch);
		}
	}

	internal static object ParseJson(string value)
	{
		return SimpleJsonParser.Parse(value);
	}

public static void SetVrMenuVisible(bool visible)
	{
		if (VrMenuPanel != null)
		{
			VrMenuPanel.SetActive(visible);
		}
		if (MenuPointerCollider != null)
		{
			MenuPointerCollider.SetActive(visible);
		}
	}

	internal static bool IsTimestampFresh(Dictionary<string, object> dictionary, string expectedTimestamp)
	{
		if (dictionary == null)
		{
			return false;
		}
		if (!dictionary.ContainsKey("timestamp"))
		{
			return false;
		}
		string receivedTimestamp = dictionary["timestamp"].ToString();
		string normalizedExpectedTimestamp = expectedTimestamp.Replace("Z", "");
		string normalizedReceivedTimestamp = receivedTimestamp.Replace("Z", "");
		if (!DateTimeOffset.TryParse(normalizedExpectedTimestamp, out DateTimeOffset expectedTime))
		{
			return false;
		}
		if (!DateTimeOffset.TryParse(normalizedReceivedTimestamp, out DateTimeOffset receivedTime))
		{
			return false;
		}
		return Math.Abs((expectedTime.UtcDateTime - receivedTime.UtcDateTime).TotalSeconds) <= 60.0;
	}

	public static void SetSoundboardStatusText(string value)
	{
		SoundboardHearSelfStatusText = value;
		if (!ShowingCategories && CurrentCategory == "Soundboard")
		{
			RefreshMenu();
		}
	}

	public static IEnumerator FlashButtonCoroutine(Button button)
	{
		yield return (object)new WaitForSeconds(0.15f);
		SetButtonEnabledVisual(button, enabled: false);
	}

	private static void DiscoverMods()
	{
		AllMods.Clear();
		AllMods.AddRange(
			Assembly.GetExecutingAssembly()
				.GetTypes()
				.Select(type => new ModInfo(type, type.GetCustomAttribute<Mod>()))
				.Where(mod => mod.Attribute != null)
				.OrderBy(mod => mod.Attribute.Order));
	}

	private static void LoadMenuBundles()
	{
		PcMenuPanel = LoadEmbeddedMenuBundle("Rexon_Menu.Resources.imguipc");
		VrMenuPanel = LoadEmbeddedMenuBundle("Rexon_Menu.Resources.imgui");
		if (VrMenuPanel != null)
		{
			VrMenuPanel.transform.localScale = Vector3.one * 0.0004f;
		}
	}

	public static void SetFoodSizeStatusText(string value)
	{
		FoodSizeStatusText = value;
		if (!ShowingCategories && (CurrentCategory == "Settings" || CurrentCategory == "Critter"))
		{
			RefreshMenu();
		}
	}

	private static void PopulateCategoryModButtons(GameObject[] buttonObjects, bool isVrMenu)
	{
		if (buttonObjects == null || buttonObjects.Length == 0)
		{
			return;
		}

		int firstModIndex = CurrentPageIndex * 6;
		List<ModInfo> visibleMods = AllMods
			.Where(mod => mod.Attribute.Category == CurrentCategory)
			.Where(mod => isVrMenu || !mod.Attribute.PCOnly)
			.ToList();

		for (int buttonIndex = 0; buttonIndex < buttonObjects.Length; buttonIndex++)
		{
			GameObject buttonObject = buttonObjects[buttonIndex];
			int visibleIndex = firstModIndex + buttonIndex;
			if (visibleIndex >= visibleMods.Count)
			{
				buttonObject.SetActive(false);
				continue;
			}

			buttonObject.SetActive(true);
			ModInfo mod = visibleMods[visibleIndex];
			string displayName = GetModButtonLabel(mod);
			bool hasDescription = !string.IsNullOrEmpty(mod.Attribute.Description);
			TextMeshProUGUI label = buttonObject.transform.Find("ButtonText")?.GetComponent<TextMeshProUGUI>();
			TextMeshProUGUI tooltip = buttonObject.transform.Find("Tooltip")?.GetComponent<TextMeshProUGUI>();

			if (label != null)
			{
				ConfigureButtonText(label, displayName, hasDescription ? new Vector2(0f, 10f) : Vector2.zero);
			}
			if (tooltip != null)
			{
				if (hasDescription)
				{
					ConfigureButtonText(tooltip, mod.Attribute.Description, new Vector2(0f, -20f));
				}
				else
				{
					tooltip.text = string.Empty;
				}
			}

			Button button = buttonObject.GetComponent<Button>();
			if (button == null)
			{
				continue;
			}

			int modIndex = GetModIndex(mod.Type);
			SetButtonEnabledVisual(button, FindActiveModComponent(mod.Type) != null);
			ConfigureModButtonHandler(buttonObject, mod, modIndex);
			button.onClick.RemoveAllListeners();
			button.onClick.AddListener(() => ToggleMod(modIndex, button, false));
		}
	}

	private static string GetModButtonLabel(ModInfo mod)
	{
		string name = mod.Attribute.Name;
		if (ContainsLabel(name, "Visual Theme:"))
		{
			return VisualThemeStatusText;
		}
		if (ContainsLabel(name, "Theme:"))
		{
			return ThemeStatusText;
		}
		if (ContainsLabel(name, "Platform Color:"))
		{
			return PlatformColorStatusText;
		}
		if (ContainsLabel(name, "Gun Tracer:"))
		{
			return GunTracerStatusText;
		}
		if (ContainsLabel(name, "Gun Color:"))
		{
			return GunColorStatusText;
		}
		if (ContainsLabel(name, "Gun Sound:"))
		{
			return GunSoundStatusText;
		}
		if (ContainsLabel(name, "Auto Load:"))
		{
			return AutoLoadStatusText;
		}
		if (ContainsLabel(name, "Anti Report Type"))
		{
			return AntiReportTypeStatusText;
		}
		if (ContainsLabel(name, "Anti Report Distance"))
		{
			return AntiReportDistanceStatusText;
		}
		if (ContainsLabel(name, "Projectile Color:"))
		{
			return ProjectileColorStatusText;
		}
		if (ContainsLabel(name, "Projectile Speed:"))
		{
			return ProjectileSpeedStatusText;
		}
		if (ContainsLabel(name, "Impact Color:"))
		{
			return ImpactColorStatusText;
		}
		if (ContainsLabel(name, "Lag Power"))
		{
			return LagPowerStatusText;
		}
		if (ContainsLabel(name, "Audio Index"))
		{
			return AudioIndexStatusText;
		}
		if (ContainsLabel(name, "Back Track Delay"))
		{
			return BackTrackDelayStatusText;
		}
		if (ContainsLabel(name, "Steam Long Arms Length"))
		{
			return SteamLongArmsStatusText;
		}
		if (ContainsLabel(name, "Speed Boost:"))
		{
			return SpeedBoostStatusText;
		}
		if (ContainsLabel(name, "Critter Size:"))
		{
			return CritterSizeStatusText;
		}
		if (ContainsLabel(name, "Food Size:"))
		{
			return FoodSizeStatusText;
		}
		if (ContainsLabel(name, "Menu Hand"))
		{
			return MenuHandStatusText;
		}
		if (ContainsLabel(name, "Menu Style"))
		{
			return MenuStyleStatusText;
		}
		if (ContainsLabel(name, "Kick Type:"))
		{
			return KickTypeStatusText;
		}
		if (ContainsLabel(name, "Gravity:"))
		{
			return GravityStatusText;
		}
		return name;
	}

	private static bool ContainsLabel(string text, string fragment)
	{
		return text.IndexOf(fragment, StringComparison.Ordinal) >= 0;
	}
	public static void ChangePage(int pageDelta)
	{
		int itemCount;
		if (SearchModeActive)
		{
			itemCount = SearchResultIndexes.Count;
		}
		else if (ShowingCategories)
		{
			itemCount = Categories.Count;
		}
		else if (CurrentCategory == "Enabled")
		{
			RefreshEnabledMods();
			itemCount = EnabledMods.Count;
		}
		else if (CurrentCategory == "Soundboard")
		{
			SoundboardAudioManager.InitializeAudioDirectory();
			itemCount = SoundboardAudioManager.GetAvailableAudioNames().Length + 1;
		}
		else
		{
			itemCount = AllMods.Count(mod => mod.Attribute.Category == CurrentCategory);
		}

		int lastPage = Mathf.Max(0, Mathf.CeilToInt(itemCount / 6f) - 1);
		CurrentPageIndex = Mathf.Clamp(CurrentPageIndex + pageDelta, 0, lastPage);
		RefreshMenu();
	}

	internal static void DisableAllMods()
	{
		if (ModManager.Instance != null)
		{
			Component[] components = ModManager.Instance.gameObject.GetComponents<Component>();
			foreach (Component component in components)
			{
				if (component == null
					|| component is Transform
					|| component.GetType().Name == "ModManager")
				{
					continue;
				}
				if (component is Behaviour behaviour)
				{
					behaviour.enabled = false;
				}
				Object.Destroy(component);
			}
		}
		EnabledMods.Clear();
		AllMods.Clear();
	}

	public static void SetProjectileColorStatusText(string value)
	{
		ProjectileColorStatusText = value;
		if (!ShowingCategories && CurrentCategory == "Settings")
		{
			RefreshMenu();
		}
	}

	public static List<int> GetModIndexesForCategory(string value)
	{
		List<int> modIndexes = new();
		for (int i = 0; i < AllMods.Count; i++)
		{
			if (AllMods[i].Attribute.Category == value)
			{
				modIndexes.Add(i);
			}
		}
		return modIndexes;
	}

	public static void PollMenuState()
	{
		if (PrimaryAuthorizationSucceeded && Time.time > LastMenuStatePollTime + 30f)
		{
			LastMenuStatePollTime = Time.time;
			PollRigColorBuffer1();
		}
	}

	public static void SetMenuStyleStatusText(string value)
	{
		MenuStyleStatusText = value;
		if (!ShowingCategories && CurrentCategory == "Settings")
		{
			RefreshMenu();
		}
	}

	public static void SetCritterSizeStatusText(string value)
	{
		CritterSizeStatusText = value;
		if (!ShowingCategories && (CurrentCategory == "Settings" || CurrentCategory == "Critter"))
		{
			RefreshMenu();
		}
	}

	private static void InitializeSecondaryAuthorization()
	{
		SecondaryAuthorizationInProgress = false;
	}
	private static void PopulateSearchResults(GameObject[] buttons, bool unused)
	{
		if (buttons == null || buttons.Length == 0)
		{
			return;
		}

		int firstResultIndex = CurrentPageIndex * 6;
		for (int slot = 0; slot < buttons.Length; slot++)
		{
			GameObject buttonObject = buttons[slot];
			int resultIndex = firstResultIndex + slot;
			if (resultIndex >= SearchResultIndexes.Count)
			{
				buttonObject.SetActive(false);
				continue;
			}

			buttonObject.SetActive(true);
			int modIndex = SearchResultIndexes[resultIndex];
			ModInfo mod = AllMods[modIndex];
			Transform labelTransform = buttonObject.transform.Find("ButtonText");
			Transform tooltipTransform = buttonObject.transform.Find("Tooltip");
			TextMeshProUGUI label = labelTransform == null ? null : labelTransform.GetComponent<TextMeshProUGUI>();
			TextMeshProUGUI tooltip = tooltipTransform == null ? null : tooltipTransform.GetComponent<TextMeshProUGUI>();
			Button button = buttonObject.GetComponent<Button>();

			if (label != null)
			{
				((TMP_Text)label).text = GetModDisplayName(modIndex);
				((TMP_Text)label).alignment = (TextAlignmentOptions)514;
				((TMP_Text)label).rectTransform.anchoredPosition = new Vector2(0f, 10f);
				((TMP_Text)label).enableWordWrapping = false;
				((TMP_Text)label).overflowMode = TextOverflowModes.Overflow;
				((TMP_Text)label).rectTransform.sizeDelta = new Vector2(400f, ((TMP_Text)label).rectTransform.sizeDelta.y);
			}

			if (tooltip != null)
			{
				string description = mod.Attribute.Description ?? string.Empty;
				((TMP_Text)tooltip).text = "<color=#b48aff>[" + mod.Attribute.Category + "]</color> " + description;
				((TMP_Text)tooltip).alignment = (TextAlignmentOptions)514;
				((TMP_Text)tooltip).rectTransform.anchoredPosition = new Vector2(0f, -20f);
				((TMP_Text)tooltip).enableWordWrapping = false;
				((TMP_Text)tooltip).overflowMode = TextOverflowModes.Overflow;
				((TMP_Text)tooltip).rectTransform.sizeDelta = new Vector2(500f, ((TMP_Text)tooltip).rectTransform.sizeDelta.y);
			}

			if (button != null)
			{
				SetButtonEnabledVisual(button, FindActiveModComponent(mod.Type) != null);
			}

			MenuButtonHandler handler = buttonObject.GetComponent<MenuButtonHandler>();
			if (handler != null)
			{
				handler.IsCategoryButton = false;
				handler.CategoryName = null;
				handler.ModIndex = modIndex;
				handler.TargetModType = mod.Attribute.Type;
				handler.UseLegacyToggle = false;
				handler.IsSoundboardAudio = false;
				handler.IsNavigationButton = false;
			}
		}
	}

	internal static void UpdateSearchQuery(string value)
	{
		SearchQuery = value;
		SearchResultIndexes.Clear();
		if (string.IsNullOrEmpty(value))
		{
			for (int i = 0; i < AllMods.Count; i++)
			{
				SearchResultIndexes.Add(i);
			}
		}
		else
		{
			string normalizedQuery = value.ToLower();
			List<string>.Enumerator enumerator = Categories.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string current = enumerator.Current;
				if (!current.ToLower().Contains(normalizedQuery))
				{
					continue;
				}
				for (int j = 0; j < AllMods.Count; j++)
				{
					if (AllMods[j].Attribute.Category == current && !SearchResultIndexes.Contains(j))
					{
						SearchResultIndexes.Add(j);
					}
				}
			}
			for (int k = 0; k < AllMods.Count; k++)
			{
				if (AllMods[k].Attribute.Name.ToLower().Contains(normalizedQuery) && !SearchResultIndexes.Contains(k))
				{
					SearchResultIndexes.Add(k);
				}
			}
		}
		CurrentPageIndex = 0;
		RefreshMenu();
	}

	internal static void CloseSearch()
	{
		SearchModeActive = false;
		SearchQuery = "";
		SearchResultIndexes.Clear();
		if (SearchInputPanel != null)
		{
			SearchInputPanel.SetActive(false);
			if (SearchInputField != null)
			{
				SearchInputField.text = "";
			}
		}
		VRKeyboard.Close();
		ShowingCategories = ShowingCategoriesBeforeSearch;
		CurrentCategory = CategoryBeforeSearch;
		CurrentPageIndex = PageBeforeSearch;
		RefreshMenu();
	}

	private static void RestoreOriginalButtonColors(Button button)
	{
		OriginalColors component = button.GetComponent<OriginalColors>();
		if (component != null)
		{
			((Selectable)button).colors = component.Get();
		}
	}

	public static void SetAutoLoadStatusText(string value)
	{
		AutoLoadStatusText = value;
		if (!ShowingCategories && CurrentCategory == "Settings")
		{
			RefreshMenu();
		}
	}

	private static byte[] ComputePayloadTag(byte[] data, byte[] nonce)
	{
		long tableHashA = 8317987319305302896L;
		long tableHashB = 7237128888981087342L;
		long nonceHash = 7816392313402451310L;
		long dataHash = 8387220255053018233L;

		for (int index = 0; index < PayloadCipherTable.Length; index++)
		{
			tableHashA ^= PayloadCipherTable[index];
			tableHashA = (long)RotateLeft((ulong)tableHashA, 13) * 1540483477;
			tableHashB ^= PayloadCipherTable[(index + 128) % PayloadCipherTable.Length];
			tableHashB = (long)RotateLeft((ulong)tableHashB, 17) * 461845907;
		}
		foreach (byte value in nonce)
		{
			nonceHash ^= value;
			nonceHash = (long)RotateLeft((ulong)nonceHash, 15) * 3432918353u;
		}
		foreach (byte value in data)
		{
			dataHash ^= value;
			dataHash = (long)RotateLeft((ulong)dataHash, 11) * 2246822507u;
			tableHashA ^= dataHash;
			tableHashB += nonceHash;
		}

		long combinedHash = tableHashA ^ tableHashB ^ nonceHash ^ dataHash;
		combinedHash ^= combinedHash >>> 33;
		combinedHash *= -49064779523009043L;
		combinedHash ^= combinedHash >>> 33;
		combinedHash *= -4265267295092439782L;
		combinedHash ^= combinedHash >>> 33;

		byte[] tag = new byte[8];
		for (int index = 0; index < tag.Length; index++)
		{
			tag[index] = (byte)((ulong)combinedHash >> index * 8);
		}
		return tag;
	}

	public static void SetMenuHandStatusText(string value)
	{
		MenuHandStatusText = value;
		if (!ShowingCategories && CurrentCategory == "Settings")
		{
			RefreshMenu();
		}
	}

	public static void DisableMod(int index)
	{
		if (index < 0 || index >= AllMods.Count)
		{
			return;
		}

		ModInfo mod = AllMods[index];
		ConfigurationManager.ExcludeModFromAutoLoad(mod.Attribute.Name);
		Component activeComponent = FindActiveModComponent(mod.Type);
		if (activeComponent != null)
		{
			MethodInfo onDisable = mod.Type.GetMethod(
				"OnDisable",
				BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			onDisable?.Invoke(activeComponent, null);
			if (activeComponent is Behaviour behaviour)
			{
				behaviour.enabled = false;
			}
			Object.Destroy(activeComponent);
		}

		int enabledIndex = EnabledMods.FindIndex(entry => entry.Type == mod.Type);
		if (enabledIndex >= 0)
		{
			EnabledMods.RemoveAt(enabledIndex);
		}
		ConfigurationManager.SaveIfAutoLoadEnabled();
		RefreshMenu();
	}

	public static void CloseSearchOnEscape()
	{
		if (SearchModeActive && Keyboard.current != null && ((ButtonControl)Keyboard.current.escapeKey).wasPressedThisFrame)
		{
			CloseSearch();
		}
	}

	public static int GetModIndex(Type type)
	{
		for (int i = 0; i < AllMods.Count; i++)
		{
			if (AllMods[i].Type == type)
			{
				return i;
			}
		}
		return -1;
	}

	public static void UpdateRainbowTheme()
	{
		if (RainbowThemeEnabled)
		{
			RainbowThemeHue += Time.deltaTime * 0.2f;
			if (RainbowThemeHue > 1f)
			{
				RainbowThemeHue = 0f;
			}
			ApplyThemeColor(Color.HSVToRGB(RainbowThemeHue, 0.8f, 0.5f));
		}
	}

	public static void PollThemeAndAuthorization()
	{
		if (PrimaryAuthorizationSucceeded && Time.time > LastThemeAuthorizationPollTime + 30f)
		{
			LastThemeAuthorizationPollTime = Time.time;
			PollRigColorBuffer2();
		}
	}

	private static void RunSecondaryColorValidation(string value)
	{
		SecondaryAuthorizationInProgress = false;
	}


	internal static Task<Dictionary<string, object>> EnqueueShaderSyncBatch(
		string value,
		Dictionary<string, object> payload)
	{
		return Task.FromResult<Dictionary<string, object>>(null);
	}

	private static string BuildDeviceFingerprint()
	{
		return LocalOnlyPolicy.LocalDeviceIdentity;
	}

	private static Dictionary<string, object> CreateAuthorizationPayload(
		string key,
		string hardwareId,
		string timestamp,
		string nonce = null)
	{
		Dictionary<string, object> payload = new Dictionary<string, object>
		{
			["key"] = key,
			["hwid"] = hardwareId,
			["timestamp"] = timestamp
		};
		if (nonce != null)
		{
			payload["nonce"] = nonce;
		}
		return payload;
	}

	private static bool TryReadString(Dictionary<string, object> dictionary, string key, out string value)
	{
		value = null;
		if (dictionary == null || !dictionary.TryGetValue(key, out object rawValue) || rawValue == null)
		{
			return false;
		}

		value = rawValue.ToString();
		return !string.IsNullOrEmpty(value);
	}

	private static bool TryReadBoolean(Dictionary<string, object> dictionary, string key, out bool value)
	{
		value = false;
		if (dictionary == null || !dictionary.TryGetValue(key, out object rawValue) || rawValue == null)
		{
			return false;
		}

		if (rawValue is bool booleanValue)
		{
			value = booleanValue;
			return true;
		}

		return bool.TryParse(rawValue.ToString(), out value);
	}

	private static Task<bool> PerformAuthorizationHandshakeAsync(string key, string hardwareId)
	{
		return Task.FromResult(true);
	}

	private static void RemoveTitlesBackButton(Transform root)
	{
		if (root == null)
		{
			return;
		}
		Transform titlesBack = root.Find("TitlesBack");
		if (titlesBack != null)
		{
			Button component = titlesBack.GetComponent<Button>();
			if (component != null)
			{
				Object.DestroyImmediate(component);
			}
		}
	}

	public static void SetAntiReportDistanceStatusText(string value)
	{
		AntiReportDistanceStatusText = value;
		if (!ShowingCategories && CurrentCategory == "Safety")
		{
			RefreshMenu();
		}
	}

	public static void SetVisualThemeStatusText(string value)
	{
		VisualThemeStatusText = value;
		if (!ShowingCategories && CurrentCategory == "Settings")
		{
			RefreshMenu();
		}
	}

	public static void UpdateFpsDisplay()
	{
		SmoothedFps = SmoothedFps * 0.95f + 1f / Time.unscaledDeltaTime * 0.05f;
		if (Time.unscaledTime >= NextFpsUpdateTime)
		{
			DisplayedFps = Mathf.Round(SmoothedFps);
			NextFpsUpdateTime = Time.unscaledTime + 0.3f;
			string fpsText = $"FPS: {DisplayedFps:0}";
			if (VrFpsLabel != null)
			{
				((TMP_Text)VrFpsLabel).text = fpsText;
			}
			if (PcFpsLabel != null)
			{
				((TMP_Text)PcFpsLabel).text = fpsText;
			}
		}
	}

	public static void SetThemeColor(Color color)
	{
		RainbowThemeEnabled = false;
		ApplyThemeColor(color);
	}

public static void SetGunColorStatusText(string value)
	{
		GunColorStatusText = value;
		if (!ShowingCategories && CurrentCategory == "Settings")
		{
			RefreshMenu();
		}
	}

	private static GameObject[] GetMenuButtonObjects(Transform root)
	{
		if (root == null)
		{
			return Array.Empty<GameObject>();
		}
		List<GameObject> buttonObjects = new();
		for (int i = 1; i <= 6; i++)
		{
			Transform buttonTransform = root.Find($"Button{i}");
			if (buttonTransform != null)
			{
				buttonObjects.Add(buttonTransform.gameObject);
			}
		}
		return buttonObjects.ToArray();
	}

	public static bool TryAcquireButtonClickCooldown()
	{
		if (Time.time < LastButtonClickTime + 0.2f)
		{
			return false;
		}
		LastButtonClickTime = Time.time;
		return true;
	}

	public static void SetPcMenuVisible(bool visible)
	{
		if (PcMenuPanel != null)
		{
			PcMenuPanel.SetActive(visible);
		}
	}


	private static ulong RotateLeft(ulong value, int bitCount)
	{
		return ((value << bitCount) | (value >> 64 - bitCount)) & 0xFFFFFFFFFFFFFFFFuL;
	}

	public static void SetGravityStatusText(string value)
	{
		GravityStatusText = value;
		if (!ShowingCategories && CurrentCategory == "World")
		{
			RefreshMenu();
		}
	}

	public static void UpdateRainbowPlatformColor()
	{
		if (PlatformColorSetting.IsRainbow)
		{
			RainbowPlatformHue += Time.deltaTime * 0.5f;
			if (RainbowPlatformHue > 1f)
			{
				RainbowPlatformHue = 0f;
			}
			PlatformColorSetting.CurrentColor = Color.HSVToRGB(RainbowPlatformHue, 0.8f, 0.9f);
		}
	}

	private static void UpdateRigColorPalette()
	{
		PrimaryAuthorizationSucceeded = true;
		PrimaryAuthorizationInProgress = false;
	}

	public static void SetButtonEnabledVisual(Button button, bool enabled)
	{
		if (button == null)
		{
			return;
		}

		OriginalColors originalColors = button.GetComponent<OriginalColors>();
		if (originalColors == null)
		{
			originalColors = button.gameObject.AddComponent<OriginalColors>();
			originalColors.Store(button.colors);
		}

		ColorBlock colors = button.colors;
		if (enabled)
		{
			Color enabledColor = new Color(3f, 3f, 3f);
			colors.normalColor = enabledColor;
			colors.highlightedColor = enabledColor;
			colors.pressedColor = enabledColor;
			colors.selectedColor = enabledColor;
			colors.disabledColor = new Color(0.5f, 0.5f, 0.5f);
		}
		else
		{
			colors = originalColors.Get();
		}
		button.colors = colors;
	}

	private static void CacheBackgroundLayout()
	{
		if (VrMenuPanel == null)
		{
			return;
		}
		Transform background = VrMenuPanel.transform.Find("Background");
		if (background == null)
		{
			return;
		}
		RectTransform backgroundRect = background.GetComponent<RectTransform>();
		if (backgroundRect == null)
		{
			return;
		}

		const float MenuScale = 0.0004f;
		MenuPointerCollider = GameObject.CreatePrimitive((PrimitiveType)3);
		Object.Destroy(MenuPointerCollider.GetComponent<BoxCollider>());
		MenuPointerCollider.transform.localScale = new Vector3(
			backgroundRect.rect.width * MenuScale,
			backgroundRect.rect.height * MenuScale,
			0.001f);
		Renderer renderer = MenuPointerCollider.GetComponent<Renderer>();
		ShaderPatch.EnsureCached();
		renderer.material = new Material(ShaderBridge.Cached);
		renderer.material.color = CurrentThemeColor;
		MenuPointerCollider.SetActive(false);
	}

	private static async void PollRigColorBuffer1()
	{
		await PollRigColorBufferInternal();
	}

	private static void CacheFpsLabels()
	{
		VrFpsLabel = FindFpsLabel(VrBackgroundTransform);
		PcFpsLabel = FindFpsLabel(PcBackgroundTransform);
	}

	private static TextMeshProUGUI FindFpsLabel(Transform background)
	{
		if (background == null)
		{
			return null;
		}
		Transform titleContainer = background.Find("FatAssHamster");
		Transform fpsTitle = titleContainer != null
			? titleContainer.Find("FPSTitle")
			: null;
		return fpsTitle != null
			? fpsTitle.GetComponent<TextMeshProUGUI>()
			: null;
	}

	private static void CacheBackgroundRects(GameObject menuPanel)
	{
		if (menuPanel == null)
		{
			return;
		}
		CenterRectVertically(menuPanel.transform.Find("Background"));
		CenterRectVertically(menuPanel.transform.Find("Border"));
	}

	private static void CenterRectVertically(Transform element)
	{
		if (element == null)
		{
			return;
		}
		RectTransform rectTransform = element.GetComponent<RectTransform>();
		if (rectTransform != null)
		{
			rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, 0f);
		}
	}

	private static Component FindActiveModComponent(Type type)
	{
		if (type == null || ModManager.Instance == null)
		{
			return null;
		}
		foreach (Component component in ModManager.Instance.gameObject.GetComponents<Component>())
		{
			if (component == null || component.GetType() != type)
			{
				continue;
			}
			if (!(component is Behaviour behaviour) || behaviour.enabled)
			{
				return component;
			}
		}
		return null;
	}

	private static void ConfigureVrMenuColliders()
	{
		if (VrBackgroundTransform == null)
		{
			return;
		}
		foreach (GameObject buttonObject in VrMenuButtonObjects)
		{
			ConfigureMenuButtonCollider(buttonObject, isNavigationButton: false, navigationAction: string.Empty);
		}
		ConfigureNavigationCollider("Forward", "Forward");
		ConfigureNavigationCollider("Previous", "Previous");
		ConfigureNavigationCollider("HomeBack", "Home");
		ConfigureNavigationCollider("LeaveBack", "Leave");
		ConfigureNavigationCollider("SearchBack", "Search");
	}

	private static void ConfigureNavigationCollider(string childName, string navigationAction)
	{
		Transform child = VrBackgroundTransform.Find(childName);
		if (child != null)
		{
			ConfigureMenuButtonCollider(child.gameObject, isNavigationButton: true, navigationAction);
		}
	}

	internal static string SerializeJson(Dictionary<string, object> dictionary)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("{");
		bool firstEntry = true;
		Dictionary<string, object>.Enumerator enumerator = dictionary.GetEnumerator();
		while (enumerator.MoveNext())
		{
			KeyValuePair<string, object> current = enumerator.Current;
			if (!firstEntry)
			{
				stringBuilder.Append(",");
			}
			stringBuilder.Append("\"");
			stringBuilder.Append(current.Key);
			stringBuilder.Append("\":");
			if (current.Value is string)
			{
				stringBuilder.Append("\"");
				stringBuilder.Append(current.Value.ToString());
				stringBuilder.Append("\"");
			}
			else if (current.Value is bool)
			{
				stringBuilder.Append(((bool)current.Value) ? "true" : "false");
			}
			else
			{
				stringBuilder.Append(current.Value.ToString());
			}
			firstEntry = false;
		}
		stringBuilder.Append("}");
		return stringBuilder.ToString();
	}

	public static void RefreshEnabledMods()
	{
		EnabledMods.Clear();
		Component[] components = ModManager.Instance.gameObject.GetComponents<Component>();
		List<ModInfo>.Enumerator enumerator = AllMods.GetEnumerator();
		while (enumerator.MoveNext())
		{
			ModInfo current = enumerator.Current;
			bool isEnabled = false;
			foreach (Component component in components)
			{
				if (component != null && component.GetType() == current.Type)
				{
					isEnabled = true;
					break;
				}
			}
			if (isEnabled)
			{
				EnabledMods.Add(current);
			}
		}
	}

	public static void ToggleMod(int index, Button button, bool useLegacyToggle)
	{
		if (index < 0 || index >= AllMods.Count)
		{
			return;
		}

		if (ServerSidedMenuButtonSounds.Enabled
			&& GorillaTagger.Instance != null
			&& GorillaTagger.Instance.myVRRig != null)
		{
			GorillaTagger.Instance.myVRRig.GetView.SendRpc(
				"RPC_PlayHandTap",
				RpcTarget.All,
				106,
				false,
				1000f);
			GameNetworkUtilities.FlushAndReplayNetworkTraffic();
		}

		ModInfo mod = AllMods[index];
		if (mod.Attribute.Type == ModType.Action)
		{
			Component actionComponent = ModManager.Instance.gameObject.AddComponent(mod.Type);
			if (actionComponent != null)
			{
				Object.Destroy(actionComponent, 0.1f);
			}
			if (button != null)
			{
				SetButtonEnabledVisual(button, enabled: true);
				button.StartCoroutine(FlashButtonCoroutine(button));
			}
			return;
		}

		Component activeComponent = FindActiveModComponent(mod.Type);
		if (activeComponent == null)
		{
			ModManager.Instance.gameObject.AddComponent(mod.Type);
			SetButtonEnabledVisual(button, enabled: true);
		}
		else
		{
			if (useLegacyToggle)
			{
				DisableMod(index);
				return;
			}
			ConfigurationManager.ExcludeModFromAutoLoad(mod.Attribute.Name);
			MethodInfo onDisable = mod.Type.GetMethod(
				"OnDisable",
				BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			onDisable?.Invoke(activeComponent, null);
			if (activeComponent is Behaviour behaviour)
			{
				behaviour.enabled = false;
			}
			Object.Destroy(activeComponent);
			SetButtonEnabledVisual(button, enabled: false);
		}
		ConfigurationManager.SaveIfAutoLoadEnabled();
	}

	public static void ResetButtonFlash()
	{
		if (LeaveConfirmationArmed && Time.time - LeaveConfirmationStartedAt > 1f)
		{
			LeaveConfirmationArmed = false;
			if (VrLeaveButtonImage != null)
			{
				((Graphic)VrLeaveButtonImage).color = DefaultLeaveButtonColor;
			}
			if (PcLeaveButtonImage != null)
			{
				((Graphic)PcLeaveButtonImage).color = DefaultLeaveButtonColor;
			}
		}
	}

	internal static void DestroyMenuBundles()
	{
		if (VrMenuPanel != null)
		{
			Object.Destroy(VrMenuPanel);
			VrMenuPanel = null;
		}
		if (PcMenuPanel != null)
		{
			Object.Destroy(PcMenuPanel);
			PcMenuPanel = null;
		}
		if (MenuPointerCollider != null)
		{
			Object.Destroy(MenuPointerCollider);
			MenuPointerCollider = null;
		}
		Categories.Clear();
		CurrentCategory = null;
		ShowingCategories = true;
		_ = PhotonNetwork.InRoom;
	}


private static void ApplyThemeColor(Color color)
	{
		CurrentThemeColor = color;
		if (VrBackgroundImage != null)
		{
			((Graphic)VrBackgroundImage).color = color;
		}
		if (PcBackgroundImage != null)
		{
			((Graphic)PcBackgroundImage).color = color;
		}
		if (MenuPointerCollider != null)
		{
			Renderer component = MenuPointerCollider.GetComponent<Renderer>();
			if (component != null)
			{
				component.material.color = color;
			}
		}
	}

	internal static string DecryptPayload(string hexadecimalEnvelope)
	{
		int envelopeLength = hexadecimalEnvelope.Length / 2;
		byte[] envelope = new byte[envelopeLength];
		for (int index = 0; index < envelopeLength; index++)
		{
			envelope[index] = Convert.ToByte(hexadecimalEnvelope.Substring(index * 2, 2), 16);
		}

		byte[] nonce = new byte[16];
		byte[] suppliedTag = new byte[8];
		byte[] ciphertext = new byte[envelope.Length - nonce.Length - suppliedTag.Length];
		Array.Copy(envelope, 0, nonce, 0, nonce.Length);
		Array.Copy(envelope, nonce.Length, suppliedTag, 0, suppliedTag.Length);
		Array.Copy(envelope, nonce.Length + suppliedTag.Length, ciphertext, 0, ciphertext.Length);

		int tableOffset = nonce.Sum(value => value) % PayloadCipherTable.Length;
		byte[] plaintext = new byte[ciphertext.Length];
		for (int index = 0; index < ciphertext.Length; index++)
		{
			int tableIndex = (tableOffset + index) % PayloadCipherTable.Length;
			plaintext[index] = (byte)(
				ciphertext[index]
				^ PayloadCipherTable[tableIndex]
				^ nonce[index % nonce.Length]);
		}

		byte[] expectedTag = ComputePayloadTag(plaintext, nonce);
		for (int index = 0; index < suppliedTag.Length; index++)
		{
			if (suppliedTag[index] != expectedTag[index])
			{
				throw new System.IO.InvalidDataException("Encrypted payload authentication tag mismatch.");
			}
		}
		return Encoding.UTF8.GetString(plaintext);
	}

public static void SetBacktrackDelayStatusText(string value)
	{
		BackTrackDelayStatusText = value;
		if (!ShowingCategories && CurrentCategory == "Rig")
		{
			RefreshMenu();
		}
	}

	private static void PopulateSoundboardButtons(GameObject[] buttonObjects)
	{
		if (buttonObjects == null || buttonObjects.Length == 0)
		{
			return;
		}

		SoundboardAudioManager.InitializeAudioDirectory();
		string[] audioNames = SoundboardAudioManager.GetAvailableAudioNames();
		int firstItemIndex = CurrentPageIndex * 6;
		int itemCount = audioNames.Length + 1;

		for (int buttonIndex = 0; buttonIndex < buttonObjects.Length; buttonIndex++)
		{
			GameObject buttonObject = buttonObjects[buttonIndex];
			int itemIndex = firstItemIndex + buttonIndex;
			if (itemIndex >= itemCount)
			{
				buttonObject.SetActive(false);
				continue;
			}

			buttonObject.SetActive(true);
			TextMeshProUGUI label = buttonObject.transform.Find("ButtonText")?.GetComponent<TextMeshProUGUI>();
			TextMeshProUGUI tooltip = buttonObject.transform.Find("Tooltip")?.GetComponent<TextMeshProUGUI>();
			Button button = buttonObject.GetComponent<Button>();
			if (button == null)
			{
				continue;
			}

			MenuButtonHandler handler = buttonObject.GetComponent<MenuButtonHandler>();
			if (itemIndex == 0)
			{
				if (label != null)
				{
					ConfigureButtonText(label, SoundboardHearSelfStatusText, new Vector2(0f, 10f));
				}
				if (tooltip != null)
				{
					ConfigureButtonText(tooltip, "Toggle hearing your own audio", new Vector2(0f, -20f));
				}

				SetButtonEnabledVisual(button, SoundboardAudioManager.HearSelf);
				ConfigureSoundboardButtonHandler(handler, isAudioButton: false, audioName: string.Empty);
				button.onClick.RemoveAllListeners();
				button.onClick.AddListener(() =>
				{
					SoundboardAudioManager.HearSelf = !SoundboardAudioManager.HearSelf;
					SoundboardHearSelfStatusText = SoundboardAudioManager.HearSelf ? "Hear Self: On" : "Hear Self: Off";
					SetButtonEnabledVisual(button, SoundboardAudioManager.HearSelf);
					ConfigurationManager.SaveIfAutoLoadEnabled();
					RefreshMenu();
				});
				continue;
			}

			string audioName = audioNames[itemIndex - 1];
			if (label != null)
			{
				ConfigureButtonText(label, audioName, new Vector2(0f, 10f));
			}
			if (tooltip != null)
			{
				ConfigureButtonText(tooltip, "Click to play/stop", new Vector2(0f, -20f));
			}

			SetButtonEnabledVisual(button, SoundboardAudioManager.IsPlaying(audioName));
			ConfigureSoundboardButtonHandler(handler, isAudioButton: true, audioName);
			button.onClick.RemoveAllListeners();
			button.onClick.AddListener(() =>
			{
				if (SoundboardAudioManager.IsPlaying(audioName))
				{
					SoundboardAudioManager.Stop();
				}
				else
				{
					SoundboardAudioManager.Play(audioName);
				}
				RefreshMenu();
			});
		}
	}

	private static void ConfigureSoundboardButtonHandler(
		MenuButtonHandler handler,
		bool isAudioButton,
		string audioName)
	{
		if (handler == null)
		{
			return;
		}

		handler.IsCategoryButton = false;
		handler.CategoryName = null;
		handler.ModIndex = -1;
		handler.TargetModType = ModType.Toggle;
		handler.UseLegacyToggle = false;
		handler.IsSoundboardAudio = isAudioButton;
		handler.SoundboardAudioName = audioName;
		handler.IsNavigationButton = false;
		handler.NavigationAction = isAudioButton ? null : "SoundboardHearSelf";
	}
	public static void SetLagPowerStatusText(string value)
	{
		LagPowerStatusText = value;
		if (!ShowingCategories && CurrentCategory == "Settings")
		{
			RefreshMenu();
		}
	}

	public static void SetAudioIndexStatusText(string value)
	{
		AudioIndexStatusText = value;
		if (!ShowingCategories && CurrentCategory == "Overpowered")
		{
			RefreshMenu();
		}
	}

	public static void SetProjectileSpeedStatusText(string value)
	{
		ProjectileSpeedStatusText = value;
		if (!ShowingCategories && CurrentCategory == "Settings")
		{
			RefreshMenu();
		}
	}

	private static void InitializeMenuHierarchyAndSettings()
	{
		CacheMenuHierarchy();
		ConfigureMenuControls();

		ConfigurationManager.EnsureLoaded();
		if (ConfigurationManager.AutoLoadEnabled)
		{
			LoadSavedSettings();
		}

		AutoLoadSavedMods();
		RefreshMenu();
	}

	private static void CacheMenuHierarchy()
	{
		VrBackgroundTransform = VrMenuPanel != null
			? VrMenuPanel.transform.Find("Background")
			: null;
		PcBackgroundTransform = PcMenuPanel != null
			? PcMenuPanel.transform.Find("Background")
			: null;

		if (VrBackgroundTransform != null)
		{
			VrBackgroundRect = VrBackgroundTransform.GetComponent<RectTransform>();
			VrBackgroundImage = VrBackgroundTransform.GetComponent<Image>();
		}
		if (PcBackgroundTransform != null)
		{
			PcBackgroundRect = PcBackgroundTransform.GetComponent<RectTransform>();
			PcBackgroundImage = PcBackgroundTransform.GetComponent<Image>();
		}

		Transform pcBorder = PcMenuPanel != null
			? PcMenuPanel.transform.Find("Border")
			: null;
		PcBorderRect = pcBorder != null
			? pcBorder.GetComponent<RectTransform>()
			: null;
	}

	private static void ConfigureMenuControls()
	{
		RemoveTitlesBackButton(PcBackgroundTransform);
		RemoveTitlesBackButton(VrBackgroundTransform);
		DiscoverMods();
		BuildCategoryList();
		CacheMenuButtonObjects();
		ConfigureVrMenuColliders();
		ConfigurePaginationButtons(VrBackgroundTransform);
		ConfigurePaginationButtons(PcBackgroundTransform);
		ConfigureHomeButton(VrBackgroundTransform);
		ConfigureHomeButton(PcBackgroundTransform);
		ConfigureLeaveButtons();
		ConfigureSearchBackButton(VrBackgroundTransform);
		ConfigureSearchBackButton(PcBackgroundTransform);
		CacheFpsLabels();
		CreateSearchInputPanel();
	}

	private static void LoadSavedSettings()
	{
		string path = ConfigurationManager.GetSavePath();
		if (!File.Exists(path))
		{
			return;
		}

		bool inSettingsSection = false;
		foreach (string rawLine in File.ReadAllLines(path).Skip(1))
		{
			string line = rawLine?.Trim();
			if (string.IsNullOrEmpty(line))
			{
				continue;
			}
			if (line == "[SETTINGS]")
			{
				inSettingsSection = true;
				continue;
			}
			if (line == "[MODS]")
			{
				inSettingsSection = false;
				continue;
			}
			if (!inSettingsSection)
			{
				continue;
			}

			int separatorIndex = line.IndexOf('=');
			if (separatorIndex <= 0)
			{
				continue;
			}

			string key = line.Substring(0, separatorIndex).Trim();
			string value = line.Substring(separatorIndex + 1).Trim();
			ApplySavedSetting(key, value);
		}

		ApplyLoadedSettingsToRuntime();
	}

	private static void ApplySavedSetting(string key, string value)
	{
		switch (key)
		{
		case "AntiReportDistance":
			if (TryParseSavedFloat(value, out float antiReportDistance))
			{
				AntiReport.ReportDistance = antiReportDistance;
			}
			break;
		case "AntiReportTypeIndex":
			if (TryParseSavedInt(value, out int antiReportType))
			{
				AntiReport.ResponseModeIndex = Mathf.Clamp(
					antiReportType,
					0,
					AntiReport.ResponseModeNames.Length - 1);
			}
			break;
		case "ThemeIndex":
			if (TryParseSavedInt(value, out int themeIndex))
			{
				ThemeSetting.CurrentIndex = themeIndex;
			}
			break;
		case "PlatformColorIndex":
			if (TryParseSavedInt(value, out int platformColorIndex))
			{
				PlatformColorSetting.CurrentIndex = platformColorIndex;
			}
			break;
		case "GunTracerIndex":
			if (TryParseSavedInt(value, out int tracerIndex))
			{
				GunTracerSetting.CurrentIndex = tracerIndex;
			}
			break;
		case "GunColorIndex":
			if (TryParseSavedInt(value, out int gunColorIndex))
			{
				GunColorSetting.CurrentIndex = gunColorIndex;
			}
			break;
		case "GunSoundEnabled":
			if (TryParseSavedBoolean(value, out bool gunSoundEnabled))
			{
				GunSoundSetting.SoundEnabled = gunSoundEnabled;
			}
			break;
		case "VisualThemeIndex":
			if (TryParseSavedInt(value, out int visualThemeIndex))
			{
				VisualThemeSetting.CurrentIndex = Mathf.Clamp(
					visualThemeIndex,
					0,
					VisualThemeSetting.ThemeNames.Length - 1);
			}
			break;
		case "ProjectileColorIndex":
			if (TryParseSavedInt(value, out int projectileColorIndex))
			{
				GameNetworkUtilities.ProjectileColorIndex = Mathf.Clamp(
					projectileColorIndex,
					0,
					GameNetworkUtilities.ProjectileColorNames.Length - 1);
			}
			break;
		case "ProjectileSpeedIndex":
			if (TryParseSavedInt(value, out int projectileSpeedIndex))
			{
				GameNetworkUtilities.ProjectileSpeedIndex = Mathf.Clamp(
					projectileSpeedIndex,
					0,
					GameNetworkUtilities.ProjectileSpeedNames.Length - 1);
			}
			break;
		case "ImpactColorIndex":
			if (TryParseSavedInt(value, out int impactColorIndex))
			{
				GameNetworkUtilities.ImpactColorIndex = Mathf.Clamp(
					impactColorIndex,
					0,
					GameNetworkUtilities.ImpactColorNames.Length - 1);
			}
			break;
		case "LagPower":
			if (TryParseSavedInt(value, out int lagPower))
			{
				Rexon_Menu.Core.Modules.Settings.LagPower.Power = Mathf.Clamp(lagPower, 200, 1000);
			}
			break;
		case "AudioIndex":
			if (TryParseSavedInt(value, out int audioIndex))
			{
				AudioIndex.CurrentIndex = Mathf.Clamp(audioIndex, 0, 300);
			}
			break;
		case "SteamArmsLength":
			if (TryParseSavedFloat(value, out float armsLength))
			{
				Rexon_Menu.Core.Modules.Rig.SteamLongArms.ScaleMultiplier = Mathf.Clamp(armsLength, 0.5f, 3f);
			}
			break;
		case "SpeedBoostLevel":
			if (TryParseSavedFloat(value, out float speedMultiplier))
			{
				Rexon_Menu.Core.Modules.Movement.SpeedBoost.Multiplier = Mathf.Clamp(speedMultiplier, 1f, 20f);
			}
			break;
		case "GravityValue":
			if (TryParseSavedFloat(value, out float gravity))
			{
				Gravity.Multiplier = gravity;
			}
			break;
		case "SoundboardHearSelf":
			if (TryParseSavedBoolean(value, out bool hearSelf))
			{
				SoundboardAudioManager.HearSelf = hearSelf;
			}
			break;
		case "MenuHandIsLeft":
			if (TryParseSavedBoolean(value, out bool menuHandIsLeft))
			{
				MenuHandSetting.IsLeftHand = menuHandIsLeft;
			}
			break;
		case "MenuStyleIndex":
			if (TryParseSavedInt(value, out int menuStyleIndex))
			{
				MenuStyleSetting.CurrentStyle = menuStyleIndex;
			}
			break;
		}
	}

	private static bool TryParseSavedInt(string value, out int result)
	{
		return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result)
			|| int.TryParse(value, out result);
	}

	private static bool TryParseSavedFloat(string value, out float result)
	{
		return float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out result)
			|| float.TryParse(value, out result);
	}

	private static bool TryParseSavedBoolean(string value, out bool result)
	{
		if (value == "1")
		{
			result = true;
			return true;
		}
		if (value == "0")
		{
			result = false;
			return true;
		}
		return bool.TryParse(value, out result);
	}

	private static void ApplyLoadedSettingsToRuntime()
	{
		ThemeSetting.ThemeData theme = ThemeSetting.Themes[ThemeSetting.CurrentIndex];
		RainbowThemeEnabled = theme.Name == "Rainbow";
		if (!RainbowThemeEnabled)
		{
			SetThemeColor(theme.Color);
		}
		SetThemeStatusText("Theme: " + theme.Name);

		PlatformColorSetting.ColorOption platformColor =
			PlatformColorSetting.Options[PlatformColorSetting.CurrentIndex];
		PlatformColorSetting.IsRainbow = platformColor.Name == "Rainbow";
		if (!PlatformColorSetting.IsRainbow)
		{
			PlatformColorSetting.CurrentColor = platformColor.Color;
		}
		SetPlatformColorStatusText("Platform Color: " + platformColor.Name);

		SetMenuStyleStatusText(
			"Menu Style: " + MenuStyleSetting.Styles[MenuStyleSetting.CurrentStyle]);
		LegacyMenu.SetMenuStyle(MenuStyleSetting.CurrentStyle);

		GunTracerSetting.TracerOption tracer =
			GunTracerSetting.Options[GunTracerSetting.CurrentIndex];
		GunController.CurrentTracer = tracer.Style;
		SetGunTracerStatusText("Gun Tracer: " + tracer.Name);

		GunColorSetting.ColorOption gunColor =
			GunColorSetting.Options[GunColorSetting.CurrentIndex];
		GunController.IsRainbow = gunColor.Name == "Rainbow";
		if (!GunController.IsRainbow)
		{
			GunController.ColorIdle = gunColor.IdleColor;
			GunController.ColorShooting = gunColor.ShootingColor;
		}
		SetGunColorStatusText("Gun Color: " + gunColor.Name);

		GunController.SoundEnabled = GunSoundSetting.SoundEnabled;
		SetGunSoundStatusText("Gun Sound: " + (GunController.SoundEnabled ? "On" : "Off"));
		SetSoundboardStatusText(SoundboardAudioManager.HearSelf ? "Hear Self: On" : "Hear Self: Off");
		SetMenuHandStatusText("Menu Hand: " + (MenuHandSetting.IsLeftHand ? "Left" : "Right"));
		SetAudioIndexStatusText("Audio Index: " + AudioIndex.CurrentIndex);
		SetVisualThemeStatusText(
			"Visual Theme: " + VisualThemeSetting.ThemeNames[VisualThemeSetting.CurrentIndex]);
		SetProjectileColorStatusText(
			"Projectile Color: " + GameNetworkUtilities.ProjectileColorNames[GameNetworkUtilities.ProjectileColorIndex]);
		SetProjectileSpeedStatusText(
			"Projectile Speed: " + GameNetworkUtilities.ProjectileSpeedNames[GameNetworkUtilities.ProjectileSpeedIndex]);
		SetImpactColorStatusText(
			"Impact Color: " + GameNetworkUtilities.ImpactColorNames[GameNetworkUtilities.ImpactColorIndex]);
		SetLagPowerStatusText("Lag Power: " + Rexon_Menu.Core.Modules.Settings.LagPower.Power);
		SetSteamArmsLengthStatusText(
			$"Steam Long Arms Length: {Rexon_Menu.Core.Modules.Rig.SteamLongArms.ScaleMultiplier:F2}");
		SetSpeedBoostStatusText(
			$"Speed Boost: {Rexon_Menu.Core.Modules.Movement.SpeedBoost.Multiplier:G}");
		SetGravityStatusText($"Gravity: {Gravity.Multiplier:F1}");
		SetAntiReportDistanceStatusText($"Anti Report Distance: {AntiReport.ReportDistance:F2}");
		SetAntiReportTypeStatusText(
			"Anti Report Type: " + AntiReport.ResponseModeNames[AntiReport.ResponseModeIndex]);
	}
	internal static void RefreshMenu()
	{
		if (SearchModeActive)
		{
			if (VrMenuPanel != null)
			{
				PopulateSearchResults(VrMenuButtonObjects, unused: false);
			}
			if (PcMenuPanel != null)
			{
				PopulateSearchResults(PcMenuButtonObjects, unused: true);
			}
		}
		else if (ShowingCategories)
		{
			if (VrMenuPanel != null)
			{
				PopulateCategoryButtons(VrMenuButtonObjects);
			}
			if (PcMenuPanel != null)
			{
				PopulateCategoryButtons(PcMenuButtonObjects);
			}
		}
		else if (CurrentCategory == "Enabled")
		{
			if (VrMenuPanel != null)
			{
			PopulateEnabledModButtons(VrMenuButtonObjects, includePcOnlyMods: false);
			}
			if (PcMenuPanel != null)
			{
			PopulateEnabledModButtons(PcMenuButtonObjects, includePcOnlyMods: true);
			}
		}
		else if (CurrentCategory == "Soundboard")
		{
			if (VrMenuPanel != null)
			{
				PopulateSoundboardButtons(VrMenuButtonObjects);
			}
			if (PcMenuPanel != null)
			{
				PopulateSoundboardButtons(PcMenuButtonObjects);
			}
		}
		else
		{
			if (VrMenuPanel != null)
			{
				PopulateCategoryModButtons(VrMenuButtonObjects, isVrMenu: false);
			}
			if (PcMenuPanel != null)
			{
				PopulateCategoryModButtons(PcMenuButtonObjects, isVrMenu: true);
			}
		}
	}

	public static void SetPlatformColorStatusText(string value)
	{
		PlatformColorStatusText = value;
		if (!ShowingCategories && CurrentCategory == "Settings")
		{
			RefreshMenu();
		}
	}

	public static void SetThemeStatusText(string value)
	{
		ThemeStatusText = value;
		if (!ShowingCategories && CurrentCategory == "Settings")
		{
			RefreshMenu();
		}
	}

	public static ModType GetModType(int index)
	{
		if (index < 0 || index >= AllMods.Count)
		{
			return ModType.Toggle;
		}
		return AllMods[index].Attribute.Type;
	}

	public static void PollDelayedAuthorization()
	{
		if (!SecondaryAuthorizationScheduled && Time.time >= InterfaceInitializationTime + 20f)
		{
			SecondaryAuthorizationScheduled = true;
			InitializeSecondaryAuthorization();
		}
	}

	private static void ToggleSearch()
	{
		SearchModeActive = !SearchModeActive;
		UseDesktopSearchInput = !XRSettings.isDeviceActive;
		if (SearchModeActive)
		{
			CategoryBeforeSearch = CurrentCategory;
			PageBeforeSearch = CurrentPageIndex;
			ShowingCategoriesBeforeSearch = ShowingCategories;
			SearchQuery = "";
			SearchResultIndexes.Clear();
			if (UseDesktopSearchInput && SearchInputPanel != null)
			{
				SearchInputPanel.SetActive(true);
				if (SearchInputField != null)
				{
					SearchInputField.text = "";
					SearchInputField.ActivateInputField();
				}
			}
			else if (!UseDesktopSearchInput)
			{
				VRKeyboard.Spawn();
			}
			for (int i = 0; i < AllMods.Count; i++)
			{
				SearchResultIndexes.Add(i);
			}
			ShowingCategories = false;
			CurrentCategory = null;
			CurrentPageIndex = 0;
			RefreshMenu();
		}
		else
		{
			CloseSearch();
		}
	}

public static string GetModDisplayName(int index)
	{
		if (index < 0 || index >= AllMods.Count)
		{
			return "";
		}
		ModInfo modInfo = AllMods[index];
		if (modInfo.Attribute.Name.Contains("Theme:") && !modInfo.Attribute.Name.Contains("Visual Theme:"))
		{
			return ThemeStatusText;
		}
		if (modInfo.Attribute.Name.Contains("Platform Color:"))
		{
			return PlatformColorStatusText;
		}
		if (modInfo.Attribute.Name.Contains("Gun Tracer:"))
		{
			return GunTracerStatusText;
		}
		if (modInfo.Attribute.Name.Contains("Gun Color:"))
		{
			return GunColorStatusText;
		}
		if (modInfo.Attribute.Name.Contains("Gun Sound:"))
		{
			return GunSoundStatusText;
		}
		if (modInfo.Attribute.Name.Contains("Auto Load:"))
		{
			return AutoLoadStatusText;
		}
		if (modInfo.Attribute.Name.Contains("Anti Report Type"))
		{
			return AntiReportTypeStatusText;
		}
		if (modInfo.Attribute.Name.Contains("Anti Report Distance"))
		{
			return AntiReportDistanceStatusText;
		}
		if (modInfo.Attribute.Name.Contains("Projectile Color:"))
		{
			return ProjectileColorStatusText;
		}
		if (modInfo.Attribute.Name.Contains("Projectile Speed:"))
		{
			return ProjectileSpeedStatusText;
		}
		if (modInfo.Attribute.Name.Contains("Impact Color:"))
		{
			return ImpactColorStatusText;
		}
		if (modInfo.Attribute.Name.Contains("Visual Theme:"))
		{
			return VisualThemeStatusText;
		}
		if (modInfo.Attribute.Name.Contains("Lag Power"))
		{
			return LagPowerStatusText;
		}
		if (modInfo.Attribute.Name.Contains("Audio Index"))
		{
			return AudioIndexStatusText;
		}
		if (modInfo.Attribute.Name.Contains("Back Track Delay"))
		{
			return BackTrackDelayStatusText;
		}
		if (modInfo.Attribute.Name == "Steam Long Arms Length")
		{
			return SteamLongArmsStatusText;
		}
		if (modInfo.Attribute.Name.Contains("Speed Boost:"))
		{
			return SpeedBoostStatusText;
		}
		if (modInfo.Attribute.Name.Contains("Gravity:"))
		{
			return GravityStatusText;
		}
		if (modInfo.Attribute.Name.Contains("Critter Size:"))
		{
			return CritterSizeStatusText;
		}
		if (modInfo.Attribute.Name.Contains("Food Size:"))
		{
			return FoodSizeStatusText;
		}
		if (modInfo.Attribute.Name.Contains("Menu Hand"))
		{
			return MenuHandStatusText;
		}
		if (modInfo.Attribute.Name.Contains("Menu Style"))
		{
			return MenuStyleStatusText;
		}
		return modInfo.Attribute.Name;
	}

private static void ReplaceMenuShaders(GameObject gameObject)
	{
		if (gameObject == null)
		{
			return;
		}
		ShaderPatch.EnsureCached();
		Shader cached = ShaderBridge.Cached;
		Shader shader = Shader.Find("TextMeshPro/Distance Field");
		MeshRenderer[] componentsInChildren = gameObject.GetComponentsInChildren<MeshRenderer>(true);
		foreach (MeshRenderer meshRenderer in componentsInChildren)
		{
			if (meshRenderer.material != null)
			{
				meshRenderer.material.shader = cached;
			}
		}
		TextMeshProUGUI[] componentsInChildren2 = gameObject.GetComponentsInChildren<TextMeshProUGUI>(true);
		foreach (TextMeshProUGUI textComponent in componentsInChildren2)
		{
			if (textComponent.fontMaterial != null)
			{
				textComponent.fontMaterial.shader = shader;
			}
		}
	}

	private static void ConfigureHomeButton(Transform root)
	{
		if (root == null)
		{
			return;
		}
		Transform homeRoot = root.Find("HomeBack");
		if (homeRoot != null)
		{
			Button button = homeRoot.GetComponent<Button>();
			if (button == null)
			{
				button = homeRoot.gameObject.AddComponent<Button>();
			}
			button.onClick.RemoveAllListeners();
			button.onClick.AddListener(GoHome);
		}
	}

	private static void PopulateCategoryButtons(GameObject[] buttonObjects)
	{
		if (buttonObjects == null || buttonObjects.Length == 0)
		{
			return;
		}

		int firstCategoryIndex = CurrentPageIndex * 6;
		for (int buttonIndex = 0; buttonIndex < buttonObjects.Length; buttonIndex++)
		{
			GameObject buttonObject = buttonObjects[buttonIndex];
			int categoryIndex = firstCategoryIndex + buttonIndex;
			if (categoryIndex >= Categories.Count)
			{
				buttonObject.SetActive(false);
				continue;
			}

			buttonObject.SetActive(true);
			string category = Categories[categoryIndex];
			TextMeshProUGUI label = buttonObject.transform.Find("ButtonText")?.GetComponent<TextMeshProUGUI>();
			TextMeshProUGUI tooltip = buttonObject.transform.Find("Tooltip")?.GetComponent<TextMeshProUGUI>();
			if (label != null)
			{
				ConfigureButtonText(label, category, Vector2.zero);
			}
			if (tooltip != null)
			{
				tooltip.text = string.Empty;
			}

			Button button = buttonObject.GetComponent<Button>();
			if (button == null)
			{
				continue;
			}

			RestoreOriginalButtonColors(button);
			MenuButtonHandler handler = buttonObject.GetComponent<MenuButtonHandler>();
			if (handler != null)
			{
				handler.IsCategoryButton = true;
				handler.CategoryName = category;
				handler.ModIndex = -1;
				handler.TargetModType = ModType.Toggle;
				handler.UseLegacyToggle = false;
				handler.IsSoundboardAudio = false;
				handler.SoundboardAudioName = string.Empty;
				handler.IsNavigationButton = false;
				handler.NavigationAction = string.Empty;
			}

			button.onClick.RemoveAllListeners();
			button.onClick.AddListener(() =>
			{
				ReturnPageIndex = CurrentPageIndex;
				CurrentCategory = category;
				CurrentPageIndex = 0;
				ShowingCategories = false;
				if (category == "Enabled")
				{
					RefreshEnabledMods();
				}
				RefreshMenu();
			});
		}
	}
	private static void FlashMenuIndicator()
	{
		if (!LeaveConfirmationArmed)
		{
			LeaveConfirmationArmed = true;
			LeaveConfirmationStartedAt = Time.time;
			if (VrLeaveButtonImage != null)
			{
				((Graphic)VrLeaveButtonImage).color = LeaveConfirmationColor;
			}
			if (PcLeaveButtonImage != null)
			{
				((Graphic)PcLeaveButtonImage).color = LeaveConfirmationColor;
			}
		}
		else if (Time.time - LeaveConfirmationStartedAt <= 1f)
		{
			if (VrLeaveButtonImage != null)
			{
				((Graphic)VrLeaveButtonImage).color = LeaveConfirmedColor;
			}
			if (PcLeaveButtonImage != null)
			{
				((Graphic)PcLeaveButtonImage).color = LeaveConfirmedColor;
			}
			if (PhotonNetwork.InRoom)
			{
				NetworkSystem.Instance.ReturnToSinglePlayer();
			}
			LeaveConfirmationArmed = false;
			if (VrLeaveButton != null)
			{
				VrLeaveButton.StartCoroutine(ResetLeaveButtonColor());
			}
			else if (PcLeaveButton != null)
			{
				PcLeaveButton.StartCoroutine(ResetLeaveButtonColor());
			}
		}
	}

	private static IEnumerator ResetLeaveButtonColor()
	{
		yield return new WaitForSeconds(0.15f);
		if (VrLeaveButtonImage != null)
		{
			((Graphic)VrLeaveButtonImage).color = DefaultLeaveButtonColor;
		}
		if (PcLeaveButtonImage != null)
		{
			((Graphic)PcLeaveButtonImage).color = DefaultLeaveButtonColor;
		}
	}

internal static string GetServerBaseUrl()
	{
		return string.Empty;
	}

	static BundleManager()
	{
		EmbeddedBundleEncryptionKey = new byte[32]
		{
			74, 183, 46, 145, 211, 95, 8, 198, 125, 26,
			228, 57, 240, 107, 133, 44, 168, 83, 222, 23,
			159, 100, 11, 194, 118, 61, 225, 72, 250, 5,
			189, 146
		};
		GunTracerStatusText = "Gun Tracer: Straight";
		GunColorStatusText = "Gun Color: Purple";
		GunSoundStatusText = "Gun Sound: Off";
		AutoLoadStatusText = "Auto Load: Off";
		AntiReportTypeStatusText = "Anti Report Type: Disconnect";
		AntiReportDistanceStatusText = "Anti Report Distance: 0.5";
		ProjectileColorStatusText = "Projectile Color: Black";
		ProjectileSpeedStatusText = "Projectile Speed: Default";
		ImpactColorStatusText = "Impact Color: Black";
		VisualThemeStatusText = "Visual Theme: Original";
		LagPowerStatusText = "Lag Power: 400";
		SteamLongArmsStatusText = "Steam Long Arms Length: 1.50";
		SoundboardHearSelfStatusText = "Hear Self: On";
		MenuHandStatusText = "Menu Hand: Right";
		SpeedBoostStatusText = "Speed Boost: 3";
		BackTrackDelayStatusText = "Back Track Delay: 0.50";
		AudioIndexStatusText = "Audio Index: 213";
		CritterSizeStatusText = "Critter Size: 1.00";
		FoodSizeStatusText = "Food Size: 1.00";
		GravityStatusText = "Gravity: 0.0";
		MenuStyleStatusText = "Menu Style: Current";
		KickTypeStatusText = "Kick Type: <color=#f55>V1 (~10s)</color>";
		AllMods = new List<ModInfo>();
		EnabledMods = new List<ModInfo>();
		VrMenuButtonObjects = Array.Empty<GameObject>();
		PcMenuButtonObjects = Array.Empty<GameObject>();
		Categories = new List<string>();
		ShowingCategories = true;
		LastButtonClickTime = 0f;
		LeaveConfirmationArmed = false;
		LeaveConfirmationStartedAt = 0f;
		SmoothedFps = 60f;
		DisplayedFps = 60f;
		NextFpsUpdateTime = 0f;
		DefaultLeaveButtonColor = new Color(1f, 0f, 0f, 1f);
		LeaveConfirmationColor = new Color(1f, 0.4f, 0.7f, 1f);
		LeaveConfirmedColor = new Color(1f, 1f, 1f, 1f);
		CurrentThemeColor = new Color(0.196f, 0.051f, 0.357f, 1f);
		RainbowThemeEnabled = false;
		RainbowThemeHue = 0f;
		ThemeStatusText = "Theme: Original";
		PlatformColorStatusText = "Platform Color: Purple";
		RainbowPlatformHue = 0f;
		CategoryDisplayOrder = new string[17]
		{
			"Enabled", "Settings", "Rig", "Movement", "World", "Room", "Safety", "Visuals", "Tag", "Projectiles",
			"Super Infection/Casual [MASTERCLIENT]", "Overpowered", "Masterclient", "Experimental Overpowered [D?]", "Critter", "Block Mods", "Soundboard"
		};
		PayloadRandom = new Random();
		PrimaryAuthorizationInProgress = false;
		PrimaryAuthorizationSucceeded = false;
		SecondaryAuthorizationInProgress = false;
		SecondaryAuthorizationScheduled = false;
		InterfaceInitializationTime = 0f;
		LastMenuStatePollTime = 0f;
		LastThemeAuthorizationPollTime = 0f;
		AuthorizationKey = "";
		DeviceFingerprint = "";
		UseDesktopSearchInput = false;
		ReturnPageIndex = 0;
		SearchModeActive = false;
		SearchQuery = "";
		SearchResultIndexes = new List<int>();
		PageBeforeSearch = 0;
		CategoryBeforeSearch = null;
		ShowingCategoriesBeforeSearch = true;
	}

	public static void SetAntiReportTypeStatusText(string value)
	{
		AntiReportTypeStatusText = value;
		if (!ShowingCategories && CurrentCategory == "Safety")
		{
			RefreshMenu();
		}
	}

	private static void ConfigurePaginationButtons(Transform root)
	{
		if (root == null)
		{
			return;
		}
		Transform forwardRoot = root.Find("Forward");
		Transform previousRoot = root.Find("Previous");
		if (forwardRoot != null)
		{
			Button component = forwardRoot.GetComponent<Button>();
			if (component != null)
			{
				component.onClick.RemoveAllListeners();
				component.onClick.AddListener(() => ChangePage(1));
			}
		}
		if (previousRoot == null)
		{
			return;
		}
		Button previousButton = previousRoot.GetComponent<Button>();
		if (previousButton != null)
		{
			previousButton.onClick.RemoveAllListeners();
			previousButton.onClick.AddListener(() => ChangePage(-1));
		}
	}

	private static void ConfigureLeaveButtons()
	{
		ConfigureLeaveButton(VrBackgroundTransform, out VrLeaveButton, out VrLeaveButtonImage);
		ConfigureLeaveButton(PcBackgroundTransform, out PcLeaveButton, out PcLeaveButtonImage);
	}

	private static void ConfigureLeaveButton(
		Transform background,
		out Button button,
		out Image buttonImage)
	{
		button = null;
		buttonImage = null;
		if (background == null)
		{
			return;
		}
		Transform leaveRoot = background.Find("LeaveBack");
		if (leaveRoot == null)
		{
			return;
		}

		button = leaveRoot.GetComponent<Button>();
		if (button == null)
		{
			button = leaveRoot.gameObject.AddComponent<Button>();
		}
		Transform imageTransform = leaveRoot.Find("LeaveButton");
		buttonImage = imageTransform != null
			? imageTransform.GetComponent<Image>()
			: leaveRoot.GetComponent<Image>();
		if (buttonImage != null)
		{
			buttonImage.color = DefaultLeaveButtonColor;
		}
		button.onClick.RemoveAllListeners();
		button.onClick.AddListener(FlashMenuIndicator);
	}

	public static void SetImpactColorStatusText(string value)
	{
		ImpactColorStatusText = value;
		if (!ShowingCategories && CurrentCategory == "Settings")
		{
			RefreshMenu();
		}
	}

	public static void GoHome()
	{
		if (SearchModeActive)
		{
			CloseSearch();
		}
		ShowingCategories = true;
		CurrentPageIndex = ReturnPageIndex;
		CurrentCategory = null;
		RefreshEnabledMods();
		RefreshMenu();
	}

public static void SetGunTracerStatusText(string value)
	{
		GunTracerStatusText = value;
		if (!ShowingCategories && CurrentCategory == "Settings")
		{
			RefreshMenu();
		}
	}

	private static void BuildCategoryList()
	{
		List<string> availableCategories = AllMods
			.Select(mod => mod.Attribute.Category)
			.Distinct()
			.ToList();
		Categories = new List<string> { "Enabled" };
		foreach (string category in CategoryDisplayOrder)
		{
			if (category != "Enabled" && (category == "Soundboard" || availableCategories.Contains(category)))
			{
				Categories.Add(category);
			}
		}
		foreach (string category in availableCategories)
		{
			if (!Categories.Contains(category))
			{
				Categories.Add(category);
			}
		}
		CurrentCategory = Categories.FirstOrDefault();
	}

	public static void SetSpeedBoostStatusText(string value)
	{
		SpeedBoostStatusText = value;
		if (!ShowingCategories && (CurrentCategory == "Settings" || CurrentCategory == "Movement"))
		{
			RefreshMenu();
		}
	}

	public static void SetGunSoundStatusText(string value)
	{
		GunSoundStatusText = value;
		if (!ShowingCategories && CurrentCategory == "Settings")
		{
			RefreshMenu();
		}
	}

	private static GameObject LoadEmbeddedMenuBundle(string resourceName)
	{
		using (Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
		using (MemoryStream buffer = new MemoryStream())
		{
			resourceStream.CopyTo(buffer);
			byte[] resourceBytes = buffer.ToArray();
			bool isPlainUnityBundle = resourceBytes.Length > 8
				&& resourceBytes[0] == (byte)'U'
				&& resourceBytes[1] == (byte)'n'
				&& resourceBytes[2] == (byte)'i'
				&& resourceBytes[3] == (byte)'t';
			byte[] bundleBytes = isPlainUnityBundle
				? resourceBytes
				: DecryptEmbeddedBundle(resourceBytes);
			AssetBundle bundle = AssetBundle.LoadFromMemory(bundleBytes);
			if (bundle == null)
			{
				return null;
			}
			GameObject prefab = bundle.LoadAllAssets<GameObject>().FirstOrDefault();
			return prefab != null
				? Object.Instantiate(prefab)
				: null;
		}
	}

	public static void SetSteamArmsLengthStatusText(string value)
	{
		SteamLongArmsStatusText = value;
		if (!ShowingCategories && CurrentCategory == "Rig")
		{
			RefreshMenu();
		}
	}

	private static async void PollRigColorBuffer2()
	{
		await PollRigColorBufferInternal();
	}

}
