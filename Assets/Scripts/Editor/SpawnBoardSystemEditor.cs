using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(BoardSpawner))]
    public class SpawnBoardSystemEditor : UnityEditor.Editor
    {
        private BoardSpawner _target;
        public override void OnInspectorGUI()
        {
            _target = (BoardSpawner)target;
            base.OnInspectorGUI();
            if (GUILayout.Button("Spawn Board"))
            {
                _target.SpawnBoard();
            }
            if (GUILayout.Button("Destroy Board"))
            {
                _target.DestroyBoard();
            }
        }
    }
}