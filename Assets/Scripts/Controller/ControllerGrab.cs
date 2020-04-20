using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ControllerGrab : MonoBehaviour
{
    [SerializeField] private SteamVR_Input_Sources _handType;
    [SerializeField] private SteamVR_Behaviour_Pose _controllerPose;
    [SerializeField] private SteamVR_Action_Boolean _grabAction;
    [SerializeField] private Material _outline;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private Transform _rigTransform;
    private GameObject _collidingObject;
    private GameObject _hitObject;
    private GameObject _objectInHand;

    void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(_controllerPose.transform.position, transform.forward, out hit, 100f, _layerMask))
        {
            if (!ReferenceEquals(_hitObject, hit.collider.gameObject))
            {
                if (_hitObject != null)
                    Highlight(_hitObject, false);

                Highlight(hit.collider.gameObject, true);
            }

            _hitObject = hit.collider.gameObject;
        }
        else
        {
            if (_hitObject)
            {
                Highlight(_hitObject, false);

                _hitObject = null;
            }
        }

        if (_grabAction.GetLastStateDown(_handType))
        {
            if (_collidingObject && InLayerMask(_collidingObject))
            {
                Highlight(_hitObject, false);

                GrabObject();
            }
        }
        else if (_grabAction.GetLastStateUp(_handType))
        {
            if (_objectInHand)
            {
                ReleaseObject();
            }
        }
    }

    private void SetCollidingObject(Collider col)
    {
        if (_collidingObject || !col.GetComponent<Rigidbody>())
        {
            return;
        }

        _collidingObject = col.gameObject;
    }

    public void OnTriggerStay(Collider other)
    {
        if (InLayerMask(other.gameObject))
        {
            SetCollidingObject(other);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (InLayerMask(other.gameObject))
        {
            _collidingObject = null;
        }
    }

    private void GrabObject()
    {
        _objectInHand = _collidingObject;
        _collidingObject = null;

        var joint = AddFixedJoint();
        joint.connectedBody = _objectInHand.GetComponent<Rigidbody>();
    }

    private FixedJoint AddFixedJoint()
    {
        FixedJoint fx = gameObject.AddComponent<FixedJoint>();
        fx.breakForce = 20000;
        fx.breakTorque = 20000;

        return fx;
    }

    private void ReleaseObject()
    {
        if (GetComponent<FixedJoint>())
        {
            GetComponent<FixedJoint>().connectedBody = null;
            Destroy(GetComponent<FixedJoint>());

            _objectInHand.GetComponent<Rigidbody>().velocity = _rigTransform.TransformVector(_controllerPose.GetVelocity());
            _objectInHand.GetComponent<Rigidbody>().angularVelocity = _rigTransform.TransformDirection(_controllerPose.GetAngularVelocity());

        }

        _objectInHand = null;
    }

    void Highlight(GameObject go, bool on)
    {
        Material secondaryMaterial = (on) ? _outline : null;
        MeshRenderer goRenderer = go.GetComponent<MeshRenderer>();

        if (goRenderer != null)
        {
            Material baseMaterial = goRenderer.materials[0];
            go.GetComponent<MeshRenderer>().materials = new Material[] { baseMaterial, secondaryMaterial };
        }
    }

    bool InLayerMask(GameObject go)
    {
        return (_layerMask == (_layerMask | (1 << go.layer)));
    }
}
