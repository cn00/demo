#!/usr/sh

export ANDROID_SYSROOT='C:/Android/sdk/ndk-bundle/sysroot'

export NDK_PROJECT_PATH="." 

APP_PLATFORM="android-14"

ndk-build
