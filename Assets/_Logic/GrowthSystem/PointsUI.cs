using TMPro;
using UnityEngine;

public class PointsUI : MonoBehaviour
{
    public PlayerResources playerResources;
    public TextMeshProUGUI pointsText;

    private void Update()
    {
        if (playerResources != null && pointsText != null)
        {
            pointsText.text = "Growth Points: " + Mathf.RoundToInt(playerResources.currentPoints).ToString();
        }
    }
}
