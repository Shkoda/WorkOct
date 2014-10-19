using UnityEngine;
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
internal class Ortho2dCamera : MonoBehaviour
{
    [SerializeField]
    private bool uniform = true;
    [SerializeField]
    private bool autoSetUniform = false;

    private void Awake()
    {
        camera.orthographic = true;

        if (uniform)
            SetUniform();
    }
    private void LateUpdate()
    {
        if (autoSetUniform && uniform)
            SetUniform();
    }
    private void SetUniform()
    {
        float orthographicSize = camera.pixelHeight / 2;
        if (orthographicSize != camera.orthographicSize)
            camera.orthographicSize = orthographicSize;
    }
}