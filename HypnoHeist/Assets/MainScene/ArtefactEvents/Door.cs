using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {

   [SerializeField] private Sprite open;
   [SerializeField] private Sprite close;
   [SerializeField] private bool isOpen = false;
   private SpriteRenderer renderer;
   private void Start() {
      renderer = GetComponent<SpriteRenderer>();
      if (!isOpen) {
         renderer.sprite = close;
         return;
      }

      GetComponent<Collider2D>().enabled = false;
      renderer.sprite = open;
      transform.position += (Vector3.right * .5f);
   }

   public void ChangeState() {
      isOpen = !isOpen;
      
      if (isOpen) {
         renderer.sprite = open;
         transform.position += (Vector3.right * .5f);
         GetComponent<Collider2D>().enabled = false;
      } else {
         renderer.sprite = close;
         transform.position -= (Vector3.right * .5f);
         GetComponent<Collider2D>().enabled = true;
      }
   }
}
