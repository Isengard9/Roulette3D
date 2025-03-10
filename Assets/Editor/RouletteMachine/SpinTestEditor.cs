using UnityEditor;
using UnityEngine;

namespace NCGames.Editor
{
    [CustomEditor(typeof(RouletteController))]
    public class SpinTestEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            RouletteController rouletteController = (RouletteController)target;

            if (GUILayout.Button("Start Spin"))
            {
                if (!rouletteController.IsSpinning)
                {
                    rouletteController.StartSpin();
                }
                else
                {
                    Debug.Log("Spin is already in progress.");
                }
            }
        }
    }
}