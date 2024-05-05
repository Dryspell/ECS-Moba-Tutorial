using Unity.Entities;
using Unity.NetCode;
using Unity.Physics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TMG.NFE_Tutorial {

    [UpdateInGroup(typeof(GhostInputSystemGroup))]
    public partial class ChampMoveInputSystem : SystemBase {

        private MobaInputActions _inputActions;
        private CollisionFilter _selectionFilter;

        protected override void OnCreate() {
            _inputActions = new MobaInputActions();
            _selectionFilter = new CollisionFilter {
                // References Physics Category Names
                BelongsTo = 1 << 5, // RaycastGroup
                CollidesWith = 1 << 0, // GroundPlane
            };
            RequireForUpdate<OwnerChampTag>();
        }

        protected override void OnStartRunning() {
            _inputActions.Enable();
            _inputActions.GameplayMap.SelectMovePosition.performed += OnSelectMovePosition;
        }

        protected override void OnStopRunning() {
            _inputActions.Disable();
            _inputActions.GameplayMap.SelectMovePosition.performed -= OnSelectMovePosition;
        }

        private void OnSelectMovePosition(InputAction.CallbackContext context) {
            var collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
            var cameraEntity = SystemAPI.GetSingletonEntity<MainCameraTag>();
            var camera = EntityManager.GetComponentData<MainCamera>(cameraEntity).Value;

            var mousePosition = Input.mousePosition;
            mousePosition.z = 100f;
            var worldPosition = camera.ScreenToWorldPoint(mousePosition);

            var selectionInput = new RaycastInput {
                Start = camera.transform.position,
                End = worldPosition,
                Filter = _selectionFilter
            };

            if (collisionWorld.CastRay(selectionInput, out var hit)) {
                var champEntity = SystemAPI.GetSingletonEntity<OwnerChampTag>();
                EntityManager.SetComponentData(champEntity, new ChampMoveTargetPosition {
                    Value = hit.Position
                });
            }
        }

        protected override void OnUpdate() {

        }
    }
}