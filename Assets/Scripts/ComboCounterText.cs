using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComboCounterText : MonoBehaviour

{
  public static ComboCounterText instance;
  public TextMeshProUGUI comboTextTemplate;
void OnEnable(){
  instance = this;
}
  public void SpawnComboText(Vector3 position, int comboCount)
  {
    TextMeshProUGUI comboText = Instantiate(comboTextTemplate, position, Quaternion.identity);
    comboText.gameObject.SetActive(true);
    comboText.text = "Combo x" + comboCount;
    Destroy(comboText.gameObject, 10f);
  }
}
