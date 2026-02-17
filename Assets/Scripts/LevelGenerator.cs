using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private List<PlatformRule> platformRules;
    [SerializeField] private float despawnDist = 10f;

    private PlayerController _playerController;
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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
