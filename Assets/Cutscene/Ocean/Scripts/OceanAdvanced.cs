﻿using UnityEngine;
using System.Collections;

public class OceanAdvanced : MonoBehaviour
{
  class Wave
  {
    public float waveLength { get; private set; }
    public float speed { get; private set; }
    public float amplitude { get; private set; }
    public float sharpness { get; private set; }
    public float frequency { get; private set; }
    public float phase { get; private set; }
    public Vector2 direction { get; private set; }

    public Wave(float waveLength, float speed, float amplitude, float sharpness, Vector2 direction)
    {
      this.waveLength = waveLength;
      this.speed = speed;
      this.amplitude = amplitude;
      this.sharpness = sharpness;
      this.direction = direction.normalized;
      frequency = (2 * Mathf.PI) / waveLength;
      phase = frequency * speed;
    }
  };

  public Material ocean;
  public Light sun;
 
  private int interaction_id = 0;
  private Vector4[] interactions = new Vector4[NB_INTERACTIONS];

  
  const int NB_WAVE = 5;
  const int NB_INTERACTIONS = 64;
    static Wave[] waves =
  {
    new Wave(10, 6.0f, 3.0f, 2.0f, new Vector2(1.0f,  0.2f)),
    new Wave(8, 7.0f, 2.8f, 1.8f, new Vector2(-1.0f,  0.8f)),
    new Wave(6, 5.5f, 2.2f, 2.2f, new Vector2(1.0f, -1.0f)),
    new Wave(5, 8.5f, 2.6f, 2.3f, new Vector2(-0.5f,  1.2f)),
    new Wave(4, 9.5f, 2.4f, 1.9f, new Vector2(0.8f,  -1.0f))
};


    void Awake()
  {
    Vector4[] v_waves = new Vector4[NB_WAVE];
    Vector4[] v_waves_dir = new Vector4[NB_WAVE];
    for (int i = 0; i < NB_WAVE; i++)
    {
      v_waves[i] = new Vector4(waves[i].frequency, waves[i].amplitude, waves[i].phase, waves[i].sharpness);
      v_waves_dir[i] = new Vector4(waves[i].direction.x, waves[i].direction.y, 0, 0);
    }

    ocean.SetVectorArray("waves_p", v_waves);
    ocean.SetVectorArray("waves_d", v_waves_dir);

    for (int i = 0; i < NB_INTERACTIONS; i++)
      interactions[i].w = 500.0F;
    ocean.SetVectorArray("interactions", interactions);
    ocean.SetVector("world_light_dir", -sun.transform.forward);
  }

  void FixedUpdate()
  {
    ocean.SetVector("world_light_dir", -sun.transform.forward);
    ocean.SetVector("sun_color", new Vector4(sun.color.r, sun.color.g, sun.color.b, 0.0F));
  }

  public void RegisterInteraction(Vector3 pos, float strength)
  {
    interactions[interaction_id].x = pos.x;
    interactions[interaction_id].y = pos.z;
    interactions[interaction_id].z = strength;
    interactions[interaction_id].w = Time.time;
    ocean.SetVectorArray("interactions", interactions);
    interaction_id = (interaction_id + 1) % NB_INTERACTIONS;
  }


  static public float GetWaterHeight(Vector3 p)
  {
    float height = 0;
    for (int i = 0; i < NB_WAVE; i++)
      height += waves[i].amplitude * Mathf.Sin(Vector2.Dot(waves[i].direction, new Vector2(p.x, p.z)) * waves[i].frequency + Time.time * waves[i].phase);
    return height;
  }
}
