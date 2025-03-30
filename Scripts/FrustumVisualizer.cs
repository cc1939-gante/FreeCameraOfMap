using UnityEngine;

[ExecuteInEditMode]
public class FrustumVisualizer : MonoBehaviour
{
    public Color frustumColor = Color.green; // 视锥体的颜色
    private Camera camera;

    void OnEnable()
    {
        camera = GetComponent<Camera>();
    }

    void OnDrawGizmos()
    {
        if (camera == null) return;

        // 获取摄像机的参数
        float fov = camera.fieldOfView;
        float near = camera.nearClipPlane;
        float far = camera.farClipPlane;
        float aspect = camera.aspect;

        // 计算近裁剪面和远裁剪面的高度和宽度
        float halfHeightNear = Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad) * near;
        float halfWidthNear = halfHeightNear * aspect;

        float halfHeightFar = Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad) * far;
        float halfWidthFar = halfHeightFar * aspect;

        // 定义近裁剪面和远裁剪面的顶点
        Vector3 nearTopLeft = transform.position + transform.forward * near + transform.up * halfHeightNear - transform.right * halfWidthNear;
        Vector3 nearTopRight = transform.position + transform.forward * near + transform.up * halfHeightNear + transform.right * halfWidthNear;
        Vector3 nearBottomLeft = transform.position + transform.forward * near - transform.up * halfHeightNear - transform.right * halfWidthNear;
        Vector3 nearBottomRight = transform.position + transform.forward * near - transform.up * halfHeightNear + transform.right * halfWidthNear;

        Vector3 farTopLeft = transform.position + transform.forward * far + transform.up * halfHeightFar - transform.right * halfWidthFar;
        Vector3 farTopRight = transform.position + transform.forward * far + transform.up * halfHeightFar + transform.right * halfWidthFar;
        Vector3 farBottomLeft = transform.position + transform.forward * far - transform.up * halfHeightFar - transform.right * halfWidthFar;
        Vector3 farBottomRight = transform.position + transform.forward * far - transform.up * halfHeightFar + transform.right * halfWidthFar;

        // 设置 Gizmos 的颜色
        Gizmos.color = frustumColor;

        // 绘制近裁剪面
        Gizmos.DrawLine(nearTopLeft, nearTopRight);
        Gizmos.DrawLine(nearTopRight, nearBottomRight);
        Gizmos.DrawLine(nearBottomRight, nearBottomLeft);
        Gizmos.DrawLine(nearBottomLeft, nearTopLeft);

        // 绘制远裁剪面
        Gizmos.DrawLine(farTopLeft, farTopRight);
        Gizmos.DrawLine(farTopRight, farBottomRight);
        Gizmos.DrawLine(farBottomRight, farBottomLeft);
        Gizmos.DrawLine(farBottomLeft, farTopLeft);

        // 连接近裁剪面和远裁剪面
        Gizmos.DrawLine(nearTopLeft, farTopLeft);
        Gizmos.DrawLine(nearTopRight, farTopRight);
        Gizmos.DrawLine(nearBottomLeft, farBottomLeft);
        Gizmos.DrawLine(nearBottomRight, farBottomRight);
    }
}