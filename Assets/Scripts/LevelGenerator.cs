using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private List<PlatformRule> platformRules;
    [SerializeField] private float despawnDist = 10f;
    [SerializeField] private float spawnDist = 50f;

    private PlayerController _playerController;
    private List<GameObject> _activePlatforms = new List<GameObject>();
    private int _lastRulePicked = -1;
    private GameObject _startPlat;
    
    [System.Serializable]
    public struct PlatformRule
    {
        public GameObject prefab;
        
        public Vector2 xRange;
        public Vector2 yRange;
        public float spawnProb;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        Random.InitState(System.DateTime.Now.Millisecond);
        _playerController = FindFirstObjectByType<PlayerController>();
        ResetLevel(Vector3.zero + new Vector3(0,10,0));
    }

    // Update is called once per frame
    void Update()
    {
        ChunkData highestChunk = _activePlatforms[^1].GetComponent<ChunkData>();
        if (_playerController.transform.position.y >= highestChunk.endPos.position.y - spawnDist)
        {
            SpawnNext();
        }
        ChunkData lowestChunk = _activePlatforms[0].GetComponent<ChunkData>();
        if (_activePlatforms[0].transform.position.y < _playerController.transform.position.y - despawnDist)
        {
            Destroy(_activePlatforms[0]);
            _activePlatforms.RemoveAt(0);
        }
    }

    private PlatformRule PickRule()
    {
        float total = 0f;
        foreach (var r in platformRules) total += r.spawnProb;

        float roll = Random.Range(0f, total);
        float cumulative = 0f;
        for (int i = 0; i < platformRules.Count; i++)
        {
            cumulative += platformRules[i].spawnProb;
            if (roll <= cumulative && i != _lastRulePicked)
            {
                _lastRulePicked = i;
                return platformRules[i];
            }
        }
        return platformRules[^1];
    }
    private void SpawnNext()
    {
        ChunkData lastChunk = _activePlatforms[^1].GetComponent<ChunkData>();

        Vector3 spawnPos = lastChunk.endPos.position;
        spawnPos.x = _playerController.transform.position.x;

        PlatformRule rule = PickRule();
        GameObject chunk = Instantiate(rule.prefab, Vector3.zero, Quaternion.identity);

        
        ChunkData newChunk = chunk.GetComponent<ChunkData>();
        chunk.transform.position = new Vector3(
            _playerController.transform.position.x,
            lastChunk.endPos.position.y - newChunk.startPos.localPosition.y,
            0f
        );
       

        _activePlatforms.Add(chunk);
    }
    public void ResetLevel(Vector3 origin)
    {
        foreach (var platform in _activePlatforms)
            Destroy(platform);
        _activePlatforms.Clear();

        PlatformRule rule = PickRule();
        GameObject chunk = Instantiate(rule.prefab, origin, Quaternion.identity);
        _activePlatforms.Add(chunk);

        GameObject startPlatform = GameObject.CreatePrimitive(PrimitiveType.Cube);
        startPlatform.transform.localScale = new Vector3(10, 1, 4);
        startPlatform.transform.position = _playerController.transform.position - new Vector3( 0,_playerController.transform.localScale.y/2,0);
    }
}
