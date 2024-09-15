using System;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer.Unity;

#nullable enable

namespace Input
{
    public class PlayerInputManager 
        : ITickable
            , IDisposable
    {
        private readonly PlayerInput _playerInput;

        public PlayerInputManager(PlayerInput playerInput)
        {
            _playerInput = playerInput;
        
            _playerInput.GameActionMap.Enable();
        }

        public void Tick()
        {
            bool isMoveLeft = _playerInput.GameActionMap.MoveLeft.WasPressedThisFrame();
            Debug.Log($"{nameof(PlayerInputManager)} Tick {nameof(isMoveLeft)}({isMoveLeft})");
        }

        public void Dispose()
        {
            _playerInput.GameActionMap.Disable();
        }
    }
}