using UnityEngine;

[System.Serializable]
public class PlayerResources : MonoBehaviour
{
    public int currentPoints = 100;

    public bool SpendPoints(int amount)
    {
        if (currentPoints >= amount)
        {
            currentPoints -= amount;
            return true;
        }
        return false;
    }
}