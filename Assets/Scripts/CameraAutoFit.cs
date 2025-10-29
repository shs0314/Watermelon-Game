using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class CameraAutoFit : MonoBehaviour
{

    public float targetWidth = 9f;
    public float targetHeight = 19f;
    public float fixedOrthoSize = 9f;

    public Color letterboxColor = Color.black;
    public bool updateEveryFrame = true;

    Camera cam;
    int lastW, lastH;

    void Awake()
    {
        cam = GetComponent<Camera>();
        cam.orthographic = true;
        cam.backgroundColor = letterboxColor;
        Apply();
    }

    void OnValidate()
    {
        if (cam == null) cam = GetComponent<Camera>();
        Apply();
    }

    void Update()
    {
        if (!updateEveryFrame) return;
        if (Screen.width != lastW || Screen.height != lastH) Apply();
    }

    void Apply()
    {
        lastW = Screen.width;
        lastH = Screen.height;

        float targetAspect = targetWidth / targetHeight; // 9/19
        float windowAspect = (float)Screen.width / (float)Screen.height;

        cam.orthographicSize = fixedOrthoSize;

        if (Mathf.Approximately(windowAspect, targetAspect))
        {
            cam.rect = new Rect(0f, 0f, 1f, 1f);
        }
        else if (windowAspect > targetAspect)
        {
            float scaleWidth = targetAspect / windowAspect;
            float xOffset = (1f - scaleWidth) / 2f;
            cam.rect = new Rect(xOffset, 0f, scaleWidth, 1f);
        }
        else
        {
            float scaleHeight = windowAspect / targetAspect;
            float yOffset = (1f - scaleHeight) / 2f;
            cam.rect = new Rect(0f, yOffset, 1f, scaleHeight);
        }
    }
}