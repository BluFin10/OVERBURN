using UnityEngine;

public class GameManager : MonoBehaviour
{
    private DeathBox _deathBox;
    private PlayerController _playerController;
    private HeatManager _heatManager;
    private LevelGenerator _levelGenerator;
    
    public Vector3 startPos;

    public Vector3 playerStartPos;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _deathBox = FindAnyObjectByType<DeathBox>();
        _heatManager = FindAnyObjectByType<HeatManager>();
        _levelGenerator = FindAnyObjectByType<LevelGenerator>();
        _playerController = FindAnyObjectByType<PlayerController>();
        
        startPos = Vector3.zero;
        playerStartPos = startPos + new Vector3(0,5,0);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayerDeath()
    {
        
    }

    public void ResetLevel()
    {
        _playerController.ResetPlayer(playerStartPos);
        _levelGenerator.ResetLevel(startPos);
        _deathBox.ResetBox();
        
    }
}
