using UnityEngine;

[ExecuteInEditMode]
public class GroundCheck : MonoBehaviour
{
    [Tooltip("Maximum distance from the ground.")]
    public float distanceThreshold = 10f;

    [Tooltip("Whether this transform is grounded now.")]
    public bool isGrounded = true;

    [Tooltip("LayerMask for what counts as ground.")]
    public LayerMask GroundLayerMask;

    [Tooltip("Ground collider position (e.g., feet position).")]
    public Transform groundCollider;

    [Tooltip("Sphere check radius.")]
    public float groundColSize = 10f;

    [Tooltip("Gravity direction (towards planet center).")]
    public Vector3 gravityDirection = Vector3.down;

    public Vector3 groundNormal = Vector3.up;

    public event System.Action Grounded;

    const float OriginOffset = 0.001f;

    void LateUpdate()
    {
        CheckGround();
    }

    void DebugGround()
{
    if (groundCollider == null)
    {
        Debug.LogWarning("GroundCollider transform is not assigned!");
        isGrounded = false;
        return;
    }

    // Existing check code...
}

    void CheckGround()
    {
        if (Physics.CheckSphere(groundCollider.position, groundColSize, GroundLayerMask))
        {
            if (Physics.Raycast(groundCollider.position, -gravityDirection.normalized, out RaycastHit hit, distanceThreshold + 1f, GroundLayerMask))
            {
                if (!isGrounded)
                {
                    Grounded?.Invoke();
                }

                isGrounded = true;
                groundNormal = hit.normal;
                return;
            }
        }

        isGrounded = false;
        groundNormal = -gravityDirection.normalized;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCollider != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCollider.position, groundColSize);

            Debug.DrawLine(groundCollider.position, groundCollider.position - gravityDirection.normalized * (distanceThreshold + 1f), isGrounded ? Color.white : Color.red);
        }
    }
}
