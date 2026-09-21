using UnityEngine;

namespace ColliderEventSystem.Samples
{
    /// <summary>
    /// Spawns a vertical stack of cube copies, each with a Rigidbody, when SpawnStack() is called - meant
    /// to be wired up to an Invoke Events Action (no parameters needed).
    /// </summary>
    public sealed class CubeStackSpawner : MonoBehaviour
    {
        [Tooltip("The cube to duplicate for each copy in the stack.")]
        public GameObject cubeToDuplicate;

        public int count = 10;
        public float spacing = 1.5f;

        public void SpawnStack()
        {
            if (cubeToDuplicate == null) return;

            for (int i = 1; i <= count; i++)
            {
                Vector3 position = transform.position + Vector3.up * (spacing * i) - Vector3.back * (spacing * i);
                GameObject instance = Instantiate(cubeToDuplicate, position, Quaternion.identity);

                if (instance.GetComponent<Rigidbody>() == null)
                {
                    instance.AddComponent<Rigidbody>();
                }
            }
        }
    }
}
