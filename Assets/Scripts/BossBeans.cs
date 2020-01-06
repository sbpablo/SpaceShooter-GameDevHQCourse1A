using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class BossBeans : MonoBehaviour
{
    [SerializeField]
    private float _speed;
    private Player _player;
    

    void Start()
    {

        try
        {
            _player = GameObject.Find("Player").GetComponent<Player>();
        }
        catch (Exception)
        {
            throw new ArgumentNullException("Player", "NULL, cannot find Player");
        }

    }

    void Update()
    {
       
        transform.Translate(this.transform.up * _speed * Time.deltaTime, Space.World);

        if (transform.position.y < Boundary.Instance.GetBottomCorner().y)
        {
            Destroy(this.gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            _player.Damage(this.gameObject.tag);
            //Destroy(this.gameObject);
            this.gameObject.SetActive(false);
        }
    }
}
