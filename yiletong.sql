/*
Navicat MySQL Data Transfer

Source Server         : localhost
Source Server Version : 50621
Source Host           : localhost:3306
Source Database       : yiletong

Target Server Type    : MYSQL
Target Server Version : 50621
File Encoding         : 65001

Date: 2015-07-29 01:36:29
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
  `bankName3` varchar(64) DEFAULT NULL,
  `sourceAccount` varchar(256) DEFAULT NULL,
  `lastQuery` varchar(10) DEFAULT NULL COMMENT '最后一次查询二清的时间',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of customers
-- ----------------------------
INSERT INTO `customers` VALUES ('1', '65878132', '詹彦城', '鼓楼区新中源陶瓷', '17097913883', '0.38', '0', '0.2', '0.2', '中国农业银行', '福建分行', '5187100012525939', '1', '福建', '福州', '鼓楼支行', null, '2015-07-29');
INSERT INTO `customers` VALUES ('2', '65734831', '林沈午', '永安市公路港通讯店', '17097913883', '0.38', '0', '0.2', '0.2', '中国农业银行', '福建分行', '5187100012525939', '1', '福建', '福州', '鼓楼支行', null, '2015-07-29');
INSERT INTO `customers` VALUES ('3', '65804054', '周水发', '台江区见亿食品商店', '17097913883', '0.38', '0', '0.2', '0.25', '中国农业银行', '福建分行', '5187100012525939', '1', '福建', '福州', '鼓楼支行', null, '2015-07-29');
INSERT INTO `customers` VALUES ('4', '66014565', '兰李琴', '福州市鼓楼区华安保险', '17097913883', '0.38', '0', '0.2', '0.2', '中国农业银行', '福建分行', '5187100012525939', '1', '福建', '福州', '鼓楼支行', null, '2015-07-29');
INSERT INTO `customers` VALUES ('5', '65810230', '姚德顺', '台江区永德家用电器商行', '17097913883', '0.4', '0', '0.2', '0.2', '中国农业银行', '福建分行', '5187100012525939', '1', '福建', '福州', '鼓楼支行', null, '2015-07-29');
INSERT INTO `customers` VALUES ('6', '65774305', '胡国平', '台江区黄鹤楼家用电器商行', '17097913883', '0.38', '0', '0.2', '0.2', '中国农业银行', '福建分行', '5187100012525939', '1', '福建', '福州', '鼓楼支行', null, '2015-07-29');
INSERT INTO `customers` VALUES ('8', '12341234', '1234', '12341234', '12341234', '1234', null, '0.2', '1212', '中国农业银行', '福建分行', '5187100012525939', '-1', '', null, '鼓楼支行', null, null);
INSERT INTO `customers` VALUES ('9', '65736970', '朱坤守', '海乐迪量贩式KTV', '13788598652', '0.38', null, '0.2', '0.2', '中国民生银行', '福建省福州市分行', '6226221501907114', '1', '福建', '福州', '台江区中亭街支行', '严时丽民生622255555555', '2015-07-29');
INSERT INTO `customers` VALUES ('10', '65888302', '朱坤守', '鼓楼区五洲通航空票务', '1355555555', '0.38', null, '0.1', '0.2', '中国民生银行', '福建省分行', '6226221501907114', '1', '福建', '福州', '福建省福州市台江区中亭街支行', '兰苏清 (6214835910968039)	', '2015-07-29');

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
INSERT INTO `erp_sequence` VALUES ('transactionLogs', '3134');
INSERT INTO `erp_sequence` VALUES ('customers', '10');
INSERT INTO `erp_sequence` VALUES ('transactionSum', '82');

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
  `finallyMoney` float(10,2) DEFAULT NULL,
  `cards` text,
  `jiesuantime` text,
  `jiesuanstatus` int(11) DEFAULT '1' COMMENT '记录标志，非结单标志',
  `record` varchar(255) DEFAULT NULL,
  `timeStr` text COMMENT '抓取来的时间格式',
  `resultCode` varchar(2) DEFAULT NULL COMMENT '结果代码',
  `discountMoney` float(4,2) DEFAULT NULL COMMENT 'pos机扣率收费',
  `tixianfeiMoney` float(4,2) DEFAULT NULL COMMENT 'T0收费',
  `status` int(11) DEFAULT NULL,
  `sumid` int(11) DEFAULT NULL COMMENT '结算编号',
  `isValid` int(1) DEFAULT '0',
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
  `results` text,
  `createDate` text,
  `uploadDate` text,
  `reciveDate` text,
  `batchAmount` int(11) DEFAULT NULL COMMENT '笔数',
  `bankName` text,
  `bankName2` varchar(64) DEFAULT NULL,
  `bankName3` varchar(64) DEFAULT NULL,
  `province` varchar(20) DEFAULT NULL,
  `city` varchar(20) DEFAULT NULL,
  `tel` varchar(20) DEFAULT NULL,
  `batchCurrnum` varchar(20) DEFAULT NULL,
  `batchCount` int(4) DEFAULT NULL,
  `faren` varchar(50) DEFAULT NULL,
  `shanghuName` varchar(64) DEFAULT NULL,
  `bankAccount` varchar(32) DEFAULT NULL,
  `sourceAccount` varchar(256) DEFAULT NULL
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
