# 修复克隆技能问题 - Product Requirement Document

## Overview
- **Summary**: 修复克隆技能在反击和复制克隆方面的bug，确保克隆能够正确朝向敌人，实现格挡后99%几率生成克隆的功能，并恢复格挡后双克隆攻击的功能。
- **Purpose**: 解决用户报告的克隆技能问题，提升游戏体验和技能的功能性。
- **Target Users**: 游戏玩家，使用克隆技能和反击技能的用户。

## Goals
- 修复反击克隆朝向始终朝右的问题（距离超过25时的bug）
- 确保克隆能够正确朝向最近的敌人
- 实现格挡后有99%几率生成第二个克隆的功能
- 恢复格挡后双克隆攻击的功能

## Non-Goals (Out of Scope)
- 完全重写克隆技能系统
- 添加新的技能机制
- 修改游戏其他核心系统

## Background & Context
现有问题：
1. Q键反击时，克隆朝向始终朝右，当敌人距离超过25时出现此bug
2. 格挡后有99%几率生成克隆的功能未实现
3. 格挡后双克隆攻击功能失效

原因分析：
- 反击克隆时，使用FindClosestEnemy搜索半径只有25，超出范围找不到敌人时克隆保持默认朝右
- 克隆初始化时直接使用玩家传递的敌人transform，但仍重新调用FindClosestEnemy，导致浪费和潜在问题

## Functional Requirements
- **FR-1**: 反击克隆必须能够正确朝向最近的敌人，无论距离远近
- **FR-2**: 克隆在初始化时应该使用反击过程中找到的具体敌人，而不是重新搜索
- **FR-3**: 克隆攻击敌人时有99%几率生成第二个克隆（已实现但需要确认和修复）
- **FR-4**: 确保克隆朝向敌人的逻辑可靠工作

## Non-Functional Requirements
- **NFR-1**: 修改必须保持现有代码结构，最小化变更
- **NFR-2**: 克隆朝向更新必须在1帧内完成
- **NFR-3**: 99%生成几率必须准确实现

## Constraints
- **Technical**: 使用Unity 2D游戏引擎，C#脚本
- **Business**: 必须最小化代码修改，保持现有功能完整性
- **Dependencies**: 依赖现有克隆技能系统、反击系统和敌人系统

## Assumptions
- Clone_Skill_controller中的99%生成克隆逻辑已基本实现
- 格挡功能可能还未完全实现（代码中未找到）
- 用户希望保持现有代码结构不变

## Acceptance Criteria

### AC-1: 反击克隆正确朝向敌人
- **Given**: 玩家使用Q键反击成功命中敌人
- **When**: 克隆被生成
- **Then**: 克隆必须朝向被反击的敌人，无论距离远近
- **Verification**: `human-judgment`
- **Notes**: 需在游戏中测试不同距离的敌人

### AC-2: 克隆朝向不依赖25距离限制
- **Given**: 敌人距离玩家超过25个单位
- **When**: 玩家成功反击并生成克隆
- **Then**: 克隆仍然能够正确朝向敌人
- **Verification**: `human-judgment`
- **Notes**: 测试距离必须大于25个单位

### AC-3: 克隆攻击时有99%几率生成新克隆
- **Given**: 克隆成功攻击敌人且canDuplicateClone为true
- **When**: 克隆触发AttackTrigger
- **Then**: 有99%的概率在敌人位置生成新的克隆
- **Verification**: `human-judgment`
- **Notes**: 多次测试验证概率

### AC-4: 克隆朝向逻辑可靠
- **Given**: 克隆被生成且有明确的敌人目标
- **When**: 克隆初始化
- **Then**: 克隆的facingDir和旋转必须正确设置
- **Verification**: `human-judgment`

## Open Questions
- [ ] 格挡功能的具体实现位置？代码中未找到相关实现
- [ ] 格挡后双克隆攻击的具体期望行为？
