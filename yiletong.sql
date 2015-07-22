/*
Navicat MySQL Data Transfer

Source Server         : 192.168.1.119
Source Server Version : 50621
Source Host           : 192.168.1.119:3306
Source Database       : yiletong

Target Server Type    : MYSQL
Target Server Version : 50621
File Encoding         : 65001

Date: 2015-07-21 23:00:18
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for customers
-- ----------------------------
DROP TABLE IF EXISTS `customers`;
CREATE TABLE `customers` (
  `id` int(11) NOT NULL DEFAULT '0',
  `terminal` text,
  `faren` text,
  `shanghuName` text,
  `tel` text,
  `discount` float DEFAULT NULL,
  `fengding` float DEFAULT NULL,
  `tixianfeiEles` float DEFAULT NULL,
  `tixianfei` float DEFAULT NULL,
  `bankName` text,
  `bankName2` varchar(64) DEFAULT NULL,
  `bankAccount` text,
  `status` int(11) DEFAULT '1',
  `province` varchar(20) DEFAULT NULL,
  `city` varchar(20) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of customers
-- ----------------------------
INSERT INTO `customers` VALUES ('1', '65878132', '詹彦城', '鼓楼区新中源陶瓷', null, '0.38', '0', '0.2', '0.2', null, null, null, '1', null, null);
INSERT INTO `customers` VALUES ('2', '65734831', '林沈午', '永安市公路港通讯店', '', '0.38', '0', '0.2', '0.2', null, null, null, '1', null, null);
INSERT INTO `customers` VALUES ('3', '65804054', '周水发', '台江区见亿食品商店', null, '0.38', '0', '0.2', '0.25', null, null, null, '1', null, null);
INSERT INTO `customers` VALUES ('4', '66014565', '兰李琴', '福州市鼓楼区华安保险', '', '0.38', '0', '0.2', '0.2', null, null, null, '1', null, null);
INSERT INTO `customers` VALUES ('5', '65810230', '姚德顺', '台江区永德家用电器商行', '', '0.4', '0', '0.2', '0.2', null, null, null, '1', null, null);
INSERT INTO `customers` VALUES ('6', '65774305', '胡国平', '台江区黄鹤楼家用电器商行', '121312', '0.38', '0', '0.2', '0.2', null, null, null, '1', null, null);
INSERT INTO `customers` VALUES ('8', '12341234', '1234', '12341234', '12341234', '1234', null, null, '1212', null, null, null, '-1', null, null);

-- ----------------------------
-- Table structure for erp_sequence
-- ----------------------------
DROP TABLE IF EXISTS `erp_sequence`;
CREATE TABLE `erp_sequence` (
  `tableName` text,
  `Val` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of erp_sequence
-- ----------------------------
INSERT INTO `erp_sequence` VALUES ('transactionLogs', '758');
INSERT INTO `erp_sequence` VALUES ('customers', '8');

-- ----------------------------
-- Table structure for transactionlogs
-- ----------------------------
DROP TABLE IF EXISTS `transactionlogs`;
CREATE TABLE `transactionlogs` (
  `id` int(11) NOT NULL DEFAULT '0',
  `terminal` text,
  `tradeName` text,
  `results` text,
  `shanghuName` text,
  `faren` text,
  `tradeMoney` float(10,2) DEFAULT NULL,
  `time` text,
  `shouxufei` float(10,2) DEFAULT NULL,
  `finallyMoney` float(10,2) DEFAULT NULL,
  `cards` text,
  `jiesuantime` text,
  `jiesuanstatus` int(11) DEFAULT '1' COMMENT '记录标志，非结单标志',
  `record` varchar(255) DEFAULT NULL,
  `timeStr` text COMMENT '抓取来的时间格式',
  `resultCode` int(11) DEFAULT NULL COMMENT '结果代码',
  `discountMoney` float DEFAULT NULL COMMENT 'pos机扣率收费',
  `tixianfeiMoney` float DEFAULT NULL COMMENT 'T0收费',
  `status` int(11) DEFAULT NULL,
  `sumid` int(11) DEFAULT NULL COMMENT '结算编号',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of transactionlogs
-- ----------------------------

-- ----------------------------
-- Table structure for transactionsum
-- ----------------------------
DROP TABLE IF EXISTS `transactionsum`;
CREATE TABLE `transactionsum` (
  `id` int(11) NOT NULL,
  `terminal` text NOT NULL,
  `tradeMoney` float NOT NULL,
  `tixianfeiMoney` float NOT NULL,
  `discountMoney` float NOT NULL,
  `finallyMoney` float NOT NULL,
  `status` int(11) NOT NULL DEFAULT '0' COMMENT '0：正常，1：送盘，2：成功，-2：失败',
  `result` text,
  `createDate` datetime DEFAULT CURRENT_TIMESTAMP,
  `uploadDate` datetime DEFAULT NULL,
  `reciveDate` datetime DEFAULT NULL,
  `batchAmount` int(11) DEFAULT NULL COMMENT '笔数',
  `bankName` text,
  `bankName2` varchar(64) DEFAULT NULL,
  `province` varchar(20) DEFAULT NULL,
  `city` varchar(20) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of transactionsum
-- ----------------------------

-- ----------------------------
-- Table structure for users
-- ----------------------------
DROP TABLE IF EXISTS `users`;
CREATE TABLE `users` (
  `id` int(11) NOT NULL,
  `userName` text NOT NULL,
  `password` text,
  `secretKey` text,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of users
-- ----------------------------
INSERT INTO `users` VALUES ('0', 'yiletong', 'yiletong123', null);
