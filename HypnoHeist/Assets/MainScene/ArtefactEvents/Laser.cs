using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour {
 [SerializeField] private GameObject laserBeam;
 [SerializeField] private bool status = true;

 private void Start() {
  laserBeam.SetActive(status);
  GetComponent<Animator>().Play(status ? "LaserOn":"LaserOff");
  GetComponent<Collider2D>().enabled = status;
 }

 public void SetLaserBeam() {
  status = !status;
  laserBeam.SetActive(status);
  GetComponent<Collider2D>().enabled = status;
  GetComponent<Animator>().Play(status ? "LaserOn":"LaserOff");
 }
 
}
