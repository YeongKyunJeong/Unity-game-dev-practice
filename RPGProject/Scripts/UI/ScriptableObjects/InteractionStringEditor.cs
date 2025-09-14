using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RSP2
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(InteractionStringTable))]
    public class InteractionStringEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Enum ��� �ʱ�ȭ"))
            {
                var table = (InteractionStringTable)target;
                table.InitializeDefaults();
                EditorUtility.SetDirty(table);
            }
        }
    }
#endif
}
