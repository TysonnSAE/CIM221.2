using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public bool isActive = false;
    public bool isActiveForPoints = false;
    public bool isActiveForSpreading = false;

    public NodeEfficiency efficiencyType;

    private float baseProduction;
    private float elapsedTime = 0f;
    private float storedPoints = 0f;
    private float sendTimer = 0f;

    private Vector3 originalScale;

    public int depth = 0;

    private void Start()
    {
        originalScale = transform.localScale;

        switch (efficiencyType)
        {
            case NodeEfficiency.ShortTerm:
                baseProduction = Random.Range(5f, 8f);
                break;

            case NodeEfficiency.LongTerm:
                baseProduction = Random.Range(1f, 4f);
                break;
        }
    }

    private void Update()
    {
        if (!isActiveForPoints) return;

        elapsedTime += Time.deltaTime;
        sendTimer += Time.deltaTime;

        float production = CalculateProduction();
        storedPoints += production * Time.deltaTime;

        if (sendTimer >= 1f)
        {
            PushPointsUpstream();
            sendTimer = 0f;
            StartCoroutine(Anim());
        }
    }

    float CalculateProduction()
    {
        switch (efficiencyType)
        {
            case NodeEfficiency.ShortTerm:
                return baseProduction * Mathf.Exp(-elapsedTime * 0.05f);

            case NodeEfficiency.LongTerm:
                return baseProduction * (1 + elapsedTime * 0.02f);
        }

        return 0f;
    }

    void PushPointsUpstream()
    {
        if (storedPoints <= 0f) return;

        if (parentNode != null)
        {
            float efficiency = 0.9f;
            parentNode.ReceivePoints(storedPoints * efficiency);
        }
        else
        {
            growthSystem.AddPoints(storedPoints);
        }

        storedPoints = 0f;
    }

    public void ReceivePoints(float amount)
    {
        storedPoints += amount;
    }

    private void OnMouseDown()
    {
        if (!isActive)
        {
            growthSystem.TryActivateNode(this);
        }
        else if (!isActiveForSpreading)
        {
            growthSystem.TrySpread(this);
        }
    }

    private IEnumerator Anim()
    {
        float duration = 0.2f;
        float scaleFactor = 1.3f;

        Vector3 targetScale = originalScale * scaleFactor;
        float timer = 0f;

        while (timer < duration)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        transform.localScale = targetScale;

        timer = 0f;
        while (timer < duration)
        {
            transform.localScale = Vector3.Lerp(targetScale, originalScale, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        transform.localScale = originalScale;
    }
}