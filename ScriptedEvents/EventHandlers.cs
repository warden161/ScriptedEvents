﻿using Exiled.API.Features;
using ScriptedEvents.API.Helpers;
using MEC;
using System.IO;
using ScriptedEvents.API.Features;
using ScriptedEvents.API.Features.Exceptions;

namespace ScriptedEvents
{
    public class EventHandlers
    {
        public void OnRestarting()
        {
            foreach (var kvp in ScriptHelper.RunningScripts)
            {
                kvp.Key.IsRunning = false;
                Timing.KillCoroutines(kvp.Value);
            }
            foreach (string key in Handlers.DefaultActions.WaitUntilAction.Coroutines)
            {
                Timing.KillCoroutines(key);
            }
            Handlers.DefaultActions.WaitUntilAction.Coroutines.Clear();
            ScriptHelper.RunningScripts.Clear();
        }

        public void OnRoundStarted()
        {
            foreach (string name in MainPlugin.Singleton.Config.AutoRunScripts)
            {
                try
                {
                    Script scr = ScriptHelper.ReadScript(name);
                    ScriptHelper.RunScript(scr);
                }
                catch (DisabledScriptException)
                {
                    Log.Warn($"The '{name}' script is set to run each round, but the script is disabled!");
                }
                catch (FileNotFoundException)
                {
                    Log.Warn($"The '{name}' script is set to run each round, but the script is not found!");
                }
            }
        }
    }
}
