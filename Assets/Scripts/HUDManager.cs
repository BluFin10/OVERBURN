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
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI scoreTextShadow;

    void Start()
    {
        _gameManager = FindAnyObjectByType<GameManager>();
        _player = FindFirstObjectByType<PlayerController>();
        fillImage = GameObject.Find("CapsuleEmpty").GetComponent<Image>();
    }

    void Update()
    {
        float t = (_player.dashPower - _player.minDashPower) / (_player.maxDashPower - _player.minDashPower);
        fillImage.fillAmount = 1-t;
        scoreText.text = _gameManager.heatedScore.ToString();
        scoreTextShadow.text = _gameManager.heatedScore.ToString();
    }
}

