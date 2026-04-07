using UnityEngine;

public class DeathBox : MonoBehaviour
{
    [SerializeField] public float followSpeed;

    [SerializeField] public float minFollowSpeed;
    [SerializeField] public float maxDistance;
    [SerializeField] private float yOffset;
    [SerializeField] private float startYOffset = 30;
    private PlayerController _player;

    private GameManager _gameManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _player = FindAnyObjectByType<PlayerController>();
        _gameManager = FindAnyObjectByType<GameManager>();
        yOffset = startYOffset;
        transform.position = new Vector3(_player.transform.position.x, _player.transform.position.y - yOffset,
            _player.transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
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
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
            _gameManager.PlayerDeath();
            Debug.Log("dead loser");
    }

    public void ResetBox()
    {
        yOffset = _player.transform.position.y - startYOffset;
        transform.position = new Vector3(_player.transform.position.x, yOffset, _player.transform.position.z);
        followSpeed = minFollowSpeed;
    }
}
