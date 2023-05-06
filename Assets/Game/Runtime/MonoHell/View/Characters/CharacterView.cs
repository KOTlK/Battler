using UnityEngine;
using UnityEngine.AI;

namespace Game.Runtime.MonoHell.View.Characters
{
    public class CharacterView : MonoBehaviour
    {
        [SerializeField] private GameObject _body;
        
        public NavMeshAgent NavMeshAgent;
        
        public void Highlight()
        {
            _body.layer = LayerMask.NameToLayer("Outline");
        }

        public void StopHighlighting()
        {
            _body.layer = LayerMask.NameToLayer("NoOutline");
        }

        public void PlayDeathAnimation()
        {
            _body.layer = LayerMask.NameToLayer("NoOutline");
            gameObject.SetActive(false);
            transform.Rotate(Vector3.forward, 90f);
        }
    }
}