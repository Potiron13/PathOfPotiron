using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(NetworkObject))]
public class PlayerControls : NetworkBehaviour
{

    [SerializeField]
    private Vector2 defaultInitialPositionOnPlane = new Vector2(-4, 4);

    [SerializeField]
    private NavMeshAgent navMeshAgent;

    [SerializeField]
    private NetworkVariable<PlayerState> networkPlayerState = new NetworkVariable<PlayerState>();

    private LayerMask walkableLayerMask;
    private Animator animator;
    private AbilityManager abilityManager;
    private PlayerState oldPlayerState = PlayerState.Idle;
    private long oldHealth;
    private int oldMagic;
    private int oldStrength;
    private int oldDefense;
    private Camera playerCamera;
    private PlayerModel playerModel;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        abilityManager = GetComponent<AbilityManager>();
        abilityManager.Init();
    }

    void Start()
    {
        gameObject.tag = "player";
        walkableLayerMask = LayerMask.GetMask("Walkable");
        playerCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        playerModel = GetComponent<PlayerModel>();
        playerModel.Init();
        CombatUIControl.Instance.Init(playerModel);
        oldHealth = playerModel.GetHealth();
        oldMagic = playerModel.GetMagic();
        oldStrength = playerModel.GetStrength();
        oldDefense = playerModel.GetDefense();

        if (IsClient && IsOwner)
        {
            transform.position = new Vector3(Random.Range(defaultInitialPositionOnPlane.x, defaultInitialPositionOnPlane.y), 0,
                   Random.Range(defaultInitialPositionOnPlane.x, defaultInitialPositionOnPlane.y));
            
            List<CharacterAbility> abilities = playerModel.Character.abilities.Where(a => a.type == "Aura").ToList();
            foreach (CharacterAbility ability in abilities)
            {
                abilityManager.InstanciateAuraServerRpc(ability.name, ability.attribute, ability.value);
                if (ability.attribute == "speed")
                {
                    GetComponent<NavMeshAgent>().speed += ability.value;
                }
                else
                {
                    abilityManager.UpdateStatsServerRpc(OwnerClientId, ability.attribute, ability.value);
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (IsClient && IsOwner)
        {
            ClientMovement();
            ClientAbilities();
            ClientTargeting();            
        }

        ClientStats();
        ClientVisuals();        
    }

    private void ClientTargeting()
    {
        lookToCursor();
    }

    private void lookToCursor()
    {
        if (Input.GetButton("Shift"))
        {
            navMeshAgent.isStopped = true;
            transform.rotation = PhysicsService.rotateCharacter(navMeshAgent.steeringTarget, gameObject.transform.position, transform.rotation);
        }
    }

    private void ClientAbilities()
    {
        List<CharacterAbility> abilities = playerModel.Character.abilities;
        for (int i = 0; i < abilities.Count; i++)
        {
            if (abilities[i].type != "Aura" && abilities[i].type != "Passif" && Input.GetButton(LoadSceneData.characterConfig.abilityConfigurations[i].buttonName) && CombatUIControl.Instance.isCooldownOver(i))
            {
                CombatUIControl.Instance.triggerCooldown(i);
                CrystalSupportService.CrystalBonus bonus = CrystalSupportService.handleSupportCrystals(abilities[i]);
                abilityManager.UseAbilityServerRpc(
                    gameObject.GetComponent<NetworkObject>().OwnerClientId,
                    abilities[i].name, gameObject.transform.position, gameObject.transform.forward, gameObject.transform.rotation.eulerAngles,
                    bonus.angleNumber, bonus.attackNumber, navMeshAgent.steeringTarget, abilities[i].range
                );
                abilityManager.UseAbilityClientSide(navMeshAgent, abilities[i]); 
            }
        }
    }   

    private void ClientVisuals()
    {
        if (oldPlayerState != networkPlayerState.Value)
        {            
            oldPlayerState = networkPlayerState.Value;
            animator.SetTrigger($"{System.Enum.GetName(typeof(PlayerState), (int)networkPlayerState.Value)}");
        }
    }

    private void ClientStats()
    {
        if (oldHealth != playerModel.GetHealth())
        {
            playerModel.UpdateHealthBar();
            oldHealth = playerModel.GetHealth();
        }
        if (oldMagic != playerModel.GetMagic() || oldDefense != playerModel.GetDefense() 
            || oldStrength != playerModel.GetStrength())
        {
            playerModel.UpdateStats();
            oldMagic = playerModel.GetMagic();
            oldDefense = playerModel.GetDefense();
            oldStrength = playerModel.GetStrength();
        }
    }

    private void ClientMovement()
    {
        if (Input.GetMouseButton(0))
        {
            navMeshAgent.isStopped = false;
            Ray myRay = playerCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit myRaycastHit;

            if (Physics.Raycast(myRay, out myRaycastHit, 500, walkableLayerMask))
            {
                navMeshAgent.SetDestination(myRaycastHit.point);
            }
        }
        PlayerState state = navMeshAgent.remainingDistance < 0.5 ? PlayerState.Idle : PlayerState.Walk;
        UpdatePlayerStateServerRpc(state);
    }   

    [ServerRpc]
    public void UpdatePlayerStateServerRpc(PlayerState state)
    {
        networkPlayerState.Value = state;
    }


}
