using System.Collections.Generic;
using UnityEngine;

public class NodeGrowth : MonoBehaviour
{
    public GameObject nodePrefab;
    public int maxNodes = 50;
    public float spreadDistance = 2f;

    private List<GameObject> nodes = new List<GameObject>();

    void Start()
    {
        GameObject startNode = Instantiate(nodePrefab, Vector3.zero, Quaternion.identity);
        nodes.Add(startNode);

        Grow();
    }

    void Grow()
    {
        for (int i = 0; i < maxNodes; i++)
        {
            GameObject baseNode = nodes[Random.Range(0, nodes.Count)];

            Vector3 dir = Random.onUnitSphere;
            dir.y = 0;

            Vector3 newPos = baseNode.transform.position + dir * spreadDistance;

            if (Physics.CheckSphere(newPos, 0.5f))
            {
                continue;
            }

            GameObject newNode = Instantiate(nodePrefab, newPos, Quaternion.identity);
            nodes.Add(newNode);


            Debug.DrawLine(baseNode.transform.position, newPos, Color.green, 100f);
        }
    }
}