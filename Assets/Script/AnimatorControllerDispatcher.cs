using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorControllerDispatcher : MonoBehaviour
{
    
    public interface IAttackFrameReceiver
    {
        void AttackFrame();
        void MeleeAOEFrame();
        void ProjectileFrame();
        void SpellAOEFrame();
        void BeamFrame();
        void DeathFrame();
    }
    public MonoBehaviour AttackFrameReceiver;

    IAttackFrameReceiver m_AttackReceiver;   

    void Start()
    {
        if (AttackFrameReceiver != null)
        {
            m_AttackReceiver = AttackFrameReceiver as IAttackFrameReceiver;

            if (m_AttackReceiver == null)
            {
                Debug.LogError("The Monobehaviour set as Attack Frame Receiver don't implement the IAttackFrameReceiver interface!", AttackFrameReceiver);
            }
        }
    }
    public void AttackEvent()
    {
        m_AttackReceiver?.AttackFrame();
    }
        public void MeleeAOEEvent()
    {
        m_AttackReceiver?.MeleeAOEFrame();
    }

    public void ProjectileEvent()
    {
        m_AttackReceiver?.ProjectileFrame();
    }       
    public void SpellAOEEvent()
    {
        m_AttackReceiver?.SpellAOEFrame();
    }      
    public void BeamEvent()
    {
        m_AttackReceiver?.BeamFrame();
    }   
    public void DeathEvent()
    {
        m_AttackReceiver?.DeathFrame();
    }
}
