mysqldump: [Warning] Using a password on the command line interface can be insecure.
-- MySQL dump 10.13  Distrib 5.7.21, for Linux (x86_64)
--
-- Host: localhost    Database: redmine
-- ------------------------------------------------------
-- Server version	5.7.21

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `attachments`
--

DROP TABLE IF EXISTS `attachments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `attachments` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `container_id` int(11) DEFAULT NULL,
  `container_type` varchar(30) COLLATE utf8_unicode_ci DEFAULT NULL,
  `filename` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `disk_filename` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `filesize` bigint(20) NOT NULL DEFAULT '0',
  `content_type` varchar(255) COLLATE utf8_unicode_ci DEFAULT '',
  `digest` varchar(64) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `downloads` int(11) NOT NULL DEFAULT '0',
  `author_id` int(11) NOT NULL DEFAULT '0',
  `created_on` datetime DEFAULT NULL,
  `description` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `disk_directory` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `index_attachments_on_author_id` (`author_id`),
  KEY `index_attachments_on_created_on` (`created_on`),
  KEY `index_attachments_on_container_id_and_container_type` (`container_id`,`container_type`),
  KEY `index_attachments_on_disk_filename` (`disk_filename`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `attachments`
--

LOCK TABLES `attachments` WRITE;
/*!40000 ALTER TABLE `attachments` DISABLE KEYS */;
/*!40000 ALTER TABLE `attachments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `auth_sources`
--

