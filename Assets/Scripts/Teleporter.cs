using UnityEngine;
using System.Collections;
using System;

public class Teleporter : MonoBehaviour {

    //public LaserPointer laserPointer;
    public ControllerEvents controllerEvents;
    private Transform target;
    private RaycastHit hitInfo;
    public Transform cameraRig;

    public void OnEnable() {
        //laserPointer.PointerOn += HandlePointerOn;
        controllerEvents.TriggerReleased += HandleTriggerReleased;
    }

    public void OnDisable() {
        //laserPointer.PointerOn -= HandlePointerOn;
        controllerEvents.TriggerReleased -= HandleTriggerReleased;
    }

    private void HandleTriggerReleased(object sender, ControllerEvents.ControllerInteractionEventArgs e) {
        if (target.GetComponent<Terrain>()) {
            var terrainHeight = Terrain.activeTerrain.SampleHeight(hitInfo.point);
            float y = (terrainHeight > hitInfo.point.y) ? hitInfo.point.y : terrainHeight;
            cameraRig.transform.position = new Vector3(hitInfo.point.x, y, hitInfo.point.z);
        }
    }

    //private void HandlePointerOn(object sender, LaserPointer.PointerEventArgs e) {
    //    target = e.target;
    //    hitInfo = e.hitInfo;
    //}
}
