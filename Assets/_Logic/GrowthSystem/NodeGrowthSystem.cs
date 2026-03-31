using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class NodeGrowthSystem : MonoBehaviour
{
    [Header("Node Settings")]
    public GameObject nodePrefab;
    public Grid grid;
    public int baseSpreadCost = 5;
    public int baseActivationCost = 3;

    [Header("Line Settings")]
    public Material lineMaterial;
    public Material activeLineMaterial;
    public float lineWidth = 0.05f;

    [Header("Player Resources")]
    public PlayerResources playerResources;

    private HashSet<Vector3Int> occupiedCells = new HashSet<Vector3Int>();
    private List<NodeConnection> connections = new List<NodeConnection>();

    public Node rootNode;

    private void Start()
    {
        rootNode = InitializeCentralNode(Vector3Int.zero);

        rootNode.isActive = true;
        rootNode.isActiveForPoints = true;
        rootNode.isActiveForSpreading = false;
        TrySpread(rootNode);
    }

    public Node InitializeCentralNode(Vector3Int gridPos)
    {
        Vector3 worldPos = grid.GetCellCenterWorld(gridPos);
        GameObject go = Instantiate(nodePrefab, worldPos, Quaternion.identity);

        Node node = go.GetComponent<Node>();
        node.isActive = true;
        node.isActiveForPoints = true;
        node.isActiveForSpreading = false;
        node.depth = 0;
        node.growthSystem = this;



        occupiedCells.Add(gridPos);

        SetColor(go, Color.gray);

        return node;
    }

    public void TryActivateNode(Node node)
    {
        if (node.isActive) return;

        int cost = Mathf.RoundToInt(baseActivationCost * (1 + node.depth * 0.15f));

        if (!playerResources.SpendPoints(cost))
        {
            Debug.Log($"Not enough points! Need: {cost}");
            return;
        }

        ActivateNode(node);
        UpdateConnectionMaterials();
    }

    public void TrySpread(Node node)
    {
        if (!node.isActive || node.isActiveForSpreading) return;

        int cost = Mathf.RoundToInt(baseSpreadCost * (1 + node.depth * 0.2f));

        if (!playerResources.SpendPoints(cost))
        {
            Debug.Log($"Not enough points! Need: {cost}");
            return;
        }

        node.isActiveForSpreading = true;
        SetColor(node.gameObject, Color.green);

        SpawnNewNodes(node);
        UpdateConnectionMaterials();
    }

    private void SpawnNewNodes(Node baseNode)
    {
        Vector3Int baseCell = grid.WorldToCell(baseNode.transform.position);

        Vector3Int[] offsets =
        {
            new Vector3Int(1,0,0), new Vector3Int(-1,0,0),
            new Vector3Int(0,0,1), new Vector3Int(0,0,-1),
            new Vector3Int(1,0,1), new Vector3Int(1,0,-1),
            new Vector3Int(-1,0,1), new Vector3Int(-1,0,-1),
        };

        List<Vector3Int> freeCells = new List<Vector3Int>();

        foreach (var offset in offsets)
        {
            Vector3Int pos = baseCell + offset;
            if (!occupiedCells.Contains(pos))
                freeCells.Add(pos);
        }

        if (freeCells.Count == 0) return;

        int spawnCount = Mathf.Min(Random.Range(1, 3), freeCells.Count);

        for (int i = 0; i < spawnCount; i++)
        {
            int index = Random.Range(0, freeCells.Count);
            Vector3Int cell = freeCells[index];
            freeCells.RemoveAt(index);

            Vector3 worldPos = grid.GetCellCenterWorld(cell);
            GameObject go = Instantiate(nodePrefab, worldPos, Quaternion.identity);

            Node newNode = go.GetComponent<Node>();
            newNode.isActive = false;
            newNode.growthSystem = this;
            newNode.parentNode = baseNode;
            newNode.depth = baseNode.depth + 1;

            baseNode.childNodes.Add(newNode);

            newNode.efficiencyType = (Random.value < 0.5f)
                ? NodeEfficiency.ShortTerm
                : NodeEfficiency.LongTerm;

            occupiedCells.Add(cell);

            SetColor(go,
                newNode.efficiencyType == NodeEfficiency.ShortTerm
                ? Color.red
                : Color.blue);

            CreateConnection(baseNode, newNode);
        }
    }

    private void CreateConnection(Node parent, Node child)
    {
        GameObject lineGO = new GameObject("ConnectionLine");
        LineRenderer lr = lineGO.AddComponent<LineRenderer>();

        lr.material = lineMaterial;
        lr.widthMultiplier = lineWidth;
        lr.positionCount = 2;

        lr.SetPosition(0, parent.transform.position + Vector3.up * 0.1f);
        lr.SetPosition(1, child.transform.position + Vector3.up * 0.1f);

        connections.Add(new NodeConnection
        {
            parentNode = parent,
            childNode = child,
            lineRenderer = lr
        });
    }

    public void ActivateNode(Node node)
    {
        node.isActive = true;
        node.isActiveForPoints = true;

        SetColor(node.gameObject, Color.yellow);
    }

    public void AddPoints(float amount)
    {
        playerResources.currentPoints += amount;
    }

    private void UpdateConnectionMaterials()
    {
        foreach (var c in connections)
            c.UpdateMaterial(lineMaterial, activeLineMaterial);
    }

    private void SetColor(GameObject obj, Color color)
    {
        Renderer r = obj.GetComponentInChildren<Renderer>();
        if (r != null) r.material.color = color;
    }
}