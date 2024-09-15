using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Data;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer.Unity;

#nullable enable

namespace Input
{
    public class PlayerInputManager :
        IDisposable
    {
        private readonly PlayerInput _playerInput;

        // since we often check for this, just create an array for memory's sake 
        private readonly (Direction direction, InputType inputType)[] directionalInputs = new[]
        {
            (Direction.Left, InputType.MoveLeft),
            (Direction.Right, InputType.MoveRight),
            (Direction.Up, InputType.MoveUp),
            (Direction.Down, InputType.MoveDown),
        };

        public PlayerInputManager(PlayerInput playerInput)
        {
            _playerInput = playerInput;

            _playerInput.GameActionMap.Enable();
        }

        public bool IsPressed(InputType inputType)
        {
            return GetInputAction(inputType).WasPressedThisFrame();
        }

        public async UniTask<Direction> WaitDirectionalInputAsync(
            CancellationToken cancellationToken = new CancellationToken())
        {
            var tasks = directionalInputs
                .Select(x =>
                    UniTask.WaitUntil(() => IsPressed(x.inputType), PlayerLoopTiming.Update, cancellationToken))
                .ToArray();
            int finishedIndex = await UniTask.WhenAny(tasks);
            return directionalInputs[finishedIndex].direction;
        }

        private InputAction GetInputAction(InputType inputType) =>
            inputType switch
            {
                InputType.MoveLeft => _playerInput.GameActionMap.MoveLeft,
                InputType.MoveRight => _playerInput.GameActionMap.MoveRight,
                InputType.MoveUp => _playerInput.GameActionMap.MoveUp,
                InputType.MoveDown => _playerInput.GameActionMap.MoveDown,
                InputType.RotateForward => _playerInput.GameActionMap.RotateForward,
                InputType.RotateBack => _playerInput.GameActionMap.RotateBack,
                _ => throw new ArgumentOutOfRangeException(nameof(inputType), inputType, null)
            };

        void IDisposable.Dispose()
        {
            _playerInput.GameActionMap.Disable();
        }
    }
}