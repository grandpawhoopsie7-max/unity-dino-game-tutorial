
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SandstormVignetteGraphic : MonoBehaviour
{
    [Header("Texture")]
    public int textureWidth = 512;
    public int textureHeight = 288;

    [Header("Clear Area Focus")]
    [Range(0f, 1f)]
    public float focusX = 0.32f;

    [Range(0f, 1f)]
    public float focusY = 0.35f;

    [Header("Vignette Shape")]
    public float clearRadius = 0.48f;
    public float fullOpacityRadius = 0.95f;

    private Image image;
    private Sprite generatedSprite;

    private void Awake()
    {
        image = GetComponent<Image>();
        image.raycastTarget = false;

        Texture2D texture = new Texture2D(
            textureWidth,
            textureHeight,
            TextureFormat.RGBA32,
            false
        );

        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Bilinear;

        float aspectRatio =
            textureWidth / (float)textureHeight;

        for (int y = 0; y < textureHeight; y++)
        {
            for (int x = 0; x < textureWidth; x++)
            {
                float normalizedX =
                    x / (float)(textureWidth - 1);

                float normalizedY =
                    y / (float)(textureHeight - 1);

                float dx = (normalizedX - focusX) * aspectRatio;
                float dy = normalizedY - focusY;

                float distance = Mathf.Sqrt(
                    dx * dx + dy * dy
                );

                float strength = Mathf.InverseLerp(
                    clearRadius,
                    fullOpacityRadius,
                    distance
                );

                strength = Mathf.SmoothStep(
                    0f,
                    1f,
                    strength
                );

                texture.SetPixel(
                    x,
                    y,
                    new Color(1f, 1f, 1f, strength)
                );
            }
        }

        texture.Apply();

        generatedSprite = Sprite.Create(
            texture,
            new Rect(0, 0, textureWidth, textureHeight),
            new Vector2(0.5f, 0.5f)
        );

        image.sprite = generatedSprite;
        image.type = Image.Type.Simple;
        image.preserveAspect = false;
    }

    private void OnDestroy()
    {
        if (generatedSprite != null)
        {
            Texture2D texture = generatedSprite.texture;

            Destroy(generatedSprite);
            Destroy(texture);
        }
    }
}
