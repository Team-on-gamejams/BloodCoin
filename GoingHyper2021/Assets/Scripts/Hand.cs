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
	[Header("Refs"), Space]
	[SerializeField] ParticleSystem[] bloodParticles;

	public void OnCollideCoin() {
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
