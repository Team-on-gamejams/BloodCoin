using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NaughtyAttributes;
using Random = UnityEngine.Random;

public class CoinSpawner : MonoBehaviour {
	[Header("Refs"), Space]
	[SerializeField] Coin firstCoin;

	[Header("Prefabs"), Space]
	[SerializeField] GameObject coinPrefab;

	private void Awake() {
		firstCoin.coinSpawner = this;
	}

	public void SpawnCoin() {
		GameObject coingo = Instantiate(coinPrefab, transform.position, Quaternion.identity, transform);
		coingo.transform.localScale = Vector3.zero;

		Coin coin = coingo.GetComponent<Coin>();
		coin.enabled = false;
		coin.coinSpawner = this;

		LeanTween.scale(coingo, Vector3.one, 1.0f)
			.setEase(LeanTweenType.easeOutBack)
			.setOnComplete(() => { 
				coin.enabled = true;
			});

	}
}
