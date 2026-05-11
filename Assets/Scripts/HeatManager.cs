using UnityEngine;

public class HeatManager : MonoBehaviour
{
    [SerializeField] public float heat;
    [SerializeField] private float heatMult;

    private DeathBox _deathBox;
    private PlayerController _playerController;
    private LevelGenerator _levelGenerator;
    private GameManager _gameManager;
    private MusicManager _musicManager;

    [SerializeField] private float walkSpeedHeatMult;
    [SerializeField] private float dashPowerHeatMult;
    [SerializeField] private float dashChargeSpeedHeatMult;
    [SerializeField] private float jumpPowerHeatMult;
    [SerializeField] private float jumpChargeSpeedHeatMult;
    [SerializeField] private float cameraZoomHeatMult;
    [SerializeField] private float uiScaleHeatMult;
    [SerializeField] public float scoreHeatMult = 1f;
    [SerializeField] private float deathBoxHeatMult;
    [SerializeField] private float maxHeatForClarity = 300;
    [SerializeField] private float muffledFreq = 500f;
    [SerializeField] private float clearFreq = 22000f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _deathBox = FindAnyObjectByType<DeathBox>();
        _levelGenerator = FindAnyObjectByType<LevelGenerator>();
        _playerController = FindAnyObjectByType<PlayerController>();
        _gameManager = FindAnyObjectByType<GameManager>();
        _musicManager = FindAnyObjectByType<MusicManager>();
    }

    // Update is called once per frame
    void Update()
    {
        heat += Time.deltaTime;
        _deathBox.followSpeed = _deathBox.minFollowSpeed + heat * deathBoxHeatMult;
        _playerController.minDashPower = _playerController.startMinDashPower + heat * dashPowerHeatMult;
        _playerController.maxDashPower = _playerController.startMaxDashPower + heat * dashPowerHeatMult;
        _playerController.chargeRate = _playerController.minDashChargeRate + heat * dashChargeSpeedHeatMult;
        _playerController.baseJumpForce = _playerController.minBaseJumpForce + heat * jumpPowerHeatMult;
        _playerController.jumpChargeSpeed = _playerController.minJumpChargeSpeed + heat * jumpChargeSpeedHeatMult;
        _playerController.moveSpeed = _playerController.baseMoveSpeed + heat * walkSpeedHeatMult;

        _gameManager.heatedScore = _gameManager.highestScore * scoreHeatMult;

        // --- Music Unmuffling Logic ---
        // 1. Calculate progress (0.0 at start, 1.0 at 300 heat)
        float progress = Mathf.Clamp01(heat / maxHeatForClarity);

        // 2. Interpolate between muffled and clear based on progress
        float dynamicFreq = Mathf.Lerp(muffledFreq, clearFreq, progress);

        // 3. Send the value to the MusicManager
        if (_musicManager != null)
        {
            _musicManager.SetDynamicFrequency(dynamicFreq);
            Debug.Log(heat);
        }
    }
}
