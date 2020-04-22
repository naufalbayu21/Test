using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> To create visual effects </summary>
public class WorldEffects : Singleton<WorldEffects> {

	[SerializeField] int SparkPullCount = 20;
	[SerializeField] int ExplosionPullCount = 10;
	[SerializeField] int TraceLinePullCount = 10;

	[SerializeField] float TraceTime = 0.1f;
	[SerializeField] float ExplosionHideTime = 1;
	[SerializeField] float SparkHideTime = 1;

	int SparkSoundIndex;

	List<ParticleSystem> Explosions = new List<ParticleSystem>();
	List<ParticleSystem> Sparks = new List<ParticleSystem>();
	List<ParticleSystem> RaySparks = new List<ParticleSystem>();
	List<LineRenderer> TraceLines = new List<LineRenderer>();
	List<AudioClipPreset> SparkSounds { get { return B.Sound.SparkSounds; } }
	List<AudioClipPreset> RaySparkSounds { get { return B.Sound.RaySparkSounds; } }

	/// <summary> Creating objects and populating lists </summary>
	protected override void AwakeSingleton () {

		for (int i = 0; i < ExplosionPullCount; i++) {
			var explosion = Instantiate(B.Resource.Prefabs.ExplosionPrefab, transform);
			explosion.SetActive(false);
			Explosions.Add(explosion);
		}
		for (int i = 0; i < SparkPullCount; i++) {
			var spark = Instantiate(B.Resource.Prefabs.SparkPrefab, transform);
			spark.SetActive(false);
			Sparks.Add(spark);
		}
		for (int i = 0; i < SparkPullCount; i++) {
			var spark = Instantiate(B.Resource.Prefabs.RaySparkPrefab, transform);
			spark.SetActive(false);
			RaySparks.Add(spark);
		}
		for (int i = 0; i < TraceLinePullCount; i++) {
			var trace = Instantiate(B.Resource.Prefabs.TraceLine, transform);
			trace.SetActive(false);
			TraceLines.Add(trace);
		}
	}

	/// <summary> Create explosion visual effect and start play sound explosion</summary>
	/// <param name = "pos"> Create position visual effect</param>
	public void CreateExplosionEffect (Vector2 pos) {
		ParticleSystem explosion = null;
		for (int i = 0; i < Explosions.Count; i++) {
			if (!Explosions[i].gameObject.activeInHierarchy) {
				explosion = Explosions[i];
			}
		}
		if (explosion == null) {
			explosion = Instantiate(B.Resource.Prefabs.ExplosionPrefab, transform);
			Explosions.Add(explosion);
		}
		explosion.SetActive(true);
		explosion.transform.position = pos;
		explosion.Play();
		SoundController.PlaySound(B.Sound.ExplosionSound);
		StartCoroutine(HideObjectWithDellay(explosion.gameObject, ExplosionHideTime));
	}

	/// <summary> Create spark visual effect and start play sound spark
	/// <param name = "pos"> Create position visual effect</param>
	/// </summary>
	public void CreateSparkEffect (Vector2 pos, SparkType sparkType = SparkType.DefaultSpark) {
		List<ParticleSystem> sparks = null;
		ParticleSystem sparkPrefab = null;
		List<AudioClipPreset> sounds = null;
		switch (sparkType) {
			case SparkType.RaySpark: {
				sparks = RaySparks;
				sparkPrefab = B.Resource.Prefabs.RaySparkPrefab;
				sounds = RaySparkSounds;
				break;
			}
			default: {
				sparks = Sparks;
				sparkPrefab = B.Resource.Prefabs.SparkPrefab;
				sounds = SparkSounds;
				break;
			}
		}
		
		ParticleSystem spark = null;
		for (int i = 0; i < sparks.Count; i++) {
			if (!sparks[i].gameObject.activeInHierarchy) {
				spark = sparks[i];
			}
		}
		if (spark == null) {
			spark = Instantiate(sparkPrefab, transform);
			sparks.Add(spark);
		}
		spark.SetActive(true);
		spark.transform.position = pos;
		SparkSoundIndex = SparkSoundIndex >= sounds.Count - 1 ? 0 : SparkSoundIndex + 1;
		SoundController.PlaySound(sounds[SparkSoundIndex]);
		StartCoroutine(HideObjectWithDellay(spark.gameObject, SparkHideTime));
	}

	/// <summary> Create trace visual effect and create spark logic</summary>
	/// <param name = "from"> Start position trace</param>
	/// <param name = "to"> End position trace</param>
	/// <param name = "createSpark"> Create spark after  trace</param>
	public void CreateTrace (Vector2 from, Vector2 to, bool createSpark = false) {
		StartCoroutine(CreateTraceCoroutine(from, to, createSpark));
	}

	IEnumerator CreateTraceCoroutine (Vector2 from, Vector2 to, bool createSpark) {
		var halfTime = TraceTime * 0.5f;
		LineRenderer trace = null;
		for (int i = 0; i < Explosions.Count; i++) {
			if (!TraceLines[i].gameObject.activeInHierarchy) {
				trace = TraceLines[i];
			}
		}
		if (trace == null) {
			trace = Instantiate(B.Resource.Prefabs.TraceLine, transform);
			TraceLines.Add(trace);
		}
		trace.SetActive(true);

		trace.SetPosition(0, from);
		trace.SetPosition(1, to);

		trace.SetStartColorAlpha(1);
		trace.SetEndColorAlpha(0);
		yield return new WaitForSeconds(halfTime);
		trace.SetStartColorAlpha(0);
		trace.SetEndColorAlpha(1);

		yield return new WaitForSeconds(halfTime);
		trace.SetStartColorAlpha(0);
		trace.SetEndColorAlpha(0);

		if (createSpark) {
			CreateSparkEffect(to);
		}

		trace.SetActive(false);
	}

	/// <summary> To hide object after paly visual effect</summary>
	IEnumerator HideObjectWithDellay (GameObject go, float dellay) {
		yield return new WaitForSeconds (dellay);
		go.SetActive(false);
	}
}

public enum SparkType {
	DefaultSpark,
	RaySpark
}
