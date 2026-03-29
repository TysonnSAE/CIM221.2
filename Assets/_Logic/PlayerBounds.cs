using System;
using UnityEngine;

public class PlayerBounds : MonoBehaviour
{
    [Header("Bounds")]
    [SerializeField] private Vector3 minBounds;
    [SerializeField] private Vector3 maxBounds;
    [Space(10)]

    [Header("References")]
    [SerializeField] private Transform headset;

    private void Update()
    {
        HandleBounds();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector3 center = (minBounds + maxBounds) / 2f;
        Vector3 size = maxBounds - minBounds;

        Gizmos.DrawWireCube(center, size);
    }

    private void HandleBounds()
    {
        Vector3 offset = headset.position - transform.position;
        Vector3 newRigPos = transform.position;

        newRigPos.x = Mathf.Clamp(transform.position.x, minBounds.x - offset.x, maxBounds.x - offset.x);
        newRigPos.y = Mathf.Clamp(transform.position.y, minBounds.y - offset.y, maxBounds.y - offset.y);
        newRigPos.z = Mathf.Clamp(transform.position.z, minBounds.z - offset.z, maxBounds.z - offset.z);

        transform.position = newRigPos;
    }
}
