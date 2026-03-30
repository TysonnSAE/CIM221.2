using UnityEngine;

public class NodeConnection
{
    public Node parentNode;
    public Node childNode;
    public LineRenderer lineRenderer;

    public void UpdateMaterial(Material defaultMat, Material activeMat)
    {
        if (parentNode.isActive && childNode.isActive)
        {
            lineRenderer.material = activeMat;
        }
        else
        {
            lineRenderer.material = defaultMat;
        }
    }
}