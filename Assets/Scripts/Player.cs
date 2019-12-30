using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;




public class SpeedCoroutineParameters 
{
    public float CurrentSpeed { get; set; }
    public bool IsSpeedPowerUp { get; set; }


    public SpeedCoroutineParameters(float currentSpeed, bool isSpeedPowerUp)
    {
        CurrentSpeed = currentSpeed;
        IsSpeedPowerUp = isSpeedPowerUp;
    }
}

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 10;
    private float _minimumSpeed;
    [SerializeField]
    private float _speedMultiplier = 1.5f;
    [SerializeField]
    private float _speedIncreasedRate = 2f;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleShotPrefab;
    private bool _isTripleShotEnabled;
    private bool _isShieldEnabled;
    private bool _isSpeedEnabled;
    private bool _areMissilesEnabled;
    [SerializeField]
    private float _fireRate = 0.5f;
    private float _nextFire = 0.0f;
    [SerializeField]
    private int _lives = 3;
    [SerializeField]
    private GameObject _shieldPrefab;
    [SerializeField]
    private int _shieldLaserHitEndurance = 3;
    [SerializeField]
    private int _maxAmmoCount = 15;
    [SerializeField]
    private int _ammoCount;
    private int _score;
    private int _killCount = 0;
    private GameObject _leftEngine;
    private GameObject _rightEngine;
    [SerializeField]
    private GameObject _explosionPrefab;
    private AudioSource _laserAudioSource;
    private AudioSource _explosionAudioSource;
    private AudioSource _missileAudioSource;
    private AudioSource _damageAudioSource;
    private AudioSource _negativeMovementAudioSource;
    private CameraShake _cameraShake;
    [SerializeField]
    private GameObject _missilePrefab;
    private bool _thrusterBoost;
    private bool _CoolDownFinish = true;
    private SpeedCoroutineParameters _speedCoroutineParameters;
    private Animator _turnLeftAnimation;
    private Animator _turnRightAnimation;
    private Rigidbody2D _rb;
    private bool _negativeMovement;
    [SerializeField]
    private float _pickUpCollectSpeed = 5;


    private void OnEnable()
    {
        PowerUp.OnPowerUpOnScreen += PickupCollect;
    }

    private void OnDisable()
    {
        PowerUp.OnPowerUpOnScreen -= PickupCollect;
    }


    void Start()
    {
        transform.position = new Vector3(0, 0, 0);

        
        _leftEngine = transform.Find("LeftEngine").gameObject;
        _rightEngine = transform.Find("RigthEngine").gameObject;

        try
        {
            _laserAudioSource = GameObject.Find("AudioManager").transform.Find("LaserSound").gameObject.GetComponent<AudioSource>();
        }
        catch (Exception)
        {
            throw new ArgumentNullException("AudioManager or_laserSound", "NULL, cannot find Audio Manager/ Clip");
        }


        try
        {
            _missileAudioSource = GameObject.Find("AudioManager").transform.Find("MissileSound").gameObject.GetComponent<AudioSource>();
        }
        catch (Exception)
        {
            throw new ArgumentNullException("AudioManager or_missileSound", "NULL, cannot find Audio Manager/ Clip");
        }

        try
        {
            _explosionAudioSource = GameObject.Find("AudioManager").transform.Find("ExplosionSound").gameObject.GetComponent<AudioSource>();
        }
        catch (Exception)
        {
            throw new ArgumentNullException("AudioManager or Explosion Sound", "NULL, cannot find Audio Manager/ Clip");
        }


        try
        {
            _damageAudioSource = GameObject.Find("AudioManager").transform.Find("DamageSound").gameObject.GetComponent<AudioSource>();
        }
        catch (Exception)
        {
            throw new ArgumentNullException("AudioManager or Explosion Sound", "NULL, cannot find Audio Manager/ Clip");
        }

        try
        {
            _negativeMovementAudioSource = GameObject.Find("AudioManager").transform.Find("NegativeMovementSound").gameObject.GetComponent<AudioSource>();
        }
        catch (Exception)
        {
            throw new ArgumentNullException("AudioManager or NegativeMovementSound", "NULL, cannot find Audio Manager/ Clip");
        }

        try
        {
            _cameraShake = GameObject.FindWithTag("MainCamera").GetComponent<CameraShake>();
        }
        catch (Exception)
        {
            throw new ArgumentNullException("MainCamera", "NULL, cannot find MainCamera");
        }

       
        
        _turnLeftAnimation = GetComponent<Animator>();
        
        if (_turnLeftAnimation is null)
        {
            Debug.LogError("Player animator not found");
        }

        _turnRightAnimation = GetComponent<Animator>();

        if (_turnLeftAnimation is null)
        {
            Debug.LogError("Player animator not found");
        }


        _minimumSpeed = _speed;
        _rb = GetComponent<Rigidbody2D>();

        _ammoCount = _maxAmmoCount;
        UIManager.Instance.ShowAmmoCount(_ammoCount,_maxAmmoCount );
        UIManager.Instance.GetSlider().minValue = _minimumSpeed;
        UIManager.Instance.GetSlider().maxValue = Mathf.Max(_speed * _speedMultiplier, _speed * _speedIncreasedRate);

    }


    private void FixedUpdate()
    {
        CalculateMovement();
    }
    void Update()
    {
       /* if (Input.GetKey(KeyCode.LeftArrow))
        {
            _turnLeftAnimation.SetInteger("Tilt", -1);
        }

        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            _turnLeftAnimation.SetInteger("Tilt", 0);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            _turnLeftAnimation.SetInteger("Tilt", -1);
        }

        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            _turnLeftAnimation.SetInteger("Tilt", 0);
        }
        */

       

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _nextFire && _ammoCount>0)
        {
            Fire();
            _nextFire = Time.time + _fireRate;
            AmmoManagement();
           
        }

        if (!_isSpeedEnabled)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {

                if (_thrusterBoost == false && _CoolDownFinish ==true)
                {
                    StartCoroutine("SpeedIncrease", new SpeedCoroutineParameters(_speed, false ));
                }
             
            }

            if (Input.GetKeyUp(KeyCode.LeftShift))
            {

                if (_isSpeedEnabled == false)
                {
                    _thrusterBoost = false;
                    StopCoroutine("SpeedIncrease");
                    //_speed = _minimumSpeed;
                    StartCoroutine("SpeedDecrease");
                }
            }
        }

         UIManager.Instance.ShowThruster(_speed);    
    }


    private void PickupCollect(Transform obj)
    {
        if (Input.GetKey(KeyCode.C))
        {
            var direction = this.transform.position - obj.transform.position;
            direction.Normalize();
            obj.transform.Translate(direction * _pickUpCollectSpeed* Time.deltaTime);
        }
        
       

    }
    IEnumerator SpeedIncrease( SpeedCoroutineParameters parameters )
    {
        _thrusterBoost = true;
        _CoolDownFinish = false;
        float speedIncreaseRate;

        if (parameters.IsSpeedPowerUp==false)
        {
            speedIncreaseRate = _speedIncreasedRate; 

        } else
        {
            speedIncreaseRate = _speedMultiplier;
        }

        for ( float f=parameters.CurrentSpeed; f<= parameters.CurrentSpeed*speedIncreaseRate; f+=0.1f)
        {
            _speed = f;
            yield return null;
        }
    }

    IEnumerator SpeedDecrease()
    {
        for (float f = _speed; f >= _minimumSpeed; f -= 0.1f)
        {
            _speed = f;
            yield return null;
        }

        _CoolDownFinish = true;
    }

    private void CalculateMovement()
    {

        var horizontalImput = Input.GetAxis("Horizontal");
        _turnLeftAnimation.SetInteger("HorizontalInput",  Mathf.FloorToInt(horizontalImput));
        _turnRightAnimation.SetInteger("HorizontalInput", Mathf.CeilToInt(horizontalImput));
        var verticalImput = Input.GetAxis("Vertical");

        var movement = new Vector3(horizontalImput, verticalImput, 0);

        if (_negativeMovement)
        {
            movement *= -1;
        }
        
        _rb.velocity = (movement * _speed);
        
        //_rb.MovePosition(transform.position + movement * _speed * Time.fixedDeltaTime );

        // El objeto puede moverse en el eje X Libremente. Si pasa del límite de la pantalla, debe salir por el otro lado.

        float restrictedX = transform.position.x;


        if (transform.position.x <= Boundary.Instance.GetBottomCorner().x)
        {
            restrictedX = Boundary.Instance.GetTopCorner().x;

        }
        else if (transform.position.x >= Boundary.Instance.GetTopCorner().x)
        {
            restrictedX = Boundary.Instance.GetBottomCorner().x;
        }

        // El objeto se puede mover en el eje y hasta el limite inferior de la pantalla, sumando su volumen.  
        // Hacia arriba, solo llega hacia la mitad de la pantalla.

        var restrictedY = Mathf.Clamp(transform.position.y,
                                     Boundary.Instance.GetBottomCorner().y + this.GetComponent<SpriteRenderer>().bounds.extents.y,
                                      (Boundary.Instance.GetTopCorner().y + Boundary.Instance.GetBottomCorner().y) / 2);


        // Posicion restringida considerando tambien el tamaño del objeto
        /* var restrectedPos = new Vector3 (Mathf.Clamp(transform.position.x, _bottomCorner.x+ this.GetComponent<MeshFilter>().mesh.bounds.extents.x, _topCorner.x- this.GetComponent<MeshFilter>().mesh.bounds.extents.x), 
                                         Mathf.Clamp (transform.position.y, _bottomCorner.y + this.GetComponent<MeshFilter>().mesh.bounds.extents.y, (_topCorner.y+_bottomCorner.y)/2));
        */

        var restrectedPos = new Vector3(restrictedX, restrictedY);

        _rb.position = restrectedPos;
    }

    private void Fire()
    {

        if (_isTripleShotEnabled == false && _areMissilesEnabled == false)
        {
            FireLaser();
            

        } else if (_isTripleShotEnabled==true)
        {
            FireTripleShot();
            

        } else
        
        {
            FireMissiles();
            _missileAudioSource.Play();
        }
    }
    private void FireLaser()
    {
        var offset = new Vector3(0, 1, 0);

        var laser = PoolManager.Instance.RetrieveObjectFromPool(_laserPrefab.gameObject.tag);
        
        if (laser != null)
        {
            laser.transform.position = transform.position + offset;
            laser.SetActive(true);
            _laserAudioSource.Play();
        }
        
    

        //Instantiate(_laserPrefab, transform.position + offset, Quaternion.identity);
    }

    private void FireTripleShot()
    {
        var offset = new Vector3(0, 1, 0);

        //Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
        var tripleShot= PoolManager.Instance.RetrieveObjectFromPool(_tripleShotPrefab.gameObject.tag);
       
        if (tripleShot != null)
        {
            
            tripleShot.transform.position = transform.position;
            
            foreach (Transform child  in tripleShot.transform)
            {
                child.gameObject.SetActive(true);
            } 
            
            tripleShot.SetActive(true);
            Debug.Log(tripleShot.transform.position);
            _laserAudioSource.Play();
            Debug.Log("TripleShot shot");
            Debug.Log(tripleShot);
        }
    }
        
       

    private void FireMissiles()
    {
        Instantiate(_missilePrefab, transform.position, Quaternion.identity);
    }
    public void AmmoManagement()
    {
        _ammoCount--;
        UIManager.Instance.ShowAmmoCount(_ammoCount,_maxAmmoCount);

        if (_ammoCount <= 5 && UIManager.Instance.IsAmmoCoroutineActive == false)
        {
            UIManager.Instance.StartCoroutine("AmmoCountFlickering");
        }
        else if (_ammoCount > 5 && UIManager.Instance.IsAmmoCoroutineActive == true)  // For inspector debugging.  AmmoPowerUP stops Routine.
        {
            UIManager.Instance.StopAmmoCoroutineSecuence();
        }
    }

    public void Damage(String sourceOfDamage)
    {
        if (_isShieldEnabled == false)
        {

            _cameraShake.Shake();
            _lives -= 1;
            UIManager.Instance.SetLivesImage(_lives);

            switch (_lives)
            {
                case 2:
                    var randomEngine = UnityEngine.Random.Range(0, 2);   //Left is 0, Right is 1.
                    if (randomEngine == 0)
                    {
                        _leftEngine.SetActive(true);
                    }
                    else
                    {
                        _rightEngine.SetActive(true);
                    }
                    _damageAudioSource.Play();
                     break;

                case 1:
                    if (_leftEngine.activeSelf == true)
                    {
                        _rightEngine.SetActive(true);
                    }
                    else
                    {
                        _leftEngine.SetActive(true);
                    }
                    _damageAudioSource.Play();
                    break;

                case 0:
                    SpawnManager.Instance.OnPlayerDeath();
                    _explosionPrefab = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);

                    if (_explosionPrefab == null)
                    {
                        Debug.LogError("Cannot instantiate explosion Prefab");
                    }
                    Destroy(_explosionPrefab,2.5f);
                    _explosionAudioSource.Play();
                    Destroy(this.gameObject);
                    break; 
            }
        }
        else
        {
            if (sourceOfDamage == "Enemy")   //The enemy ship destroys the shield completely.
            {
                StopCoroutine("ShieldCoolDownRoutine");
                _isShieldEnabled = false;
                _shieldPrefab.SetActive(false);
            }
            else // its the Enemy's Laser
            {
                _shieldLaserHitEndurance--;
                
                switch (_shieldLaserHitEndurance)  
                {
                    case 2:
                        _shieldPrefab.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.66f);
                        break;
                    case 1:
                        _shieldPrefab.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.33f);
                        break;
                    case 0:
                        StopCoroutine("ShieldCoolDownRoutine");
                        _isShieldEnabled = false;
                        _shieldPrefab.SetActive(false);
                        break;

                }

            }
        }
    }

    public void OnTripleShotPowerUpCollection(float duration)
    {
        _isTripleShotEnabled = true;
        _areMissilesEnabled = false;
        StartCoroutine(TripleShotCoolDownRoutine(duration));

    }

    IEnumerator TripleShotCoolDownRoutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        _isTripleShotEnabled = false;
    }

    public void OnSpeedPowerUpCollection(float duration)
    {
        _isSpeedEnabled = true;
        StartCoroutine("SpeedIncrease",new SpeedCoroutineParameters(_speed,true));
        StartCoroutine(SpeedCoolDownRoutine(duration));

    }
    IEnumerator SpeedCoolDownRoutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        
      for (float f = _speed; f >= _minimumSpeed ; f -= 0.1f)
      {
            _speed = f;
            yield return null;
      }

        _thrusterBoost = false;
        _isSpeedEnabled = false;
        _CoolDownFinish = true;

    }
    public void OnShieldPowerUpCollection (float duration)
    {
        _isShieldEnabled = true;
        _shieldPrefab.SetActive(true);

        StartCoroutine(ShieldCoolDownRoutine(duration));
    }

    IEnumerator ShieldCoolDownRoutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        _isShieldEnabled = false;
        _shieldPrefab.SetActive(false);
    }

    public void OnNegativeMovementPowerUpCollection (float duration)
    {
        _negativeMovement = true;
        _negativeMovementAudioSource.Play();
        StartCoroutine(OnNegativeMovementCoolDownRoutine(duration));
    }

    IEnumerator OnNegativeMovementCoolDownRoutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        _negativeMovement = false;
        _negativeMovementAudioSource.Stop();
    }

    public void OnAmmoPowerUpCollection()
    {
        _ammoCount = _maxAmmoCount;
        UIManager.Instance.ShowAmmoCount(_ammoCount,_maxAmmoCount);
        UIManager.Instance.StopAmmoCoroutineSecuence();
    }
    public void OnLifePowerUpCollection()
    {
        
        if (_lives <3)
        {
            _lives++;
            UIManager.Instance.SetLivesImage(_lives);

            if (_leftEngine.activeSelf == true)
            {
                _leftEngine.SetActive(false);
            }
            else
            {
                _rightEngine.SetActive(false);
            }
        }
       
    }
    public void OnMissilePowerUpCollection(float duration)
    {
        _areMissilesEnabled = true;
        _isTripleShotEnabled = false;
        StartCoroutine(MissilesCoolDownRoutine(duration));
    }

    IEnumerator MissilesCoolDownRoutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        _areMissilesEnabled = false;
    }
    public void SetScore(int score)
    {
        _killCount += 1;
        _score += score;
        UIManager.Instance.ShowScore(_score);
    }
    public int GetScore()
    {
        return _score;
    }

    public int GetAmmoCount()
    {
        return _ammoCount;
    }

    public int GetLives()
    {
        return _lives;
    }
}