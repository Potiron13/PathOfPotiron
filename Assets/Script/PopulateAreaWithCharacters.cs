using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using static LoadFromResourcesService;
using static StatsSystem;

public class PopulateAreaWithCharacters : MonoBehaviour
{
    public Camera playerCamera;
    public List<GameObject> petHUD;
   
    private List<CharacterFromDB> pets;
    private CharacterFromDB player;
    private PlayerControl playerControl;

    void Awake()
    {
        TeamData teamData = LoadDataFromJson.LoadPlayerData();
        pets = teamData.Pets;
        player = teamData.Player;
        GameObject playerGameObject = GameObject.Find("Player");
        Rigidbody rigidbody = playerGameObject.AddComponent<Rigidbody>();
        rigidbody.isKinematic = true;
        Transform playerTransform = playerGameObject.transform;
        NavMeshAgent navMeshAgent = playerGameObject.GetComponent<NavMeshAgent>();
        navMeshAgent.speed = 5;
        navMeshAgent.angularSpeed = 10000;
        navMeshAgent.acceleration = 200;
        SphereCollider sphereCollider = playerGameObject.AddComponent<SphereCollider>();
        sphereCollider.radius = 2;
        sphereCollider.isTrigger = false;
        CooldownController cooldownController = playerGameObject.AddComponent<CooldownController>();
        LoadHUDForBattle loadHUDForBattle = GameObject.Find("HUD").GetComponent<LoadHUDForBattle>();        
        cooldownController.loadHUDForBattle = loadHUDForBattle;
        cooldownController.character = player;
        playerControl = playerGameObject.GetComponent<PlayerControl>();
        playerControl.cooldownController = cooldownController;
        loadHUDForBattle.playerControl = playerControl;

        // pets
        for (int i = 0; i < pets.Count; i++)
        {
            petHUD[i].SetActive(true);
            CharacterFromDB pet = pets[i];
            GameObject petGameObject = new GameObject(pet.name + "Pet" + i);
            petGameObject.tag = "player";

            GameObject sphereGameObject = instantiatePrefab("BlueSphere", Vector3.zero, ResourcesTypeEnum.DEFAULT);

            CharacterScript characterScript = attachScriptAndMeshAndInstantiate(petGameObject, pet, playerTransform, sphereGameObject, i+1);

            attachPetHealthBar(pet.name, i, characterScript.PlayerData);

            MovementController movementController = petGameObject.AddComponent<MovementController>();
            movementController.playerNavMeshAgent = characterScript.NavMeshAgent;
            movementController.playerCamera = playerCamera;
            movementController.walkableLayerMask = LayerMask.GetMask("Walkable");

            playerGameObject.GetComponent<PlayerControl>().petControls.Add(characterScript.CharacterControl);            
        }       
        // player        
        PlayerData playerData = playerGameObject.GetComponent<PlayerData>();
        playerData.statsSystem = new StatsSystem();
        playerData.statsSystem.stats = new Stats();
        playerData.statsSystem.stats.health = player.health;
        playerData.statsSystem.stats.strength = player.strength;
        playerData.statsSystem.stats.magic = player.magic;
        playerData.statsSystem.stats.abilities = player.abilities;
        playerData.character = player;
        playerGameObject.GetComponent<PlayerData>().statsSystem.stats.abilities = player.abilities;
        AuraController auraController = playerGameObject.AddComponent<AuraController>();
        auraController.auras = player.abilities.Where(a => a.type == "Aura").ToList();
        auraController.loadHUDForBattle = loadHUDForBattle;
        // enemy
        for (int i = 0; i < LoadSceneData.enemies.Count; i++)
        {
            spawnPack(LoadSceneData.enemies[i], i);
        }
        spawnBoss();
    }   

    private void spawnPack(List<CharacterFromDB> pack, int packIndex) 
    {        
        for (int i = 0; i < pack.Count; i++)
        {
            CharacterFromDB enemy = pack[i];
            spawnEnemy(packIndex, i, enemy, "enemy");
        }
    }

    private void spawnEnemy(int packIndex, int enemyIndex, CharacterFromDB enemy, string tag)
    {
        GameObject enemyGameObject = new GameObject(enemy.name + "Enemy" + packIndex + enemyIndex);
        enemyGameObject.tag = tag;
        GameObject sphereGameObject = instantiatePrefab("RedSphere", enemyGameObject.transform.position, ResourcesTypeEnum.DEFAULT);
        CharacterScript characterScript = attachScriptAndMeshAndInstantiate(enemyGameObject, enemy, gameObject.transform.GetChild(packIndex), sphereGameObject, enemyIndex);        
        attachEnemyHealthBar(enemyGameObject, characterScript.PlayerData);
    } 
    
    private void spawnBoss()
    {
        for (int i = 0; i < LoadSceneData.bosses.Count; i++)
        {
            GameObject bossGO = new GameObject("boss" + i);
            bossGO.tag = "boss";
            GameObject sphereGameObject = instantiatePrefab("BigRedSphere", bossGO.transform.position, ResourcesTypeEnum.DEFAULT);
            CharacterScript characterScript = attachScriptAndMeshAndInstantiate(bossGO, LoadSceneData.bosses[i], GameObject.Find("zoneBoss" + i).transform, sphereGameObject, 9999 + i);
            attachEnemyHealthBar(bossGO, characterScript.PlayerData);
            characterScript.NavMeshAgent.radius = 3;
        }
    }

