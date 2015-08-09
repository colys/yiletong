/*
Navicat MySQL Data Transfer

Source Server         : 192.168.1.119
Source Server Version : 50624
Source Host           : 192.168.1.119:3306
Source Database       : yiletong

Target Server Type    : MYSQL
Target Server Version : 50624
File Encoding         : 65001

Date: 2015-08-09 12:10:24
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for `customers`
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
  `IsFengDing` int(1) DEFAULT NULL,
  `frozen` int(1) DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of customers
-- ----------------------------
INSERT INTO `customers` VALUES ('1', '65878132', '詹彦城', '鼓楼区新中源陶瓷', '17097913883', '0.38', '35', '0.2', '0.2', '中国农业银行', '福建分行', '5187100012525939', '1', '福建', '福州', '鼓楼支行', null, '2015-08-09', '0', '0');
INSERT INTO `customers` VALUES ('2', '65734831', '林沈午', '永安市公路港通讯店', '17097913883', '0.38', '0', '0.2', '0.2', '中国农业银行', '福建分行', '5187100012525939', '-1', '福建', '福州', '鼓楼支行', null, '2015-08-08', '0', '0');
INSERT INTO `customers` VALUES ('3', '65804054', '周水发', '台江区见亿食品商店', '17097913883', '0.38', '0', '0.2', '0.25', '中国农业银行', '福建分行', '5187100012525939', '-1', '福建', '福州', '鼓楼支行', null, '2015-08-08', '0', '0');
INSERT INTO `customers` VALUES ('4', '66014565', '兰李琴', '福州市鼓楼区华安保险', '17097913883', '0.38', '0', '0.2', '0.2', '中国农业银行', '福建分行', '5187100012525939', '-1', '福建', '福州', '鼓楼支行', null, '2015-08-08', '0', '0');
INSERT INTO `customers` VALUES ('5', '65810230', '姚德顺', '台江区永德家用电器商行', '17097913883', '0.4', '0', '0.2', '0.2', '中国农业银行', '福建分行', '5187100012525939', '-1', '福建', '福州', '鼓楼支行', null, '2015-08-08', '0', '0');
INSERT INTO `customers` VALUES ('6', '65774305', '胡国平', '台江区黄鹤楼家用电器商行', '17097913883', '0.38', '0', '0.2', '0.2', '中国农业银行', '福建分行', '5187100012525939', '-1', '福建', '福州', '鼓楼支行', null, '2015-08-08', '0', '0');
INSERT INTO `customers` VALUES ('8', '12341234', '1234', '12341234', '12341234', '1234', '0', '0.2', '0.2', '中国农业银行', '福建分行', '5187100012525939', '-1', '', null, '鼓楼支行', null, null, '0', '0');
INSERT INTO `customers` VALUES ('9', '65736970', '朱坤守', '海乐迪量贩式KTV', '13788598652', '0.38', '0', '0.2', '0.2', '中国民生银行', '福州市分行', '6226221501907114', '1', '福建', '福州', '台江区中亭街支行', '严时丽民生622255555555', '2015-08-09', '0', '0');
INSERT INTO `customers` VALUES ('10', '65888302', '朱坤守', '鼓楼区五洲通航空票务', '1355555555', '0.38', '0', '0.1', '0.2', '中国民生银行', '福建省分行', '6226221501907114', '1', '福建', '福州', '福建省福州市台江区中亭街支行', '兰苏清 (6214835910968039)	', '2015-08-09', '0', '0');

-- ----------------------------
-- Table structure for `erp_sequence`
-- ----------------------------
DROP TABLE IF EXISTS `erp_sequence`;
CREATE TABLE `erp_sequence` (
  `tableName` text,
  `Val` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of erp_sequence
-- ----------------------------
INSERT INTO `erp_sequence` VALUES ('transactionLogs', '4180');
INSERT INTO `erp_sequence` VALUES ('customers', '10');
INSERT INTO `erp_sequence` VALUES ('transactionSum', '119');

-- ----------------------------
-- Table structure for `sourceaccounts`
-- ----------------------------
DROP TABLE IF EXISTS `sourceaccounts`;
CREATE TABLE `sourceaccounts` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `faren` varchar(20) NOT NULL,
  `bankName` varchar(32) NOT NULL,
  `bankAccount` varchar(32) NOT NULL,
  `status` int(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=15 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of sourceaccounts
-- ----------------------------
INSERT INTO `sourceaccounts` VALUES ('13', '吕存库', '招商银行', '123412342143', '0');
INSERT INTO `sourceaccounts` VALUES ('14', '灵', '1234', '234243', '0');

-- ----------------------------
-- Table structure for `transactionlogs`
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
INSERT INTO `transactionlogs` VALUES ('4148', '65804054', '??', '????', '?????????', '中文', '2000.00', '2015-08-07 23:59:33', '1983.40', null, null, '1', null, '20150807 235933', '00', '7.60', '9.00', '1', '119', '1');
INSERT INTO `transactionlogs` VALUES ('4149', '65804054', '??', '????', '?????????', '中文', '0.00', '2015-08-08 01:16:55', '0.00', null, null, '1', null, '20150808 011655', '00', '0.00', '0.00', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('4150', '65804054', '中茜(????)', '????', '?????????', '中文', '1000.00', '2015-08-08 01:16:58', '987.70', null, null, '1', null, '20150808 011658', '00', '3.80', '8.50', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('4151', '65804054', 'IC????????(????)', '????', '?????????', '中文', '1500.00', '2015-08-08 01:17:01', '1478.55', null, null, '1', null, '20150808 011701', '00', '5.70', '15.75', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('4152', '65804054', 'IC????????(????)', '????', '?????????', '中文', '294.00', '2015-08-08 01:18:24', '289.21', null, null, '1', null, '20150808 011824', '00', '1.12', '3.67', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('4153', '65804054', 'IC????????(????)', '????', '?????????', '中文', '1500.00', '2015-08-08 01:18:27', '1472.55', null, null, '1', null, '20150808 011827', '00', '5.70', '21.75', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('4154', '65804054', 'IC????????(????)', '????', '?????????', '中文', '1500.00', '2015-08-08 01:18:32', '1469.55', null, null, '1', null, '20150808 011832', '00', '5.70', '24.75', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('4155', '65804054', 'IC????????(????)', '????', '?????????', '中文', '100.00', '2015-08-08 01:18:36', '97.77', null, null, '1', null, '20150808 011836', '00', '0.38', '1.85', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('4156', '65804054', 'IC????????(????)', '????', '?????????', '中文', '2000.00', '2015-08-08 01:18:39', '1951.40', null, null, '1', null, '20150808 011839', '00', '7.60', '41.00', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('4157', '65804054', '?????(??)', '????', '?????????', '中文', '0.00', '2015-08-08 01:18:43', '0.00', null, null, '1', null, '20150808 011843', '00', '0.00', '0.00', '1', '119', '0');
INSERT INTO `transactionlogs` VALUES ('4158', '65804054', '??????', '????', '?????????', '中文', '0.00', '2015-08-08 10:59:15', '0.00', null, null, '1', null, '20150808 105915', '00', '0.00', '0.00', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('4159', '65804054', '??', '????', '?????????', '中文', '1000.00', '2015-08-08 20:17:00', '969.70', null, null, '1', null, '20150808 201700', '00', '3.80', '26.50', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('4160', '65804054', '??', '????', '?????????', '中文', '200.00', '2015-08-08 20:17:46', '193.54', null, null, '1', null, '20150808 201746', '00', '0.76', '5.70', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('4161', '65804054', '??', '????', '?????????', '中文', '800.00', '2015-08-08 20:19:29', '772.56', null, null, '1', null, '20150808 201929', '00', '3.04', '24.40', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('4162', '65810230', '??', '????', '???????????', '中文', '1480.00', '2015-08-07 23:09:22', '1468.16', null, null, '1', null, '20150807 230922', '00', '5.92', '5.92', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('4163', '65810230', '??', '????', '???????????', '中文', '2000.00', '2015-08-08 19:19:56', '1980.00', null, null, '1', null, '20150808 191956', '00', '8.00', '12.00', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('4164', '65774305', '??', '????', '????????????', '中文', '0.00', '2015-08-08 10:23:51', '0.00', null, null, '1', null, '20150808 102351', '00', '0.00', '0.00', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('4165', '65774305', 'IC????????(????)', '????', '????????????', '中文', '201.00', '2015-08-08 10:23:56', '199.03', null, null, '1', null, '20150808 102356', '00', '0.76', '1.21', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('4166', '65774305', 'IC????????(????)', '????', '????????????', '中文', '94.00', '2015-08-08 10:24:01', '92.89', null, null, '1', null, '20150808 102401', '00', '0.36', '0.75', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('4167', '65774305', 'IC????????(????)', '????', '????????????', '中文', '102.00', '2015-08-08 10:24:06', '100.59', null, null, '1', null, '20150808 102406', '00', '0.39', '1.02', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('4168', '65774305', 'IC????????(????)', '????', '????????????', '中文', '320.00', '2015-08-08 10:24:10', '314.94', null, null, '1', null, '20150808 102410', '00', '1.22', '3.84', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('4169', '65774305', 'IC????????(????)', '????', '????????????', '中文', '195.00', '2015-08-08 10:24:14', '191.53', null, null, '1', null, '20150808 102414', '00', '0.74', '2.73', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('4170', '65774305', 'IC????????(????)', '????', '????????????', '中文', '445.00', '2015-08-08 10:24:19', '436.19', null, null, '1', null, '20150808 102419', '00', '1.69', '7.12', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('4171', '65774305', 'IC????????(????)', '????', '????????????', '中文', '161.00', '2015-08-08 10:24:24', '157.49', null, null, '1', null, '20150808 102424', '00', '0.61', '2.90', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('4172', '65774305', 'IC????????(????)', '????', '????????????', '中文', '120.00', '2015-08-08 10:24:28', '117.14', null, null, '1', null, '20150808 102428', '00', '0.46', '2.40', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('4173', '65774305', 'IC????????(????)', '????', '????????????', '中文', '665.00', '2015-08-08 10:24:32', '647.84', null, null, '1', null, '20150808 102432', '00', '2.53', '14.63', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('4174', '65774305', '?????(??)', '????', '????????????', '中文', '0.00', '2015-08-08 10:24:36', '0.00', null, null, '1', null, '20150808 102436', '00', '0.00', '0.00', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('4175', '65774305', '??', '????', '????????????', '中文', '0.00', '2015-08-08 10:24:41', '0.00', null, null, '1', null, '20150808 102441', '00', '0.00', '0.00', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('4176', '65774305', '??????', '????', '????????????', '中文', '0.00', '2015-08-08 10:27:51', '0.00', null, null, '1', null, '20150808 102751', '00', '0.00', '0.00', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('4177', '65774305', '??', '????', '????????????', '中文', '225.00', '2015-08-08 14:26:34', '217.39', null, null, '1', null, '20150808 142634', '00', '0.86', '6.75', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('4178', '65888302', '??', '????', '??????????', '中文', '9800.00', '2015-08-08 15:34:57', '9733.36', null, null, '1', null, '20150808 153457', '00', '37.24', '29.40', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('4179', '65804054', 'IC????????(????)', '????', '?????????', '中文', '1000.00', '2015-08-08 01:16:58', '991.70', null, null, '1', null, '20150808 011658', '00', '3.80', '4.50', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('4180', '65888302', '消费', '交易成功', '鼓楼区五洲通航空票务', '朱坤守', '9800.00', '2015-08-08 15:34:57', '9733.36', null, null, '1', null, '20150808 153457', '00', '37.24', '29.40', '0', null, '1');

-- ----------------------------
-- Table structure for `transactionsum`
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
INSERT INTO `transactionsum` VALUES ('119', '65804054', '2000', '9', '7.6', '1983.4', '2', '?????', '2015-08-08 22:49:19', '2015-08-08 22:49:39', null, null, '??????', '????', '????', '??', '??', '17097913883', '201508082249395676', '1', '???', '?????????', '5187100012525939', '');

-- ----------------------------
-- Table structure for `users`
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
