using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine;
using  UnityEngine.EventSystems;
using UnityEngine.UI;

public class BubbleElement : MonoBehaviour {
    [HideInInspector] public Sides side;
    
    public float circleRadius = 50;
    public float effectHeight = 70;

    private RectTransform uiBox;
    private Image image;
    private Vector2 pos;
    private bool wasTouched;

    // brain target
    private RectTransform target;
    private ParticleSystem particles;
    private RectTransform lineEffect;
    private Color col;
    public void Setup(Vector2 canvasWidth, Vector2 canvasHeight, Vector2 windowRes, RectTransform target,
        ParticleSystem particles, Sides owner, Color col) {
        uiBox = GetComponent<RectTransform>();
        image = uiBox.GetComponent<Image>();
        uiBox.anchoredPosition = new Vector2(Random.Range(canvasWidth.x, canvasWidth.y) * windowRes.x,
            Random.Range(canvasHeight.x, canvasHeight.y) * windowRes.y);
        pos = uiBox.anchoredPosition;
        
        this.target = target;
        this.particles = particles;
        this.particles.transform.localPosition = uiBox.localPosition; //- new Vector3(202, 128);
        side = owner;
        var particlesMain = this.particles.main;
        this.col = col;
        particlesMain.startColor = this.col;
        image.color = new Color(col.r,col.g,col.b,1);

    }

    private void Update() {
        if(side == Sides.bot) return;
        if (wasTouched) return;
        if (!Input.GetMouseButtonDown(0) || !IsOverBubble()) return;

        Vector2 mousePos = Input.mousePosition;
        if (Mathf.Sqrt(Mathf.Pow(pos.x - mousePos.x, 2) + Mathf.Pow(pos.y - mousePos.y, 2)) < circleRadius &&
            !wasTouched)
        {
            Pop();
        }
    }
    
    public void Pop() {
        wasTouched = true;
        image.raycastTarget = false;
        image.color = new Color(col.r, col.g, col.b, .4f);
        
        particles.Play();
        IncreaseParticles(20);
        SkillCheck._.Popped(side);
    }

    public void Attack() {
        duringAttack = false;
        lineEffect = Instantiate(SkillCheck._AttackEffect, uiBox.parent);
        Image img;
        (img = lineEffect.GetComponent<Image>()).color = col;
        lineEffect.localPosition = (uiBox.localPosition + target.localPosition) / 2;
        Vector3 dif = target.localPosition - uiBox.localPosition;
        lineEffect.sizeDelta = new Vector3(dif.magnitude - uiBox.sizeDelta.x / 2, effectHeight);
        lineEffect.rotation = Quaternion.Euler(new Vector3(0, 0, 180 * Mathf.Atan(dif.y / dif.x) / Mathf.PI));
        StartCoroutine(FadeElement(img,.4f));
    }

    bool IsOverBubble() {
        return EventSystem.current.IsPointerOverGameObject();
    }

    // VFX zone ↓
    private bool duringAttack;
    private const int baseShake = 3;
    public void SetExternalForces() {
        var particlesExternalForces = particles.externalForces;
        particlesExternalForces.enabled = true;
        duringAttack = true;
        StartCoroutine(Shake());
    }

    IEnumerator Shake() {
        Vector2 basePosUI = uiBox.anchoredPosition;
        Vector2 basePosParticle = particles.transform.localPosition;
        float time = 0;
        while (duringAttack) {
            int shake = Random.Range(-baseShake, baseShake);
            Vector2 factor = new Vector2(shake, shake);
            
            uiBox.anchoredPosition = basePosUI + factor;
            particles.transform.localPosition = basePosParticle + factor;
            image.color = Color.Lerp( image.color,Color.white, time+= Time.fixedDeltaTime * (1f/10));
            yield return new WaitForEndOfFrame();
        }
        
    }

    public void IncreaseParticles(int count) {
        var particlesEmission = particles.emission;
        particlesEmission.rateOverTime = count;
    }

    IEnumerator FadeElement(Image img, float wait = 0, bool destroy = false) {
        yield return new WaitForSeconds(wait);
        var color = img.color;
        
        while (color.a > 0) {
            color.a -= 0.01f;
            img.color = color;
            yield return new WaitForSeconds(.01f);
        }

        if (destroy) Destroy(img.gameObject);
    }
    
    public void RemoveBubble() {
        
        if(lineEffect != null)Destroy(lineEffect.gameObject);
        if(uiBox != null) StartCoroutine(FadeElement(image, 0,true));
        if(particles != null)Destroy(particles.gameObject);

    }
}