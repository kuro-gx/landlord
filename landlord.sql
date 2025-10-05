/*
 Navicat Premium Data Transfer

 Source Server         : MySQL
 Source Server Type    : MySQL
 Source Server Version : 80017
 Source Host           : localhost:3306
 Source Schema         : landlord

 Target Server Type    : MySQL
 Target Server Version : 80017
 File Encoding         : 65001

 Date: 05/10/2025 21:01:24
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for tb_user
-- ----------------------------
DROP TABLE IF EXISTS `tb_user`;
CREATE TABLE `tb_user`  (
  `id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `username` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '昵称',
  `mobile` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '手机号',
  `password` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '密码',
  `avatar` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '头像',
  `gender` tinyint(2) NOT NULL DEFAULT 0 COMMENT '0未知，1男，2女',
  `level` int(6) UNSIGNED NOT NULL DEFAULT 0 COMMENT '等级',
  `money` int(10) NOT NULL DEFAULT 0 COMMENT '豆子',
  `diamond` int(10) NOT NULL DEFAULT 0 COMMENT '钻石',
  `state` tinyint(2) NOT NULL DEFAULT 1 COMMENT '1正常，2冻结，3删除',
  `win_count` int(8) NOT NULL DEFAULT 0 COMMENT '胜场数',
  `lose_count` int(8) NOT NULL DEFAULT 0 COMMENT '负场数',
  `win_last` int(10) UNSIGNED NOT NULL DEFAULT 0 COMMENT '连续胜利数',
  `remark` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT '备注',
  `create_time` datetime(0) NULL DEFAULT NULL,
  `update_time` datetime(0) NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP(0),
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 5 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '用户表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of tb_user
-- ----------------------------
INSERT INTO `tb_user` VALUES (2, '斗地主糕手', '13000000001', '4QrcOUm6Wau+VuBX8g+IPg==', NULL, 1, 0, 8060, 500, 1, 1, 1, 0, NULL, '2025-08-24 22:00:49', '2025-09-21 15:27:52');
INSERT INTO `tb_user` VALUES (3, '用户8927', '13000000002', '4QrcOUm6Wau+VuBX8g+IPg==', NULL, 0, 0, 7880, 500, 1, 0, 2, 0, NULL, '2025-09-21 17:37:04', '2025-09-21 17:37:04');
INSERT INTO `tb_user` VALUES (4, '用户5329', '13000000003', '4QrcOUm6Wau+VuBX8g+IPg==', NULL, 0, 0, 8060, 500, 1, 1, 1, 0, NULL, '2025-09-21 17:37:38', '2025-09-21 17:37:38');

SET FOREIGN_KEY_CHECKS = 1;
