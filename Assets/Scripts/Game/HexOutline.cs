using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class HexOutline : MonoBehaviour
{
    public float radius = 0.5f;
    public float yOffset = 0.01f;

    void Awake()
    {
        DrawOutline();
    }

    void DrawOutline()
    {
        LineRenderer lr = GetComponent<LineRenderer>();

        lr.useWorldSpace = false;
        lr.loop = true;

        Vector3[] points = new Vector3[6];

        for (int i = 0; i < 6; i++)
        {
            float angleDeg = 60f * i;
            float angleRad = Mathf.Deg2Rad * angleDeg;

            float x = Mathf.Cos(angleRad) * radius;
            float z = Mathf.Sin(angleRad) * radius;

            points[i] = new Vector3(x, yOffset, z);
        }

        lr.positionCount = points.Length;
        lr.SetPositions(points);
    }
}
