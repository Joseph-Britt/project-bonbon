using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BattleStateMachine;

public class BattleStateInput : StateInput {

    #region Global Variables
    private List<Actor> turnQueue;
    private int currActorIndex = 0;
    private int currentTurn = 0;
    #endregion Global Variables

    #region | Events |

    public event System.Func<SkillAction, Actor[], BonbonObject, ActiveSkillPrep> OnSkillUpdate;
    public void UpdateSkill(SkillAction sa, Actor[] targets, BonbonObject bonbon = null) => OnSkillUpdate?.Invoke(sa, targets, bonbon);

    public event System.Func<ActiveSkillPrep> OnRetrieveSkillPrep;
    public ActiveSkillPrep SkillPrep => OnRetrieveSkillPrep?.Invoke();

    public event System.Action OnSkillReset;
    public void ResetSkill() => OnSkillReset?.Invoke();

    public event System.Func<ActiveSkillPrep> OnSkillActivate;
    public ActiveSkillPrep ActivateSkill() => OnSkillActivate?.Invoke();

    public event System.Action<SkillAction> OnSkillAnimation;
    public void AnimateSkill(SkillAction sa) => OnSkillAnimation?.Invoke(sa);

    #endregion

    #region Managers
    public BonbonFactory BonbonFactory { get; private set; }
    #endregion

    public void Initialize() { }

    public void InsertTurnQueue(List<Actor> queue) {
        turnQueue = queue;
    }

    public void OpenBonbonFactory(BonbonFactory bonbonFactory) {
        BonbonFactory = bonbonFactory;
        BonbonFactory.OpenFactory(GameManager.CurrLevel);
    }

    /// <summary> Advances until the next undefeated Actor. Returns to initial Actor if not available.</summary>
    public void AdvanceTurn() 
    {
        Actor initialActor = ActiveActor();
        do {
            currActorIndex = (currActorIndex + 1) % turnQueue.Count;
        } while (ActiveActor().Defeated && !initialActor.Equals(ActiveActor()));
        currentTurn++;
    }

    public Actor ActiveActor() {
        return turnQueue[currActorIndex];
    }

    public int CurrTurn() {
        return currentTurn;
    }

    public List<Actor> GetTurnQueue()
    {
        return turnQueue;
    }
}
