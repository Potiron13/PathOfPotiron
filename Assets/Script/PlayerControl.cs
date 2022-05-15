using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour, AnimatorControllerDispatcher.IAttackFrameReceiver
{
    public PlayerData playerData;
    public Camera playerCamera;
    public NavMeshAgent playerNavMeshAgent;
    public bool isAutoMode;
    public Transform target;
    public List<CharacterControl> petControls;
    public CooldownController cooldownController;
    public bool isStoped;
    public GameObject arrowGO;

    private Text HUDtext;
    private PlayerData targetData;
    private List<GameObject> targetGameObjectList;        
    private List<CharacterAbility> characterAbilities;
    private int currentAbilityIndex;    

    private void Awake()
    {
        TeamData teamData = LoadDataFromJson.LoadPlayerData();
        characterAbilities = teamData.Player.abilities;
        cooldownController.init(0);
        playerData.Init();
        targetGameObjectList = aquireTarget();
        petControls.ForEach(p => p.aquireTargetForPet(targetGameObjectList));
        isAutoMode = false;
        GameObject automodeButton = GameObject.Find("AutoHUD").transform.GetChild(0).gameObject;        
        HUDtext = automodeButton.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<Text>();
        automodeButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            handleAutoMode();
        });
        GameObject quitButton = GameObject.Find("QuitButton").transform.GetChild(0).gameObject;
        quitButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Menu");
        });        
    }

    private void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            handleAutoMode();
        }

        if (isAutoMode)
        {
            if (target == null)
            {
                playerNavMeshAgent.isStopped = false;
                for (int i = 0; i < targetGameObjectList.Count; i++)
                {
                    GameObject targetFameObject = targetGameObjectList[i];
                    if (targetFameObject != null)
                    {
                        target = targetFameObject.transform;
                        targetData = targetFameObject.GetComponent<PlayerData>();
                        return;
                    }
                }
            }
            else
            {
                if (playerData.CanAttackReach(playerData, targetData, currentAbilityIndex, playerNavMeshAgent))
                {                    
                    transform.rotation = PhysicsService.rotateCharacter(target.position, GetComponentInParent<Transform>().position, transform.rotation);
                    playerNavMeshAgent.isStopped = true;
                    if (cooldownController.isCooldownOver(currentAbilityIndex))
                    {
                        cooldownController.triggerCooldown(currentAbilityIndex);
                        startAsync(characterAbilities[currentAbilityIndex]);
                    }
                }
                else
                {
                    playerNavMeshAgent.isStopped = false;
                    playerNavMeshAgent.SetDestination(target.position);
                 }
            }
        }
        else
        {
            playerNavMeshAgent.isStopped = false;
            setTargetManualy();
            lookToTargetWithClick();
            resetTarget();
            lookToCursor();

            for (int i = 0; i < characterAbilities.Count; i++)
            {
                UseAbility(characterAbilities[i], i);
            }
        }
        if (isStoped)
        {
            playerNavMeshAgent.isStopped = true;            
        }
        if (playerData.statsSystem.CurrentHealth <= 0)
        {
            SceneManager.LoadScene("Menu");
        }
    }

    private void lookToCursor()
    {
        if (Input.GetButton("Shift"))
        {
            playerNavMeshAgent.isStopped = true;
            transform.rotation = PhysicsService.rotateCharacter(playerNavMeshAgent.steeringTarget, gameObject.transform.position, transform.rotation);
        }
    }

    private void lookToTargetWithClick()
    {
        if (target != null)
        {
            if (Input.GetMouseButton(0) && arrowGO != null)
            {
                playerNavMeshAgent.isStopped = true;
                transform.rotation = PhysicsService.rotateCharacter(target.transform.position, GetComponentInParent<Transform>().position, transform.rotation);
            }
        }
    }

    private void setTargetManualy()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Transform targetTransform = hit.transform;
                GameObject targetGO = targetTransform.gameObject;
                if (targetGO != null && (targetGO.tag == "enemy" || targetGO.tag == "boss"))
                {
                    target = targetTransform;
                    GameObject arrowPrefab = LoadFromResourcesService.prefab("Arrow");
                    Vector3 arrowPosition = new Vector3(target.position.x, target.position.y + 2, target.position.z);
                    arrowGO = Instantiate(arrowPrefab, arrowPosition, Quaternion.Euler(180, 0, 0), target.transform);
                }
            }
        }
    }

    private void resetTarget()
    {
        if (!Input.GetMouseButton(0))
        {
            if (target != null)
            {
                target = null;
            }
            if (arrowGO != null)
            {
                Destroy(arrowGO);
            }
        }
    }

    private void handleAutoMode()
    {
        isAutoMode = !isAutoMode;
        GetComponent<MovementController>().isAutomode = isAutoMode;
        petControls.ForEach(p =>
        {
            p.isAutomode = isAutoMode;
        });
        if (isAutoMode)
        {
            HUDtext.text = "Auto mode : on";
            targetGameObjectList = aquireTarget();
            petControls.ForEach(p => p.aquireTargetForPet(targetGameObjectList));
        }
        else
        {
            playerNavMeshAgent.SetDestination(transform.position);
            petControls.ForEach(p =>
            {
                p.target = null;
                if(p.characterNavMeshAgent != null)
                    p.characterNavMeshAgent.isStopped = true;
            });
            HUDtext.text = "Auto mode : off";
        }
    }

    private List<GameObject> aquireTarget()
    {
        List<GameObject> targetGameObjects = new List<GameObject>();
        for (int i = 0; i < LoadSceneData.enemies.Count; i++)
        {
            for (int j = 0; j < LoadSceneData.enemies[i].Count; j++)
            {
                targetGameObjects.Add(GameObject.Find(LoadSceneData.enemies[i][j].name + "Enemy" + i + j));
            }
        }
        for (int j = 0; j < LoadSceneData.bosses.Count; j++)
        {
            targetGameObjects.Add(GameObject.Find("boss" + j));
        }
        return targetGameObjects.Where(t => t!=null).OrderBy(t => Vector3.Distance(t.transform.position, transform.position)).ToList();
    }

    private void UseAbility(CharacterAbility characterAbilitie, int abilityIndex)
    {
        if (Input.GetButton(LoadSceneData.characterConfig.abilityConfigurations[abilityIndex].buttonName) && cooldownController.isCooldownOver(abilityIndex))
        {            
            cooldownController.triggerCooldown(abilityIndex);
            startAsync(characterAbilitie);
        }
    }

    private void startAsync(CharacterAbility characterAbilitie)
    {
        switch (characterAbilitie.type)
        {
            case "Projectile":
                StartCoroutine(Projectile(transform, characterAbilitie, playerNavMeshAgent));
                break;
            case "MeleeAOE":
                StartCoroutine(MeleeAOE(characterAbilitie));
                break;
            case "Beam":
                StartCoroutine(Beam(transform, characterAbilitie, playerNavMeshAgent));
                break;
            case "SpellAOE":
                StartCoroutine(CastAOE(transform, characterAbilitie, playerNavMeshAgent));
                break;
            case "Travel":                                
                gameObject.transform.position = PhysicsService.calculateAbilityPosition(gameObject.transform.position, playerNavMeshAgent.steeringTarget, characterAbilitie.range);
                break;            
            default:
                break;
        }
    }

    IEnumerator MeleeAOE(CharacterAbility characterAbility)
    {
        CrystalSupportService.CrystalBonus bonus = CrystalSupportService.handleSupportCrystals(characterAbility);
        Vector3 abilityPosition;
        if (isAutoMode)
        {
            abilityPosition = target.position;
        }
        else
        {
            abilityPosition = PhysicsService.calculateAbilityPosition(gameObject.transform.position, playerNavMeshAgent.steeringTarget, characterAbility.range);
        }
        for (int i = 0; i < bonus.attackNumber; i++)
        {
            foreach (int angle in bonus.offsetAngles)
            {
                GameObject attack = LoadFromResourcesService.abilityPrefab(characterAbility.name);
                GameObject meleeAOE = Instantiate(attack, transform.position + Quaternion.AngleAxis(angle, Vector3.up) * (transform.forward * 2), Quaternion.identity);
                PhysicsService.slameTheEath(meleeAOE, playerNavMeshAgent, transform, gameObject.tag, playerData, characterAbility);
                Destroy(meleeAOE, 5);
                yield return new WaitForSeconds(0.25f);                            
            }
        }
    }

    IEnumerator Projectile(Transform startTransform, CharacterAbility characterAbilitie, NavMeshAgent navMeshAgent)
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
        PhysicsService.shootOneBeam(projectileGameObject, navMeshAgent, startTransform, gameObject, playerData, characterAbility);
        Destroy(projectileGameObject, 5);
    }
    private void ShootOneProjectile(Transform startTransform, CharacterAbility characterAbility, NavMeshAgent navMeshAgent, int offsetAngle)
    {
        GameObject projectile = LoadFromResourcesService.abilityPrefab(characterAbility.name);
        GameObject projectileGameObject = Instantiate(projectile, startTransform.position + (startTransform.forward * 2), Quaternion.Euler(CrystalSupportService.rotateVectorOnY(startTransform.rotation.eulerAngles, offsetAngle)));
        PhysicsService.shootOneProjectile(projectileGameObject, navMeshAgent, startTransform, gameObject, playerData, characterAbility);
        Destroy(projectileGameObject, 5);
    } 

    IEnumerator CastAOE(Transform startTransform, CharacterAbility characterAbilitie, NavMeshAgent navMeshAgent)
    {
        CrystalSupportService.CrystalBonus bonus = CrystalSupportService.handleSupportCrystals(characterAbilitie);

        for (int i = 0; i < bonus.attackNumber; i++)
        {
            foreach (int angle in bonus.offsetAngles)
            {
                CastOneAOE(startTransform, characterAbilitie, navMeshAgent, angle);
            }
            yield return new WaitForSeconds(0.25f);
        }
    }

    private void CastOneAOE(Transform startTransform, CharacterAbility characterAbility, NavMeshAgent navMeshAgent, int offsetAngle)
    {
        GameObject aoe = LoadFromResourcesService.abilityPrefab(characterAbility.name);
        GameObject aoeGameObject = Instantiate(aoe, startTransform.position + Quaternion.AngleAxis(offsetAngle, Vector3.up) * (startTransform.forward * 2), Quaternion.identity);
        PhysicsService.castAOE(aoeGameObject, navMeshAgent, startTransform, gameObject, playerData, characterAbility);
        Destroy(aoeGameObject, 10);
    }

    public void switchCurrentAbility(int index)
    {
        currentAbilityIndex = index;
    }

    public void DeathFrame()
    {
        Debug.Log("dead");
    }

    public void AttackFrame()
    {
        throw new System.NotImplementedException();
    }

    public void MeleeAOEFrame()
    {
        throw new System.NotImplementedException();
    }

    public void ProjectileFrame()
    {
        throw new System.NotImplementedException();
    }

    public void BeamFrame()
    {
        throw new System.NotImplementedException();
    }

    public void SpellAOEFrame()
    {
        throw new System.NotImplementedException();
    }
}