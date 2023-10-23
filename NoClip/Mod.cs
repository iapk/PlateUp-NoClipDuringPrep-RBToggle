using Kitchen;
using KitchenMods;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NoClipPrepToggle
{
    public class NoClipSystem : GenericSystemBase, IModSystem {
        private const int LayerPlayers = 12;
        private const int LayerStatics = 9;
        private const int LayerDefault = 0;
        private bool PrevPrepTime = false;

        private bool noClipPref = true;
        private SceneType prevScene = SceneType.Null;

        //Toggles NoClip based on if the current scene is "kitchen", if it is "prep time", and the players NoClip preference
        private void ToggleNoClip()
        {
            //When not in the "Kitchen" scene NoClip is disabled
            if(GameInfo.CurrentScene != SceneType.Kitchen)
            {
                Physics.IgnoreLayerCollision(LayerPlayers, LayerDefault, false);
                Physics.IgnoreLayerCollision(LayerPlayers, LayerStatics, false);
                return;//ignores rest
            }

            //Everything here and bellow assumes scene is kitchen
            //If prep time, NoClip based on pref
            if(GameInfo.IsPreparationTime)
            {
                Physics.IgnoreLayerCollision(LayerPlayers, LayerDefault, noClipPref);
                Physics.IgnoreLayerCollision(LayerPlayers, LayerStatics, noClipPref);
                Debug.Log($"NoClipPrepToggle Mod: NoClip is now {(noClipPref ? "on" : "off")}.");
            }
            //if not prep time or correct scene, noClip disabled
            else
            {
                Physics.IgnoreLayerCollision(LayerPlayers, LayerDefault, false);
                Physics.IgnoreLayerCollision(LayerPlayers, LayerStatics, false);
                Debug.Log("NoClipPrepToggle Mod: NoClip is now off.");
            }
        }

        protected override void OnUpdate() {
            //When prep time swaps
            if (GameInfo.IsPreparationTime != PrevPrepTime)
            {
                PrevPrepTime = GameInfo.IsPreparationTime;
                ToggleNoClip();
            }

            //When we leave/ rejoin the kitchen scene noClip corrections need to be made
            if(GameInfo.CurrentScene != prevScene)
            {
                ToggleNoClip();
                prevScene = GameInfo.CurrentScene;
            }

            //Checks for input from player to toggle noClip (only changeable while in prep)
            if ((Gamepad.current.rightShoulder.wasPressedThisFrame || Input.GetKeyDown("n")) && GameInfo.IsPreparationTime == true && GameInfo.CurrentScene == SceneType.Kitchen)
            {
                noClipPref = !noClipPref;
                ToggleNoClip();
            }
        }
    }
}
