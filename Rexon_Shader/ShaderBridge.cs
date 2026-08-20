// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Shader.ShaderBridge
// Assembly: Rexon-Shader, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace Rexon_Shader;

public static class ShaderBridge
{
    private const string MainShaderName = "GorillaTag/UberShader";
    private const string HarmonyId = "live.rexon.shader.bridge";

    private static bool _initialized;
    private static Harmony _harmony;
    private static Shader _espShader;

    public static Shader Cached { get; private set; }

    public static void Initialize()
    {
        if (_initialized)
        {
            return;
        }

        _initialized = true;
        Cached = Shader.Find(MainShaderName);
        _harmony = new Harmony(HarmonyId);
        _harmony.PatchAll(Assembly.GetExecutingAssembly());
    }

    public static Material CreateMaterial(Color color)
    {
        if (Cached == null)
        {
            Cached = Shader.Find(MainShaderName);
        }

        return new Material(Cached)
        {
            color = color
        };
    }

    public static Shader EspShader
    {
        get
        {
            if (_espShader == null)
            {
                _espShader = Shader.Find("GUI/Text Shader");
                if (_espShader == null)
                {
                    _espShader = Shader.Find("Sprites/Default");
                }

                if (_espShader == null)
                {
                    _espShader = Cached;
                }
            }

            return _espShader;
        }
    }

    public static Material CreateTransparentMaterial(Color color)
    {
        Material material = new Material(EspShader)
        {
            color = color
        };
        material.SetInt("_ZTest", 8);
        material.SetFloat("_ZWrite", 0f);
        material.renderQueue = 4000;
        return material;
    }
}

[HarmonyPatch(typeof(GameObject), "CreatePrimitive")]
internal class CreatePrimitivePatch
{
    private static void Postfix(GameObject __result)
    {
        if (ShaderBridge.Cached == null)
        {
            return;
        }

        Renderer renderer = __result.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.shader = ShaderBridge.Cached;
        }
    }
}
