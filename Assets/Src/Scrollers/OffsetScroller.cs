using UnityEngine;
using System.Collections;

public class OffsetScroller : MonoBehaviour
{
    public float scrollSpeed;
    private Vector2 savedOffset;

    private void Start()
    {
        savedOffset = renderer.sharedMaterial.GetTextureOffset("_MainTex");
    }

    private void Update()
    {
//        float y = Mathf.Repeat(Time.time * scrollSpeed, 1);
//        Vector2 offset = new Vector2(savedOffset.x, y);

        float x = Mathf.Repeat(Time.time*scrollSpeed, 1);
        Vector2 offset = new Vector2(x, savedOffset.y);
        renderer.sharedMaterial.SetTextureOffset("_MainTex", offset);
    }

    private void OnDisable()
    {
        renderer.sharedMaterial.SetTextureOffset("_MainTex", savedOffset);
    }
}