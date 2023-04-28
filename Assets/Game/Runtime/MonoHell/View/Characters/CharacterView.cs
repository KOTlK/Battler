using UnityEngine;
using UnityEngine.AI;

namespace Game.Runtime.MonoHell.View.Characters
{
    public class CharacterView : MonoBehaviour
    {
        public NavMeshAgent NavMeshAgent;
        
        public void Highlight()
        {
            gameObject.layer = LayerMask.NameToLayer("Outline");
        }

        public void StopHighlighting()
        {
            gameObject.layer = LayerMask.NameToLayer("NoOutline");
        }
    }
}