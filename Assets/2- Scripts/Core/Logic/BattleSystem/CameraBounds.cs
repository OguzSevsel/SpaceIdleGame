using UnityEngine;

public static class CameraBounds
{
    public static float Left;
    public static float Right;
    public static float Top;
    public static float Bottom;

    public static void Calculate(Camera cam, float maxZoomSize)
    {
        float height = maxZoomSize * 2f;
        float width = height * cam.aspect;

        Vector3 camPos = cam.transform.position;

        Left = camPos.x - width / 2f;
        Right = camPos.x + width / 2f;
        Bottom = camPos.y - height / 2f;
        Top = camPos.y + height / 2f;
    }
}
