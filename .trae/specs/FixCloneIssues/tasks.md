# 修复克隆技能问题 - The Implementation Plan (Decomposed and Prioritized Task List)

## [ ] Task 1: 修复Clone_Skill.CreateClone方法，添加可选的敌人参数
- **Priority**: P0
- **Depends On**: None
- **Description**: 
  - 修改CreateClone方法，添加可选的_closeEnemy参数
  - 当提供此参数时，直接使用而不重新调用FindClosestEnemy
  - 这样可以避免25距离限制的问题
- **Acceptance Criteria Addressed**: AC-1, AC-2
- **Test Requirements**:
  - `programmatic` TR-1.1: CreateClone方法能够接受可选的_closeEnemy参数
  - `human-judgement` TR-1.2: 当提供_closeEnemy时，克隆使用该敌人而非重新搜索
- **Notes**: 最小化修改，保持向后兼容

## [ ] Task 2: 修改CreateCloneOnCounterAttack，传递找到的敌人
- **Priority**: P0
- **Depends On**: Task 1
- **Description**: 
  - 修改CreateCloneOnCounterAttack方法
  - 将_counterAttack中找到的敌人transform传递给CreateClone
  - 确保克隆直接使用该敌人
- **Acceptance Criteria Addressed**: AC-1, AC-2, AC-4
- **Test Requirements**:
  - `human-judgement` TR-2.1: 反击时克隆朝向正确的敌人
  - `human-judgement` TR-2.2: 距离超过25时克隆仍能正确朝向敌人
- **Notes**: 确保敌人信息被正确传递

## [ ] Task 3: 修复Clone_Skill_controller中复制克隆的facingDir问题
- **Priority**: P1
- **Depends On**: None
- **Description**: 
  - 检查AttackTrigger中创建新克隆的逻辑
  - 确保新克隆的offset正确使用facingDir
  - 验证facingDir在FaceClosestTarget中被正确设置
- **Acceptance Criteria Addressed**: AC-3, AC-4
- **Test Requirements**:
  - `human-judgement` TR-3.1: 克隆攻击时有99%几率生成新克隆
  - `human-judgement` TR-3.2: 新克隆的位置和朝向正确
- **Notes**: 确保facingDir变量正确使用

## [ ] Task 4: 优化Clone_Skill_controller的FaceClosestTarget方法
- **Priority**: P1
- **Depends On**: None
- **Description**: 
  - 确保FaceClosestTarget方法正确设置facingDir
  - 当没有敌人时，默认朝向玩家的朝向
- **Acceptance Criteria Addressed**: AC-4
- **Test Requirements**:
  - `human-judgement` TR-4.1: 有敌人时克隆正确朝向敌人
  - `human-judgement` TR-4.2: 无敌人时克隆正确朝向玩家方向
- **Notes**: 改善无敌人时的行为
