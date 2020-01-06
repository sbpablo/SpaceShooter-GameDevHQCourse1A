using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRedLaser : MonoBehaviour
{
    private float _intensity=0.1f;
  
    void Update()
    {
        transform.position = transform.parent.position + new Vector3(0,-4,0) + Random.insideUnitSphere * _intensity;
    }
}
