using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour, IDamageable
{
    [Header("Variables")]
    public float currentLife;
    public float maxLife = 10f;
    public bool defending;
    public bool invulnerable;

    [Header("Damage")]
    public bool doDamageEffect = true;
    public Color damageColor = Color.white;
    public UnityEvent<Vector3> OnDamage;
    public string damageAudioName;
    [HideInInspector] public List<FlashColor> flashColors;

    [Header("Death")]
    public UnityEvent OnDeath;
    public bool destroyOnKill = false;
    public Transform lootPosition;

    [Header("Loot")]
    public GameObject loot1;

    [Header("Invulnerability")]
    public float invulnerabilityTime = 2f;
    public bool doInvunerabilityEffect = false;
    public Color invunerabilityColor = Color.white;

    public GameObject targetObjToDestroy;

    protected virtual void Start()
    {
        Init();

        flashColors = new List<FlashColor>();

        if(TryGetComponent<FlashColor>(out var f)) flashColors.Add(f);

        flashColors = GetComponentsInChildren<FlashColor>().ToList();

        if (gameObject.TryGetComponent(out FlashColor flash))
        {
            flashColors.Add(flash);
        }
    }

    public void Init()
    {
        ResetLife();
    }

    public virtual void ResetLife()
    {
        currentLife = maxLife;
    }

    public virtual void Death()
    {

        if (destroyOnKill)
        {
            Destroy(targetObjToDestroy);

            if (targetObjToDestroy == null)
                Destroy(gameObject);
        }

        Drop();
        OnDeath?.Invoke();
    }

    public virtual void Damage(float damage, Vector3 damageDirection, bool heavyAttack)
    {
        if (invulnerable) return;

        if (defending && !heavyAttack) return;

        //if(!string.IsNullOrEmpty(damageAudioName)) AudioManager.Instance.PlaySoundFXClip(transform.position, sfxName: damageAudioName, volume: 1);

        currentLife = Mathf.Clamp(currentLife - damage, 0, maxLife);

        if(currentLife <= 0)
        {
            Death();
            return;
        }

        if(invulnerabilityTime > 0) StartCoroutine(Invulnerable());


        if (doDamageEffect)
        {
            if (flashColors.Count > 0)
            {
                foreach(var color in flashColors)
                {
                    color.Flash(damageColor);
                }
            }
        }

        OnDamage?.Invoke(damageDirection);
    }

    public virtual void Heal(float heal)
    {
        float newLife = currentLife + heal;
        currentLife = Mathf.Min(newLife, maxLife);
    }

    protected IEnumerator Invulnerable()
    {
        invulnerable = true;

        if(doInvunerabilityEffect)
        {
            foreach(var flash in flashColors)
            {
                flash.FlashLoop(invunerabilityColor, invulnerabilityTime);
            }
        }

        yield return new WaitForSeconds(invulnerabilityTime);

        invulnerable = false;
    }

    public virtual void Drop ()
    {
        if (loot1 != null)
            Instantiate(loot1, lootPosition.position, Quaternion.identity);
    }
}
