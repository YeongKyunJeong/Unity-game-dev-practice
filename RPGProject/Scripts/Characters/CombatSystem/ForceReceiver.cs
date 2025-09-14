using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RSP2
{
    public class ForceReceiver : MonoBehaviour
    {
        protected Vector3 force;
        protected bool isForced;
        protected bool isFirstFrame;
        protected MomentumDampingMode momentumDampingMode;

        protected virtual void Awake()
        {
            force = Vector3.zero;
            isForced = false;
            isFirstFrame = false;
            momentumDampingMode = MomentumDampingMode.InstantStop;
        }

        public virtual void CallUpdate()
        {
            if (!isForced) return;

            if (force.sqrMagnitude <= 0.001f)
            {
                force = Vector3.zero;
                isForced = false;
                isFirstFrame = false;
                return;
            }

            if (isFirstFrame)
            {
                isFirstFrame = false;
            }
            else
            {
                switch (momentumDampingMode)
                {
                    case MomentumDampingMode.DefaultDamping:
                        {
                            force = Vector3.Lerp(force, Vector3.zero, 1 - Mathf.Exp(-5 * Time.deltaTime));
                            break;
                        }
                    case MomentumDampingMode.SoftDamping:
                        {
                            force = Vector3.Lerp(force, Vector3.zero, 1 - Mathf.Exp(-2 * Time.deltaTime));
                            break;
                        }
                    case MomentumDampingMode.HardDamping:
                        {
                            force = Vector3.Lerp(force, Vector3.zero, 1 - Mathf.Exp(-10 * Time.deltaTime));
                            break;
                        }
                    case MomentumDampingMode.InstantStop:
                        {
                            force = Vector3.zero;
                            break;
                        }
                    case MomentumDampingMode.NoDamping:
                        {
                            break;
                        }
                    default:
                        {
                            force = Vector3.zero;
                            break;
                        }
                }
            }
        }

        public virtual void CallPhysicsUpdate() { }

        public virtual void AddForce(Vector3 _force, MomentumDampingMode _momentumDampingMode = MomentumDampingMode.DefaultDamping)
        {
            isForced = true;
            isFirstFrame = true;
            force = _force;
            momentumDampingMode = _momentumDampingMode;
        }


    }
}
