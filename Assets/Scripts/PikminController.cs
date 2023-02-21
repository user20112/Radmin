using UnityEngine;

public class PikminController : MonoBehaviour
{
    [HideInInspector] public Vector3 hitPoint = Vector3.zero;
    [SerializeField] private Transform follow = default;
    [SerializeField] private Vector3 followOffset;
    public Transform target = default;
    [SerializeField] private Vector3 targetOffset;
    private Camera cam = default;
    private LineRenderer line = default;
    public AudioSource audio;
    private const int linePoints = 5;
    public float Radius;

    [Header("Visual")]
    public Transform visualCylinder;

    private void Start()
    {
        cam = Camera.main;
        line = GetComponentInChildren<LineRenderer>();
        line.positionCount = linePoints;
    }

    private void Update()
    {
        UpdateMousePosition();
        visualCylinder.transform.position = target.position;
    }

    private void UpdateMousePosition()
    {
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            hitPoint = hit.point;
            float x = hit.point.x - follow.position.x;
            float y = hit.point.y - follow.position.y;
            float z = hit.point.z - follow.position.z;
            float Distance = Mathf.Sqrt(x * x + z * z + y * y);
            if (Distance > Radius)
            {
                //we are outside the circle so just get the direction and max it out.
                float Divisor = Distance / Radius;
                x /= Divisor;
                z /= Divisor;
                y /= Divisor;
            }
            hit.point = new Vector3(x, y, z);
            target.position = hit.point + targetOffset + follow.position;
            target.up = Vector3.Lerp(target.up, hit.normal, .3f);
            for (int i = 0; i < linePoints; i++)
            {
                Vector3 linePos = Vector3.Lerp(follow.position + followOffset, target.position, (float)i / 5f);
                line.SetPosition(i, linePos);
            }
        }
    }
}