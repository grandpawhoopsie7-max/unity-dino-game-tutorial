
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [System.Serializable]
    public class MileSettings
    {
        public string mileName;

        [Min(1f)]
        public float durationSeconds = 150f;

        [Min(0f)]
        public float startingSpeed = 5f;

        [Min(0f)]
        public float speedIncreasePerSecond = 0.05f;

        [Min(0.1f)]
        public float minSpawnRate = 1.5f;

        [Min(0.1f)]
        public float maxSpawnRate = 2.5f;

        [Header("Jump Physics")]
        public float jumpForce = 8f;
        public float gravity = 24f;
    }

    [Header("Mile Configuration")]
    [SerializeField]
    private MileSettings[] miles =
    {
        new MileSettings
        {
            mileName = "MILE 1: HEAVY FOG",
            durationSeconds = 150f,
            startingSpeed = 3.5f,
            speedIncreasePerSecond = 0.015f,
            minSpawnRate = 2.0f,
            maxSpawnRate = 3.0f,
            jumpForce = 8f,
            gravity = 24f
        },

        new MileSettings
        {
            mileName = "MILE 2: THE JUNGLE",
            durationSeconds = 150f,
            startingSpeed = 5.0f,
            speedIncreasePerSecond = 0.04f,
            minSpawnRate = 1.3f,
            maxSpawnRate = 2.0f,
            jumpForce = 8f,
            gravity = 19.62f
        },

        new MileSettings
        {
            mileName = "MILE 3: THE FINAL SPRINT",
            durationSeconds = 150f,
            startingSpeed = 7.0f,
            speedIncreasePerSecond = 0.06f,
            minSpawnRate = 0.85f,
            maxSpawnRate = 1.4f,
            jumpForce = 8f,
            gravity = 19.62f
        }
    };

    [Header("New HUD")]
    [SerializeField] private MileHUD mileHUD;

    [Header("Existing UI")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI hiscoreText;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private Button retryButton;

    private Player player;
    private Spawner spawner;

    private float score;
    private float mileElapsed;
    private int currentMileIndex;
    private bool runEnded;

    public float gameSpeed { get; private set; }

    public float Score => score;

    public int CurrentMile => currentMileIndex + 1;

    public float MileTimeRemaining =>
        miles[currentMileIndex].durationSeconds - mileElapsed;

    public bool IsRunActive => !runEnded && enabled;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void Start()
    {
        player = FindObjectOfType<Player>();
        spawner = FindObjectOfType<Spawner>();

        if (mileHUD == null)
            mileHUD = FindObjectOfType<MileHUD>();

        NewGame();
    }

    public void NewGame()
    {
        ClearObstacles();

        score = 0f;
        currentMileIndex = 0;
        mileElapsed = 0f;
        runEnded = false;
        enabled = true;

        player.gameObject.SetActive(true);
        spawner.gameObject.SetActive(true);

        if (scoreText != null)
            scoreText.gameObject.SetActive(false);

        gameOverText.gameObject.SetActive(false);
        retryButton.gameObject.SetActive(false);

        player.ResetStamina();

        ApplyCurrentMile();
        UpdateHiscore();
        UpdateHud();
    }

    private void Update()
    {
        if (runEnded)
            return;

        MileSettings mile = miles[currentMileIndex];

        mileElapsed += Time.deltaTime;

        gameSpeed +=
            mile.speedIncreasePerSecond * Time.deltaTime;

        score += gameSpeed * Time.deltaTime;

        if (mileElapsed >= mile.durationSeconds)
        {
            AdvanceMile();
            return;
        }

        UpdateHud();
    }

    private void ApplyCurrentMile()
    {
        MileSettings mile = miles[currentMileIndex];

        gameSpeed = mile.startingSpeed;

        // Apply this mile's jump physics.
        player.jumpForce = mile.jumpForce;
        player.gravity = mile.gravity;

        spawner.SetSpawnRates(
            mile.minSpawnRate,
            mile.maxSpawnRate
        );
    }

    private void AdvanceMile()
    {
        ClearObstacles();

        currentMileIndex++;

        if (currentMileIndex >= miles.Length)
        {
            CompleteRun();
            return;
        }

        mileElapsed = 0f;

        ApplyCurrentMile();
        UpdateHud();
    }

    private void UpdateHud()
    {
        if (mileHUD == null)
            return;

        mileHUD.SetLevel(
            miles[currentMileIndex].mileName
        );

        mileHUD.SetTimer(MileTimeRemaining);

        if (player != null)
        {
            mileHUD.SetStamina(
                player.CurrentStamina,
                player.MaxStamina
            );
        }
    }

    public void UpdateStaminaHud(
        float currentStamina,
        float maxStamina)
    {
        if (mileHUD != null)
        {
            mileHUD.SetStamina(
                currentStamina,
                maxStamina
            );
        }
    }

    private void ClearObstacles()
    {
        // Remove all spawned obstacles.
        foreach (Obstacle obstacle in FindObjectsOfType<Obstacle>())
        {
            Destroy(obstacle.gameObject);
        }

        // Remove all spawned water bottles.
        foreach (WaterBottle bottle in FindObjectsOfType<WaterBottle>())
        {
            Destroy(bottle.gameObject);
        }
    }

    public void GameOver()
    {
        EndRun("RUN OVER");
    }

    private void CompleteRun()
    {
        EndRun("THE MILE TRIALS\nCOMPLETE!");
    }

    private void EndRun(string message)
    {
        runEnded = true;
        gameSpeed = 0f;

        player.gameObject.SetActive(false);
        spawner.gameObject.SetActive(false);

        gameOverText.text = message;
        gameOverText.gameObject.SetActive(true);

        retryButton.gameObject.SetActive(true);

        UpdateHiscore();
    }

    private void UpdateHiscore()
    {
        float hiscore =
            PlayerPrefs.GetFloat("hiscore", 0f);

        if (score > hiscore)
        {
            hiscore = score;
            PlayerPrefs.SetFloat("hiscore", hiscore);
        }

        if (hiscoreText != null)
        {
            hiscoreText.text =
    "BEST DISTANCE: " +
    Mathf.FloorToInt(hiscore).ToString("D5");
        }
    }
}
