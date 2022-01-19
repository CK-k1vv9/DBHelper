/*
Navicat MySQL Data Transfer

Source Server         : localhost
Source Server Version : 50728
Source Host           : localhost:3306
Source Database       : dbhelper_test

Target Server Type    : MYSQL
Target Server Version : 50728
File Encoding         : 65001

Date: 2022-01-19 12:11:24
*/

SET FOREIGN_KEY_CHECKS=0;
-- ----------------------------
-- Table structure for `bs_order`
-- ----------------------------
DROP TABLE IF EXISTS `bs_order`;
CREATE TABLE `bs_order` (
  `id` varchar(50) NOT NULL COMMENT '主键',
  `order_time` datetime NOT NULL COMMENT '订单时间',
  `amount` decimal(20,2) DEFAULT NULL COMMENT '订单金额',
  `order_userid` bigint(20) NOT NULL COMMENT '下单用户',
  `status` tinyint(4) NOT NULL COMMENT '订单状态(0草稿 1已下单 2已付款 3已发货 4完成)',
  `remark` varchar(255) DEFAULT NULL COMMENT '备注',
  `create_userid` varchar(50) NOT NULL COMMENT '创建者ID',
  `create_time` datetime NOT NULL COMMENT '创建时间',
  `update_userid` varchar(50) DEFAULT NULL COMMENT '更新者ID',
  `update_time` datetime DEFAULT NULL COMMENT '更新时间',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='订单表';

-- ----------------------------
-- Records of bs_order
-- ----------------------------
BEGIN;
INSERT INTO `bs_order` VALUES ('100001', '2022-01-19 12:04:42', '17268.42', '10', '0', '测试订单001', '10', '2022-01-19 12:04:42', null, null), ('100002', '2022-01-19 12:05:23', '17268.42', '10', '0', '测试订单002', '10', '2022-01-19 12:05:23', null, null), ('100003', '2022-01-19 12:05:59', '17268.42', '10', '0', '测试订单003', '10', '2022-01-19 12:05:59', null, null);
COMMIT;

-- ----------------------------
-- Table structure for `bs_order_detail`
-- ----------------------------
DROP TABLE IF EXISTS `bs_order_detail`;
CREATE TABLE `bs_order_detail` (
  `id` varchar(50) NOT NULL COMMENT '主键',
  `order_id` varchar(50) NOT NULL COMMENT '订单ID',
  `goods_name` varchar(200) NOT NULL COMMENT '商品名称',
  `quantity` int(11) NOT NULL COMMENT '数量',
  `price` decimal(20,2) NOT NULL COMMENT '价格',
  `spec` varchar(100) DEFAULT NULL COMMENT '物品规格',
  `order_num` int(11) DEFAULT NULL COMMENT '排序',
  `create_userid` varchar(50) NOT NULL COMMENT '创建者ID',
  `create_time` datetime NOT NULL COMMENT '创建时间',
  `update_userid` varchar(50) DEFAULT NULL COMMENT '更新者ID',
  `update_time` datetime DEFAULT NULL COMMENT '更新时间',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='订单明细表';

-- ----------------------------
-- Records of bs_order_detail
-- ----------------------------
BEGIN;
INSERT INTO `bs_order_detail` VALUES ('10000101', '100001', '电脑', '3', '5100.00', '台', '1', '10', '2022-01-19 12:04:42', null, null), ('10000102', '100001', '鼠标', '12', '50.68', '个', '2', '10', '2022-01-19 12:04:43', null, null), ('10000103', '100001', '键盘', '11', '123.66', '个', '3', '10', '2022-01-19 12:04:43', null, null), ('10000201', '100002', '键盘', '11', '123.66', '个', '3', '10', '2022-01-19 12:05:23', null, null), ('10000202', '100002', '鼠标', '12', '50.68', '个', '2', '10', '2022-01-19 12:05:23', null, null), ('10000203', '100002', '电脑', '3', '5100.00', '台', '1', '10', '2022-01-19 12:05:23', null, null), ('10000301', '100003', '鼠标', '12', '50.68', '个', '2', '10', '2022-01-19 12:05:59', null, null), ('10000302', '100003', '电脑', '3', '5100.00', '台', '1', '10', '2022-01-19 12:05:59', null, null), ('10000303', '100003', '键盘', '11', '123.66', '个', '3', '10', '2022-01-19 12:05:59', null, null);
COMMIT;

-- ----------------------------
-- Table structure for `sys_user`
-- ----------------------------
DROP TABLE IF EXISTS `sys_user`;
CREATE TABLE `sys_user` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT COMMENT '主键',
  `user_name` varchar(50) NOT NULL COMMENT '用户名',
  `real_name` varchar(50) DEFAULT NULL COMMENT '用户姓名',
  `password` varchar(200) NOT NULL COMMENT '用户密码',
  `remark` varchar(200) DEFAULT NULL COMMENT '备注',
  `create_userid` varchar(50) NOT NULL COMMENT '创建者ID',
  `create_time` datetime NOT NULL COMMENT '创建时间',
  `update_userid` varchar(50) DEFAULT NULL COMMENT '更新者ID',
  `update_time` datetime DEFAULT NULL COMMENT '更新时间',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=961675 DEFAULT CHARSET=utf8mb4 COMMENT='用户表';

-- ----------------------------
-- Records of sys_user
-- ----------------------------
BEGIN;
INSERT INTO `sys_user` VALUES ('1', 'admin', '超级管理员', '123456', '超级管理员', '1', '2020-11-01 13:39:43', '1', '2020-11-01 13:39:47'), ('2', 'admin2020', '普通管理员', '123456', '普通管理员', '1', '2020-11-01 13:42:55', '1', '2020-11-01 13:42:58'), ('9', 'wangwu', '王五', '123456', '测试修改用户02', '1', '2022-01-19 12:10:17', null, null), ('10', 'zhangsan', '张三', '123456', '测试修改用户01', '1', '2020-11-01 13:40:30', '1', '2022-01-19 11:23:57'), ('11', 'lisi', '李四', '123456', '测试修改用户03', '1', '2020-11-01 13:42:08', '1', '2022-01-17 10:29:55');
COMMIT;
