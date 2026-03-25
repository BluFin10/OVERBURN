using UnityEngine;

public class DeathBox : MonoBehaviour
{
    [SerializeField] public float followSpeed;

    [SerializeField] private float minFollowSpeed;
    [SerializeField] public float maxDistance;
    [SerializeField] private float yOffset;
    [SerializeField] private float startYOffset;
    private PlayerController _player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _player = FindAnyObjectByType<PlayerController>();
        yOffset = startYOffset;
        transform.position = new Vector3(_player.transform.position.x, _player.transform.position.y - yOffset,
            _player.transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(transform.position.y - _player.transform.position.y);
        if (_player.transform.position.y - transform.position.y > maxDistance)
        {
            transform.position = new Vector3(transform.position.x, _player.transform.position.y - (maxDistance - 1), transform.position.z);
            Debug.Log("max distance reached");
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
            Debug.Log("dead loser");
    }

    public void ResetBox()
    {
        followSpeed = minFollowSpeed;
        yOffset = startYOffset;
    }
}
