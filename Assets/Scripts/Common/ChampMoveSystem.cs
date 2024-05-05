using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

namespace TMG.NFE_Tutorial {
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial struct ChampMoveSystem : ISystem {

        public void OnUpdate(ref SystemState state) {
            var deltaTime = SystemAPI.Time.DeltaTime;

            foreach (var (transform, targetPosition, moveSpeed) in SystemAPI.Query<RefRW<LocalTransform>, ChampMoveTargetPosition, CharacterMoveSpeed>().WithAll<Simulate>()) {
                var moveTarget = targetPosition.Value;
                moveTarget.y = transform.ValueRO.Position.y;

                if (math.distance(transform.ValueRO.Position, moveTarget) < 0.1f) continue;

                var moveDirection = math.normalize(moveTarget - transform.ValueRO.Position);
                var moveVector = moveDirection * moveSpeed.Value * deltaTime;
                transform.ValueRW.Position += moveVector;
                transform.ValueRW.Rotation = quaternion.LookRotationSafe(moveDirection, math.up());
            }
        }
    }
}