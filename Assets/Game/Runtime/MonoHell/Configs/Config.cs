using System;
using Game.Runtime.MonoHell.View.Characters;
using UnityEngine.VFX;

namespace Game.Runtime.MonoHell.Configs
{
    [Serializable]
    public class Config
    {
        public CameraConfig CameraConfig;
        public VisualEffect PositionPreviewEffect;
        public UnitPreview PreviewPrefab;
    }
}