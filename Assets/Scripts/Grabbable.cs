using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabbable : MonoBehaviour
{
    private Rigidbody _rb;

    private readonly List<FingerGrabber> _fingersTouching = new List<FingerGrabber>();
    private bool _isGrabbed;

    private Vector3 _oldPos, _currentPos;
    private Vector3 _moveVel;

    private LayerMask _layerMask = 1 << 7;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (_fingersTouching.Count > 1)
        {
            if (!_isGrabbed)
            {
                for (int i = 0; i < _fingersTouching.Count; ++i)
                {
                    Physics.Linecast(_fingersTouching[0].transform.position, _fingersTouching[i].transform.position, out RaycastHit hit, _layerMask);

                    if (hit.collider && hit.collider.gameObject == gameObject)
                    {
                        Grab();
                    }
                }
            }
        }
        else if (_isGrabbed)
        {
            Drop();
        }
    }

    public static (int a, int b, int c) Test()
    {
        return (1, 2, 3);
    }

    public static (int a, int c, int e) Test2()
    {
        return (3, 4, 5);
    }

    private void FixedUpdate()
    {
        _oldPos = _currentPos;

        _moveVel = Vector3.Lerp(_moveVel, _currentPos - _oldPos, Time.fixedDeltaTime * 25f);

        _currentPos = transform.position;
    }

    public void AddFinger(FingerGrabber finger)
    {
        _fingersTouching.Add(finger);
    }

    public void RemoveFinger(FingerGrabber finger)
    {
        _fingersTouching.Remove(finger);
    }

    public bool HasFinger(FingerGrabber finger)
    {
        return _fingersTouching.Contains(finger);
    }

    public void Grab()
    {
        _isGrabbed = true;
        _rb.useGravity = false;

        Debug.Log("Grabbed " + name);
        transform.SetParent(_fingersTouching[0].transform, true);
    }

    public void Drop()
    {
        _isGrabbed = false;
        _rb.useGravity = true;

        Debug.Log("Dropped " + name);
        transform.SetParent(null);

        Throw();
    }

    public void Throw()
    {
        _rb.AddForce(_moveVel * 10f);
    }
}
