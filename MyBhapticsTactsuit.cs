﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MelonLoader;

namespace MyBhapticsTactsuit
{
    public class TactsuitVR
    {
        public bool suitDisabled = true;
        public bool systemInitialized = false;
        private static ManualResetEvent HeartBeat_mrse = new ManualResetEvent(false);
        private static ManualResetEvent Water_mrse = new ManualResetEvent(false);
        private static ManualResetEvent Choking_mrse = new ManualResetEvent(false);
        public Dictionary<String, FileInfo> FeedbackMap = new Dictionary<String, FileInfo>();

        private static bHapticsLib.RotationOption defaultRotationOption = new bHapticsLib.RotationOption(0.0f, 0.0f);

        public void HeartBeatFunc()
        {
            while (true)
            {
                HeartBeat_mrse.WaitOne();
                bHapticsLib.bHapticsManager.PlayRegistered("HeartBeat");
                Thread.Sleep(1000);
            }
        }

        public void WaterFunc()
        {
            while (true)
            {
                Water_mrse.WaitOne();
                bHapticsLib.bHapticsManager.PlayRegistered("WaterSlushing");
                Thread.Sleep(5050);
            }
        }

        public void ChokingFunc()
        {
            while (true)
            {
                Choking_mrse.WaitOne();
                bHapticsLib.bHapticsManager.PlayRegistered("Choking");
                Thread.Sleep(1050);
            }
        }

        public TactsuitVR()
        {
            LOG("Initializing suit");
            suitDisabled = false;
            RegisterAllTactFiles();
            LOG("Starting HeartBeat and NeckTingle thread...");
            Thread HeartBeatThread = new Thread(HeartBeatFunc);
            HeartBeatThread.Start();
            Thread WaterThread = new Thread(WaterFunc);
            WaterThread.Start();
            Thread ChokingThread = new Thread(ChokingFunc);
            ChokingThread.Start();
        }

        public void LOG(string logStr)
        {
            MelonLogger.Msg(logStr);
        }



        void RegisterAllTactFiles()
        {
            string configPath = Directory.GetCurrentDirectory() + "\\Mods\\bHaptics";
            DirectoryInfo d = new DirectoryInfo(configPath);
            FileInfo[] Files = d.GetFiles("*.tact", SearchOption.AllDirectories);
            for (int i = 0; i < Files.Length; i++)
            {
                string filename = Files[i].Name;
                string fullName = Files[i].FullName;
                string prefix = Path.GetFileNameWithoutExtension(filename);
                // LOG("Trying to register: " + prefix + " " + fullName);
                if (filename == "." || filename == "..")
                    continue;
                string tactFileStr = File.ReadAllText(fullName);
                try
                {
                    bHapticsLib.bHapticsManager.RegisterPatternFromJson(prefix, tactFileStr);
                    LOG("Pattern registered: " + prefix);
                }
                catch (Exception e) { LOG(e.ToString()); }

                FeedbackMap.Add(prefix, Files[i]);
            }
            systemInitialized = true;
            //PlaybackHaptics("HeartBeat");
        }

        public void PlaybackHaptics(String key, float intensity = 1.0f, float duration = 1.0f)
        {
            if (FeedbackMap.ContainsKey(key))
            {
                bHapticsLib.ScaleOption scaleOption = new bHapticsLib.ScaleOption(intensity, duration);
                bHapticsLib.bHapticsManager.PlayRegistered(key, key, scaleOption, defaultRotationOption);
                // LOG("Playing back: " + key);
            }
            else
            {
                LOG("Feedback not registered: " + key);
            }
        }

        public void PlayBackHit(String key, float xzAngle, float yShift)
        {
            bHapticsLib.ScaleOption scaleOption = new bHapticsLib.ScaleOption(1f, 1f);
            bHapticsLib.RotationOption rotationOption = new bHapticsLib.RotationOption(xzAngle, yShift);
            bHapticsLib.bHapticsManager.PlayRegistered(key, key, scaleOption, rotationOption);
        }

