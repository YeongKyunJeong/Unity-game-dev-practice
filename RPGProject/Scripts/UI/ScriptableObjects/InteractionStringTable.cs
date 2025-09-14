using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;
using static UnityEngine.EventSystems.EventTrigger;

namespace RSP2
{
    [CreateAssetMenu(fileName = "InteractionStringTable", menuName = "Custom/SO/InteractionStringTable")]
    public class InteractionStringTable : ScriptableObject
    {
        [System.Serializable]
        public class InteractionToName
        {
            public NPCInteraction Interaction;
            public string InteractionName;
        }

        [field: SerializeField]
        private List<InteractionToName> InteractionWithNames = new List<InteractionToName>();

        private Dictionary<NPCInteraction, string> lookup;

        public string GetString(NPCInteraction interaction)
        {
            if (lookup == null)
            {
                lookup = new Dictionary<NPCInteraction, string>();
                foreach (var entry in InteractionWithNames)
                {
                    lookup[entry.Interaction] = entry.InteractionName;
                }
            }

            return lookup.TryGetValue(interaction, out var name) ? name : string.Empty;
        }

        public void InitializeDefaults()
        {
            InteractionWithNames.Clear();
            foreach (NPCInteraction value in Enum.GetValues(typeof(NPCInteraction)))
            {
                if (!InteractionWithNames.Exists(e => e.Interaction.Equals(value)))
                {
                    InteractionWithNames.Add(new InteractionToName { Interaction = value, InteractionName = "" });
                }
            }
        }

        private void OnEnable()
        {
#if UNITY_EDITOR
            if (InteractionWithNames == null || InteractionWithNames.Count == 0)
            {
                InitializeDefaults();
            }
#endif
        }
    }


}
