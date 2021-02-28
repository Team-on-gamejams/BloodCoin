using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NaughtyAttributes;
using Random = UnityEngine.Random;

public class Floor : MonoBehaviour {
	private void OnCollisionEnter(Collision collision) {
		switch (collision.gameObject.tag) {
			case UnityConstants.Tags.Coin:
				OnCollideCoin(collision.gameObject.GetComponent<Coin>());
				break;
		}
	}

	void OnCollideCoin(Coin c) {
		c.Fall();
	}
}
