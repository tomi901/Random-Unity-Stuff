using Unity.Rendering;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;


public class EntityManagerStarter : MonoBehaviour
{

    private EntityManager entityManager;


    private void Awake()
    {
        EntityManager entityManager = World.Active.GetOrCreateManager<EntityManager>();

        var testEntity = entityManager.CreateArchetype(
            ComponentType.Create<Rotator>(),
            ComponentType.Create<Position>(),
            ComponentType.Create<Matrix4x4>(),
            ComponentType.ReadOnly<MeshInstanceRenderer>()
            );

    }

}
