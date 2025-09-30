using UnityEngine;

public class MouseLook : MonoBehaviour
{
    #region variables
    Vector2 _mouseFinal;
    Vector2 _smoothMouse;

    public Vector2 clampInDegrees = new Vector2(360, 180);
    public bool lockCursor = true;

    public Vector2 sensitivity = new Vector2(2, 2);
    public Vector2 smoothing = new Vector2(3, 3);
    Vector2 targetDirection;
    Vector2 targetCharacterDirection;

    public GameObject characterBody;
    #endregion

    void Start()
    {
        // Lock the cursor if enabled
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // Store the initial rotation of the camera and character
        targetDirection = transform.localRotation.eulerAngles;

        if (characterBody)
            targetCharacterDirection = characterBody.transform.localRotation.eulerAngles;
    }

    void LateUpdate()
    {
        // Read mouse delta from Input system
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        _mouseFinal += ScaleAndSmooth(mouseDelta);

        ClampValues();
        AlignToBody();
    }

    Vector2 ScaleAndSmooth(Vector2 delta)
    {
        // Scale by sensitivity * smoothing
        delta = Vector2.Scale(delta, new Vector2(sensitivity.x * smoothing.x, sensitivity.y * smoothing.y));

        // Interpolate toward delta
        _smoothMouse.x = Mathf.Lerp(_smoothMouse.x, delta.x, 1f / smoothing.x);
        _smoothMouse.y = Mathf.Lerp(_smoothMouse.y, delta.y, 1f / smoothing.y);

        return _smoothMouse;
    }

    void ClampValues()
    {
        // Clamp x rotation
        if (clampInDegrees.x < 360)
            _mouseFinal.x = Mathf.Clamp(_mouseFinal.x, -clampInDegrees.x * 0.5f, clampInDegrees.x * 0.5f);

        // Clamp y rotation
        if (clampInDegrees.y < 360)
            _mouseFinal.y = Mathf.Clamp(_mouseFinal.y, -clampInDegrees.y * 0.5f, clampInDegrees.y * 0.5f);

        var targetOrientation = Quaternion.Euler(targetDirection);
        transform.localRotation = Quaternion.AngleAxis(-_mouseFinal.y, targetOrientation * Vector3.right) * targetOrientation;
    }

    void AlignToBody()
    {
        var targetCharacterOrientation = Quaternion.Euler(targetCharacterDirection);

        if (characterBody)
        {
            var yRotation = Quaternion.AngleAxis(_mouseFinal.x, Vector3.up);
            characterBody.transform.localRotation = yRotation * targetCharacterOrientation;
        }
        else
        {
            var yRotation = Quaternion.AngleAxis(_mouseFinal.x, transform.InverseTransformDirection(Vector3.up));
            transform.localRotation *= yRotation;
        }
    }
}
