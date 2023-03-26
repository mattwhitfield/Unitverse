﻿using System.Collections.Generic;
using Unitverse.Core.Models;
using Unitverse.Core.Templating.Model;

namespace Unitverse.Core.Templating
{
    public interface ITemplatingContext
    {
        IOwningType ClassModel { get; }
        ModelGenerationContext ModelGenerationContext { get; }
        IList<ITemplate> Templates { get; }
    }
}