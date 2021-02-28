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

	[Header("Audio"), Space]
	[SerializeField] AudioClip spinStart;
	[SerializeField] AudioClip pickupStar;
	[SerializeField] AudioClip fall;
	[SerializeField] AudioClip pushStart;
	[SerializeField] AudioClip reflect;

	[Header("Visual")]
	[SerializeField] float lineLenghtMod = 2.0f;

	[Header("Balance")]
	[SerializeField] float force = 5.0f;
	[Space]
	[SerializeField] float minForce = 1.0f;
	[SerializeField] float maxForce = 10.0f;
	[Space]
	[SerializeField] float speedToStop = 0.33f;

	[Header("Refs")]
	[SerializeField] LineRenderer forcelr;
	[SerializeField] Rigidbody rb;

	bool isHold = false;
	bool isThrowed = false;
	bool isHit = false;
	bool isStop = false;

	Vector3 startHoldPos;
	Vector3 holdPos;

	List<Vector3> posToMove = new List<Vector3>();
	int nextPos = 1;

	private void Awake() {
		forcelr.useWorldSpace = true;
		forcelr.gameObject.SetActive(false);

		speedToStop *= speedToStop;
	}

	private void Update() {
		if (isThrowed)
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
			
			if (force.magnitude <= Mathf.Epsilon) {
				pos.Add(transform.position);
			}
			else {
				float y = transform.position.y;
				float dist = force.magnitude * lineLenghtMod;
				Vector3 lastProcessedPos = transform.position;
				Vector3 lastDir = force.normalized;
				RaycastHit hit;

				while (dist > 0) {
					if (Physics.Raycast(lastProcessedPos, lastDir, out hit, dist, UnityConstants.Layers.DefaultMask)) {
						pos.Add(hit.point.SetY(y));
						
						if (hit.transform.CompareTag(UnityConstants.Tags.Coin) || hit.transform.CompareTag(UnityConstants.Tags.Hand)) {
							dist = 0;
						}
						else {
							dist -= hit.distance;
							lastProcessedPos = hit.point;
							lastDir = Vector3.Reflect(lastDir, hit.normal);
						}
					}
					else {
						pos.Add(lastProcessedPos + lastDir * dist);
						dist = 0;
					}
				}
			}

			forcelr.positionCount = pos.Count;
			forcelr.SetPositions(pos.ToArray());
		}
	}

	private void FixedUpdate() {
		if (isThrowed && isHold) {
			Vector3 force = GetThrowVector();
			if (force.magnitude <= Mathf.Epsilon) {
				isThrowed = isHold = false;
			}
			else {
				isHold = false;
				LaunchCoin();
			}
				
			return;
		}

		if(isThrowed && !isStop) {
			if (Physics.Raycast(transform.position, rb.velocity.normalized, out RaycastHit hit, rb.velocity.magnitude * Time.deltaTime, UnityConstants.Layers.DefaultMask)) {
				if (hit.transform.CompareTag(UnityConstants.Tags.Hand)) {
					OnCollideHand(hit.transform.GetComponent<Hand>());
				}
				else {
					rb.velocity = Vector3.Reflect(rb.velocity, hit.normal);

					OnCollideObstacle();
				}
			}
		}

		if (isThrowed && !isStop && !isHit && rb.velocity.sqrMagnitude <= speedToStop) {
			OnStop();
		}
	}

	void OnCollideHand(Hand hand) {
		if (isStop)
			return;
		isStop = true;

		hand.OnCollideCoin();

		isHit = true;
		rb.velocity = Vector3.zero;
		rb.isKinematic = true;

		LeanTween.scale(gameObject, Vector3.zero, 1.0f)
		.setEase(LeanTweenType.easeInOutQuart)
		.setOnComplete(() => Destroy(gameObject));

		//TODO: win
		coinSpawner.SpawnCoin();
	}

	void OnCollideObstacle() {
		AudioManager.Instance.Play(reflect);
	}

	void LaunchCoin() {
		Vector3 vec = GetThrowVector();
		rb.AddForce(vec * force, ForceMode.Impulse);
		AudioManager.Instance.Play(pushStart);
	}

	public void Fall() {
		if (isStop)
			return;
		AudioManager.Instance.Play(fall);
		OnStop();
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
		if((startHoldPos - holdPos).magnitude <= 10.0f) {
			return Vector3.zero;
		}

		Vector2 diff = TemplateGameManager.Instance.Camera.ScreenToWorldPoint(startHoldPos) - TemplateGameManager.Instance.Camera.ScreenToWorldPoint(holdPos);
		Vector3 force = new Vector3(diff.x, 0, diff.y);

		if(force.magnitude < minForce) {
			force = force.normalized * minForce;
		}
		else if (force.magnitude > maxForce) {
			force = force.normalized * maxForce;
		}

		return force;
	}
}
