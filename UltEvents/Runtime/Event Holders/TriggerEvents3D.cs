﻿// UltEvents // https://kybernetik.com.au/ultevents // Copyright 2021-2024 Kybernetik //

#if UNITY_PHYSICS_3D

using UnityEngine;

namespace UltEvents
{
    /// <summary>
    /// An event that takes a single <see cref="Collider"/> parameter.
    /// </summary>
    [System.Serializable]
    public sealed class TriggerEvent3D : UltEvent<Collider> { }

    /************************************************************************************************************************/

    /// <summary>
    /// Holds <see cref="UltEvent"/>s which are called by various <see cref="MonoBehaviour"/> trigger events:
    /// <see cref="OnTriggerEnter"/>, <see cref="OnTriggerStay"/>, and <see cref="OnTriggerExit"/>.
    /// </summary>
    [AddComponentMenu(UltEventUtils.ComponentMenuPrefix + "Trigger Events 3D")]
    [HelpURL(UltEventUtils.APIDocumentationURL + "/TriggerEvents3D")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider))]
    public class TriggerEvents3D : MonoBehaviour
    {
        /************************************************************************************************************************/

        [SerializeField]
        private TriggerEvent3D _TriggerEnterEvent;

        /// <summary>Invoked by <see cref="OnTriggerEnter"/>.</summary>
        public TriggerEvent3D TriggerEnterEvent
        {
            get => _TriggerEnterEvent ??= new TriggerEvent3D();
            set => _TriggerEnterEvent = value;
        }

        /// <summary>Invokes <see cref="TriggerEnterEvent"/>.</summary>
        public virtual void OnTriggerEnter(Collider collider)
            => _TriggerEnterEvent?.Invoke(collider);

        /************************************************************************************************************************/

        [SerializeField]
        private TriggerEvent3D _TriggerStayEvent;

        /// <summary>Invoked by <see cref="OnTriggerStay"/>.</summary>
        public TriggerEvent3D TriggerStayEvent
        {
            get => _TriggerStayEvent ??= new TriggerEvent3D();
            set => _TriggerStayEvent = value;
        }

        /// <summary>Invokes <see cref="TriggerStayEvent"/>.</summary>
        public virtual void OnTriggerStay(Collider collider)
            => _TriggerStayEvent?.Invoke(collider);

        /************************************************************************************************************************/

        [SerializeField]
        private TriggerEvent3D _TriggerExitEvent;

        /// <summary>Invoked by <see cref="OnTriggerExit"/>.</summary>
        public TriggerEvent3D TriggerExitEvent
        {
            get => _TriggerExitEvent ??= new TriggerEvent3D();
            set => _TriggerExitEvent = value;
        }

        /// <summary>Invokes <see cref="TriggerExitEvent"/>.</summary>
        public virtual void OnTriggerExit(Collider collider)
            => _TriggerExitEvent?.Invoke(collider);

        /************************************************************************************************************************/
    }
}

#endif