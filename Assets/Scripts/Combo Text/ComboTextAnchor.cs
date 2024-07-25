using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ComboTextAnchor : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private string countPrefix;

    public void SetCount(int count)
    {
        Destroy(gameObject, 10f);

        text.text = $"{countPrefix}{count}";
    }
}
