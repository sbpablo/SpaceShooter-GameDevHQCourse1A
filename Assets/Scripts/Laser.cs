using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _speed = 10;
    private Boundary _boundary;
    void Start()
    {
        try
        {
            _boundary = GameObject.Find("BoundaryManager").GetComponent<Boundary>();
        }
        catch (Exception)
        {
            throw new ArgumentNullException("BoundaryManager", "NULL, cannot find BoundaryManager");
        }

        _boundary.Offset = 5.0f;
    }

    void Update()
    {
        transform.Translate(Vector3.up*_speed* Time.deltaTime);

        if (transform.position.y > _boundary.GetTopCorner().y + _boundary.Offset)
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
