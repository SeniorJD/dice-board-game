using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinchZoomCameraScript : MonoBehaviour {

    public float perspectiveZoomSpeed = 0.5f;        // The rate of change of the field of view in perspective mode.
    public float orthoZoomSpeed = 0.5f;        // The rate of change of the orthographic size in orthographic mode.

    Camera myCamera;

    private void Awake()
    {
        myCamera = gameObject.GetComponent<Camera>();
    }

    void Update()
    {
        // If there are two touches on the device...
        if (Input.touchCount == 2)
        {
            // Store both touches.
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            // If the camera is orthographic...
            if (myCamera.orthographic)
            {
                // ... change the orthographic size based on the change in distance between the touches.
                myCamera.orthographicSize += deltaMagnitudeDiff * orthoZoomSpeed;

                // Make sure the orthographic size never drops below zero.
                myCamera.orthographicSize = Mathf.Max(myCamera.orthographicSize, 0.1f);
            }
            else
            {
                // Otherwise change the field of view based on the change in distance between the touches.
                myCamera.fieldOfView += deltaMagnitudeDiff * perspectiveZoomSpeed;

                // Clamp the field of view to make sure it's between 0 and 180.
                myCamera.fieldOfView = Mathf.Clamp(myCamera.fieldOfView, 0.1f, 179.9f);
            }
        } else
        {
            float wheel = Input.GetAxis("Mouse ScrollWheel");

            float orthographicSize = myCamera.orthographicSize;
            orthographicSize -= wheel * 10;

            if (orthographicSize > 50)
            {
                orthographicSize = 50;
            } else if (orthographicSize < 20)
            {
                orthographicSize = 20;
            }

            myCamera.orthographicSize = orthographicSize;
        }
    }
}
