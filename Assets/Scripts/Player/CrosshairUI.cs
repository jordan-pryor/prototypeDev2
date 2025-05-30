using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class CrosshairUI : MonoBehaviour
    {
        [SerializeField] private GameObject crosshairRoot; // Reference to the parent UI panel (Canvas or group)
        [SerializeField] private Image crosshairDot;       

        private void Start()
        {
            // Load saved preference on start
            bool isOn = PlayerPrefs.GetInt("crosshairOn", 1) == 1;
            SetVisible(isOn);
        }

        public void SetVisible(bool isVisible)
        {
            if (crosshairRoot != null)
                crosshairRoot.SetActive(isVisible);
        }
    }
}