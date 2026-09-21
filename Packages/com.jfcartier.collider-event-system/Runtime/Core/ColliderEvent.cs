using UnityEngine;

namespace ColliderEventSystem
{
    /// <summary>
    /// Attach any native Collider OR Collider2D (mark it "Is Trigger") to a GameObject alongside this
    /// component to build a zone: when something enters, the Conditions are checked; once they're met
    /// (and stay met for Hold Time) the Actions run. Works with both the 3D and 2D physics engines -
    /// use whichever collider type matches your project. For a version that isn't tied to a physical
    /// zone, use ConditionWatcher instead.
    ///
    /// NOTE (2D physics): for OnTriggerEnter2D to fire, AT LEAST ONE of the two objects involved (this
    /// zone, or the object entering) needs a Rigidbody2D.
    /// </summary>
    [AddComponentMenu("Collider Event System/Collider Event")]
    public sealed class ColliderEvent : ColliderEventBase
    {
        [Tooltip("Only objects with one of these tags can trigger this zone. Leave empty to allow anything with a Collider.")]
        public string[] requiredTags = System.Array.Empty<string>();

        [Tooltip("The colour of the zone gizmo in the editor.")]
        public Color debugColor = new Color(1f, 0.4f, 0f, 0.3f);

        private Collider m_Collider3D;
        private Collider2D m_Collider2D;

        protected override void Start()
        {
            base.Start();
            m_Collider3D = GetComponent<Collider>();
            m_Collider2D = GetComponent<Collider2D>();

            if (m_Collider3D == null && m_Collider2D == null)
            {
                Debug.LogWarning(gameObject.name + " (Collider Event System) has no Collider or Collider2D - it will never trigger.", this);
            }
        }

        protected override void OnFiredAndReset()
        {
            // Require the object to leave and re-enter the zone before this can fire again.
            StopChecking();
        }

        // ---- 3D ----

        private void OnTriggerEnter(Collider other)
        {
            if (!PassesTagFilter(other.tag)) return;

            collidingObject = other.gameObject;
            BeginChecking();
        }

        private void OnTriggerExit(Collider other)
        {
            HandleExit(other.gameObject);
        }

        // ---- 2D ----

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!PassesTagFilter(other.tag)) return;

