using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ControllerTurn : MonoBehaviour
{
    [SerializeField] private SteamVR_Input_Sources _handType;
    [SerializeField] private SteamVR_Action_Boolean snapLeftAction;
    [SerializeField] private SteamVR_Action_Boolean snapRightAction;
    [SerializeField] private int snapAngle;
    [SerializeField] private Transform _rigTransform;

    void Update()
    {
        if (snapLeftAction.GetStateDown(_handType))
        {
            Turn(-snapAngle);
        }
        else if (snapRightAction.GetStateDown(_handType))
        {
            Turn(snapAngle);
        }
    }

    private void Turn(int angle)
    {
        _rigTransform.Rotate(Vector3.up, angle);
    }
}
