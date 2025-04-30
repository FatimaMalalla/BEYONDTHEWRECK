using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ArchimedsLab;

[RequireComponent(typeof(MeshFilter))]
public class FloatingGameEntityRealist : GameEntity
{
    public Mesh buoyancyMesh;

    /* These 4 arrays are cache array, preventing some operations to be done each frame. */
    tri[] _triangles;
    tri[] worldBuffer;
    tri[] wetTris;
    tri[] dryTris;

    // These variables will store the number of valid triangles
    uint nbrWet, nbrDry;

    WaterSurface.GetWaterHeight realist = delegate (Vector3 pos)
    {
        const float eps = 0.1f;
        return (OceanAdvanced.GetWaterHeight(pos + new Vector3(-eps, 0F, -eps))
              + OceanAdvanced.GetWaterHeight(pos + new Vector3(eps, 0F, -eps))
              + OceanAdvanced.GetWaterHeight(pos + new Vector3(0F, 0F, eps))) / 3F;
    };

    protected override void Awake()
    {
        base.Awake();

        // Use custom buoyancy mesh or fallback to MeshFilter mesh
        Mesh m = buoyancyMesh == null ? GetComponent<MeshFilter>().mesh : buoyancyMesh;
        WaterCutter.CookCache(m, ref _triangles, ref worldBuffer, ref wetTris, ref dryTris);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (rb.IsSleeping())
            return;

        // Prepare world-space mesh
        WaterCutter.CookMesh(transform.position, transform.rotation, ref _triangles, ref worldBuffer);

        // Separate submerged and non-submerged triangles
        WaterCutter.SplitMesh(worldBuffer, ref wetTris, ref dryTris, out nbrWet, out nbrDry, realist);

        // Apply buoyancy forces
        Archimeds.ComputeAllForces(wetTris, dryTris, nbrWet, nbrDry, speed, rb);

        // 🌊 Simulate rocking motion (torque along Z axis)
        float waveIntensity = Mathf.Sin(Time.time * 1.5f) * 10f;
        rb.AddTorque(transform.forward * waveIntensity);

        // ⛵ Apply forward force and side sway like in a storm
        Vector3 stormDrift = transform.forward * 5f + transform.right * Mathf.Sin(Time.time * 0.6f) * 1.5f;
        rb.AddForce(stormDrift, ForceMode.Force);
    }

#if UNITY_EDITOR
    // Visualize triangles in scene view (optional for debugging)
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        if (!Application.isPlaying)
            return;

        Gizmos.color = Color.blue;
        for (uint i = 0; i < nbrWet; i++)
        {
            Gizmos.DrawLine(wetTris[i].a, wetTris[i].b);
            Gizmos.DrawLine(wetTris[i].b, wetTris[i].c);
            Gizmos.DrawLine(wetTris[i].a, wetTris[i].c);
        }

        Gizmos.color = Color.yellow;
        for (uint i = 0; i < nbrDry; i++)
        {
            Gizmos.DrawLine(dryTris[i].a, dryTris[i].b);
            Gizmos.DrawLine(dryTris[i].b, dryTris[i].c);
            Gizmos.DrawLine(dryTris[i].a, dryTris[i].c);
        }
    }
#endif
}
