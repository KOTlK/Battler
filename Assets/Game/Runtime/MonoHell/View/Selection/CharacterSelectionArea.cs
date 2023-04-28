using System;
using UnityEngine;
using UnityEngine.UI.Extensions;

namespace Game.Runtime.MonoHell.View.Selection
{
    public class CharacterSelectionArea : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private UILineRenderer _lineRenderer;
        
        public void DrawRect(SelectArea rect)
        {
            _lineRenderer.Points = new Vector2[]
            {
                new (rect.Position.x - rect.HalfExtents.x, rect.Position.y + rect.HalfExtents.y),
                new (rect.Position.x + rect.HalfExtents.x, rect.Position.y + rect.HalfExtents.y),
                new (rect.Position.x + rect.HalfExtents.x, rect.Position.y - rect.HalfExtents.y),
                new (rect.Position.x - rect.HalfExtents.x, rect.Position.y - rect.HalfExtents.y),
                new (rect.Position.x - rect.HalfExtents.x, rect.Position.y + rect.HalfExtents.y)
            };
        }

        public void Clear()
        {
            _lineRenderer.Points = Array.Empty<Vector2>();
        }

    }
}