// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.BoostFPSSetting
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("Boost FPS", "Settings", "Reduces quality settings to boost performance.", false, 20, ModType.Toggle, false)]
internal class BoostFPSSetting : MonoBehaviour
{
	private float _originalShadowDistance;

	private ShadowQuality _originalShadowQuality;

	private int _originalTargetFrameRate;

	private int _originalVSyncCount;

	private int _originalMipmapLimit;

	private bool _settingsCaptured;


	private void OnEnable()
	{
		_originalShadowDistance = QualitySettings.shadowDistance;
		_originalShadowQuality = QualitySettings.shadows;
		_originalTargetFrameRate = Application.targetFrameRate;
		_originalVSyncCount = QualitySettings.vSyncCount;
		_originalMipmapLimit = QualitySettings.globalTextureMipmapLimit;
		_settingsCaptured = true;
	}

	private void Update()
	{
		QualitySettings.shadowDistance = 0f;
		QualitySettings.shadows = (ShadowQuality)0;
		Application.targetFrameRate = 999;
		QualitySettings.vSyncCount = 0;
		QualitySettings.globalTextureMipmapLimit = 99;
	}

	private void OnDisable()
	{
		RestoreQualitySettings();
	}

	private void RestoreQualitySettings()
	{
		if (_settingsCaptured)
		{
			_settingsCaptured = false;
			QualitySettings.shadowDistance = _originalShadowDistance;
			QualitySettings.shadows = _originalShadowQuality;
			Application.targetFrameRate = _originalTargetFrameRate;
			QualitySettings.vSyncCount = _originalVSyncCount;
			QualitySettings.globalTextureMipmapLimit = _originalMipmapLimit;
		}
	}
}
