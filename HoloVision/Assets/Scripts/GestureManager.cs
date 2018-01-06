using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

public class GestureManager : MonoBehaviour
{
    private GestureRecognizer gestures = null;

    public GameObject ReceiverObject = null;

	// Use this for initialization
	void Start ()
    {
        InitializeGestures();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void InitializeGestures()
    {
        gestures = new GestureRecognizer();
        gestures.SetRecognizableGestures(GestureSettings.Tap);

        // events
        gestures.Tapped += OnGesturesTapped;

        gestures.StartCapturingGestures();
    }

    private void OnGesturesTapped(TappedEventArgs obj)
    {
        if (ReceiverObject != null)
        {
            VisionManager mgr = ReceiverObject.GetComponent<VisionManager>();

            if (mgr != null)
            {
                mgr.OnTapped();
            }

            //ReceiverObject.SendMessage("OnTapped");
        }
    }



}
