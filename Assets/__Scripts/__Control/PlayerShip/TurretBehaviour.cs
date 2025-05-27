using System;
using Course.Core;
using Course.Utility;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Course.Control.Turret
{
    public class TurretBehaviour : MonoBehaviour
    {
        [SerializeField]
        private bool debugDrawLine = false;
        
        private InputReader _inputReader;
        private IRotator _rotator;
        private IShooter _shooter;
        private Transform   _turret;
        private Transform   _spawn;
        private Camera      _cam;
        private Vector2    _screenPos;
        private bool _isInitialized = false;

        public void Initialize(InputReader inputReader,
            IRotator rotator,
            IShooter shooter,
            Transform turret,
            Transform spawn)
        {
            _inputReader   = inputReader;
            _rotator       = rotator;
            _shooter       = shooter;
            _turret        = turret;
            _spawn         = spawn;
            _cam           = Camera.main;
            _inputReader.OnAimEvent  += OnAim;
            _inputReader.OnFireEvent += OnFire;
            _isInitialized = true;
        }

        private void OnEnable()
        {
            if(!_isInitialized)
                return;
            _inputReader.OnAimEvent  += OnAim;
            _inputReader.OnFireEvent += OnFire;
        }

        private void OnDisable()
        {
            if(!_isInitialized)
                return;
            _inputReader.OnAimEvent  -= OnAim;
            _inputReader.OnFireEvent -= OnFire;
        }

        private void OnAim(Vector2 screenPos)
        {
           _rotator.Aim(_turret, screenPos, _cam);
           if(debugDrawLine)
           {
               float zDist = _turret.position.z - _cam.transform.position.z;
               _screenPos = _cam.ScreenToWorldPoint(
                new Vector3(screenPos.x, screenPos.y, zDist));
           }
        }

        private void OnFire()
        {
            Vector2 screenPos;
            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
            {
                // mobile tap
                screenPos = Touchscreen.current.primaryTouch.position.ReadValue();
            }
            else
            {
                // mouse click or gamepad fallback
                screenPos = Mouse.current.position.ReadValue();
            }
            _rotator.Aim(_turret, screenPos, _cam);
            if(debugDrawLine)
            {
                float zDist = _turret.position.z - _cam.transform.position.z;
                _screenPos = _cam.ScreenToWorldPoint(
                    new Vector3(screenPos.x, screenPos.y, zDist));
            }
            _shooter.Fire(_turret, _spawn);
        }

        private void OnDrawGizmos()
        {
            if(debugDrawLine && Application.isPlaying)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(_turret.position, _screenPos);
                Gizmos.DrawSphere(_screenPos, 0.2f);
            }
        }
    }
    
}