    private CharacterScript attachScriptAndMeshAndInstantiate(GameObject characterGameObject, CharacterFromDB character, Transform spawningTransform, GameObject miniMapSphere, int characterIndex)
    {
        Vector3 spawningPosition = spawningTransform.position;
        Vector3 randomCharacterPosition = new Vector3(Random.Range(spawningPosition.x - 0.1F, spawningPosition.x + 0.1F), spawningPosition.y,
                    Random.Range(spawningPosition.z - 0.1F, spawningPosition.z + 0.1F));
        characterGameObject.transform.position = randomCharacterPosition;

        GameObject prefabGameObject = instantiatePrefab(character.name, randomCharacterPosition, ResourcesTypeEnum.MONSTER);        

        prefabGameObject.transform.parent = characterGameObject.transform;
        NavMeshAgent navMeshAgent = characterGameObject.AddComponent<NavMeshAgent>();
        navMeshAgent.speed = 5;        

        miniMapSphere.transform.parent = prefabGameObject.transform;
        miniMapSphere.transform.position = Vector3.zero;
        miniMapSphere.transform.localPosition = Vector3.zero;

        SphereCollider sphereCollider = characterGameObject.AddComponent<SphereCollider>();
        sphereCollider.radius = characterGameObject.tag == "boss" ? 3 : 1f;
        sphereCollider.isTrigger = false;

        Rigidbody rigidbody = characterGameObject.AddComponent<Rigidbody>();
        rigidbody.isKinematic = true;       

        PlayerData playerData = characterGameObject.AddComponent<PlayerData>();
        playerData.statsSystem = new StatsSystem();
        playerData.statsSystem.stats = new Stats();
        playerData.statsSystem.stats.health = character.health;
        playerData.statsSystem.stats.strength = character.strength;
        playerData.statsSystem.stats.magic = character.magic;
        playerData.statsSystem.stats.abilities = character.abilities;
        playerData.character = character;

        CharacterControl characterControl = characterGameObject.AddComponent<CharacterControl>();
        characterControl.characterNavMeshAgent = characterGameObject.GetComponent<NavMeshAgent>();
        characterControl.characterIndex = characterIndex;

        AuraController auraController = characterGameObject.AddComponent<AuraController>();
        auraController.auras = character.abilities.Where(a => a.type == "Aura").ToList();

        CooldownController cooldownControllerPet = characterGameObject.AddComponent<CooldownController>();
        if (characterGameObject.tag == "player")
        {
            LoadHUDForBattle loadHUDForBattle = GameObject.Find("HUD").GetComponent<LoadHUDForBattle>();
            cooldownControllerPet.loadHUDForBattle = loadHUDForBattle;
            loadHUDForBattle.characterControls.Add(characterControl);
            auraController.loadHUDForBattle = loadHUDForBattle;
        }
        else
        {
            cooldownControllerPet.loadHUDForBattle = null;            
        }
        cooldownControllerPet.character = character;
        characterGameObject.GetComponent<CharacterControl>().cooldownController = cooldownControllerPet;

        AnimatorControllerDispatcher animatorControllerDispatcher = prefabGameObject.AddComponent<AnimatorControllerDispatcher>();
        animatorControllerDispatcher.AttackFrameReceiver = characterControl;

        CharacterScript characterScript = new CharacterScript();
        characterScript.PlayerData = playerData;
        characterScript.NavMeshAgent = navMeshAgent;
        characterScript.CharacterControl = characterControl;

        return characterScript;
    }

    private static GameObject instantiatePrefab(string prefabName, Vector3 position, ResourcesTypeEnum resourcesType)
    {
        GameObject prefab = withType(resourcesType, prefabName);    
        GameObject prefabGameObject = Instantiate(prefab, position, Quaternion.identity);
        return prefabGameObject;
    }

    private void attachPetHealthBar(string pet, int i, PlayerData playerData)
    {              
        playerData.healthBar = GameObject.Find("HealthBarPet" + i).GetComponent<HealthBar>();
    }

    private void attachEnemyHealthBar(GameObject enemyGameObject, PlayerData playerData)
    {
        GameObject healthbarResource = LoadFromResourcesService.prefab("EnemyHealthBar");
        GameObject healthbarGameObject = Instantiate(healthbarResource, enemyGameObject.transform.position, Quaternion.identity);
        healthbarGameObject.transform.parent = enemyGameObject.transform;
        Billboard billboard = healthbarGameObject.AddComponent<Billboard>();
        billboard.cam = playerCamera.transform;
        playerData.healthBar = healthbarGameObject.GetComponentInChildren<HealthBar>();        
    }

    class CharacterScript
    {
        public PlayerData PlayerData { get; set; }
        public NavMeshAgent NavMeshAgent { get; set; }
        public CharacterControl CharacterControl { get; set; }
    }
}
