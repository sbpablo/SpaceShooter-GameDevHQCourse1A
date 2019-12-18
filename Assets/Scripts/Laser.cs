using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _speed = 10;
    private Vector3 localPosition;
   
    void Start()
    {
        Boundary.Instance.Offset = 5.0f; 
    }

    void Update()
    {
        transform.Translate(Vector3.up*_speed* Time.deltaTime);

        if (transform.position.y > Boundary.Instance.GetTopCorner().y + Boundary.Instance.Offset)
        {

            if (transform.parent.gameObject.tag == "TripleShotLaser")
            {
                ResetLaserPrefabPosition();
            }

             this.gameObject.SetActive(false);
        }
    }

    void ResetLaserPrefabPosition()
    {
        var parent = transform.parent.gameObject;

        parent.SetActive(false);
        parent.transform.GetChild(0).localPosition = new Vector3(0, 0.5f, 0);
        parent.transform.GetChild(1).localPosition = new Vector3(-0.78f, -0.29f, 0);
        parent.transform.GetChild(2).localPosition = new Vector3(0.78f, -0.29f, 0);

    }
}
