// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu_Mat.MatBridge
// Assembly: Rexon-Menu-Mat, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System;
using System.Threading;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Rexon_Menu_Mat;

public static class MatBridge
{
    private static bool _customRpcMode;
    private static int _initializationCount;
    private static long _initializationToken;

    public static bool GetCRPC() => _customRpcMode;

    public static void SetCRPC(bool value) => _customRpcMode = value;

    public static int GetInitCount() => _initializationCount;

    public static long GetInitToken() => _initializationToken;

    public static void Initialize()
    {
        _initializationCount++;

        long tickCount = Environment.TickCount;
        long utcTicks = DateTime.UtcNow.Ticks;
        long threadId = Thread.CurrentThread.ManagedThreadId;
        _initializationToken = (tickCount << 32)
            ^ (tickCount * 31L)
            ^ utcTicks
            ^ (utcTicks >> 7)
            ^ (threadId << 24);

        if (_initializationToken == 0)
        {
            _initializationToken = 1;
        }

        _customRpcMode = false;
    }

    public static bool IsTokenValid(long candidate) =>
        candidate != 0 && _initializationCount > 0;

    public static VRRig GetVRRigFor(Player player)
    {
        if (_initializationCount < 1)
        {
            return null;
        }

        NetPlayer netPlayer = player;
        return GorillaGameManager.StaticFindRigForPlayer(netPlayer);
    }

    public static bool IsInfected(Player player)
    {
        if (_initializationCount < 1 || player == null || GorillaGameManager.instance == null)
        {
            return false;
        }

        foreach (Player otherPlayer in PhotonNetwork.PlayerListOthers)
        {
            NetPlayer target = player;
            NetPlayer other = otherPlayer;
            if (GorillaGameManager.instance.LocalCanTag(target, other))
            {
                return true;
            }
        }

        return false;
    }

    public static bool IsModEnabledFor(GameObject host, Type modType)
    {
        if (_initializationCount < 1 || host == null || modType == null)
        {
            return false;
        }

        return host.GetComponent(modType) != null;
    }

    public static bool IsModEnabledByName(GameObject host, string typeFullName)
    {
        if (_initializationCount < 1 || host == null || string.IsNullOrEmpty(typeFullName))
        {
            return false;
        }

        foreach (Component component in host.GetComponents<Component>())
        {
            if (component != null && component.GetType().FullName == typeFullName)
            {
                return true;
            }
        }

        return false;
    }

    public static VRRig GetLocalRigFor()
    {
        return _initializationCount < 1 ? null : GorillaTagger.Instance.offlineVRRig;
    }

    public static void SetSerializationRateFor(int rate)
    {
        if (_initializationCount < 1)
        {
            return;
        }

        PhotonNetwork.SendRate = rate;
        PhotonNetwork.SerializationRate = rate;
    }

    public static bool ShouldSendRPC(string method)
    {
        if (_initializationCount < 1)
        {
            return false;
        }

        if (!_customRpcMode)
        {
            return true;
        }

        return method == "JoinWithItemsRPC"
            || method == "RequestPartyGameMode"
            || method == "RPC_PlaySplashEffect"
            || method == "OwnershipRequested";
    }
}
