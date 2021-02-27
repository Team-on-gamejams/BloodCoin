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
	private void OnCollisionEnter(Collision collision) {
		switch (collision.gameObject.tag) {
			case UnityConstants.Tags.Coin:
				OnCollideCoin();
				break;
		}
	}

	void OnCollideCoin() {

	}
}
