using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _speed = 10;
   
    void Start()
    {
       

        Boundary.Instance.Offset = 5.0f;
    }

    void Update()
    {
        transform.Translate(Vector3.up*_speed* Time.deltaTime);

        if (transform.position.y > Boundary.Instance.GetTopCorner().y + Boundary.Instance.Offset)
        {
           
            if (this.transform.parent!=null)
                Destroy(transform.parent.gameObject);
            else
            {
               Destroy(this.gameObject);
            }
        }
    }
}
