using System.Collections;
using UnityEngine;

public abstract class Movement : MonoBehaviour
{
    #region Fields

    [SerializeField]
    protected MoveData moveData;

    public Transform planet;
    [SerializeField]
    Transform transformBody;

    protected Vector3 movementVector;
    protected Vector3 gravityDirection;
    protected float gravityStrength = 1f;
    protected Vector3 jumpVector;

    [SerializeField]
    LayerMask GroundLayerMask;
    [SerializeField]
    Transform groundCollider;

    RayData groundData;

    #endregion

    void Start()
    {
        groundData = new RayData();
    }

    void Update()
    {
        ApplyGravity();
        CheckGround();
        RotateToSurface();
        Move();

        if (Input.GetButtonDown("Jump") && groundData.grounded)
        {
            StartCoroutine(ApplyJump());
        }
    }

    void ApplyGravity()
    {
        gravityDirection = (planet.position - transform.position).normalized;

        if (!groundData.grounded)
            gravityStrength += planet.GetComponent<Planet>().GravitationalPull * Time.deltaTime;
        else
            gravityStrength = moveData.surfaceGravity;
    }

    void RotateToSurface()
    {
        Quaternion gravityRotation = Quaternion.FromToRotation(transform.up, -gravityDirection) * transform.rotation;
        Quaternion surfaceRotation = Quaternion.FromToRotation(transform.up, groundData.normal) * transform.rotation;
        Quaternion finalRotation = Quaternion.Lerp(gravityRotation, surfaceRotation, moveData.stickToSurface);

        transform.rotation = Quaternion.Slerp(transform.rotation, finalRotation, moveData.surfaceRotationSpeed * Time.deltaTime);
    }

    IEnumerator ApplyJump()
    {
        gravityStrength = 0f;
        jumpVector = Vector3.zero;

        float force = moveData.jumpForce;
        float t = 0f;

        while (t < moveData.jumpDuration)
        {
            jumpVector = -gravityDirection * force;
            force = Mathf.Lerp(moveData.jumpForce, 0f, t / moveData.jumpDuration);
            t += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }

        jumpVector = Vector3.zero;
    }

void Move()
{
    float horizontal = Input.GetAxis("Horizontal");
    float vertical = Input.GetAxis("Vertical");

    if (groundData.grounded)
    {
        movementVector = (transformBody.forward * vertical + transformBody.right * horizontal) * moveData.moveSpeed;
    }

    Vector3 movement = movementVector + jumpVector + gravityDirection * gravityStrength;

    Vector3 newPosition = transform.position + movement * Time.deltaTime;

    // Calculate planet radius (assuming uniform scale)
    float planetRadius = planet.localScale.x * 0.5f;
    // Minimum allowed distance from planet center to prevent going inside
    float minDistanceFromCenter = planetRadius + moveData.groundColSize;

    float distanceFromCenter = Vector3.Distance(newPosition, planet.position);

    if (distanceFromCenter < minDistanceFromCenter)
    {
        // Clamp position so player stays just outside planet surface + collider size
        transform.position = planet.position + (newPosition - planet.position).normalized * minDistanceFromCenter;
    }
    else
    {
        transform.position = newPosition;
    }
}

void CheckGround()
{
    if (Physics.CheckSphere(groundCollider.position, moveData.groundColSize, GroundLayerMask))
    {
        // Raycast along gravity direction to find surface normal
        if (Physics.Raycast(groundCollider.position, gravityDirection, out RaycastHit hit, 5f, GroundLayerMask))
        {
            groundData.grounded = true;
            groundData.normal = hit.normal;
            return;
        }
    }

    groundData.grounded = false;
    groundData.normal = -gravityDirection;
}

}

