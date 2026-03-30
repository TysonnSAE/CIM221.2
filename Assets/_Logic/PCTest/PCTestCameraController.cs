using UnityEngine;
using UnityEngine.InputSystem;

public class PCTestCameraController : MonoBehaviour
{
    [Header("Input")]
    public InputAction moveAction;
    public InputAction dragAction;
    public InputAction mousePosAction;

    [Header("Movement")]
    public float moveSpeed = 20f;
    public float edgeScrollSpeed = 20f;
    public float edgeSize = 10f;

    [Header("Drag")]
    public float dragSpeed = 2f;

    private Vector2 dragOrigin;

    void OnEnable()
    {
        moveAction.Enable();
        dragAction.Enable();
        mousePosAction.Enable();
    }

    void OnDisable()
    {
        moveAction.Disable();
        dragAction.Disable();
        mousePosAction.Disable();
    }

    void Update()
    {
        HandleMovement();
        //HandleDrag();
    }

    void HandleMovement()
    {
        Vector3 pos = transform.position;

        // WASD / Arrow keys
        Vector2 input = moveAction.ReadValue<Vector2>();
        pos += new Vector3(input.x, 0, input.y) * moveSpeed * Time.deltaTime;

        // Edge scrolling
        /*Vector2 mousePos = mousePosAction.ReadValue<Vector2>();

        if (mousePos.x >= Screen.width - edgeSize)
            pos.x += edgeScrollSpeed * Time.deltaTime;

        if (mousePos.x <= edgeSize)
            pos.x -= edgeScrollSpeed * Time.deltaTime;

        if (mousePos.y >= Screen.height - edgeSize)
            pos.z += edgeScrollSpeed * Time.deltaTime;

        if (mousePos.y <= edgeSize)
            pos.z -= edgeScrollSpeed * Time.deltaTime; */

        transform.position = pos;
    }

    /*void HandleDrag()
    {
        if (dragAction.WasPressedThisFrame())
        {
            dragOrigin = mousePosAction.ReadValue<Vector2>();
        }

        if (!dragAction.IsPressed()) return;

        Vector2 currentMouse = mousePosAction.ReadValue<Vector2>();
        Vector2 delta = currentMouse - dragOrigin;

        Vector3 move = new Vector3(-delta.x, 0, -delta.y) * dragSpeed * Time.deltaTime;

        transform.Translate(move, Space.World);

        dragOrigin = currentMouse; // smoother continuous drag
    }*/
}