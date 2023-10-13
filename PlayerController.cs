using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SurvivalProject
{
    public class PlayerController : MonoBehaviour, IDamageable
    {
        [Header("Player Stats")]
        [SerializeField] private float _walkSpeed = 3f;
        [SerializeField] private float _runSpeed = 6f;
        [SerializeField] private float _maxHP = 100;
        [SerializeField] private float _maxSP = 100;

        [Header("Player Settings")]
        [Range(0.5f, 30f)][SerializeField] private float _mouseSensitivity = 2f;

        private CharacterController _controller;
        private Animator _animator;
        private InputManager _inputManager;
        private Camera _mainCamera;

        private Vector2 _inputMove;
        private Vector2 _inputLook;

        private bool _isSprint;

        private float _speed;
        private float _targetSpeed;
        private float _cameraHorizontalRot;
        private float _cameraVerticalRot;
        private float _targetRotation;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _animationBlend;
        private float _currentHP;
        private float _currentSP;

        private const float _threshold = 0.01f;
        private const float _speedChangeRate = 10f;
        private const float _upperLimit = -30f;
        private const float _bottomLimit = 70f;

        private int _speedAnimByID;
        private void Awake()
        {
            _inputManager = new InputManager();

            _controller = GetComponent<CharacterController>();
            _animator = GetComponent<Animator>();

            InitializeAnimById();
        }
        private void OnEnable()
        {
            _inputManager.Enable();

            _inputManager.Player.Move.performed += OnMoveInputAction;
            _inputManager.Player.Look.performed += OnLookInputAction;
            _inputManager.Player.Sprint.performed += OnSprintInputAction;

            _inputManager.Player.Move.canceled += OnMoveInputAction;
            _inputManager.Player.Look.canceled += OnLookInputAction;
            _inputManager.Player.Sprint.canceled += OnSprintInputAction;
        }
        private void Start()
        {
            _targetSpeed = _walkSpeed;
            _currentHP = _maxHP; // þimdilik.. böyle kaldýracam sonra
            _mainCamera = Camera.main;
        }
        private void Update()
        {
            if (!GUIManager.Instance.InventoryIsOpen)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
        private void FixedUpdate()
        {
            Move();
        }
        private void LateUpdate()
        {
            CameraRotation();
        }
        private void OnMoveInputAction(InputAction.CallbackContext context) => _inputMove = context.ReadValue<Vector2>();
        private void OnLookInputAction(InputAction.CallbackContext context)
        {
            if (GUIManager.Instance.InventoryIsOpen) { _inputLook = Vector2.zero; return; }

            _inputLook = context.ReadValue<Vector2>();
        }
        private void OnSprintInputAction(InputAction.CallbackContext context)
        {
            _isSprint = context.ReadValueAsButton();
        }
        private void InitializeAnimById()
        {
            _speedAnimByID = Animator.StringToHash("speed");
        }
        private void CameraRotation()
        {
            if (_inputLook.sqrMagnitude > _threshold)
            {
                _cameraVerticalRot -= _inputLook.y * _mouseSensitivity * 1.5f * Time.deltaTime;
                _cameraHorizontalRot = _inputLook.x * _mouseSensitivity * 1.5f * Time.deltaTime;

                _cameraVerticalRot = Mathf.Clamp(_cameraVerticalRot, _upperLimit, _bottomLimit);
                _mainCamera.transform.localRotation = Quaternion.Euler(_cameraVerticalRot * 1.25f, 0.0f, 0.0f);

                transform.Rotate(Vector3.up * _cameraHorizontalRot);
            }
        }
        private void Move()
        {
            if (GUIManager.Instance.InventoryIsOpen) { _targetSpeed = 0; _animator.SetFloat(_speedAnimByID, 0.0f); return; }

            _targetSpeed = _isSprint ? _runSpeed : _walkSpeed;

            if (_inputMove == Vector2.zero) { _targetSpeed = 0.0f; }

            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
            float speedOffset = 0.1f;

            if (currentHorizontalSpeed < _targetSpeed - speedOffset | currentHorizontalSpeed > _targetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, _targetSpeed, Time.deltaTime * _speedChangeRate);

                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = _targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, _targetSpeed, Time.deltaTime * _speedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            Vector3 inputDirection = new Vector3(_inputMove.x, 0.0f, _inputMove.y).normalized;

            if (_inputMove != Vector2.zero)
            {
                inputDirection = transform.right * _inputMove.x + transform.forward * _inputMove.y;
            }
            _controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            _animator.SetFloat(_speedAnimByID, _animationBlend);
        }
        private void OnDisable()
        {
            _inputManager.Disable();
        }

        public void TakeDamage(int amount)
        {
            if (amount < _currentHP)
            {
                _currentHP -= amount;
                if (_currentHP < amount)
                {
                    _currentHP = 0;
                }
            }
        }
    }
}
