using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GameOverZoneScaler : MonoBehaviour
{
  public float scoreMax = 1000f;

 [Range(0,1)] public float startPointZ = 0f;
 [Range(0,2)] public float endPointZ = 1f;

  public void SetScale(float scale)
  {
    scale = Mathf.Clamp01(scale/scoreMax);
    transform.localScale = new Vector3(1, 1, Mathf.Lerp(startPointZ, endPointZ, scale));
  }
  void OnValidate()
  {
  SetScale(startPointZ*scoreMax);
  }
}

