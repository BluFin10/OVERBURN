using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private List<PlatformRule> platformRules;
    [SerializeField] private float despawnDist = 10f;
    [SerializeField] private float spawnDist = 10f;

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
        _playerController = FindFirstObjectByType<PlayerController>();
        PlatformRule rule = PickRule();
        GameObject chunk = Instantiate(rule.prefab, new Vector3(0, 0, _playerController.transform.position.z), Quaternion.identity);
        _activePlatforms.Add(chunk);
        Debug.Log("started");
        Debug.Log(_playerController.transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
