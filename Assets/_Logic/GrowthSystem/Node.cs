using UnityEngine;
using System.Collections.Generic;

public enum NodeEfficiency
{
    ShortTerm,
    LongTerm
}

public class Node : MonoBehaviour
{
    [HideInInspector] public NodeGrowthSystem growthSystem;
    [HideInInspector] public Node parentNode;
    public List<Node> childNodes = new List<Node>();

    [HideInInspector] public int depth = 0;
    public bool isActive = false;
    public NodeEfficiency efficiencyType = NodeEfficiency.ShortTerm;

    private float baseProduction;
    private float elapsedTime = 0f;
    private float pointAccumulator = 0f;

    [Header("Scale Animation")]
    public float minScale = 0.8f;
    public float maxScale = 1.6f;
    private Vector3 baseScale;
    public bool isActiveForPoints = false;    // produces points
    public bool isActiveForSpreading = false;

    private void Start()
    {
        baseScale = transform.localScale;

        switch (efficiencyType)
        {
            case NodeEfficiency.ShortTerm:
                baseProduction = Random.Range(5f, 10f);
                GetComponentInChildren<Renderer>().material.color = Color.red;
                break;
            case NodeEfficiency.LongTerm:
                baseProduction = Random.Range(1f, 3f);
                GetComponentInChildren<Renderer>().material.color = Color.blue;
                break;
        }
    }

    private void Update()
    {
        if (!isActiveForPoints || growthSystem == null) return;

        elapsedTime += Time.deltaTime;

        float production = 0f;
        switch (efficiencyType)
        {
            case NodeEfficiency.ShortTerm:
                production = baseProduction * Mathf.Exp(-elapsedTime * 0.05f);
                break;
            case NodeEfficiency.LongTerm:
                production = baseProduction * (1 + elapsedTime * 0.01f);
                break;
        }

        pointAccumulator += production * Time.deltaTime;

        float drainFactor = 0.2f;
        float pointsToPass = pointAccumulator * (1 - drainFactor);
        pointAccumulator *= drainFactor;

        if (parentNode != null && parentNode.isActiveForPoints)
        {
            parentNode.ReceivePoints(pointsToPass);
        }
        else
        {
            growthSystem.AddPoints(Mathf.FloorToInt(pointsToPass));
        }

        float scaleFactor = minScale + (maxScale - minScale) * Mathf.Clamp(production / 10f, 0f, 1f);
        transform.localScale = baseScale * scaleFactor;
    }

    public void ReceivePoints(float points)
    {
        if (!isActiveForPoints) return;
        pointAccumulator += points;
    }
}