DROP TABLE IF EXISTS `auth_sources`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `auth_sources` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `type` varchar(30) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `name` varchar(60) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `host` varchar(60) COLLATE utf8_unicode_ci DEFAULT NULL,
  `port` int(11) DEFAULT NULL,
  `account` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `account_password` varchar(255) COLLATE utf8_unicode_ci DEFAULT '',
  `base_dn` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `attr_login` varchar(30) COLLATE utf8_unicode_ci DEFAULT NULL,
  `attr_firstname` varchar(30) COLLATE utf8_unicode_ci DEFAULT NULL,
  `attr_lastname` varchar(30) COLLATE utf8_unicode_ci DEFAULT NULL,
  `attr_mail` varchar(30) COLLATE utf8_unicode_ci DEFAULT NULL,
  `onthefly_register` tinyint(1) NOT NULL DEFAULT '0',
  `tls` tinyint(1) NOT NULL DEFAULT '0',
  `filter` text COLLATE utf8_unicode_ci,
  `timeout` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `index_auth_sources_on_id_and_type` (`id`,`type`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `auth_sources`
--

LOCK TABLES `auth_sources` WRITE;
/*!40000 ALTER TABLE `auth_sources` DISABLE KEYS */;
/*!40000 ALTER TABLE `auth_sources` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `boards`
--

DROP TABLE IF EXISTS `boards`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `boards` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `project_id` int(11) NOT NULL,
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `description` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `position` int(11) DEFAULT NULL,
  `topics_count` int(11) NOT NULL DEFAULT '0',
  `messages_count` int(11) NOT NULL DEFAULT '0',
  `last_message_id` int(11) DEFAULT NULL,
  `parent_id` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `boards_project_id` (`project_id`),
  KEY `index_boards_on_last_message_id` (`last_message_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `boards`
--

LOCK TABLES `boards` WRITE;
/*!40000 ALTER TABLE `boards` DISABLE KEYS */;
/*!40000 ALTER TABLE `boards` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `changes`
--

DROP TABLE IF EXISTS `changes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `changes` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `changeset_id` int(11) NOT NULL,
  `action` varchar(1) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `path` text COLLATE utf8_unicode_ci NOT NULL,
  `from_path` text COLLATE utf8_unicode_ci,
  `from_revision` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `revision` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `branch` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `changesets_changeset_id` (`changeset_id`)
) ENGINE=InnoDB AUTO_INCREMENT=2105 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `changes`
--

LOCK TABLES `changes` WRITE;
/*!40000 ALTER TABLE `changes` DISABLE KEYS */;
INSERT INTO `changes` VALUES (1053,44,'A','.gitignore',NULL,NULL,NULL,NULL),(1054,44,'A','LICENSE',NULL,NULL,NULL,NULL),(1055,45,'A','Assets/Plugins.meta',NULL,NULL,NULL,NULL),(1056,45,'A','Assets/Plugins/Android.meta',NULL,NULL,NULL,NULL),(1057,45,'A','Assets/Plugins/Android/libs.meta',NULL,NULL,NULL,NULL),(1058,45,'A','Assets/Plugins/Android/libs/armeabi-v7a.meta',NULL,NULL,NULL,NULL),(1059,45,'A','Assets/Plugins/Android/libs/armeabi-v7a/libxlua.so',NULL,NULL,NULL,NULL),(1060,45,'A','Assets/Plugins/Android/libs/armeabi-v7a/libxlua.so.meta',NULL,NULL,NULL,NULL),(1061,45,'A','Assets/Plugins/Android/libs/x86.meta',NULL,NULL,NULL,NULL),(1062,45,'A','Assets/Plugins/Android/libs/x86/libxlua.so',NULL,NULL,NULL,NULL),(1063,45,'A','Assets/Plugins/Android/libs/x86/libxlua.so.meta',NULL,NULL,NULL,NULL),(1064,45,'A','Assets/Plugins/WSA.meta',NULL,NULL,NULL,NULL),(1065,45,'A','Assets/Plugins/WSA/ARM.meta',NULL,NULL,NULL,NULL),(1066,45,'A','Assets/Plugins/WSA/ARM/xlua.dll',NULL,NULL,NULL,NULL),(1067,45,'A','Assets/Plugins/WSA/ARM/xlua.dll.meta',NULL,NULL,NULL,NULL),(1068,45,'A','Assets/Plugins/WSA/x64.meta',NULL,NULL,NULL,NULL),(1069,45,'A','Assets/Plugins/WSA/x64/xlua.dll',NULL,NULL,NULL,NULL),(1070,45,'A','Assets/Plugins/WSA/x64/xlua.dll.meta',NULL,NULL,NULL,NULL),(1071,45,'A','Assets/Plugins/WSA/x86.meta',NULL,NULL,NULL,NULL),(1072,45,'A','Assets/Plugins/WSA/x86/xlua.dll',NULL,NULL,NULL,NULL),(1073,45,'A','Assets/Plugins/WSA/x86/xlua.dll.meta',NULL,NULL,NULL,NULL),(1074,45,'A','Assets/Plugins/WebGL.meta',NULL,NULL,NULL,NULL),(1075,45,'A','Assets/Plugins/WebGL/xlua_webgl.cpp',NULL,NULL,NULL,NULL),(1076,45,'A','Assets/Plugins/WebGL/xlua_webgl.cpp.meta',NULL,NULL,NULL,NULL),(1077,45,'A','Assets/Plugins/iOS.meta',NULL,NULL,NULL,NULL),(1078,45,'A','Assets/Plugins/iOS/HotfixFlags.cpp',NULL,NULL,NULL,NULL),(1079,45,'A','Assets/Plugins/iOS/HotfixFlags.cpp.meta',NULL,NULL,NULL,NULL),(1080,45,'A','Assets/Plugins/iOS/libxlua.a',NULL,NULL,NULL,NULL),(1081,45,'A','Assets/Plugins/iOS/libxlua.a.meta',NULL,NULL,NULL,NULL),(1082,45,'A','Assets/Plugins/x86.meta',NULL,NULL,NULL,NULL),(1083,45,'A','Assets/Plugins/x86/libxlua.so',NULL,NULL,NULL,NULL),(1084,45,'A','Assets/Plugins/x86/libxlua.so.meta',NULL,NULL,NULL,NULL),(1085,45,'A','Assets/Plugins/x86/xlua.dll',NULL,NULL,NULL,NULL),(1086,45,'A','Assets/Plugins/x86/xlua.dll.meta',NULL,NULL,NULL,NULL),(1087,45,'A','Assets/Plugins/x86_64.meta',NULL,NULL,NULL,NULL),(1088,45,'A','Assets/Plugins/x86_64/libxlua.so',NULL,NULL,NULL,NULL),(1089,45,'A','Assets/Plugins/x86_64/libxlua.so.meta',NULL,NULL,NULL,NULL),(1090,45,'A','Assets/Plugins/x86_64/xlua.dll',NULL,NULL,NULL,NULL),(1091,45,'A','Assets/Plugins/x86_64/xlua.dll.meta',NULL,NULL,NULL,NULL),(1092,45,'A','Assets/Plugins/xlua.bundle.meta',NULL,NULL,NULL,NULL),(1093,45,'A','Assets/Plugins/xlua.bundle/Contents.meta',NULL,NULL,NULL,NULL),(1094,45,'A','Assets/Plugins/xlua.bundle/Contents/Info.plist',NULL,NULL,NULL,NULL),(1095,45,'A','Assets/Plugins/xlua.bundle/Contents/Info.plist.meta',NULL,NULL,NULL,NULL),(1096,45,'A','Assets/Plugins/xlua.bundle/Contents/MacOS.meta',NULL,NULL,NULL,NULL),(1097,45,'A','Assets/Plugins/xlua.bundle/Contents/MacOS/xlua',NULL,NULL,NULL,NULL),(1098,45,'A','Assets/Plugins/xlua.bundle/Contents/MacOS/xlua.meta',NULL,NULL,NULL,NULL),(1099,45,'A','Assets/XLua.meta',NULL,NULL,NULL,NULL),(1100,45,'A','Assets/XLua/CHANGELOG.txt',NULL,NULL,NULL,NULL),(1101,45,'A','Assets/XLua/CHANGELOG.txt.meta',NULL,NULL,NULL,NULL),(1102,45,'A','Assets/XLua/Doc.meta',NULL,NULL,NULL,NULL),(1103,45,'A','Assets/XLua/Doc/Materials.meta',NULL,NULL,NULL,NULL),(1104,45,'A','Assets/XLua/Doc/Materials/logo.mat',NULL,NULL,NULL,NULL),(1105,45,'A','Assets/XLua/Doc/Materials/logo.mat.meta',NULL,NULL,NULL,NULL),(1106,45,'A','Assets/XLua/Doc/XLua_API.doc',NULL,NULL,NULL,NULL),(1107,45,'A','Assets/XLua/Doc/XLua_API.doc.meta',NULL,NULL,NULL,NULL),(1108,45,'A','Assets/XLua/Doc/XLua_API.md',NULL,NULL,NULL,NULL),(1109,45,'A','Assets/XLua/Doc/XLua增加删除第三方lua库.doc',NULL,NULL,NULL,NULL),(1110,45,'A','Assets/XLua/Doc/XLua增加删除第三方lua库.doc.meta',NULL,NULL,NULL,NULL),(1111,45,'A','Assets/XLua/Doc/XLua增加删除第三方lua库.md',NULL,NULL,NULL,NULL),(1112,45,'A','Assets/XLua/Doc/XLua复杂值类型（struct）gc优化指南.doc',NULL,NULL,NULL,NULL),(1113,45,'A','Assets/XLua/Doc/XLua复杂值类型（struct）gc优化指南.doc.meta',NULL,NULL,NULL,NULL),(1114,45,'A','Assets/XLua/Doc/XLua复杂值类型（struct）gc优化指南.md',NULL,NULL,NULL,NULL),(1115,45,'A','Assets/XLua/Doc/XLua性能分析工具.doc',NULL,NULL,NULL,NULL),(1116,45,'A','Assets/XLua/Doc/XLua性能分析工具.doc.meta',NULL,NULL,NULL,NULL),(1117,45,'A','Assets/XLua/Doc/XLua性能分析工具.md',NULL,NULL,NULL,NULL),(1118,45,'A','Assets/XLua/Doc/XLua教程.doc',NULL,NULL,NULL,NULL),(1119,45,'A','Assets/XLua/Doc/XLua教程.doc.meta',NULL,NULL,NULL,NULL),(1120,45,'A','Assets/XLua/Doc/XLua教程.md',NULL,NULL,NULL,NULL),(1121,45,'A','Assets/XLua/Doc/XLua的配置.doc',NULL,NULL,NULL,NULL),(1122,45,'A','Assets/XLua/Doc/XLua的配置.doc.meta',NULL,NULL,NULL,NULL),(1123,45,'A','Assets/XLua/Doc/configure.md',NULL,NULL,NULL,NULL),(1124,45,'A','Assets/XLua/Doc/configure.md.meta',NULL,NULL,NULL,NULL),(1125,45,'A','Assets/XLua/Doc/custom_generate.md',NULL,NULL,NULL,NULL),(1126,45,'A','Assets/XLua/Doc/custom_generate.md.meta',NULL,NULL,NULL,NULL),(1127,45,'A','Assets/XLua/Doc/faq.md',NULL,NULL,NULL,NULL),(1128,45,'A','Assets/XLua/Doc/faq.md.meta',NULL,NULL,NULL,NULL),(1129,45,'A','Assets/XLua/Doc/features.md',NULL,NULL,NULL,NULL),(1130,45,'A','Assets/XLua/Doc/features.md.meta',NULL,NULL,NULL,NULL),(1131,45,'A','Assets/XLua/Doc/hotfix.md',NULL,NULL,NULL,NULL),(1132,45,'A','Assets/XLua/Doc/hotfix.md.meta',NULL,NULL,NULL,NULL),(1133,45,'A','Assets/XLua/Doc/logo.png',NULL,NULL,NULL,NULL),(1134,45,'A','Assets/XLua/Doc/logo.png.meta',NULL,NULL,NULL,NULL),(1135,45,'A','Assets/XLua/Doc/signature.md',NULL,NULL,NULL,NULL),(1136,45,'A','Assets/XLua/Doc/signature.md.meta',NULL,NULL,NULL,NULL),(1137,45,'A','Assets/XLua/Doc/xLua.png',NULL,NULL,NULL,NULL),(1138,45,'A','Assets/XLua/Doc/xLua.png.meta',NULL,NULL,NULL,NULL),(1139,45,'A','Assets/XLua/Resources.meta',NULL,NULL,NULL,NULL),(1140,45,'A','Assets/XLua/Resources/perf.meta',NULL,NULL,NULL,NULL),(1141,45,'A','Assets/XLua/Resources/perf/memory.lua.txt',NULL,NULL,NULL,NULL),(1142,45,'A','Assets/XLua/Resources/perf/memory.lua.txt.meta',NULL,NULL,NULL,NULL),(1143,45,'A','Assets/XLua/Resources/perf/profiler.lua.txt',NULL,NULL,NULL,NULL),(1144,45,'A','Assets/XLua/Resources/perf/profiler.lua.txt.meta',NULL,NULL,NULL,NULL),(1145,45,'A','Assets/XLua/Resources/xlua.meta',NULL,NULL,NULL,NULL),(1146,45,'A','Assets/XLua/Resources/xlua/util.lua.txt',NULL,NULL,NULL,NULL),(1147,45,'A','Assets/XLua/Resources/xlua/util.lua.txt.meta',NULL,NULL,NULL,NULL),(1148,45,'A','Assets/XLua/Src.meta',NULL,NULL,NULL,NULL),(1149,45,'A','Assets/XLua/Src/CodeEmit.cs',NULL,NULL,NULL,NULL),(1150,45,'A','Assets/XLua/Src/CodeEmit.cs.meta',NULL,NULL,NULL,NULL),(1151,45,'A','Assets/XLua/Src/CopyByValue.cs',NULL,NULL,NULL,NULL),(1152,45,'A','Assets/XLua/Src/CopyByValue.cs.meta',NULL,NULL,NULL,NULL),(1153,45,'A','Assets/XLua/Src/DelegateBridge.cs',NULL,NULL,NULL,NULL),(1154,45,'A','Assets/XLua/Src/DelegateBridge.cs.meta',NULL,NULL,NULL,NULL),(1155,45,'A','Assets/XLua/Src/Editor.meta',NULL,NULL,NULL,NULL),(1156,45,'A','Assets/XLua/Src/Editor/Generator.cs',NULL,NULL,NULL,NULL),(1157,45,'A','Assets/XLua/Src/Editor/Generator.cs.meta',NULL,NULL,NULL,NULL),(1158,45,'A','Assets/XLua/Src/Editor/Hotfix.cs',NULL,NULL,NULL,NULL),(1159,45,'A','Assets/XLua/Src/Editor/Hotfix.cs.meta',NULL,NULL,NULL,NULL),(1160,45,'A','Assets/XLua/Src/Editor/LinkXmlGen.meta',NULL,NULL,NULL,NULL),(1161,45,'A','Assets/XLua/Src/Editor/LinkXmlGen/LinkXmlGen.cs',NULL,NULL,NULL,NULL),(1162,45,'A','Assets/XLua/Src/Editor/LinkXmlGen/LinkXmlGen.cs.meta',NULL,NULL,NULL,NULL),(1163,45,'A','Assets/XLua/Src/Editor/LinkXmlGen/LinkXmlGen.tpl.txt',NULL,NULL,NULL,NULL),(1164,45,'A','Assets/XLua/Src/Editor/LinkXmlGen/LinkXmlGen.tpl.txt.meta',NULL,NULL,NULL,NULL),(1165,45,'A','Assets/XLua/Src/Editor/Template.meta',NULL,NULL,NULL,NULL),(1166,45,'A','Assets/XLua/Src/Editor/Template/LuaClassWrap.tpl.txt',NULL,NULL,NULL,NULL),(1167,45,'A','Assets/XLua/Src/Editor/Template/LuaClassWrap.tpl.txt.meta',NULL,NULL,NULL,NULL),(1168,45,'A','Assets/XLua/Src/Editor/Template/LuaClassWrapGCM.tpl.txt',NULL,NULL,NULL,NULL),(1169,45,'A','Assets/XLua/Src/Editor/Template/LuaClassWrapGCM.tpl.txt.meta',NULL,NULL,NULL,NULL),(1170,45,'A','Assets/XLua/Src/Editor/Template/LuaDelegateBridge.tpl.txt',NULL,NULL,NULL,NULL),(1171,45,'A','Assets/XLua/Src/Editor/Template/LuaDelegateBridge.tpl.txt.meta',NULL,NULL,NULL,NULL),(1172,45,'A','Assets/XLua/Src/Editor/Template/LuaDelegateWrap.tpl.txt',NULL,NULL,NULL,NULL),(1173,45,'A','Assets/XLua/Src/Editor/Template/LuaDelegateWrap.tpl.txt.meta',NULL,NULL,NULL,NULL),(1174,45,'A','Assets/XLua/Src/Editor/Template/LuaEnumWrap.tpl.txt',NULL,NULL,NULL,NULL),(1175,45,'A','Assets/XLua/Src/Editor/Template/LuaEnumWrap.tpl.txt.meta',NULL,NULL,NULL,NULL),(1176,45,'A','Assets/XLua/Src/Editor/Template/LuaEnumWrapGCM.tpl.txt',NULL,NULL,NULL,NULL),(1177,45,'A','Assets/XLua/Src/Editor/Template/LuaEnumWrapGCM.tpl.txt.meta',NULL,NULL,NULL,NULL),(1178,45,'A','Assets/XLua/Src/Editor/Template/LuaInterfaceBridge.tpl.txt',NULL,NULL,NULL,NULL),(1179,45,'A','Assets/XLua/Src/Editor/Template/LuaInterfaceBridge.tpl.txt.meta',NULL,NULL,NULL,NULL),(1180,45,'A','Assets/XLua/Src/Editor/Template/LuaRegister.tpl.txt',NULL,NULL,NULL,NULL),(1181,45,'A','Assets/XLua/Src/Editor/Template/LuaRegister.tpl.txt.meta',NULL,NULL,NULL,NULL),(1182,45,'A','Assets/XLua/Src/Editor/Template/LuaRegisterGCM.tpl.txt',NULL,NULL,NULL,NULL),(1183,45,'A','Assets/XLua/Src/Editor/Template/LuaRegisterGCM.tpl.txt.meta',NULL,NULL,NULL,NULL),(1184,45,'A','Assets/XLua/Src/Editor/Template/LuaWrapPusher.tpl.txt',NULL,NULL,NULL,NULL),(1185,45,'A','Assets/XLua/Src/Editor/Template/LuaWrapPusher.tpl.txt.meta',NULL,NULL,NULL,NULL),(1186,45,'A','Assets/XLua/Src/Editor/Template/PackUnpack.tpl.txt',NULL,NULL,NULL,NULL),(1187,45,'A','Assets/XLua/Src/Editor/Template/PackUnpack.tpl.txt.meta',NULL,NULL,NULL,NULL),(1188,45,'A','Assets/XLua/Src/Editor/Template/TemplateCommon.lua.txt',NULL,NULL,NULL,NULL),(1189,45,'A','Assets/XLua/Src/Editor/Template/TemplateCommon.lua.txt.meta',NULL,NULL,NULL,NULL),(1190,45,'A','Assets/XLua/Src/Editor/TemplateRef.cs',NULL,NULL,NULL,NULL),(1191,45,'A','Assets/XLua/Src/Editor/TemplateRef.cs.meta',NULL,NULL,NULL,NULL),(1192,45,'A','Assets/XLua/Src/GenAttributes.cs',NULL,NULL,NULL,NULL),(1193,45,'A','Assets/XLua/Src/GenAttributes.cs.meta',NULL,NULL,NULL,NULL),(1194,45,'A','Assets/XLua/Src/InternalGlobals.cs',NULL,NULL,NULL,NULL),(1195,45,'A','Assets/XLua/Src/InternalGlobals.cs.meta',NULL,NULL,NULL,NULL),(1196,45,'A','Assets/XLua/Src/LuaBase.cs',NULL,NULL,NULL,NULL),(1197,45,'A','Assets/XLua/Src/LuaBase.cs.meta',NULL,NULL,NULL,NULL),(1198,45,'A','Assets/XLua/Src/LuaDLL.cs',NULL,NULL,NULL,NULL),(1199,45,'A','Assets/XLua/Src/LuaDLL.cs.meta',NULL,NULL,NULL,NULL),(1200,45,'A','Assets/XLua/Src/LuaEnv.cs',NULL,NULL,NULL,NULL),(1201,45,'A','Assets/XLua/Src/LuaEnv.cs.meta',NULL,NULL,NULL,NULL),(1202,45,'A','Assets/XLua/Src/LuaException.cs',NULL,NULL,NULL,NULL),(1203,45,'A','Assets/XLua/Src/LuaException.cs.meta',NULL,NULL,NULL,NULL),(1204,45,'A','Assets/XLua/Src/LuaFunction.cs',NULL,NULL,NULL,NULL),(1205,45,'A','Assets/XLua/Src/LuaFunction.cs.meta',NULL,NULL,NULL,NULL),(1206,45,'A','Assets/XLua/Src/LuaTable.cs',NULL,NULL,NULL,NULL),(1207,45,'A','Assets/XLua/Src/LuaTable.cs.meta',NULL,NULL,NULL,NULL),(1208,45,'A','Assets/XLua/Src/MethodWarpsCache.cs',NULL,NULL,NULL,NULL),(1209,45,'A','Assets/XLua/Src/MethodWarpsCache.cs.meta',NULL,NULL,NULL,NULL),(1210,45,'A','Assets/XLua/Src/ObjectCasters.cs',NULL,NULL,NULL,NULL),(1211,45,'A','Assets/XLua/Src/ObjectCasters.cs.meta',NULL,NULL,NULL,NULL),(1212,45,'A','Assets/XLua/Src/ObjectPool.cs',NULL,NULL,NULL,NULL),(1213,45,'A','Assets/XLua/Src/ObjectPool.cs.meta',NULL,NULL,NULL,NULL),(1214,45,'A','Assets/XLua/Src/ObjectTranslator.cs',NULL,NULL,NULL,NULL),(1215,45,'A','Assets/XLua/Src/ObjectTranslator.cs.meta',NULL,NULL,NULL,NULL),(1216,45,'A','Assets/XLua/Src/ObjectTranslatorPool.cs',NULL,NULL,NULL,NULL),(1217,45,'A','Assets/XLua/Src/ObjectTranslatorPool.cs.meta',NULL,NULL,NULL,NULL),(1218,45,'A','Assets/XLua/Src/RawObject.cs',NULL,NULL,NULL,NULL),(1219,45,'A','Assets/XLua/Src/RawObject.cs.meta',NULL,NULL,NULL,NULL),(1220,45,'A','Assets/XLua/Src/SignatureLoader.cs',NULL,NULL,NULL,NULL),(1221,45,'A','Assets/XLua/Src/SignatureLoader.cs.meta',NULL,NULL,NULL,NULL),(1222,45,'A','Assets/XLua/Src/StaticLuaCallbacks.cs',NULL,NULL,NULL,NULL),(1223,45,'A','Assets/XLua/Src/StaticLuaCallbacks.cs.meta',NULL,NULL,NULL,NULL),(1224,45,'A','Assets/XLua/Src/TemplateEngine.meta',NULL,NULL,NULL,NULL),(1225,45,'A','Assets/XLua/Src/TemplateEngine/TemplateEngine.cs',NULL,NULL,NULL,NULL),(1226,45,'A','Assets/XLua/Src/TemplateEngine/TemplateEngine.cs.meta',NULL,NULL,NULL,NULL),(1227,45,'A','Assets/XLua/Src/TypeExtensions.cs',NULL,NULL,NULL,NULL),(1228,45,'A','Assets/XLua/Src/TypeExtensions.cs.meta',NULL,NULL,NULL,NULL),(1229,45,'A','Assets/XLua/Src/Utils.cs',NULL,NULL,NULL,NULL),(1230,45,'A','Assets/XLua/Src/Utils.cs.meta',NULL,NULL,NULL,NULL),(1231,45,'A','ProjectSettings/AudioManager.asset',NULL,NULL,NULL,NULL),(1232,45,'A','ProjectSettings/ClusterInputManager.asset',NULL,NULL,NULL,NULL),(1233,45,'A','ProjectSettings/DynamicsManager.asset',NULL,NULL,NULL,NULL),(1234,45,'A','ProjectSettings/EditorBuildSettings.asset',NULL,NULL,NULL,NULL),(1235,45,'A','ProjectSettings/EditorSettings.asset',NULL,NULL,NULL,NULL),(1236,45,'A','ProjectSettings/GraphicsSettings.asset',NULL,NULL,NULL,NULL),(1237,45,'A','ProjectSettings/InputManager.asset',NULL,NULL,NULL,NULL),(1238,45,'A','ProjectSettings/NavMeshAreas.asset',NULL,NULL,NULL,NULL),(1239,45,'A','ProjectSettings/NetworkManager.asset',NULL,NULL,NULL,NULL),(1240,45,'A','ProjectSettings/Physics2DSettings.asset',NULL,NULL,NULL,NULL),(1241,45,'A','ProjectSettings/ProjectSettings.asset',NULL,NULL,NULL,NULL),(1242,45,'A','ProjectSettings/ProjectVersion.txt',NULL,NULL,NULL,NULL),(1243,45,'A','ProjectSettings/QualitySettings.asset',NULL,NULL,NULL,NULL),(1244,45,'A','ProjectSettings/TagManager.asset',NULL,NULL,NULL,NULL),(1245,45,'A','ProjectSettings/TimeManager.asset',NULL,NULL,NULL,NULL),(1246,45,'A','ProjectSettings/UnityConnectSettings.asset',NULL,NULL,NULL,NULL),(1247,45,'A','doc/actions.md',NULL,NULL,NULL,NULL),(1248,46,'M','.gitignore',NULL,NULL,NULL,NULL),(1249,46,'A','.gitmodules',NULL,NULL,NULL,NULL),(1250,46,'A','Assets/ABResources.meta',NULL,NULL,NULL,NULL),(1251,46,'A','Assets/ABResources/Lua.meta',NULL,NULL,NULL,NULL),(1252,46,'A','Assets/ABResources/Lua/Table.meta',NULL,NULL,NULL,NULL),(1253,46,'A','Assets/ABResources/Lua/Table/.lastWriteTime',NULL,NULL,NULL,NULL),(1254,46,'A','Assets/ABResources/Lua/lua.meta',NULL,NULL,NULL,NULL),(1255,46,'A','Assets/ABResources/Lua/lua/.lastWriteTime',NULL,NULL,NULL,NULL),(1256,46,'A','Assets/ABResources/Lua/lua/main.lua',NULL,NULL,NULL,NULL),(1257,46,'A','Assets/ABResources/Lua/lua/main.lua.meta',NULL,NULL,NULL,NULL),(1258,46,'A','Assets/ABResources/Lua/utility.meta',NULL,NULL,NULL,NULL),(1259,46,'A','Assets/ABResources/Lua/utility/.lastWriteTime',NULL,NULL,NULL,NULL),(1260,46,'A','Assets/ABResources/Lua/utility/BridgingClass.lua',NULL,NULL,NULL,NULL),(1261,46,'A','Assets/ABResources/Lua/utility/BridgingClass.lua.meta',NULL,NULL,NULL,NULL),(1262,46,'A','Assets/ABResources/PreDownload.meta',NULL,NULL,NULL,NULL),(1263,46,'A','Assets/ABResources/TestGroup1.meta',NULL,NULL,NULL,NULL),(1264,46,'A','Assets/ABResources/TestGroup1/b1.meta',NULL,NULL,NULL,NULL),(1265,46,'A','Assets/ABResources/TestGroup1/b1/.lastWriteTime',NULL,NULL,NULL,NULL),(1266,46,'A','Assets/ABResources/TestGroup1/b1/Stripe.png',NULL,NULL,NULL,NULL),(1267,46,'A','Assets/ABResources/TestGroup1/b1/Stripe.png.meta',NULL,NULL,NULL,NULL),(1268,46,'A','Assets/ABResources/TestGroup2.meta',NULL,NULL,NULL,NULL),(1269,46,'A','Assets/ABResources/TestGroup2/b1.meta',NULL,NULL,NULL,NULL),(1270,46,'A','Assets/ABResources/TestGroup2/b1/.lastWriteTime',NULL,NULL,NULL,NULL),(1271,46,'A','Assets/ABResources/TestGroup2/b1/Stripe.png',NULL,NULL,NULL,NULL),(1272,46,'A','Assets/ABResources/TestGroup2/b1/Stripe.png.meta',NULL,NULL,NULL,NULL),(1273,46,'A','Assets/BuildTools.meta',NULL,NULL,NULL,NULL),(1274,46,'A','Assets/BuildTools/BuildScript.cs',NULL,NULL,NULL,NULL),(1275,46,'A','Assets/BuildTools/BuildScript.cs.meta',NULL,NULL,NULL,NULL),(1276,46,'A','Assets/BuildTools/BundleConfig.asset',NULL,NULL,NULL,NULL),(1277,46,'A','Assets/BuildTools/BundleConfig.asset.meta',NULL,NULL,NULL,NULL),(1278,46,'A','Assets/BuildTools/EditorLuaHelper.cs',NULL,NULL,NULL,NULL),(1279,46,'A','Assets/BuildTools/EditorLuaHelper.cs.meta',NULL,NULL,NULL,NULL),(1280,46,'A','Assets/Editor.meta',NULL,NULL,NULL,NULL),(1281,46,'A','Assets/Editor/LibXCProjModifier.dll',NULL,NULL,NULL,NULL),(1282,46,'A','Assets/Editor/LibXCProjModifier.dll.meta',NULL,NULL,NULL,NULL),(1283,46,'A','Assets/Editor/XcodeSettingsPostProcesser.cs',NULL,NULL,NULL,NULL),(1284,46,'A','Assets/Editor/XcodeSettingsPostProcesser.cs.meta',NULL,NULL,NULL,NULL),(1285,46,'A','Assets/Plugins/.YamlDotNet',NULL,NULL,NULL,NULL),(1286,46,'A','Assets/Plugins/Lzma.meta',NULL,NULL,NULL,NULL),(1287,46,'A','Assets/Plugins/Lzma/Common.meta',NULL,NULL,NULL,NULL),(1288,46,'A','Assets/Plugins/Lzma/Common/CRC.cs',NULL,NULL,NULL,NULL),(1289,46,'A','Assets/Plugins/Lzma/Common/CRC.cs.meta',NULL,NULL,NULL,NULL),(1290,46,'A','Assets/Plugins/Lzma/Common/CommandLineParser.cs',NULL,NULL,NULL,NULL),(1291,46,'A','Assets/Plugins/Lzma/Common/CommandLineParser.cs.meta',NULL,NULL,NULL,NULL),(1292,46,'A','Assets/Plugins/Lzma/Common/InBuffer.cs',NULL,NULL,NULL,NULL),(1293,46,'A','Assets/Plugins/Lzma/Common/InBuffer.cs.meta',NULL,NULL,NULL,NULL),(1294,46,'A','Assets/Plugins/Lzma/Common/OutBuffer.cs',NULL,NULL,NULL,NULL),(1295,46,'A','Assets/Plugins/Lzma/Common/OutBuffer.cs.meta',NULL,NULL,NULL,NULL),(1296,46,'A','Assets/Plugins/Lzma/Compress.meta',NULL,NULL,NULL,NULL),(1297,46,'A','Assets/Plugins/Lzma/Compress/LZ.meta',NULL,NULL,NULL,NULL),(1298,46,'A','Assets/Plugins/Lzma/Compress/LZ/IMatchFinder.cs',NULL,NULL,NULL,NULL),(1299,46,'A','Assets/Plugins/Lzma/Compress/LZ/IMatchFinder.cs.meta',NULL,NULL,NULL,NULL),(1300,46,'A','Assets/Plugins/Lzma/Compress/LZ/LzBinTree.cs',NULL,NULL,NULL,NULL),(1301,46,'A','Assets/Plugins/Lzma/Compress/LZ/LzBinTree.cs.meta',NULL,NULL,NULL,NULL),(1302,46,'A','Assets/Plugins/Lzma/Compress/LZ/LzInWindow.cs',NULL,NULL,NULL,NULL),(1303,46,'A','Assets/Plugins/Lzma/Compress/LZ/LzInWindow.cs.meta',NULL,NULL,NULL,NULL),(1304,46,'A','Assets/Plugins/Lzma/Compress/LZ/LzOutWindow.cs',NULL,NULL,NULL,NULL),(1305,46,'A','Assets/Plugins/Lzma/Compress/LZ/LzOutWindow.cs.meta',NULL,NULL,NULL,NULL),(1306,46,'A','Assets/Plugins/Lzma/Compress/LZMA.meta',NULL,NULL,NULL,NULL),(1307,46,'A','Assets/Plugins/Lzma/Compress/LZMA/LzmaBase.cs',NULL,NULL,NULL,NULL),(1308,46,'A','Assets/Plugins/Lzma/Compress/LZMA/LzmaBase.cs.meta',NULL,NULL,NULL,NULL),(1309,46,'A','Assets/Plugins/Lzma/Compress/LZMA/LzmaDecoder.cs',NULL,NULL,NULL,NULL),(1310,46,'A','Assets/Plugins/Lzma/Compress/LZMA/LzmaDecoder.cs.meta',NULL,NULL,NULL,NULL),(1311,46,'A','Assets/Plugins/Lzma/Compress/LZMA/LzmaEncoder.cs',NULL,NULL,NULL,NULL),(1312,46,'A','Assets/Plugins/Lzma/Compress/LZMA/LzmaEncoder.cs.meta',NULL,NULL,NULL,NULL),(1313,46,'A','Assets/Plugins/Lzma/Compress/LzmaAlone.meta',NULL,NULL,NULL,NULL),(1314,46,'A','Assets/Plugins/Lzma/Compress/LzmaAlone/LzmaAlone.cs',NULL,NULL,NULL,NULL),(1315,46,'A','Assets/Plugins/Lzma/Compress/LzmaAlone/LzmaAlone.cs.meta',NULL,NULL,NULL,NULL),(1316,46,'A','Assets/Plugins/Lzma/Compress/LzmaAlone/LzmaAlone.csproj.meta',NULL,NULL,NULL,NULL),(1317,46,'A','Assets/Plugins/Lzma/Compress/LzmaAlone/LzmaAlone.sln.meta',NULL,NULL,NULL,NULL),(1318,46,'A','Assets/Plugins/Lzma/Compress/LzmaAlone/LzmaBench.cs',NULL,NULL,NULL,NULL),(1319,46,'A','Assets/Plugins/Lzma/Compress/LzmaAlone/LzmaBench.cs.meta',NULL,NULL,NULL,NULL),(1320,46,'A','Assets/Plugins/Lzma/Compress/LzmaAlone/Properties.meta',NULL,NULL,NULL,NULL),(1321,46,'A','Assets/Plugins/Lzma/Compress/LzmaAlone/Properties/AssemblyInfo.cs',NULL,NULL,NULL,NULL),(1322,46,'A','Assets/Plugins/Lzma/Compress/LzmaAlone/Properties/AssemblyInfo.cs.meta',NULL,NULL,NULL,NULL),(1323,46,'A','Assets/Plugins/Lzma/Compress/LzmaAlone/Properties/Resources.cs',NULL,NULL,NULL,NULL),(1324,46,'A','Assets/Plugins/Lzma/Compress/LzmaAlone/Properties/Resources.cs.meta',NULL,NULL,NULL,NULL),(1325,46,'A','Assets/Plugins/Lzma/Compress/LzmaAlone/Properties/Settings.cs',NULL,NULL,NULL,NULL),(1326,46,'A','Assets/Plugins/Lzma/Compress/LzmaAlone/Properties/Settings.cs.meta',NULL,NULL,NULL,NULL),(1327,46,'A','Assets/Plugins/Lzma/Compress/RangeCoder.meta',NULL,NULL,NULL,NULL),(1328,46,'A','Assets/Plugins/Lzma/Compress/RangeCoder/RangeCoder.cs',NULL,NULL,NULL,NULL),(1329,46,'A','Assets/Plugins/Lzma/Compress/RangeCoder/RangeCoder.cs.meta',NULL,NULL,NULL,NULL),(1330,46,'A','Assets/Plugins/Lzma/Compress/RangeCoder/RangeCoderBit.cs',NULL,NULL,NULL,NULL),(1331,46,'A','Assets/Plugins/Lzma/Compress/RangeCoder/RangeCoderBit.cs.meta',NULL,NULL,NULL,NULL),(1332,46,'A','Assets/Plugins/Lzma/Compress/RangeCoder/RangeCoderBitTree.cs',NULL,NULL,NULL,NULL),(1333,46,'A','Assets/Plugins/Lzma/Compress/RangeCoder/RangeCoderBitTree.cs.meta',NULL,NULL,NULL,NULL),(1334,46,'A','Assets/Plugins/Lzma/ICoder.cs',NULL,NULL,NULL,NULL),(1335,46,'A','Assets/Plugins/Lzma/ICoder.cs.meta',NULL,NULL,NULL,NULL),(1336,46,'A','Assets/Plugins/Lzma/TailStruct.cs',NULL,NULL,NULL,NULL),(1337,46,'A','Assets/Plugins/Lzma/TailStruct.cs.meta',NULL,NULL,NULL,NULL),(1338,46,'A','Assets/Plugins/Yaml.meta',NULL,NULL,NULL,NULL),(1339,46,'A','Assets/Plugins/Yaml/net35.meta',NULL,NULL,NULL,NULL),(1340,46,'A','Assets/Plugins/Yaml/net35/YamlDotNet.dll',NULL,NULL,NULL,NULL),(1341,46,'A','Assets/Plugins/Yaml/net35/YamlDotNet.dll.meta',NULL,NULL,NULL,NULL),(1342,46,'A','Assets/Plugins/Yaml/net35/YamlDotNet.pdb.meta',NULL,NULL,NULL,NULL),(1343,46,'A','Assets/Plugins/Yaml/net35/YamlDotNet.xml',NULL,NULL,NULL,NULL),(1344,46,'A','Assets/Plugins/Yaml/net35/YamlDotNet.xml.meta',NULL,NULL,NULL,NULL),(1345,46,'A','Assets/ProjectConfig.asset',NULL,NULL,NULL,NULL),(1346,46,'A','Assets/ProjectConfig.asset.meta',NULL,NULL,NULL,NULL),(1347,46,'A','Assets/Scripts.meta',NULL,NULL,NULL,NULL),(1348,46,'A','Assets/Scripts/BundleConfig.cs',NULL,NULL,NULL,NULL),(1349,46,'A','Assets/Scripts/BundleConfig.cs.meta',NULL,NULL,NULL,NULL),(1350,46,'A','Assets/Scripts/BundleSys.cs',NULL,NULL,NULL,NULL),(1351,46,'A','Assets/Scripts/BundleSys.cs.meta',NULL,NULL,NULL,NULL),(1352,46,'A','Assets/Scripts/ProjectConfig.cs',NULL,NULL,NULL,NULL),(1353,46,'A','Assets/Scripts/ProjectConfig.cs.meta',NULL,NULL,NULL,NULL),(1354,46,'A','Assets/Scripts/UpdateSys.cs',NULL,NULL,NULL,NULL),(1355,46,'A','Assets/Scripts/UpdateSys.cs.meta',NULL,NULL,NULL,NULL),(1356,46,'A','Assets/Scripts/YamlHelper.cs',NULL,NULL,NULL,NULL),(1357,46,'A','Assets/Scripts/YamlHelper.cs.meta',NULL,NULL,NULL,NULL),(1358,46,'A','Assets/XLua/.Doc/Materials.meta',NULL,NULL,NULL,NULL),(1359,46,'A','Assets/XLua/.Doc/Materials/logo.mat',NULL,NULL,NULL,NULL),(1360,46,'A','Assets/XLua/.Doc/Materials/logo.mat.meta',NULL,NULL,NULL,NULL),(1361,46,'A','Assets/XLua/.Doc/XLua_API.doc',NULL,NULL,NULL,NULL),(1362,46,'A','Assets/XLua/.Doc/XLua_API.doc.meta',NULL,NULL,NULL,NULL),(1363,46,'A','Assets/XLua/.Doc/XLua_API.md',NULL,NULL,NULL,NULL),(1364,46,'A','Assets/XLua/.Doc/XLua_API.md.meta',NULL,NULL,NULL,NULL),(1365,46,'A','Assets/XLua/.Doc/XLua增加删除第三方lua库.doc',NULL,NULL,NULL,NULL),(1366,46,'A','Assets/XLua/.Doc/XLua增加删除第三方lua库.doc.meta',NULL,NULL,NULL,NULL),(1367,46,'A','Assets/XLua/.Doc/XLua增加删除第三方lua库.md',NULL,NULL,NULL,NULL),(1368,46,'A','Assets/XLua/.Doc/XLua增加删除第三方lua库.md.meta',NULL,NULL,NULL,NULL),(1369,46,'A','Assets/XLua/.Doc/XLua复杂值类型（struct）gc优化指南.doc',NULL,NULL,NULL,NULL),(1370,46,'A','Assets/XLua/.Doc/XLua复杂值类型（struct）gc优化指南.doc.meta',NULL,NULL,NULL,NULL),(1371,46,'A','Assets/XLua/.Doc/XLua复杂值类型（struct）gc优化指南.md',NULL,NULL,NULL,NULL),(1372,46,'A','Assets/XLua/.Doc/XLua复杂值类型（struct）gc优化指南.md.meta',NULL,NULL,NULL,NULL),(1373,46,'A','Assets/XLua/.Doc/XLua性能分析工具.doc',NULL,NULL,NULL,NULL),(1374,46,'A','Assets/XLua/.Doc/XLua性能分析工具.doc.meta',NULL,NULL,NULL,NULL),(1375,46,'A','Assets/XLua/.Doc/XLua性能分析工具.md',NULL,NULL,NULL,NULL),(1376,46,'A','Assets/XLua/.Doc/XLua性能分析工具.md.meta',NULL,NULL,NULL,NULL),(1377,46,'A','Assets/XLua/.Doc/XLua教程.doc',NULL,NULL,NULL,NULL),(1378,46,'A','Assets/XLua/.Doc/XLua教程.doc.meta',NULL,NULL,NULL,NULL),(1379,46,'A','Assets/XLua/.Doc/XLua教程.md',NULL,NULL,NULL,NULL),(1380,46,'A','Assets/XLua/.Doc/XLua教程.md.meta',NULL,NULL,NULL,NULL),(1381,46,'A','Assets/XLua/.Doc/XLua的配置.doc',NULL,NULL,NULL,NULL),(1382,46,'A','Assets/XLua/.Doc/XLua的配置.doc.meta',NULL,NULL,NULL,NULL),(1383,46,'A','Assets/XLua/.Doc/configure.md',NULL,NULL,NULL,NULL),(1384,46,'A','Assets/XLua/.Doc/configure.md.meta',NULL,NULL,NULL,NULL),(1385,46,'A','Assets/XLua/.Doc/custom_generate.md',NULL,NULL,NULL,NULL),(1386,46,'A','Assets/XLua/.Doc/custom_generate.md.meta',NULL,NULL,NULL,NULL),(1387,46,'A','Assets/XLua/.Doc/faq.md',NULL,NULL,NULL,NULL),(1388,46,'A','Assets/XLua/.Doc/faq.md.meta',NULL,NULL,NULL,NULL),(1389,46,'A','Assets/XLua/.Doc/features.md',NULL,NULL,NULL,NULL),(1390,46,'A','Assets/XLua/.Doc/features.md.meta',NULL,NULL,NULL,NULL),(1391,46,'A','Assets/XLua/.Doc/hotfix.md',NULL,NULL,NULL,NULL),(1392,46,'A','Assets/XLua/.Doc/hotfix.md.meta',NULL,NULL,NULL,NULL),(1393,46,'A','Assets/XLua/.Doc/logo.png',NULL,NULL,NULL,NULL),(1394,46,'A','Assets/XLua/.Doc/logo.png.meta',NULL,NULL,NULL,NULL),(1395,46,'A','Assets/XLua/.Doc/signature.md',NULL,NULL,NULL,NULL),(1396,46,'A','Assets/XLua/.Doc/signature.md.meta',NULL,NULL,NULL,NULL),(1397,46,'A','Assets/XLua/.Doc/xLua.png',NULL,NULL,NULL,NULL),(1398,46,'A','Assets/XLua/.Doc/xLua.png.meta',NULL,NULL,NULL,NULL),(1399,46,'D','Assets/XLua/Doc.meta',NULL,NULL,NULL,NULL),(1400,46,'D','Assets/XLua/Doc/Materials.meta',NULL,NULL,NULL,NULL),(1401,46,'D','Assets/XLua/Doc/Materials/logo.mat',NULL,NULL,NULL,NULL),(1402,46,'D','Assets/XLua/Doc/Materials/logo.mat.meta',NULL,NULL,NULL,NULL),(1403,46,'D','Assets/XLua/Doc/XLua_API.doc',NULL,NULL,NULL,NULL),(1404,46,'D','Assets/XLua/Doc/XLua_API.doc.meta',NULL,NULL,NULL,NULL),(1405,46,'D','Assets/XLua/Doc/XLua_API.md',NULL,NULL,NULL,NULL),(1406,46,'D','Assets/XLua/Doc/XLua增加删除第三方lua库.doc',NULL,NULL,NULL,NULL),(1407,46,'D','Assets/XLua/Doc/XLua增加删除第三方lua库.doc.meta',NULL,NULL,NULL,NULL),(1408,46,'D','Assets/XLua/Doc/XLua增加删除第三方lua库.md',NULL,NULL,NULL,NULL),(1409,46,'D','Assets/XLua/Doc/XLua复杂值类型（struct）gc优化指南.doc',NULL,NULL,NULL,NULL),(1410,46,'D','Assets/XLua/Doc/XLua复杂值类型（struct）gc优化指南.doc.meta',NULL,NULL,NULL,NULL),(1411,46,'D','Assets/XLua/Doc/XLua复杂值类型（struct）gc优化指南.md',NULL,NULL,NULL,NULL),(1412,46,'D','Assets/XLua/Doc/XLua性能分析工具.doc',NULL,NULL,NULL,NULL),(1413,46,'D','Assets/XLua/Doc/XLua性能分析工具.doc.meta',NULL,NULL,NULL,NULL),(1414,46,'D','Assets/XLua/Doc/XLua性能分析工具.md',NULL,NULL,NULL,NULL),(1415,46,'D','Assets/XLua/Doc/XLua教程.doc',NULL,NULL,NULL,NULL),(1416,46,'D','Assets/XLua/Doc/XLua教程.doc.meta',NULL,NULL,NULL,NULL),(1417,46,'D','Assets/XLua/Doc/XLua教程.md',NULL,NULL,NULL,NULL),(1418,46,'D','Assets/XLua/Doc/XLua的配置.doc',NULL,NULL,NULL,NULL),(1419,46,'D','Assets/XLua/Doc/XLua的配置.doc.meta',NULL,NULL,NULL,NULL),(1420,46,'D','Assets/XLua/Doc/configure.md',NULL,NULL,NULL,NULL),(1421,46,'D','Assets/XLua/Doc/configure.md.meta',NULL,NULL,NULL,NULL),(1422,46,'D','Assets/XLua/Doc/custom_generate.md',NULL,NULL,NULL,NULL),(1423,46,'D','Assets/XLua/Doc/custom_generate.md.meta',NULL,NULL,NULL,NULL),(1424,46,'D','Assets/XLua/Doc/faq.md',NULL,NULL,NULL,NULL),(1425,46,'D','Assets/XLua/Doc/faq.md.meta',NULL,NULL,NULL,NULL),(1426,46,'D','Assets/XLua/Doc/features.md',NULL,NULL,NULL,NULL),(1427,46,'D','Assets/XLua/Doc/features.md.meta',NULL,NULL,NULL,NULL),(1428,46,'D','Assets/XLua/Doc/hotfix.md',NULL,NULL,NULL,NULL),(1429,46,'D','Assets/XLua/Doc/hotfix.md.meta',NULL,NULL,NULL,NULL),(1430,46,'D','Assets/XLua/Doc/logo.png',NULL,NULL,NULL,NULL),(1431,46,'D','Assets/XLua/Doc/logo.png.meta',NULL,NULL,NULL,NULL),(1432,46,'D','Assets/XLua/Doc/signature.md',NULL,NULL,NULL,NULL),(1433,46,'D','Assets/XLua/Doc/signature.md.meta',NULL,NULL,NULL,NULL),(1434,46,'D','Assets/XLua/Doc/xLua.png',NULL,NULL,NULL,NULL),(1435,46,'D','Assets/XLua/Doc/xLua.png.meta',NULL,NULL,NULL,NULL),(1436,46,'M','Assets/XLua/Src/LuaTable.cs',NULL,NULL,NULL,NULL),(1437,46,'M','ProjectSettings/AudioManager.asset',NULL,NULL,NULL,NULL),(1438,46,'M','ProjectSettings/ClusterInputManager.asset',NULL,NULL,NULL,NULL),(1439,46,'M','ProjectSettings/DynamicsManager.asset',NULL,NULL,NULL,NULL),(1440,46,'M','ProjectSettings/EditorBuildSettings.asset',NULL,NULL,NULL,NULL),(1441,46,'M','ProjectSettings/EditorSettings.asset',NULL,NULL,NULL,NULL),(1442,46,'M','ProjectSettings/GraphicsSettings.asset',NULL,NULL,NULL,NULL),(1443,46,'M','ProjectSettings/InputManager.asset',NULL,NULL,NULL,NULL),(1444,46,'M','ProjectSettings/NavMeshAreas.asset',NULL,NULL,NULL,NULL),(1445,46,'M','ProjectSettings/NetworkManager.asset',NULL,NULL,NULL,NULL),(1446,46,'M','ProjectSettings/Physics2DSettings.asset',NULL,NULL,NULL,NULL),(1447,46,'M','ProjectSettings/ProjectSettings.asset',NULL,NULL,NULL,NULL),(1448,46,'M','ProjectSettings/QualitySettings.asset',NULL,NULL,NULL,NULL),(1449,46,'M','ProjectSettings/TagManager.asset',NULL,NULL,NULL,NULL),(1450,46,'M','ProjectSettings/TimeManager.asset',NULL,NULL,NULL,NULL),(1451,46,'M','ProjectSettings/UnityConnectSettings.asset',NULL,NULL,NULL,NULL),(1452,46,'A','doc/design.md',NULL,NULL,NULL,NULL),(1453,46,'A','doc/todo.md',NULL,NULL,NULL,NULL),(1454,46,'A','tools/excel2lua/Convert.exe',NULL,NULL,NULL,NULL),(1455,46,'A','tools/excel2lua/Convert.exe.config',NULL,NULL,NULL,NULL),(1456,46,'A','tools/excel2lua/ICSharpCode.SharpZipLib.dll',NULL,NULL,NULL,NULL),(1457,46,'A','tools/excel2lua/NPOI.OOXML.dll',NULL,NULL,NULL,NULL),(1458,46,'A','tools/excel2lua/NPOI.OOXML.xml',NULL,NULL,NULL,NULL),(1459,46,'A','tools/excel2lua/NPOI.OpenXml4Net.dll',NULL,NULL,NULL,NULL),(1460,46,'A','tools/excel2lua/NPOI.OpenXml4Net.xml',NULL,NULL,NULL,NULL),(1461,46,'A','tools/excel2lua/NPOI.OpenXmlFormats.dll',NULL,NULL,NULL,NULL),(1462,46,'A','tools/excel2lua/NPOI.dll',NULL,NULL,NULL,NULL),(1463,46,'A','tools/excel2lua/NPOI.xml',NULL,NULL,NULL,NULL),(1464,47,'M','Assets/BuildTools/BuildScript.cs',NULL,NULL,NULL,NULL),(1465,47,'D','Assets/BuildTools/EditorLuaHelper.cs',NULL,NULL,NULL,NULL),(1466,47,'D','Assets/BuildTools/EditorLuaHelper.cs.meta',NULL,NULL,NULL,NULL),(1467,47,'A','Assets/BuildTools/LuaHelper.cs',NULL,NULL,NULL,NULL),(1468,47,'M','Assets/Scripts/BundleConfig.cs',NULL,NULL,NULL,NULL),(1469,47,'M','Assets/Scripts/BundleSys.cs',NULL,NULL,NULL,NULL),(1470,48,'D','Assets/ABResources/PreDownload.meta',NULL,NULL,NULL,NULL),(1471,48,'A','Assets/ABResources/README.md',NULL,NULL,NULL,NULL),(1472,48,'A','Assets/ABResources/README.md.meta',NULL,NULL,NULL,NULL),(1473,48,'D','Assets/ABResources/TestGroup1.meta',NULL,NULL,NULL,NULL),(1474,48,'D','Assets/ABResources/TestGroup1/b1.meta',NULL,NULL,NULL,NULL),(1475,48,'D','Assets/ABResources/TestGroup1/b1/.lastWriteTime',NULL,NULL,NULL,NULL),(1476,48,'D','Assets/ABResources/TestGroup1/b1/Stripe.png',NULL,NULL,NULL,NULL),(1477,48,'D','Assets/ABResources/TestGroup1/b1/Stripe.png.meta',NULL,NULL,NULL,NULL),(1478,48,'D','Assets/ABResources/TestGroup2.meta',NULL,NULL,NULL,NULL),(1479,48,'D','Assets/ABResources/TestGroup2/b1.meta',NULL,NULL,NULL,NULL),(1480,48,'D','Assets/ABResources/TestGroup2/b1/.lastWriteTime',NULL,NULL,NULL,NULL),(1481,48,'D','Assets/ABResources/TestGroup2/b1/Stripe.png',NULL,NULL,NULL,NULL),(1482,48,'D','Assets/ABResources/TestGroup2/b1/Stripe.png.meta',NULL,NULL,NULL,NULL),(1483,48,'A','Assets/ABResources/common.meta',NULL,NULL,NULL,NULL),(1484,48,'A','Assets/ABResources/common/config.meta',NULL,NULL,NULL,NULL),(1485,48,'A','Assets/ABResources/common/config/BundleConfig.asset',NULL,NULL,NULL,NULL),(1486,48,'A','Assets/ABResources/common/config/BundleConfig.asset.meta',NULL,NULL,NULL,NULL),(1487,48,'A','Assets/ABResources/common/config/BundleConfig.cs',NULL,NULL,NULL,NULL),(1488,48,'A','Assets/ABResources/common/config/BundleConfig.cs.meta',NULL,NULL,NULL,NULL),(1489,48,'A','Assets/ABResources/common/config/ProjectConfig.asset',NULL,NULL,NULL,NULL),(1490,48,'A','Assets/ABResources/common/config/ProjectConfig.asset.meta',NULL,NULL,NULL,NULL),(1491,48,'A','Assets/ABResources/common/config/ProjectConfig.cs',NULL,NULL,NULL,NULL),(1492,48,'A','Assets/ABResources/common/config/ProjectConfig.cs.meta',NULL,NULL,NULL,NULL),(1493,48,'A','Assets/ABResources/common/template.meta',NULL,NULL,NULL,NULL),(1494,48,'A','Assets/ABResources/common/template/.lastWriteTime',NULL,NULL,NULL,NULL),(1495,48,'A','Assets/ABResources/common/template/res.meta',NULL,NULL,NULL,NULL),(1496,48,'A','Assets/ABResources/common/template/res/.lastWriteTime',NULL,NULL,NULL,NULL),(1497,48,'A','Assets/ABResources/common/template/src.meta',NULL,NULL,NULL,NULL),(1498,48,'A','Assets/ABResources/common/template/src/.lastWriteTime',NULL,NULL,NULL,NULL),(1499,48,'A','Assets/ABResources/common/template/src/LuaMonoBehaviour.lua',NULL,NULL,NULL,NULL),(1500,48,'A','Assets/ABResources/common/template/src/LuaMonoBehaviour.lua.meta',NULL,NULL,NULL,NULL),(1501,48,'A','Assets/ABResources/common/template/template.prefab',NULL,NULL,NULL,NULL),(1502,48,'A','Assets/ABResources/common/template/template.prefab.meta',NULL,NULL,NULL,NULL),(1503,48,'A','Assets/ABResources/ui.meta',NULL,NULL,NULL,NULL),(1504,48,'A','Assets/ABResources/ui/login.meta',NULL,NULL,NULL,NULL),(1505,48,'A','Assets/ABResources/ui/login/.lastWriteTime',NULL,NULL,NULL,NULL),(1506,48,'A','Assets/ABResources/ui/login/res.meta',NULL,NULL,NULL,NULL),(1507,48,'A','Assets/ABResources/ui/login/res/.lastWriteTime',NULL,NULL,NULL,NULL),(1508,48,'A','Assets/ABResources/ui/login/src.meta',NULL,NULL,NULL,NULL),(1509,48,'A','Assets/ABResources/ui/login/src/.lastWriteTime',NULL,NULL,NULL,NULL),(1510,48,'A','Assets/ABResources/ui/login/src/LuaMonoBehaviour.lua',NULL,NULL,NULL,NULL),(1511,48,'A','Assets/ABResources/ui/login/src/LuaMonoBehaviour.lua.meta',NULL,NULL,NULL,NULL),(1512,48,'A','Assets/ABResources/ui/login/template.prefab',NULL,NULL,NULL,NULL),(1513,48,'A','Assets/ABResources/ui/login/template.prefab.meta',NULL,NULL,NULL,NULL),(1514,48,'M','Assets/BuildTools/BuildScript.cs',NULL,NULL,NULL,NULL),(1515,48,'M','Assets/BuildTools/BundleConfig.asset',NULL,NULL,NULL,NULL),(1516,48,'D','Assets/BuildTools/LuaHelper.cs',NULL,NULL,NULL,NULL),(1517,48,'A','Assets/Plugins/AppLog.dll',NULL,NULL,NULL,NULL),(1518,48,'A','Assets/Plugins/AppLog.dll.meta',NULL,NULL,NULL,NULL),(1519,48,'A','Assets/Plugins/LibSocket.dll',NULL,NULL,NULL,NULL),(1520,48,'A','Assets/Plugins/LibSocket.dll.meta',NULL,NULL,NULL,NULL),(1521,48,'D','Assets/ProjectConfig.asset',NULL,NULL,NULL,NULL),(1522,48,'D','Assets/ProjectConfig.asset.meta',NULL,NULL,NULL,NULL),(1523,48,'A','Assets/Scene.meta',NULL,NULL,NULL,NULL),(1524,48,'A','Assets/Scene/boot.unity',NULL,NULL,NULL,NULL),(1525,48,'A','Assets/Scene/boot.unity.meta',NULL,NULL,NULL,NULL),(1526,48,'M','Assets/Scripts.meta',NULL,NULL,NULL,NULL),(1527,48,'A','Assets/Scripts/AppBoot.cs',NULL,NULL,NULL,NULL),(1528,48,'A','Assets/Scripts/AppBoot.cs.meta',NULL,NULL,NULL,NULL),(1529,48,'D','Assets/Scripts/BundleConfig.cs',NULL,NULL,NULL,NULL),(1530,48,'D','Assets/Scripts/BundleConfig.cs.meta',NULL,NULL,NULL,NULL),(1531,48,'M','Assets/Scripts/BundleSys.cs',NULL,NULL,NULL,NULL),(1532,48,'A','Assets/Scripts/LuaAsset.cs',NULL,NULL,NULL,NULL),(1533,48,'A','Assets/Scripts/LuaAsset.cs.meta',NULL,NULL,NULL,NULL),(1534,48,'A','Assets/Scripts/LuaMonoBehaviour.cs',NULL,NULL,NULL,NULL),(1535,48,'A','Assets/Scripts/LuaMonoBehaviour.cs.meta',NULL,NULL,NULL,NULL),(1536,48,'D','Assets/Scripts/ProjectConfig.cs',NULL,NULL,NULL,NULL),(1537,48,'D','Assets/Scripts/ProjectConfig.cs.meta',NULL,NULL,NULL,NULL),(1538,48,'A','Assets/Scripts/Singleton.cs',NULL,NULL,NULL,NULL),(1539,48,'A','Assets/Scripts/Singleton.cs.meta',NULL,NULL,NULL,NULL),(1540,48,'A','Assets/Scripts/module.meta',NULL,NULL,NULL,NULL),(1541,48,'A','Assets/Scripts/module/LuaSingleton.cs',NULL,NULL,NULL,NULL),(1542,48,'A','Assets/Scripts/module/LuaSingleton.cs.meta',NULL,NULL,NULL,NULL),(1543,48,'A','Assets/XLua/.gitignore',NULL,NULL,NULL,NULL),(1544,48,'M','Assets/XLua/Src/LuaTable.cs',NULL,NULL,NULL,NULL),(1545,48,'M','Assets/XLua/Src/StaticLuaCallbacks.cs',NULL,NULL,NULL,NULL),(1546,48,'M','ProjectSettings/EditorSettings.asset',NULL,NULL,NULL,NULL),(1547,48,'M','ProjectSettings/ProjectSettings.asset',NULL,NULL,NULL,NULL),(1548,48,'A','unity_template.sublime-project',NULL,NULL,NULL,NULL),(1549,48,'A','unity_template.sublime-workspace',NULL,NULL,NULL,NULL),(1550,49,'M','.gitignore',NULL,NULL,NULL,NULL),(1551,49,'D','Assets/ABResources/Lua/Table/.lastWriteTime',NULL,NULL,NULL,NULL),(1552,49,'D','Assets/ABResources/Lua/lua/.lastWriteTime',NULL,NULL,NULL,NULL),(1553,49,'D','Assets/ABResources/Lua/utility/.lastWriteTime',NULL,NULL,NULL,NULL),(1554,49,'A','Assets/ABResources/common/config/.lastWriteTime',NULL,NULL,NULL,NULL),(1555,49,'M','Assets/ABResources/common/config/BundleConfig.asset',NULL,NULL,NULL,NULL),(1556,49,'D','Assets/ABResources/common/config/BundleConfig.cs',NULL,NULL,NULL,NULL),(1557,49,'D','Assets/ABResources/common/config/BundleConfig.cs.meta',NULL,NULL,NULL,NULL),(1558,49,'D','Assets/ABResources/common/config/ProjectConfig.cs',NULL,NULL,NULL,NULL),(1559,49,'D','Assets/ABResources/common/config/ProjectConfig.cs.meta',NULL,NULL,NULL,NULL),(1560,49,'M','Assets/ABResources/common/template/src/LuaMonoBehaviour.lua',NULL,NULL,NULL,NULL),(1561,49,'A','Assets/ABResources/lua/lua/.lastWriteTime',NULL,NULL,NULL,NULL),(1562,49,'A','Assets/ABResources/lua/table/.lastWriteTime',NULL,NULL,NULL,NULL),(1563,49,'A','Assets/ABResources/lua/utility/.lastWriteTime',NULL,NULL,NULL,NULL),(1564,49,'M','Assets/ABResources/ui/login/.lastWriteTime',NULL,NULL,NULL,NULL),(1565,49,'A','Assets/ABResources/ui/login/login.prefab',NULL,NULL,NULL,NULL),(1566,49,'A','Assets/ABResources/ui/login/login.prefab.meta',NULL,NULL,NULL,NULL),(1567,49,'A','Assets/ABResources/ui/login/src/Login.lua',NULL,NULL,NULL,NULL),(1568,49,'A','Assets/ABResources/ui/login/src/Login.lua.meta',NULL,NULL,NULL,NULL),(1569,49,'D','Assets/ABResources/ui/login/src/LuaMonoBehaviour.lua',NULL,NULL,NULL,NULL),(1570,49,'D','Assets/ABResources/ui/login/src/LuaMonoBehaviour.lua.meta',NULL,NULL,NULL,NULL),(1571,49,'D','Assets/ABResources/ui/login/template.prefab',NULL,NULL,NULL,NULL),(1572,49,'D','Assets/ABResources/ui/login/template.prefab.meta',NULL,NULL,NULL,NULL),(1573,49,'M','Assets/BuildTools/BuildScript.cs',NULL,NULL,NULL,NULL),(1574,49,'A','Assets/Editor/TextInspector.cs',NULL,NULL,NULL,NULL),(1575,49,'A','Assets/Editor/TextInspector.cs.meta',NULL,NULL,NULL,NULL),(1576,49,'M','Assets/Scripts/AppBoot.cs',NULL,NULL,NULL,NULL),(1577,49,'M','Assets/Scripts/BundleSys.cs',NULL,NULL,NULL,NULL),(1578,49,'M','Assets/Scripts/LuaAsset.cs',NULL,NULL,NULL,NULL),(1579,49,'D','Assets/Scripts/Singleton.cs',NULL,NULL,NULL,NULL),(1580,49,'D','Assets/Scripts/Singleton.cs.meta',NULL,NULL,NULL,NULL),(1581,49,'A','Assets/Scripts/SingletonMB.cs',NULL,NULL,NULL,NULL),(1582,49,'A','Assets/Scripts/SingletonMB.cs.meta',NULL,NULL,NULL,NULL),(1583,49,'M','Assets/Scripts/UpdateSys.cs',NULL,NULL,NULL,NULL),(1584,49,'A','Assets/Scripts/config.meta',NULL,NULL,NULL,NULL),(1585,49,'A','Assets/Scripts/config/BundleConfig.cs',NULL,NULL,NULL,NULL),(1586,49,'A','Assets/Scripts/config/BundleConfig.cs.meta',NULL,NULL,NULL,NULL),(1587,49,'A','Assets/Scripts/config/ProjectConfig.cs',NULL,NULL,NULL,NULL),(1588,49,'A','Assets/Scripts/config/ProjectConfig.cs.meta',NULL,NULL,NULL,NULL),(1589,49,'M','Assets/Scripts/module/LuaSingleton.cs',NULL,NULL,NULL,NULL),(1590,50,'M','Assets/ABResources/common/config/.lastWriteTime',NULL,NULL,NULL,NULL),(1591,50,'M','Assets/ABResources/common/config/BundleConfig.asset',NULL,NULL,NULL,NULL),(1592,50,'M','Assets/ABResources/common/config/ProjectConfig.asset',NULL,NULL,NULL,NULL),(1593,50,'M','Assets/ABResources/common/template/.lastWriteTime',NULL,NULL,NULL,NULL),(1594,50,'M','Assets/ABResources/common/template/src/LuaMonoBehaviour.lua',NULL,NULL,NULL,NULL),(1595,50,'M','Assets/ABResources/ui/login/.lastWriteTime',NULL,NULL,NULL,NULL),(1596,50,'M','Assets/ABResources/ui/login/src/Login.lua',NULL,NULL,NULL,NULL),(1597,50,'M','Assets/Scene/boot.unity',NULL,NULL,NULL,NULL),(1598,50,'M','Assets/Scripts/AppBoot.cs',NULL,NULL,NULL,NULL),(1599,50,'M','Assets/Scripts/BundleSys.cs',NULL,NULL,NULL,NULL),(1600,50,'M','Assets/Scripts/LuaMonoBehaviour.cs',NULL,NULL,NULL,NULL),(1601,50,'M','Assets/Scripts/SingletonMB.cs',NULL,NULL,NULL,NULL),(1602,50,'M','Assets/Scripts/UpdateSys.cs',NULL,NULL,NULL,NULL),(1603,50,'M','Assets/Scripts/config/ProjectConfig.cs',NULL,NULL,NULL,NULL),(1604,50,'M','Assets/Scripts/module/LuaSingleton.cs',NULL,NULL,NULL,NULL),(1605,50,'M','ProjectSettings/ProjectSettings.asset',NULL,NULL,NULL,NULL),(1606,51,'D','Assets/ABResources/common/config/.lastWriteTime',NULL,NULL,NULL,NULL),(1607,51,'D','Assets/ABResources/common/template/.lastWriteTime',NULL,NULL,NULL,NULL),(1608,51,'D','Assets/ABResources/common/template/res/.lastWriteTime',NULL,NULL,NULL,NULL),(1609,51,'D','Assets/ABResources/common/template/src/.lastWriteTime',NULL,NULL,NULL,NULL),(1610,51,'D','Assets/ABResources/lua/lua/.lastWriteTime',NULL,NULL,NULL,NULL),(1611,51,'D','Assets/ABResources/lua/table/.lastWriteTime',NULL,NULL,NULL,NULL),(1612,51,'D','Assets/ABResources/lua/utility/.lastWriteTime',NULL,NULL,NULL,NULL),(1613,51,'D','Assets/ABResources/ui/login/.lastWriteTime',NULL,NULL,NULL,NULL),(1614,51,'D','Assets/ABResources/ui/login/res/.lastWriteTime',NULL,NULL,NULL,NULL),(1615,51,'D','Assets/ABResources/ui/login/src/.lastWriteTime',NULL,NULL,NULL,NULL),(1616,52,'M','.gitignore',NULL,NULL,NULL,NULL),(1617,53,'M','.gitignore',NULL,NULL,NULL,NULL),(1618,53,'D','Assets/ABResources/Lua/Table.meta',NULL,NULL,NULL,NULL),(1619,53,'D','Assets/ABResources/Lua/lua.meta',NULL,NULL,NULL,NULL),(1620,53,'D','Assets/ABResources/Lua/lua/main.lua',NULL,NULL,NULL,NULL),(1621,53,'D','Assets/ABResources/Lua/lua/main.lua.meta',NULL,NULL,NULL,NULL),(1622,53,'M','Assets/ABResources/Lua/utility/BridgingClass.lua',NULL,NULL,NULL,NULL),(1623,53,'M','Assets/ABResources/README.md',NULL,NULL,NULL,NULL),(1624,53,'M','Assets/ABResources/common/config/BundleConfig.asset',NULL,NULL,NULL,NULL),(1625,53,'D','Assets/ABResources/common/template/res.meta',NULL,NULL,NULL,NULL),(1626,53,'A','Assets/ABResources/lua/utility/init.lua',NULL,NULL,NULL,NULL),(1627,53,'A','Assets/ABResources/lua/utility/init.lua.meta',NULL,NULL,NULL,NULL),(1628,53,'A','Assets/ABResources/lua/utility/main.lua',NULL,NULL,NULL,NULL),(1629,53,'A','Assets/ABResources/lua/utility/main.lua.meta',NULL,NULL,NULL,NULL),(1630,53,'A','Assets/ABResources/lua/utility/memory.lua',NULL,NULL,NULL,NULL),(1631,53,'A','Assets/ABResources/lua/utility/memory.lua.meta',NULL,NULL,NULL,NULL),(1632,53,'A','Assets/ABResources/lua/utility/profiler.lua',NULL,NULL,NULL,NULL),(1633,53,'A','Assets/ABResources/lua/utility/profiler.lua.meta',NULL,NULL,NULL,NULL),(1634,53,'A','Assets/ABResources/lua/utility/xlua.meta',NULL,NULL,NULL,NULL),(1635,53,'A','Assets/ABResources/lua/utility/xlua/util.lua',NULL,NULL,NULL,NULL),(1636,53,'A','Assets/ABResources/lua/utility/xlua/util.lua.meta',NULL,NULL,NULL,NULL),(1637,53,'M','Assets/ABResources/ui/login/login.prefab',NULL,NULL,NULL,NULL),(1638,53,'M','Assets/ABResources/ui/login/res.meta',NULL,NULL,NULL,NULL),(1639,53,'A','Assets/ABResources/ui/login/res/fadin.anim',NULL,NULL,NULL,NULL),(1640,53,'A','Assets/ABResources/ui/login/res/fadin.anim.meta',NULL,NULL,NULL,NULL),(1641,53,'M','Assets/ABResources/ui/login/src/Login.lua',NULL,NULL,NULL,NULL),(1642,53,'A','Assets/ABResources/ui/login/src/LoginHelper.lua',NULL,NULL,NULL,NULL),(1643,53,'A','Assets/ABResources/ui/login/src/LoginHelper.lua.meta',NULL,NULL,NULL,NULL),(1644,53,'M','Assets/BuildTools/BuildScript.cs',NULL,NULL,NULL,NULL),(1645,53,'M','Assets/Scripts/AppBoot.cs',NULL,NULL,NULL,NULL),(1646,53,'A','Assets/Scripts/AssetHelper.cs',NULL,NULL,NULL,NULL),(1647,53,'A','Assets/Scripts/AssetHelper.cs.meta',NULL,NULL,NULL,NULL),(1648,53,'D','Assets/Scripts/BundleSys.cs',NULL,NULL,NULL,NULL),(1649,53,'D','Assets/Scripts/BundleSys.cs.meta',NULL,NULL,NULL,NULL),(1650,53,'M','Assets/Scripts/LuaAsset.cs',NULL,NULL,NULL,NULL),(1651,53,'M','Assets/Scripts/LuaMonoBehaviour.cs',NULL,NULL,NULL,NULL),(1652,53,'M','Assets/Scripts/SingletonMB.cs',NULL,NULL,NULL,NULL),(1653,53,'M','Assets/Scripts/UpdateSys.cs',NULL,NULL,NULL,NULL),(1654,53,'M','Assets/Scripts/config/BundleConfig.cs',NULL,NULL,NULL,NULL),(1655,53,'M','Assets/Scripts/config/ProjectConfig.cs',NULL,NULL,NULL,NULL),(1656,53,'M','Assets/Scripts/module/LuaSingleton.cs',NULL,NULL,NULL,NULL),(1657,54,'M','Assets/Scripts/AssetHelper.cs',NULL,NULL,NULL,NULL),(1658,54,'M','Assets/Scripts/LuaMonoBehaviour.cs',NULL,NULL,NULL,NULL),(1659,54,'M','Assets/Scripts/config/BundleConfig.cs',NULL,NULL,NULL,NULL),(1660,54,'M','Assets/Scripts/config/ProjectConfig.cs',NULL,NULL,NULL,NULL),(1661,54,'M','Assets/Scripts/module/LuaSingleton.cs',NULL,NULL,NULL,NULL),(1662,56,'M','Assets/ABResources/README.md',NULL,NULL,NULL,NULL),(1663,56,'M','Assets/ABResources/common/template/src/LuaMonoBehaviour.lua',NULL,NULL,NULL,NULL),(1664,56,'M','Assets/ABResources/ui/login/login.prefab',NULL,NULL,NULL,NULL),(1665,56,'M','Assets/ABResources/ui/login/src/Login.lua',NULL,NULL,NULL,NULL),(1666,56,'M','Assets/ABResources/ui/login/src/LoginHelper.lua',NULL,NULL,NULL,NULL),(1667,56,'M','Assets/ABResources/ui/login/src/LoginHelper.lua.meta',NULL,NULL,NULL,NULL),(1668,56,'M','Assets/BuildTools/BuildScript.cs',NULL,NULL,NULL,NULL),(1669,56,'D','Assets/BuildTools/BundleConfig.asset',NULL,NULL,NULL,NULL),(1670,56,'D','Assets/BuildTools/BundleConfig.asset.meta',NULL,NULL,NULL,NULL),(1671,56,'M','Assets/Plugins/AppLog.dll',NULL,NULL,NULL,NULL),(1672,56,'M','Assets/Scene/boot.unity',NULL,NULL,NULL,NULL),(1673,56,'M','Assets/Scripts/AppBoot.cs',NULL,NULL,NULL,NULL),(1674,56,'M','Assets/Scripts/AssetHelper.cs',NULL,NULL,NULL,NULL),(1675,56,'M','Assets/Scripts/LuaAsset.cs',NULL,NULL,NULL,NULL),(1676,56,'M','Assets/Scripts/LuaMonoBehaviour.cs',NULL,NULL,NULL,NULL),(1677,56,'A','Assets/Scripts/SingleMono.cs',NULL,NULL,NULL,NULL),(1678,56,'A','Assets/Scripts/SingleMono.cs.meta',NULL,NULL,NULL,NULL),(1679,56,'D','Assets/Scripts/SingletonMB.cs',NULL,NULL,NULL,NULL),(1680,56,'D','Assets/Scripts/SingletonMB.cs.meta',NULL,NULL,NULL,NULL),(1681,56,'M','Assets/Scripts/UpdateSys.cs',NULL,NULL,NULL,NULL),(1682,56,'M','Assets/Scripts/config/BundleConfig.cs',NULL,NULL,NULL,NULL),(1683,56,'A','Assets/Scripts/config/BundleConfig2.cs',NULL,NULL,NULL,NULL),(1684,56,'A','Assets/Scripts/config/BundleConfig2.cs.meta',NULL,NULL,NULL,NULL),(1685,56,'A','Assets/Scripts/module/LuaHelper.cs',NULL,NULL,NULL,NULL),(1686,56,'A','Assets/Scripts/module/LuaHelper.cs.meta',NULL,NULL,NULL,NULL),(1687,56,'D','Assets/Scripts/module/LuaSingleton.cs',NULL,NULL,NULL,NULL),(1688,56,'D','Assets/Scripts/module/LuaSingleton.cs.meta',NULL,NULL,NULL,NULL),(1689,57,'A','Assets/XLua/Gen/SystemTextEncodingWrap.cs',NULL,NULL,NULL,NULL),(1690,58,'M','Assets/ABResources/common/config/ProjectConfig.asset',NULL,NULL,NULL,NULL),(1691,58,'A','Assets/ABResources/ui/boot.meta',NULL,NULL,NULL,NULL),(1692,58,'A','Assets/ABResources/ui/boot/boot.prefab',NULL,NULL,NULL,NULL),(1693,58,'A','Assets/ABResources/ui/boot/boot.prefab.meta',NULL,NULL,NULL,NULL),(1694,58,'A','Assets/ABResources/ui/boot/src.meta',NULL,NULL,NULL,NULL),(1695,58,'A','Assets/ABResources/ui/boot/src/boot.lua',NULL,NULL,NULL,NULL),(1696,58,'A','Assets/ABResources/ui/boot/src/boot.lua.meta',NULL,NULL,NULL,NULL),(1697,58,'A','Assets/ABResources/ui/loading.meta',NULL,NULL,NULL,NULL),(1698,58,'A','Assets/ABResources/ui/loading/loading.prefab',NULL,NULL,NULL,NULL),(1699,58,'A','Assets/ABResources/ui/loading/loading.prefab.meta',NULL,NULL,NULL,NULL),(1700,58,'A','Assets/ABResources/ui/loading/res.meta',NULL,NULL,NULL,NULL),(1701,58,'A','Assets/ABResources/ui/loading/res/animation.anim',NULL,NULL,NULL,NULL),(1702,58,'A','Assets/ABResources/ui/loading/res/animation.anim.meta',NULL,NULL,NULL,NULL),(1703,58,'A','Assets/ABResources/ui/loading/src.meta',NULL,NULL,NULL,NULL),(1704,58,'A','Assets/ABResources/ui/loading/src/loading.lua',NULL,NULL,NULL,NULL),(1705,58,'A','Assets/ABResources/ui/loading/src/loading.lua.meta',NULL,NULL,NULL,NULL),(1706,58,'M','Assets/ABResources/ui/login/src/Login.lua',NULL,NULL,NULL,NULL),(1707,58,'M','Assets/BuildTools/BuildScript.cs',NULL,NULL,NULL,NULL),(1708,58,'M','Assets/Scene/boot.unity',NULL,NULL,NULL,NULL),(1709,58,'M','Assets/Scripts/AppBoot.cs',NULL,NULL,NULL,NULL),(1710,58,'M','Assets/Scripts/AssetHelper.cs',NULL,NULL,NULL,NULL),(1711,58,'M','Assets/Scripts/LuaMonoBehaviour.cs',NULL,NULL,NULL,NULL),(1712,58,'M','Assets/Scripts/SingleMono.cs',NULL,NULL,NULL,NULL),(1713,58,'M','Assets/Scripts/UpdateSys.cs',NULL,NULL,NULL,NULL),(1714,58,'M','Assets/Scripts/config/BundleConfig.cs',NULL,NULL,NULL,NULL),(1715,58,'M','Assets/XLua/Src/ObjectCasters.cs',NULL,NULL,NULL,NULL),(1716,58,'M','Assets/XLua/Src/StaticLuaCallbacks.cs',NULL,NULL,NULL,NULL),(1717,58,'M','ProjectSettings/GraphicsSettings.asset',NULL,NULL,NULL,NULL),(1718,58,'M','ProjectSettings/ProjectSettings.asset',NULL,NULL,NULL,NULL),(1719,59,'D','Assets/ABResources.meta',NULL,NULL,NULL,NULL),(1720,59,'D','Assets/ABResources/Lua.meta',NULL,NULL,NULL,NULL),(1721,59,'D','Assets/ABResources/Lua/utility.meta',NULL,NULL,NULL,NULL),(1722,59,'D','Assets/ABResources/Lua/utility/BridgingClass.lua',NULL,NULL,NULL,NULL),(1723,59,'D','Assets/ABResources/Lua/utility/BridgingClass.lua.meta',NULL,NULL,NULL,NULL),(1724,59,'D','Assets/ABResources/README.md',NULL,NULL,NULL,NULL),(1725,59,'D','Assets/ABResources/README.md.meta',NULL,NULL,NULL,NULL),(1726,59,'D','Assets/ABResources/common.meta',NULL,NULL,NULL,NULL),(1727,59,'D','Assets/ABResources/common/config.meta',NULL,NULL,NULL,NULL),(1728,59,'D','Assets/ABResources/common/config/BundleConfig.asset',NULL,NULL,NULL,NULL),(1729,59,'D','Assets/ABResources/common/config/BundleConfig.asset.meta',NULL,NULL,NULL,NULL),(1730,59,'D','Assets/ABResources/common/config/ProjectConfig.asset',NULL,NULL,NULL,NULL),(1731,59,'D','Assets/ABResources/common/config/ProjectConfig.asset.meta',NULL,NULL,NULL,NULL),(1732,59,'D','Assets/ABResources/common/template.meta',NULL,NULL,NULL,NULL),(1733,59,'D','Assets/ABResources/common/template/src.meta',NULL,NULL,NULL,NULL),(1734,59,'D','Assets/ABResources/common/template/src/LuaMonoBehaviour.lua',NULL,NULL,NULL,NULL),(1735,59,'D','Assets/ABResources/common/template/src/LuaMonoBehaviour.lua.meta',NULL,NULL,NULL,NULL),(1736,59,'D','Assets/ABResources/common/template/template.prefab',NULL,NULL,NULL,NULL),(1737,59,'D','Assets/ABResources/common/template/template.prefab.meta',NULL,NULL,NULL,NULL),(1738,59,'D','Assets/ABResources/lua/utility/init.lua',NULL,NULL,NULL,NULL),(1739,59,'D','Assets/ABResources/lua/utility/init.lua.meta',NULL,NULL,NULL,NULL),(1740,59,'D','Assets/ABResources/lua/utility/main.lua',NULL,NULL,NULL,NULL),(1741,59,'D','Assets/ABResources/lua/utility/main.lua.meta',NULL,NULL,NULL,NULL),(1742,59,'D','Assets/ABResources/lua/utility/memory.lua',NULL,NULL,NULL,NULL),(1743,59,'D','Assets/ABResources/lua/utility/memory.lua.meta',NULL,NULL,NULL,NULL),(1744,59,'D','Assets/ABResources/lua/utility/profiler.lua',NULL,NULL,NULL,NULL),(1745,59,'D','Assets/ABResources/lua/utility/profiler.lua.meta',NULL,NULL,NULL,NULL),(1746,59,'D','Assets/ABResources/lua/utility/xlua.meta',NULL,NULL,NULL,NULL),(1747,59,'D','Assets/ABResources/lua/utility/xlua/util.lua',NULL,NULL,NULL,NULL),(1748,59,'D','Assets/ABResources/lua/utility/xlua/util.lua.meta',NULL,NULL,NULL,NULL),(1749,59,'D','Assets/ABResources/ui.meta',NULL,NULL,NULL,NULL),(1750,59,'D','Assets/ABResources/ui/boot.meta',NULL,NULL,NULL,NULL),(1751,59,'D','Assets/ABResources/ui/boot/boot.prefab',NULL,NULL,NULL,NULL),(1752,59,'D','Assets/ABResources/ui/boot/boot.prefab.meta',NULL,NULL,NULL,NULL),(1753,59,'D','Assets/ABResources/ui/boot/src.meta',NULL,NULL,NULL,NULL),(1754,59,'D','Assets/ABResources/ui/boot/src/boot.lua',NULL,NULL,NULL,NULL),(1755,59,'D','Assets/ABResources/ui/boot/src/boot.lua.meta',NULL,NULL,NULL,NULL),(1756,59,'D','Assets/ABResources/ui/loading.meta',NULL,NULL,NULL,NULL),(1757,59,'D','Assets/ABResources/ui/loading/loading.prefab',NULL,NULL,NULL,NULL),(1758,59,'D','Assets/ABResources/ui/loading/loading.prefab.meta',NULL,NULL,NULL,NULL),(1759,59,'D','Assets/ABResources/ui/loading/res.meta',NULL,NULL,NULL,NULL),(1760,59,'D','Assets/ABResources/ui/loading/res/animation.anim',NULL,NULL,NULL,NULL),(1761,59,'D','Assets/ABResources/ui/loading/res/animation.anim.meta',NULL,NULL,NULL,NULL),(1762,59,'D','Assets/ABResources/ui/loading/src.meta',NULL,NULL,NULL,NULL),(1763,59,'D','Assets/ABResources/ui/loading/src/loading.lua',NULL,NULL,NULL,NULL),(1764,59,'D','Assets/ABResources/ui/loading/src/loading.lua.meta',NULL,NULL,NULL,NULL),(1765,59,'D','Assets/ABResources/ui/login.meta',NULL,NULL,NULL,NULL),(1766,59,'D','Assets/ABResources/ui/login/login.prefab',NULL,NULL,NULL,NULL),(1767,59,'D','Assets/ABResources/ui/login/login.prefab.meta',NULL,NULL,NULL,NULL),(1768,59,'D','Assets/ABResources/ui/login/res.meta',NULL,NULL,NULL,NULL),(1769,59,'D','Assets/ABResources/ui/login/res/fadin.anim',NULL,NULL,NULL,NULL),(1770,59,'D','Assets/ABResources/ui/login/res/fadin.anim.meta',NULL,NULL,NULL,NULL),(1771,59,'D','Assets/ABResources/ui/login/src.meta',NULL,NULL,NULL,NULL),(1772,59,'D','Assets/ABResources/ui/login/src/Login.lua',NULL,NULL,NULL,NULL),(1773,59,'D','Assets/ABResources/ui/login/src/Login.lua.meta',NULL,NULL,NULL,NULL),(1774,59,'D','Assets/ABResources/ui/login/src/LoginHelper.lua',NULL,NULL,NULL,NULL),(1775,59,'D','Assets/ABResources/ui/login/src/LoginHelper.lua.meta',NULL,NULL,NULL,NULL),(1776,59,'M','Assets/BuildTools/BuildScript.cs',NULL,NULL,NULL,NULL),(1777,59,'A','Assets/BundleRes.meta',NULL,NULL,NULL,NULL),(1778,59,'A','Assets/BundleRes/Lua.meta',NULL,NULL,NULL,NULL),(1779,59,'A','Assets/BundleRes/Lua/utility.meta',NULL,NULL,NULL,NULL),(1780,59,'A','Assets/BundleRes/Lua/utility/BridgingClass.lua',NULL,NULL,NULL,NULL),(1781,59,'A','Assets/BundleRes/Lua/utility/BridgingClass.lua.meta',NULL,NULL,NULL,NULL),(1782,59,'A','Assets/BundleRes/Lua/utility/init.lua',NULL,NULL,NULL,NULL),(1783,59,'A','Assets/BundleRes/Lua/utility/init.lua.meta',NULL,NULL,NULL,NULL),(1784,59,'A','Assets/BundleRes/Lua/utility/main.lua',NULL,NULL,NULL,NULL),(1785,59,'A','Assets/BundleRes/Lua/utility/main.lua.meta',NULL,NULL,NULL,NULL),(1786,59,'A','Assets/BundleRes/Lua/utility/memory.lua',NULL,NULL,NULL,NULL),(1787,59,'A','Assets/BundleRes/Lua/utility/memory.lua.meta',NULL,NULL,NULL,NULL),(1788,59,'A','Assets/BundleRes/Lua/utility/profiler.lua',NULL,NULL,NULL,NULL),(1789,59,'A','Assets/BundleRes/Lua/utility/profiler.lua.meta',NULL,NULL,NULL,NULL),(1790,59,'A','Assets/BundleRes/Lua/utility/xlua.meta',NULL,NULL,NULL,NULL),(1791,59,'A','Assets/BundleRes/Lua/utility/xlua/util.lua',NULL,NULL,NULL,NULL),(1792,59,'A','Assets/BundleRes/Lua/utility/xlua/util.lua.meta',NULL,NULL,NULL,NULL),(1793,59,'A','Assets/BundleRes/README.md',NULL,NULL,NULL,NULL),(1794,59,'A','Assets/BundleRes/README.md.meta',NULL,NULL,NULL,NULL),(1795,59,'A','Assets/BundleRes/common.meta',NULL,NULL,NULL,NULL),(1796,59,'A','Assets/BundleRes/common/config.meta',NULL,NULL,NULL,NULL),(1797,59,'A','Assets/BundleRes/common/config/BundleConfig.asset',NULL,NULL,NULL,NULL),(1798,59,'A','Assets/BundleRes/common/config/BundleConfig.asset.meta',NULL,NULL,NULL,NULL),(1799,59,'A','Assets/BundleRes/common/config/ProjectConfig.asset',NULL,NULL,NULL,NULL),(1800,59,'A','Assets/BundleRes/common/config/ProjectConfig.asset.meta',NULL,NULL,NULL,NULL),(1801,59,'A','Assets/BundleRes/common/template.meta',NULL,NULL,NULL,NULL),(1802,59,'A','Assets/BundleRes/common/template/src.meta',NULL,NULL,NULL,NULL),(1803,59,'A','Assets/BundleRes/common/template/src/LuaMonoBehaviour.lua.txt',NULL,NULL,NULL,NULL),(1804,59,'A','Assets/BundleRes/common/template/src/LuaMonoBehaviour.lua.txt.meta',NULL,NULL,NULL,NULL),(1805,59,'A','Assets/BundleRes/common/template/template.prefab',NULL,NULL,NULL,NULL),(1806,59,'A','Assets/BundleRes/common/template/template.prefab.meta',NULL,NULL,NULL,NULL),(1807,59,'A','Assets/BundleRes/ui.meta',NULL,NULL,NULL,NULL),(1808,59,'A','Assets/BundleRes/ui/boot.meta',NULL,NULL,NULL,NULL),(1809,59,'A','Assets/BundleRes/ui/boot/boot.prefab',NULL,NULL,NULL,NULL),(1810,59,'A','Assets/BundleRes/ui/boot/boot.prefab.meta',NULL,NULL,NULL,NULL),(1811,59,'A','Assets/BundleRes/ui/boot/src.meta',NULL,NULL,NULL,NULL),(1812,59,'A','Assets/BundleRes/ui/boot/src/boot.lua',NULL,NULL,NULL,NULL),(1813,59,'A','Assets/BundleRes/ui/boot/src/boot.lua.meta',NULL,NULL,NULL,NULL),(1814,59,'A','Assets/BundleRes/ui/loading.meta',NULL,NULL,NULL,NULL),(1815,59,'A','Assets/BundleRes/ui/loading/loading.prefab',NULL,NULL,NULL,NULL),(1816,59,'A','Assets/BundleRes/ui/loading/loading.prefab.meta',NULL,NULL,NULL,NULL),(1817,59,'A','Assets/BundleRes/ui/loading/res.meta',NULL,NULL,NULL,NULL),(1818,59,'A','Assets/BundleRes/ui/loading/res/animation.anim',NULL,NULL,NULL,NULL),(1819,59,'A','Assets/BundleRes/ui/loading/res/animation.anim.meta',NULL,NULL,NULL,NULL),(1820,59,'A','Assets/BundleRes/ui/loading/src.meta',NULL,NULL,NULL,NULL),(1821,59,'A','Assets/BundleRes/ui/loading/src/loading.lua',NULL,NULL,NULL,NULL),(1822,59,'A','Assets/BundleRes/ui/loading/src/loading.lua.meta',NULL,NULL,NULL,NULL),(1823,59,'A','Assets/BundleRes/ui/login.meta',NULL,NULL,NULL,NULL),(1824,59,'A','Assets/BundleRes/ui/login/login.prefab',NULL,NULL,NULL,NULL),(1825,59,'A','Assets/BundleRes/ui/login/login.prefab.meta',NULL,NULL,NULL,NULL),(1826,59,'A','Assets/BundleRes/ui/login/res.meta',NULL,NULL,NULL,NULL),(1827,59,'A','Assets/BundleRes/ui/login/res/fadin.anim',NULL,NULL,NULL,NULL),(1828,59,'A','Assets/BundleRes/ui/login/res/fadin.anim.meta',NULL,NULL,NULL,NULL),(1829,59,'A','Assets/BundleRes/ui/login/src.meta',NULL,NULL,NULL,NULL),(1830,59,'A','Assets/BundleRes/ui/login/src/Login.lua',NULL,NULL,NULL,NULL),(1831,59,'A','Assets/BundleRes/ui/login/src/Login.lua.meta',NULL,NULL,NULL,NULL),(1832,59,'A','Assets/BundleRes/ui/login/src/LoginHelper.lua',NULL,NULL,NULL,NULL),(1833,59,'A','Assets/BundleRes/ui/login/src/LoginHelper.lua.meta',NULL,NULL,NULL,NULL),(1834,59,'M','Assets/Scripts/AssetHelper.cs',NULL,NULL,NULL,NULL),(1835,59,'M','Assets/Scripts/LuaAsset.cs',NULL,NULL,NULL,NULL),(1836,59,'M','Assets/Scripts/config/BundleConfig.cs',NULL,NULL,NULL,NULL),(1837,59,'M','Assets/Scripts/config/BundleConfig2.cs',NULL,NULL,NULL,NULL),(1838,59,'M','Assets/Scripts/config/ProjectConfig.cs',NULL,NULL,NULL,NULL),(1839,59,'M','Assets/Scripts/module/LuaHelper.cs',NULL,NULL,NULL,NULL),(1840,59,'M','ProjectSettings/GraphicsSettings.asset',NULL,NULL,NULL,NULL),(1841,60,'M','Assets/BuildTools/BuildScript.cs',NULL,NULL,NULL,NULL),(1842,60,'M','Assets/Scripts/config/BundleConfig.cs',NULL,NULL,NULL,NULL),(1843,60,'D','Assets/Scripts/config/BundleConfig2.cs',NULL,NULL,NULL,NULL),(1844,60,'D','Assets/Scripts/config/BundleConfig2.cs.meta',NULL,NULL,NULL,NULL),(1845,61,'M','Assets/Scripts/AssetHelper.cs',NULL,NULL,NULL,NULL),(1846,62,'M','Assets/Plugins/Yaml/net35/YamlDotNet.dll',NULL,NULL,NULL,NULL),(1847,62,'M','Assets/Plugins/Yaml/net35/YamlDotNet.xml',NULL,NULL,NULL,NULL),(1848,63,'M','Assets/BuildTools/BuildScript.cs',NULL,NULL,NULL,NULL),(1849,63,'A','Assets/Scripts/App.cs',NULL,NULL,NULL,NULL),(1850,63,'A','Assets/Scripts/App.cs.meta',NULL,NULL,NULL,NULL),(1851,63,'D','Assets/Scripts/AppBoot.cs',NULL,NULL,NULL,NULL),(1852,63,'D','Assets/Scripts/AppBoot.cs.meta',NULL,NULL,NULL,NULL),(1853,63,'D','Assets/Scripts/AssetHelper.cs',NULL,NULL,NULL,NULL),(1854,63,'D','Assets/Scripts/AssetHelper.cs.meta',NULL,NULL,NULL,NULL),(1855,63,'A','Assets/Scripts/AssetSys.cs',NULL,NULL,NULL,NULL),(1856,63,'A','Assets/Scripts/AssetSys.cs.meta',NULL,NULL,NULL,NULL),(1857,63,'M','Assets/Scripts/LuaMonoBehaviour.cs',NULL,NULL,NULL,NULL),(1858,63,'M','Assets/Scripts/UpdateSys.cs',NULL,NULL,NULL,NULL),(1859,63,'M','Assets/Scripts/YamlHelper.cs',NULL,NULL,NULL,NULL),(1860,63,'M','Assets/Scripts/config/BundleConfig.cs',NULL,NULL,NULL,NULL),(1861,63,'M','Assets/Scripts/config/ProjectConfig.cs',NULL,NULL,NULL,NULL),(1862,63,'D','Assets/Scripts/module/LuaHelper.cs',NULL,NULL,NULL,NULL),(1863,63,'D','Assets/Scripts/module/LuaHelper.cs.meta',NULL,NULL,NULL,NULL),(1864,63,'A','Assets/Scripts/module/LuaSys.cs',NULL,NULL,NULL,NULL),(1865,63,'A','Assets/Scripts/module/LuaSys.cs.meta',NULL,NULL,NULL,NULL),(1866,64,'M','Assets/XLua/Src/CodeEmit.cs',NULL,NULL,NULL,NULL),(1867,64,'M','Assets/XLua/Src/Editor/Generator.cs',NULL,NULL,NULL,NULL),(1868,64,'M','Assets/XLua/Src/Editor/Hotfix.cs',NULL,NULL,NULL,NULL),(1869,64,'M','Assets/XLua/Src/LuaEnv.cs',NULL,NULL,NULL,NULL),(1870,64,'M','Assets/XLua/Src/MethodWarpsCache.cs',NULL,NULL,NULL,NULL),(1871,64,'M','Assets/XLua/Src/StaticLuaCallbacks.cs',NULL,NULL,NULL,NULL),(1872,64,'M','Assets/XLua/Src/TemplateEngine/TemplateEngine.cs',NULL,NULL,NULL,NULL),(1873,64,'M','Assets/XLua/Src/Utils.cs',NULL,NULL,NULL,NULL),(1874,65,'M','Assets/Plugins/AppLog.dll',NULL,NULL,NULL,NULL),(1875,66,'M','.gitmodules',NULL,NULL,NULL,NULL),(1876,66,'D','Assets/Plugins/.YamlDotNet',NULL,NULL,NULL,NULL),(1877,66,'A','tools/YamlDotNet',NULL,NULL,NULL,NULL),(1878,67,'A','Assets/BundleRes/scene.meta',NULL,NULL,NULL,NULL),(1879,67,'A','Assets/BundleRes/scene/boot.unity',NULL,NULL,NULL,NULL),(1880,67,'A','Assets/BundleRes/scene/boot.unity.meta',NULL,NULL,NULL,NULL),(1881,67,'D','Assets/Scene.meta',NULL,NULL,NULL,NULL),(1882,67,'D','Assets/Scene/boot.unity',NULL,NULL,NULL,NULL),(1883,67,'D','Assets/Scene/boot.unity.meta',NULL,NULL,NULL,NULL),(1884,68,'D','Assets/BundleRes/Lua/utility.meta',NULL,NULL,NULL,NULL),(1885,68,'D','Assets/BundleRes/Lua/utility/BridgingClass.lua',NULL,NULL,NULL,NULL),(1886,68,'D','Assets/BundleRes/Lua/utility/BridgingClass.lua.meta',NULL,NULL,NULL,NULL),(1887,68,'D','Assets/BundleRes/Lua/utility/init.lua',NULL,NULL,NULL,NULL),(1888,68,'D','Assets/BundleRes/Lua/utility/init.lua.meta',NULL,NULL,NULL,NULL),(1889,68,'D','Assets/BundleRes/Lua/utility/main.lua',NULL,NULL,NULL,NULL),(1890,68,'D','Assets/BundleRes/Lua/utility/main.lua.meta',NULL,NULL,NULL,NULL),(1891,68,'D','Assets/BundleRes/Lua/utility/memory.lua',NULL,NULL,NULL,NULL),(1892,68,'D','Assets/BundleRes/Lua/utility/memory.lua.meta',NULL,NULL,NULL,NULL),(1893,68,'D','Assets/BundleRes/Lua/utility/profiler.lua',NULL,NULL,NULL,NULL),(1894,68,'D','Assets/BundleRes/Lua/utility/profiler.lua.meta',NULL,NULL,NULL,NULL),(1895,68,'D','Assets/BundleRes/Lua/utility/xlua.meta',NULL,NULL,NULL,NULL),(1896,68,'D','Assets/BundleRes/Lua/utility/xlua/util.lua',NULL,NULL,NULL,NULL),(1897,68,'D','Assets/BundleRes/Lua/utility/xlua/util.lua.meta',NULL,NULL,NULL,NULL),(1898,68,'A','Assets/BundleRes/lua/utility.meta',NULL,NULL,NULL,NULL),(1899,68,'A','Assets/BundleRes/lua/utility/BridgingClass.lua',NULL,NULL,NULL,NULL),(1900,68,'A','Assets/BundleRes/lua/utility/BridgingClass.lua.meta',NULL,NULL,NULL,NULL),(1901,68,'A','Assets/BundleRes/lua/utility/init.lua',NULL,NULL,NULL,NULL),(1902,68,'A','Assets/BundleRes/lua/utility/init.lua.meta',NULL,NULL,NULL,NULL),(1903,68,'A','Assets/BundleRes/lua/utility/main.lua',NULL,NULL,NULL,NULL),(1904,68,'A','Assets/BundleRes/lua/utility/main.lua.meta',NULL,NULL,NULL,NULL),(1905,68,'A','Assets/BundleRes/lua/utility/memory.lua',NULL,NULL,NULL,NULL),(1906,68,'A','Assets/BundleRes/lua/utility/memory.lua.meta',NULL,NULL,NULL,NULL),(1907,68,'A','Assets/BundleRes/lua/utility/profiler.lua',NULL,NULL,NULL,NULL),(1908,68,'A','Assets/BundleRes/lua/utility/profiler.lua.meta',NULL,NULL,NULL,NULL),(1909,68,'A','Assets/BundleRes/lua/utility/xlua.meta',NULL,NULL,NULL,NULL),(1910,68,'A','Assets/BundleRes/lua/utility/xlua/util.lua',NULL,NULL,NULL,NULL),(1911,68,'A','Assets/BundleRes/lua/utility/xlua/util.lua.meta',NULL,NULL,NULL,NULL),(1912,69,'M','Assets/BuildTools/BuildScript.cs',NULL,NULL,NULL,NULL),(1913,69,'M','Assets/BundleRes/README.md',NULL,NULL,NULL,NULL),(1914,69,'A','Assets/BundleRes/chapter_01.meta',NULL,NULL,NULL,NULL),(1915,69,'A','Assets/BundleRes/chapter_01/level_01_01.meta',NULL,NULL,NULL,NULL),(1916,69,'M','Assets/BundleRes/common/config/BundleConfig.asset',NULL,NULL,NULL,NULL),(1917,69,'M','Assets/BundleRes/common/config/BundleConfig.asset.meta',NULL,NULL,NULL,NULL),(1918,69,'A','Assets/BundleRes/common/template/res.meta',NULL,NULL,NULL,NULL),(1919,69,'A','Assets/BundleRes/common/template/src/LuaMonoBehaviour.lua',NULL,NULL,NULL,NULL),(1920,69,'A','Assets/BundleRes/common/template/src/LuaMonoBehaviour.lua.meta',NULL,NULL,NULL,NULL),(1921,69,'D','Assets/BundleRes/common/template/src/LuaMonoBehaviour.lua.txt',NULL,NULL,NULL,NULL),(1922,69,'D','Assets/BundleRes/common/template/src/LuaMonoBehaviour.lua.txt.meta',NULL,NULL,NULL,NULL),(1923,69,'A','Assets/BundleRes/common/ui.meta',NULL,NULL,NULL,NULL),(1924,69,'A','Assets/BundleRes/common/ui/diamond.png',NULL,NULL,NULL,NULL),(1925,69,'A','Assets/BundleRes/common/ui/diamond.png.meta',NULL,NULL,NULL,NULL),(1926,69,'M','Assets/BundleRes/ui/boot/boot.prefab',NULL,NULL,NULL,NULL),(1927,69,'M','Assets/BundleRes/ui/boot/src/boot.lua',NULL,NULL,NULL,NULL),(1928,69,'M','Assets/BundleRes/ui/login/login.prefab',NULL,NULL,NULL,NULL),(1929,69,'D','Assets/BundleRes/ui/login/src/LoginHelper.lua',NULL,NULL,NULL,NULL),(1930,69,'D','Assets/BundleRes/ui/login/src/LoginHelper.lua.meta',NULL,NULL,NULL,NULL),(1931,69,'A','Assets/BundleRes/ui/login/src/login_helper.lua',NULL,NULL,NULL,NULL),(1932,69,'A','Assets/BundleRes/ui/login/src/login_helper.lua.meta',NULL,NULL,NULL,NULL),(1933,69,'A','Assets/Editor/XLuaGenConfig.cs',NULL,NULL,NULL,NULL),(1934,69,'A','Assets/Editor/XLuaGenConfig.cs.meta',NULL,NULL,NULL,NULL),(1935,69,'M','Assets/Plugins/Lzma/Compress/LZMA/LzmaEncoder.cs',NULL,NULL,NULL,NULL),(1936,69,'M','Assets/Plugins/Lzma/ICoder.cs',NULL,NULL,NULL,NULL),(1937,69,'M','Assets/Plugins/Yaml/net35/YamlDotNet.dll',NULL,NULL,NULL,NULL),(1938,69,'M','Assets/Scripts/App.cs',NULL,NULL,NULL,NULL),(1939,69,'M','Assets/Scripts/AssetSys.cs',NULL,NULL,NULL,NULL),(1940,69,'M','Assets/Scripts/LuaAsset.cs',NULL,NULL,NULL,NULL),(1941,69,'M','Assets/Scripts/LuaMonoBehaviour.cs',NULL,NULL,NULL,NULL),(1942,69,'M','Assets/Scripts/UpdateSys.cs',NULL,NULL,NULL,NULL),(1943,69,'M','Assets/Scripts/config/BundleConfig.cs',NULL,NULL,NULL,NULL),(1944,69,'M','Assets/Scripts/config/ProjectConfig.cs',NULL,NULL,NULL,NULL),(1945,69,'M','Assets/Scripts/module/LuaSys.cs',NULL,NULL,NULL,NULL),(1946,69,'A','Assets/XLua/Gen.meta',NULL,NULL,NULL,NULL),(1947,69,'M','ProjectSettings/ProjectSettings.asset',NULL,NULL,NULL,NULL),(1948,69,'M','doc/todo.md',NULL,NULL,NULL,NULL),(1949,69,'M','unity_template.sublime-project',NULL,NULL,NULL,NULL),(1950,70,'M','Assets/BuildTools/BuildScript.cs',NULL,NULL,NULL,NULL),(1951,72,'A','Assets/XLua/Gen/AppLogWrap.cs',NULL,NULL,NULL,NULL),(1952,72,'A','Assets/XLua/Gen/AppLogWrap.cs.meta',NULL,NULL,NULL,NULL),(1953,72,'A','Assets/XLua/Gen/AssetSysWrap.cs',NULL,NULL,NULL,NULL),(1954,72,'A','Assets/XLua/Gen/AssetSysWrap.cs.meta',NULL,NULL,NULL,NULL),(1955,72,'A','Assets/XLua/Gen/DelegatesGensBridge.cs',NULL,NULL,NULL,NULL),(1956,72,'A','Assets/XLua/Gen/DelegatesGensBridge.cs.meta',NULL,NULL,NULL,NULL),(1957,72,'A','Assets/XLua/Gen/EnumWrap.cs',NULL,NULL,NULL,NULL),(1958,72,'A','Assets/XLua/Gen/EnumWrap.cs.meta',NULL,NULL,NULL,NULL),(1959,72,'A','Assets/XLua/Gen/LuaMonoBehaviourWrap.cs',NULL,NULL,NULL,NULL),(1960,72,'A','Assets/XLua/Gen/LuaMonoBehaviourWrap.cs.meta',NULL,NULL,NULL,NULL),(1961,72,'A','Assets/XLua/Gen/PackUnpack.cs',NULL,NULL,NULL,NULL),(1962,72,'A','Assets/XLua/Gen/PackUnpack.cs.meta',NULL,NULL,NULL,NULL),(1963,72,'A','Assets/XLua/Gen/SystemCollectionsGenericList1SystemInt32Wrap.cs',NULL,NULL,NULL,NULL),(1964,72,'A','Assets/XLua/Gen/SystemCollectionsGenericList1SystemInt32Wrap.cs.meta',NULL,NULL,NULL,NULL),(1965,72,'A','Assets/XLua/Gen/SystemCollectionsIEnumeratorBridge.cs',NULL,NULL,NULL,NULL),(1966,72,'A','Assets/XLua/Gen/SystemCollectionsIEnumeratorBridge.cs.meta',NULL,NULL,NULL,NULL),(1967,72,'A','Assets/XLua/Gen/SystemObjectWrap.cs',NULL,NULL,NULL,NULL),(1968,72,'A','Assets/XLua/Gen/SystemObjectWrap.cs.meta',NULL,NULL,NULL,NULL),(1969,72,'A','Assets/XLua/Gen/SystemTextEncodingWrap.cs.meta',NULL,NULL,NULL,NULL),(1970,72,'A','Assets/XLua/Gen/UnityEngineAnimationClipWrap.cs',NULL,NULL,NULL,NULL),(1971,72,'A','Assets/XLua/Gen/UnityEngineAnimationClipWrap.cs.meta',NULL,NULL,NULL,NULL),(1972,72,'A','Assets/XLua/Gen/UnityEngineAnimationCurveWrap.cs',NULL,NULL,NULL,NULL),(1973,72,'A','Assets/XLua/Gen/UnityEngineAnimationCurveWrap.cs.meta',NULL,NULL,NULL,NULL),(1974,72,'A','Assets/XLua/Gen/UnityEngineBehaviourWrap.cs',NULL,NULL,NULL,NULL),(1975,72,'A','Assets/XLua/Gen/UnityEngineBehaviourWrap.cs.meta',NULL,NULL,NULL,NULL),(1976,72,'A','Assets/XLua/Gen/UnityEngineBoundsWrap.cs',NULL,NULL,NULL,NULL),(1977,72,'A','Assets/XLua/Gen/UnityEngineBoundsWrap.cs.meta',NULL,NULL,NULL,NULL),(1978,72,'A','Assets/XLua/Gen/UnityEngineColorWrap.cs',NULL,NULL,NULL,NULL),(1979,72,'A','Assets/XLua/Gen/UnityEngineColorWrap.cs.meta',NULL,NULL,NULL,NULL),(1980,72,'A','Assets/XLua/Gen/UnityEngineComponentWrap.cs',NULL,NULL,NULL,NULL),(1981,72,'A','Assets/XLua/Gen/UnityEngineComponentWrap.cs.meta',NULL,NULL,NULL,NULL),(1982,72,'A','Assets/XLua/Gen/UnityEngineDebugWrap.cs',NULL,NULL,NULL,NULL),(1983,72,'A','Assets/XLua/Gen/UnityEngineDebugWrap.cs.meta',NULL,NULL,NULL,NULL),(1984,72,'A','Assets/XLua/Gen/UnityEngineGameObjectWrap.cs',NULL,NULL,NULL,NULL),(1985,72,'A','Assets/XLua/Gen/UnityEngineGameObjectWrap.cs.meta',NULL,NULL,NULL,NULL),(1986,72,'A','Assets/XLua/Gen/UnityEngineKeyframeWrap.cs',NULL,NULL,NULL,NULL),(1987,72,'A','Assets/XLua/Gen/UnityEngineKeyframeWrap.cs.meta',NULL,NULL,NULL,NULL),(1988,72,'A','Assets/XLua/Gen/UnityEngineMonoBehaviourWrap.cs',NULL,NULL,NULL,NULL),(1989,72,'A','Assets/XLua/Gen/UnityEngineMonoBehaviourWrap.cs.meta',NULL,NULL,NULL,NULL),(1990,72,'A','Assets/XLua/Gen/UnityEngineObjectWrap.cs',NULL,NULL,NULL,NULL),(1991,72,'A','Assets/XLua/Gen/UnityEngineObjectWrap.cs.meta',NULL,NULL,NULL,NULL),(1992,72,'A','Assets/XLua/Gen/UnityEngineParticleSystemWrap.cs',NULL,NULL,NULL,NULL),(1993,72,'A','Assets/XLua/Gen/UnityEngineParticleSystemWrap.cs.meta',NULL,NULL,NULL,NULL),(1994,72,'A','Assets/XLua/Gen/UnityEngineQuaternionWrap.cs',NULL,NULL,NULL,NULL),(1995,72,'A','Assets/XLua/Gen/UnityEngineQuaternionWrap.cs.meta',NULL,NULL,NULL,NULL),(1996,72,'A','Assets/XLua/Gen/UnityEngineRay2DWrap.cs',NULL,NULL,NULL,NULL),(1997,72,'A','Assets/XLua/Gen/UnityEngineRay2DWrap.cs.meta',NULL,NULL,NULL,NULL),(1998,72,'A','Assets/XLua/Gen/UnityEngineRayWrap.cs',NULL,NULL,NULL,NULL),(1999,72,'A','Assets/XLua/Gen/UnityEngineRayWrap.cs.meta',NULL,NULL,NULL,NULL),(2000,72,'A','Assets/XLua/Gen/UnityEngineRendererWrap.cs',NULL,NULL,NULL,NULL),(2001,72,'A','Assets/XLua/Gen/UnityEngineRendererWrap.cs.meta',NULL,NULL,NULL,NULL),(2002,72,'A','Assets/XLua/Gen/UnityEngineResourcesWrap.cs',NULL,NULL,NULL,NULL),(2003,72,'A','Assets/XLua/Gen/UnityEngineResourcesWrap.cs.meta',NULL,NULL,NULL,NULL),(2004,72,'A','Assets/XLua/Gen/UnityEngineSkinnedMeshRendererWrap.cs',NULL,NULL,NULL,NULL),(2005,72,'A','Assets/XLua/Gen/UnityEngineSkinnedMeshRendererWrap.cs.meta',NULL,NULL,NULL,NULL),(2006,72,'A','Assets/XLua/Gen/UnityEngineTextAssetWrap.cs',NULL,NULL,NULL,NULL),(2007,72,'A','Assets/XLua/Gen/UnityEngineTextAssetWrap.cs.meta',NULL,NULL,NULL,NULL),(2008,72,'A','Assets/XLua/Gen/UnityEngineTimeWrap.cs',NULL,NULL,NULL,NULL),(2009,72,'A','Assets/XLua/Gen/UnityEngineTimeWrap.cs.meta',NULL,NULL,NULL,NULL),(2010,72,'A','Assets/XLua/Gen/UnityEngineTransformWrap.cs',NULL,NULL,NULL,NULL),(2011,72,'A','Assets/XLua/Gen/UnityEngineTransformWrap.cs.meta',NULL,NULL,NULL,NULL),(2012,72,'A','Assets/XLua/Gen/UnityEngineVector2Wrap.cs',NULL,NULL,NULL,NULL),(2013,72,'A','Assets/XLua/Gen/UnityEngineVector2Wrap.cs.meta',NULL,NULL,NULL,NULL),(2014,72,'A','Assets/XLua/Gen/UnityEngineVector3Wrap.cs',NULL,NULL,NULL,NULL),(2015,72,'A','Assets/XLua/Gen/UnityEngineVector3Wrap.cs.meta',NULL,NULL,NULL,NULL),(2016,72,'A','Assets/XLua/Gen/UnityEngineVector4Wrap.cs',NULL,NULL,NULL,NULL),(2017,72,'A','Assets/XLua/Gen/UnityEngineVector4Wrap.cs.meta',NULL,NULL,NULL,NULL),(2018,72,'A','Assets/XLua/Gen/UnityEngineWWWWrap.cs',NULL,NULL,NULL,NULL),(2019,72,'A','Assets/XLua/Gen/UnityEngineWWWWrap.cs.meta',NULL,NULL,NULL,NULL),(2020,72,'A','Assets/XLua/Gen/UpdateSysWrap.cs',NULL,NULL,NULL,NULL),(2021,72,'A','Assets/XLua/Gen/UpdateSysWrap.cs.meta',NULL,NULL,NULL,NULL),(2022,72,'A','Assets/XLua/Gen/WrapPusher.cs',NULL,NULL,NULL,NULL),(2023,72,'A','Assets/XLua/Gen/WrapPusher.cs.meta',NULL,NULL,NULL,NULL),(2024,72,'A','Assets/XLua/Gen/XLuaGenAutoRegister.cs',NULL,NULL,NULL,NULL),(2025,72,'A','Assets/XLua/Gen/XLuaGenAutoRegister.cs.meta',NULL,NULL,NULL,NULL),(2026,72,'A','Assets/XLua/Gen/link.xml',NULL,NULL,NULL,NULL),(2027,72,'A','Assets/XLua/Gen/link.xml.meta',NULL,NULL,NULL,NULL),(2028,73,'M','Assets/XLua/Gen.meta',NULL,NULL,NULL,NULL),(2029,74,'D','Assets/BundleRes/chapter_01/level_01_01.meta',NULL,NULL,NULL,NULL),(2030,74,'D','Assets/BundleRes/common/template/res.meta',NULL,NULL,NULL,NULL),(2031,74,'D','Assets/Plugins/Lzma/Compress/LzmaAlone/LzmaAlone.csproj.meta',NULL,NULL,NULL,NULL),(2032,74,'D','Assets/Plugins/Lzma/Compress/LzmaAlone/LzmaAlone.sln.meta',NULL,NULL,NULL,NULL),(2033,74,'D','Assets/Plugins/Yaml/net35/YamlDotNet.pdb.meta',NULL,NULL,NULL,NULL),(2034,75,'M','Assets/Scripts/config/BundleConfig.cs',NULL,NULL,NULL,NULL),(2035,76,'M','tools/YamlDotNet',NULL,NULL,NULL,NULL),(2036,77,'A','Assets/Plugins/Android/AndroidManifest.xml',NULL,NULL,NULL,NULL),(2037,77,'A','Assets/Plugins/Android/AndroidManifest.xml.meta',NULL,NULL,NULL,NULL),(2038,78,'M','Assets/BuildTools/BuildScript.cs',NULL,NULL,NULL,NULL),(2039,78,'D','Assets/BundleRes/chapter_01.meta',NULL,NULL,NULL,NULL),(2040,78,'M','Assets/BundleRes/common/config/BundleConfig.asset',NULL,NULL,NULL,NULL),(2041,78,'M','Assets/BundleRes/common/config/ProjectConfig.asset',NULL,NULL,NULL,NULL),(2042,78,'M','Assets/Scripts/AssetSys.cs',NULL,NULL,NULL,NULL),(2043,78,'M','Assets/Scripts/config/BundleConfig.cs',NULL,NULL,NULL,NULL),(2044,78,'M','ProjectSettings/GraphicsSettings.asset',NULL,NULL,NULL,NULL),(2045,78,'M','ProjectSettings/ProjectSettings.asset',NULL,NULL,NULL,NULL),(2046,78,'M','ProjectSettings/ProjectVersion.txt',NULL,NULL,NULL,NULL),(2047,79,'M','Assets/BundleRes/common/config/BundleConfig.asset',NULL,NULL,NULL,NULL),(2048,79,'M','Assets/Plugins/Android/AndroidManifest.xml',NULL,NULL,NULL,NULL),(2049,79,'M','Assets/Scripts/AssetSys.cs',NULL,NULL,NULL,NULL),(2050,79,'M','Assets/Scripts/UpdateSys.cs',NULL,NULL,NULL,NULL),(2051,79,'M','Assets/Scripts/config/BundleConfig.cs',NULL,NULL,NULL,NULL),(2052,79,'M','ProjectSettings/ProjectSettings.asset',NULL,NULL,NULL,NULL),(2053,82,'M','Assets/BuildTools/BuildScript.cs',NULL,NULL,NULL,NULL),(2054,82,'M','Assets/BundleRes/common/config/BundleConfig.asset',NULL,NULL,NULL,NULL),(2055,82,'M','Assets/BundleRes/common/config/BundleConfig.asset.meta',NULL,NULL,NULL,NULL),(2056,82,'D','Assets/BundleRes/common/config/ProjectConfig.asset',NULL,NULL,NULL,NULL),(2057,82,'D','Assets/BundleRes/common/config/ProjectConfig.asset.meta',NULL,NULL,NULL,NULL),(2058,82,'M','Assets/BundleRes/scene/boot.unity',NULL,NULL,NULL,NULL),(2059,82,'M','Assets/BundleRes/ui/loading/loading.prefab',NULL,NULL,NULL,NULL),(2060,82,'M','Assets/Scripts/App.cs',NULL,NULL,NULL,NULL),(2061,82,'M','Assets/Scripts/AssetSys.cs',NULL,NULL,NULL,NULL),(2062,82,'M','Assets/Scripts/LuaAsset.cs',NULL,NULL,NULL,NULL),(2063,82,'M','Assets/Scripts/LuaMonoBehaviour.cs',NULL,NULL,NULL,NULL),(2064,82,'M','Assets/Scripts/UpdateSys.cs',NULL,NULL,NULL,NULL),(2065,82,'M','Assets/Scripts/config/BundleConfig.cs',NULL,NULL,NULL,NULL),(2066,82,'D','Assets/Scripts/config/ProjectConfig.cs',NULL,NULL,NULL,NULL),(2067,82,'D','Assets/Scripts/config/ProjectConfig.cs.meta',NULL,NULL,NULL,NULL),(2068,82,'M','Assets/Scripts/module/LuaSys.cs',NULL,NULL,NULL,NULL),(2069,82,'M','Assets/XLua/Gen/AssetSysWrap.cs',NULL,NULL,NULL,NULL),(2070,82,'M','Assets/XLua/Gen/UpdateSysWrap.cs',NULL,NULL,NULL,NULL),(2071,82,'M','ProjectSettings/GraphicsSettings.asset',NULL,NULL,NULL,NULL),(2072,82,'M','ProjectSettings/ProjectSettings.asset',NULL,NULL,NULL,NULL),(2073,82,'M','ProjectSettings/ProjectVersion.txt',NULL,NULL,NULL,NULL),(2074,83,'M','Assets/BuildTools/BuildScript.cs',NULL,NULL,NULL,NULL),(2075,83,'M','Assets/BundleRes/common/config/BundleConfig.asset',NULL,NULL,NULL,NULL),(2076,83,'M','Assets/BundleRes/ui/boot/src/boot.lua',NULL,NULL,NULL,NULL),(2077,83,'M','Assets/Scripts/AssetSys.cs',NULL,NULL,NULL,NULL),(2078,83,'M','Assets/Scripts/UpdateSys.cs',NULL,NULL,NULL,NULL),(2079,83,'M','Assets/Scripts/config/BundleConfig.cs',NULL,NULL,NULL,NULL),(2080,83,'M','Assets/XLua/Gen/UpdateSysWrap.cs',NULL,NULL,NULL,NULL),(2081,83,'M','ProjectSettings/ProjectSettings.asset',NULL,NULL,NULL,NULL),(2082,84,'M','Assets/BuildTools/BuildScript.cs',NULL,NULL,NULL,NULL),(2083,84,'M','Assets/BundleRes/common/config/BundleConfig.asset',NULL,NULL,NULL,NULL),(2084,84,'M','Assets/BundleRes/common/config/BundleConfig.asset.meta',NULL,NULL,NULL,NULL),(2085,84,'M','Assets/BundleRes/scene/boot.unity',NULL,NULL,NULL,NULL),(2086,84,'M','Assets/BundleRes/ui/boot/src/boot.lua',NULL,NULL,NULL,NULL),(2087,84,'M','Assets/BundleRes/ui/loading/loading.prefab',NULL,NULL,NULL,NULL),(2088,84,'M','Assets/BundleRes/ui/login/src/Login.lua',NULL,NULL,NULL,NULL),(2089,84,'M','Assets/Scripts/App.cs',NULL,NULL,NULL,NULL),(2090,84,'M','Assets/Scripts/AssetSys.cs',NULL,NULL,NULL,NULL),(2091,84,'M','Assets/Scripts/UpdateSys.cs',NULL,NULL,NULL,NULL),(2092,84,'M','Assets/Scripts/YamlHelper.cs',NULL,NULL,NULL,NULL),(2093,84,'M','Assets/Scripts/config/BundleConfig.cs',NULL,NULL,NULL,NULL),(2094,84,'M','ProjectSettings/ProjectSettings.asset',NULL,NULL,NULL,NULL),(2095,84,'M','ProjectSettings/QualitySettings.asset',NULL,NULL,NULL,NULL),(2096,84,'M','unity_template.sublime-workspace',NULL,NULL,NULL,NULL),(2097,85,'M','Assets/BuildTools/BuildScript.cs',NULL,NULL,NULL,NULL),(2098,85,'M','Assets/BundleRes/common/config/BundleConfig.asset',NULL,NULL,NULL,NULL),(2099,86,'A','Assets/BundleRes/chapter_01.meta',NULL,NULL,NULL,NULL),(2100,86,'A','Assets/BundleRes/chapter_01/level_01_01.meta',NULL,NULL,NULL,NULL),(2101,86,'M','Assets/BundleRes/common/config/BundleConfig.asset',NULL,NULL,NULL,NULL),(2102,86,'A','Assets/BundleRes/common/template/res.meta',NULL,NULL,NULL,NULL),(2103,86,'A','Assets/BundleRes/ui/loading/res/cube.mat',NULL,NULL,NULL,NULL),(2104,86,'A','Assets/BundleRes/ui/loading/res/cube.mat.meta',NULL,NULL,NULL,NULL);
/*!40000 ALTER TABLE `changes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `changeset_parents`
--

DROP TABLE IF EXISTS `changeset_parents`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `changeset_parents` (
  `changeset_id` int(11) NOT NULL,
  `parent_id` int(11) NOT NULL,
  KEY `changeset_parents_changeset_ids` (`changeset_id`),
  KEY `changeset_parents_parent_ids` (`parent_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `changeset_parents`
--

LOCK TABLES `changeset_parents` WRITE;
/*!40000 ALTER TABLE `changeset_parents` DISABLE KEYS */;
INSERT INTO `changeset_parents` VALUES (45,44),(46,45),(47,46),(48,47),(49,48),(50,49),(51,50),(52,51),(53,51),(54,53),(55,54),(55,52),(56,55),(57,56),(58,57),(59,58),(60,59),(61,60),(62,61),(63,62),(64,63),(65,64),(66,65),(67,66),(68,67),(69,67),(70,68),(71,70),(71,69),(72,71),(73,72),(74,73),(75,74),(76,75),(77,75),(78,77),(79,78),(80,76),(80,78),(81,80),(81,79),(82,81),(83,82),(84,83),(85,84),(86,85);
/*!40000 ALTER TABLE `changeset_parents` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `changesets`
--

DROP TABLE IF EXISTS `changesets`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `changesets` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `repository_id` int(11) NOT NULL,
  `revision` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `committer` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `committed_on` datetime NOT NULL,
  `comments` longtext COLLATE utf8_unicode_ci,
  `commit_date` date DEFAULT NULL,
  `scmid` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `user_id` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `changesets_repos_rev` (`repository_id`,`revision`),
  KEY `index_changesets_on_user_id` (`user_id`),
  KEY `index_changesets_on_repository_id` (`repository_id`),
  KEY `index_changesets_on_committed_on` (`committed_on`),
  KEY `changesets_repos_scmid` (`repository_id`,`scmid`)
) ENGINE=InnoDB AUTO_INCREMENT=87 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `changesets`
--

LOCK TABLES `changesets` WRITE;
/*!40000 ALTER TABLE `changesets` DISABLE KEYS */;
INSERT INTO `changesets` VALUES (44,8,'62c51222db25cfba8326fac61717d7e572a8867a','cn <cool_navy@qq.com>','2018-02-07 04:00:12','Initial commit','2018-02-07','62c51222db25cfba8326fac61717d7e572a8867a',11),(45,8,'0479975749194bc370aaa78e94da52e944a9481d','cn <cool_navy@126.com>','2018-02-07 04:42:13','add xlua','2018-02-07','0479975749194bc370aaa78e94da52e944a9481d',11),(46,8,'b9df2b8ca6a37fedfddd6fd9335894159e78ef94','cn <cool_navy@126.com>','2018-02-07 14:04:47','add tools: yaml excel2lua','2018-02-07','b9df2b8ca6a37fedfddd6fd9335894159e78ef94',11),(47,8,'f79902b5e5ee77e97db5a1284e3a19ce3be8ab51','cn <cool_navy@126.com>','2018-02-10 15:44:40','add bundlesys luahelper','2018-02-10','f79902b5e5ee77e97db5a1284e3a19ce3be8ab51',11),(48,8,'e5c65a436d52f9f3fe0cd8f70917527cb740f74b','cn <cool_navy@126.com>','2018-03-18 09:27:45','lua 1.0','2018-03-18','e5c65a436d52f9f3fe0cd8f70917527cb740f74b',11),(49,8,'7f4ac6bfa109fbdae2cef389a0b869b64501d8fe','cn <cool_navy@126.com>','2018-03-18 10:53:08','add lua 1.0','2018-03-18','7f4ac6bfa109fbdae2cef389a0b869b64501d8fe',11),(50,8,'3d1013a92520882a4fd3f4d0ca1706926c1e37c5','cn <cool_navy@126.com>','2018-03-18 12:54:13','fixed load lua from bundle','2018-03-18','3d1013a92520882a4fd3f4d0ca1706926c1e37c5',11),(51,8,'ea0057e2de3864c81e1f9552ac5d836ea21e003c','cn <cool_navy@126.com>','2018-03-18 12:55:36','rm .lastWriteTime','2018-03-18','ea0057e2de3864c81e1f9552ac5d836ea21e003c',11),(52,8,'f984ff351897fc9f82eb637bc33a5486cb9bc5af','cn <cool_navy@126.com>','2018-03-18 12:56:42','fixed ignore','2018-03-18','f984ff351897fc9f82eb637bc33a5486cb9bc5af',11),(53,8,'45b2c111d339826f4f38bb8eb502a2f33a9df35e','cn <cool_navy@126.com>','2018-03-19 14:24:17','fixed ignore','2018-03-19','45b2c111d339826f4f38bb8eb502a2f33a9df35e',11),(54,8,'8633de34c6df5fc91c214fdb3025007fa06fde4d','cn <cool_navy@126.com>','2018-03-19 14:24:18','load lua from bundle & file ok','2018-03-19','8633de34c6df5fc91c214fdb3025007fa06fde4d',11),(55,8,'e7fc5712aed0035e68a0bee5d566cffe0a75f977','cn <cool_navy@126.com>','2018-03-19 14:25:16','Merge branch \'master\' of github.com:cn00/unity_template','2018-03-19','e7fc5712aed0035e68a0bee5d566cffe0a75f977',11),(56,8,'5310d970d78cda9749993ef875e6fce5e370e1d2','cn <cool_navy@126.com>','2018-03-21 14:21:46','load .lua .lua.txt from file or bundle prefect in editor modle','2018-03-21','5310d970d78cda9749993ef875e6fce5e370e1d2',11),(57,8,'3738e52531f11ca13f4bd27f171f0d10d39c8ab5','cn <cool_navy@126.com>','2018-03-22 06:46:31','fixed XLua/Gen/SystemTextEncodingWrap.cs','2018-03-22','3738e52531f11ca13f4bd27f171f0d10d39c8ab5',11),(58,8,'901a55079fa172c11439aa959feee36390c08821','cn <cool_navy@126.com>','2018-03-22 08:03:56','add lua c# Coroutine ok','2018-03-22','901a55079fa172c11439aa959feee36390c08821',11),(59,8,'05946d1b355bb15d16424b7878e42a01da7bed15','cn <cool_navy@126.com>','2018-03-22 08:42:26','rename','2018-03-22','05946d1b355bb15d16424b7878e42a01da7bed15',11),(60,8,'45d9145360b4912af347f6e52899de7688f4446f','cn <cool_navy@126.com>','2018-03-22 11:57:42','bundle run an android ok','2018-03-22','45d9145360b4912af347f6e52899de7688f4446f',11),(61,8,'f782e9a9383194b8854272d3f59bf5de2c4e1c4c','cn <cool_navy@126.com>','2018-03-22 12:05:01','only save compressed file in editor moddle','2018-03-22','f782e9a9383194b8854272d3f59bf5de2c4e1c4c',11),(62,8,'457fd362ff3abc4dbf7f33cff598de59232fbb77','cn <cool_navy@126.com>','2018-03-23 08:28:35','fixed yaml for android YamlDotNet.git@7ee5a2a','2018-03-23','457fd362ff3abc4dbf7f33cff598de59232fbb77',11),(63,8,'518a8e32d92d31d59669b187effb8e0f20cf920d','cn <cool_navy@126.com>','2018-03-23 08:30:19','rename','2018-03-23','518a8e32d92d31d59669b187effb8e0f20cf920d',11),(64,8,'878565f76c727d62d41b2f011e0de80d0b5ff0da','cn <cool_navy@126.com>','2018-03-23 08:31:22','use AppLog','2018-03-23','878565f76c727d62d41b2f011e0de80d0b5ff0da',11),(65,8,'bf0936334fa0a3758ffadd544286e8b12167163e','cn <cool_navy@126.com>','2018-03-23 08:32:06','use AppLog','2018-03-23','bf0936334fa0a3758ffadd544286e8b12167163e',11),(66,8,'8201a5c94ccbddff2e90578f870f205fd71cc6f8','cn <cool_navy@126.com>','2018-03-23 12:39:26','move YamlDotNet','2018-03-23','8201a5c94ccbddff2e90578f870f205fd71cc6f8',11),(67,8,'59fd4c1d200d393f1739b397fad9fa65957b81c2','cn <cool_navy@126.com>','2018-03-26 11:03:08','rename scene','2018-03-26','59fd4c1d200d393f1739b397fad9fa65957b81c2',11),(68,8,'ae8ffb86e60db2fa957bb64c6c6e9e9d3fae0039','cn <cool_navy@126.com>','2018-03-28 08:40:54','rename Lua => lua','2018-03-28','ae8ffb86e60db2fa957bb64c6c6e9e9d3fae0039',11),(69,8,'68ac6e279dc7b7a14e3efe0743645e4874800057','cn <cool_navy@126.com>','2018-03-29 02:43:36','update package pack','2018-03-29','68ac6e279dc7b7a14e3efe0743645e4874800057',11),(70,8,'372a0c57e736a69bfd98c3f181b0325f5d1d2b92','cn <cool_navy@126.com>','2018-03-29 02:47:27','add iOS pack method','2018-03-29','372a0c57e736a69bfd98c3f181b0325f5d1d2b92',11),(71,8,'6c156aea202ccd221de95eadb22d9280f2fac72e','cn <cool_navy@126.com>','2018-03-29 02:49:50','Merge branch \'master\' of 10.23.114.141:/cygdrive/d/unity_test','2018-03-29','6c156aea202ccd221de95eadb22d9280f2fac72e',11),(72,8,'41a2a76787bfef2b5dad24d11b6124ba338da935','cn <cool_navy@126.com>','2018-03-29 02:51:58','add Assets/XLua/Gen','2018-03-29','41a2a76787bfef2b5dad24d11b6124ba338da935',11),(73,8,'d80ea29b1959732e23c358a58a1aae2df4b167d2','cn <cool_navy@126.com>','2018-03-29 02:52:27','add Assets/XLua/Gen','2018-03-29','d80ea29b1959732e23c358a58a1aae2df4b167d2',11),(74,8,'bf3f3573429bd82895481c801eb0284e4049c8f5','cn <cool_navy@126.com>','2018-03-29 02:54:48','clean meta','2018-03-29','bf3f3573429bd82895481c801eb0284e4049c8f5',11),(75,8,'66e823e474439dd8d0f5270b324232775348f4d1','cn <cool_navy@qq.com>','2018-04-01 09:57:13','fixed build bundle','2018-04-01','66e823e474439dd8d0f5270b324232775348f4d1',11),(76,8,'ee9633fffc51fbf100e3fcbc8d14fa934cf1e735','cn <cool_navy@126.com>','2018-04-01 10:27:08','update submodule tools/YamlDotNet','2018-04-01','ee9633fffc51fbf100e3fcbc8d14fa934cf1e735',11),(77,8,'ab9ed7853dff93a929fad4e07ee73fd4499cc567','cn <cool_navy@qq.com>','2018-04-01 14:20:35','Assets/Plugins/Android/AndroidManifest.xml','2018-04-01','ab9ed7853dff93a929fad4e07ee73fd4499cc567',11),(78,8,'6df1f50c760ba356bbea74aa10832981a33b8398','cn <cool_navy@qq.com>','2018-04-01 15:09:37','update bundle','2018-04-01','6df1f50c760ba356bbea74aa10832981a33b8398',11),(79,8,'a3e3bc2e8423c7494bf9b8192fe5ec53b1e248d4','cn <cool_navy@qq.com>','2018-04-02 03:34:57','update config','2018-04-02','a3e3bc2e8423c7494bf9b8192fe5ec53b1e248d4',11),(80,8,'a10c214c5de9318c41582181acd4b0583aab4787','cn <cool_navy@126.com>','2018-04-02 03:38:30','Merge branch \'master\' of github.com:cn00/unity_template','2018-04-02','a10c214c5de9318c41582181acd4b0583aab4787',11),(81,8,'8cbdf3058bf828c14df92030ad6575bc32933bc5','cn <cool_navy@126.com>','2018-04-02 03:39:41','Merge branch \'master\' of github.com:cn00/unity_template','2018-04-02','8cbdf3058bf828c14df92030ad6575bc32933bc5',11),(82,8,'0c6a4caebf96d476f354b62595bb84a754d26408','cn <cool_navy@126.com>','2018-04-03 06:02:26','update bundle group build','2018-04-03','0c6a4caebf96d476f354b62595bb84a754d26408',11),(83,8,'aa89d91b22cd99b78122427cb582cf99cb9e2f85','cn <cool_navy@126.com>','2018-04-03 06:38:46','fixed Fun name','2018-04-03','aa89d91b22cd99b78122427cb582cf99cb9e2f85',11),(84,8,'67da96b9671af0e51992fda0aa644989a2749037','cn <cool_navy@126.com>','2018-04-04 16:05:28','update hotfix ok.','2018-04-05','67da96b9671af0e51992fda0aa644989a2749037',11),(85,8,'3f732230f22dd9cae1f517d584582702f661d721','cn <cool_navy@126.com>','2018-04-04 16:14:43','fixed bundle increment build','2018-04-05','3f732230f22dd9cae1f517d584582702f661d721',11),(86,8,'256762a9691e2d05b9a0b241117a14007227114b','cn <cool_navy@126.com>','2018-04-05 04:55:35','add missing files','2018-04-05','256762a9691e2d05b9a0b241117a14007227114b',11);
/*!40000 ALTER TABLE `changesets` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `changesets_issues`
--

DROP TABLE IF EXISTS `changesets_issues`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `changesets_issues` (
  `changeset_id` int(11) NOT NULL,
  `issue_id` int(11) NOT NULL,
  UNIQUE KEY `changesets_issues_ids` (`changeset_id`,`issue_id`),
  KEY `index_changesets_issues_on_issue_id` (`issue_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `changesets_issues`
--

LOCK TABLES `changesets_issues` WRITE;
/*!40000 ALTER TABLE `changesets_issues` DISABLE KEYS */;
/*!40000 ALTER TABLE `changesets_issues` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `comments`
--

DROP TABLE IF EXISTS `comments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `comments` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `commented_type` varchar(30) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `commented_id` int(11) NOT NULL DEFAULT '0',
  `author_id` int(11) NOT NULL DEFAULT '0',
  `comments` text COLLATE utf8_unicode_ci,
  `created_on` datetime NOT NULL,
  `updated_on` datetime NOT NULL,
  PRIMARY KEY (`id`),
  KEY `index_comments_on_commented_id_and_commented_type` (`commented_id`,`commented_type`),
  KEY `index_comments_on_author_id` (`author_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `comments`
--

LOCK TABLES `comments` WRITE;
/*!40000 ALTER TABLE `comments` DISABLE KEYS */;
/*!40000 ALTER TABLE `comments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `custom_field_enumerations`
--

DROP TABLE IF EXISTS `custom_field_enumerations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `custom_field_enumerations` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `custom_field_id` int(11) NOT NULL,
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `active` tinyint(1) NOT NULL DEFAULT '1',
  `position` int(11) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `custom_field_enumerations`
--

LOCK TABLES `custom_field_enumerations` WRITE;
/*!40000 ALTER TABLE `custom_field_enumerations` DISABLE KEYS */;
/*!40000 ALTER TABLE `custom_field_enumerations` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `custom_fields`
--

DROP TABLE IF EXISTS `custom_fields`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `custom_fields` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `type` varchar(30) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `name` varchar(30) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `field_format` varchar(30) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `possible_values` text COLLATE utf8_unicode_ci,
  `regexp` varchar(255) COLLATE utf8_unicode_ci DEFAULT '',
  `min_length` int(11) DEFAULT NULL,
  `max_length` int(11) DEFAULT NULL,
  `is_required` tinyint(1) NOT NULL DEFAULT '0',
  `is_for_all` tinyint(1) NOT NULL DEFAULT '0',
  `is_filter` tinyint(1) NOT NULL DEFAULT '0',
  `position` int(11) DEFAULT NULL,
  `searchable` tinyint(1) DEFAULT '0',
  `default_value` text COLLATE utf8_unicode_ci,
  `editable` tinyint(1) DEFAULT '1',
  `visible` tinyint(1) NOT NULL DEFAULT '1',
  `multiple` tinyint(1) DEFAULT '0',
  `format_store` text COLLATE utf8_unicode_ci,
  `description` text COLLATE utf8_unicode_ci,
  PRIMARY KEY (`id`),
  KEY `index_custom_fields_on_id_and_type` (`id`,`type`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `custom_fields`
--

LOCK TABLES `custom_fields` WRITE;
/*!40000 ALTER TABLE `custom_fields` DISABLE KEYS */;
/*!40000 ALTER TABLE `custom_fields` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `custom_fields_projects`
--

DROP TABLE IF EXISTS `custom_fields_projects`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `custom_fields_projects` (
  `custom_field_id` int(11) NOT NULL DEFAULT '0',
  `project_id` int(11) NOT NULL DEFAULT '0',
  UNIQUE KEY `index_custom_fields_projects_on_custom_field_id_and_project_id` (`custom_field_id`,`project_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `custom_fields_projects`
--

LOCK TABLES `custom_fields_projects` WRITE;
/*!40000 ALTER TABLE `custom_fields_projects` DISABLE KEYS */;
/*!40000 ALTER TABLE `custom_fields_projects` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `custom_fields_roles`
--

DROP TABLE IF EXISTS `custom_fields_roles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `custom_fields_roles` (
  `custom_field_id` int(11) NOT NULL,
  `role_id` int(11) NOT NULL,
  UNIQUE KEY `custom_fields_roles_ids` (`custom_field_id`,`role_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `custom_fields_roles`
--

LOCK TABLES `custom_fields_roles` WRITE;
/*!40000 ALTER TABLE `custom_fields_roles` DISABLE KEYS */;
/*!40000 ALTER TABLE `custom_fields_roles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `custom_fields_trackers`
--

DROP TABLE IF EXISTS `custom_fields_trackers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `custom_fields_trackers` (
  `custom_field_id` int(11) NOT NULL DEFAULT '0',
  `tracker_id` int(11) NOT NULL DEFAULT '0',
  UNIQUE KEY `index_custom_fields_trackers_on_custom_field_id_and_tracker_id` (`custom_field_id`,`tracker_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `custom_fields_trackers`
--

LOCK TABLES `custom_fields_trackers` WRITE;
/*!40000 ALTER TABLE `custom_fields_trackers` DISABLE KEYS */;
/*!40000 ALTER TABLE `custom_fields_trackers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `custom_values`
--

DROP TABLE IF EXISTS `custom_values`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `custom_values` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `customized_type` varchar(30) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `customized_id` int(11) NOT NULL DEFAULT '0',
  `custom_field_id` int(11) NOT NULL DEFAULT '0',
  `value` text COLLATE utf8_unicode_ci,
  PRIMARY KEY (`id`),
  KEY `custom_values_customized` (`customized_type`,`customized_id`),
  KEY `index_custom_values_on_custom_field_id` (`custom_field_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `custom_values`
--

LOCK TABLES `custom_values` WRITE;
/*!40000 ALTER TABLE `custom_values` DISABLE KEYS */;
/*!40000 ALTER TABLE `custom_values` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `documents`
--

DROP TABLE IF EXISTS `documents`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `documents` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `project_id` int(11) NOT NULL DEFAULT '0',
  `category_id` int(11) NOT NULL DEFAULT '0',
  `title` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `description` text COLLATE utf8_unicode_ci,
  `created_on` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `documents_project_id` (`project_id`),
  KEY `index_documents_on_category_id` (`category_id`),
  KEY `index_documents_on_created_on` (`created_on`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `documents`
--

LOCK TABLES `documents` WRITE;
/*!40000 ALTER TABLE `documents` DISABLE KEYS */;
/*!40000 ALTER TABLE `documents` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `email_addresses`
--

DROP TABLE IF EXISTS `email_addresses`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `email_addresses` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `user_id` int(11) NOT NULL,
  `address` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `is_default` tinyint(1) NOT NULL DEFAULT '0',
  `notify` tinyint(1) NOT NULL DEFAULT '1',
  `created_on` datetime NOT NULL,
  `updated_on` datetime NOT NULL,
  PRIMARY KEY (`id`),
  KEY `index_email_addresses_on_user_id` (`user_id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `email_addresses`
--

LOCK TABLES `email_addresses` WRITE;
/*!40000 ALTER TABLE `email_addresses` DISABLE KEYS */;
INSERT INTO `email_addresses` VALUES (1,1,'admin@example.net',1,1,'2018-04-05 06:32:43','2018-04-05 06:32:43'),(2,11,'lenghaijun@bilibili.com',1,1,'2018-04-05 06:40:30','2018-04-05 06:40:30');
/*!40000 ALTER TABLE `email_addresses` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `enabled_modules`
--

DROP TABLE IF EXISTS `enabled_modules`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `enabled_modules` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `project_id` int(11) DEFAULT NULL,
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  PRIMARY KEY (`id`),
  KEY `enabled_modules_project_id` (`project_id`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `enabled_modules`
--

LOCK TABLES `enabled_modules` WRITE;
/*!40000 ALTER TABLE `enabled_modules` DISABLE KEYS */;
INSERT INTO `enabled_modules` VALUES (1,1,'issue_tracking'),(2,1,'time_tracking'),(3,1,'news'),(4,1,'documents'),(5,1,'files'),(6,1,'wiki'),(7,1,'repository'),(8,1,'boards'),(9,1,'calendar'),(10,1,'gantt');
/*!40000 ALTER TABLE `enabled_modules` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `enumerations`
--

DROP TABLE IF EXISTS `enumerations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `enumerations` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(30) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `position` int(11) DEFAULT NULL,
  `is_default` tinyint(1) NOT NULL DEFAULT '0',
  `type` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `active` tinyint(1) NOT NULL DEFAULT '1',
  `project_id` int(11) DEFAULT NULL,
  `parent_id` int(11) DEFAULT NULL,
  `position_name` varchar(30) COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `index_enumerations_on_project_id` (`project_id`),
  KEY `index_enumerations_on_id_and_type` (`id`,`type`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `enumerations`
--

LOCK TABLES `enumerations` WRITE;
/*!40000 ALTER TABLE `enumerations` DISABLE KEYS */;
INSERT INTO `enumerations` VALUES (1,'一般',1,1,'IssuePriority',1,NULL,NULL,'default'),(2,'紧急',2,0,'IssuePriority',1,NULL,NULL,'highest');
/*!40000 ALTER TABLE `enumerations` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `groups_users`
--

DROP TABLE IF EXISTS `groups_users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `groups_users` (
  `group_id` int(11) NOT NULL,
  `user_id` int(11) NOT NULL,
  UNIQUE KEY `groups_users_ids` (`group_id`,`user_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `groups_users`
--

LOCK TABLES `groups_users` WRITE;
/*!40000 ALTER TABLE `groups_users` DISABLE KEYS */;
INSERT INTO `groups_users` VALUES (5,1),(5,11);
/*!40000 ALTER TABLE `groups_users` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `import_items`
--

DROP TABLE IF EXISTS `import_items`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `import_items` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `import_id` int(11) NOT NULL,
  `position` int(11) NOT NULL,
  `obj_id` int(11) DEFAULT NULL,
  `message` text COLLATE utf8_unicode_ci,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `import_items`
--

LOCK TABLES `import_items` WRITE;
/*!40000 ALTER TABLE `import_items` DISABLE KEYS */;
INSERT INTO `import_items` VALUES (1,1,1,NULL,'主题 不能为空字符\n项目 不能为空字符\n跟踪 不能为空字符\n状态 不能为空字符'),(2,2,1,NULL,'项目 不能为空字符\n跟踪 不能为空字符\n状态 不能为空字符'),(3,3,1,NULL,'项目 不能为空字符\n跟踪 不能为空字符\n状态 不能为空字符'),(4,4,1,NULL,'项目 不能为空字符\n跟踪 不能为空字符\n状态 不能为空字符'),(5,5,1,NULL,'主题 不能为空字符\n项目 不能为空字符\n跟踪 不能为空字符\n状态 不能为空字符'),(6,6,1,NULL,'项目 不能为空字符\n跟踪 不能为空字符\n状态 不能为空字符');
/*!40000 ALTER TABLE `import_items` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `imports`
--

DROP TABLE IF EXISTS `imports`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `imports` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `type` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `user_id` int(11) NOT NULL,
  `filename` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `settings` text COLLATE utf8_unicode_ci,
  `total_items` int(11) DEFAULT NULL,
  `finished` tinyint(1) NOT NULL DEFAULT '0',
  `created_at` datetime NOT NULL,
  `updated_at` datetime NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `imports`
--

LOCK TABLES `imports` WRITE;
/*!40000 ALTER TABLE `imports` DISABLE KEYS */;
INSERT INTO `imports` VALUES (1,'IssueImport',11,'8d22a875a2e0c6fd9519c3eae819b2e3','---\nseparator: \";\"\nwrapper: \"\\\"\"\nencoding: GBK\ndate_format: \"%Y-%m-%d\"\nmapping: !ruby/hash:ActiveSupport::HashWithIndifferentAccess\n  project_id: \'1\'\n  tracker: \'\'\n  status: \'\'\n  subject: \'\'\n  description: \'\'\n  priority: \'\'\n  category: \'\'\n  assigned_to: \'\'\n  fixed_version: \'\'\n  is_private: \'\'\n  parent_issue_id: \'\'\n  start_date: \'\'\n  due_date: \'\'\n  estimated_hours: \'\'\n  done_ratio: \'\'\n',1,1,'2018-04-05 08:07:35','2018-04-05 08:07:57'),(2,'IssueImport',11,'d4a43c5eef60524b44796825ead9f426','---\nseparator: \";\"\nwrapper: \"\\\"\"\nencoding: GBK\ndate_format: \"%Y-%m-%d\"\nmapping: !ruby/hash:ActiveSupport::HashWithIndifferentAccess\n  project_id: \'1\'\n  tracker: \'\'\n  status: \'\'\n  subject: \'6\'\n  description: \'26\'\n  priority: \'5\'\n  category: \'10\'\n  assigned_to: \'8\'\n  fixed_version: \'11\'\n  is_private: \'\'\n  parent_issue_id: \'\'\n  start_date: \'\'\n  due_date: \'\'\n  estimated_hours: \'\'\n  done_ratio: \'\'\n',1,1,'2018-04-05 08:08:22','2018-04-05 08:09:34'),(3,'IssueImport',11,'ee7308acc3fac07050c158d9b11d47a2','---\nseparator: \";\"\nwrapper: \"\\\"\"\nencoding: GBK\ndate_format: \"%Y-%m-%d\"\nmapping: !ruby/hash:ActiveSupport::HashWithIndifferentAccess\n  project_id: \'1\'\n  tracker: \'2\'\n  status: \'\'\n  subject: \'6\'\n  description: \'\'\n  priority: \'\'\n  category: \'\'\n  assigned_to: \'\'\n  fixed_version: \'\'\n  is_private: \'\'\n  parent_issue_id: \'\'\n  start_date: \'\'\n  due_date: \'\'\n  estimated_hours: \'\'\n  done_ratio: \'\'\n',1,1,'2018-04-05 08:09:59','2018-04-05 08:10:25'),(4,'IssueImport',11,'58fede3579ea4900f5d3c4c06f9101fa','---\nseparator: \";\"\nwrapper: \"\\\"\"\nencoding: GBK\ndate_format: \"%Y-%m-%d\"\nmapping: !ruby/hash:ActiveSupport::HashWithIndifferentAccess\n  project_id: \'1\'\n  tracker: \'2\'\n  status: \'4\'\n  subject: \'6\'\n  description: \'26\'\n  priority: \'5\'\n  category: \'10\'\n  create_categories: \'1\'\n  assigned_to: \'8\'\n  fixed_version: \'11\'\n  create_versions: \'1\'\n  is_private: \'\'\n  parent_issue_id: \'\'\n  start_date: \'\'\n  due_date: \'\'\n  estimated_hours: \'\'\n  done_ratio: \'\'\n',1,1,'2018-04-05 08:12:05','2018-04-05 08:13:11'),(5,'IssueImport',11,'445365eebafffbfc896bf60cc93987cc','---\nseparator: \";\"\nwrapper: \"\\\"\"\nencoding: GBK\ndate_format: \"%Y-%m-%d\"\nmapping: !ruby/hash:ActiveSupport::HashWithIndifferentAccess\n  project_id: \'1\'\n  tracker: \'\'\n  status: \'\'\n  subject: \'\'\n  description: \'\'\n  priority: \'\'\n  category: \'\'\n  assigned_to: \'\'\n  fixed_version: \'\'\n  is_private: \'\'\n  parent_issue_id: \'\'\n  start_date: \'\'\n  due_date: \'\'\n  estimated_hours: \'\'\n  done_ratio: \'\'\n',1,1,'2018-04-05 08:14:09','2018-04-05 08:14:20'),(6,'IssueImport',11,'42dcce41a499809cd27c64faab17eb06','---\nseparator: \";\"\nwrapper: \"\\\"\"\nencoding: GBK\ndate_format: \"%Y-%m-%d\"\nmapping: !ruby/hash:ActiveSupport::HashWithIndifferentAccess\n  project_id: \'1\'\n  tracker: \'2\'\n  status: \'4\'\n  subject: \'6\'\n  description: \'26\'\n  priority: \'\'\n  category: \'\'\n  assigned_to: \'\'\n  fixed_version: \'\'\n  is_private: \'\'\n  parent_issue_id: \'\'\n  start_date: \'\'\n  due_date: \'\'\n  estimated_hours: \'\'\n  done_ratio: \'\'\n',1,1,'2018-04-05 08:15:19','2018-04-05 08:15:54');
/*!40000 ALTER TABLE `imports` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `issue_categories`
--

DROP TABLE IF EXISTS `issue_categories`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `issue_categories` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `project_id` int(11) NOT NULL DEFAULT '0',
  `name` varchar(60) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `assigned_to_id` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `issue_categories_project_id` (`project_id`),
  KEY `index_issue_categories_on_assigned_to_id` (`assigned_to_id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `issue_categories`
--

LOCK TABLES `issue_categories` WRITE;
/*!40000 ALTER TABLE `issue_categories` DISABLE KEYS */;
INSERT INTO `issue_categories` VALUES (5,1,'新功能',NULL),(6,1,'老问题',NULL);
/*!40000 ALTER TABLE `issue_categories` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `issue_relations`
--

DROP TABLE IF EXISTS `issue_relations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `issue_relations` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `issue_from_id` int(11) NOT NULL,
  `issue_to_id` int(11) NOT NULL,
  `relation_type` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `delay` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `index_issue_relations_on_issue_from_id_and_issue_to_id` (`issue_from_id`,`issue_to_id`),
  KEY `index_issue_relations_on_issue_from_id` (`issue_from_id`),
  KEY `index_issue_relations_on_issue_to_id` (`issue_to_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `issue_relations`
--

LOCK TABLES `issue_relations` WRITE;
/*!40000 ALTER TABLE `issue_relations` DISABLE KEYS */;
/*!40000 ALTER TABLE `issue_relations` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `issue_statuses`
--

DROP TABLE IF EXISTS `issue_statuses`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `issue_statuses` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(30) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `is_closed` tinyint(1) NOT NULL DEFAULT '0',
  `position` int(11) DEFAULT NULL,
  `default_done_ratio` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `index_issue_statuses_on_position` (`position`),
  KEY `index_issue_statuses_on_is_closed` (`is_closed`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `issue_statuses`
--

LOCK TABLES `issue_statuses` WRITE;
/*!40000 ALTER TABLE `issue_statuses` DISABLE KEYS */;
INSERT INTO `issue_statuses` VALUES (2,'已认领',0,2,NULL),(3,'处理中',0,3,NULL),(4,'待验收',0,4,NULL),(5,'已解决',1,5,NULL),(6,'新建',0,1,NULL);
/*!40000 ALTER TABLE `issue_statuses` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `issues`
--

DROP TABLE IF EXISTS `issues`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `issues` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `tracker_id` int(11) NOT NULL,
  `project_id` int(11) NOT NULL,
  `subject` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `description` longtext COLLATE utf8_unicode_ci,
  `due_date` date DEFAULT NULL,
  `category_id` int(11) DEFAULT NULL,
  `status_id` int(11) NOT NULL,
  `assigned_to_id` int(11) DEFAULT NULL,
  `priority_id` int(11) NOT NULL,
  `fixed_version_id` int(11) DEFAULT NULL,
  `author_id` int(11) NOT NULL,
  `lock_version` int(11) NOT NULL DEFAULT '0',
  `created_on` datetime DEFAULT NULL,
  `updated_on` datetime DEFAULT NULL,
  `start_date` date DEFAULT NULL,
  `done_ratio` int(11) NOT NULL DEFAULT '0',
  `estimated_hours` float DEFAULT NULL,
  `parent_id` int(11) DEFAULT NULL,
  `root_id` int(11) DEFAULT NULL,
  `lft` int(11) DEFAULT NULL,
  `rgt` int(11) DEFAULT NULL,
  `is_private` tinyint(1) NOT NULL DEFAULT '0',
  `closed_on` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `issues_project_id` (`project_id`),
  KEY `index_issues_on_status_id` (`status_id`),
  KEY `index_issues_on_category_id` (`category_id`),
  KEY `index_issues_on_assigned_to_id` (`assigned_to_id`),
  KEY `index_issues_on_fixed_version_id` (`fixed_version_id`),
  KEY `index_issues_on_tracker_id` (`tracker_id`),
  KEY `index_issues_on_priority_id` (`priority_id`),
  KEY `index_issues_on_author_id` (`author_id`),
  KEY `index_issues_on_created_on` (`created_on`),
  KEY `index_issues_on_root_id_and_lft_and_rgt` (`root_id`,`lft`,`rgt`),
  KEY `index_issues_on_parent_id` (`parent_id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `issues`
--

LOCK TABLES `issues` WRITE;
/*!40000 ALTER TABLE `issues` DISABLE KEYS */;
INSERT INTO `issues` VALUES (1,3,1,'资源打包更新系统','','2018-04-08',5,6,11,1,2,1,3,'2018-04-05 08:56:24','2018-04-05 09:13:19','2018-04-05',0,NULL,NULL,1,1,4,0,NULL),(2,3,1,'资源下载重试','','2018-04-08',5,6,11,1,2,1,1,'2018-04-05 08:59:20','2018-04-05 09:13:19','2018-04-05',0,NULL,1,1,2,3,0,NULL);
/*!40000 ALTER TABLE `issues` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `journal_details`
--

DROP TABLE IF EXISTS `journal_details`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `journal_details` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `journal_id` int(11) NOT NULL DEFAULT '0',
  `property` varchar(30) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `prop_key` varchar(30) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `old_value` longtext COLLATE utf8_unicode_ci,
  `value` longtext COLLATE utf8_unicode_ci,
  PRIMARY KEY (`id`),
  KEY `journal_details_journal_id` (`journal_id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `journal_details`
--

LOCK TABLES `journal_details` WRITE;
/*!40000 ALTER TABLE `journal_details` DISABLE KEYS */;
INSERT INTO `journal_details` VALUES (1,1,'attr','done_ratio','0','70'),(2,2,'attr','due_date',NULL,'2018-04-08');
/*!40000 ALTER TABLE `journal_details` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `journals`
--

DROP TABLE IF EXISTS `journals`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `journals` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `journalized_id` int(11) NOT NULL DEFAULT '0',
  `journalized_type` varchar(30) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `user_id` int(11) NOT NULL DEFAULT '0',
  `notes` longtext COLLATE utf8_unicode_ci,
  `created_on` datetime NOT NULL,
  `private_notes` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `journals_journalized_id` (`journalized_id`,`journalized_type`),
  KEY `index_journals_on_user_id` (`user_id`),
  KEY `index_journals_on_journalized_id` (`journalized_id`),
  KEY `index_journals_on_created_on` (`created_on`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `journals`
--

LOCK TABLES `journals` WRITE;
/*!40000 ALTER TABLE `journals` DISABLE KEYS */;
INSERT INTO `journals` VALUES (1,1,'Issue',1,NULL,'2018-04-05 08:57:42',0),(2,2,'Issue',11,'','2018-04-05 09:13:19',0);
/*!40000 ALTER TABLE `journals` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `member_roles`
--

DROP TABLE IF EXISTS `member_roles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `member_roles` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `member_id` int(11) NOT NULL,
  `role_id` int(11) NOT NULL,
  `inherited_from` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `index_member_roles_on_member_id` (`member_id`),
  KEY `index_member_roles_on_role_id` (`role_id`),
  KEY `index_member_roles_on_inherited_from` (`inherited_from`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `member_roles`
--

LOCK TABLES `member_roles` WRITE;
/*!40000 ALTER TABLE `member_roles` DISABLE KEYS */;
INSERT INTO `member_roles` VALUES (2,2,3,NULL),(4,2,5,NULL);
/*!40000 ALTER TABLE `member_roles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `members`
--

DROP TABLE IF EXISTS `members`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `members` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `user_id` int(11) NOT NULL DEFAULT '0',
  `project_id` int(11) NOT NULL DEFAULT '0',
  `created_on` datetime DEFAULT NULL,
  `mail_notification` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `index_members_on_user_id_and_project_id` (`user_id`,`project_id`),
  KEY `index_members_on_user_id` (`user_id`),
  KEY `index_members_on_project_id` (`project_id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `members`
--

LOCK TABLES `members` WRITE;
/*!40000 ALTER TABLE `members` DISABLE KEYS */;
INSERT INTO `members` VALUES (2,11,1,'2018-04-05 06:52:32',0);
/*!40000 ALTER TABLE `members` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `messages`
--

DROP TABLE IF EXISTS `messages`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `messages` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `board_id` int(11) NOT NULL,
  `parent_id` int(11) DEFAULT NULL,
  `subject` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `content` text COLLATE utf8_unicode_ci,
  `author_id` int(11) DEFAULT NULL,
  `replies_count` int(11) NOT NULL DEFAULT '0',
  `last_reply_id` int(11) DEFAULT NULL,
  `created_on` datetime NOT NULL,
  `updated_on` datetime NOT NULL,
  `locked` tinyint(1) DEFAULT '0',
  `sticky` int(11) DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `messages_board_id` (`board_id`),
  KEY `messages_parent_id` (`parent_id`),
  KEY `index_messages_on_last_reply_id` (`last_reply_id`),
  KEY `index_messages_on_author_id` (`author_id`),
  KEY `index_messages_on_created_on` (`created_on`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `messages`
--

LOCK TABLES `messages` WRITE;
/*!40000 ALTER TABLE `messages` DISABLE KEYS */;
/*!40000 ALTER TABLE `messages` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `news`
--

DROP TABLE IF EXISTS `news`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `news` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `project_id` int(11) DEFAULT NULL,
  `title` varchar(60) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `summary` varchar(255) COLLATE utf8_unicode_ci DEFAULT '',
  `description` text COLLATE utf8_unicode_ci,
  `author_id` int(11) NOT NULL DEFAULT '0',
  `created_on` datetime DEFAULT NULL,
  `comments_count` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `news_project_id` (`project_id`),
  KEY `index_news_on_author_id` (`author_id`),
  KEY `index_news_on_created_on` (`created_on`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `news`
--

LOCK TABLES `news` WRITE;
/*!40000 ALTER TABLE `news` DISABLE KEYS */;
/*!40000 ALTER TABLE `news` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `open_id_authentication_associations`
--

DROP TABLE IF EXISTS `open_id_authentication_associations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `open_id_authentication_associations` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `issued` int(11) DEFAULT NULL,
  `lifetime` int(11) DEFAULT NULL,
  `handle` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `assoc_type` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `server_url` blob,
  `secret` blob,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `open_id_authentication_associations`
--

LOCK TABLES `open_id_authentication_associations` WRITE;
/*!40000 ALTER TABLE `open_id_authentication_associations` DISABLE KEYS */;
/*!40000 ALTER TABLE `open_id_authentication_associations` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `open_id_authentication_nonces`
--

DROP TABLE IF EXISTS `open_id_authentication_nonces`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `open_id_authentication_nonces` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `timestamp` int(11) NOT NULL,
  `server_url` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `salt` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `open_id_authentication_nonces`
--

LOCK TABLES `open_id_authentication_nonces` WRITE;
/*!40000 ALTER TABLE `open_id_authentication_nonces` DISABLE KEYS */;
/*!40000 ALTER TABLE `open_id_authentication_nonces` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `projects`
--

DROP TABLE IF EXISTS `projects`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `projects` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `description` text COLLATE utf8_unicode_ci,
  `homepage` varchar(255) COLLATE utf8_unicode_ci DEFAULT '',
  `is_public` tinyint(1) NOT NULL DEFAULT '1',
  `parent_id` int(11) DEFAULT NULL,
  `created_on` datetime DEFAULT NULL,
  `updated_on` datetime DEFAULT NULL,
  `identifier` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `status` int(11) NOT NULL DEFAULT '1',
  `lft` int(11) DEFAULT NULL,
  `rgt` int(11) DEFAULT NULL,
  `inherit_members` tinyint(1) NOT NULL DEFAULT '0',
  `default_version_id` int(11) DEFAULT NULL,
  `default_assigned_to_id` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `index_projects_on_lft` (`lft`),
  KEY `index_projects_on_rgt` (`rgt`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `projects`
--

LOCK TABLES `projects` WRITE;
/*!40000 ALTER TABLE `projects` DISABLE KEYS */;
INSERT INTO `projects` VALUES (1,'2233 的日常','','',1,NULL,'2018-04-05 06:36:21','2018-04-05 09:03:49','hello2233',1,1,2,0,2,NULL);
/*!40000 ALTER TABLE `projects` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `projects_trackers`
--

DROP TABLE IF EXISTS `projects_trackers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `projects_trackers` (
  `project_id` int(11) NOT NULL DEFAULT '0',
  `tracker_id` int(11) NOT NULL DEFAULT '0',
  UNIQUE KEY `projects_trackers_unique` (`project_id`,`tracker_id`),
  KEY `projects_trackers_project_id` (`project_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `projects_trackers`
--

LOCK TABLES `projects_trackers` WRITE;
/*!40000 ALTER TABLE `projects_trackers` DISABLE KEYS */;
INSERT INTO `projects_trackers` VALUES (1,2),(1,3);
/*!40000 ALTER TABLE `projects_trackers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `queries`
--

DROP TABLE IF EXISTS `queries`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `queries` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `project_id` int(11) DEFAULT NULL,
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `filters` text COLLATE utf8_unicode_ci,
  `user_id` int(11) NOT NULL DEFAULT '0',
  `column_names` text COLLATE utf8_unicode_ci,
  `sort_criteria` text COLLATE utf8_unicode_ci,
  `group_by` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `type` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `visibility` int(11) DEFAULT '0',
  `options` text COLLATE utf8_unicode_ci,
  PRIMARY KEY (`id`),
  KEY `index_queries_on_project_id` (`project_id`),
  KEY `index_queries_on_user_id` (`user_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `queries`
--

LOCK TABLES `queries` WRITE;
/*!40000 ALTER TABLE `queries` DISABLE KEYS */;
/*!40000 ALTER TABLE `queries` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `queries_roles`
--

DROP TABLE IF EXISTS `queries_roles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `queries_roles` (
  `query_id` int(11) NOT NULL,
  `role_id` int(11) NOT NULL,
  UNIQUE KEY `queries_roles_ids` (`query_id`,`role_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `queries_roles`
--

LOCK TABLES `queries_roles` WRITE;
/*!40000 ALTER TABLE `queries_roles` DISABLE KEYS */;
/*!40000 ALTER TABLE `queries_roles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `repositories`
--

DROP TABLE IF EXISTS `repositories`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `repositories` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `project_id` int(11) NOT NULL DEFAULT '0',
  `url` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `login` varchar(60) COLLATE utf8_unicode_ci DEFAULT '',
  `password` varchar(255) COLLATE utf8_unicode_ci DEFAULT '',
  `root_url` varchar(255) COLLATE utf8_unicode_ci DEFAULT '',
  `type` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `path_encoding` varchar(64) COLLATE utf8_unicode_ci DEFAULT NULL,
  `log_encoding` varchar(64) COLLATE utf8_unicode_ci DEFAULT NULL,
  `extra_info` longtext COLLATE utf8_unicode_ci,
  `identifier` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `is_default` tinyint(1) DEFAULT '0',
  `created_on` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `index_repositories_on_project_id` (`project_id`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `repositories`
--

LOCK TABLES `repositories` WRITE;
/*!40000 ALTER TABLE `repositories` DISABLE KEYS */;
INSERT INTO `repositories` VALUES (8,1,'/home/redmine/unity_template.git','','','/home/redmine/unity_template.git','Repository::Git','',NULL,'---\nextra_report_last_commit: \'0\'\nheads:\n- 256762a9691e2d05b9a0b241117a14007227114b\ndb_consistent:\n  ordering: 1\n','',1,'2018-04-05 11:22:49');
/*!40000 ALTER TABLE `repositories` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `roles`
--

DROP TABLE IF EXISTS `roles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `roles` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(30) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `position` int(11) DEFAULT NULL,
  `assignable` tinyint(1) DEFAULT '1',
  `builtin` int(11) NOT NULL DEFAULT '0',
  `permissions` text COLLATE utf8_unicode_ci,
  `issues_visibility` varchar(30) COLLATE utf8_unicode_ci NOT NULL DEFAULT 'default',
  `users_visibility` varchar(30) COLLATE utf8_unicode_ci NOT NULL DEFAULT 'all',
  `time_entries_visibility` varchar(30) COLLATE utf8_unicode_ci NOT NULL DEFAULT 'all',
  `all_roles_managed` tinyint(1) NOT NULL DEFAULT '1',
  `settings` text COLLATE utf8_unicode_ci,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `roles`
--

LOCK TABLES `roles` WRITE;
/*!40000 ALTER TABLE `roles` DISABLE KEYS */;
INSERT INTO `roles` VALUES (1,'Non member',0,1,1,'---\n- :view_messages\n- :view_issues\n- :view_news\n','default','all','all',1,NULL),(2,'Anonymous',0,1,2,NULL,'default','all','all',1,'--- !ruby/hash:ActiveSupport::HashWithIndifferentAccess\npermissions_all_trackers: !ruby/hash:ActiveSupport::HashWithIndifferentAccess\n  view_issues: \'1\'\n  add_issues: \'1\'\n  edit_issues: \'1\'\n  add_issue_notes: \'1\'\n  delete_issues: \'1\'\npermissions_tracker_ids: !ruby/hash:ActiveSupport::HashWithIndifferentAccess\n  view_issues: []\n  add_issues: []\n  edit_issues: []\n  add_issue_notes: []\n  delete_issues: []\n'),(3,'客户端',1,1,0,'---\n- :view_messages\n- :add_messages\n- :edit_own_messages\n- :view_calendar\n- :view_documents\n- :add_documents\n- :edit_documents\n- :view_gantt\n- :view_issues\n- :add_issues\n- :edit_issues\n- :copy_issues\n- :manage_issue_relations\n- :set_issues_private\n- :set_own_issues_private\n- :add_issue_notes\n- :edit_own_issue_notes\n- :add_issue_watchers\n- :import_issues\n- :view_news\n- :comment_news\n- :commit_access\n- :log_time\n- :edit_own_time_entries\n- :view_wiki_pages\n- :view_wiki_edits\n- :edit_wiki_pages\n','default','all','all',1,'--- !ruby/hash:ActiveSupport::HashWithIndifferentAccess\npermissions_all_trackers: !ruby/hash:ActiveSupport::HashWithIndifferentAccess\n  view_issues: \'1\'\n  add_issues: \'1\'\n  edit_issues: \'1\'\n  add_issue_notes: \'1\'\n  delete_issues: \'1\'\npermissions_tracker_ids: !ruby/hash:ActiveSupport::HashWithIndifferentAccess\n  view_issues: []\n  add_issues: []\n  edit_issues: []\n  add_issue_notes: []\n  delete_issues: []\n'),(4,'服务器',2,1,0,'---\n- :edit_own_messages\n- :delete_own_messages\n- :view_calendar\n- :view_documents\n- :add_documents\n- :edit_documents\n- :view_files\n- :view_issues\n- :add_issues\n- :edit_issues\n- :copy_issues\n- :manage_issue_relations\n- :set_issues_private\n- :set_own_issues_private\n- :add_issue_notes\n- :edit_own_issue_notes\n- :add_issue_watchers\n- :import_issues\n- :view_news\n- :comment_news\n- :log_time\n- :edit_own_time_entries\n- :view_wiki_pages\n- :view_wiki_edits\n- :edit_wiki_pages\n','default','all','all',1,'--- !ruby/hash:ActiveSupport::HashWithIndifferentAccess\npermissions_all_trackers: !ruby/hash:ActiveSupport::HashWithIndifferentAccess\n  view_issues: \'1\'\n  add_issues: \'1\'\n  edit_issues: \'1\'\n  add_issue_notes: \'1\'\n  delete_issues: \'1\'\npermissions_tracker_ids: !ruby/hash:ActiveSupport::HashWithIndifferentAccess\n  view_issues: []\n  add_issues: []\n  edit_issues: []\n  add_issue_notes: []\n  delete_issues: []\n'),(5,'管理员',3,1,0,'---\n- :add_project\n- :edit_project\n- :close_project\n- :select_project_modules\n- :manage_members\n- :manage_versions\n- :add_subprojects\n- :manage_public_queries\n- :save_queries\n- :view_messages\n- :add_messages\n- :edit_messages\n- :edit_own_messages\n- :delete_messages\n- :delete_own_messages\n- :manage_boards\n- :view_calendar\n- :view_documents\n- :add_documents\n- :edit_documents\n- :delete_documents\n- :view_files\n- :manage_files\n- :view_gantt\n- :view_issues\n- :add_issues\n- :edit_issues\n- :copy_issues\n- :manage_issue_relations\n- :manage_subtasks\n- :set_issues_private\n- :set_own_issues_private\n- :add_issue_notes\n- :edit_issue_notes\n- :edit_own_issue_notes\n- :view_private_notes\n- :set_notes_private\n- :delete_issues\n- :view_issue_watchers\n- :add_issue_watchers\n- :delete_issue_watchers\n- :import_issues\n- :manage_categories\n- :view_news\n- :manage_news\n- :comment_news\n- :view_changesets\n- :browse_repository\n- :commit_access\n- :manage_related_issues\n- :manage_repository\n- :view_time_entries\n- :log_time\n- :edit_time_entries\n- :edit_own_time_entries\n- :manage_project_activities\n- :view_wiki_pages\n- :view_wiki_edits\n- :export_wiki_pages\n- :edit_wiki_pages\n- :rename_wiki_pages\n- :delete_wiki_pages\n- :delete_wiki_pages_attachments\n- :protect_wiki_pages\n- :manage_wiki\n','default','all','all',1,'--- !ruby/hash:ActiveSupport::HashWithIndifferentAccess\npermissions_all_trackers: !ruby/hash:ActiveSupport::HashWithIndifferentAccess\n  view_issues: \'1\'\n  add_issues: \'1\'\n  edit_issues: \'1\'\n  add_issue_notes: \'1\'\n  delete_issues: \'1\'\npermissions_tracker_ids: !ruby/hash:ActiveSupport::HashWithIndifferentAccess\n  view_issues: []\n  add_issues: []\n  edit_issues: []\n  add_issue_notes: []\n  delete_issues: []\n');
/*!40000 ALTER TABLE `roles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `roles_managed_roles`
--

DROP TABLE IF EXISTS `roles_managed_roles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `roles_managed_roles` (
  `role_id` int(11) NOT NULL,
  `managed_role_id` int(11) NOT NULL,
  UNIQUE KEY `index_roles_managed_roles_on_role_id_and_managed_role_id` (`role_id`,`managed_role_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `roles_managed_roles`
--

LOCK TABLES `roles_managed_roles` WRITE;
/*!40000 ALTER TABLE `roles_managed_roles` DISABLE KEYS */;
/*!40000 ALTER TABLE `roles_managed_roles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `schema_migrations`
--

DROP TABLE IF EXISTS `schema_migrations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `schema_migrations` (
  `version` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  UNIQUE KEY `unique_schema_migrations` (`version`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `schema_migrations`
--

LOCK TABLES `schema_migrations` WRITE;
/*!40000 ALTER TABLE `schema_migrations` DISABLE KEYS */;
INSERT INTO `schema_migrations` VALUES ('1'),('10'),('100'),('101'),('102'),('103'),('104'),('105'),('106'),('107'),('108'),('11'),('12'),('13'),('14'),('15'),('16'),('17'),('18'),('19'),('2'),('20'),('20090214190337'),('20090312172426'),('20090312194159'),('20090318181151'),('20090323224724'),('20090401221305'),('20090401231134'),('20090403001910'),('20090406161854'),('20090425161243'),('20090503121501'),('20090503121505'),('20090503121510'),('20090614091200'),('20090704172350'),('20090704172355'),('20090704172358'),('20091010093521'),('20091017212227'),('20091017212457'),('20091017212644'),('20091017212938'),('20091017213027'),('20091017213113'),('20091017213151'),('20091017213228'),('20091017213257'),('20091017213332'),('20091017213444'),('20091017213536'),('20091017213642'),('20091017213716'),('20091017213757'),('20091017213835'),('20091017213910'),('20091017214015'),('20091017214107'),('20091017214136'),('20091017214236'),('20091017214308'),('20091017214336'),('20091017214406'),('20091017214440'),('20091017214519'),('20091017214611'),('20091017214644'),('20091017214720'),('20091017214750'),('20091025163651'),('20091108092559'),('20091114105931'),('20091123212029'),('20091205124427'),('20091220183509'),('20091220183727'),('20091220184736'),('20091225164732'),('20091227112908'),('20100129193402'),('20100129193813'),('20100221100219'),('20100313132032'),('20100313171051'),('20100705164950'),('20100819172912'),('20101104182107'),('20101107130441'),('20101114115114'),('20101114115359'),('20110220160626'),('20110223180944'),('20110223180953'),('20110224000000'),('20110226120112'),('20110226120132'),('20110227125750'),('20110228000000'),('20110228000100'),('20110401192910'),('20110408103312'),('20110412065600'),('20110511000000'),('20110902000000'),('20111201201315'),('20120115143024'),('20120115143100'),('20120115143126'),('20120127174243'),('20120205111326'),('20120223110929'),('20120301153455'),('20120422150750'),('20120705074331'),('20120707064544'),('20120714122000'),('20120714122100'),('20120714122200'),('20120731164049'),('20120930112914'),('20121026002032'),('20121026003537'),('20121209123234'),('20121209123358'),('20121213084931'),('20130110122628'),('20130201184705'),('20130202090625'),('20130207175206'),('20130207181455'),('20130215073721'),('20130215111127'),('20130215111141'),('20130217094251'),('20130602092539'),('20130710182539'),('20130713104233'),('20130713111657'),('20130729070143'),('20130911193200'),('20131004113137'),('20131005100610'),('20131124175346'),('20131210180802'),('20131214094309'),('20131215104612'),('20131218183023'),('20140228130325'),('20140903143914'),('20140920094058'),('20141029181752'),('20141029181824'),('20141109112308'),('20141122124142'),('20150113194759'),('20150113211532'),('20150113213922'),('20150113213955'),('20150208105930'),('20150510083747'),('20150525103953'),('20150526183158'),('20150528084820'),('20150528092912'),('20150528093249'),('20150725112753'),('20150730122707'),('20150730122735'),('20150921204850'),('20150921210243'),('20151020182334'),('20151020182731'),('20151021184614'),('20151021185456'),('20151021190616'),('20151024082034'),('20151025072118'),('20151031095005'),('20160404080304'),('20160416072926'),('20160529063352'),('20161001122012'),('20161002133421'),('20161010081301'),('20161010081528'),('20161010081600'),('20161126094932'),('20161220091118'),('20170207050700'),('20170302015225'),('20170309214320'),('20170320051650'),('20170418090031'),('20170419144536'),('21'),('22'),('23'),('24'),('25'),('26'),('27'),('28'),('29'),('3'),('30'),('31'),('32'),('33'),('34'),('35'),('36'),('37'),('38'),('39'),('4'),('40'),('41'),('42'),('43'),('44'),('45'),('46'),('47'),('48'),('49'),('5'),('50'),('51'),('52'),('53'),('54'),('55'),('56'),('57'),('58'),('59'),('6'),('60'),('61'),('62'),('63'),('64'),('65'),('66'),('67'),('68'),('69'),('7'),('70'),('71'),('72'),('73'),('74'),('75'),('76'),('77'),('78'),('79'),('8'),('80'),('81'),('82'),('83'),('84'),('85'),('86'),('87'),('88'),('89'),('9'),('90'),('91'),('92'),('93'),('94'),('95'),('96'),('97'),('98'),('99');
/*!40000 ALTER TABLE `schema_migrations` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `settings`
--

DROP TABLE IF EXISTS `settings`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `settings` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `value` text COLLATE utf8_unicode_ci,
  `updated_on` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `index_settings_on_name` (`name`)
) ENGINE=InnoDB AUTO_INCREMENT=35 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `settings`
--

LOCK TABLES `settings` WRITE;
/*!40000 ALTER TABLE `settings` DISABLE KEYS */;
INSERT INTO `settings` VALUES (1,'app_title','Redmine','2018-04-05 06:47:10'),(2,'welcome_text','','2018-04-05 06:41:40'),(3,'per_page_options','25,50,100','2018-04-05 06:41:40'),(4,'search_results_per_page','30','2018-04-05 06:47:42'),(5,'activity_days_default','30','2018-04-05 06:41:40'),(6,'host_name','a3.local:3000','2018-04-05 06:41:40'),(7,'protocol','http','2018-04-05 06:41:40'),(8,'text_formatting','textile','2018-04-05 06:41:40'),(9,'cache_formatted_text','1','2018-04-05 06:41:40'),(10,'wiki_compression','','2018-04-05 06:41:40'),(11,'feeds_limit','15','2018-04-05 06:41:40'),(12,'ui_theme','classic','2018-04-05 09:11:47'),(13,'default_language','zh','2018-04-05 06:42:04'),(14,'force_default_language_for_anonymous','0','2018-04-05 06:42:04'),(15,'force_default_language_for_loggedin','0','2018-04-05 06:42:04'),(16,'start_of_week','1','2018-04-05 06:42:18'),(17,'date_format','','2018-04-05 06:42:04'),(18,'time_format','%H:%M','2018-04-05 06:42:44'),(19,'timespan_format','decimal','2018-04-05 06:42:04'),(20,'user_format','lastnamefirstname','2018-04-05 06:53:03'),(21,'gravatar_enabled','1','2018-04-05 09:11:47'),(22,'gravatar_default','wavatar','2018-04-05 06:53:16'),(23,'thumbnails_enabled','0','2018-04-05 06:42:04'),(24,'thumbnails_size','100','2018-04-05 06:42:04'),(25,'new_item_menu_tab','2','2018-04-05 06:42:04'),(26,'enabled_scm','---\n- Subversion\n- Git\n','2018-04-05 06:45:46'),(27,'autofetch_changesets','1','2018-04-05 06:45:46'),(28,'sys_api_enabled','0','2018-04-05 06:45:46'),(29,'repository_log_display_limit','100','2018-04-05 06:45:46'),(30,'commit_logs_formatting','1','2018-04-05 06:45:46'),(31,'commit_ref_keywords','refs,references,IssueID','2018-04-05 06:45:46'),(32,'commit_cross_project_ref','0','2018-04-05 06:45:46'),(33,'commit_logtime_enabled','0','2018-04-05 06:45:46'),(34,'commit_update_keywords','---\n- status_id: \'0\'\n  keywords: fixed,fix\n','2018-04-05 06:45:46');
/*!40000 ALTER TABLE `settings` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `time_entries`
--

DROP TABLE IF EXISTS `time_entries`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `time_entries` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `project_id` int(11) NOT NULL,
  `user_id` int(11) NOT NULL,
  `issue_id` int(11) DEFAULT NULL,
  `hours` float NOT NULL,
  `comments` varchar(1024) COLLATE utf8_unicode_ci DEFAULT NULL,
  `activity_id` int(11) NOT NULL,
  `spent_on` date NOT NULL,
  `tyear` int(11) NOT NULL,
  `tmonth` int(11) NOT NULL,
  `tweek` int(11) NOT NULL,
  `created_on` datetime NOT NULL,
  `updated_on` datetime NOT NULL,
  PRIMARY KEY (`id`),
  KEY `time_entries_project_id` (`project_id`),
  KEY `time_entries_issue_id` (`issue_id`),
  KEY `index_time_entries_on_activity_id` (`activity_id`),
  KEY `index_time_entries_on_user_id` (`user_id`),
  KEY `index_time_entries_on_created_on` (`created_on`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `time_entries`
--

LOCK TABLES `time_entries` WRITE;
/*!40000 ALTER TABLE `time_entries` DISABLE KEYS */;
/*!40000 ALTER TABLE `time_entries` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tokens`
--

DROP TABLE IF EXISTS `tokens`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tokens` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `user_id` int(11) NOT NULL DEFAULT '0',
  `action` varchar(30) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `value` varchar(40) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `created_on` datetime NOT NULL,
  `updated_on` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `tokens_value` (`value`),
  KEY `index_tokens_on_user_id` (`user_id`)
) ENGINE=InnoDB AUTO_INCREMENT=19 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tokens`
--

LOCK TABLES `tokens` WRITE;
/*!40000 ALTER TABLE `tokens` DISABLE KEYS */;
INSERT INTO `tokens` VALUES (3,1,'feeds','d35730dc683fcf5a03d6905cff2a686cdb7af012','2018-04-05 06:34:01','2018-04-05 06:34:01'),(6,11,'feeds','1fb39ee639ff013eb6799167a2d0a9eb1c16d169','2018-04-05 06:43:02','2018-04-05 06:43:02'),(14,11,'session','8f82eb0192129d970624a2eb4d20a7ed8d88f19f','2018-04-05 09:07:41','2018-04-05 09:41:39'),(15,11,'session','8db843369e9fbe646b066e03666d1e21d7903175','2018-04-05 09:57:11','2018-04-05 11:00:40'),(18,11,'session','d6c4a5e74ada41d595a05f6084b95c806f162190','2018-04-05 11:28:49','2018-04-05 11:33:02');
/*!40000 ALTER TABLE `tokens` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `trackers`
--

DROP TABLE IF EXISTS `trackers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `trackers` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(30) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `is_in_chlog` tinyint(1) NOT NULL DEFAULT '0',
  `position` int(11) DEFAULT NULL,
  `is_in_roadmap` tinyint(1) NOT NULL DEFAULT '1',
  `fields_bits` int(11) DEFAULT '0',
  `default_status_id` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `trackers`
--

LOCK TABLES `trackers` WRITE;
/*!40000 ALTER TABLE `trackers` DISABLE KEYS */;
INSERT INTO `trackers` VALUES (2,'问题跟踪',0,1,1,0,6),(3,'功能开发',0,2,1,0,6);
/*!40000 ALTER TABLE `trackers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `user_preferences`
--

DROP TABLE IF EXISTS `user_preferences`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `user_preferences` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `user_id` int(11) NOT NULL DEFAULT '0',
  `others` text COLLATE utf8_unicode_ci,
  `hide_mail` tinyint(1) DEFAULT '1',
  `time_zone` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `index_user_preferences_on_user_id` (`user_id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user_preferences`
--

LOCK TABLES `user_preferences` WRITE;
/*!40000 ALTER TABLE `user_preferences` DISABLE KEYS */;
INSERT INTO `user_preferences` VALUES (1,1,'---\n:no_self_notified: \'1\'\n:my_page_layout:\n  left:\n  - issuesassignedtome\n  right:\n  - issuesreportedbyme\n:my_page_settings: {}\n:comments_sorting: asc\n:warn_on_leaving_unsaved: \'1\'\n:textarea_font: \'\'\n:gantt_zoom: 2\n:gantt_months: 6\n',1,''),(2,11,'---\n:no_self_notified: \'1\'\n:comments_sorting: asc\n:warn_on_leaving_unsaved: \'1\'\n:textarea_font: \'\'\n:my_page_layout:\n  left:\n  - issuesassignedtome\n  - issueswatched\n  - issuesreportedbyme\n  right:\n  - news\n  top: []\n:my_page_settings:\n  issuesassignedtome:\n    :columns:\n    - subject\n    - status\n    - due_date\n:gantt_zoom: 2\n:gantt_months: 6\n:diff_type: inline\n',1,'');
/*!40000 ALTER TABLE `user_preferences` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `users` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `login` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `hashed_password` varchar(40) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `firstname` varchar(30) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `lastname` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `admin` tinyint(1) NOT NULL DEFAULT '0',
  `status` int(11) NOT NULL DEFAULT '1',
  `last_login_on` datetime DEFAULT NULL,
  `language` varchar(5) COLLATE utf8_unicode_ci DEFAULT '',
  `auth_source_id` int(11) DEFAULT NULL,
  `created_on` datetime DEFAULT NULL,
  `updated_on` datetime DEFAULT NULL,
  `type` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `identity_url` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `mail_notification` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `salt` varchar(64) COLLATE utf8_unicode_ci DEFAULT NULL,
  `must_change_passwd` tinyint(1) NOT NULL DEFAULT '0',
  `passwd_changed_on` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `index_users_on_id_and_type` (`id`,`type`),
  KEY `index_users_on_auth_source_id` (`auth_source_id`),
  KEY `index_users_on_type` (`type`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES (1,'admin','9f89780781ac11651554c5035d77866420f147a6','min','Ad',1,1,'2018-04-05 08:26:47','zh',NULL,'2018-04-05 06:32:38','2018-04-05 08:46:31','User',NULL,'all','d7be44bcf4ea7699fa85da33f588cc5d',0,'2018-04-05 06:33:20'),(2,'','','','Anonymous users',0,1,NULL,'',NULL,'2018-04-05 06:32:42','2018-04-05 06:32:42','GroupAnonymous',NULL,'',NULL,0,NULL),(3,'','','','Non member users',0,1,NULL,'',NULL,'2018-04-05 06:32:42','2018-04-05 06:32:42','GroupNonMember',NULL,'',NULL,0,NULL),(4,'','','','Anonymous',0,0,NULL,'',NULL,'2018-04-05 06:32:56','2018-04-05 06:32:56','AnonymousUser',NULL,'only_my_events',NULL,0,NULL),(5,'','','','工程师',0,1,NULL,'',NULL,'2018-04-05 06:37:21','2018-04-05 06:38:01','Group',NULL,'',NULL,0,NULL),(6,'','','','画家',0,1,NULL,'',NULL,'2018-04-05 06:37:39','2018-04-05 06:37:39','Group',NULL,'',NULL,0,NULL),(7,'','','','动画师',0,1,NULL,'',NULL,'2018-04-05 06:37:44','2018-04-05 06:37:44','Group',NULL,'',NULL,0,NULL),(8,'','','','音乐家',0,1,NULL,'',NULL,'2018-04-05 06:37:49','2018-04-05 06:37:49','Group',NULL,'',NULL,0,NULL),(9,'','','','测试',0,1,NULL,'',NULL,'2018-04-05 06:38:23','2018-04-05 06:38:23','Group',NULL,'',NULL,0,NULL),(10,'','','','策划',0,1,NULL,'',NULL,'2018-04-05 06:38:43','2018-04-05 06:38:43','Group',NULL,'',NULL,0,NULL),(11,'cn','aed8092d44523390e1163056303760efa517cecf','海军','冷',1,1,'2018-04-05 11:28:49','zh',NULL,'2018-04-05 06:40:30','2018-04-05 06:40:30','User',NULL,'only_my_events','cccdc3ddab58dd7faf191d5343643b70',0,'2018-04-05 06:40:30');
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `versions`
--

DROP TABLE IF EXISTS `versions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `versions` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `project_id` int(11) NOT NULL DEFAULT '0',
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `description` varchar(255) COLLATE utf8_unicode_ci DEFAULT '',
  `effective_date` date DEFAULT NULL,
  `created_on` datetime DEFAULT NULL,
  `updated_on` datetime DEFAULT NULL,
  `wiki_page_title` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `status` varchar(255) COLLATE utf8_unicode_ci DEFAULT 'open',
  `sharing` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT 'none',
  PRIMARY KEY (`id`),
  KEY `versions_project_id` (`project_id`),
  KEY `index_versions_on_sharing` (`sharing`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `versions`
--

LOCK TABLES `versions` WRITE;
/*!40000 ALTER TABLE `versions` DISABLE KEYS */;
INSERT INTO `versions` VALUES (2,1,'基础架构','基础架构搭建，技术选型，工作流定义等','2018-04-15','2018-04-05 08:53:13','2018-04-05 08:54:18','','open','none'),(3,1,'内容设计','','2018-04-29','2018-04-05 11:32:19','2018-04-05 11:33:02','','open','none');
/*!40000 ALTER TABLE `versions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `watchers`
--

DROP TABLE IF EXISTS `watchers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `watchers` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `watchable_type` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `watchable_id` int(11) NOT NULL DEFAULT '0',
  `user_id` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `watchers_user_id_type` (`user_id`,`watchable_type`),
  KEY `index_watchers_on_user_id` (`user_id`),
  KEY `index_watchers_on_watchable_id_and_watchable_type` (`watchable_id`,`watchable_type`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `watchers`
--

LOCK TABLES `watchers` WRITE;
/*!40000 ALTER TABLE `watchers` DISABLE KEYS */;
/*!40000 ALTER TABLE `watchers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `wiki_content_versions`
--

DROP TABLE IF EXISTS `wiki_content_versions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `wiki_content_versions` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `wiki_content_id` int(11) NOT NULL,
  `page_id` int(11) NOT NULL,
  `author_id` int(11) DEFAULT NULL,
  `data` longblob,
  `compression` varchar(6) COLLATE utf8_unicode_ci DEFAULT '',
  `comments` varchar(1024) COLLATE utf8_unicode_ci DEFAULT '',
  `updated_on` datetime NOT NULL,
  `version` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `wiki_content_versions_wcid` (`wiki_content_id`),
  KEY `index_wiki_content_versions_on_updated_on` (`updated_on`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `wiki_content_versions`
--

LOCK TABLES `wiki_content_versions` WRITE;
/*!40000 ALTER TABLE `wiki_content_versions` DISABLE KEYS */;
/*!40000 ALTER TABLE `wiki_content_versions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `wiki_contents`
--

DROP TABLE IF EXISTS `wiki_contents`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `wiki_contents` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `page_id` int(11) NOT NULL,
  `author_id` int(11) DEFAULT NULL,
  `text` longtext COLLATE utf8_unicode_ci,
  `comments` varchar(1024) COLLATE utf8_unicode_ci DEFAULT '',
  `updated_on` datetime NOT NULL,
  `version` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `wiki_contents_page_id` (`page_id`),
  KEY `index_wiki_contents_on_author_id` (`author_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `wiki_contents`
--

LOCK TABLES `wiki_contents` WRITE;
/*!40000 ALTER TABLE `wiki_contents` DISABLE KEYS */;
/*!40000 ALTER TABLE `wiki_contents` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `wiki_pages`
--

DROP TABLE IF EXISTS `wiki_pages`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `wiki_pages` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `wiki_id` int(11) NOT NULL,
  `title` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `created_on` datetime NOT NULL,
  `protected` tinyint(1) NOT NULL DEFAULT '0',
  `parent_id` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `wiki_pages_wiki_id_title` (`wiki_id`,`title`),
  KEY `index_wiki_pages_on_wiki_id` (`wiki_id`),
  KEY `index_wiki_pages_on_parent_id` (`parent_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `wiki_pages`
--

LOCK TABLES `wiki_pages` WRITE;
/*!40000 ALTER TABLE `wiki_pages` DISABLE KEYS */;
/*!40000 ALTER TABLE `wiki_pages` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `wiki_redirects`
--

DROP TABLE IF EXISTS `wiki_redirects`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `wiki_redirects` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `wiki_id` int(11) NOT NULL,
  `title` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `redirects_to` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `created_on` datetime NOT NULL,
  `redirects_to_wiki_id` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `wiki_redirects_wiki_id_title` (`wiki_id`,`title`),
  KEY `index_wiki_redirects_on_wiki_id` (`wiki_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `wiki_redirects`
--

LOCK TABLES `wiki_redirects` WRITE;
/*!40000 ALTER TABLE `wiki_redirects` DISABLE KEYS */;
/*!40000 ALTER TABLE `wiki_redirects` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `wikis`
--

DROP TABLE IF EXISTS `wikis`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `wikis` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `project_id` int(11) NOT NULL,
  `start_page` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `status` int(11) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  KEY `wikis_project_id` (`project_id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `wikis`
--

LOCK TABLES `wikis` WRITE;
/*!40000 ALTER TABLE `wikis` DISABLE KEYS */;
INSERT INTO `wikis` VALUES (1,1,'Wiki',1);
/*!40000 ALTER TABLE `wikis` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `workflows`
--

DROP TABLE IF EXISTS `workflows`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `workflows` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `tracker_id` int(11) NOT NULL DEFAULT '0',
  `old_status_id` int(11) NOT NULL DEFAULT '0',
  `new_status_id` int(11) NOT NULL DEFAULT '0',
  `role_id` int(11) NOT NULL DEFAULT '0',
  `assignee` tinyint(1) NOT NULL DEFAULT '0',
  `author` tinyint(1) NOT NULL DEFAULT '0',
  `type` varchar(30) COLLATE utf8_unicode_ci DEFAULT NULL,
  `field_name` varchar(30) COLLATE utf8_unicode_ci DEFAULT NULL,
  `rule` varchar(30) COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `wkfs_role_tracker_old_status` (`role_id`,`tracker_id`,`old_status_id`),
  KEY `index_workflows_on_old_status_id` (`old_status_id`),
  KEY `index_workflows_on_role_id` (`role_id`),
  KEY `index_workflows_on_new_status_id` (`new_status_id`),
  KEY `index_workflows_on_tracker_id` (`tracker_id`)
) ENGINE=InnoDB AUTO_INCREMENT=375 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `workflows`
--

LOCK TABLES `workflows` WRITE;
/*!40000 ALTER TABLE `workflows` DISABLE KEYS */;
INSERT INTO `workflows` VALUES (1,2,0,6,1,0,0,'WorkflowTransition',NULL,NULL),(2,2,0,6,2,0,0,'WorkflowTransition',NULL,NULL),(3,2,0,6,3,0,0,'WorkflowTransition',NULL,NULL),(4,2,0,6,4,0,0,'WorkflowTransition',NULL,NULL),(5,2,0,6,5,0,0,'WorkflowTransition',NULL,NULL),(6,2,0,2,1,0,0,'WorkflowTransition',NULL,NULL),(7,2,0,2,2,0,0,'WorkflowTransition',NULL,NULL),(8,2,0,2,3,0,0,'WorkflowTransition',NULL,NULL),(9,2,0,2,4,0,0,'WorkflowTransition',NULL,NULL),(10,2,0,2,5,0,0,'WorkflowTransition',NULL,NULL),(11,2,0,3,1,0,0,'WorkflowTransition',NULL,NULL),(12,2,0,3,2,0,0,'WorkflowTransition',NULL,NULL),(13,2,0,3,3,0,0,'WorkflowTransition',NULL,NULL),(14,2,0,3,4,0,0,'WorkflowTransition',NULL,NULL),(15,2,0,3,5,0,0,'WorkflowTransition',NULL,NULL),(16,2,0,4,1,0,0,'WorkflowTransition',NULL,NULL),(17,2,0,4,2,0,0,'WorkflowTransition',NULL,NULL),(18,2,0,4,3,0,0,'WorkflowTransition',NULL,NULL),(19,2,0,4,4,0,0,'WorkflowTransition',NULL,NULL),(20,2,0,4,5,0,0,'WorkflowTransition',NULL,NULL),(21,2,6,2,1,0,0,'WorkflowTransition',NULL,NULL),(22,2,6,2,2,0,0,'WorkflowTransition',NULL,NULL),(23,2,6,2,3,0,0,'WorkflowTransition',NULL,NULL),(24,2,6,2,4,0,0,'WorkflowTransition',NULL,NULL),(25,2,6,2,5,0,0,'WorkflowTransition',NULL,NULL),(26,2,6,2,1,1,0,'WorkflowTransition',NULL,NULL),(27,2,6,2,2,1,0,'WorkflowTransition',NULL,NULL),(28,2,6,2,3,1,0,'WorkflowTransition',NULL,NULL),(29,2,6,2,4,1,0,'WorkflowTransition',NULL,NULL),(30,2,6,2,5,1,0,'WorkflowTransition',NULL,NULL),(31,2,6,3,1,0,0,'WorkflowTransition',NULL,NULL),(32,2,6,3,2,0,0,'WorkflowTransition',NULL,NULL),(33,2,6,3,3,0,0,'WorkflowTransition',NULL,NULL),(34,2,6,3,4,0,0,'WorkflowTransition',NULL,NULL),(35,2,6,3,5,0,0,'WorkflowTransition',NULL,NULL),(36,2,6,3,1,1,0,'WorkflowTransition',NULL,NULL),(37,2,6,3,2,1,0,'WorkflowTransition',NULL,NULL),(38,2,6,3,3,1,0,'WorkflowTransition',NULL,NULL),(39,2,6,3,4,1,0,'WorkflowTransition',NULL,NULL),(40,2,6,3,5,1,0,'WorkflowTransition',NULL,NULL),(41,2,6,4,1,0,0,'WorkflowTransition',NULL,NULL),(42,2,6,4,2,0,0,'WorkflowTransition',NULL,NULL),(43,2,6,4,3,0,0,'WorkflowTransition',NULL,NULL),(44,2,6,4,4,0,0,'WorkflowTransition',NULL,NULL),(45,2,6,4,5,0,0,'WorkflowTransition',NULL,NULL),(46,2,6,4,1,1,0,'WorkflowTransition',NULL,NULL),(47,2,6,4,2,1,0,'WorkflowTransition',NULL,NULL),(48,2,6,4,3,1,0,'WorkflowTransition',NULL,NULL),(49,2,6,4,4,1,0,'WorkflowTransition',NULL,NULL),(50,2,6,4,5,1,0,'WorkflowTransition',NULL,NULL),(51,2,6,5,1,0,1,'WorkflowTransition',NULL,NULL),(52,2,6,5,2,0,1,'WorkflowTransition',NULL,NULL),(53,2,6,5,3,0,1,'WorkflowTransition',NULL,NULL),(54,2,6,5,4,0,1,'WorkflowTransition',NULL,NULL),(55,2,6,5,5,0,1,'WorkflowTransition',NULL,NULL),(56,2,2,6,1,0,0,'WorkflowTransition',NULL,NULL),(57,2,2,6,2,0,0,'WorkflowTransition',NULL,NULL),(58,2,2,6,3,0,0,'WorkflowTransition',NULL,NULL),(59,2,2,6,4,0,0,'WorkflowTransition',NULL,NULL),(60,2,2,6,5,0,0,'WorkflowTransition',NULL,NULL),(61,2,2,3,1,0,0,'WorkflowTransition',NULL,NULL),(62,2,2,3,2,0,0,'WorkflowTransition',NULL,NULL),(63,2,2,3,3,0,0,'WorkflowTransition',NULL,NULL),(64,2,2,3,4,0,0,'WorkflowTransition',NULL,NULL),(65,2,2,3,5,0,0,'WorkflowTransition',NULL,NULL),(66,2,2,3,1,1,0,'WorkflowTransition',NULL,NULL),(67,2,2,3,2,1,0,'WorkflowTransition',NULL,NULL),(68,2,2,3,3,1,0,'WorkflowTransition',NULL,NULL),(69,2,2,3,4,1,0,'WorkflowTransition',NULL,NULL),(70,2,2,3,5,1,0,'WorkflowTransition',NULL,NULL),(71,2,2,4,1,0,0,'WorkflowTransition',NULL,NULL),(72,2,2,4,2,0,0,'WorkflowTransition',NULL,NULL),(73,2,2,4,3,0,0,'WorkflowTransition',NULL,NULL),(74,2,2,4,4,0,0,'WorkflowTransition',NULL,NULL),(75,2,2,4,5,0,0,'WorkflowTransition',NULL,NULL),(76,2,2,4,1,1,0,'WorkflowTransition',NULL,NULL),(77,2,2,4,2,1,0,'WorkflowTransition',NULL,NULL),(78,2,2,4,3,1,0,'WorkflowTransition',NULL,NULL),(79,2,2,4,4,1,0,'WorkflowTransition',NULL,NULL),(80,2,2,4,5,1,0,'WorkflowTransition',NULL,NULL),(81,2,2,5,1,0,1,'WorkflowTransition',NULL,NULL),(82,2,2,5,2,0,1,'WorkflowTransition',NULL,NULL),(83,2,2,5,3,0,1,'WorkflowTransition',NULL,NULL),(84,2,2,5,4,0,1,'WorkflowTransition',NULL,NULL),(85,2,2,5,5,0,1,'WorkflowTransition',NULL,NULL),(86,2,3,6,1,0,0,'WorkflowTransition',NULL,NULL),(87,2,3,6,2,0,0,'WorkflowTransition',NULL,NULL),(88,2,3,6,3,0,0,'WorkflowTransition',NULL,NULL),(89,2,3,6,4,0,0,'WorkflowTransition',NULL,NULL),(90,2,3,6,5,0,0,'WorkflowTransition',NULL,NULL),(91,2,3,2,1,0,0,'WorkflowTransition',NULL,NULL),(92,2,3,2,2,0,0,'WorkflowTransition',NULL,NULL),(93,2,3,2,3,0,0,'WorkflowTransition',NULL,NULL),(94,2,3,2,4,0,0,'WorkflowTransition',NULL,NULL),(95,2,3,2,5,0,0,'WorkflowTransition',NULL,NULL),(96,2,3,2,1,1,0,'WorkflowTransition',NULL,NULL),(97,2,3,2,2,1,0,'WorkflowTransition',NULL,NULL),(98,2,3,2,3,1,0,'WorkflowTransition',NULL,NULL),(99,2,3,2,4,1,0,'WorkflowTransition',NULL,NULL),(100,2,3,2,5,1,0,'WorkflowTransition',NULL,NULL),(101,2,3,4,1,0,0,'WorkflowTransition',NULL,NULL),(102,2,3,4,2,0,0,'WorkflowTransition',NULL,NULL),(103,2,3,4,3,0,0,'WorkflowTransition',NULL,NULL),(104,2,3,4,4,0,0,'WorkflowTransition',NULL,NULL),(105,2,3,4,5,0,0,'WorkflowTransition',NULL,NULL),(106,2,3,4,1,1,0,'WorkflowTransition',NULL,NULL),(107,2,3,4,2,1,0,'WorkflowTransition',NULL,NULL),(108,2,3,4,3,1,0,'WorkflowTransition',NULL,NULL),(109,2,3,4,4,1,0,'WorkflowTransition',NULL,NULL),(110,2,3,4,5,1,0,'WorkflowTransition',NULL,NULL),(111,2,3,5,1,0,1,'WorkflowTransition',NULL,NULL),(112,2,3,5,2,0,1,'WorkflowTransition',NULL,NULL),(113,2,3,5,3,0,1,'WorkflowTransition',NULL,NULL),(114,2,3,5,4,0,1,'WorkflowTransition',NULL,NULL),(115,2,3,5,5,0,1,'WorkflowTransition',NULL,NULL),(116,2,4,6,1,0,0,'WorkflowTransition',NULL,NULL),(117,2,4,6,2,0,0,'WorkflowTransition',NULL,NULL),(118,2,4,6,3,0,0,'WorkflowTransition',NULL,NULL),(119,2,4,6,4,0,0,'WorkflowTransition',NULL,NULL),(120,2,4,6,5,0,0,'WorkflowTransition',NULL,NULL),(121,2,4,2,1,0,0,'WorkflowTransition',NULL,NULL),(122,2,4,2,2,0,0,'WorkflowTransition',NULL,NULL),(123,2,4,2,3,0,0,'WorkflowTransition',NULL,NULL),(124,2,4,2,4,0,0,'WorkflowTransition',NULL,NULL),(125,2,4,2,5,0,0,'WorkflowTransition',NULL,NULL),(126,2,4,2,1,1,0,'WorkflowTransition',NULL,NULL),(127,2,4,2,2,1,0,'WorkflowTransition',NULL,NULL),(128,2,4,2,3,1,0,'WorkflowTransition',NULL,NULL),(129,2,4,2,4,1,0,'WorkflowTransition',NULL,NULL),(130,2,4,2,5,1,0,'WorkflowTransition',NULL,NULL),(131,2,4,3,1,0,0,'WorkflowTransition',NULL,NULL),(132,2,4,3,2,0,0,'WorkflowTransition',NULL,NULL),(133,2,4,3,3,0,0,'WorkflowTransition',NULL,NULL),(134,2,4,3,4,0,0,'WorkflowTransition',NULL,NULL),(135,2,4,3,5,0,0,'WorkflowTransition',NULL,NULL),(136,2,4,3,1,1,0,'WorkflowTransition',NULL,NULL),(137,2,4,3,2,1,0,'WorkflowTransition',NULL,NULL),(138,2,4,3,3,1,0,'WorkflowTransition',NULL,NULL),(139,2,4,3,4,1,0,'WorkflowTransition',NULL,NULL),(140,2,4,3,5,1,0,'WorkflowTransition',NULL,NULL),(141,2,4,5,1,0,1,'WorkflowTransition',NULL,NULL),(142,2,4,5,2,0,1,'WorkflowTransition',NULL,NULL),(143,2,4,5,3,0,1,'WorkflowTransition',NULL,NULL),(144,2,4,5,4,0,1,'WorkflowTransition',NULL,NULL),(145,2,4,5,5,0,1,'WorkflowTransition',NULL,NULL),(146,2,5,6,1,0,0,'WorkflowTransition',NULL,NULL),(147,2,5,6,2,0,0,'WorkflowTransition',NULL,NULL),(148,2,5,6,3,0,0,'WorkflowTransition',NULL,NULL),(149,2,5,6,4,0,0,'WorkflowTransition',NULL,NULL),(150,2,5,6,5,0,0,'WorkflowTransition',NULL,NULL),(151,2,5,2,1,0,0,'WorkflowTransition',NULL,NULL),(152,2,5,2,2,0,0,'WorkflowTransition',NULL,NULL),(153,2,5,2,3,0,0,'WorkflowTransition',NULL,NULL),(154,2,5,2,4,0,0,'WorkflowTransition',NULL,NULL),(155,2,5,2,5,0,0,'WorkflowTransition',NULL,NULL),(156,2,5,2,1,1,0,'WorkflowTransition',NULL,NULL),(157,2,5,2,2,1,0,'WorkflowTransition',NULL,NULL),(158,2,5,2,3,1,0,'WorkflowTransition',NULL,NULL),(159,2,5,2,4,1,0,'WorkflowTransition',NULL,NULL),(160,2,5,2,5,1,0,'WorkflowTransition',NULL,NULL),(161,2,5,3,1,0,0,'WorkflowTransition',NULL,NULL),(162,2,5,3,2,0,0,'WorkflowTransition',NULL,NULL),(163,2,5,3,3,0,0,'WorkflowTransition',NULL,NULL),(164,2,5,3,4,0,0,'WorkflowTransition',NULL,NULL),(165,2,5,3,5,0,0,'WorkflowTransition',NULL,NULL),(166,2,5,3,1,1,0,'WorkflowTransition',NULL,NULL),(167,2,5,3,2,1,0,'WorkflowTransition',NULL,NULL),(168,2,5,3,3,1,0,'WorkflowTransition',NULL,NULL),(169,2,5,3,4,1,0,'WorkflowTransition',NULL,NULL),(170,2,5,3,5,1,0,'WorkflowTransition',NULL,NULL),(171,2,5,4,1,0,0,'WorkflowTransition',NULL,NULL),(172,2,5,4,2,0,0,'WorkflowTransition',NULL,NULL),(173,2,5,4,3,0,0,'WorkflowTransition',NULL,NULL),(174,2,5,4,4,0,0,'WorkflowTransition',NULL,NULL),(175,2,5,4,5,0,0,'WorkflowTransition',NULL,NULL),(176,2,5,4,1,1,0,'WorkflowTransition',NULL,NULL),(177,2,5,4,2,1,0,'WorkflowTransition',NULL,NULL),(178,2,5,4,3,1,0,'WorkflowTransition',NULL,NULL),(179,2,5,4,4,1,0,'WorkflowTransition',NULL,NULL),(180,2,5,4,5,1,0,'WorkflowTransition',NULL,NULL),(181,2,5,5,1,0,1,'WorkflowTransition',NULL,NULL),(182,2,5,5,2,0,1,'WorkflowTransition',NULL,NULL),(183,2,5,5,3,0,1,'WorkflowTransition',NULL,NULL),(184,2,5,5,4,0,1,'WorkflowTransition',NULL,NULL),(185,2,5,5,5,0,1,'WorkflowTransition',NULL,NULL),(186,3,0,6,3,0,0,'WorkflowTransition',NULL,NULL),(187,3,0,2,3,0,0,'WorkflowTransition',NULL,NULL),(188,3,0,3,3,0,0,'WorkflowTransition',NULL,NULL),(189,3,0,4,3,0,0,'WorkflowTransition',NULL,NULL),(190,3,2,6,3,0,0,'WorkflowTransition',NULL,NULL),(191,3,2,3,3,0,0,'WorkflowTransition',NULL,NULL),(192,3,2,3,3,1,0,'WorkflowTransition',NULL,NULL),(193,3,2,4,3,0,0,'WorkflowTransition',NULL,NULL),(194,3,2,4,3,1,0,'WorkflowTransition',NULL,NULL),(195,3,2,5,3,0,1,'WorkflowTransition',NULL,NULL),(196,3,3,6,3,0,0,'WorkflowTransition',NULL,NULL),(197,3,3,2,3,0,0,'WorkflowTransition',NULL,NULL),(198,3,3,2,3,1,0,'WorkflowTransition',NULL,NULL),(199,3,3,4,3,0,0,'WorkflowTransition',NULL,NULL),(200,3,3,4,3,1,0,'WorkflowTransition',NULL,NULL),(201,3,3,5,3,0,1,'WorkflowTransition',NULL,NULL),(202,3,4,6,3,0,0,'WorkflowTransition',NULL,NULL),(203,3,4,2,3,0,0,'WorkflowTransition',NULL,NULL),(204,3,4,2,3,1,0,'WorkflowTransition',NULL,NULL),(205,3,4,3,3,0,0,'WorkflowTransition',NULL,NULL),(206,3,4,3,3,1,0,'WorkflowTransition',NULL,NULL),(207,3,4,5,3,0,1,'WorkflowTransition',NULL,NULL),(208,3,5,6,3,0,0,'WorkflowTransition',NULL,NULL),(209,3,5,2,3,0,0,'WorkflowTransition',NULL,NULL),(210,3,5,2,3,1,0,'WorkflowTransition',NULL,NULL),(211,3,5,3,3,0,0,'WorkflowTransition',NULL,NULL),(212,3,5,3,3,1,0,'WorkflowTransition',NULL,NULL),(213,3,5,4,3,0,0,'WorkflowTransition',NULL,NULL),(214,3,5,4,3,1,0,'WorkflowTransition',NULL,NULL),(215,3,5,5,3,0,1,'WorkflowTransition',NULL,NULL),(216,3,6,2,3,0,0,'WorkflowTransition',NULL,NULL),(217,3,6,2,3,1,0,'WorkflowTransition',NULL,NULL),(218,3,6,3,3,0,0,'WorkflowTransition',NULL,NULL),(219,3,6,3,3,1,0,'WorkflowTransition',NULL,NULL),(220,3,6,4,3,0,0,'WorkflowTransition',NULL,NULL),(221,3,6,4,3,1,0,'WorkflowTransition',NULL,NULL),(222,3,6,5,3,0,1,'WorkflowTransition',NULL,NULL),(249,3,0,6,4,0,0,'WorkflowTransition',NULL,NULL),(250,3,0,2,4,0,0,'WorkflowTransition',NULL,NULL),(251,3,0,3,4,0,0,'WorkflowTransition',NULL,NULL),(252,3,0,4,4,0,0,'WorkflowTransition',NULL,NULL),(253,3,2,6,4,0,0,'WorkflowTransition',NULL,NULL),(254,3,2,3,4,0,0,'WorkflowTransition',NULL,NULL),(255,3,2,3,4,1,0,'WorkflowTransition',NULL,NULL),(256,3,2,4,4,0,0,'WorkflowTransition',NULL,NULL),(257,3,2,4,4,1,0,'WorkflowTransition',NULL,NULL),(258,3,2,5,4,0,1,'WorkflowTransition',NULL,NULL),(259,3,3,6,4,0,0,'WorkflowTransition',NULL,NULL),(260,3,3,2,4,0,0,'WorkflowTransition',NULL,NULL),(261,3,3,2,4,1,0,'WorkflowTransition',NULL,NULL),(262,3,3,4,4,0,0,'WorkflowTransition',NULL,NULL),(263,3,3,4,4,1,0,'WorkflowTransition',NULL,NULL),(264,3,3,5,4,0,1,'WorkflowTransition',NULL,NULL),(265,3,4,6,4,0,0,'WorkflowTransition',NULL,NULL),(266,3,4,2,4,0,0,'WorkflowTransition',NULL,NULL),(267,3,4,2,4,1,0,'WorkflowTransition',NULL,NULL),(268,3,4,3,4,0,0,'WorkflowTransition',NULL,NULL),(269,3,4,3,4,1,0,'WorkflowTransition',NULL,NULL),(270,3,4,5,4,0,1,'WorkflowTransition',NULL,NULL),(271,3,5,6,4,0,0,'WorkflowTransition',NULL,NULL),(272,3,5,2,4,0,0,'WorkflowTransition',NULL,NULL),(273,3,5,2,4,1,0,'WorkflowTransition',NULL,NULL),(274,3,5,3,4,0,0,'WorkflowTransition',NULL,NULL),(275,3,5,3,4,1,0,'WorkflowTransition',NULL,NULL),(276,3,5,4,4,0,0,'WorkflowTransition',NULL,NULL),(277,3,5,4,4,1,0,'WorkflowTransition',NULL,NULL),(278,3,5,5,4,0,1,'WorkflowTransition',NULL,NULL),(279,3,6,2,4,0,0,'WorkflowTransition',NULL,NULL),(280,3,6,2,4,1,0,'WorkflowTransition',NULL,NULL),(281,3,6,3,4,0,0,'WorkflowTransition',NULL,NULL),(282,3,6,3,4,1,0,'WorkflowTransition',NULL,NULL),(283,3,6,4,4,0,0,'WorkflowTransition',NULL,NULL),(284,3,6,4,4,1,0,'WorkflowTransition',NULL,NULL),(285,3,6,5,4,0,1,'WorkflowTransition',NULL,NULL),(312,3,0,6,5,0,0,'WorkflowTransition',NULL,NULL),(313,3,0,2,5,0,0,'WorkflowTransition',NULL,NULL),(314,3,0,3,5,0,0,'WorkflowTransition',NULL,NULL),(315,3,0,4,5,0,0,'WorkflowTransition',NULL,NULL),(316,3,2,6,5,0,0,'WorkflowTransition',NULL,NULL),(317,3,2,3,5,0,0,'WorkflowTransition',NULL,NULL),(318,3,2,3,5,1,0,'WorkflowTransition',NULL,NULL),(319,3,2,4,5,0,0,'WorkflowTransition',NULL,NULL),(320,3,2,4,5,1,0,'WorkflowTransition',NULL,NULL),(321,3,2,5,5,0,1,'WorkflowTransition',NULL,NULL),(322,3,3,6,5,0,0,'WorkflowTransition',NULL,NULL),(323,3,3,2,5,0,0,'WorkflowTransition',NULL,NULL),(324,3,3,2,5,1,0,'WorkflowTransition',NULL,NULL),(325,3,3,4,5,0,0,'WorkflowTransition',NULL,NULL),(326,3,3,4,5,1,0,'WorkflowTransition',NULL,NULL),(327,3,3,5,5,0,1,'WorkflowTransition',NULL,NULL),(328,3,4,6,5,0,0,'WorkflowTransition',NULL,NULL),(329,3,4,2,5,0,0,'WorkflowTransition',NULL,NULL),(330,3,4,2,5,1,0,'WorkflowTransition',NULL,NULL),(331,3,4,3,5,0,0,'WorkflowTransition',NULL,NULL),(332,3,4,3,5,1,0,'WorkflowTransition',NULL,NULL),(333,3,4,5,5,0,1,'WorkflowTransition',NULL,NULL),(334,3,5,6,5,0,0,'WorkflowTransition',NULL,NULL),(335,3,5,2,5,0,0,'WorkflowTransition',NULL,NULL),(336,3,5,2,5,1,0,'WorkflowTransition',NULL,NULL),(337,3,5,3,5,0,0,'WorkflowTransition',NULL,NULL),(338,3,5,3,5,1,0,'WorkflowTransition',NULL,NULL),(339,3,5,4,5,0,0,'WorkflowTransition',NULL,NULL),(340,3,5,4,5,1,0,'WorkflowTransition',NULL,NULL),(341,3,5,5,5,0,1,'WorkflowTransition',NULL,NULL),(342,3,6,2,5,0,0,'WorkflowTransition',NULL,NULL),(343,3,6,2,5,1,0,'WorkflowTransition',NULL,NULL),(344,3,6,3,5,0,0,'WorkflowTransition',NULL,NULL),(345,3,6,3,5,1,0,'WorkflowTransition',NULL,NULL),(346,3,6,4,5,0,0,'WorkflowTransition',NULL,NULL),(347,3,6,4,5,1,0,'WorkflowTransition',NULL,NULL),(348,3,6,5,5,0,1,'WorkflowTransition',NULL,NULL);
/*!40000 ALTER TABLE `workflows` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2018-04-05 11:41:04
