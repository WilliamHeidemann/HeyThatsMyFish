using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Settings
{
    public class SettingsSystem : MonoBehaviour
    {
        [SerializeField] private GameSettings gameSettings;

        private void Start()
        {
            gameSettings.playerCount = 2;
            gameSettings.penguinsPerPlayer = 4;
        }

        public void StartGame()
        {
            SceneManager.LoadScene("Game Scene");
        }
        
        public void SetPlayers(int players)
        {
            gameSettings.playerCount = players;
        }

        public void SetPenguins(int penguins)
        {
            gameSettings.penguinsPerPlayer = penguins;
        }
    }
}