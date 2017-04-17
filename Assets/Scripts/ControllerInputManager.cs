using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerInputManager : MonoBehaviour {

	public ControllerEvents controllerEvents;
	public Transform cameraRig;
	public GameObject teleportAimerObject;
	public Vector3 teleportLocation;
	public LayerMask laserMask;
	public float yNudgeAmount = 0f;
	public float throwForce = 1.5f;

	public LineRenderer laser;

	private ControllerEvents.ControllerInteractionEventArgs args;

	public void OnEnable() {

		controllerEvents.TriggerPressed += HandleTriggerPressed;
		controllerEvents.TriggerReleased += HandleTriggerReleased;
		controllerEvents.TouchpadPressed += HandleTouchpadPressed;
		controllerEvents.TouchpadReleased += HandleTouchpadReleased;
	}

	public void OnDisable() {

		controllerEvents.TriggerPressed -= HandleTriggerPressed;
		controllerEvents.TriggerReleased -= HandleTriggerReleased;
		controllerEvents.TouchpadPressed -= HandleTouchpadPressed;
		controllerEvents.TouchpadReleased -= HandleTouchpadReleased;
	}

	private void Start() {
		//laser = GetComponentInChildren<LineRenderer>();
	}

	

	private void HandleTriggerPressed(object sender, ControllerEvents.ControllerInteractionEventArgs e) {
		args = e;
	}

	private void HandleTriggerReleased(object sender, ControllerEvents.ControllerInteractionEventArgs e) {
		args = e;
	}

	private void HandleTouchpadPressed(object sender, ControllerEvents.ControllerInteractionEventArgs e) {
		laser.gameObject.SetActive(true);
		teleportAimerObject.SetActive(true);

		laser.SetPosition(0, controllerEvents.gameObject.transform.position);
		RaycastHit hit;
		if (Physics.Raycast(controllerEvents.gameObject.transform.position, controllerEvents.gameObject.transform.forward, out hit, 15, laserMask)) {
			teleportLocation = hit.point;
			laser.SetPosition(1, teleportLocation);
			// aimer position
			teleportAimerObject.transform.position = new Vector3(teleportLocation.x, teleportLocation.y + yNudgeAmount, teleportLocation.z);
		} else {
			teleportLocation = new Vector3(controllerEvents.gameObject.transform.forward.x * 15 + controllerEvents.gameObject.transform.position.x, controllerEvents.gameObject.transform.forward.y, controllerEvents.gameObject.transform.forward.z * 15 + controllerEvents.gameObject.transform.position.z);
			RaycastHit groundRay;
			if (Physics.Raycast(teleportLocation, -Vector3.up, out groundRay, 17, laserMask)) {
				teleportLocation = new Vector3(controllerEvents.gameObject.transform.forward.x * 15 + controllerEvents.gameObject.transform.position.x, groundRay.point.y, transform.forward.z * 15 + transform.position.z);

			}
			laser.SetPosition(1, controllerEvents.gameObject.transform.forward * 15 + controllerEvents.gameObject.transform.position);
			// aimer position
			teleportAimerObject.transform.position = teleportLocation + new Vector3(0, yNudgeAmount, 0);

		}
	}

	private void HandleTouchpadReleased(object sender, ControllerEvents.ControllerInteractionEventArgs e) {
		laser.gameObject.SetActive(false);
		teleportAimerObject.SetActive(false);
		cameraRig.transform.position = teleportLocation;
	}

	private void GrabObject(ControllerEvents.ControllerInteractionEventArgs e) {
		
			e.col.transform.SetParent(controllerEvents.gameObject.transform);
			e.col.GetComponent<Rigidbody>().isKinematic = true;
			e.device.TriggerHapticPulse(2000);
		
		
		
	}

	private void ThrowObject(ControllerEvents.ControllerInteractionEventArgs e) {
		
			e.col.transform.SetParent(null);
			Rigidbody rb = e.col.GetComponent<Rigidbody>();
			rb.isKinematic = false;
			rb.velocity = e.device.velocity * throwForce;
			rb.angularVelocity = e.device.angularVelocity;
			
		
	}

}
