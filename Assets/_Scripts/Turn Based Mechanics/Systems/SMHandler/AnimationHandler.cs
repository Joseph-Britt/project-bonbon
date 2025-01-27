using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnimationHandler : StateMachineHandler {

    [SerializeField] private SkillAnimationMap skillAnimationMap;
    private BattleStateMachine battleStateMachine => BattleStateMachine.Instance;

    #region Events
    public event Action<int, Actor, bool> DamageEvent;
    public event Action<int, Actor> HealEvent;
    public event Action<EffectBlueprint, Actor> EffectEvent;
    public event Action<Actor> StaminaEvent;
    #endregion Events
    public Dictionary<SkillObject, Dictionary<ActorData, SkillAnimation>> SkillAMap { get; private set; }

    void Awake() {
        SkillAMap = SKAEUtils.ProcessInternalDictionary(skillAnimationMap.animationMap);
    }

    public override void Initialize(BattleStateInput input) {
        base.Initialize(input);
        input.SkillHandler.OnSkillTrigger += OnSkillTrigger;
        input.VFXHandler.Connect(this);
    }

    public void OnSkillTrigger(ActiveSkillPrep skillPrep) {
        SkillAction skillAction = skillPrep.skill;
        BonbonObject bonbon = skillPrep.bonbon;
        try {
            SkillAnimation sAnim = SkillAMap[skillAction.SkillData][skillAction.Caster.Data];

            CameraAnimationPackage cap = sAnim.CameraAnimationPackage;
            if (cap != null) input.CameraHandler.PlayAnimation(cap);
            Animator casterAnimator = skillAction.Caster.GetComponentInChildren<Animator>(true);
            casterAnimator.SetTrigger(sAnim.AnimationTrigger);

            battleStateMachine.StartBattle(sAnim.AnimationDuration);
            if (bonbon != null) ; /// Do VFXs

            CompileAnimationSequence(skillPrep, sAnim);
        } catch (KeyNotFoundException) {
            Debug.LogWarning($"Animation Undefined for {skillAction.SkillData.Name} -> {skillAction.Caster.Data.DisplayName}");
        }
    }

    private void CompileAnimationSequence(ActiveSkillPrep skillPrep, SkillAnimation sAnim) {
        List<(PercentTrigger, DelaySkillAnimation)> triggers = new();
        foreach (DelaySkillAnimation dsa in sAnim.DelaySkills) {
            if (dsa is DelaySkillPercentAnimation) {
                (dsa as DelaySkillPercentAnimation).triggers.ForEach(trigger => triggers.Add((trigger, dsa)));
            }
        } triggers.Sort((e1, e2) => Math.Sign(e1.Item1.TriggerTime - e2.Item1.TriggerTime));

        List<AIActionValue> avs = skillPrep.targets.Select(target => skillPrep.skill
                                                           .ComputeSkillActionValues(target, skillPrep.bonbon)).ToList();

        Queue<(float, Action)> actionQueue = new();

        float prevDuration = 0;
        foreach ((PercentTrigger, DelaySkillAnimation) trigger in triggers) {
            if (trigger.Item2 is DelaySkillDamageAnimation) {
                PercentTrigger data = trigger.Item1;
                actionQueue.Enqueue((data.TriggerTime - prevDuration,
                                    () => Enumerable.Range(0, avs.Count).ToList()
                                          .ForEach(i => TriggerDamage((int) (avs[i].immediateDamage * data.Multiplier),
                                                                      skillPrep.targets[i], skillPrep.bonbon != null))));
            } prevDuration += trigger.Item1.TriggerTime - prevDuration;
        } StartCoroutine(SkillAnimation(actionQueue));
    }

    private IEnumerator SkillAnimation(Queue<(float, Action)> actionQueue) {
        while (actionQueue.Count > 0) {
            (float, Action) actionTuple = actionQueue.Dequeue();
            yield return new WaitForSeconds(actionTuple.Item1);
            actionTuple.Item2?.Invoke();
        }
    }

    #region Events
    public void TriggerDamage(int damage, Actor actor, bool augmented = false) {
        DamageEvent?.Invoke(damage, actor, augmented);
    }

    public void TriggerHeal(int heal, Actor actor) {
        HealEvent?.Invoke(heal, actor);
    }

    public void TriggerEffect(EffectBlueprint effect, Actor actor) {
        EffectEvent?.Invoke(effect, actor);
    }

    public void TriggerStamina(Actor actor) {
        StaminaEvent?.Invoke(actor);
    }
    #endregion Events
}
