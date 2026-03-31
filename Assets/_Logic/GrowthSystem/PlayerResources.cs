using UnityEngine;

public class PlayerResources : MonoBehaviour
{
    public float currentPoints = 50f;

    public bool SpendPoints(float amount)
    {
        if (currentPoints >= amount)
        {
            currentPoints -= amount;
            return true;
        }
        return false;
    }
}