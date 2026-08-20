// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Bootstrapper
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Reflection;
using HarmonyLib;
using Rexon_Menu.Core.Utilities;
using Rexon_Menu.Interface;
using Rexon_Menu_Mat;
using UnityEngine;

using Object = UnityEngine.Object;

namespace Rexon_Menu.Core;

internal static class Bootstrapper
{
	private const string HarmonyId = "live.rexon.plugins.menu";
	private const string RuntimeObjectName = "rexon_runtime";
	private const string NotificationObjectName = "RexonNotifications";


	internal static void Run()
	{
		if (MatBridge.GetInitCount() < 1 || !MatBridge.IsTokenValid(MatBridge.GetInitToken()))
		{
			return;
		}

		new Harmony(HarmonyId).PatchAll(Assembly.GetExecutingAssembly());

		GameObject runtimeObject = new GameObject(RuntimeObjectName);
		runtimeObject.AddComponent<Main>();
		runtimeObject.AddComponent<ModManager>();
		runtimeObject.AddComponent<LegacyMenu>();
		Object.DontDestroyOnLoad(runtimeObject);

		GameObject notificationObject = new GameObject(NotificationObjectName);
		notificationObject.AddComponent<NotificationManager>();
		Object.DontDestroyOnLoad(notificationObject);
	}
}
