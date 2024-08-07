﻿using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ServerHandler = Exiled.Events.Handlers.Server;
using ScriptedEvents.API.Features.Aliases;
using ScriptedEvents.API.Helpers;
using ScriptedEvents.Handlers;
using System.ComponentModel;
using System.Collections.ObjectModel;
using ScriptedEvents.DemoScripts;

namespace ScriptedEvents
{
    public class MainPlugin : Plugin<Config>
    {
        public override string Name => "ScriptedEvents";
        public override string Author => "Thunder";
        public override Version Version => new(0, 1, 0);
        public override Version RequiredExiledVersion => new(6, 0, 0);
        public override PluginPriority Priority => PluginPriority.High;

        public static ReadOnlyCollection<IDemoScript> DemoScripts { get; } = new List<IDemoScript>()
        {
            new About(),
            new DemoScript(),
            new ConditionSamples(),
        }.AsReadOnly();

        public static MainPlugin Singleton;
        public static EventHandlers Handlers;

        public override void OnEnabled()
        {
            Singleton = this;
            Handlers = new();

            if (!Directory.Exists(ScriptHelper.ScriptPath))
            {
                var info = Directory.CreateDirectory(ScriptHelper.ScriptPath);
                foreach (var demo in DemoScripts)
                {
                    File.WriteAllText(Path.Combine(info.FullName, $"{demo.FileName}.txt"), demo.Contents);
                }
            }

            ServerHandler.RestartingRound += Handlers.OnRestarting;
            ServerHandler.RoundStarted += Handlers.OnRoundStarted;

            ApiHelper.RegisterActions(GetType());
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            ServerHandler.RestartingRound -= Handlers.OnRestarting;
            ServerHandler.RoundStarted -= Handlers.OnRoundStarted;

            ScriptHelper.ActionTypes.Clear();

            Singleton = null;
            Handlers = null;
            
            base.OnDisabled();
        }
    }

    public class Config : IConfig
    {
        [Description("Whether or not to enable the Scripted Events plugin.")]
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;

        [Description("List of scripts to run as soon as the round starts.")]
        public List<string> AutoRunScripts { get; set; } = new();

        // todo: un-alias door commands, because they dont have duration anymore
        [Description("Define a custom set of actions and the action they run when used.")]
        public List<Alias> Aliases { get; set; } = new()
        {
            new("LOCKDOORBRIEF", "DOOR LOCK * 10")
        };

        [Description("Define a custom set of permissions used to run a certain script. The provided permission will be added AFTER es.execute (eg. es.execute.examplepermission for the provided example).")]
        public Dictionary<string, string> RequiredPermissions { get; set; } = new()
        {
            { "ExampleScriptNameHere", "examplepermission" },
        };
    }
}
