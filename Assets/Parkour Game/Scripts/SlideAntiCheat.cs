using System.Collections;
using UnityEngine;

namespace Assets.Parkour_Game.Scripts
{
    public class SlideAntiCheat : MonoBehaviour
    {
        [SerializeField] private Vector3 startTransformation;
        [SerializeField] private Vector3 endTransformation;
        [SerializeField] private float transitionTime;
        
        private EnvironmentController environmentController;
        
        private void Start()
        {
            environmentController = transform.parent.gameObject.transform.GetChild(0).GetComponent<EnvironmentController>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                environmentController.StartTransform = startTransformation;
                environmentController.TargetTransform = endTransformation;
                environmentController.TransitionTime = transitionTime;
                environmentController.Movement = true;
            }
            
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                environmentController.Movement = false;
            }
        }
    }
}