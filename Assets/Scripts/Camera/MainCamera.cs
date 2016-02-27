using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections;

public class MainCamera : MonoBehaviour
{
    // WASDQE Panning
    public float minPanSpeed = 0.1f;    // Starting panning speed
    public float maxPanSpeed = 1000f;   // Max panning speed
    public float panTimeConstant = 20f; // Time to reach max panning speed

    // Mouse right-down rotation
    public float rotateSpeed = 10; // mouse down rotation speed about x and y axes
    public float zoomSpeed = 2;    // zoom speed
    public int ScrollArea = 25;
    public float ScrollSpeed = 1;

     [Header("Border Limits")]
    public float xMinLimit;
    public float xMaxLimit;
    public float zMinLimit;
    public float zMaxLimit;
    public float minFieldOfView;
    public float maxFieldOfView;
    

    float panT = 0;
    float panSpeed = 10;
    Vector3 panTranslation;
    bool wKeyDown = false;
    bool aKeyDown = false;
    bool sKeyDown = false;
    bool dKeyDown = false;
    bool qKeyDown = false;
    bool eKeyDown = false;

    Vector3 lastMousePosition;
    new Camera camera;

    public bool isControllerOn = true;
    void Start()
    {
        camera = GetComponent<Camera>();
    }

    void Update()
    {
        if (isControllerOn)
        {
            //
            // WASDQE Panning

            // read key inputs
            wKeyDown = Input.GetKey(KeyCode.W);
            aKeyDown = Input.GetKey(KeyCode.A);
            sKeyDown = Input.GetKey(KeyCode.S);
            dKeyDown = Input.GetKey(KeyCode.D);
            qKeyDown = Input.GetKey(KeyCode.Q);
            eKeyDown = Input.GetKey(KeyCode.E);

            // determine panTranslation
            panTranslation = Vector3.zero;
            if (dKeyDown && !aKeyDown)
                panTranslation += Vector3.right * Time.deltaTime * panSpeed;
            else if (aKeyDown && !dKeyDown)
                panTranslation += Vector3.left * Time.deltaTime * panSpeed;

            if (wKeyDown && !sKeyDown)
                panTranslation += Vector3.forward * Time.deltaTime * panSpeed;
            else if (sKeyDown && !wKeyDown)
                panTranslation += Vector3.back * Time.deltaTime * panSpeed;

            if (qKeyDown && !eKeyDown)
                panTranslation += Vector3.down * Time.deltaTime * panSpeed;
            else if (eKeyDown && !qKeyDown)
                panTranslation += Vector3.up * Time.deltaTime * panSpeed;

            if (Input.GetMouseButton(2)) // MMB
            {
                // Hold button and drag camera around
                panTranslation -= new Vector3(Input.GetAxis("Mouse X") * Time.deltaTime * panSpeed * Time.deltaTime, 0,
                                           Input.GetAxis("Mouse Y") * Time.deltaTime * panSpeed * Time.deltaTime);
            }
            else
            {
                // Move camera if mouse pointer reaches screen borders
                if (Input.mousePosition.x < ScrollArea)
                {
                    panTranslation += Vector3.right * -ScrollSpeed * camera.transform.position.y * Time.deltaTime;
                }

                if (Input.mousePosition.x >= Screen.width - ScrollArea)
                {
                    panTranslation += Vector3.right * ScrollSpeed * camera.transform.position.y * Time.deltaTime;
                }

                if (Input.mousePosition.y < ScrollArea)
                {
                    panTranslation += Vector3.forward * -ScrollSpeed * camera.transform.position.y * Time.deltaTime;
                }

                if (Input.mousePosition.y > Screen.height - ScrollArea)
                {
                    panTranslation += Vector3.forward * ScrollSpeed * camera.transform.position.y * Time.deltaTime;
                }
            }

            //        transform.Translate(panTranslation, Space.Self);
            // Finally move camera parallel to world axis
            Vector3 newCameraPos = camera.transform.position + panTranslation;
            if (newCameraPos.x > xMinLimit && newCameraPos.x < xMaxLimit && newCameraPos.z > zMinLimit && newCameraPos.z < zMaxLimit)
            {
                camera.transform.position = newCameraPos;
            }

            // Update panSpeed
            if (wKeyDown || aKeyDown || sKeyDown ||
                dKeyDown || qKeyDown || eKeyDown)
            {
                panT += Time.deltaTime / panTimeConstant;
                panSpeed = Mathf.Lerp(minPanSpeed, maxPanSpeed, panT * panT);
            }
            else
            {
                panT = 0;
                panSpeed = minPanSpeed;
            }

            //
            // Mouse Rotation
            if (Input.GetMouseButton(1))
            {
                // if the game window is separate from the editor window and the editor
                // window is active then you go to right-click on the game window the
                // rotation jumps if  we don't ignore the mouseDelta for that frame.
                Vector3 mouseDelta;
                if (lastMousePosition.x >= 0 &&
                    lastMousePosition.y >= 0 &&
                    lastMousePosition.x <= Screen.width &&
                    lastMousePosition.y <= Screen.height)
                    mouseDelta = Input.mousePosition - lastMousePosition;
                else
                    mouseDelta = Vector3.zero;

                var rotation = Vector3.up * Time.deltaTime * rotateSpeed * mouseDelta.x;
                rotation += Vector3.left * Time.deltaTime * rotateSpeed * mouseDelta.y;
                transform.Rotate(rotation, Space.Self);

                // Make sure z rotation stays locked
                rotation = transform.rotation.eulerAngles;
                rotation.z = 0;
                transform.rotation = Quaternion.Euler(rotation);
            }

            lastMousePosition = Input.mousePosition;

            //
            // Mouse Zoom
            float newfieldOfView = camera.fieldOfView - Input.mouseScrollDelta.y * zoomSpeed;
            if (newfieldOfView > minFieldOfView && newfieldOfView < maxFieldOfView)
            {
                camera.fieldOfView -= Input.mouseScrollDelta.y * zoomSpeed;
            }
        }
    }

}