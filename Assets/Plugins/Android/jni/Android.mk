LOCAL_PATH := $(call my-dir)

include $(CLEAR_VARS)

LOCAL_MODULE    := sqlite3_static
LOCAL_MODULE_FILENAME := libsqlite3

# LOCAL_ARM_MODE := arm

LOCAL_SRC_FILES := ../../Sqlite/src/sqlite3.c

LOCAL_EXPORT_C_INCLUDES := $(LOCAL_PATH)/src

LOCAL_C_INCLUDES := $(LOCAL_PATH)/src \
${ANDROID_SYSROOT}/usr/include/arm-linux-androideabi

# include $(BUILD_STATIC_LIBRARY)
include $(BUILD_SHARED_LIBRARY)
