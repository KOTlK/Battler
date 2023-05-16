using System;
using UnityEngine;
using UnityEngine.UI.Extensions;

namespace Game.Runtime.MonoHell.View.Selection
{
    public class SelectionArea : MonoBehaviour
    {
        [SerializeField] private UILineRenderer _lineRenderer;
        
        public void DrawRect(Area rect)
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