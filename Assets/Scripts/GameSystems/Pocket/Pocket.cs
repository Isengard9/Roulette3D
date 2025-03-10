using System;
using NCGames.Scriptables;
using TMPro;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NCGames.GameSystems
{
    public class Pocket : MonoBehaviour
    {
        [SerializeField] private PocketSO data;
        public PocketSO Data => data;
        [SerializeField] private TMP_Text pocketNumberText;
        
        private bool isPocketActive;
        public bool IsPocketActive => isPocketActive;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Ball"))
            {
                pocketNumberText.color = Color.yellow;
                isPocketActive = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Ball"))
            {
                pocketNumberText.color = Color.white;
                isPocketActive = false;
            }
        }

        public void SetData(PocketSO pocketData)
        {
            data = pocketData;
            Initialize();
        }

        private void OnValidate()
        {
            if (Application.isEditor)
                Initialize();
        }

        private void Initialize()
        {
            if (data == null)
            {
                Debug.LogError("Pocket data is not assigned.");
                return;
            }

            pocketNumberText.text = data.pocketNumber.ToString();

#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }
    }
}