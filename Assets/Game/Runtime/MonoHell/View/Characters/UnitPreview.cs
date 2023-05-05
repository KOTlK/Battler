using UnityEngine;

namespace Game.Runtime.MonoHell.View.Characters
{
    public class UnitPreview : MonoBehaviour
    {
        public bool Hidden;
        
        public void Show()
        {
            gameObject.SetActive(true);
            Hidden = false;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            Hidden = true;
        }
    }
}