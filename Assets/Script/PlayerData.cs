using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class PlayerData : MonoBehaviour
{
    [HideInInspector]
    public StatsSystem statsSystem;
    public HealthBar healthBar;
    public CharacterFromDB character;

    public void Init()
    {
        statsSystem.Init(this);
        healthBar.setMaxHealth();
    }

    public bool isDamaged()
    {
        return healthBar.image.fillAmount < 1;
    }

    public int Damage(CharacterFromDB attacker, CharacterFromDB target, CharacterAbility ability)
    {
        int damage = DamageService.calculate(attacker, target, ability);
        StartCoroutine(handleEffect(ability));
        statsSystem.Damage(damage);        
        healthBar.setHealth(statsSystem.CurrentHealth / (float) statsSystem.stats.health);
        return damage;
    }

    public int Heal(CharacterFromDB healer, CharacterFromDB target, CharacterAbility ability)
    {
        int damage = -DamageService.calculate(healer, target, ability);
        StartCoroutine(handleEffect(ability));
        statsSystem.Damage(damage);
        healthBar.setHealth(statsSystem.CurrentHealth / (float)statsSystem.stats.health);
        return damage;
    }

    IEnumerator handleEffect(CharacterAbility ability)
    {
        if (ability.effect != null && ability.effect == "Stop")
        {
            setIsStoped(true);
            yield return new WaitForSeconds(5f);
            setIsStoped(false);
        }

    }

    private void setIsStoped(bool isStoped)
    {
        MovementController movementController = GetComponent<MovementController>();
        if(movementController != null)
        {
            movementController.isStoped = isStoped;
        }        
        PlayerControl playerControl = GetComponent<PlayerControl>();
        if (playerControl != null)
        {
            playerControl.isStoped = isStoped;
        }
        else
        {
            GetComponent<CharacterControl>().isStoped = isStoped;
        }
    }

    public void Attack(CharacterFromDB attacker, CharacterAbility ability)
    {
        int damage = Damage(attacker, character, ability);
        applyDamage(damage, gameObject);
        List<CharacterAbility> reflectionAbilities = character.abilities.Where(a => a.effect == "Reflect").ToList();
        foreach(CharacterAbility a in reflectionAbilities)
        {
            Damage(character, attacker, a);
        }
    }
    public void Heal(CharacterFromDB healer, CharacterAbility ability)
    {
        int damage = Heal(healer, character, ability);
        applyDamage(damage, gameObject);
    }

    private void applyDamage(int damage, GameObject targetGO)
    {
        GameObject damageHolderPrefab = damage > 0 ? LoadFromResourcesService.prefab("DamagePopUp") : LoadFromResourcesService.prefab("HealPopUp");
        GameObject damageHolder = Instantiate(damageHolderPrefab, targetGO.transform.position, Quaternion.identity, targetGO.transform);
        Destroy(HUDService.displayText(damage > 0 ? damage.ToString() : (-damage).ToString(), targetGO.transform.position, damageHolder), 0.5f);
    }

    public bool CanAttackReach(PlayerData attacker, PlayerData target, int abilityIndex, NavMeshAgent navMeshAgent)
    {
        if (target == null)
            return false;
        float range = attacker.statsSystem.stats.abilities[abilityIndex].range + navMeshAgent.radius;
        return Vector3.SqrMagnitude(attacker.transform.position - target.transform.position) < range * range;
    }

    public bool isInAggroRange(Vector3 targetPosition)
    {
        return Vector3.SqrMagnitude(GetComponentInParent<Transform>().position - targetPosition) < 150f;
    }

}
