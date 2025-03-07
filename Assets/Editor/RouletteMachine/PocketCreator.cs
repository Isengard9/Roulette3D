using NCGames.GameSystems;
using NCGames.Scriptables;
using UnityEditor;
using UnityEngine;

namespace NCGames.Editor
{
    public class PocketCreator : EditorWindow
    {
        private GameObject parentObject;
        private GameObject pocketPrefab;
        private float startAngle;
        private const int totalPockets = 37;
        private const float angleIncrement = 360f / totalPockets;
        private readonly int[] pocketNumbers = { 0, 32, 15, 19, 4, 21, 2, 25, 17, 34, 6, 27, 13, 36, 11, 30, 8, 23, 10, 5, 24, 16, 33, 1, 20, 14, 31, 9, 22, 18, 29, 7, 28, 12, 35, 3, 26 };

        [MenuItem("Tools/Pocket Creator")]
        public static void ShowWindow()
        {
            GetWindow<PocketCreator>("Pocket Creator");
        }

        private void OnGUI()
        {
            GUILayout.Label("Pocket Creator", EditorStyles.boldLabel);

            parentObject = (GameObject)EditorGUILayout.ObjectField("Parent Object", parentObject, typeof(GameObject), true);
            pocketPrefab = (GameObject)EditorGUILayout.ObjectField("Pocket Prefab", pocketPrefab, typeof(GameObject), false);
            startAngle = EditorGUILayout.FloatField("Start Angle", startAngle);

            if (GUILayout.Button("Create Pockets"))
            {
                CreatePockets();
            }
        }

        private void CreatePockets()
        {
            if (parentObject == null || pocketPrefab == null)
            {
                Debug.LogError("Parent Object and Pocket Prefab must be assigned.");
                return;
            }

            for (int i = 0; i < totalPockets; i++)
            {
                float angle = startAngle + i * angleIncrement;
                GameObject pocketInstance = Instantiate(pocketPrefab, parentObject.transform);
                pocketInstance.transform.localRotation = Quaternion.Euler(0, angle, 0);

                Pocket pocketScript = pocketInstance.GetComponent<Pocket>();
                if (pocketScript != null)
                {
                    PocketSO pocketSO = CreatePocketSO(pocketNumbers[i]);
                    pocketScript.SetData(pocketSO);
                }
            }
        }

        private PocketSO CreatePocketSO(int pocketNumber)
        {
            PocketSO pocketSO = ScriptableObject.CreateInstance<PocketSO>();
            pocketSO.pocketNumber = pocketNumber;
            pocketSO.pocketColor = GetPocketColor(pocketNumber);

            string assetPath = $"Assets/Data/Pocket/Pocket_{pocketNumber}.asset";
            AssetDatabase.CreateAsset(pocketSO, assetPath);
            AssetDatabase.SaveAssets();

            return pocketSO;
        }

        private Color GetPocketColor(int pocketNumber)
        {
            // Define the color logic based on the pocket number
            if (pocketNumber == 0)
                return Color.green;
            else if (pocketNumber % 2 == 0)
                return Color.black;
            else
                return Color.red;
        }
    }
}