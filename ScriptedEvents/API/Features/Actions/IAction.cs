﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptedEvents.API.Features.Actions;

namespace ScriptedEvents.API.Features.Actions
{
    public interface IAction
    {
        string Name { get; }
        string[] Aliases { get; }
        string[] Arguments { get; set; }
        ActionResponse Execute();
    }
}
