﻿using System.Linq;
using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace BattleUI {
    public class BonbonMenuAnimator : HandsOnStateAnimator {

        [SerializeField] private Transform[] decorations;
        private BonbonSlotAnimator[] slotAnimators;
        private BonbonMainHandler mainHandler;
        private BonbonBakeHandler bakeHandler;
        private BonbonFXInfo bonbonFXInfo;

        protected override void Awake() {
            base.Awake();
            mainHandler = GetComponent<BonbonMainHandler>();
            bakeHandler = GetComponent<BonbonBakeHandler>();

            slotAnimators = animators.Select(animator => animator as BonbonSlotAnimator).ToArray();
            foreach (Transform transform in decorations) transform.DOScale(0, 0);
            
            mainHandler.OnHandlerToggle += OnMainHandlerToggle;
            bakeHandler.OnHandlerToggle += OnBakeHandlerToggle;
        }

        public override void Init(UIAnimationBrain brain) {
            base.Init(brain);
            bakeHandler.OnBonbonModification += Brain.PropagateAnimationCall;
            Brain.OnBonbonAnimationCall += (info) => bonbonFXInfo = info;
        } 

        private void OnMainHandlerToggle(bool toggle) {
            StateHandler = mainHandler;
            if (bonbonFXInfo == null) base.UIStateHandler_OnHandlerToggle(toggle);
            else {
                foreach (BonbonSlotAnimator slotAnimator in slotAnimators) {
                    if (slotAnimator.Slot == bonbonFXInfo.animationSlot) {
                        slotAnimator.ToggleWithAnimation(bonbonFXInfo);
                    } else slotAnimator.Toggle(toggle);
                } bonbonFXInfo = null;
                state = toggle ? UIAnimatorState.Loading : UIAnimatorState.Unloading;
            }
        }
        
        private void OnBakeHandlerToggle(bool toggle) {
            StateHandler = bakeHandler;
            foreach (BonbonSlotAnimator slotAnimator in slotAnimators) slotAnimator.Toggle(toggle);
        }

        protected override IEnumerator Load() {
            foreach (Transform transform in decorations) {
                transform.DOScale(Vector2.one, animationDuration).SetEase(Ease.InOutCirc);
                yield return new WaitForSeconds(animationDuration / 2);
            } yield return null;
        }

        protected override IEnumerator Unload() {
            foreach (Transform transform in decorations) {
                transform.DOScale(Vector2.zero, animationDuration).SetEase(Ease.InOutQuart);
                yield return new WaitForSeconds(animationDuration / 2);
            } yield return null;
        }
    }
}