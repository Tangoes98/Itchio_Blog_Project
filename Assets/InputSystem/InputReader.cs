using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour, Controls.IPlayerActions, Controls.IAutoDriveActions
{
    public static InputReader Instance;
    void Awake()
    {
        Instance?.NullCheck();
        Instance = this;
    }
    void NullCheck()
    {
        Debug.LogWarning("Multiple Instance occoured");
        Destroy(this);
    }

    Controls _control;
    void Start()
    {
        _control = new();
        _control.AutoDrive.Enable();
        _control.AutoDrive.SetCallbacks(this);
    }
    void Update()
    {
        isAutoDriveActive = _control.AutoDrive.enabled;
    }
    void OnDestroy()
    {
        _control.AutoDrive.RemoveCallbacks(this);
        Instance = null;
    }

    [SerializeField] bool isPlayerActive;
    [SerializeField] bool isUIActive;
    [SerializeField] bool isTitlePageActive;
    [SerializeField] bool isAutoDriveActive;




    #region PlayerAction

    public void OnCallMenu(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnCheck(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnDodge(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnExecution(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnFreeLook(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnHealthRecover(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnRegularAttack(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnSkillAction(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnSkillActionPanel(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnTarget(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnTargetSelection(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnTimeStop(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnTransformAttack(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }
    #endregion

    #region Drive

    [field: Header("Drive Control")]
    [field: SerializeField] public bool IsAccelerate { get; private set; }
    [field: SerializeField] public bool IsBrake { get; private set; }
    [field: SerializeField] public Action<bool> OnIsBrake { get; set; }
    [field: SerializeField] public bool IsHandBrake { get; private set; }
    [field: SerializeField] public float SteerValue { get; private set; }





    public void OnAccelerate(InputAction.CallbackContext context)
    {
        if (context.performed)
            IsAccelerate = true;
        if (context.canceled)
            IsAccelerate = false;
    }

    public void OnBrake(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //Debug.Log("Start on break");
            IsBrake = true;
            OnIsBrake?.Invoke(true);
        }

        if (context.canceled)
        {
            //Debug.Log("Stop on break");
            IsBrake = false;
            OnIsBrake?.Invoke(false);
        }
    }

    public void OnSteer(InputAction.CallbackContext context)
    {
        SteerValue = context.ReadValue<float>();
    }

    public void OnHandBrake(InputAction.CallbackContext context)
    {
        if (context.performed)
            IsHandBrake = true;
        if (context.canceled)
            IsHandBrake = false;
    }

    #endregion
}
