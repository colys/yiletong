/*
Navicat MySQL Data Transfer

Source Server         : localhost
Source Server Version : 50624
Source Host           : 127.0.0.1:3306
Source Database       : yiletong

Target Server Type    : MYSQL
Target Server Version : 50624
File Encoding         : 65001

Date: 2015-08-22 10:44:02
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for customers
-- ----------------------------
DROP TABLE IF EXISTS `customers`;
CREATE TABLE `customers` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
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
  `eachMin` float DEFAULT '0' COMMENT '单笔下限',
  `eachMax` float DEFAULT '0' COMMENT '单笔上限',
  `dayMin` float DEFAULT '0' COMMENT '当日累计下限',
  `dayMax` float DEFAULT '0' COMMENT '当日累计上限',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=14 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of customers
-- ----------------------------
INSERT INTO `customers` VALUES ('1', '65878132', '詹彦城', '鼓楼区新中源陶瓷', '17097913883', '0', '0', '0', '0', '中国农业银行', '福建分行', '5187100012525939', '-1', '福建', '福州', '鼓楼支行', '15', '2015-08-01', '0', '1', '500', '50000', '500', '200000');
INSERT INTO `customers` VALUES ('2', '65734831', '林沈午', '永安市公路港通讯店', '17097913883', '0.38', '0', '0.2', '0.2', '中国农业银行', '福建分行', '5187100012525939', '-1', '福建', '福州', '鼓楼支行', '13', '2015-08-01', '0', '1', '500', '50000', '500', '200000');
INSERT INTO `customers` VALUES ('3', '65804054', '周水发', '台江区见亿食品商店', '17097913883', '0.38', '0', '0.2', '0.25', '中国农业银行', '福建分行', '5187100012525939', '0', '福建', '福州', '鼓楼支行', '13', '2015-08-22', '0', '0', '500', '50000', '500', '200000');
INSERT INTO `customers` VALUES ('4', '66014565', '兰李琴', '福州市鼓楼区华安保险', '17097913883', '0.38', '0', '0.2', '0.2', '中国农业银行', '福建分行', '5187100012525939', '-1', '福建', '福州', '鼓楼支行', '13', '2015-08-01', '0', '0', '500', '50000', '500', '200000');
INSERT INTO `customers` VALUES ('5', '65810230', '姚德顺', '台江区永德家用电器商行', '17097913883', '0.4', '0', '0.2', '0.2', '中国农业银行', '福建分行', '5187100012525939', '-1', '福建', '福州', '鼓楼支行', '13', '2015-08-01', '0', '0', '500', '50000', '500', '200000');
INSERT INTO `customers` VALUES ('6', '65774305', '胡国平', '台江区黄鹤楼家用电器商行', '17097913883', '0.38', '0', '0.2', '0.2', '中国农业银行', '福建分行', '5187100012525939', '-1', '福建', '福州', '鼓楼支行', '13', '2015-08-01', '0', '0', '500', '50000', '500', '200000');
INSERT INTO `customers` VALUES ('8', '12341234', '1234', '12341234', '12341234', '1234', '0', '0.2', '0.2', '中国农业银行', '福建分行', '5187100012525939', '-1', '', null, '鼓楼支行', '13', '2015-08-01', '0', '0', '500', '50000', '500', '200000');
INSERT INTO `customers` VALUES ('9', '65736970', '朱坤守', '海乐迪量贩式KTV', '13788598652', '0.38', '0', '0.2', '0.2', '中国民生银行', '福州市分行', '6226221501907114', '1', '福建', '福州', '台江区中亭街支行', '13', '2015-08-22', '0', '0', '500', '50000', '500', '200000');
INSERT INTO `customers` VALUES ('10', '65888302', '朱坤守', '鼓楼区五洲通航空票务', '13817941672', '0.38', '0', '0.1', '0.2', '中国民生银行', '福建省分行', '6226221501907114', '0', '福建', '福州', '福建省福州市台江区中亭街支行', '14', '2015-08-22', '0', '0', '500', '50000', '500', '200000');
INSERT INTO `customers` VALUES ('11', '66032367', '谢宜宝', '霞浦县立平装修材料经营部', '18059195136', '0.78', '40', '0.1', '0.2', '兴业银行', '福建省分行', '622908133445969812', '1', '福建', '宁德', '宁德市霞浦支行', '13', '2015-08-22', '0', '0', '500', '50000', '500', '200000');
INSERT INTO `customers` VALUES ('12', '66032367', '谢宜宝', '立平装修材料', '13385055887', '0.78', '40', '10', '20', '兴业银行', '福建省分行', '622908133445969812', '-1', '福建', '宁德', '福建省宁德市霞浦支行', '13', '2015-08-22', '1', '0', '500', '50000', '500', '200000');
INSERT INTO `customers` VALUES ('13', '123', '123', '123', '2', '2', '22', '2', '2', '2', '2', '2', '-1', '2', '2', '2', '13', '2015-08-01', '1', '1', '500', '50000', '500', '200000');

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
INSERT INTO `erp_sequence` VALUES ('transactionLogs', '4212');
INSERT INTO `erp_sequence` VALUES ('customers', '10');
INSERT INTO `erp_sequence` VALUES ('transactionSum', '119');

-- ----------------------------
-- Table structure for sourceaccounts
-- ----------------------------
DROP TABLE IF EXISTS `sourceaccounts`;
CREATE TABLE `sourceaccounts` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `faren` varchar(20) NOT NULL,
  `bankName` varchar(32) NOT NULL,
  `bankAccount` varchar(32) NOT NULL,
  `status` int(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=16 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of sourceaccounts
-- ----------------------------
INSERT INTO `sourceaccounts` VALUES ('13', '吕存库', '招商银行', '123412342143', '0');
INSERT INTO `sourceaccounts` VALUES ('14', '灵', '1234', '234243', '0');
INSERT INTO `sourceaccounts` VALUES ('15', '兰晖', '兴业银行股份有限公司霞浦支行', '622908133564338419', '0');

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
INSERT INTO `transactionlogs` VALUES ('4181', '65878132', '消费', '交易成功', '鼓楼区新中源陶瓷', '詹彦城', '49776.00', '2015-08-10 13:03:25', '49487.30', null, null, '1', null, '20150810 130325', '00', '99.99', '99.55', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('4182', '65878132', '消费', '交易成功', '鼓楼区新中源陶瓷', '詹彦城', '10000.00', '2015-08-10 19:39:53', '9942.00', null, null, '1', null, '20150810 193953', '00', '38.00', '20.00', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('4183', '65878132', '消费', '交易成功', '鼓楼区新中源陶瓷', '詹彦城', '59998.00', '2015-08-11 16:12:23', '59650.01', null, null, '1', null, '20150811 161223', '00', '99.99', '99.99', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('4184', '65878132', '消费', '交易成功', '鼓楼区新中源陶瓷', '詹彦城', '69999.00', '2015-08-11 16:48:56', '69593.01', null, null, '1', null, '20150811 164856', '00', '99.99', '99.99', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('4185', '65878132', '消费', '交易成功', '鼓楼区新中源陶瓷', '詹彦城', '49999.00', '2015-08-11 23:05:17', '49709.01', null, null, '1', null, '20150811 230517', '00', '99.99', '99.99', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('4186', '65878132', '消费', '交易成功', '鼓楼区新中源陶瓷', '詹彦城', '20000.00', '2015-08-11 23:25:40', '19884.00', null, null, '1', null, '20150811 232540', '00', '76.00', '40.00', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('4187', '65878132', '消费', '账户内余额不足', '鼓楼区新中源陶瓷', '詹彦城', '25210.00', '2015-08-13 11:15:18', '25063.78', null, null, '1', null, '20150813 111518', '51', '95.80', '50.42', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('4188', '65878132', '查询余额', '受限制的卡', '鼓楼区新中源陶瓷', '詹彦城', '0.00', '2015-08-13 11:15:57', '0.00', null, null, '1', null, '20150813 111557', '62', '0.00', '0.00', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('4189', '65878132', '消费', '受限制的卡', '鼓楼区新中源陶瓷', '詹彦城', '19999.00', '2015-08-13 11:17:07', '19883.01', null, null, '1', null, '20150813 111707', '62', '76.00', '39.99', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('4190', '65878132', '消费', '交易成功', '鼓楼区新中源陶瓷', '詹彦城', '25210.00', '2015-08-13 11:19:07', '25063.78', null, null, '1', null, '20150813 111907', '00', '95.80', '50.42', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('4191', '65878132', '消费', '密码输错', '鼓楼区新中源陶瓷', '詹彦城', '25210.00', '2015-08-13 11:20:09', '25063.78', null, null, '1', null, '20150813 112009', '55', '95.80', '50.42', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('4192', '65878132', '消费', '交易成功', '鼓楼区新中源陶瓷', '詹彦城', '25210.00', '2015-08-13 11:20:30', '25063.78', null, null, '1', null, '20150813 112030', '00', '95.80', '50.42', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('4193', '65878132', '消费', '受限制的卡', '鼓楼区新中源陶瓷', '詹彦城', '25210.00', '2015-08-13 11:27:13', '25063.78', null, null, '1', null, '20150813 112713', '62', '95.80', '50.42', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('4194', '65878132', '消费', '账户内余额不足', '鼓楼区新中源陶瓷', '詹彦城', '25210.00', '2015-08-13 11:30:28', '25063.78', null, null, '1', null, '20150813 113028', '51', '95.80', '50.42', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('4195', '65878132', '消费', '账户内余额不足', '鼓楼区新中源陶瓷', '詹彦城', '25210.00', '2015-08-13 11:31:44', '25063.78', null, null, '1', null, '20150813 113144', '51', '95.80', '50.42', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('4196', '65878132', '消费', '交易成功', '鼓楼区新中源陶瓷', '詹彦城', '0.01', '2015-08-13 18:44:48', '0.01', null, null, '1', null, '20150813 184448', '00', '0.00', '0.00', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('4197', '65878132', '消费', '金额太大', '鼓楼区新中源陶瓷', '詹彦城', '60001.00', '2015-08-13 18:45:31', '59652.99', null, null, '1', null, '20150813 184531', '61', '99.99', '99.99', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('4198', '65878132', '消费', '金额太大', '鼓楼区新中源陶瓷', '詹彦城', '59999.00', '2015-08-13 18:46:17', '59651.01', null, null, '1', null, '20150813 184617', '61', '99.99', '99.99', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('4199', '65878132', '消费', '交易成功', '鼓楼区新中源陶瓷', '詹彦城', '30001.00', '2015-08-13 18:46:55', '29826.99', null, null, '1', null, '20150813 184655', '00', '99.99', '60.01', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('4200', '65878132', '消费', '受限制的卡', '鼓楼区新中源陶瓷', '詹彦城', '29999.00', '2015-08-13 18:47:35', '29825.01', null, null, '1', null, '20150813 184735', '62', '99.99', '59.99', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('4201', '65878132', '消费', '受限制的卡', '鼓楼区新中源陶瓷', '詹彦城', '19998.00', '2015-08-13 18:48:36', '19882.01', null, null, '1', null, '20150813 184836', '62', '75.99', '40.00', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('4202', '65878132', '消费', '金额太大', '鼓楼区新中源陶瓷', '詹彦城', '20000.00', '2015-08-13 18:50:19', '19884.00', null, null, '1', null, '20150813 185019', '61', '76.00', '40.00', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('4203', '65736970', '签到双倍密钥', '交易成功', '海乐迪量贩式KTV', '朱坤守', '0.00', '2015-08-12 13:27:18', '0.00', null, null, '1', null, '20150812 132718', '00', '0.00', '0.00', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('4204', '65736970', '消费', '交易成功', '海乐迪量贩式KTV', '朱坤守', '1200.00', '2015-08-12 13:28:36', '1193.04', null, null, '1', null, '20150812 132836', '00', '4.56', '2.40', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('4205', '65736970', '消费', '交易成功', '海乐迪量贩式KTV', '朱坤守', '500.00', '2015-08-12 13:54:50', '497.10', null, null, '1', null, '20150812 135450', '00', '1.90', '1.00', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('4206', '65736970', '消费', '交易成功', '海乐迪量贩式KTV', '朱坤守', '800.00', '2015-08-12 14:04:06', '795.36', null, null, '1', null, '20150812 140406', '00', '3.04', '1.60', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('4207', '65888302', '消费', '交易成功', '鼓楼区五洲通航空票务', '朱坤守', '5300.00', '2015-08-12 18:23:43', '5269.26', null, null, '1', null, '20150812 182343', '00', '20.14', '10.60', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('4208', '65888302', '消费', '交易成功', '鼓楼区五洲通航空票务', '朱坤守', '9288.00', '2015-08-12 18:24:26', '9234.13', null, null, '1', null, '20150812 182426', '00', '35.29', '18.58', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('4209', '65888302', '消费', '交易成功', '鼓楼区五洲通航空票务', '朱坤守', '9230.00', '2015-08-12 18:24:53', '9176.47', null, null, '1', null, '20150812 182453', '00', '35.07', '18.46', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('4210', '65888302', '消费', '交易成功', '鼓楼区五洲通航空票务', '朱坤守', '10000.00', '2015-08-13 17:39:42', '9942.00', null, null, '1', null, '20150813 173942', '00', '38.00', '20.00', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('4211', '65888302', '消费', '交易成功', '鼓楼区五洲通航空票务', '朱坤守', '2000.00', '2015-08-13 17:40:03', '1988.40', null, null, '1', null, '20150813 174003', '00', '7.60', '4.00', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('4212', '65888302', '消费', '交易成功', '鼓楼区五洲通航空票务', '朱坤守', '10000.00', '2015-08-13 17:41:33', '9942.00', null, null, '1', null, '20150813 174133', '00', '38.00', '20.00', '0', null, '1');

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
