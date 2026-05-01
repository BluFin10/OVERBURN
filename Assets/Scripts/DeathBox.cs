using UnityEngine;

public class DeathBox : MonoBehaviour
{
    [Header("Settings")]
    public float followSpeed;
    public float minFollowSpeed;
    public float maxDistance;
    [SerializeField] private float startYOffset = 30;
    
    [Header("Visuals - Body")]
    [SerializeField] private Sprite bodyTiledSprite;
    [SerializeField] private float bodyTileHeight = 500f;

    [Header("Visuals - Top Edge")]
    [SerializeField] private Sprite topEdgeSprite;
    [SerializeField] private float topEdgeTextureScale = 1f; 
    [SerializeField] private float topEdgeHeight = 2f;

    [Header("Layers")]
    [SerializeField] private string minimapLayerName = "Minimap";

    private PlayerController _player;
    private GameManager _gameManager;

    void Start()
    {
        _player = FindAnyObjectByType<PlayerController>();
        _gameManager = FindAnyObjectByType<GameManager>();
        
        // Initial Position
        transform.position = new Vector3(_player.transform.position.x, _player.transform.position.y - startYOffset, _player.transform.position.z);
        
        // Create visuals once at start
        UpdateVisuals();
    }

    void Update()
    {
        // Smooth Follow Logic
        if (_player.transform.position.y - transform.position.y > maxDistance)
        {
            transform.position = new Vector3(transform.position.x, _player.transform.position.y - (maxDistance - 1), transform.position.z);
        }
        else if (transform.position.y < _player.transform.position.y)
        {
            float newY = Mathf.MoveTowards(transform.position.y, _player.transform.position.y,
                Mathf.Max(minFollowSpeed, followSpeed) * Time.deltaTime);
            transform.position = new Vector3(_player.transform.position.x, newY, _player.transform.position.z);
        }
    }

    public void ResetBox()
    {
        transform.position = new Vector3(_player.transform.position.x, _player.transform.position.y - startYOffset, _player.transform.position.z);
        followSpeed = minFollowSpeed;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        // Get the actual world width of the DeathBox
        float worldWidth = transform.lossyScale.x;
        float boxHalfHeight = transform.localScale.y / 2f;

        // 1. Setup Body (Hanging down from the middle/top)
        HandleTiledSprite("BodySprite", bodyTiledSprite, new Vector2(worldWidth, bodyTileHeight), 1f, 0, boxHalfHeight);
        
        // 2. Setup Top Edge (Sitting exactly on the top edge)
        HandleTiledSprite("TopEdgeSprite", topEdgeSprite, new Vector2(worldWidth, topEdgeHeight), topEdgeTextureScale, 1, boxHalfHeight);
    }

    private void HandleTiledSprite(string objName, Sprite sprite, Vector2 targetSize, float texScale, int sortOrder, float yOffset)
    {
        if (sprite == null) return;

        Transform t = transform.Find(objName);
        if (t == null)
        {
            GameObject go = new GameObject(objName);
            go.transform.SetParent(this.transform);
            t = go.transform;
            t.gameObject.layer = LayerMask.NameToLayer(minimapLayerName);
            go.AddComponent<SpriteRenderer>().drawMode = SpriteDrawMode.Tiled;
        }

        SpriteRenderer sr = t.GetComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingOrder = sortOrder;

        // FIX: Remove parent scale influence
        t.localScale = new Vector3(1f / transform.localScale.x * texScale, 1f / transform.localScale.y * texScale, 1f);
        
        // Set renderer size (compensated for the scale)
        sr.size = new Vector2(targetSize.x / texScale, targetSize.y / texScale);

        // Position it at the top of the Parent Box
        t.localPosition = new Vector3(0, yOffset, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            _gameManager.PlayerDeath();
        }
    }
}