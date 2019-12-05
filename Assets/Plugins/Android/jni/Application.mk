APP_STL := gnustl_static

APP_CPPFLAGS := -frtti -DCC_ENABLE_CHIPMUNK_INTEGRATION=1 -std=c++11 -fsigned-char
APP_LDFLAGS := -latomic

APP_PLATFORM:=android-14

APP_ABI := armeabi armeabi-v7a x86 arm64-v8a mips #arm64
# APP_ABI := arm64-v8a armeabi-v7a x86
# APP_ABI := armeabi

ifeq ($(NDK_DEBUG),1)
  APP_CPPFLAGS += -DDEBUG=1
  APP_OPTIM := debug
else
  # LOCAL_CPPFLAGS += -ffunction-sections -fdata-sections -fvisibility=hidden
  # LOCAL_CFLAGS += -ffunction-sections -fdata-sections -fvisibility=hidden
  LOCAL_LDFLAGS += -Wl,--gc-sections
  APP_CPPFLAGS += -DNDEBUG -Os -D__USE_SYMBLE_MIX__=1
  APP_OPTIM := release
endif
# ANDROID_SDK_TYPE_BILI = 1 
# APP_CPPFLAGS += -DANDROID_SDK_TYPE=1