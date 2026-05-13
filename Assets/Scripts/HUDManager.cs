using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    private HeatManager _heatManager;

    private GameManager _gameManager;

    private PlayerController _player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Image fillImage;
    public Image fillImageHeat;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI scoreTextShadow;
    [SerializeField] private float heatCapsuleMax = 500;
    void Start()
    {
        _gameManager = FindAnyObjectByType<GameManager>();
        _heatManager = FindAnyObjectByType<HeatManager>();
        _player = FindFirstObjectByType<PlayerController>();
        fillImage = GameObject.Find("CapsuleEmpty").GetComponent<Image>();
        fillImageHeat = GameObject.Find("CapsuleBackround").GetComponent<Image>();
    }

    void Update()
    {
        float t = (_player.dashPower - _player.minDashPower) / (_player.maxDashPower - _player.minDashPower);
        fillImage.fillAmount = 1-t;
        // --- Heat Capsule Logic (New) ---
        // Calculate 0 to 1 progress based on heatCapsuleMax
        float heatProgress = ((_heatManager.heat / heatCapsuleMax) - 1)*-1;
        // Apply to fill amount (Clamped so it doesn't break if heat > max)
        fillImageHeat.fillAmount = Mathf.Clamp01(heatProgress);
        scoreText.text = _gameManager.heatedScore.ToString();
        scoreTextShadow.text = _gameManager.heatedScore.ToString();
        Debug.Log(fillImageHeat);
        Debug.Log(fillImageHeat.fillAmount);
    }
}

