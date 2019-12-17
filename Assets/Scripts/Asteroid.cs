using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Asteroid : MonoBehaviour
{
    [SerializeField]
    private float _rotationSpeed = 100f;
    [SerializeField]
    private GameObject _explosionPrefab;
    [SerializeField]
    private AudioSource _explosionAudioSource;

    void Start()
    {
       

        try
        {
            _explosionAudioSource = GameObject.Find("AudioManager").transform.Find("ExplosionSound").GetComponent<AudioSource>();
        }
        catch (Exception)
        {
            throw new ArgumentNullException(" Audio MAnager / Explosion Sound", "NULL, cannot find the AudioManager or  audio source of enemy explosion");
        }

    }

    // Update is called once per frame
    void Update()
    {

        transform.Rotate(Vector3.forward * _rotationSpeed * Time.deltaTime);
       
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.tag == "Laser")
        {

            _explosionPrefab = Instantiate(_explosionPrefab, transform.position,Quaternion.identity);

            if (_explosionPrefab == null)
            {
                Debug.LogError("Cannot instantiate explosion Prefab");
            }
            
            Destroy(collision.gameObject);
            Destroy (_explosionPrefab,2.5f);
            _explosionAudioSource.Play();
            SpawnManager.Instance.StartSpawning();
            Destroy(this.gameObject);

        }

    }


}
