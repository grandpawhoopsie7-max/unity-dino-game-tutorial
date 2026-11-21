using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class SandstormParticles : MonoBehaviour
{
    [Header("Particle Spawn")]
    public float minimumSpawnDelay = 0.025f;
    public float maximumSpawnDelay = 0.06f;

    [Header("Particle Appearance")]
    public float minimumParticleWidth = 30f;
    public float maximumParticleWidth = 100f;

    public float minimumParticleHeight = 2f;
    public float maximumParticleHeight = 6f;

    [Header("Particle Movement")]
    public float minimumSpeed = 500f;
    public float maximumSpeed = 1000f;

    private class SandParticle
    {
        public RectTransform rect;
        public Image image;
        public Vector2 velocity;
        public float lifetime;
        public float age;
        public float baseAlpha;
    }

    private readonly List<SandParticle> particles =
        new List<SandParticle>();

    private RectTransform overlayRect;
    private CanvasGroup canvasGroup;

    private Sprite particleSprite;
    private float spawnTimer;

    private void Awake()
    {
        overlayRect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        particleSprite = Sprite.Create(
            Texture2D.whiteTexture,
            new Rect(0, 0, 1, 1),
            new Vector2(0.5f, 0.5f)
        );
    }

    private void Update()
    {
        UpdateParticles();

        // Only create particles while the storm is visible.
        if (canvasGroup.alpha > 0.02f)
        {
            spawnTimer -= Time.deltaTime;

            if (spawnTimer <= 0f)
            {
                SpawnParticle();

                spawnTimer = Random.Range(
                    minimumSpawnDelay,
                    maximumSpawnDelay
                );
            }
        }
    }

    private void SpawnParticle()
    {
        float screenWidth = overlayRect.rect.width;
        float screenHeight = overlayRect.rect.height;

        GameObject particleObject = new GameObject(
            "SandParticle",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Image)
        );

        RectTransform rect =
            particleObject.GetComponent<RectTransform>();

        rect.SetParent(transform, false);

        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);

        float width = Random.Range(
            minimumParticleWidth,
            maximumParticleWidth
        );

        float height = Random.Range(
            minimumParticleHeight,
            maximumParticleHeight
        );

        rect.sizeDelta = new Vector2(width, height);

        rect.anchoredPosition = new Vector2(
            screenWidth / 2f + 60f,
            Random.Range(-screenHeight / 2f, screenHeight / 2f)
        );

        rect.localRotation = Quaternion.Euler(
            0f,
            0f,
            Random.Range(-8f, 8f)
        );

        Image image = particleObject.GetComponent<Image>();

        image.sprite = particleSprite;
        image.raycastTarget = false;

        float alpha = Random.Range(0.65f, 1f);

        image.color = new Color(
            Random.Range(0.65f, 0.85f),
            Random.Range(0.48f, 0.68f),
            Random.Range(0.28f, 0.45f),
            alpha
        );

        SandParticle particle = new SandParticle();

        particle.rect = rect;
        particle.image = image;

        particle.velocity = new Vector2(
            -Random.Range(minimumSpeed, maximumSpeed),
            Random.Range(-35f, 35f)
        );

        particle.lifetime = Random.Range(1f, 2f);
        particle.age = 0f;
        particle.baseAlpha = alpha;

        particles.Add(particle);
    }

    private void UpdateParticles()
    {
        for (int i = particles.Count - 1; i >= 0; i--)
        {
            SandParticle particle = particles[i];

            if (particle.rect == null)
            {
                particles.RemoveAt(i);
                continue;
            }

            particle.age += Time.deltaTime;

            particle.rect.anchoredPosition +=
                particle.velocity * Time.deltaTime;

            float fade = 1f -
                Mathf.Clamp01(particle.age / particle.lifetime);

            Color color = particle.image.color;

            color.a = particle.baseAlpha * fade;

            particle.image.color = color;

            if (particle.age >= particle.lifetime ||
                particle.rect.anchoredPosition.x <
                -overlayRect.rect.width / 2f - 100f)
            {
                Destroy(particle.rect.gameObject);
                particles.RemoveAt(i);
            }
        }
    }

    private void OnDisable()
    {
        foreach (SandParticle particle in particles)
        {
            if (particle.rect != null)
            {
                Destroy(particle.rect.gameObject);
            }
        }

        particles.Clear();
        spawnTimer = 0f;
    }

    private void OnDestroy()
    {
        if (particleSprite != null)
        {
            Destroy(particleSprite);
        }
    }
}
