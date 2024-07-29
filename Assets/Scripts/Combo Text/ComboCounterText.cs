using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComboCounterText : MonoBehaviour
{
  public static ComboCounterText instance;
  public ComboTextAnchor comboTextTemplate;
  void OnEnable()
  {
    instance = this;
  }
  public void SpawnComboText(Vector3 position, int comboCount)
  {
    if (comboCount < 2)
    {
      return;
    }

    Vector3 ScreenPoint = Camera.main.WorldToScreenPoint(position);
    ComboTextAnchor comboText = Instantiate(comboTextTemplate, transform);
    comboText.transform.position = ScreenPoint;

    comboText.SetCount(comboCount);
  }
}
