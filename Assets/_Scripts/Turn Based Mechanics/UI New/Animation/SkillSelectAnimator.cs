﻿using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace BattleUI {
    public class SkillSelectAnimator : HandsOnStateAnimator {

        [SerializeField] private Transform icon;
        [SerializeField] private Transform ribbon;
        [SerializeField] private Transform ribbon2;
        [SerializeField] private Transform display;
        
        protected override void Awake() {
            base.Awake();
            (StateHandler as SkillSelectHandler)
            .OnButtonArrange += SkillSelectHandler_OnButtonArrange;
            icon.DOScale(0f, 0f);
            ribbon.DOScaleX(0f, 0f);
            ribbon2.DOScaleX(0f, 0f);
            display.DOScaleY(0f, 0f);
        }

        private void SkillSelectHandler_OnButtonArrange() {
            animators = GetComponentsInChildren<UIAnimator>();
            foreach (UIAnimator animator in animators) {
                animator.Init(this);
                animator.Toggle(true);
            }
        }

        protected override IEnumerator Load() {
            icon.DOScale(1f, animationDuration);
            icon.DORotate(new Vector3(0, 0, 750), 2f, RotateMode.FastBeyond360).SetEase(Ease.InOutBack);
            yield return new WaitForSeconds(animationDuration);
            ribbon.DOScaleX(1f, 1f).SetEase(Ease.OutBounce);
            ribbon2.DOScaleX(1f, 1.5f).SetEase(Ease.OutBounce);
            yield return new WaitForSeconds(animationDuration);
            display.DOScaleY(1f, animationDuration);
        }

        protected override IEnumerator Unload() {
            ribbon.DOScaleX(0f, 0.2f).SetEase(Ease.OutBounce);
            ribbon2.DOScaleX(0f, 0.2f).SetEase(Ease.OutBounce);
            yield return new WaitForSeconds(animationDuration / 2);
            icon.DOScale(0f, animationDuration);
            display.DOScaleY(0f, animationDuration);
        }
    }
}