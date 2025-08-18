# Changelog
All notable changes to the input system package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

Due to package verification, the latest version below is the unpublished version and the date is meaningless.
however, it has to be formatted properly to pass verification tests.

## [0.1.12-preview] - 2023-06-30

Set Input System runtime to run in the background. This fixes issues where the InputSystem would not receive input events when the game was not in focus.
Update Input System dependency to version 1.6.1

## [0.1.11-preview] - 2022-09-01

Add support for different slotIndex values in Editor for up to four connected gamepads

## [0.1.10-preview] - 2022-01-19

Change when the device layout is registered from being after a scene is loaded to after the assemblies are loaded. This fixes issues where some systems could create the InputSystem code earlier than we were registering the device layout causing PS5 controllers to not be registered correctly until they were disconnected and reconnected.

## [0.1.9-preview] - 2021-11-23

Add support for new TriggerEffect modes

## [0.1.8-preview] - 2021-10-21

Fixes for TriggerEffectMode.Vibration when in editor

## [0.1.7-preview] - 2021-01-14

Fixes for input debugger

## [0.1.6-preview] - 2020-07-24

Added support for name change of device to "PS5DualSenseGamepad"

## [0.1.5-preview] - 2020-05-26

Added support for using the controller inside Unity editor

## [0.1.4-preview] - 2020-05-08

Fixed missing meta files

## [0.1.3-preview] - 2020-04-10

Fixed incorrect triggering of "select" gamepad events when square button is pressed 

## [0.1.2-preview] - 2019-11-27

Updated package to include platform name

## [0.1.1-preview] - 2019-10-30

Updated package dependencies to use latest `com.unity.inputsystem` package version 1.0.0-preview.1

## [0.1.0-preview] - 2019-10-18

First release.





