using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("Generator Settings")] [SerializeField]
    private List<PlatformRule> platformRules;

    [SerializeField] private float despawnDist = 10f;
    [SerializeField] private float spawnDist = 50f;

    [Header("Minimap Settings")] [SerializeField]
    private Sprite minimapSprite;

    [SerializeField] private Color minimapColor = Color.white;
    [SerializeField] private string minimapLayerName = "Minimap";
    [SerializeField] private float iconHeightOffset = 1.0f;
    [SerializeField] private float iconHorizontalOffset = 1.0f;
    [SerializeField] private float platformIconHeight = 1.0f;
    

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

    void Start()
    {
        Random.InitState(System.DateTime.Now.Millisecond);
        _playerController = FindFirstObjectByType<PlayerController>();
    }

    void Update()
    {
        // Safety check if no platforms exist yet
        if (_activePlatforms.Count == 0) return;

        ChunkData highestChunk = _activePlatforms[^1].GetComponent<ChunkData>();
        if (_playerController.transform.position.y >= highestChunk.endPos.position.y - spawnDist)
        {
            SpawnNext();
        }

        if (_activePlatforms[0].transform.position.y < _playerController.transform.position.y - despawnDist)
        {
            Destroy(_activePlatforms[0]);
            _activePlatforms.RemoveAt(0);
            if (_startPlat != null)
            {
                Destroy(_startPlat);
            }
        }
    }

    private void SpawnNext()
    {
        ChunkData lastChunk = _activePlatforms[^1].GetComponent<ChunkData>();

        PlatformRule rule = PickRule();
        GameObject chunk = Instantiate(rule.prefab, Vector3.zero, Quaternion.identity);

        ChunkData newChunk = chunk.GetComponent<ChunkData>();
        chunk.transform.position = new Vector3(
            _playerController.transform.position.x - newChunk.startPos.localPosition.x,
            lastChunk.endPos.position.y - newChunk.startPos.localPosition.y,
            0f
        );

        // --- NEW: Add icons to all platforms inside this chunk ---
        AddMinimapIcons(chunk);

        _activePlatforms.Add(chunk);
    }

    // This helper function iterates through the chunk and finds platform visuals
    private void AddMinimapIcons(GameObject chunk)
    {
        int layer = LayerMask.NameToLayer(minimapLayerName);
        Renderer[] renderers = chunk.GetComponentsInChildren<Renderer>();

        foreach (Renderer r in renderers)
        {
            if (r.gameObject.name == "MinimapIcon") continue;

            GameObject iconObj = new GameObject("MinimapIcon");
            iconObj.transform.SetParent(r.transform);
        
            SpriteRenderer sr = iconObj.AddComponent<SpriteRenderer>();
            sr.sprite = minimapSprite;
        
            // --- 9-SLICE SETTINGS ---
            sr.drawMode = SpriteDrawMode.Sliced; // Enable 9-slicing
            sr.color = (minimapColor.a == 0) ? Color.white : minimapColor;
            sr.material = new Material(Shader.Find("Sprites/Default"));
            iconObj.layer = layer;

            // 1. NEUTRALIZE SCALE
            // We set localScale to exactly (1 / ParentScale). 
            // This makes it so 1 unit of 'sr.size' equals 1 unit in the World.
            Vector3 pScale = r.transform.lossyScale;
            iconObj.transform.localScale = new Vector3(
                (pScale.x != 0) ? (1.0f / pScale.x) : 1f,
                (pScale.y != 0) ? (1.0f / pScale.y) : 1f,
                1f
            );

            // 2. SET THE SIZE (In World Units)
            // Now that scale is neutralized, we just set width and height directly.
            sr.size = new Vector2(r.bounds.size.x, platformIconHeight);

            // 3. SET THE POSITION (Intuitive Offsets)
            // Positive X = Right, Positive Y = Up
            Vector3 worldPos = r.bounds.center;
            worldPos.x += iconHorizontalOffset;
            worldPos.y += iconHeightOffset;
            worldPos.z -= 0.1f;
            iconObj.transform.position = worldPos;

            // 4. SET THE ROTATION
            iconObj.transform.eulerAngles = new Vector3(0, 0, 180f);
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

    public void ResetLevel(Vector3 origin)
    {
        foreach (var platform in _activePlatforms)
            Destroy(platform);
        _activePlatforms.Clear();

        PlatformRule rule = PickRule();
        GameObject chunk = Instantiate(rule.prefab, _playerController.transform.position, Quaternion.identity);
        
        AddMinimapIcons(chunk); // Added here for the first chunk too
        _activePlatforms.Add(chunk);

        GameObject startPlatform = GameObject.CreatePrimitive(PrimitiveType.Cube);
        startPlatform.transform.localScale = new Vector3(10, 1, 4);
        startPlatform.transform.position = _playerController.transform.position - new Vector3(0, _playerController.transform.localScale.y + 3, 0);
        
        AddMinimapIcons(startPlatform); // Add icon to the starting platform
        _startPlat = startPlatform;
    }
}