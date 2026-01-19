using HarmonyLib;
using Rhythm;
using UNBEATAP.Traps;

namespace UNBEATAP.Patches;

public class TrapUpdatePatch
{
    private static float lastPosition;


    [HarmonyPatch(typeof(RhythmTracker), "FixedUpdate")]
    [HarmonyPostfix]
    static void TrackerFixedUpdatePostfix(RhythmTracker __instance)
    {
        if(!Plugin.Client.Connected)
        {
            return;
        }

        if(!JeffBezosController.isPlayingBeatmap)
        {
            return;
        }

        if(JeffBezosController.isPaused)
        {
            return;
        }

        if(!RhythmController.Instance)
        {
            return;
        }

        if(!RhythmController.Instance.Playing)
        {
            return;
        }

        float currentPosition = __instance.TimelinePosition;
        if(currentPosition <= lastPosition)
        {
            lastPosition = currentPosition;
            return;
        }

        float deltaTime = currentPosition - lastPosition;
        TrapController.UpdateTraps(deltaTime / 1000);

        lastPosition = currentPosition;
    }


    [HarmonyPatch(typeof(RhythmController), "Update")]
    [HarmonyPostfix]
    static void ControllerUpdatePostfix(RhythmController __instance)
    {
        if(!Plugin.Client.Connected)
        {
            return;
        }

        Traverse traverse = new Traverse(__instance);
        bool isFinished = traverse.Field("isFinished").GetValue<bool>();
        if(isFinished)
        {
            // Deactivate all traps at the end of a song
            TrapController.DeactivateTraps();
            return;
        }

        if(__instance.song.gameOver)
        {
            // Always unmute the music after leaving the song
            if(Muted.IsMuted)
            {
                Muted.UnMute();
            }
        }
        else if(__instance.Playing && !isFinished)
        {
            bool shouldMute = Muted.Timer.GetActive();
            if(shouldMute && !Muted.IsMuted)
            {
                Muted.Mute();
            }
            else if(!shouldMute && Muted.IsMuted)
            {
                Muted.UnMute();
            }
        }
        else if(Muted.IsMuted)
        {
            Muted.UnMute();
        }
    }
}