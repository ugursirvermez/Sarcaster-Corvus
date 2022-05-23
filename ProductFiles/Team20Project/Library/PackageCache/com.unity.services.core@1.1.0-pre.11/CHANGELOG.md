# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [1.1.0-pre.11] - 2021-10-22
### Added
- Getter methods for `ConfigurationBuilder`.

### Fixed
- Fix layout for Project Bind Redirect Popup for Light theme

## [1.1.0-pre.10] - 2021-10-08
### Added
- `IActionScheduler` component to schedule actions at runtime.
- `ICloudProjectId` component to access cloudProjectId.
  
### Removed
- Removed the Service Activation Popup

### Fixed
- Fix issue on 2020.1
- Fix define check bug on Android and WebGL

## [1.1.0-pre.9] - 2021-09-23
### Added
- New common error codes: ApiMissing, RequestRejected, NotFound, InvalidRequest.
- Link project pop-up dialog

### Fixed
- Core registry throwing exceptions when domain reloads are disabled

## [1.1.0-pre.8] - 2021-08-04
### Added
- Added base exception type for other Operate SDKs to derive from. Consistent error handling experience.

## [1.1.0-pre.7] - 2021-07-30
### Fixed
- Package structure for promotion

## [1.1.0-pre.6] - 2021-07-29
### Fixed
- Fixed incorrect services state in case `InitializeAsync` is called too early

### Removed
- Removed privacy component.

## [1.1.0-pre.5] - 2021-07-23
### Changed
- [Breaking Change] Renamed `UnityServices.Initialize` to `InitializeAsync` to explicit its asynchronous behaviour.

## [1.1.0-pre.4] - 2021-07-21
### Changed
- [Breaking Change] Move all classes meant for internal use to `*.Internal` namespace.

## [1.1.0-pre.3] - 2021-07-15
### Added
- `SetEnvironmentName` Initialization option to set the environment services should use.
- MiniJson.

### Removed
- Newtonsoft dependency.

## [1.1.0-pre.2] - 2021-06-14
### Added
- `IProjectConfiguration` component to access services settings at runtime.
- `IConfigurationProvider` to provide configuration values that need to be available at runtime.
- `InitializationOptions` to enable services initialization customization through code.
  `UnityServices` API has been changed accordingly.

### Changed
- Moves all class meant for internal use from _Unity.Service.Core_ to _Unity.Service.Core.Internal_
- Make `AsyncOperation` and related classes internal

## [1.1.0-pre.1] - 2021-05-31
### Changed
- BREAKING CHANGES:
  - `IInitializablePackage.Initialize` now returns a `System.Threading.Tasks.Task` instead of `IAsyncOperation`
  - `UnityServices.Initialize` now returns a `System.Threading.Tasks.Task` instead of `IAsyncOperation`

### Removed
- Removed Moq dependency.

## [0.3.0-preview] - 2021-05-04
### Added
- Installation Identifier component.
- Service Activation popup.

### Changed
- Review of the Editor API to rename the following:
  - `OperateService` to `EditorGameService`
    - Members:
      - `ServiceName` to `Name`
      - `OperateServiceEnabler` to `Enabler`
  - `IServiceIdentifier` to `IEditorGameServiceIdentifier`
  - `OperateDashboardHelper` to `EditorGameServiceDashboardHelper`
  - `ServiceFlagEnabler` to `EditorGameServiceFlagEnabler`
    - Members:
      - `ServiceFlagName` to `FlagName`
  - `IOperateServiceEnabler` to `IEditorGameServiceEnabler`
  - `OperateServiceRegistry` to `EditorGameServiceRegistry`
    - Methods:
       - `GetService` to `GetEditorGameService`
  - `IOperateServiceRegistry` to `IEditorGameServiceRegistry`
    - Methods:
      - `GetService` to `GetEditorGameService`
  - `OperateRemoteConfiguration` to `EditorGameServiceRemoteConfiguration`
  - `OperateServiceSettingsProvider` to `EditorGameServiceSettingsProvider`
    - Members:
      - `OperateService` to `EditorGameService`
  - `OperateSettingsCommonHeaderUiHelper` to `SettingsCommonHeaderUiHelper`
  - GlobalDefine:
    - `ENABLE_OPERATE_SERVICES` to `ENABLE_EDITOR_GAME_SERVICES`

## [0.2.0-preview] - 2021-04-14
### Added
- DevEx integration into the editor.

### Changed
- `IAsyncOperation` to extend `IEnumerator` so they can be yielded in routines.

### Removed
- Removed all API under the `Unity.Services.Core.Networking` namespace because it wasn't ready for use yet.

## [0.1.0-preview] - 2021-03-12

### This is the first release of *com.unity.services.core*.
