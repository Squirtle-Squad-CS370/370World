using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform target;   // the transform (x,y position) of object to follow
    [SerializeField]
    private Vector3 offset = new Vector3(0, 0, 0);  // is the player centered in the screen? how far can we see?
    private float smoothTime = .15f;    // a variable used to smooth out the camera's velocity

    private Vector3 currentVelocity = Vector3.zero;
    private Vector3 desiredPosition;
    private float zoomSpeed = 10;
    private float targetOrtho;
    public float smoothSpeed = 2.0f;
    public float minOrtho = 1.0f;
    public float maxOrtho = 1000.0f;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        targetOrtho = Camera.main.orthographicSize;
    }

    void Update() 
    {
        //Added the ability to zoom out just so I can see what the world looks like.
        float scroll = Input.GetAxis ("Mouse ScrollWheel");
        if (Input.GetKey(KeyCode.LeftControl) && scroll != 0.0f) 
        {
            targetOrtho -= scroll * zoomSpeed;
             
            if (targetOrtho < 1) 
            {
                targetOrtho = 1;
            }
            //targetOrtho = Mathf.Clamp(targetOrtho, minOrtho, maxOrtho);
        }
         
         Camera.main.orthographicSize = Mathf.MoveTowards(Camera.main.orthographicSize, targetOrtho, smoothSpeed);
    }

    void LateUpdate() // we use LateUpdate because player position may have moved in the same update frame
    {
        desiredPosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, smoothTime);
    }
}
