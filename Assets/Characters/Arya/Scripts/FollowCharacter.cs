using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCharacter : MonoBehaviour
{
    [Tooltip("Drag your player (arya) Transform here")]
    public Transform target;

    [Tooltip("Offset from the target in world?space. Tweak this in the Inspector.")]
    public Vector3 offset;

    void LateUpdate()
    {
        if (target == null) return;

        // Snap the camera to target + offset
        transform.position = target.position + offset;
    }
}
