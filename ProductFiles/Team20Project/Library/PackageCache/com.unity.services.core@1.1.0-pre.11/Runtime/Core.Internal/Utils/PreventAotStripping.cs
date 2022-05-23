using System;
using Newtonsoft.Json.Converters;
using UnityEngine.Scripting;

namespace Unity.Services.Core.Internal
{
    [Preserve]
    class PreventAotStripping
    {
        [Preserve]
        public void PreserveStringEnumConverterForAOT()
        {
            // AOT needs onl
            new StringEnumConverter();

            throw new InvalidOperationException("This method is used for AOT code generation only. Do not call it at runtime.");
        }
    }
}
