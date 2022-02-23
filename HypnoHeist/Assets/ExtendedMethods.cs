using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtendedMethods {
   static public Vector2 Round(Vector2 v) => new Vector2(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y)); 
   static public Vector2 Round(Vector3 v) => new Vector2(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));
}