            collidingObject = other.gameObject;
            BeginChecking();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            HandleExit(other.gameObject);
        }

        // ---- Shared ----

        private void HandleExit(GameObject other)
        {
            if (collidingObject != other) return;

            if (afterTrigger == AfterTrigger.ExecuteExitActions && HasFired)
            {
                TriggerExit();
            }
            else
            {
                StopChecking();
            }
        }

        private bool PassesTagFilter(string otherTag)
        {
            if (requiredTags == null || requiredTags.Length == 0) return true;

            for (int i = 0; i < requiredTags.Length; i++)
            {
                if (otherTag == requiredTags[i]) return true;
            }

            return false;
        }

        // A hair larger than the real collider so the translucent gizmo doesn't z-fight with a same-sized
        // visible mesh/sprite on the object. Proportional rather than a fixed add-on, so it stays
        // negligible regardless of the object's actual size.
        private const float GizmoInflation = 1.0001f;

        private void OnDrawGizmos()
        {
            if (m_Collider3D == null) m_Collider3D = GetComponent<Collider>();
            if (m_Collider2D == null) m_Collider2D = GetComponent<Collider2D>();

            Gizmos.color = debugColor;

            // A GameObject could technically have both a Collider and a Collider2D - draw whichever
            // is found first (3D takes priority since it was the original behaviour).
            if (m_Collider3D != null)
            {
                DrawGizmo3D(m_Collider3D);
            }
            else if (m_Collider2D != null)
            {
                DrawGizmo2D(m_Collider2D);
            }
        }

        private void DrawGizmo3D(Collider collider)
        {
            switch (collider)
            {
                case BoxCollider box:
                    // Bounds is always an axis-aligned world-space box, so it can't follow the object's
                    // rotation. Drawing in the collider's own local space (via Gizmos.matrix) can.
                    Gizmos.matrix = transform.localToWorldMatrix;
                    Gizmos.DrawCube(box.center, box.size * GizmoInflation);
                    Gizmos.matrix = Matrix4x4.identity;
                    break;

                case SphereCollider sphere:
                    // Mirrors Unity's own physics behaviour for SphereCollider: non-uniform scale is
                    // ignored, only the largest axis is used.
                    Vector3 sphereLossyScale = transform.lossyScale;
                    float sphereScale = Mathf.Max(Mathf.Max(sphereLossyScale.x, sphereLossyScale.y), sphereLossyScale.z);
                    Gizmos.DrawSphere(transform.TransformPoint(sphere.center), sphere.radius * sphereScale * GizmoInflation);
                    break;

                case CapsuleCollider capsule:
                    DrawCapsuleGizmo3D(capsule);
                    break;

                case MeshCollider meshCollider when meshCollider.sharedMesh != null:
                    Gizmos.DrawMesh(meshCollider.sharedMesh, transform.position, transform.rotation, transform.lossyScale * GizmoInflation);
                    break;

                default:
                    // Uncommon collider type (e.g. TerrainCollider) - fall back to the bounding box.
                    Bounds bounds = collider.bounds;
                    Gizmos.DrawCube(bounds.center, bounds.size * GizmoInflation);
                    break;
            }
        }

        private Mesh m_CapsuleMesh;
        private const float CapsulePrimitiveHeight = 2f;
        private const float CapsulePrimitiveRadius = 0.5f;

        private void DrawCapsuleGizmo3D(CapsuleCollider capsule)
        {
            if (m_CapsuleMesh == null)
            {
                // Resources.GetBuiltinResource's mesh names are undocumented/internal and have drifted
                // between Unity versions, so grab the primitive's mesh the reliable way instead.
                GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                m_CapsuleMesh = temp.GetComponent<MeshFilter>().sharedMesh;
                DestroyImmediate(temp);
            }

            // Mirrors Unity's own physics behaviour for CapsuleCollider under scale: the axis picked by
            // "direction" drives the height, the other two axes' largest value drives the radius.
            Vector3 scale = transform.lossyScale;
            Quaternion axisRotation;
            float heightScale, radiusScale;
            switch (capsule.direction)
            {
                case 0:
                    axisRotation = Quaternion.Euler(0f, 0f, 90f);
                    heightScale = Mathf.Abs(scale.x);
                    radiusScale = Mathf.Max(Mathf.Abs(scale.y), Mathf.Abs(scale.z));
                    break;
                case 2:
                    axisRotation = Quaternion.Euler(90f, 0f, 0f);
                    heightScale = Mathf.Abs(scale.z);
                    radiusScale = Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.y));
                    break;
                default:
                    axisRotation = Quaternion.identity;
                    heightScale = Mathf.Abs(scale.y);
                    radiusScale = Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.z));
                    break;
            }

            float radius = capsule.radius * radiusScale * GizmoInflation;
            float height = Mathf.Max(capsule.height * heightScale, radius * 2f) * GizmoInflation;

            Vector3 meshScale = new Vector3(radius / CapsulePrimitiveRadius, height / CapsulePrimitiveHeight, radius / CapsulePrimitiveRadius);
            Gizmos.DrawMesh(m_CapsuleMesh, transform.TransformPoint(capsule.center), transform.rotation * axisRotation, meshScale);
        }

        private void DrawGizmo2D(Collider2D collider)
        {
            switch (collider)
            {
                case BoxCollider2D box:
                    Gizmos.matrix = transform.localToWorldMatrix;
                    Gizmos.DrawCube(box.offset, box.size * GizmoInflation);
                    Gizmos.matrix = Matrix4x4.identity;
                    break;

                case CircleCollider2D circle:
                    Vector3 circleLossyScale = transform.lossyScale;
                    float circleScale = Mathf.Max(circleLossyScale.x, circleLossyScale.y);
                    Gizmos.DrawSphere(transform.TransformPoint(circle.offset), circle.radius * circleScale * GizmoInflation);
                    break;

                case CapsuleCollider2D capsule:
                    Gizmos.matrix = transform.localToWorldMatrix;
                    Vector2 capsuleSize = capsule.size * GizmoInflation;
                    Gizmos.DrawCube(capsule.offset, new Vector3(capsuleSize.x, capsuleSize.y, 0.01f));
                    Gizmos.matrix = Matrix4x4.identity;
                    break;

                case EdgeCollider2D edge:
                    DrawEdgeGizmo2D(edge);
                    break;

                case PolygonCollider2D polygon:
                    DrawPolygonGizmo2D(polygon);
                    break;

                default:
                    // Uncommon collider type (e.g. CompositeCollider2D) - fall back to the bounding box.
                    Bounds bounds = collider.bounds;
                    Gizmos.DrawCube(bounds.center, bounds.size * GizmoInflation);
                    break;
            }
        }

        private void DrawEdgeGizmo2D(EdgeCollider2D edge)
        {
            Vector2[] points = edge.points;
            if (points == null || points.Length < 2) return;

            for (int i = 0; i < points.Length - 1; i++)
            {
                Vector3 a = transform.TransformPoint((Vector3)(points[i] + edge.offset));
                Vector3 b = transform.TransformPoint((Vector3)(points[i + 1] + edge.offset));
                Gizmos.DrawLine(a, b);
            }
        }

        private void DrawPolygonGizmo2D(PolygonCollider2D polygon)
        {
            for (int p = 0; p < polygon.pathCount; p++)
            {
                Vector2[] path = polygon.GetPath(p);
                if (path.Length < 2) continue;

                for (int i = 0; i < path.Length; i++)
                {
                    Vector3 a = transform.TransformPoint((Vector3)(path[i] + polygon.offset));
                    Vector3 b = transform.TransformPoint((Vector3)(path[(i + 1) % path.Length] + polygon.offset));
                    Gizmos.DrawLine(a, b);
                }
            }
        }
    }
}