using UnityEngine;

/// <summary>
/// Attached to each enemy prefab. Carries the data that the pile system
/// needs when this enemy dies. No runtime logic — pure data container.
/// </summary>
public class CorpseUnit : MonoBehaviour
{
  [Tooltip("Mass this enemy contributes to the pile")]
  public float mass = 1f;

  [Tooltip("Corpse mesh")]
  public Mesh corpseMesh;
  
  [Tooltip("Corpse material")]
  public Material corpseMaterial;
  
  [Tooltip("Scale applied to mesh wen placed inside a pile")]
  public Vector3 meshScale = Vector3.one;
}
