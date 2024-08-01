using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Player player;
    private Vector3 offset;

    void Start()
    {
        player = GameObject.FindObjectOfType<Player>();
        offset = transform.position - player.transform.position;
    }

    void Update()
    {
        if (player != null)
        {
            transform.position = player.transform.position + offset;
        }
    }
}
