using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoController : MonoBehaviour
{

    [field: Header("Scene Reference")]
    [SerializeField] List<WheelCollider> _frontWheels;
    [SerializeField] List<WheelCollider> _rearWheels;
    [SerializeField] List<Transform> _ForwardRotatewheels;
    [SerializeField] List<Transform> _AngluarRotateWheels;
    [field: Header("Automobile config")]
    [field: SerializeField] public float ForwardRpmLimit; //* 30f
    [field: SerializeField] public float RearRpmLimit; //* 20f
    [field: SerializeField] public float MaxMotorTorqueSpeed;
    [field: Header("Rear")]
    [field: SerializeField] public float MaxRearMotorTorqueSpeed;
    [field: Header("Brake")]
    [field: SerializeField] public float MaxBreakTorque;
    [field: SerializeField] public float GroundFrictionTorque;
    [field: Header("Steer")]
    [field: SerializeField] public float MaxSteerAngle;
    [field: SerializeField] public float SteerSensitivity;

    [Header("Debug")]
    [SerializeField] float _curRpm;
    [SerializeField] float Debug_rotationSpeed;
    [SerializeField] float Debug_motorTorque;
    [SerializeField] bool Debug_isBreaking;
    [SerializeField] bool Debug_isMoveBackward;
    [SerializeField] bool Debug_isMoveForward;
    [SerializeField] float _inputAcceleration;
    [SerializeField] float _torqueLimitCheck;
    [SerializeField] float _curBreakTorque;
    [SerializeField] float _curFrictionToruqe;
    [SerializeField] float _curSteerAngle;
    [SerializeField] float _curRearTorque;

    InputReader _inputReader;
    void Start()
    {
        _inputReader = InputReader.Instance;

        _inputReader.OnIsBrake -= OnIsBreakAction;
        _inputReader.OnIsBrake += OnIsBreakAction;
    }

    void Update()
    {
        Debug_rotationSpeed = _frontWheels[0].rotationSpeed;
        _curRpm = _frontWheels[0].rpm;
        Debug_motorTorque = _frontWheels[0].motorTorque;

        CheckFriction();
        MotorRPMLimitCheck();
        CheckAccelerateInput();
        CheckSteerAngleValue();

        _ForwardRotatewheels.ForEach(i => Anim_ZRotateWheel(i, _curRpm * 6f));
        _AngluarRotateWheels.ForEach(i => Anim_YRotateWheel(i, _curSteerAngle));
    }

    void FixedUpdate()
    {
        _frontWheels.ForEach(i => i.motorTorque = _inputAcceleration * MaxMotorTorqueSpeed * _torqueLimitCheck + _curRearTorque * _torqueLimitCheck);
        _rearWheels.ForEach(i => i.motorTorque = _inputAcceleration * MaxMotorTorqueSpeed * _torqueLimitCheck + _curRearTorque * _torqueLimitCheck);

        _frontWheels.ForEach(i => i.brakeTorque = _curBreakTorque + _curFrictionToruqe);
        _rearWheels.ForEach(i => i.brakeTorque = _curBreakTorque + _curFrictionToruqe);

        _frontWheels.ForEach(i => i.steerAngle = Mathf.Lerp(i.steerAngle, _curSteerAngle, SteerSensitivity));
    }

    #region Friction
    void CheckFriction()
    {
        if (_inputReader.IsAccelerate || _inputReader.IsBrake)
            _curFrictionToruqe = 0f;
        else
            _curFrictionToruqe = GroundFrictionTorque;
    }




    #endregion

    #region Accelerate

    void MotorRPMLimitCheck()
    {
        if (_curRpm > 1)
            MoveRPMLimitCheck(_curRpm, ForwardRpmLimit, ref _torqueLimitCheck);
        else
            MoveRPMLimitCheck(-_curRpm, RearRpmLimit, ref _torqueLimitCheck);
    }

    void MoveRPMLimitCheck(float curRmp, float limitRmp, ref float torqueLimit)
    {
        if (curRmp > limitRmp)
            torqueLimit = 0;
        else
            torqueLimit = 1;
    }

    void CheckAccelerateInput()
    {
        if (_inputReader.IsAccelerate)
        {
            _inputAcceleration = 1f;
            Debug_isMoveForward = true;
        }
        else
        {
            _inputAcceleration = 0f;
            Debug_isMoveForward = false;
        }
    }

    #endregion

    #region Steering

    void CheckSteerAngleValue()
    {
        _curSteerAngle = _inputReader.SteerValue * MaxSteerAngle;
    }

    #endregion
    #region Break / Rear
    void OnIsBreakAction(bool isBreakInput)
    {
        if (isBreakInput)
        {
            //* check if turn to move backward
            if (Mathf.Abs(_curRpm) < 1)
            {
                Debug_isBreaking = false;
                Debug_isMoveBackward = true;
                _curBreakTorque = 0f;
                _curRearTorque = MaxRearMotorTorqueSpeed;
            }
            else //* Breaking
            {
                Debug_isBreaking = true;
                Debug_isMoveBackward = false;
                _curBreakTorque = MaxBreakTorque;
                _curRearTorque = 0f;
            }
        }
        else
        {
            Debug_isBreaking = false;
            Debug_isMoveBackward = false;
            _curBreakTorque = 0f;
            _curRearTorque = 0f;
        }
    }

    #endregion

    #region Script Anim
    void Anim_ZRotateWheel(Transform wheel, float forwardRotSpeed)
    {
        wheel.Rotate(0f, 0f, forwardRotSpeed * Time.deltaTime);
    }

    void Anim_YRotateWheel(Transform wheel, float angle)
    {
        wheel.localEulerAngles = new Vector3(0f, angle, 0f);
    }


    #endregion

}
