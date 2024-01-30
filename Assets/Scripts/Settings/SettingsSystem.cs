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
            gameSettings.boardType = BoardType.Hexagon;
            gameSettings.boardSize = 5;
        }

        public void StartGame()
        {
            SceneManager.LoadScene("Game Scene");
        }
        
        public void SetPlayers(int players) => gameSettings.playerCount = players;

        public void SetPenguins(int penguins) => gameSettings.penguinsPerPlayer = penguins;

        public void SetBoardType(int boardType)
        {
            if (boardType >= Enum.GetValues(typeof(BoardType)).Length) return;
            gameSettings.boardType = (BoardType)boardType;
        }

        public void SetBoardSize(int boardSize) => gameSettings.boardSize = boardSize;
    }
}