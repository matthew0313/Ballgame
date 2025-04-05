using UnityEngine;

public class FollowingCam : MonoBehaviour
{
    [SerializeField] float speed = 1.0f;
    Transform player;
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    private void Start()
    {
        transform.position = player.position;
        transform.position += new Vector3(0.0f, 0.0f, -10.0f);
    }
    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.Lerp(transform.position, player.position, speed * Time.deltaTime);
        transform.position += new Vector3(0.0f, 0.0f, -10.0f);
    }
}
