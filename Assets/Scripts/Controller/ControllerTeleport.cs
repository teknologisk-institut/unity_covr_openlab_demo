using UnityEngine;
using Valve.VR;

public class ControllerTeleport : MonoBehaviour
{
    [SerializeField] private SteamVR_Input_Sources _handType;
    [SerializeField] private SteamVR_Behaviour_Pose _controllerPose;
    [SerializeField] private SteamVR_Action_Boolean _action;
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private GameObject _reticlePrefab;
    [SerializeField] private GameObject _invalidReticlePrefab;
    [SerializeField] private Color _laserColorValid;
    [SerializeField] private Color _laserColorNotValid;
    [SerializeField] private Vector3 _reticleOffset;
    [SerializeField] private Transform _rigTransform;
    [SerializeField] private Transform _headTransform;
    [SerializeField] private LayerMask _mask;
    private GameObject _laser;
    private GameObject _reticle;
    private GameObject _invalidReticle;
    private Transform _laserTransform;
    private Material _laserMaterial;
    private Material _reticleMaterial;
    private Material _invalidReticleMaterial;
    private Vector3 _hitPoint;
    private Vector3 _hitNormal;
    private bool _canTeleport;

    private void Start()
    {
        _laser = Instantiate(_laserPrefab);
        _laserTransform = _laser.transform;
        _laserMaterial = _laser.GetComponent<MeshRenderer>().material;
        _laser.SetActive(false);

        _reticle = Instantiate(_reticlePrefab);
        _reticleMaterial = _reticle.GetComponent<MeshRenderer>().material;
        _reticle.SetActive(false);

        _invalidReticle = Instantiate(_invalidReticlePrefab);
        _invalidReticleMaterial = _invalidReticle.GetComponentInChildren<MeshRenderer>().material;
        _invalidReticle.SetActive(false);
    }

    private void Update()
    {
        if (_action.GetState(_handType))
        {
            _canTeleport = false;

            RaycastHit hit;

            if (Physics.Raycast(_controllerPose.transform.position, transform.forward, out hit, 100f, _mask))
            {
                _hitPoint = hit.point;
                _hitNormal = hit.normal;
                int hitLayer = hit.collider.gameObject.layer;
                string hitTag = hit.collider.gameObject.tag;

                if (hitTag == "Floor")
                {
                    _canTeleport = true;
                }
                else
                    SetPointerColor(_laserColorNotValid);

                ShowLaser(hit.distance);
                ShowIndicator();
            }
            else
                HidePointer();

            if (_canTeleport)
            {
                SetPointerColor(_laserColorValid);
            }

        }

        if (_action.GetStateUp(_handType))
        {
            HidePointer();

            if (_canTeleport)
                Teleport();
        }
    }

    private void SetPointerColor(Color c)
    {
        _laserMaterial.color = c;
        _reticleMaterial.color = c;
        _invalidReticleMaterial.color = c;
    }

    private void ShowLaser(float length)
    {
        _laser.SetActive(true);
        _laserTransform.position = Vector3.Lerp(_controllerPose.transform.position, _hitPoint, .5f);
        _laserTransform.LookAt(_hitPoint);
        _laserTransform.localScale = new Vector3(_laserTransform.localScale.x, _laserTransform.localScale.y, length);
    }

    private void ShowIndicator()
    {
        GameObject curReticle;
        curReticle = (_canTeleport) ? _reticle : _invalidReticle;
        curReticle.SetActive(true);

        if (curReticle == _reticle)
            _invalidReticle.SetActive(false);
        else
            _reticle.SetActive(false);

        curReticle.transform.position = _hitPoint + _reticleOffset;
        curReticle.transform.up = _hitNormal;
    }

    private void HidePointer()
    {
        _laser.SetActive(false);
        _reticle.SetActive(false);
        _invalidReticle.SetActive(false);
    }
    private void Teleport()
    {
        Vector3 difference = _rigTransform.position - _headTransform.position;
        difference.y = 0;

        _rigTransform.position = _hitPoint + difference;
    }
}