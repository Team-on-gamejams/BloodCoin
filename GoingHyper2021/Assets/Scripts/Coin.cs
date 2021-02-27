using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NaughtyAttributes;
using Random = UnityEngine.Random;

public class Coin : MonoBehaviour {
	[NonSerialized] public CoinSpawner coinSpawner;

	[Header("Balance")]
	[SerializeField] float force = 5.0f;
	[SerializeField] float minForce = 1.0f;
	[SerializeField] float maxForce = 10.0f;

	[Header("Refs")]
	[SerializeField] LineRenderer forcelr;
	[SerializeField] Rigidbody rb;

	bool isStart = false;
	bool isHold = false;
	bool isThrowed = false;
	bool isHit = false;
	bool isStop = false;

	Vector3 startHoldPos;
	Vector3 holdPos;

	private void Awake() {
		forcelr.useWorldSpace = true;
	}

	private IEnumerator Start() {
		yield return null;
		yield return null;

		isStart = true;
	}

	private void Update() {
		if (isThrowed || !isStart)
			return;

		holdPos = Input.mousePosition;
		holdPos.z = 10.0f;
		
		if (Input.GetMouseButtonDown(0)) {
			isHold = true;
			startHoldPos = Input.mousePosition;
			startHoldPos.z = 10.0f;
			forcelr.gameObject.SetActive(true);
		}
		else if (isHold && Input.GetMouseButtonUp(0)) {
			isThrowed = true;
			forcelr.gameObject.SetActive(false);
		}

		if (isHold) {
			Vector3 force = GetThrowVector();
			List<Vector3> pos = new List<Vector3>();

			pos.Add(transform.position);
			pos.Add(transform.position + force);
			//TODO: draw full line

			forcelr.SetPositions(pos.ToArray());
		}
	}

	private void FixedUpdate() {
		if (isThrowed && isHold) {
			isHold = false;
			LaunchCoin();
			return;
		}

		if (isThrowed && !isHit && rb.velocity.sqrMagnitude <= 0.01f) {
			OnStop();
		}
	}

	private void OnCollisionEnter(Collision collision) {
		switch (collision.gameObject.tag) {
			case UnityConstants.Tags.Hand:
				OnCollideHand();
				break;

			case UnityConstants.Tags.Untagged:
				OnCollideObstacle(collision);
				break;
		}
	}

	void OnCollideHand() {
		if (isStop)
			return;
		isStop = true;

		isHit = true;
		rb.velocity = Vector3.zero;

		//TODO: win
		coinSpawner.SpawnCoin();
	}

	void OnCollideObstacle(Collision collision) {
		//rb.velocity = Vector3.Reflect(rb.velocity, collision.GetContact(0).normal);
	}

	void LaunchCoin() {
		Vector3 vec = GetThrowVector();
		rb.AddForce(vec * force, ForceMode.Impulse);
	}

	public void OnStop() {
		if (isStop)
			return;
		isStop = true;

		coinSpawner.SpawnCoin();
		LeanTween.scale(gameObject, Vector3.zero, 1.0f)
			.setEase(LeanTweenType.easeOutQuad)
			.setOnComplete(() => Destroy(gameObject));
	}

	Vector3 GetThrowVector() {
		Vector2 diff = TemplateGameManager.Instance.Camera.ScreenToWorldPoint(holdPos) - TemplateGameManager.Instance.Camera.ScreenToWorldPoint(startHoldPos);
		Vector3 force = new Vector3(diff.x, 0, diff.y);
		//Debug.Log($"{holdPos} {startHoldPos} {TemplateGameManager.Instance.Camera.ScreenToWorldPoint(holdPos)} {TemplateGameManager.Instance.Camera.ScreenToWorldPoint(startHoldPos)} {diff}");

		if(force.magnitude < minForce) {
			force = force.normalized * minForce;
		}
		else if (force.magnitude > maxForce) {
			force = force.normalized * maxForce;
		}

		return force;
	}
}
