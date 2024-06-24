using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GameOverZoneScaler : MonoBehaviour
{
  public float scoreMax = 1000f;

  public void SetScale(float scale)
  {
    scale = Mathf.Clamp01(scale/scoreMax);
    transform.localScale = new Vector3(1, 1, Mathf.Lerp(.25f, 1f, scale));
  }
}

