using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Boundary : MonoSingleton<Boundary>
{
    private float _camDistance;
    private Vector3 _bottomCorner;
    private Vector3 _topCorner;
    public float Offset { get; set; } = 0;
 
    
    void Start()
    {
        var player = GameObject.Find("Player");
        
        if (player == null)
        {
            Debug.LogError("Gameobject named Player is not found");
        }
       
        _camDistance = Vector3.Distance(player.transform.position, Camera.main.transform.position);
        _bottomCorner = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, _camDistance));
        _topCorner = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, _camDistance));
    }

    public Vector3 GetBottomCorner()
    {
        return _bottomCorner;
    }

    public Vector3 GetTopCorner()
    {
        return _topCorner;
    }
}
