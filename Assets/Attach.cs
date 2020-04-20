using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attach : MonoBehaviour
{
    [SerializeField] Transform _attachTo;
    [SerializeField] bool _followPosition;
    [SerializeField] bool _followRotation;
    [SerializeField] Vector3 _positionOffset;
    [SerializeField] Vector3 _rotationOffset;

    void Start()
    {
        
    }

    void Update()
    {
        if (_followPosition)
        {
            transform.position = _attachTo.position + _positionOffset;
        }

        if (_followRotation)
        {
            transform.eulerAngles = _attachTo.eulerAngles + _rotationOffset;
        }
    }
}
