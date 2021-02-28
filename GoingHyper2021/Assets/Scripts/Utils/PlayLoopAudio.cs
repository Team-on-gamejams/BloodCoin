using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NaughtyAttributes;
using Random = UnityEngine.Random;

public class PlayLoopAudio : MonoBehaviour {
	[Header("Audio"), Space]
	[SerializeField] AudioClip audio;

	void Start() {
		AudioManager.Instance.PlayMusic(audio, 0.6f);
	}
}
