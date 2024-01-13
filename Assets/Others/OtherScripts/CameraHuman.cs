using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHuman : MonoBehaviour
{
    public Transform lookAtTransform;

    public float minDistance = 3.0f;
    public float maxDistance = 8.0f;
    public float yawRotationalSpeed = 360.0f;
    public float pitchRotationalSpeed = 180.0f;
    public float minPitch = -60.0f;
    public float maxPitch = 20.0f;

    public KeyCode debugLockAngleKeyCode = KeyCode.I;
    public KeyCode debugLockKeyCode = KeyCode.O;

    public LayerMask cameraCollisionLayers;
    public float cameraCollisionOffset = 0.1f;

    [Header("Camera Restart")]
    public float timeBeforeRestartCamera = 5.0f;
    public Vector3 cameraOffset = new Vector3(0, 2.7f, -4.1f);

    float pitch = 0.0f;
    float restartCamTimer = 0;

    bool angleLocked = false;
    bool mouseLocked = true;
    bool restartingCamPos = false;

    public GameObject targetObject;

    Vector3 camStartPos;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        mouseLocked = Cursor.lockState == CursorLockMode.Locked;


        camStartPos = cameraOffset;
    }

    private void LateUpdate()
    {
#if UNITY_EDITOR
        UpdateInputDebug();
#endif

        restartCamTimer += Time.deltaTime;

        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        { restartCamTimer = 0; restartingCamPos = false; }

        float _mouseX = Input.GetAxis("Mouse X");
        float _mouseY = Input.GetAxis("Mouse Y");

        if (angleLocked)
        {
            _mouseX = 0;
            _mouseY = 0;
        }

        transform.LookAt(lookAtTransform.position);
        float _distance = Vector3.Distance(transform.position, lookAtTransform.position);
        _distance = Mathf.Clamp(_distance, minDistance, maxDistance);
        Vector3 _eulerAngles = transform.rotation.eulerAngles;
        float yaw = _eulerAngles.y;

        if (!restartingCamPos)
        {
            yaw += _mouseX * yawRotationalSpeed * Time.deltaTime;
            pitch += _mouseY * pitchRotationalSpeed * Time.deltaTime;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }

        Vector3 _fowardCamera = new Vector3(Mathf.Sin(yaw * Mathf.Deg2Rad) * Mathf.Cos(pitch * Mathf.Deg2Rad),
            Mathf.Sin(pitch * Mathf.Deg2Rad), Mathf.Cos(yaw * Mathf.Deg2Rad) * Mathf.Cos(pitch * Mathf.Deg2Rad));
        Vector3 _desiredPosition = lookAtTransform.position - _fowardCamera * _distance;

        Ray _ray = new Ray(lookAtTransform.position, -_fowardCamera);
        RaycastHit _raycastHit;

        if (Physics.Raycast(_ray, out _raycastHit, _distance, cameraCollisionLayers.value))
        {
            _desiredPosition = _raycastHit.point + _fowardCamera * cameraCollisionOffset;
        }

        transform.position = _desiredPosition;
        transform.LookAt(lookAtTransform.position);

    }

    void UpdateInputDebug()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(debugLockAngleKeyCode)) { angleLocked = !angleLocked; }

        if (Input.GetKeyDown(debugLockKeyCode))
        {
            if (Cursor.lockState == CursorLockMode.Locked) { Cursor.lockState = CursorLockMode.None; }
            else { Cursor.lockState = CursorLockMode.Locked; }

            mouseLocked = Cursor.lockState == CursorLockMode.Locked;
        }

#endif
    }

    public void SetCamera(Transform lookAt, GameObject objectTar)
    {
        lookAtTransform = lookAt;
        targetObject = objectTar;
    }

    
}
