using UnityEngine;

public class ChunkData : MonoBehaviour
{
    public Transform startPos;

    public Transform endPos;
    private void Awake()
    {
        startPos = transform.Find("StartPos");
        endPos = transform.Find("EndPos");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
