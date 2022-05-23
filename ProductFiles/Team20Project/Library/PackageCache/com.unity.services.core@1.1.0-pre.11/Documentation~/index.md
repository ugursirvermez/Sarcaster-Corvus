# Services Core
This package provides a solution to initialize all game services in a single call
and defines common components used by multiple game service packages.
These components are standardized and aim to unify the overall experience of working with game service packages.

## Package contents

### Initialize game services
To initialize all game services at once you just have to call `UnityServices.InitializeAsync()`.
It returns a `Task` that enables you to monitor the initialization's progression.

#### Example
```cs
using System;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;

public class InitializeUnityServices : MonoBehaviour
{
    public string environment = "production";

    async void Start()
    {
        try
        {
            var options = new InitializationOptions()
                .SetEnvironmentName(environment);

            await UnityServices.InitializeAsync(options);
        }
        catch (Exception exception)
        {
            // An error occured during services initialization.
        }
    }
}
```

## Technical details

The `InitializeAsync` methods affects the currently installed service packages in your Unity project.

Note that this method is not supported during edit time.

### Requirements
* Supported Unity Editor: 2019.4 and later.
