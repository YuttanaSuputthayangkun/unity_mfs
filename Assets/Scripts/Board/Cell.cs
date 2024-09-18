using Unity.Mathematics;
using UnityEngine;

namespace Board
{
    public class Cell
    {
        private readonly CellComponent _cellComponent;

        public Vector2 Position => _cellComponent.transform.position;

        public Cell(CellComponent cellComponent)
        {
            _cellComponent = cellComponent;
        }

        public void SetName(string name)
        {
            _cellComponent.name = name;
        }

        public void SetPosition(Vector2 position)
        {
            _cellComponent.transform.SetLocalPositionAndRotation(position, quaternion.identity); 
        }

        public Vector3 GetWorldPosition() => _cellComponent.transform.position;

        public override string ToString() => _cellComponent.gameObject.name;
    }
}