using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NaughtyAttributes;
using Random = UnityEngine.Random;

public class Hand : MonoBehaviour {
	[Header("Audio"), Space]
	[SerializeField] AudioClip hitFirstTime;
	[SerializeField] AudioClip hitSecondTime;

	[Header("Refs"), Space]
	[SerializeField] ParticleSystem[] bloodParticles;

	int hitted = 0;

	public void OnCollideCoin() {
		++hitted;

		AudioManager.Instance.Play(hitted == 1 ? hitFirstTime : hitSecondTime);

		StartCoroutine(Bleed());
	}

	IEnumerator Bleed() {
		foreach (var part in bloodParticles) {
			part.Stop();
			part.Play();
		}
		yield return new WaitForSeconds(1.0f);
	}
}
