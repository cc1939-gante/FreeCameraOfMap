using UnityEngine;

[ExecuteInEditMode]
public class CameraForwardAndFrustum : MonoBehaviour
{
    public Color intersectionColor = Color.red; // 矩形的颜色
    public Color forwardColor = Color.blue;    // forward 射线的颜色
    [SerializeField]public float forwardLength = 10000000f;          // forward 射线的长度

    private Camera camera;

    void OnEnable()
    {
        camera = GetComponent<Camera>();
    }

    void OnDrawGizmos()
    {
        if (camera == null) return;

        // 定义 XZ 平面（法线为 Y 轴）
        Plane xzPlane = new Plane(Vector3.up, Vector3.zero);

        // 获取视锥体的 4 条边界射线
        Ray topLeftRay = camera.ViewportPointToRay(new Vector3(0, 1, 0));     // 左上角
        Ray topRightRay = camera.ViewportPointToRay(new Vector3(1, 1, 0));    // 右上角
        Ray bottomLeftRay = camera.ViewportPointToRay(new Vector3(0, 0, 0));  // 左下角
        Ray bottomRightRay = camera.ViewportPointToRay(new Vector3(1, 0, 0)); // 右下角

        // 计算射线与平面的交点
        if (xzPlane.Raycast(topLeftRay, out float topLeftDistance) &&
            xzPlane.Raycast(topRightRay, out float topRightDistance) &&
            xzPlane.Raycast(bottomLeftRay, out float bottomLeftDistance) &&
            xzPlane.Raycast(bottomRightRay, out float bottomRightDistance))
        {
            // 获取交点
            Vector3 topLeft = topLeftRay.GetPoint(topLeftDistance);
            Vector3 topRight = topRightRay.GetPoint(topRightDistance);
            Vector3 bottomLeft = bottomLeftRay.GetPoint(bottomLeftDistance);
            Vector3 bottomRight = bottomRightRay.GetPoint(bottomRightDistance);

            // 设置 Gizmos 的颜色
            Gizmos.color = intersectionColor;

            // 绘制矩形
            Gizmos.DrawLine(topLeft, topRight);       // 上边
            Gizmos.DrawLine(topRight, bottomRight);   // 右边
            Gizmos.DrawLine(bottomRight, bottomLeft); // 下边
            Gizmos.DrawLine(bottomLeft, topLeft);     // 左边
        }

        // 绘制摄像机的 forward 射线
        Gizmos.color = forwardColor;
        Vector3 forwardStart = transform.position;
        Vector3 forwardEnd = forwardStart + transform.forward * forwardLength;
        Gizmos.DrawLine(forwardStart, forwardEnd);
    }
}