using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace RSP2
{
    public class GizmosDrawer : MonoBehaviour
    {
        private Vector3 position;
        private Vector3 size;
        private DetectionType detectionType;
        public void UpdateParameter(Vector3 _position, Vector3 _size, DetectionType _detectionType)
        {
            position = _position;
            size = _size;
            detectionType = _detectionType;
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;

            Gizmos.color = Color.red;
            switch (detectionType)
            {
                case DetectionType.SphereRaycast:
                    {
                        Gizmos.DrawWireSphere(position, size.x);
                        break;
                    }
                case DetectionType.BoxRaycast:
                    {
                        Gizmos.DrawCube(position, size);
                        break;
                    }
                default:
                    return;
            }
        }
    }
}
