using System.Collections.Generic;
using UnityEngine;

public class NodeGrowthSystem : MonoBehaviour
{
    [Header("Node Settings")]
    public GameObject nodePrefab;
    public Grid grid;
    public int nodesPerSpread = 2;
    public int baseSpreadCost = 5;

    [Header("Line Settings")]
    public Material lineMaterial;
    public Material activeLineMaterial;
    public float lineWidth = 0.05f;

    [Header("Player Resources")]
    public PlayerResources playerResources;

    private HashSet<Vector3Int> occupiedCells = new HashSet<Vector3Int>();
    private List<NodeConnection> connections = new List<NodeConnection>();

    private void Start()
    {
        Node centralNode = InitializeCentralNode(Vector3Int.zero);

        centralNode.isActiveForPoints = true;
        centralNode.isActiveForSpreading = false;

        Renderer rend = centralNode.GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            rend.material.color = Color.gray;
        }
    }

    public Node InitializeCentralNode(Vector3Int gridPos)
    {
        Vector3 worldPos = grid.GetCellCenterWorld(gridPos);
        GameObject centralNodeGO = Instantiate(nodePrefab, worldPos, Quaternion.identity);
        Node centralNode = centralNodeGO.GetComponent<Node>();
        centralNode.isActive = false;
        centralNode.depth = 0;
        centralNode.growthSystem = this;
        centralNode.parentNode = null;

        occupiedCells.Add(gridPos);

        Renderer rend = centralNodeGO.GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            rend.material.color = Color.gray;
        }

        return centralNode;
    }

    public void TrySpread(Node node)
    {
        if (node.isActiveForSpreading)
        {
#if UNITY_EDITOR
            Debug.Log("Node already active for spreading!");
#endif
            return;
        }

        int dynamicCost = Mathf.RoundToInt(baseSpreadCost * (1 + node.depth * 0.2f));
        if (!playerResources.SpendPoints(dynamicCost))
        {
            Debug.Log($"Not enough points to spread! Cost: {dynamicCost}");
            return;
        }

        node.isActive = true;
        node.isActiveForSpreading = true;
        node.isActiveForPoints = true;

        Renderer rend = node.GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            rend.material.color = Color.green;
        }

        SpawnNewNodes(node);
        UpdateConnectionMaterials();
    }

    private void SpawnNewNodes(Node baseNode)
    {
        Vector3Int baseCell = grid.WorldToCell(baseNode.transform.position);

        Vector3Int[] neighborOffsets = new Vector3Int[]
        {
            new Vector3Int(1,0,0),
            new Vector3Int(-1,0,0),
            new Vector3Int(0,0,1),
            new Vector3Int(0,0,-1),
            new Vector3Int(1,0,1),
            new Vector3Int(1,0,-1),
            new Vector3Int(-1,0,1),
            new Vector3Int(-1,0,-1),
        };

        List<Vector3Int> freeCells = new List<Vector3Int>();
        foreach (var offset in neighborOffsets)
        {
            Vector3Int neighbor = baseCell + offset;
            if (!occupiedCells.Contains(neighbor))
            {
                freeCells.Add(neighbor);
            }
        }

        if (freeCells.Count == 0)
        {
            Debug.Log("No free neighboring cells to spawn new nodes.");
            return;
        }

        int nodesToSpawn = Mathf.Min(nodesPerSpread, freeCells.Count);

        for (int i = 0; i < nodesToSpawn; i++)
        {
            int index = Random.Range(0, freeCells.Count);
            Vector3Int cellToSpawn = freeCells[index];
            freeCells.RemoveAt(index);

            Vector3 worldPos = grid.GetCellCenterWorld(cellToSpawn);
            GameObject newNodeGO = Instantiate(nodePrefab, worldPos, Quaternion.identity);
            Node newNode = newNodeGO.GetComponent<Node>();
            newNode.isActive = false;
            newNode.growthSystem = this;
            newNode.parentNode = baseNode;
            newNode.depth = baseNode.depth + 1;
            baseNode.childNodes.Add(newNode);

            newNode.efficiencyType = (Random.value < 0.5f) ? NodeEfficiency.ShortTerm : NodeEfficiency.LongTerm;

            occupiedCells.Add(cellToSpawn);

            Renderer rend = newNodeGO.GetComponentInChildren<Renderer>();
            if (rend != null)
            {
                rend.material.color = (newNode.efficiencyType == NodeEfficiency.ShortTerm) ? Color.red : Color.blue;
            }

            GameObject lineGO = new GameObject("ConnectionLine");
            LineRenderer lr = lineGO.AddComponent<LineRenderer>();
            lr.material = lineMaterial;
            lr.widthMultiplier = lineWidth;
            lr.positionCount = 2;
            lr.SetPosition(0, baseNode.transform.position + Vector3.up * 0.1f);
            lr.SetPosition(1, newNode.transform.position + Vector3.up * 0.1f);
            lr.useWorldSpace = true;

            connections.Add(new NodeConnection
            {
                parentNode = baseNode, childNode = newNode, lineRenderer = lr} );
            }
    }

    public void AddPoints(int amount)
    {
        playerResources.currentPoints += amount;
    }

    private void UpdateConnectionMaterials()
    {
        foreach (var conn in connections)
        {
            conn.UpdateMaterial(lineMaterial, activeLineMaterial);
        }
    }
}