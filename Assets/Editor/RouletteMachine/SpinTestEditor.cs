using UnityEditor;
using UnityEngine;

namespace NCGames.Editor
{
    [CustomEditor(typeof(SpinTest))]
    public class SpinTestEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            SpinTest spinTest = (SpinTest)target;

            if (GUILayout.Button("Start Spin"))
            {
                if (!spinTest.IsSpinning)
                {
                    spinTest.StartSpin();
                }
                else
                {
                    Debug.Log("Spin is already in progress.");
                }
            }
        }
    }
}