using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallReset : MonoBehaviour {

	Rigidbody rb;

	private void Start() {
		rb = GetComponent<Rigidbody>();
	}

	private void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.CompareTag("Ground")) {
			transform.position = new Vector3(1.052f, 0.575f, 0.008f);
			rb.isKinematic = true;
			rb.velocity = Vector3.zero; 
			rb.angularVelocity = Vector3.zero;

		}
	}
}
