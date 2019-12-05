#!/usr/sh

if [ -z "$ANDROID_NDK" ]; then
    export ANDROID_NDK=/usr/local/share/android-ndk
fi
export ANDROID_SYSROOT="$ANDROID_NDK"

export NDK_PROJECT_PATH="." 

APP_PLATFORM="android-14"

cd $(dirname $0;pwd)

ndk-build

rm -rf ./obj