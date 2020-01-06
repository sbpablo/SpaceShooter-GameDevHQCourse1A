using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Vector3 _initialPosition;
    private Quaternion _initialRotation;
    [SerializeField] 
    private float _decreasement;
    [SerializeField]
    private float _intensity;
    private bool _isShaking;

    private void Start()
    {
        _initialPosition = transform.position;
        _initialRotation = transform.rotation;
        
;    }
    void Update()
    {
     
        
        if (!_isShaking) return;
        else
        {   
            if (_intensity > 0)
            {
                
                transform.position = _initialPosition + Random.insideUnitSphere * _intensity;
                transform.rotation = new Quaternion(
                _initialRotation.x + Random.Range(-_intensity, _intensity) * .2f,
                _initialRotation.y + Random.Range(-_intensity, _intensity) * .2f,
                _initialRotation.z + Random.Range(-_intensity, _intensity) * .2f,
                _initialRotation.w + Random.Range(-_intensity, -_intensity) * .2f);
                _intensity -= _decreasement;
            }
            else
            {
                _isShaking = false;
                transform.position = _initialPosition;
                transform.rotation = _initialRotation;
            }
        }
    }

    public void Shake()
    {
        _isShaking = true;
        _intensity = .11f;
        _decreasement = 0.002f;
    }
}
