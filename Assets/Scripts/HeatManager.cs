using UnityEngine;

public class HeatManager : MonoBehaviour
{
    [SerializeField]
    public float heat;
    [SerializeField]
    private float heatMult;

    [SerializeField] private float walkSpeedHeatMult;
    [SerializeField] private float dashPowerHeatMult;
    [SerializeField] private float dashChargeSpeedHeatMult;
    [SerializeField] private float jumpPowerHeatMult;
    [SerializeField] private float jumpChargeSpeedHeatMult;
    [SerializeField] private float cameraZoomHeatMult;
    [SerializeField] private float uiScaleHeatMult;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
