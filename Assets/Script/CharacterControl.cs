using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class CharacterControl : MonoBehaviour, AnimatorControllerDispatcher.IAttackFrameReceiver
{
    public NavMeshAgent characterNavMeshAgent;
    public Transform target;
    public bool isAutomode;
    public int currentAbilityIndex = 0;
    public List<GameObject> targetGameObjectList;
    public int characterIndex;
    public CooldownController cooldownController;
    public bool isStoped;

    private PlayerData targetData;
    private PlayerData characterData;

    void Start()
    {
        TeamData teamData = LoadDataFromJson.LoadPlayerData();

        characterData = GetComponent<PlayerData>();
        characterData.Init();

        cooldownController.init(characterIndex);

        if (gameObject.tag != "player")
        {
            targetGameObjectList = new List<GameObject>();
            for (int i = 0; i < teamData.Pets.Count; i++)
            {
                targetGameObjectList.Add(GameObject.Find(teamData.Pets[i].name + "Pet" + i));
            }
            targetGameObjectList.Add(GameObject.Find("Player"));
        }
    }

    void Update()
    {
        if (tag == "player")
        {
            if (isAutomode)
            {
                if (target == null)
                {
                    characterNavMeshAgent.isStopped = false;
                    for (int i = 0; i < targetGameObjectList.Count; i++)
                    {
                        GameObject targetGameObject = targetGameObjectList[i];
                        if (targetGameObject != null)
                        {
                            setTarget(targetGameObject);
                            return;
                        }
                    }
                }
                else
                {
                    if (characterData.CanAttackReach(characterData, targetData, currentAbilityIndex, characterNavMeshAgent))
                    {
                        attackTarget();
                    }
                    else
                    {
                        moveToTarget();
                    }
                }
            }
            else
            {
                characterNavMeshAgent.isStopped = false;
                if (target == null)
                {
                    characterNavMeshAgent.isStopped = false;
                    for (int i = 0; i < targetGameObjectList.Count; i++)
                    {
                        GameObject targetGameObject = targetGameObjectList[i];
                        if (targetGameObject != null)
                        {
                            if (characterData.isInAggroRange(targetGameObject.transform.position))
                            {
                                setTarget(targetGameObject);
                                return;
                            }
                        }
                    }
                }
                else
                {
                    if (characterData.CanAttackReach(characterData, targetData, currentAbilityIndex, characterNavMeshAgent))
                    {
                        attackTarget();
                    }
                }
            }
        } else
        {
            if (target == null)
            {
                characterNavMeshAgent.isStopped = false;
                for (int i = 0; i < targetGameObjectList.Count; i++)
                {
                    GameObject targetGameObject = targetGameObjectList[i];
                    if (targetGameObject != null && (characterData.isInAggroRange(targetGameObject.transform.position) || characterData.isDamaged()))
                    {
                        setTarget(targetGameObject);
                        return;
                    }
                }
            } else
            {
                if (characterData.CanAttackReach(characterData, targetData, currentAbilityIndex, characterNavMeshAgent))
                {
                    attackTarget();
                }
                else
                {
                    moveToTarget();
                }
            }
        }
        
        if (characterData.statsSystem.CurrentHealth <= 0)
        {
            characterNavMeshAgent.isStopped = true;
            GetComponentInChildren<Animator>().SetTrigger("Death");
        }
        if (isStoped)
        {
            characterNavMeshAgent.isStopped = true;
        }
    }

    private void attackTarget()
    {
        GetComponentInChildren<Animator>().SetTrigger("Idle");
        transform.rotation = PhysicsService.rotateCharacter(target.position, GetComponentInParent<Transform>().position, transform.rotation);
        characterNavMeshAgent.isStopped = true;
        if (cooldownController.isCooldownOver(currentAbilityIndex))
        {
            CharacterAbility abilitie = characterData.statsSystem.stats.abilities[currentAbilityIndex];
            cooldownController.triggerCooldown(currentAbilityIndex);
            GetComponentInChildren<Animator>().SetTrigger(abilitie.type);
        }
    }

    private void moveToTarget()
    {
        GetComponentInChildren<Animator>().SetTrigger("Walk");
        characterNavMeshAgent.isStopped = false;
        characterNavMeshAgent.SetDestination(target.position);
    }

    private void setTarget(GameObject targetGameObject)
    {
        target = targetGameObject.transform;
        targetData = targetGameObject.GetComponent<PlayerData>();        
    }

    private void handleBossSwitchAbility()
    {
        if (gameObject.tag == "boss")
        {
            currentAbilityIndex = CalculateChosenAbilityService.switchCurrentAbility(characterData.character);
        }
    }

    public void switchCurrentAbility(int index)
    {
        currentAbilityIndex =  index;
    }

    public void AttackFrame()
    {
        if (!characterData.CanAttackReach(characterData, targetData, currentAbilityIndex, characterNavMeshAgent))
            return;
        targetData.Attack(characterData.character, characterData.statsSystem.stats.abilities[currentAbilityIndex]);
        handleBossSwitchAbility();
    }

    public void BeamFrame()
    {
        StartCoroutine(Beam(transform, characterData.statsSystem.stats.abilities[currentAbilityIndex], characterNavMeshAgent));
        handleBossSwitchAbility();
    }

    public void MeleeAOEFrame()
    {
        StartCoroutine(MeleeAOE(characterData.statsSystem.stats.abilities[currentAbilityIndex]));
        handleBossSwitchAbility();
    }

    public void ProjectileFrame()
    {
        StartCoroutine(Shoot(transform, characterData.statsSystem.stats.abilities[currentAbilityIndex], characterNavMeshAgent));
        handleBossSwitchAbility();
    }    
    public void SpellAOEFrame()
    {
        CharacterAbility characterAbilitie = characterData.statsSystem.stats.abilities[currentAbilityIndex];        
        StartCoroutine(CastAOE(characterAbilitie.effect == "Heal" ? transform : target.transform, characterAbilitie, characterNavMeshAgent));
        handleBossSwitchAbility();
    }    
    
    public void DeathFrame()
    {
        if (gameObject.tag == "boss")
        {
            GameObject[] bosses = GameObject.FindGameObjectsWithTag("boss");
            int remainingBosses = bosses.Where(b => b.GetComponent<CharacterControl>() != null).Count(); 
            if (remainingBosses <= 1)
            {
                SceneManager.LoadScene("EndMenu");
            }
        }
        // REFACTO
        if (gameObject.tag != "player" && characterData.character.craftingMaterialLoot != null && Random.Range(0, 100) > 95)
        {            
            GameObject lootHolder = new GameObject(characterData.character.craftingMaterialLoot.name);
            lootHolder.transform.position = gameObject.transform.position;
            GameObject lootPrefab = LoadFromResourcesService.lootPrefab(characterData.character.craftingMaterialLoot.getLootPrefabName());
            if (lootPrefab != null) {
                GameObject loot = Instantiate(lootPrefab, gameObject.transform.position, Quaternion.identity, lootHolder.transform);
                loot.AddComponent<LootController>().lootId = characterData.character.craftingMaterialLoot.id;
                loot.transform.Translate(new Vector3(0, 0.5f, 0));
            }            
        }
        Destroy(gameObject);
    }

    IEnumerator Shoot(Transform startTransform, CharacterAbility characterAbilitie, NavMeshAgent navMeshAgent)
    {        
        CrystalSupportService.CrystalBonus bonus = CrystalSupportService.handleSupportCrystals(characterAbilitie);

        for (int i = 0; i < bonus.attackNumber; i++)
        {
            foreach (int angle in bonus.offsetAngles)
            {
                ShootOneProjectile(startTransform, characterAbilitie, navMeshAgent, angle);
            }
            yield return new WaitForSeconds(0.25f);
        }
    }
    IEnumerator CastAOE(Transform startTransform, CharacterAbility characterAbilitie, NavMeshAgent navMeshAgent)
    {        
        CrystalSupportService.CrystalBonus bonus = CrystalSupportService.handleSupportCrystals(characterAbilitie);

        for (int i = 0; i < bonus.attackNumber; i++)
        {
            foreach (int angle in bonus.offsetAngles)
            {
                if(target != null)
                {
                    CastOneAOE(startTransform, characterAbilitie, navMeshAgent, angle);
                }                
            }
            yield return new WaitForSeconds(0.25f);
        }
    }

    IEnumerator MeleeAOE(CharacterAbility characterAbility)
    {
        CrystalSupportService.CrystalBonus bonus = CrystalSupportService.handleSupportCrystals(characterAbility);
        for (int i = 0; i < bonus.attackNumber; i++)
        {
            foreach (int angle in bonus.offsetAngles)
            {
                if (target != null)
                {
                    GameObject attack = LoadFromResourcesService.abilityPrefab(characterAbility.name);
                    GameObject meleeAOE = Instantiate(attack, transform.position + Quaternion.AngleAxis(angle, Vector3.up) * (transform.forward * 2), Quaternion.identity);
                    PhysicsService.slameTheEath(meleeAOE, characterNavMeshAgent, target.transform, gameObject.tag, characterData, characterAbility);
                    Destroy(meleeAOE, 5);
                }
            }
            yield return new WaitForSeconds(0.25f);
        }
    }
    IEnumerator Beam(Transform startTransform, CharacterAbility characterAbilitie, NavMeshAgent navMeshAgent)
    {
        CrystalSupportService.CrystalBonus bonus = CrystalSupportService.handleSupportCrystals(characterAbilitie);
        for (int i = 0; i < bonus.attackNumber; i++)
        {
            foreach (int angle in bonus.offsetAngles)
            {
                ShootOneBeam(startTransform, characterAbilitie, navMeshAgent, angle);
            }
            yield return new WaitForSeconds(0.25f);
        }
    }

    private void ShootOneBeam(Transform startTransform, CharacterAbility characterAbility, NavMeshAgent navMeshAgent, int offsetAngle)
    {
        GameObject projectile = LoadFromResourcesService.abilityPrefab(characterAbility.name);
        GameObject projectileGameObject = Instantiate(projectile, new Vector3(startTransform.position.x, startTransform.position.y, startTransform.position.z), Quaternion.Euler(CrystalSupportService.rotateVectorOnY(startTransform.rotation.eulerAngles, offsetAngle)));
        PhysicsService.shootOneBeam(projectileGameObject, navMeshAgent, startTransform, gameObject, characterData, characterAbility);
        Destroy(projectileGameObject, 5);
    }
    public void aquireTargetForPet(List<GameObject> targetGameObjectList)
    {
        this.targetGameObjectList = targetGameObjectList;
    }

    private void ShootOneProjectile(Transform startTransform, CharacterAbility characterAbility, NavMeshAgent navMeshAgent, int offsetAngle)
    {
        GameObject projectile = LoadFromResourcesService.abilityPrefab(characterAbility.name);
        GameObject projectileGameObject = Instantiate(projectile, startTransform.position + (startTransform.forward * 2), Quaternion.Euler(CrystalSupportService.rotateVectorOnY(startTransform.rotation.eulerAngles, offsetAngle)));
        PhysicsService.shootOneProjectile(projectileGameObject, navMeshAgent, startTransform, gameObject, characterData, characterAbility);
        Destroy(projectileGameObject, 5);
    }    
    
    private void CastOneAOE(Transform startTransform, CharacterAbility characterAbility, NavMeshAgent navMeshAgent, int offsetAngle)
    {        
        GameObject aoe = LoadFromResourcesService.abilityPrefab(characterAbility.name);
        GameObject aoeGameObject = Instantiate(aoe, startTransform.position + Quaternion.AngleAxis(offsetAngle, Vector3.up) * (startTransform.forward * 2), Quaternion.identity);
        PhysicsService.castAOE(aoeGameObject, navMeshAgent, startTransform, gameObject, characterData, characterAbility);
        Destroy(aoeGameObject, 10);
    }

}
