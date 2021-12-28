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

        public override void OnApplicationStart()
        {
            base.OnApplicationStart();
            tactsuitVr = new TactsuitVR();
            tactsuitVr.PlaybackHaptics("HeartBeat");
        }

        [HarmonyPatch(typeof(DragonHitController), "GetHit")]
        public class bhaptics_HitByDragon
        {
            [HarmonyPostfix]
            public static void Postfix()
            {

                //tactsuitVr.StopThreads();
            }
        }

    }
}