        public void GunRecoil(bool isRightHand, float intensity = 1.0f )
        {
            float duration = 1.0f;
            var scaleOption = new bHapticsLib.ScaleOption(intensity, duration);
            var rotationFront = new bHapticsLib.RotationOption(0f, 0f);
            string postfix = "_L";
            if (isRightHand) { postfix = "_R"; }
            string keyArm = "Recoil" + postfix;
            string keyVest = "RecoilVest" + postfix;
            bHapticsLib.bHapticsManager.PlayRegistered(keyArm, keyArm, scaleOption, rotationFront);
            bHapticsLib.bHapticsManager.PlayRegistered(keyVest, keyVest, scaleOption, rotationFront);
        }
        public void SwordRecoil(bool isRightHand, float intensity = 1.0f)
        {
            float duration = 1.0f;
            var scaleOption = new bHapticsLib.ScaleOption(intensity, duration);
            var rotationFront = new bHapticsLib.RotationOption(0f, 0f);
            string postfix = "_L";
            if (isRightHand) { postfix = "_R"; }
            string keyArm = "Sword" + postfix;
            string keyVest = "SwordVest" + postfix;
            bHapticsLib.bHapticsManager.PlayRegistered(keyArm, keyArm, scaleOption, rotationFront);
            bHapticsLib.bHapticsManager.PlayRegistered(keyVest, keyVest, scaleOption, rotationFront);
        }

        public bool isMinigunPlaying()
        {
            if (IsPlaying("Minigun_L")) { return true; }
            if (IsPlaying("Minigun_R")) { return true; }
            if (IsPlaying("MinigunDual_L")) { return true; }
            if (IsPlaying("MinigunDual_R")) { return true; }
            return false;
        }

        public void FireMinigun(bool isRightHand, bool twoHanded)
        {
            if (isMinigunPlaying()) { return; }

            string postfix = "";
            if (twoHanded) { postfix += "Dual"; }
            if (isRightHand) { postfix += "_R"; }
            else { postfix += "_L"; }
            string key = "Minigun" + postfix;
            string keyVest = "MinigunVest" + postfix;
            PlaybackHaptics(key);
            PlaybackHaptics(keyVest);
        }

        public void StopMinigun(bool isRightHand, bool twoHanded)
        {
            string postfix = "";
            if (twoHanded) { postfix += "Dual"; }
            if (isRightHand) { postfix += "_R"; }
            else { postfix += "_L"; }
            string key = "Minigun" + postfix;
            string keyVest = "MinigunVest" + postfix;
            StopHapticFeedback(key);
            StopHapticFeedback(keyVest);
        }

        public void HeadShot()
        {
            if (bHapticsLib.bHapticsManager.IsDeviceConnected(bHapticsLib.PositionID.Head)) { PlaybackHaptics("HitInTheFace"); }
            else { PlaybackHaptics("HeadShotVest"); }
        }

        public void FootStep(bool isRightFoot)
        {
            if (!bHapticsLib.bHapticsManager.IsDeviceConnected(bHapticsLib.PositionID.FootLeft)) { return; }
            string postfix = "_L";
            if (isRightFoot) { postfix = "_R"; }
            string key = "FootStep" + postfix;
            PlaybackHaptics(key);
        }

        public void StartHeartBeat()
        {
            HeartBeat_mrse.Set();
        }

        public void StopHeartBeat()
        {
            HeartBeat_mrse.Reset();
        }

        public void StartWater()
        {
            Water_mrse.Set();
        }

        public void StopWater()
        {
            Water_mrse.Reset();
        }

        public void StartChoking()
        {
            Choking_mrse.Set();
        }

        public void StopChoking()
        {
            Choking_mrse.Reset();
            bHapticsLib.bHapticsManager.StopPlaying("TeleportOpened");
        }

        public bool IsPlaying(String effect)
        {
            return bHapticsLib.bHapticsManager.IsPlaying(effect);
        }

        public void StopHapticFeedback(String effect)
        {
            bHapticsLib.bHapticsManager.StopPlaying(effect);
        }

        public void StopAllHapticFeedback()
        {
            StopThreads();
            foreach (String key in FeedbackMap.Keys)
            {
                bHapticsLib.bHapticsManager.StopPlaying(key);
            }
        }

        public void StopThreads()
        {
            StopHeartBeat();
            StopWater();
            StopChoking();
        }


    }
}
