using System;
using NCGames.Scriptables;
using TMPro;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NCGames.GameSystems
{
    /// <summary>
    /// Represents a pocket on the roulette wheel that can be activated when the ball lands in it.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class Pocket : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private PocketSO data;
        [SerializeField] private TMP_Text pocketNumberText;
        
        [Header("Visual Feedback")]
        [SerializeField] private Color activeColor = Color.yellow;
        [SerializeField] private Color defaultColor = Color.white;

        private bool _isPocketActive;
        
        /// <summary>
        /// The data associated with this pocket.
        /// </summary>
        public PocketSO Data => data;
        
        /// <summary>
        /// Indicates whether the ball is currently in this pocket.
        /// </summary>
        public bool IsPocketActive => _isPocketActive;
        
        #region Unity Lifecycle Methods
        
        private void Awake()
        {
            Initialize();
        }
        
        private void OnValidate()
        {
            if (Application.isEditor)
                Initialize();
        }
        
        private void OnTriggerEnter(Collider other)
        {
            HandleBallInteraction(other, true);
        }

        private void OnTriggerExit(Collider other)
        {
            HandleBallInteraction(other, false);
        }
        
        #endregion
        
        #region Pocket Functionality
        
        /// <summary>
        /// Initializes the pocket with its configuration data.
        /// </summary>
        private void Initialize()
        {
            if (data == null)
            {
                Debug.LogError($"Pocket data is not assigned on {gameObject.name}.");
                return;
            }

            if (pocketNumberText != null)
            {
                pocketNumberText.text = data.pocketNumber.ToString();
                pocketNumberText.color = defaultColor;
            }

#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }
        
        /// <summary>
        /// Handles ball entering or exiting the pocket.
        /// </summary>
        /// <param name="other">The collider that entered/exited the trigger</param>
        /// <param name="isEntering">True if entering, false if exiting</param>
        private void HandleBallInteraction(Collider other, bool isEntering)
        {
            if (other.CompareTag("Ball"))
            {
                _isPocketActive = isEntering;
                
                if (pocketNumberText != null)
                {
                    pocketNumberText.color = isEntering ? activeColor : defaultColor;
                }
                
                // Could emit events here for pocket activation/deactivation if needed
            }
        }
        
        /// <summary>
        /// Sets the pocket data and initializes the pocket.
        /// </summary>
        /// <param name="pocketData">The data to assign to this pocket</param>
        public void SetData(PocketSO pocketData)
        {
            data = pocketData;
            Initialize();
        }
        
        #endregion
    }
}