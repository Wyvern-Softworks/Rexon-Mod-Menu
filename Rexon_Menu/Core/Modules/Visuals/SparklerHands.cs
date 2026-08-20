// Processed with MiDeobf Engine v2.1.2rc (14/08/26)
// discord.gg/wyvern
// https://wyvern.im

// Type: Rexon_Menu.Core.Modules.Visuals.SparklerHands
// Assembly: Rexon_Menu, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using Rexon_Menu.Core.Attributes;
using Rexon_Menu.Core.Patches;
using UnityEngine;
using ColorOverLifetimeModule = UnityEngine.ParticleSystem.ColorOverLifetimeModule;
using EmissionModule = UnityEngine.ParticleSystem.EmissionModule;
using MainModule = UnityEngine.ParticleSystem.MainModule;
using MinMaxCurve = UnityEngine.ParticleSystem.MinMaxCurve;
using MinMaxGradient = UnityEngine.ParticleSystem.MinMaxGradient;
using ShapeModule = UnityEngine.ParticleSystem.ShapeModule;
using SizeOverLifetimeModule = UnityEngine.ParticleSystem.SizeOverLifetimeModule;

namespace Rexon_Menu.Core.Modules.Visuals;

[Mod("Sparkler Hands", "Visuals", "Sparks from your fingertips.", false, 29, ModType.Toggle, false)]
internal class SparklerHands : MonoBehaviour
{
	private ParticleSystem _leftSparkler;
	private ParticleSystem _rightSparkler;

	private void OnEnable()
	{
		Transform leftHand = GorillaTagger.Instance.leftHandTransform;
		Transform rightHand = GorillaTagger.Instance.rightHandTransform;
		if (leftHand != null && _leftSparkler == null)
		{
			_leftSparkler = CreateSparkler(leftHand);
		}
		if (rightHand != null && _rightSparkler == null)
		{
			_rightSparkler = CreateSparkler(rightHand);
		}
	}

	private static ParticleSystem CreateSparkler(Transform parent)
	{
		GameObject sparklerObject = new GameObject("Sparkler");
		sparklerObject.transform.SetParent(parent, false);
		sparklerObject.transform.localPosition = Vector3.zero;
		ParticleSystem particles = sparklerObject.AddComponent<ParticleSystem>();
		MainModule main = particles.main;
		main.startLifetime = (MinMaxCurve)0.5f;
		main.startSpeed = new MinMaxCurve(1f, 3f);
		main.startSize = (MinMaxCurve)0.02f;
		main.gravityModifier = (MinMaxCurve)2f;
		main.simulationSpace = ParticleSystemSimulationSpace.World;
		main.maxParticles = 200;
		EmissionModule emission = particles.emission;
		emission.rateOverTime = (MinMaxCurve)50f;
		ShapeModule shape = particles.shape;
		shape.shapeType = ParticleSystemShapeType.Cone;
		shape.angle = 25f;
		shape.radius = 0.01f;

		Gradient gradient = new Gradient
		{
			colorKeys = new[]
			{
				new GradientColorKey(new Color(1f, 0.5f, 0f), 0f),
				new GradientColorKey(new Color(1f, 0.9f, 0f), 1f)
			},
			alphaKeys = new[]
			{
				new GradientAlphaKey(1f, 0f),
				new GradientAlphaKey(0f, 1f)
			}
		};
		ColorOverLifetimeModule colorOverLifetime = particles.colorOverLifetime;
		colorOverLifetime.enabled = true;
		colorOverLifetime.color = new MinMaxGradient(gradient);
		SizeOverLifetimeModule sizeOverLifetime = particles.sizeOverLifetime;
		sizeOverLifetime.enabled = true;
		sizeOverLifetime.size = new MinMaxCurve(1f, AnimationCurve.Linear(0f, 1f, 1f, 0f));
		ParticleSystemRenderer renderer = sparklerObject.GetComponent<ParticleSystemRenderer>();
		((Renderer)renderer).material = ShaderPatch.CreateTransparentMaterial(new Color(1f, 0.6f, 0f));
		return particles;
	}

	private void OnDisable() => DestroySparklers();
	private void OnDestroy() => DestroySparklers();

	private void DestroySparklers()
	{
		DestroySparkler(ref _leftSparkler);
		DestroySparkler(ref _rightSparkler);
	}

	private static void DestroySparkler(ref ParticleSystem particles)
	{
		if (particles == null)
		{
			return;
		}
		Object.Destroy(particles.gameObject);
		particles = null;
	}
}
