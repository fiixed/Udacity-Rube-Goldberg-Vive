using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System;

/// <summary>
/// Defines and Publishes Vive controller events
/// Add this as a component to each controller for which you would like to listen to events
/// </summary>

[RequireComponent(typeof(SteamVR_TrackedObject))]
public class ControllerEvents : MonoBehaviour {

	// Event Declaration Code
    public struct ControllerInteractionEventArgs {
        public uint controllerIndex;
		public SteamVR_Controller.Device device;
		public Vector2 touchpadAxis;
		public Collider col;
    }

    public delegate void ControllerInteractionEventHandler(object sender, ControllerInteractionEventArgs e);

    [System.Serializable]
    public class ControllerEvent : UnityEvent<ControllerInteractionEventArgs> { }

    // Native C# Events are more efficient but can only be used from code
    public event ControllerInteractionEventHandler TriggerPressed;
    public event ControllerInteractionEventHandler TriggerReleased;
	
    public event ControllerInteractionEventHandler TouchpadPressDown;
	public event ControllerInteractionEventHandler TouchpadPressed;
    public event ControllerInteractionEventHandler TouchpadReleased;

    public event ControllerInteractionEventHandler TouchpadTouchStart;
    public event ControllerInteractionEventHandler TouchpadTouchEnd;

    public event ControllerInteractionEventHandler TouchpadAxisChanged;
   

    //// Unity Events add a little overhead but listeners can be assigned via Unity editor
    //public ControllerEvent unityTriggerPressed = new ControllerEvent();
    //public ControllerEvent unityTriggerReleased = new ControllerEvent();

    // Member Variables
    [HideInInspector]
    public bool triggerPressed = false;
    [HideInInspector]
    public bool touchpadPressDown = false;
	[HideInInspector]
	public bool touchpadPressed = false;
	[HideInInspector]
    public bool touchpadTouched = false;
    [HideInInspector]
    public bool touchpadAxisChanged = false;


    private uint controllerIndex;
    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device device;
	private Collider ballCollider;

    private Vector2 touchpadAxis = Vector2.zero;

    // Unity lifecycle method
    private void Awake() {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    // Unity lifecycle method
    private void Update() {
        controllerIndex = (uint)trackedObj.index;
        device = SteamVR_Controller.Input((int)controllerIndex);
        Vector2 currentTouchpadAxis = device.GetAxis();

        //// Trigger Pressed
        //if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger)) { 
        //    OnTriggerPressed(SetButtonEvent(ref triggerPressed, true));
        //} else if (device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger)) { 
        //    OnTriggerReleased(SetButtonEvent(ref triggerPressed, false));
        //}

        // Touchpad PressDown
        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad)) {
            OnTouchpadPressDown(SetButtonEvent(ref touchpadPressDown, true));
        } else if (device.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad)) {
            OnTouchpadReleased(SetButtonEvent(ref touchpadPressDown, false));
        }

		// Touchpad Pressed
		if (device.GetPress(SteamVR_Controller.ButtonMask.Touchpad)) {
			OnTouchpadPressed(SetButtonEvent(ref touchpadPressed, true));
		} else if (device.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad)) {
			OnTouchpadReleased(SetButtonEvent(ref touchpadPressed, false));
		}

		// Touchpad Touched
		if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Touchpad)) {
            OnTouchpadTouchStart(SetButtonEvent(ref touchpadTouched, true));
        } else if (device.GetTouchUp(SteamVR_Controller.ButtonMask.Touchpad)) {
            OnTouchpadTouchEnd(SetButtonEvent(ref touchpadTouched, false));
        }

        // Touchpad axis changed
        if (touchpadAxis == currentTouchpadAxis) {
            touchpadAxisChanged = false;
        } else {
            OnTouchpadAxisChanged(SetButtonEvent(ref touchpadTouched, true));
            touchpadAxisChanged = true;
        }

        touchpadAxis = new Vector2(currentTouchpadAxis.x, currentTouchpadAxis.y);
    }


    // Creates, fills out, and returns a new ControllerInteractionEventArgs struct
    // (Convenience method to reduce code duplication)
    private ControllerInteractionEventArgs SetButtonEvent(ref bool buttonBool, bool value) {
        buttonBool = value;
        ControllerInteractionEventArgs e;
		e.device = device;
        e.controllerIndex = controllerIndex;
        e.touchpadAxis = device.GetAxis();
		e.col = ballCollider;
        return e;
    }

    // Event publisher
    public virtual void OnTriggerPressed(ControllerInteractionEventArgs e) {
        if (TriggerPressed != null) {
            TriggerPressed(this, e);
        }
        //unityTriggerPressed.Invoke(e);
    }

    public virtual void OnTriggerReleased(ControllerInteractionEventArgs e) {
        if (TriggerReleased != null) {
            TriggerReleased(this, e);
        }
        //unityTriggerReleased.Invoke(e);
    }

    public virtual void OnTouchpadPressDown(ControllerInteractionEventArgs e) {
        if (TouchpadPressDown != null) {
            TouchpadPressDown(this, e);
        }
       
    }

	public virtual void OnTouchpadPressed(ControllerInteractionEventArgs e) {
		if (TouchpadPressed != null) {
			TouchpadPressed(this, e);
		}

	}

	public virtual void OnTouchpadReleased(ControllerInteractionEventArgs e) {
        if (TouchpadReleased != null) {
            TouchpadReleased(this, e);
        }
        
    }

    public virtual void OnTouchpadTouchStart(ControllerInteractionEventArgs e) {
        if (TouchpadTouchStart != null) {
            TouchpadTouchStart(this, e);
        }
    }

    public virtual void OnTouchpadTouchEnd(ControllerInteractionEventArgs e) {
        if (TouchpadTouchEnd != null) {
            TouchpadTouchEnd(this, e);
        }
    }

    public virtual void OnTouchpadAxisChanged(ControllerInteractionEventArgs e) {
        if (TouchpadAxisChanged != null) {
            TouchpadAxisChanged(this, e);
        }
    }

	private void OnTriggerStay(Collider other) {
		if (other.gameObject.CompareTag("Throwable")) {
			if (device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger)) {
				OnTriggerReleased(SetButtonEvent(ref triggerPressed, false));
				ballCollider = other;
			} else if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger)) {
				OnTriggerPressed(SetButtonEvent(ref triggerPressed, true));
				ballCollider = other;
			}
		}
	}

}
