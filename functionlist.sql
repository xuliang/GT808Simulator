/*
 Navicat Premium Data Transfer

 Source Server         : 10.1.97.7
 Source Server Type    : MySQL
 Source Server Version : 80019
 Source Host           : 10.1.97.7:3306
 Source Schema         : gt808Simulator

 Target Server Type    : MySQL
 Target Server Version : 80019
 File Encoding         : 65001

 Date: 19/05/2020 21:29:24
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for functionlist
-- ----------------------------
DROP TABLE IF EXISTS `functionlist`;
CREATE TABLE `functionlist`  (
  `FunctionID` int(0) NOT NULL AUTO_INCREMENT COMMENT '功能编号',
  `MessageID` varchar(20) CHARACTER SET gbk COLLATE gbk_bin NOT NULL DEFAULT '-1' COMMENT '消息ID',
  `FunctionName` varchar(30) CHARACTER SET gbk COLLATE gbk_bin NOT NULL COMMENT '功能名称',
  `Level` int(0) NOT NULL COMMENT '级别',
  `ParentFunctionID` int(0) NOT NULL COMMENT '父功能',
  `RootFunctionID` int(0) NOT NULL COMMENT '根功能',
  `Note` varchar(200) CHARACTER SET gbk COLLATE gbk_bin NULL DEFAULT NULL COMMENT '备注',
  `UserFlag` int(0) NOT NULL DEFAULT 1,
  `SerialID` int(0) NOT NULL DEFAULT 0 COMMENT '排序使用',
  PRIMARY KEY (`FunctionID`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 237 CHARACTER SET = gbk COLLATE = gbk_bin COMMENT = '功能信息' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of functionlist
-- ----------------------------
INSERT INTO `functionlist` VALUES (175, '', '1.终端管理', 1, 0, 0, '', 2, 0);
INSERT INTO `functionlist` VALUES (176, '', '2.位置报警', 1, 0, 0, '', 2, 0);
INSERT INTO `functionlist` VALUES (177, '', '3.信息', 1, 0, 0, '', 2, 0);
INSERT INTO `functionlist` VALUES (178, '', '4.电话', 1, 0, 0, '', 2, 0);
INSERT INTO `functionlist` VALUES (179, '', '5.车辆控制', 1, 0, 0, '', 2, 0);
INSERT INTO `functionlist` VALUES (180, '', '6.车辆管理', 1, 0, 0, '', 2, 0);
INSERT INTO `functionlist` VALUES (181, '', '7.信息采集', 1, 0, 0, '', 2, 0);
INSERT INTO `functionlist` VALUES (182, '', '8.多媒体', 1, 0, 0, '', 2, 0);
INSERT INTO `functionlist` VALUES (183, '', '9.通用数据传输', 1, 0, 0, '', 2, 0);
INSERT INTO `functionlist` VALUES (186, '0x0100', '1.1终端注册', 2, 175, 175, '', 2, 0);
INSERT INTO `functionlist` VALUES (187, '0x0003', '1.2终端注销', 2, 175, 175, '', 2, 0);
INSERT INTO `functionlist` VALUES (188, '0x0102', '1.3终端鉴权', 2, 175, 175, '', 2, 0);
INSERT INTO `functionlist` VALUES (189, '0x0002', '1.4终端心跳', 2, 175, 175, '', 2, 0);
INSERT INTO `functionlist` VALUES (190, '0x8103', '1.5设置终端参数', 2, 175, 175, NULL, 2, 0);
INSERT INTO `functionlist` VALUES (191, '0x8104', '1.6查询终端参数', 2, 175, 175, '', 2, 0);
INSERT INTO `functionlist` VALUES (192, '0x8105', '1.7终端控制', 2, 175, 175, '', 2, 0);
INSERT INTO `functionlist` VALUES (193, '0x0200', '2.1位置及报警', 2, 176, 176, '', 2, 0);
INSERT INTO `functionlist` VALUES (194, '0x8201', '2.2位置信息查询', 2, 176, 176, '', 2, 0);
INSERT INTO `functionlist` VALUES (195, '0x8202', '2.3临时位置跟踪控制', 2, 176, 176, '', 2, 0);
INSERT INTO `functionlist` VALUES (196, '0x8300', '3.1文本信息下发', 2, 177, 177, '', 2, 0);
INSERT INTO `functionlist` VALUES (197, '0x8301', '3.2事件设置', 2, 177, 177, '', 2, 0);
INSERT INTO `functionlist` VALUES (198, '0x0301', '3.3事件报告', 2, 177, 177, '', 2, 0);
INSERT INTO `functionlist` VALUES (199, '0x8302', '3.4提问下发', 2, 177, 177, '', 2, 0);
INSERT INTO `functionlist` VALUES (200, '0x8303', '3.5信息点播菜单设置', 2, 177, 177, '', 2, 0);
INSERT INTO `functionlist` VALUES (201, '0x0303', '3.6信息点播/取消', 2, 177, 177, '', 2, 0);
INSERT INTO `functionlist` VALUES (202, '0x8304', '3.7信息服务', 2, 177, 177, '', 2, 0);
INSERT INTO `functionlist` VALUES (203, '0x8400', '4.1电话回拨', 2, 178, 178, '', 2, 0);
INSERT INTO `functionlist` VALUES (204, '0x8401', '4.2设置电话本', 2, 178, 178, '', 2, 0);
INSERT INTO `functionlist` VALUES (205, '0x8500', '5.1车辆控制', 2, 179, 179, '', 2, 0);
INSERT INTO `functionlist` VALUES (206, '0x8600', '6.1设置圆形区域', 2, 180, 180, '', 2, 0);
INSERT INTO `functionlist` VALUES (207, '0x8601', '6.2删除圆形区域', 2, 180, 180, '', 2, 0);
INSERT INTO `functionlist` VALUES (208, '0x8602', '6.3设置矩形区域', 2, 180, 180, '', 2, 0);
INSERT INTO `functionlist` VALUES (209, '0x8603', '6.4删除矩形区域', 2, 180, 180, '', 2, 0);
INSERT INTO `functionlist` VALUES (210, '0x8604', '6.5设置多边形区域', 2, 180, 180, '', 2, 0);
INSERT INTO `functionlist` VALUES (211, '0x8605', '6.6删除多边形区域', 2, 180, 180, '', 2, 0);
INSERT INTO `functionlist` VALUES (212, '0x8606', '6.7设置路线', 2, 180, 180, '', 2, 0);
INSERT INTO `functionlist` VALUES (213, '0x8607', '6.8删除路线', 2, 180, 180, '', 2, 0);
INSERT INTO `functionlist` VALUES (214, '0x8700', '7.1行驶记录仪数据采集命令', 2, 181, 181, '', 2, 0);
INSERT INTO `functionlist` VALUES (216, '0x0701', '7.2电子运单上报', 2, 181, 181, '', 2, 0);
INSERT INTO `functionlist` VALUES (217, '0x0702', '7.3驾驶员身份信息采集上报', 2, 181, 181, '', 2, 0);
INSERT INTO `functionlist` VALUES (218, '0x0800', '8.1多媒体事件信息上传', 2, 182, 182, '', 2, 0);
INSERT INTO `functionlist` VALUES (220, '0x8801', '8.2摄像头立即拍摄命令', 2, 182, 182, '', 2, 0);
INSERT INTO `functionlist` VALUES (221, '0x8802', '8.3存储多媒体数据检索', 2, 182, 182, '', 2, 0);
INSERT INTO `functionlist` VALUES (222, '0x8803', '8.4存储多媒体数据上传', 2, 182, 182, '', 2, 0);
INSERT INTO `functionlist` VALUES (223, '0x8804', '8.5录音开始命令', 2, 182, 182, '', 2, 0);
INSERT INTO `functionlist` VALUES (224, '0x8900', '9.1数据下行透传', 2, 183, 183, '', 2, 0);
INSERT INTO `functionlist` VALUES (225, '0x0900', '9.2数据上行透传', 2, 183, 183, '', 2, 0);
INSERT INTO `functionlist` VALUES (231, '0x0302', '3.8提问应答', 2, 177, 177, '', 2, 0);
INSERT INTO `functionlist` VALUES (236, '0x0801', '8.6多媒体数据上传', 2, 182, 182, NULL, 1, 0);

SET FOREIGN_KEY_CHECKS = 1;
