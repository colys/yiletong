/*
Navicat MySQL Data Transfer

Source Server         : localhost
Source Server Version : 50621
Source Host           : localhost:3306
Source Database       : yiletong

Target Server Type    : MYSQL
Target Server Version : 50621
File Encoding         : 65001

Date: 2015-07-31 00:59:43
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
  `IsFengDing` int(1) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of customers
-- ----------------------------
INSERT INTO `customers` VALUES ('1', '65878132', '詹彦城', '鼓楼区新中源陶瓷', '17097913883', '0.38', '35', '0.2', '0.2', '中国农业银行', '福建分行', '5187100012525939', '1', '福建', '福州', '鼓楼支行', null, '2015-07-30', '0');
INSERT INTO `customers` VALUES ('2', '65734831', '林沈午', '永安市公路港通讯店', '17097913883', '0.38', '0', '0.2', '0.2', '中国农业银行', '福建分行', '5187100012525939', '1', '福建', '福州', '鼓楼支行', null, '2015-07-30', '0');
INSERT INTO `customers` VALUES ('3', '65804054', '周水发', '台江区见亿食品商店', '17097913883', '0.38', '0', '0.2', '0.25', '中国农业银行', '福建分行', '5187100012525939', '1', '福建', '福州', '鼓楼支行', null, '2015-07-30', '0');
INSERT INTO `customers` VALUES ('4', '66014565', '兰李琴', '福州市鼓楼区华安保险', '17097913883', '0.38', '0', '0.2', '0.2', '中国农业银行', '福建分行', '5187100012525939', '1', '福建', '福州', '鼓楼支行', null, '2015-07-30', '0');
INSERT INTO `customers` VALUES ('5', '65810230', '姚德顺', '台江区永德家用电器商行', '17097913883', '0.4', '0', '0.2', '0.2', '中国农业银行', '福建分行', '5187100012525939', '1', '福建', '福州', '鼓楼支行', null, '2015-07-30', '0');
INSERT INTO `customers` VALUES ('6', '65774305', '胡国平', '台江区黄鹤楼家用电器商行', '17097913883', '0.38', '0', '0.2', '0.2', '中国农业银行', '福建分行', '5187100012525939', '1', '福建', '福州', '鼓楼支行', null, '2015-07-30', '0');
INSERT INTO `customers` VALUES ('8', '12341234', '1234', '12341234', '12341234', '1234', '0', '0.2', '0.2', '中国农业银行', '福建分行', '5187100012525939', '-1', '', null, '鼓楼支行', null, null, '0');
INSERT INTO `customers` VALUES ('9', '65736970', '朱坤守', '海乐迪量贩式KTV', '13788598652', '0.38', '0', '0.2', '0.2', '中国民生银行', '福州市分行', '6226221501907114', '1', '福建', '福州', '台江区中亭街支行', '严时丽民生622255555555', '2015-07-30', '0');
INSERT INTO `customers` VALUES ('10', '65888302', '朱坤守', '鼓楼区五洲通航空票务', '1355555555', '0.38', '0', '0.1', '0.2', '中国民生银行', '福建省分行', '6226221501907114', '1', '福建', '福州', '福建省福州市台江区中亭街支行', '兰苏清 (6214835910968039)	', '2015-07-30', '0');

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
INSERT INTO `erp_sequence` VALUES ('transactionLogs', '3727');
INSERT INTO `erp_sequence` VALUES ('customers', '10');
INSERT INTO `erp_sequence` VALUES ('transactionSum', '101');

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
INSERT INTO `transactionlogs` VALUES ('3567', '65878132', '消费', '交易成功', '鼓楼区新中源陶瓷', '詹彦城', '99998.00', '2015-07-29 23:27:46', '99418.01', null, null, '1', null, '20150729 232746', '0', '99.99', '99.99', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('3568', '65878132', '消费', '交易成功', '鼓楼区新中源陶瓷', '詹彦城', '81300.00', '2015-07-29 23:37:23', '80828.46', null, null, '1', null, '20150729 233723', '0', '99.99', '99.99', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('3569', '65878132', '消费', '交易成功', '鼓楼区新中源陶瓷', '詹彦城', '4050.00', '2015-07-30 15:27:15', '4026.51', null, null, '1', null, '20150730 152715', '0', '15.39', '8.10', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('3570', '65878132', '消费', '交易成功', '鼓楼区新中源陶瓷', '詹彦城', '4050.00', '2015-07-30 15:27:57', '4026.51', null, null, '1', null, '20150730 152757', '0', '15.39', '8.10', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('3571', '65734831', '消费', '交易成功', '永安市公路港通讯店', '林沈午', '236.00', '2015-07-29 23:16:31', '234.63', null, null, '1', null, '20150729 231631', '0', '0.90', '0.47', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('3572', '65734831', '消费', '交易成功', '永安市公路港通讯店', '林沈午', '300.00', '2015-07-30 00:10:24', '298.26', null, null, '1', null, '20150730 001024', '0', '1.14', '0.60', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('3573', '65734831', '消费', '交易成功', '永安市公路港通讯店', '林沈午', '300.00', '2015-07-30 00:37:19', '298.26', null, null, '1', null, '20150730 003719', '0', '1.14', '0.60', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('3574', '65734831', '消费', '交易成功', '永安市公路港通讯店', '林沈午', '300.00', '2015-07-30 11:03:02', '298.26', null, null, '1', null, '20150730 110302', '0', '1.14', '0.60', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('3575', '65734831', '消费', '交易成功', '永安市公路港通讯店', '林沈午', '118.00', '2015-07-30 12:08:43', '117.31', null, null, '1', null, '20150730 120843', '0', '0.45', '0.24', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('3576', '65734831', '查询余额', '交易成功', '永安市公路港通讯店', '林沈午', '0.00', '2015-07-30 12:10:49', '0.00', null, null, '1', null, '20150730 121049', '0', '0.00', '0.00', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('3577', '65734831', '查询余额', '交易成功', '永安市公路港通讯店', '林沈午', '0.00', '2015-07-30 12:11:59', '0.00', null, null, '1', null, '20150730 121159', '0', '0.00', '0.00', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('3578', '65804054', '消费', '交易成功', '台江区见亿食品商店', '周水发', '106.00', '2015-07-30 00:13:54', '105.33', null, null, '1', null, '20150730 001354', '0', '0.40', '0.27', '1', '88', '1');
INSERT INTO `transactionlogs` VALUES ('3579', '65804054', '结算', '交易成功', '台江区见亿食品商店', '周水发', '0.00', '2015-07-30 00:50:27', '0.00', null, null, '1', null, '20150730 005027', '0', '0.00', '0.00', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('3580', '65804054', 'IC卡批上送通知交易(联机平账)', '交易成功', '台江区见亿食品商店', '周水发', '405.00', '2015-07-30 00:50:31', '402.45', null, null, '1', null, '20150730 005031', '0', '1.54', '1.01', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('3581', '65804054', 'IC卡批上送通知交易(联机平账)', '交易成功', '台江区见亿食品商店', '周水发', '106.00', '2015-07-30 00:50:34', '105.33', null, null, '1', null, '20150730 005034', '0', '0.40', '0.27', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('3582', '65804054', '批上送结束(平账)', '交易成功', '台江区见亿食品商店', '周水发', '0.00', '2015-07-30 00:50:38', '0.00', null, null, '1', null, '20150730 005038', '0', '0.00', '0.00', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('3583', '65804054', '签退', '交易成功', '台江区见亿食品商店', '周水发', '0.00', '2015-07-30 00:50:42', '0.00', null, null, '1', null, '20150730 005042', '0', '0.00', '0.00', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('3584', '65804054', '签到双倍密钥', '交易成功', '台江区见亿食品商店', '周水发', '0.00', '2015-07-30 11:17:53', '0.00', null, null, '1', null, '20150730 111753', '0', '0.00', '0.00', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('3585', '65804054', '消费', '交易成功', '台江区见亿食品商店', '周水发', '58.00', '2015-07-30 19:05:38', '57.63', null, null, '1', null, '20150730 190538', '0', '0.22', '0.15', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('3586', '65810230', '消费', '交易成功', '台江区永德家用电器商行', '姚德顺', '550.00', '2015-07-30 20:21:10', '546.70', null, null, '1', null, '20150730 202110', '0', '2.20', '1.10', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('3587', '65810230', '消费', '交易成功', '台江区永德家用电器商行', '姚德顺', '487.00', '2015-07-30 20:25:53', '484.08', null, null, '1', null, '20150730 202553', '0', '1.95', '0.97', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('3588', '65774305', '结算', '交易成功', '台江区黄鹤楼家用电器商行', '胡国平', '0.00', '2015-07-30 09:28:56', '0.00', null, null, '1', null, '20150730 092856', '0', '0.00', '0.00', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('3589', '65774305', 'IC卡批上送通知交易(联机平账)', '交易成功', '台江区黄鹤楼家用电器商行', '胡国平', '155.00', '2015-07-30 09:28:59', '154.10', null, null, '1', null, '20150730 092859', '0', '0.59', '0.31', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('3590', '65774305', 'IC卡批上送通知交易(联机平账)', '交易成功', '台江区黄鹤楼家用电器商行', '胡国平', '408.00', '2015-07-30 09:29:04', '405.63', null, null, '1', null, '20150730 092904', '0', '1.55', '0.82', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('3591', '65774305', 'IC卡批上送通知交易(联机平账)', '交易成功', '台江区黄鹤楼家用电器商行', '胡国平', '179.00', '2015-07-30 09:29:08', '177.96', null, null, '1', null, '20150730 092908', '0', '0.68', '0.36', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('3592', '65774305', 'IC卡批上送通知交易(联机平账)', '交易成功', '台江区黄鹤楼家用电器商行', '胡国平', '102.00', '2015-07-30 09:29:13', '101.41', null, null, '1', null, '20150730 092913', '0', '0.39', '0.20', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('3593', '65774305', '批上送结束(平账)', '交易成功', '台江区黄鹤楼家用电器商行', '胡国平', '0.00', '2015-07-30 09:29:19', '0.00', null, null, '1', null, '20150730 092919', '0', '0.00', '0.00', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('3594', '65774305', '签退', '交易成功', '台江区黄鹤楼家用电器商行', '胡国平', '0.00', '2015-07-30 09:29:23', '0.00', null, null, '1', null, '20150730 092923', '0', '0.00', '0.00', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('3595', '65774305', '签到双倍密钥', '交易成功', '台江区黄鹤楼家用电器商行', '胡国平', '0.00', '2015-07-30 09:29:44', '0.00', null, null, '1', null, '20150730 092944', '0', '0.00', '0.00', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('3596', '65774305', '消费', '交易成功', '台江区黄鹤楼家用电器商行', '胡国平', '159.00', '2015-07-30 12:47:36', '158.08', null, null, '1', null, '20150730 124736', '0', '0.60', '0.32', '1', '93', '1');
INSERT INTO `transactionlogs` VALUES ('3597', '65774305', '消费', '交易成功', '台江区黄鹤楼家用电器商行', '胡国平', '240.00', '2015-07-30 12:48:40', '238.61', null, null, '1', null, '20150730 124840', '0', '0.91', '0.48', '1', '93', '1');
INSERT INTO `transactionlogs` VALUES ('3598', '65774305', '消费', '交易成功', '台江区黄鹤楼家用电器商行', '胡国平', '276.00', '2015-07-30 12:49:45', '274.40', null, null, '1', null, '20150730 124945', '0', '1.05', '0.55', '1', '93', '1');
INSERT INTO `transactionlogs` VALUES ('3599', '65774305', '消费', '交易成功', '台江区黄鹤楼家用电器商行', '胡国平', '300.00', '2015-07-30 12:52:38', '298.26', null, null, '1', null, '20150730 125238', '0', '1.14', '0.60', '1', '93', '1');
INSERT INTO `transactionlogs` VALUES ('3600', '65774305', '消费', '交易成功', '台江区黄鹤楼家用电器商行', '胡国平', '218.00', '2015-07-30 13:18:33', '216.73', null, null, '1', null, '20150730 131833', '0', '0.83', '0.44', '1', '93', '1');
INSERT INTO `transactionlogs` VALUES ('3601', '65774305', '消费', '交易成功', '台江区黄鹤楼家用电器商行', '胡国平', '128.00', '2015-07-30 13:26:20', '127.25', null, null, '1', null, '20150730 132620', '0', '0.49', '0.26', '1', '93', '1');
INSERT INTO `transactionlogs` VALUES ('3602', '65774305', '消费', '交易成功', '台江区黄鹤楼家用电器商行', '胡国平', '93.00', '2015-07-30 17:50:55', '92.46', null, null, '1', null, '20150730 175055', '0', '0.35', '0.19', '1', '93', '1');
INSERT INTO `transactionlogs` VALUES ('3603', '65774305', '消费', '交易成功', '台江区黄鹤楼家用电器商行', '胡国平', '178.00', '2015-07-30 18:54:58', '176.96', null, null, '1', null, '20150730 185458', '0', '0.68', '0.36', '1', '93', '1');
INSERT INTO `transactionlogs` VALUES ('3604', '65774305', '消费', '交易成功', '台江区黄鹤楼家用电器商行', '胡国平', '619.00', '2015-07-30 19:24:23', '615.41', null, null, '1', null, '20150730 192423', '0', '2.35', '1.24', '1', '93', '1');
INSERT INTO `transactionlogs` VALUES ('3605', '65774305', '消费', '交易成功', '台江区黄鹤楼家用电器商行', '胡国平', '180.00', '2015-07-30 19:28:00', '178.96', null, null, '1', null, '20150730 192800', '0', '0.68', '0.36', '1', '93', '1');
INSERT INTO `transactionlogs` VALUES ('3606', '65774305', '消费', '交易成功', '台江区黄鹤楼家用电器商行', '胡国平', '500.00', '2015-07-30 19:35:44', '497.10', null, null, '1', null, '20150730 193544', '0', '1.90', '1.00', '1', '93', '1');
INSERT INTO `transactionlogs` VALUES ('3607', '65774305', '消费', '交易成功', '台江区黄鹤楼家用电器商行', '胡国平', '249.00', '2015-07-30 19:41:24', '247.55', null, null, '1', null, '20150730 194124', '0', '0.95', '0.50', '1', '93', '1');
INSERT INTO `transactionlogs` VALUES ('3608', '65774305', '消费', '交易成功', '台江区黄鹤楼家用电器商行', '胡国平', '87.00', '2015-07-30 20:02:40', '86.50', null, null, '1', null, '20150730 200240', '0', '0.33', '0.17', '1', '93', '1');
INSERT INTO `transactionlogs` VALUES ('3609', '65774305', '消费', '交易成功', '台江区黄鹤楼家用电器商行', '胡国平', '249.00', '2015-07-30 20:12:01', '247.55', null, null, '1', null, '20150730 201201', '0', '0.95', '0.50', '1', '93', '1');
INSERT INTO `transactionlogs` VALUES ('3610', '65774305', '消费', '交易成功', '台江区黄鹤楼家用电器商行', '胡国平', '63.00', '2015-07-30 20:12:29', '62.63', null, null, '1', null, '20150730 201229', '0', '0.24', '0.13', '1', '93', '1');
INSERT INTO `transactionlogs` VALUES ('3611', '65774305', '消费', '交易成功', '台江区黄鹤楼家用电器商行', '胡国平', '127.00', '2015-07-30 20:15:12', '126.27', null, null, '1', null, '20150730 201512', '0', '0.48', '0.25', '1', '93', '1');
INSERT INTO `transactionlogs` VALUES ('3612', '65774305', '消费', '交易成功', '台江区黄鹤楼家用电器商行', '胡国平', '670.00', '2015-07-30 20:15:48', '666.11', null, null, '1', null, '20150730 201548', '0', '2.55', '1.34', '1', '93', '1');
INSERT INTO `transactionlogs` VALUES ('3613', '65774305', '消费', '交易成功', '台江区黄鹤楼家用电器商行', '胡国平', '1050.00', '2015-07-30 21:05:12', '1043.91', null, null, '1', null, '20150730 210512', '0', '3.99', '2.10', '1', '93', '1');
INSERT INTO `transactionlogs` VALUES ('3621', '65888302', '消费', '交易成功', '鼓楼区五洲通航空票务', '朱坤守', '5000.00', '2015-07-30 18:31:36', '4971.00', null, null, '1', null, '20150730 183136', '0', '19.00', '10.00', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('3622', '65888302', '消费', '交易成功', '鼓楼区五洲通航空票务', '朱坤守', '9750.00', '2015-07-30 18:34:44', '9693.45', null, null, '1', null, '20150730 183444', '0', '37.05', '19.50', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('3623', '65804054', '消费', '交易成功', '台江区见亿食品商店', '周水发', '58.00', '2015-07-30 21:16:47', '57.63', null, null, '1', null, '20150730 211647', '0', '0.22', '0.15', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('3645', '65774305', '结算', '交易成功', '台江区黄鹤楼家用电器商行', '胡国平', '0.00', '2015-07-30 21:31:49', '0.00', null, null, '1', null, '20150730 213149', '0', '0.00', '0.00', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('3646', '65774305', 'IC卡批上送通知交易(联机平账)', '交易成功', '台江区黄鹤楼家用电器商行', '胡国平', '218.00', '2015-07-30 21:31:55', '216.73', null, null, '1', null, '20150730 213155', '0', '0.83', '0.44', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('3647', '65774305', 'IC卡批上送通知交易(联机平账)', '交易成功', '台江区黄鹤楼家用电器商行', '胡国平', '178.00', '2015-07-30 21:31:59', '176.96', null, null, '1', null, '20150730 213159', '0', '0.68', '0.36', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('3648', '65774305', 'IC卡批上送通知交易(联机平账)', '交易成功', '台江区黄鹤楼家用电器商行', '胡国平', '87.00', '2015-07-30 21:32:04', '86.50', null, null, '1', null, '20150730 213204', '0', '0.33', '0.17', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('3649', '65774305', 'IC卡批上送通知交易(联机平账)', '交易成功', '台江区黄鹤楼家用电器商行', '胡国平', '249.00', '2015-07-30 21:32:10', '247.55', null, null, '1', null, '20150730 213210', '0', '0.95', '0.50', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('3650', '65774305', 'IC卡批上送通知交易(联机平账)', '交易成功', '台江区黄鹤楼家用电器商行', '胡国平', '670.00', '2015-07-30 21:32:15', '666.11', null, null, '1', null, '20150730 213215', '0', '2.55', '1.34', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('3651', '65774305', '批上送结束(平账)', '交易成功', '台江区黄鹤楼家用电器商行', '胡国平', '0.00', '2015-07-30 21:32:20', '0.00', null, null, '1', null, '20150730 213220', '0', '0.00', '0.00', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('3652', '65774305', '签退', '交易成功', '台江区黄鹤楼家用电器商行', '胡国平', '0.00', '2015-07-30 21:32:25', '0.00', null, null, '1', null, '20150730 213225', '0', '0.00', '0.00', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('3653', '65804054', '消费', '密码输错', '台江区见亿食品商店', '周水发', '268.00', '2015-07-30 21:37:57', '266.31', null, null, '1', null, '20150730 213757', '0', '1.02', '0.67', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('3654', '65804054', '消费', '交易成功', '台江区见亿食品商店', '周水发', '268.00', '2015-07-30 21:38:14', '266.31', null, null, '1', null, '20150730 213814', '0', '1.02', '0.67', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('3675', '65736970', '签到双倍密钥', '交易成功', '海乐迪量贩式KTV', '朱坤守', '0.00', '2015-07-30 20:44:09', '0.00', null, null, '1', null, '20150730 204409', '0', '0.00', '0.00', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('3676', '65736970', '结算', '交易成功', '海乐迪量贩式KTV', '朱坤守', '0.00', '2015-07-30 20:46:14', '0.00', null, null, '1', null, '20150730 204614', '0', '0.00', '0.00', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('3677', '65736970', '批上送结束(平账)', '交易成功', '海乐迪量贩式KTV', '朱坤守', '0.00', '2015-07-30 20:46:19', '0.00', null, null, '1', null, '20150730 204619', '0', '0.00', '0.00', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('3678', '65736970', '消费', '交易成功', '海乐迪量贩式KTV', '朱坤守', '2.00', '2015-07-30 20:54:31', '1.99', null, null, '1', null, '20150730 205431', '0', '0.01', '0.00', '1', '98', '1');
INSERT INTO `transactionlogs` VALUES ('3679', '65736970', '结算', '交易成功', '海乐迪量贩式KTV', '朱坤守', '0.00', '2015-07-30 20:58:20', '0.00', null, null, '1', null, '20150730 205820', '0', '0.00', '0.00', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('3680', '65736970', 'IC卡批上送通知交易(联机平账)', '交易成功', '海乐迪量贩式KTV', '朱坤守', '2.00', '2015-07-30 20:58:26', '1.99', null, null, '1', null, '20150730 205826', '0', '0.01', '0.00', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('3681', '65736970', '批上送结束(平账)', '交易成功', '海乐迪量贩式KTV', '朱坤守', '0.00', '2015-07-30 20:58:32', '0.00', null, null, '1', null, '20150730 205832', '0', '0.00', '0.00', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('3682', '65736970', '消费', '交易成功', '海乐迪量贩式KTV', '朱坤守', '0.01', '2015-07-30 21:40:37', '0.01', null, null, '1', null, '20150730 214037', '0', '0.00', '0.00', '1', '99', '1');
INSERT INTO `transactionlogs` VALUES ('3683', '65736970', '结算', '交易成功', '海乐迪量贩式KTV', '朱坤守', '0.00', '2015-07-30 21:49:49', '0.00', null, null, '1', null, '20150730 214949', '0', '0.00', '0.00', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('3684', '65736970', '批上送结束(平账)', '交易成功', '海乐迪量贩式KTV', '朱坤守', '0.00', '2015-07-30 21:49:54', '0.00', null, null, '1', null, '20150730 214954', '0', '0.00', '0.00', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('3685', '65736970', '消费', '交易成功', '海乐迪量贩式KTV', '朱坤守', '100.00', '2015-07-30 22:22:36', '99.42', null, null, '1', null, '20150730 222236', '0', '0.38', '0.20', '1', '100', '1');
INSERT INTO `transactionlogs` VALUES ('3686', '65736970', '消费', '账户内余额不足', '海乐迪量贩式KTV', '朱坤守', '150.00', '2015-07-30 22:22:58', '149.13', null, null, '1', null, '20150730 222258', '0', '0.57', '0.30', '1', '100', '1');
INSERT INTO `transactionlogs` VALUES ('3687', '65736970', '消费', '交易成功', '海乐迪量贩式KTV', '朱坤守', '150.00', '2015-07-30 22:23:27', '149.13', null, null, '1', null, '20150730 222327', '0', '0.57', '0.30', '1', '100', '1');
INSERT INTO `transactionlogs` VALUES ('3688', '65736970', '消费', '账户内余额不足', '海乐迪量贩式KTV', '朱坤守', '100.00', '2015-07-30 22:23:58', '99.42', null, null, '1', null, '20150730 222358', '0', '0.38', '0.20', '1', '100', '1');
INSERT INTO `transactionlogs` VALUES ('3689', '65736970', '签到双倍密钥', '交易成功', '海乐迪量贩式KTV', '朱坤守', '0.00', '2015-07-30 22:40:10', '0.00', null, null, '1', null, '20150730 224010', '0', '0.00', '0.00', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('3690', '65736970', '自动冲正', '发卡行未能找到有关记录', '海乐迪量贩式KTV', '朱坤守', '-100.00', '2015-07-30 22:41:03', '-99.42', null, null, '1', null, '20150730 224103', '0', '-0.38', '-0.20', '1', '100', '1');
INSERT INTO `transactionlogs` VALUES ('3691', '65736970', '消费', '交易成功', '海乐迪量贩式KTV', '朱坤守', '100.00', '2015-07-30 22:41:21', '99.42', null, null, '1', null, '20150730 224121', '0', '0.38', '0.20', '1', '100', '1');
INSERT INTO `transactionlogs` VALUES ('3692', '65736970', '自动冲正', '发卡行未能找到有关记录', '海乐迪量贩式KTV', '朱坤守', '-100.00', '2015-07-30 22:41:03', '-99.42', null, null, '1', null, '20150730 224103', '0', '-0.38', '-0.20', '1', '100', '1');
INSERT INTO `transactionlogs` VALUES ('3693', '65736970', '自动冲正', '发卡行未能找到有关记录', '海乐迪量贩式KTV', '朱坤守', '-100.00', '2015-07-30 22:41:03', '-99.42', null, null, '1', null, '20150730 224103', '0', '-0.38', '-0.20', '1', '100', '1');
INSERT INTO `transactionlogs` VALUES ('3694', '65736970', '自动冲正', '发卡行未能找到有关记录', '海乐迪量贩式KTV', '朱坤守', '-100.00', '2015-07-30 22:41:03', '-99.42', null, null, '1', null, '20150730 224103', '0', '-0.38', '-0.20', '1', '100', '1');
INSERT INTO `transactionlogs` VALUES ('3695', '65736970', '自动冲正', '发卡行未能找到有关记录', '海乐迪量贩式KTV', '朱坤守', '-100.00', '2015-07-30 22:45:49', '-99.42', null, null, '1', null, '20150730 224549', '0', '-0.38', '-0.20', '1', '100', '1');
INSERT INTO `transactionlogs` VALUES ('3696', '65736970', '结算', '交易成功', '海乐迪量贩式KTV', '朱坤守', '0.00', '2015-07-30 22:45:53', '0.00', null, null, '1', null, '20150730 224553', '0', '0.00', '0.00', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('3697', '65736970', 'IC卡批上送通知交易(联机平账)', '交易成功', '海乐迪量贩式KTV', '朱坤守', '150.00', '2015-07-30 22:45:58', '149.13', null, null, '1', null, '20150730 224558', '0', '0.57', '0.30', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('3698', '65736970', 'IC卡批上送通知交易(联机平账)', '交易成功', '海乐迪量贩式KTV', '朱坤守', '100.00', '2015-07-30 22:46:08', '99.42', null, null, '1', null, '20150730 224608', '0', '0.38', '0.20', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('3699', '65736970', 'IC卡风险信息批上送通知交易(平账)', '交易成功', '海乐迪量贩式KTV', '朱坤守', '100.00', '2015-07-30 22:46:13', '99.42', null, null, '1', null, '20150730 224613', '0', '0.38', '0.20', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('3700', '65736970', '批上送结束(平账)', '交易成功', '海乐迪量贩式KTV', '朱坤守', '0.00', '2015-07-30 22:46:18', '0.00', null, null, '1', null, '20150730 224618', '0', '0.00', '0.00', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('3701', '65736970', '自动冲正', '发卡行未能找到有关记录', '海乐迪量贩式KTV', '朱坤守', '-100.00', '2015-07-30 22:41:03', '-99.42', null, null, '1', null, '20150730 224103', '0', '-0.38', '-0.20', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('3702', '65736970', '自动冲正', '发卡行未能找到有关记录', '海乐迪量贩式KTV', '朱坤守', '-100.00', '2015-07-30 22:45:49', '-99.42', null, null, '1', null, '20150730 224549', '0', '-0.38', '-0.20', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('3703', '65736970', '自动冲正', '发卡行未能找到有关记录', '海乐迪量贩式KTV', '朱坤守', '-100.00', '2015-07-30 22:41:03', '-99.42', null, null, '1', null, '20150730 224103', '0', '-0.38', '-0.20', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('3704', '65736970', '自动冲正', '发卡行未能找到有关记录', '海乐迪量贩式KTV', '朱坤守', '-100.00', '2015-07-30 22:45:49', '-99.42', null, null, '1', null, '20150730 224549', '0', '-0.38', '-0.20', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('3705', '65736970', '自动冲正', '发卡行未能找到有关记录', '海乐迪量贩式KTV', '朱坤守', '-100.00', '2015-07-30 22:41:03', '-99.42', null, null, '1', null, '20150730 224103', '0', '-0.38', '-0.20', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('3706', '65736970', '自动冲正', '发卡行未能找到有关记录', '海乐迪量贩式KTV', '朱坤守', '-100.00', '2015-07-30 22:45:49', '-99.42', null, null, '1', null, '20150730 224549', '0', '-0.38', '-0.20', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('3707', '65736970', '消费', '交易成功', '海乐迪量贩式KTV', '朱坤守', '150.00', '2015-07-30 22:52:44', '149.13', null, null, '1', null, '20150730 225244', '0', '0.57', '0.30', '1', '101', '1');
INSERT INTO `transactionlogs` VALUES ('3708', '65736970', '自动冲正', '发卡行未能找到有关记录', '海乐迪量贩式KTV', '朱坤守', '-100.00', '2015-07-30 22:41:03', '-99.42', null, null, '1', null, '20150730 224103', '0', '-0.38', '-0.20', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('3709', '65736970', '自动冲正', '发卡行未能找到有关记录', '海乐迪量贩式KTV', '朱坤守', '-100.00', '2015-07-30 22:45:49', '-99.42', null, null, '1', null, '20150730 224549', '0', '-0.38', '-0.20', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('3710', '65736970', '自动冲正', '发卡行未能找到有关记录', '海乐迪量贩式KTV', '朱坤守', '-100.00', '2015-07-30 22:41:03', '-99.42', null, null, '1', null, '20150730 224103', '0', '-0.38', '-0.20', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('3711', '65736970', '自动冲正', '发卡行未能找到有关记录', '海乐迪量贩式KTV', '朱坤守', '-100.00', '2015-07-30 22:45:49', '-99.42', null, null, '1', null, '20150730 224549', '0', '-0.38', '-0.20', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('3712', '65736970', '自动冲正', '发卡行未能找到有关记录', '海乐迪量贩式KTV', '朱坤守', '-100.00', '2015-07-30 22:41:03', '-99.42', null, null, '1', null, '20150730 224103', '0', '-0.38', '-0.20', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('3713', '65736970', '自动冲正', '发卡行未能找到有关记录', '海乐迪量贩式KTV', '朱坤守', '-100.00', '2015-07-30 22:45:49', '-99.42', null, null, '1', null, '20150730 224549', '0', '-0.38', '-0.20', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('3714', '65736970', '自动冲正', '发卡行未能找到有关记录', '海乐迪量贩式KTV', '朱坤守', '-100.00', '2015-07-30 22:41:03', '-99.42', null, null, '1', null, '20150730 224103', '0', '-0.38', '-0.20', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('3715', '65736970', '自动冲正', '发卡行未能找到有关记录', '海乐迪量贩式KTV', '朱坤守', '-100.00', '2015-07-30 22:45:49', '-99.42', null, null, '1', null, '20150730 224549', '0', '-0.38', '-0.20', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('3716', '65736970', '结算', '交易成功', '海乐迪量贩式KTV', '朱坤守', '0.00', '2015-07-30 23:00:32', '0.00', null, null, '1', null, '20150730 230032', '0', '0.00', '0.00', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('3717', '65736970', '批上送结束(平账)', '交易成功', '海乐迪量贩式KTV', '朱坤守', '0.00', '2015-07-30 23:00:43', '0.00', null, null, '1', null, '20150730 230043', '0', '0.00', '0.00', '0', null, '0');
INSERT INTO `transactionlogs` VALUES ('3718', '65736970', '自动冲正', '发卡行未能找到有关记录', '海乐迪量贩式KTV', '朱坤守', '-100.00', '2015-07-30 22:41:03', '-99.42', null, null, '1', null, '20150730 224103', '0', '-0.38', '-0.20', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('3719', '65736970', '自动冲正', '发卡行未能找到有关记录', '海乐迪量贩式KTV', '朱坤守', '-100.00', '2015-07-30 22:45:49', '-99.42', null, null, '1', null, '20150730 224549', '0', '-0.38', '-0.20', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('3720', '65736970', '自动冲正', '发卡行未能找到有关记录', '海乐迪量贩式KTV', '朱坤守', '-100.00', '2015-07-30 22:41:03', '-99.42', null, null, '1', null, '20150730 224103', '0', '-0.38', '-0.20', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('3721', '65736970', '自动冲正', '发卡行未能找到有关记录', '海乐迪量贩式KTV', '朱坤守', '-100.00', '2015-07-30 22:45:49', '-99.42', null, null, '1', null, '20150730 224549', '0', '-0.38', '-0.20', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('3722', '65736970', '自动冲正', '发卡行未能找到有关记录', '海乐迪量贩式KTV', '朱坤守', '-100.00', '2015-07-30 22:41:03', '-99.42', null, null, '1', null, '20150730 224103', '0', '-0.38', '-0.20', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('3723', '65736970', '自动冲正', '发卡行未能找到有关记录', '海乐迪量贩式KTV', '朱坤守', '-100.00', '2015-07-30 22:45:49', '-99.42', null, null, '1', null, '20150730 224549', '0', '-0.38', '-0.20', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('3724', '65736970', '自动冲正', '发卡行未能找到有关记录', '海乐迪量贩式KTV', '朱坤守', '-100.00', '2015-07-30 22:41:03', '-99.42', null, null, '1', null, '20150730 224103', '0', '-0.38', '-0.20', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('3725', '65736970', '自动冲正', '发卡行未能找到有关记录', '海乐迪量贩式KTV', '朱坤守', '-100.00', '2015-07-30 22:45:49', '-99.42', null, null, '1', null, '20150730 224549', '0', '-0.38', '-0.20', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('3726', '65736970', '自动冲正', '发卡行未能找到有关记录', '海乐迪量贩式KTV', '朱坤守', '-100.00', '2015-07-30 22:41:03', '-99.42', null, null, '1', null, '20150730 224103', '0', '-0.38', '-0.20', '0', null, '1');
INSERT INTO `transactionlogs` VALUES ('3727', '65736970', '自动冲正', '发卡行未能找到有关记录', '海乐迪量贩式KTV', '朱坤守', '-100.00', '2015-07-30 22:45:49', '-99.42', null, null, '1', null, '20150730 224549', '0', '-0.38', '-0.20', '0', null, '1');

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
INSERT INTO `transactionsum` VALUES ('100', '65736970', '100', '0.2', '0.38', '99.42', '-2', '商户审核拒绝', '2015-07-30 22:46:57', '2015-07-30 22:47:10', '2015-07-30 23:02:15', null, '中国民生银行', '福州市分行', '台江区中亭街支行', '福建', '福州', '13788598652', '201507302247105744', '10', '朱坤守', '海乐迪量贩式KTV', '6226221501907114', '严时丽民生622255555555');
INSERT INTO `transactionsum` VALUES ('101', '65736970', '150', '0.3', '0.57', '149.13', '2', '银行处理中', '2015-07-30 23:01:55', '2015-07-30 23:02:13', null, null, '中国民生银行', '福州市分行', '台江区中亭街支行', '福建', '福州', '13788598652', '201507302302135465', '1', '朱坤守', '海乐迪量贩式KTV', '6226221501907114', '严时丽民生622255555555');

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
