using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    private HeatManager _heatManager;

    private GameManager _gameManager;

    private PlayerController _player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Image fillImage;

    void Start()
    {
        _player = FindFirstObjectByType<PlayerController>();
    }

    void Update()
    {
        float t = (_player.dashPower - _player.minDashPower) / (_player.maxDashPower - _player.minDashPower);
        fillImage.fillAmount = t;
    }
}

