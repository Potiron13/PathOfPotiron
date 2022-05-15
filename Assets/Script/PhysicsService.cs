using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public static class PhysicsService
{
    public static Quaternion rotateCharacter(Vector3 targetPosition, Vector3 pivotPosition, Quaternion rotation)
    {
        Vector3 direction = (targetPosition - pivotPosition).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        return Quaternion.Slerp(rotation, lookRotation, Time.deltaTime * 10f);
    }

    public static void shootOneProjectile(GameObject projectileGameObject,
        NavMeshAgent navMeshAgent,
        Transform startTransform,
        GameObject gameObject,
        PlayerData attacker, 
        CharacterAbility ability)
    {
        //navMeshAgent.SetDestination(startTransform.position);
        projectileGameObject.tag = gameObject.tag;
        projectileGameObject.GetComponentInChildren<RFX4_PhysicsMotion>().attacker = attacker;
        projectileGameObject.GetComponentInChildren<RFX4_PhysicsMotion>().ability = ability;
    }  
    
    public static void castAOE(GameObject projectileGameObject,
        NavMeshAgent navMeshAgent,
        Transform startTransform,
        GameObject gameObject,
        PlayerData attacker, 
        CharacterAbility ability)
    {
        navMeshAgent.SetDestination(startTransform.position);
        projectileGameObject.tag = gameObject.tag;
        projectileGameObject.GetComponentInChildren<SpellAOEController>().attacker = attacker;
        projectileGameObject.GetComponentInChildren<SpellAOEController>().ability= ability;        
    }

    public static void shootOneBeam(GameObject projectileGameObject,
       NavMeshAgent navMeshAgent,
       Transform startTransform,
       GameObject gameObject,
       PlayerData attacker, 
       CharacterAbility ability)
    {
        navMeshAgent.SetDestination(startTransform.position);
        projectileGameObject.tag = gameObject.tag;
        projectileGameObject.GetComponentInChildren<RFX4_RaycastCollision>().attacker = attacker;
        projectileGameObject.GetComponentInChildren<RFX4_RaycastCollision>().ability = ability;
    }

    public static void slameTheEath(GameObject meleeAOE, 
        NavMeshAgent navMeshAgent, 
        Transform transform, 
        string tag, 
        PlayerData attacker, 
        CharacterAbility ability)
    {
        navMeshAgent.SetDestination(transform.position);
        meleeAOE.tag = tag;
        meleeAOE.GetComponent<MeleeAOEController>().attacker = attacker;
        meleeAOE.GetComponent<MeleeAOEController>().ability = ability;
    }

    public static Vector3 calculateAbilityPosition(Vector3 originPosition, Vector3 targetPosition, float abilityRange)
    {
        return originPosition + (targetPosition - originPosition).normalized * abilityRange;
    }
}
