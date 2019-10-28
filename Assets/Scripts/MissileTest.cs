using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileTest : MonoBehaviour
{

    public GameObject missilePrefab;
    public GameObject player;
    private void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 80, 20), "Shoot Missile"))
        {
            ShootTest();
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShootTest()
    {
        Instantiate(missilePrefab, player.transform.position, Quaternion.identity);
    }
}
