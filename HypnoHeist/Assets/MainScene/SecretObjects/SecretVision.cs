using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretVision : MonoBehaviour {
    public List<GameObject> secretObjects = new List<GameObject>();

    private void OnEnable() => PlayerBehaviours.onHypnosis += ShowSecrets;
    private void OnDisable() => PlayerBehaviours.onHypnosis -= ShowSecrets;

    private void Start() => ShowSecrets();

    void ShowSecrets() {
        bool set;
        set = PlayerData.pb.hypnotizing;
        
        foreach (var secretObject in secretObjects) {
            if (secretObject.TryGetComponent(out Callback cb)){
                if (cb != null) {
                    secretObject.SetActive(set);
                    continue;
                }
            }
            secretObject.GetComponent<SpriteRenderer>().enabled = set;
        }
    }
}
