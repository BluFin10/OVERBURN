using UnityEngine;

public class HeatManager : MonoBehaviour
{
    [SerializeField]
    public float heat;
    [SerializeField]
    private float heatMult;
    
    private DeathBox _deathBox;
    private PlayerController _playerController;
    private LevelGenerator _levelGenerator;

    [SerializeField] private float walkSpeedHeatMult;
    [SerializeField] private float dashPowerHeatMult;
    [SerializeField] private float dashChargeSpeedHeatMult;
    [SerializeField] private float jumpPowerHeatMult;
    [SerializeField] private float jumpChargeSpeedHeatMult;
    [SerializeField] private float cameraZoomHeatMult;
    [SerializeField] private float uiScaleHeatMult;
    [SerializeField] public float scoreHeatMult = 1f;
    [SerializeField] private float deathBoxHeatMult;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _deathBox = FindAnyObjectByType<DeathBox>();
        _levelGenerator = FindAnyObjectByType<LevelGenerator>();
        _playerController = FindAnyObjectByType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        heat += Time.deltaTime;
        _deathBox.followSpeed = _deathBox.minFollowSpeed + heat * deathBoxHeatMult;
        Debug.Log(heat);
    }
}
