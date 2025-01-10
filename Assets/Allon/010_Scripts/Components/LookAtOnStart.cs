using UnityEngine;

namespace Aion.Highlights.Components
{

   public class LookAtOnStart : MonoBehaviour
   {
      [SerializeField] private Transform _lookAtObject;

      private void Start()
      {
         transform.LookAt(_lookAtObject);
      }
   }
}