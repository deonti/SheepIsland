using System.Collections;
using UnityEngine;

// public class Spawner : MonoBehaviour
// {
//   [SerializeField] private Grass _grassPrototype;
//   [SerializeField] private float _grassRespawnDelay = 2f;
//   [SerializeField] private Sheep _sheepPrototype;
//
//   private GroundInfo _ground;
//
//   private void Awake() =>
//     _ground = FindObjectOfType<GroundInfo>();
//
//   private void Start()
//   {
//     foreach (Grass grass in FindObjectsByType<Grass>(FindObjectsSortMode.None))
//       ScheduleRespawn(grass);
//   }
//
//   private void ScheduleRespawn(Grass grass)
//   {
//     grass.destroyCancellationToken.Register(Respawn);
//
//     void Respawn()
//     {
//       if (_ground)
//         _ground.StartCoroutine(DelayedSpawnGrass());
//     }
//   }
//
//   private IEnumerator DelayedSpawnGrass()
//   {
//     GroundInfo ground = _ground;
//     Grass prototype = _grassPrototype;
//     float respawnDelay = _grassRespawnDelay;
//
//     while (true)
//     {
//       yield return new WaitForSeconds(respawnDelay);
//       if (!ground.TryGetRandomUsedCellPosition(IsFreeCell, out Vector3 freePosition)) continue;
//
//       ScheduleRespawn(Instantiate(prototype, freePosition, Quaternion.identity));
//       break;
//     }
//
//     bool IsFreeCell(GroundInfo.CellState state) =>
//       state is { IsWalkable: true, HasSheep: false, HasGrass: false };
//   }
// }