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
	[SerializeField] SpriteRenderer getDamageBillboard;

	int hitted = 0;

	private void Awake() {
		getDamageBillboard.color = getDamageBillboard.color.SetA(0.0f);
	}

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

		if(getDamageBillboard)
			LeanTweenEx.ChangeAlpha(getDamageBillboard, 1.0f, 0.2f).setEase(LeanTweenType.easeInOutQuad);

		yield return new WaitForSeconds(1.0f);

		if(getDamageBillboard)
			LeanTweenEx.ChangeAlpha(getDamageBillboard, 0.0f, 0.2f).setEase(LeanTweenType.easeInOutQuad);
	}
}
