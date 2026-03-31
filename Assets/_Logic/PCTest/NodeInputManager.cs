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
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Node node = hit.collider.GetComponent<Node>();
            if (node != null)
            {
                if (!node.isActive)
                {
                    growthSystem.TryActivateNode(node);
                }
                else if (!node.isActiveForSpreading)
                {
                    growthSystem.TrySpread(node);
                }

                Debug.Log($"Clicked Node (Depth: {node.depth}, Active: {node.isActive}, CanSpread: {!node.isActiveForSpreading})");
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (Mouse.current != null && mainCamera != null)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.yellow);
        }
    }
}