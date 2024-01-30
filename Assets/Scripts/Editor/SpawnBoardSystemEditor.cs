// using Settings;
// using UnityEditor;
// using UnityEngine;
//
// namespace Editor
// {
//     [CustomEditor(typeof(BoardSpawner))]
//     public class SpawnBoardSystemEditor : UnityEditor.Editor
//     {
//         private BoardSpawner _target;
//         public override void OnInspectorGUI()
//         {
//             _target = (BoardSpawner)target;
//             base.OnInspectorGUI();
//             if (GUILayout.Button("Spawn Hexagonal Board"))
//             {
//                 _target.SpawnBoard(BoardType.Hexagon, 5);
//             }
//             if (GUILayout.Button("Spawn Square Board"))
//             {
//                 _target.SpawnBoard(BoardType.Square, 5);
//             }
//             if (GUILayout.Button("Destroy Board"))
//             {
//                 _target.DestroyBoard();
//             }
//         }
//     }
// }