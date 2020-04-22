using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameBalance;
using System;

/// <summary> 
/// Sound Controller for management 2D sounds and music.
/// </summary>
public class SoundController : Singleton<SoundController> {

	[Serializable]
	private class AudioSourcePreset {
		public AudioSource Source;
		public ClipPriority Priority;
	}

	[SerializeField] List<AudioSourcePreset> SoundsSource = new List<AudioSourcePreset>();
	[SerializeField] AudioSource MusicSource;

	bool MuteSound = false;
	bool MuteMusic = false;
	float SoundVolume = 1f;
	float MusicVolume = 1f;

	protected override void AwakeSingleton () {
		UpdateVolume();
		SetMusicPlayList(B.Sound.Music);
	}

	Coroutine MusicCoroutine;

	/// <summary> Static method for start play sound. </summary>
	public static void PlaySound (AudioClipPreset clipPreset) {
		if (Instance == null) {
			Debug.LogError("SoundController not initialized");
			return;
		}
		if (clipPreset != null) {
			Instance.PlaySound(clipPreset.Clip, clipPreset.Volume, clipPreset.Priority);
		}
	}

	/// <summary> Static method for start play sound with custom volume. </summary>
	public static void PlaySound (AudioClipPreset clipPreset, float multiPlierVolume) {
		if (Instance == null) {
			Debug.LogError("SoundController not initialized");
			return;
		}
		if (clipPreset != null) {
			Instance.PlaySound(clipPreset.Clip, clipPreset.Volume * multiPlierVolume, clipPreset.Priority);
		}
	}

	/// <summary> Static method for stop play sound. </summary>
	public static void StopSound (AudioClipPreset clipPreset) {
		if (Instance == null) {
			Debug.LogError("SoundController not initialized");
			return;
		}
		if (clipPreset != null) {
			Instance.StopSound(clipPreset.Clip);
		}
	}

	/// <summary> Update mute settings, need to call when the sound settings change. </summary>
	public void UpdateMute () {
		MuteSound = Mathf.Approximately(SoundVolume, 0);
		MuteMusic = Mathf.Approximately(MusicVolume, 0);
		PlayStopMusic (!MuteMusic);
	}

	public void UpdateVolume () {
		SoundVolume = PlayerProfile.SoundVolume;
		MusicVolume = PlayerProfile.MusicVolume;
		MusicSource.volume = MusicVolume;
		UpdateMute ();
	}

	/// <summary> method for start play music </summary>
	private void PlayStopMusic (bool play) {
		if (play) {
			if (MusicCoroutine == null) {
				MusicCoroutine = StartCoroutine(PlayMusic());
			}	
		} else {
			if (MusicCoroutine != null)
				StopCoroutine(MusicCoroutine);
			MusicCoroutine = null;
			MusicSource.Stop();
		}
	}

	/// <summary> private method for start play sound. </summary>
	private void PlaySound (AudioClip clip, float volume, ClipPriority priority = ClipPriority.Low) {
		if (MuteSound) return;
		AudioSourcePreset sourcePreset = SoundsSource.Find(s => !s.Source.isPlaying);
		if (sourcePreset == null) {
			sourcePreset = SoundsSource.Find(s => s.Priority < priority);
		}
		if (sourcePreset == null) {
			sourcePreset = SoundsSource.FindLast(s => s.Priority == priority);
		}
		if (sourcePreset == null) return;

		sourcePreset.Source.Stop();
		sourcePreset.Source.clip = clip;
		sourcePreset.Source.volume = volume * SoundVolume;
		sourcePreset.Source.Play();
		sourcePreset.Priority = priority;
	}

	/// <summary> private method for stop play sound. </summary>
	private void StopSound (AudioClip clip) {
		if (MuteSound) return;
		AudioSourcePreset sourcePreset = SoundsSource.Find(s => s.Source.isPlaying && s.Source.clip == clip);
		if (sourcePreset != null) {
			sourcePreset.Source.Stop();
		}
	}

	#region Music

	List<AudioClip> MusicPlayList;

	public void SetMusicPlayList (List<AudioClip> newPlayList) {
		if (MusicPlayList != newPlayList) {
			MusicPlayList = newPlayList;
			if (MusicCoroutine != null) {
				StopCoroutine(MusicCoroutine);
			}
			MusicCoroutine = StartCoroutine(PlayMusic());
		}
	}

	/// <summary> Coroutine to play music in turn </summary>
	private IEnumerator PlayMusic () {
		if (MusicPlayList == null || MusicPlayList.Count == 0 || MuteMusic) yield break;
		System.Random random = new System.Random();
		MusicSource.loop = false;
		if (MusicSource.isPlaying) {
			while (!Mathf.Approximately(MusicSource.volume, 0)) {
				yield return null;
				MusicSource.volume = Mathf.MoveTowards(MusicSource.volume, 0f, Time.deltaTime);
			}
			MusicSource.Stop();
		}
		MusicSource.volume = MusicVolume;
		while (true) {
			if (!MusicSource.isPlaying) {
				AudioClip clip = MusicPlayList[random.Next(0, (MusicPlayList.Count - 1))];
				MusicSource.clip = clip;
				MusicSource.Play();
			}
			yield return new WaitForSecondsRealtime(2f);
		}
	}

	#endregion //Music
}

public enum ClipPriority {
	Low,
	Medium,
	Hight
}

[Serializable]
public class AudioClipPreset {
	public AudioClip Clip;
	public ClipPriority Priority;
	public float Volume = 1f;
}
