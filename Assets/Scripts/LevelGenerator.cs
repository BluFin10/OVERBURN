using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private List<PlatformRule> platformRules;
    [SerializeField] private float despawnDist = 10f;
    [SerializeField] private float spawnDist = 50f;

    private PlayerController _playerController;
    private List<GameObject> _activePlatforms = new List<GameObject>();
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
        PlatformRule rule = PickRule();
        Vector3 spawnPos = new Vector3(0, 0, _playerController.transform.position.z);
        GameObject chunk = Instantiate(rule.prefab, spawnPos, Quaternion.identity);
        _activePlatforms.Add(chunk);
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
        foreach (var r in platformRules)
        {
            cumulative += r.spawnProb;
            if (roll <= cumulative) return r;
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
        Vector3 c = spawnPos - newChunk.startPos.localPosition;
        chunk.transform.position = new Vector3(c.x, c.y, 0f);
       

        _activePlatforms.Add(chunk);
    }
}
