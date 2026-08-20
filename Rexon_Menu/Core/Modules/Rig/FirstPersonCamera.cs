// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Recovered.Obfuscated.FirstPersonCamera
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Rexon_Menu.Core.Attributes;
using UnityEngine;

namespace Recovered.Obfuscated;

[Mod("First Person Camera", "Rig", "First person camera mode.", false, 45, ModType.Toggle, false)]
internal class FirstPersonCamera : MonoBehaviour
{
	private const string ThirdPersonCameraName = "Third Person Camera";

	private GameObject _cameraObject;


	private void OnEnable()
	{
		_cameraObject = GameObject.Find(ThirdPersonCameraName);
		if (_cameraObject != null)
		{
			_cameraObject.SetActive(false);
		}
	}

	private void OnDisable()
	{
		DisableFirstPersonCamera();
	}

	private void DisableFirstPersonCamera()
	{
		if (_cameraObject != null)
		{
			_cameraObject.SetActive(true);
			return;
		}
		GameObject thirdPersonCamera = GameObject.Find(ThirdPersonCameraName);
		if (thirdPersonCamera != null)
		{
			thirdPersonCamera.SetActive(true);
		}
	}
}
