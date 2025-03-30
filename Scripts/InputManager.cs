using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] public float Width = 5000;
    [SerializeField] public float Height = 5000;
    [SerializeField] public float ScrollSpeed = 10;

    private bool isDragging = false;
    private Vector3 lastMousePosition;
    private Plane myPlane;
    private Vector3 cameraMaxPos;
    private Vector3 cameraMinPos;

    private Ray cameraCenterRay;
    private bool zChangeDirty = false;

    // Start is called before the first frame update
    void Start()
    {
        myPlane = new Plane(Vector3.up, Vector3.zero);
        RecalculateCameraRect();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("Left Mouse Button Released");
            isDragging = false;
        }
        
        float scrollValue = Input.GetAxis("Mouse ScrollWheel");
        if (scrollValue != 0)
        {
            OnMouseScrollWheel(scrollValue);
        }


        if (isDragging)
        {
            var cam = Camera.main;
            var tRay = cam.ScreenPointToRay(lastMousePosition);
            myPlane.Raycast(tRay, out var distance);
            var tPrePointWorldPos = tRay.GetPoint(distance);
            tRay = cam.ScreenPointToRay(Input.mousePosition);
            myPlane.Raycast(tRay, out distance);
            var curPointWorldPos = tRay.GetPoint(distance);
            var translation = tPrePointWorldPos - curPointWorldPos;

            lastMousePosition = Input.mousePosition;

            translation.y = 0;
            var targetPos = cam.transform.position + translation;
            targetPos = ClampWorldPos(targetPos);
            cam.transform.Translate(targetPos - cam.transform.position, Space.World);
        }
    }

    private Vector3 ClampWorldPos(Vector3 inPos)
    {
        if (zChangeDirty)
        {
            RecalculateCameraRect();
            zChangeDirty = false;
        }

        inPos.x = Mathf.Clamp(inPos.x, cameraMinPos.x, cameraMaxPos.x);
        inPos.z = Mathf.Clamp(inPos.z, cameraMinPos.z, cameraMaxPos.z);
        return inPos;
    }

    private void OnMouseScrollWheel(float value)
    {
        var cam = Camera.main;
        var changeY = value * -1 * ScrollSpeed;
        cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y + changeY, cam.transform.position.z);
        zChangeDirty = true;
        cam.transform.position = ClampWorldPos(cam.transform.position);
    }

    private void RecalculateCameraRect()
    {
        var cam = Camera.main;
        Ray topRightRay = cam.ViewportPointToRay(new Vector3(1, 1, 0)); // 右上角
        Ray bottomLeftRay = cam.ViewportPointToRay(new Vector3(0, 0, 0)); // 左下角

        // 计算射线与平面的交点
        if (
            myPlane.Raycast(topRightRay, out float topRightDistance) &&
            myPlane.Raycast(bottomLeftRay, out float bottomLeftDistance))
        {
            // 获取交点
            Vector3 topRight = topRightRay.GetPoint(topRightDistance);
            Vector3 bottomLeft = bottomLeftRay.GetPoint(bottomLeftDistance);

            cameraCenterRay = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            float distance = 0;
            myPlane.Raycast(cameraCenterRay, out distance);
            var camPos = cam.transform.position;
            var camToRightTopOffset = new Vector3(camPos.x - topRight.x, 0, camPos.z - topRight.z);
            var camToLeftBottomOffset = new Vector3(camPos.x - bottomLeft.x, 0, camPos.z - bottomLeft.z);

            cameraMaxPos = new Vector3(Width + camToRightTopOffset.x, 0, Height + camToRightTopOffset.z);
            cameraMinPos = new Vector3(0 + camToLeftBottomOffset.x, 0, 0 + camToLeftBottomOffset.z);
        }
        else
        {
            Debug.LogError("Failed to calculate camera rect， because the ray does not intersect with the plane");
        }
    }
}