using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("References")]
    public GameObject character;
    public GameObject cameraCenter;
    public Camera cam;

    [Header("CameraSensitivity")]
    [SerializeField] public float sensitivity = 3f;

    [Header("Zoom Settings")]
    [Tooltip("FOV once zoomed")]
    [SerializeField] private float _zoomMax = 40f;

    [Tooltip("FOV when not zoomed")]
    [SerializeField] private float _zoomDefault = 60f;

    [Tooltip("Transition speed between the zoom states")]
    [SerializeField] private float _zoomSpeed = 10f;

    [Header("Collision Offset")]
    [SerializeField] private float _collisionSensitivity = 4.5f;

    private bool _isZoomed = false;
    private float _yOffset = 1f;
    private RaycastHit _camHit;
    private Vector3 _camDist;

    private void Start()
    {
        _camDist = cam.transform.localPosition;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        cameraCenter.transform.position = new Vector3(character.transform.position.x,
            character.transform.position.y + _yOffset, character.transform.position.z);

        var rotation = Quaternion.Euler(
            cameraCenter.transform.rotation.eulerAngles.x - Input.GetAxis("Mouse Y") * sensitivity / 2,
            cameraCenter.transform.rotation.eulerAngles.y + Input.GetAxis("Mouse X") * sensitivity,
            cameraCenter.transform.rotation.eulerAngles.z);
        cameraCenter.transform.rotation = rotation;

        if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonUp(1))
        {
            CheckCameraZoom();
        }

        cam.transform.localPosition = _camDist;

        GameObject obj = new GameObject();
        obj.transform.SetParent(cam.transform.parent);
        obj.transform.localPosition = new Vector3(cam.transform.localPosition.x, cam.transform.localPosition.y,
            cam.transform.localPosition.z - _collisionSensitivity);

        if (Physics.Linecast(cameraCenter.transform.position, obj.transform.position, out _camHit))
        {
            cam.transform.position = _camHit.point;

            var localPosition = new Vector3(cam.transform.localPosition.x, cam.transform.localPosition.y,
                cam.transform.localPosition.z);

            cam.transform.localPosition = localPosition;
        }

        Destroy(obj);

        if (cam.transform.localPosition.z > -1f)
        {
            cam.transform.localPosition =
                new Vector3(cam.transform.localPosition.x, cam.transform.localPosition.y, -1f);
        }
    }

    private IEnumerator ChangeFov(float startFov, float targetFov, float speed)
    {
        var t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * speed;
            cam.fieldOfView = Mathf.Lerp(startFov, targetFov, t);
            yield return null;
        }
    }

    private void CheckCameraZoom()
    {
        _isZoomed = !_isZoomed;

        float targetFov = _isZoomed ? _zoomMax : _zoomDefault;
        StartCoroutine(ChangeFov(cam.fieldOfView, targetFov, _zoomSpeed));
    }
}
