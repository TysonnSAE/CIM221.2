using UnityEngine;
using UnityEngine.InputSystem;

public class NodeInputManager : MonoBehaviour
{
    private PlayerInputActions inputActions;
    public Camera mainCamera;
    public NodeGrowthSystem growthSystem;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.PC.LeftClick.performed += OnClick;
        inputActions.PC.LeftClick.Enable();
    }

    private void OnDisable()
    {
        inputActions.PC.LeftClick.performed -= OnClick;
        inputActions.PC.LeftClick.Disable();
    }

    private void OnClick(InputAction.CallbackContext context)
    {
        // Raycast from mouse position
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Node node = hit.collider.GetComponent<Node>();
            if (node != null)
            {
                // Attempt to activate/spread this node
                growthSystem.TrySpread(node);
                node.isActiveForPoints = true;
                node.isActiveForSpreading = true;

                // Optional visual feedback for selection
                Renderer rend = node.GetComponentInChildren<Renderer>();
                if (rend != null && node.isActive)
                {
                    rend.material.color = Color.green;
                }

                Debug.Log($"Clicked Node (Depth: {node.depth}, Active: {node.isActive})");
            }
        }
    }

    private void OnDrawGizmos()
    {
        // Optional: visualize mouse ray in editor
        if (Mouse.current != null && mainCamera != null)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.yellow);
        }
    }
}