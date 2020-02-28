using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform target;   // the transform (x,y position) of object to follow
    [SerializeField]
    private Vector3 offset = new Vector3(0, 0, 0);  // is the player centered in the screen? how far can we see?
    private float smoothTime = .15f;    // a variable used to smooth out the camera's velocity

    private Vector3 currentVelocity = Vector3.zero;
    private Vector3 desiredPosition;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void LateUpdate() // we use LateUpdate because player position may have moved in the same update frame
    {
        desiredPosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, smoothTime);
    }
}
