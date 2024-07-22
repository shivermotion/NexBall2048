using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoulLineViisuals : MonoBehaviour
{
  public GameObject FoulLineStart;
  public GameObject FoulLineEnd;
   
   void LateUpdate()
   {
   
   
      transform.position = (FoulLineStart.transform.position +  FoulLineEnd.transform.position) / 2;
   }
}
