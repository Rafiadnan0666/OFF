using UnityEngine;

public class GrapplingGun : MonoBehaviour
{

    private LineRenderer lr;
    private Vector3 grapplePoint;
    public LayerMask whatIsGrappleable;
    public Transform gunTip, camera, player;
    private float maxDistance = 100f;
    private SpringJoint joint;
    private bool isEquipped;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        if (lr == null)
        {
            Debug.LogError("LineRenderer component is missing.");
        }
    }

    void Update()
    {
        if (isEquipped)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("Grapple button pressed.");
                StartGrapple();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                Debug.Log("Grapple button released.");
                StopGrapple();
            }
        }
    }

    void LateUpdate()
    {
        DrawRope();
    }

    public void Equip(bool equipStatus)
    {
        isEquipped = equipStatus;
        if (!isEquipped && joint)
        {
            StopGrapple(); 
        }
    }

    void StartGrapple()
    {
        RaycastHit hit;
        if (Physics.Raycast(camera.position, camera.forward, out hit, maxDistance, whatIsGrappleable))
        {
            Debug.Log("Grapple point found: " + hit.point);
            grapplePoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;

            joint.spring = 4.5f;
            joint.damper = 7f;
            joint.massScale = 4.5f;

            lr.positionCount = 2;
            currentGrapplePosition = gunTip.position;
        }
        else
        {
            Debug.Log("No grapple point found.");
        }
    }

    void StopGrapple()
    {
        lr.positionCount = 0;
        if (joint)
        {
            Destroy(joint);
        }
    }

    private Vector3 currentGrapplePosition;

    void DrawRope()
    {
        if (!joint) return;

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 8f);

        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, currentGrapplePosition);
    }

    public bool IsGrappling()
    {
        return joint != null;
    }

    public Vector3 GetGrapplePoint()
    {
        return grapplePoint;
    }
}
