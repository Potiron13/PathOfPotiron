using UnityEngine;
using Potiron.Core.Singletons;
using Unity.Netcode;
using System.Collections;

public class DamageControl : NetworkSingleton<DamageControl>
{
    [ServerRpc]
    public void ApplyDamageAbilityServerRpc(ulong targetId, ulong attackerId, string abilityName)
    {
        PlayerModel targetModel = NetworkManager.Singleton.ConnectedClients[targetId].PlayerObject.GetComponent<PlayerModel>();
        PlayerModel attackerModel = NetworkManager.Singleton.ConnectedClients[attackerId].PlayerObject.GetComponent<PlayerModel>();

        IDamageAbility ability = attackerModel.Character.abilitiesNew.GetDamageAbility(abilityName);

        targetModel.ApplyDamage(ability.GetDamage(attackerModel.Character, targetModel.Character), targetId);
    }

    [ServerRpc]
    public void ApplyDamageServerRpc(ulong targetId, ulong attackerId, string abilityName)
    {
        DamageModelOld damageModel = GetDamageModelOld(targetId, attackerId, abilityName);
        damageModel.TargetModel.ApplyDamage(DamageService.calculate(damageModel.AttackerModel.Character, damageModel.TargetModel.Character, damageModel.Ability), targetId);
    }

    [ServerRpc]
    public void ApplyDamageOverTimeServerRpc(ulong targetId, ulong attackerId, string abilityName)
    {
        DamageModelOld damageModel = GetDamageModelOld(targetId, attackerId, abilityName);
        StartCoroutine(ApplyDamageOverTime(targetId, damageModel.TargetModel, damageModel.TargetModel.Character, damageModel.Ability));
    }

    IEnumerator ApplyDamageOverTime(ulong targetId, PlayerModel targetModel, CharacterFromDB targetCharacter, CharacterAbility ability)
    {
        int count = 10;
        while (count >= 1)
        {
            targetModel.ApplyDamage(DamageService.calculate(targetCharacter, targetModel.Character, ability), targetId);         
            count--;
            yield return new WaitForSeconds(1f);
        }
    }

    private DamageModel GetDamageModel(ulong targetId, ulong attackerId, string abilityName)
    {
        PlayerModel targetModel = NetworkManager.Singleton.ConnectedClients[targetId]
            .PlayerObject.GetComponent<PlayerModel>();
        PlayerModel attackerModel = NetworkManager.Singleton.ConnectedClients[attackerId]
            .PlayerObject.GetComponent<PlayerModel>();

        IDamageAbility ability = attackerModel.Character.abilitiesNew.ProjectileAbilities.Find(a => a.Name == abilityName);

        DamageModel damageModel = new DamageModel();
        damageModel.TargetModel = targetModel;
        damageModel.AttackerModel = attackerModel;
        damageModel.DamageAbility = ability;

        return damageModel;
    }

    private DamageModelOld GetDamageModelOld(ulong targetId, ulong attackerId, string abilityName)
    {
        PlayerModel targetModel = NetworkManager.Singleton.ConnectedClients[targetId]
            .PlayerObject.GetComponent<PlayerModel>();
        PlayerModel attackerModel = NetworkManager.Singleton.ConnectedClients[attackerId]
            .PlayerObject.GetComponent<PlayerModel>();

        CharacterAbility ability = attackerModel.Character.abilities.Find(a => a.name == abilityName);

        DamageModelOld damageModel = new DamageModelOld();
        damageModel.TargetModel = targetModel;
        damageModel.AttackerModel = attackerModel;
        damageModel.Ability = ability;

        return damageModel;
    }

    class DamageModel
    {
        public PlayerModel TargetModel { get; set; }
        public PlayerModel AttackerModel { get; set; }
        public IDamageAbility DamageAbility { get; set; }
    }

    class DamageModelOld
    {
        public PlayerModel TargetModel { get; set; }
        public PlayerModel AttackerModel { get; set; }
        public CharacterAbility Ability { get; set; }
    }
}


