using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleUI {
    public partial class BonbonSlotAnimator {
    
        /// Enumerator Requirements:
        /// - Must set the animator state to 'Idle' at the end;
        /// - Must update the icon at some point by calling the UpdateIcon() method;
        /// Recommendations & Notes:
        /// - The next state will lerp the scale towards 1 or 1.2 depending on input;
        /// - The animation should last between 0.2 and 2 seconds;
        /// - The Logic won't be affected by this animation, this means:
        ///     - The button may exit the coroutine at any moment;
        ///     - The button may be scaled down before the animation is finished;
        /// 
        /// Any local parameters processed in the Coroutine should be reset *conditionally* in ResolveAnimations();
        /// This applies to both Crafting and Baking Animations;
        /// Animations encompassing several slots are more complex and require a stronger framework;
        /// 
        /// You may use any [SerializeField] at the top of this script to reference VFX assets and such;
        /// Feel free to delete this long-a$$ message at any time;

        /// BonbonCraftInfo contains the following variable:
        /// bonbon: The BonbonObject to be crafted (from which you can access the icon);

        private IEnumerator CraftAnimation(BonbonCraftInfo info) {
            yield return null;
            state = UIAnimatorState.Idle;
        }


        /// BonbonBakeInfo contains the following variables:
        /// ingredients: An array of BonbonObjects featuring the two ingredients that were baked;
        /// result: The BonbonObject that will be placed in the slot after combining the ingredients;

        private IEnumerator BakeAnimation(BonbonBakeInfo info) {
            yield return null;
            state = UIAnimatorState.Idle;
        }

        private void ResolveAnimations() {
            if (specialAnimation != null) {
                StopCoroutine(specialAnimation);
                /// Reset local parameters;
            }
        }
    }
}
