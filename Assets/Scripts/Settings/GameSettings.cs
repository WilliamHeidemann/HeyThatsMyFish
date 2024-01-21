using UnityEngine;

namespace Settings
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "GameSettings", order = 0)]
    public class GameSettings : ScriptableObject
    {
        public int playerCount;
        public int penguinsPerPlayer;
        public BoardType boardType;
        public int boardSize;
    }
}