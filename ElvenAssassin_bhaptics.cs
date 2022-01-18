using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

using MelonLoader;
using HarmonyLib;

using MyBhapticsTactsuit;


namespace ElvenAssassin_bhaptics
{
    public class ElvenAssassin_bhaptics : MelonMod
    {
        public static TactsuitVR tactsuitVr;
        public static bool isRightHanded = true;

        public override void OnApplicationStart()
        {
            base.OnApplicationStart();
            tactsuitVr = new TactsuitVR();
            tactsuitVr.PlaybackHaptics("HeartBeat");
        }

        #region Shoot bow
        [HarmonyPatch(typeof(HandsDominanceSwitcher), "InitializeWithPlayer", new Type[] { typeof(bool) })]
        public class bhaptics_HandsDominance
        {
            [HarmonyPostfix]
            public static void Postfix(HandsDominanceSwitcher __instance, bool isLocalPlayer)
            {
                if (!isLocalPlayer) return;
                if (__instance.HandsDominance == HandsDominanceSwitcher.HandsDominanceType.Left) isRightHanded = false;
            }
        }

        [HarmonyPatch(typeof(WenklyStudio.BowController), "Shoot", new Type[] { })]
        public class bhaptics_ShootBow
        {
            [HarmonyPostfix]
            public static void Postfix(WenklyStudio.BowController __instance)
            {
                if (!__instance.IsHandAttached) return;
                if (isRightHanded) tactsuitVr.PlaybackHaptics("BowRelease_R");
                else tactsuitVr.PlaybackHaptics("BowRelease_L");
            }
        }
        #endregion

        #region Get hit
        [HarmonyPatch(typeof(WenklyStudio.ElvenAssassin.DragonAttackControler), "KillPlayer", new Type[] { typeof(WenklyStudio.ElvenAssassin.PlayerController) })]
        public class bhaptics_DragonKillPlayer
        {
            [HarmonyPostfix]
            public static void Postfix(WenklyStudio.ElvenAssassin.PlayerController playerToBeKilled)
            {
                if (playerToBeKilled != PlayersManager.Instance.LocalPlayer) return;
                tactsuitVr.PlaybackHaptics("FlameThrower");
            }
        }

        #endregion
    }
}
