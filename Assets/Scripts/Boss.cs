using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class Boss : Enemy
{
    private Vector3 _pos;
    [SerializeField]
    private float _frequency=3;
    [SerializeField]
    private float _magnitude=3;
    private float _offset;
    private bool _moveToRightSide;
    [SerializeField]
    private Slider _healthbar;
    [SerializeField]
    private GameObject _greenBeanPrefab;
    [SerializeField]
    private GameObject _blueBeanPrefab;
    private GameObject _readLaserPrefab;
    [SerializeField]
    private float _nextShootWait=5;
    [SerializeField]
    private float _nextLaserActivationWait = 5;
    private float _laserDuration = 6;
    private float _phase = 0.0f;
    private float _frequencyOffset = 0.1f;
    private float _movementChange = 5f;
    [SerializeField]
    private GameObject _explosionPrefab;
    private bool _hasArrivedAtScene;
    private bool _hasBeenDestroyed;
    public static event Action SceneArriving;

    public static event Func<bool> Destroyed;




    private void Awake()
    {
        _healthbar = Resources.FindObjectsOfTypeAll<Slider>().First(c => c.gameObject.tag== "HealthSlider") ;
        
        if (_healthbar == null)
        {
            Debug.LogError("Healthbar slider is not assigned to the Boss");
        }

    }
    void Start()
    {
        transform.position = new Vector3((Boundary.Instance.GetBottomCorner().x + Boundary.Instance.GetTopCorner().x)/2, Boundary.Instance.GetTopCorner().y +20 , 0);
        _pos = transform.position;
        _offset = GetComponent<SpriteRenderer>().bounds.size.x/2;
        StartCoroutine(RandomSideCoroutine());

        try
        {
            _player = GameObject.Find("Player").GetComponent<Player>();
        }
        catch (Exception)
        {

            throw new ArgumentNullException("Player", "NULL, cannot find Player");
        }

        _readLaserPrefab = transform.Find ("BossRedLaser").gameObject; //laser

        if (_explosionPrefab == null)
        {
            Debug.LogError("Explosion Prefab not assigned to Boss");
        }

        try
        {
            _explosionAudioSource = GameObject.Find("AudioManager").transform.Find("ExplosionSound").GetComponent<AudioSource>();
        }
        catch (Exception)
        {
            throw new ArgumentNullException(" Audio MAnager / Explosion Sound", "NULL, cannot find the AudioManager or  audio source of enemy explosion");
        }


        StartCoroutine(ShootingRoutine());
        StartCoroutine(LaserRoutine());
        OnSceneArriving();
    }

   
    void Update()
    {
        CalculateMovement();
        AllignHealthBar();
    }

    
    private void OnDestroyed()
    {
        if (Destroyed != null)
        {
            Destroyed();
        }
    }
    
    private void OnSceneArriving()
    {
        if (SceneArriving != null)
        {
            SceneArriving();
        }
    }
    
    private void AllignHealthBar()
    {
        var pos = Camera.main.WorldToScreenPoint(this.transform.position + new Vector3(0.5f, 0.6f, 0));
        _healthbar.transform.position = pos;
    }
    
    private protected override void CalculateMovement()
    {   

        if (!_hasArrivedAtScene)
        {
            transform.Translate(Vector3.down*_speed*Time.deltaTime);
            if (transform.position.y < 3)
            {
                _hasArrivedAtScene = true;
                _pos = transform.position;
                _healthbar.gameObject.SetActive(true);
            }
        }
        else
        {

            if (_healthbar.value > 0)
            {
                Move(_moveToRightSide);
            }
            else
            {
                transform.position = transform.position + new Vector3(UnityEngine.Random.Range(-0.2f, 0.2f), UnityEngine.Random.Range(-0.2f, 0.2f), 0);

            }
        }

    }

    private  void Move (bool toRight )
    {
        
        
        
        if (toRight)
        {
           
            _pos += transform.right * Time.deltaTime * _speed;
            transform.position = _pos + transform.up * Mathf.Sin(_frequency*Time.time + _phase) * _magnitude;

            if (transform.position.x > Boundary.Instance.GetTopCorner().x + _offset)

            {
                Debug.Log(transform.position.x);
                Debug.Log($"GetTopCorner:{Boundary.Instance.GetTopCorner()} ");
                transform.position = new Vector3(Boundary.Instance.GetBottomCorner().x, transform.position.y, 0);
                _pos.x = Boundary.Instance.GetBottomCorner().x;
            }
        }
        else
        {

            
            _pos += -transform.right * Time.deltaTime * _speed;
            transform.position = _pos + transform.up * Mathf.Sin(_frequency*Time.time + _phase) * _magnitude;

            if (transform.position.x < Boundary.Instance.GetBottomCorner().x - _offset)

            {
              
                transform.position = new Vector3(Boundary.Instance.GetTopCorner().x, transform.position.y, 0);
                _pos.x = Boundary.Instance.GetTopCorner().x;
            }
        }
        
    }

    private void CalcNewFrequ()
    {
        float curr = (Time.time * _frequency + _phase) % (2.0f * Mathf.PI);
        float next = (Time.time * (_frequency+_frequencyOffset) ) % (2.0f * Mathf.PI);
        _phase = curr - next;
        _frequency+=_frequencyOffset;
    }

    IEnumerator RandomSideCoroutine()
    {

        
       while (true)
       {
            
            bool boolean = (UnityEngine.Random.value > 0.5f);
            _moveToRightSide = boolean;
            yield return new WaitForSeconds(_movementChange);

       }


    }


    private protected override void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.tag == "Laser" || collision.tag=="Missile")
        {
            if ((_healthbar.gameObject.activeSelf))
            {
                Damage();
                CalcNewFrequ();
                AdjustAttackingValues();
            }
            
            if (collision.tag == "Laser")
            {
                collision.gameObject.SetActive(false);
            }
            else
            {
               var explosion =Instantiate(_explosionPrefab, this.transform.position, Quaternion.identity);
                _explosionAudioSource.Play();
                Destroy(explosion, 2.0f);
                Destroy(collision.gameObject);
            }
            
            

        }

        if (collision.tag == "Player")
        {
            _player.Damage(this.gameObject.tag);
       
        }

    }

    
    private void Damage()
    {
        
        _healthbar.value--;
        

        if (_healthbar.value <= 0 && _hasBeenDestroyed==false)
        {

            _hasBeenDestroyed = true;
            _healthbar.gameObject.SetActive(false);
            StartCoroutine(ExplosionCoroutine());
           
           
            if (_player != null )
            {
                _player.SetScore(_scoreIfkilled);
            }

            SpawnManager.Instance.TotalEnemiesInCurrentWave--;
            OnDestroyed();
            Destroy(this.gameObject,3f);

        }
       
    }
    
    IEnumerator ExplosionCoroutine()
    {
        for (int i =0; i<=10; i++)
        {
            var explosion1 = Instantiate(_explosionPrefab, transform.position + UnityEngine.Random.insideUnitSphere * 3  , Quaternion.identity);
            _explosionAudioSource.Play();
            yield return new WaitForSeconds(0.3f);
            Destroy(explosion1, 2.5f);
        }
        
    }
    
    private void AdjustAttackingValues()
    {
        _nextShootWait -= 0.04f;
        _nextLaserActivationWait -= 0.04f;
        _magnitude += 0.03f;
        _movementChange -= 0.01f;
    }

    IEnumerator ShootingRoutine()
    {

        while (true)
        {

            Shoot();
            yield return new WaitForSeconds(_nextShootWait);
                       
        }

    }

    private void Shoot()
    {
       
        if (_player != null && _healthbar.value>0 && _hasArrivedAtScene==true)
        {
            Instantiate(_greenBeanPrefab, this.transform.position + new Vector3(0, -2f, 0), Quaternion.Euler(0, 0, -120));
            Instantiate(_blueBeanPrefab, this.transform.position + new Vector3(0, -2f, 0), Quaternion.Euler(0, 0, 120));
        }
        
    }

    IEnumerator LaserRoutine()
    {
        while (_healthbar.value>0 )
        {
            yield return new WaitForSeconds(_nextLaserActivationWait);
            _readLaserPrefab.SetActive(true);
            yield return new WaitForSeconds(_laserDuration);
            _readLaserPrefab.SetActive(false);
        }
    }


}
