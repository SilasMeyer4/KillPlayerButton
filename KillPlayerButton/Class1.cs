using BepInEx.Logging;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using GameNetcodeStuff;
using UnityEngine.InputSystem;

namespace KillPlayerButton
{
   [BepInPlugin(modGUID, modName, modVersion)]
   public class KillButton : BaseUnityPlugin
   {
      public const string modGUID = "SilasMeyer.KillPlayerButton";
      public const string modName = "KillPlayerButton";
      public const string modVersion = "0.0.1";

      private readonly Harmony harmony = new Harmony(modGUID);

      public static KillButton Instance;

      internal ManualLogSource mls;

      public InputAction killPlayerAction;





      void Awake()
      {
         if (Instance == null)
         {
            Instance = this;
         }
         mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);

         mls.LogMessage(modGUID + " is loading.");


         //Input 

         var inputActions = ScriptableObject.CreateInstance<InputActionAsset>();

         var actionMap = inputActions.AddActionMap("PlayerActions");
         killPlayerAction = actionMap.AddAction("KillPlayer", binding: "<keyboard>/equals");
         killPlayerAction.Enable();


         harmony.PatchAll(typeof(KillPlayerFix));

         mls.LogMessage(modGUID + " has loaded succesfully.");
      }

      private void OnDestroy()
      {
         killPlayerAction.Disable();
      }




   }

   [HarmonyPatch(typeof(PlayerControllerB), "Update")]
   public static class KillPlayerFix
   {

      [HarmonyPostfix]
      public static void Postfix()
      {
         if (KillButton.Instance.killPlayerAction != null)
         {
            if (KillButton.Instance.killPlayerAction.triggered)
            {
               PlayerControllerB localPlayer = StartOfRound.Instance.localPlayerController;
               localPlayer.KillPlayer(Vector3.zero, true, CauseOfDeath.Unknown, 0, Vector3.zero);
            }
         }
      }
   }
}
