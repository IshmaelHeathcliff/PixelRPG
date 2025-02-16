# PixelRPG

受流放之路、塔科夫、DND、Roguelite等类型游戏启发的一款刷宝、背包管理、跑商经营类RPG游戏。游戏的主要玩法为在一个城镇购买物资、接取任务等，然后选择路线前往另一个城镇的途中进行战斗，在目的地出售物资賺取差价并完成任务。着重于玩家角色状态管理、背包管理、资源管理，并通过天赋、技能、装备的构筑进行刷宝，通过获取的资源来成长。在数值设计上有一些激进的地方，在和平模式中兼顾较休闲的体验。

游戏的重复游玩性上，将在和平模式以外设置一个游戏时间限制。游戏背景设定为在魔王即将毁灭世界的前夕不断轮回，角色应尽力成长从而尝试在末日来临前击败魔王。每局游戏的平均时长目标为10到20小时，游戏结束后，玩家的剩余资产将转化为局外成长点数，之后的游戏可消耗成长点数解锁更多的选择和提供更多的初始资源。

## 开发工具

- Unity 6
- QFramework
- Newton Json
- UniTask
- Odin Inspector
- DOTween

## 核心内容

### 资源循环

- 主要资源：

  - 金币
  - 材料
  - 装备
  - 药品
  - 食物
  - *公会点数
- 资源获取：

  - 金币主要由商人交易和任务产出，战斗不直接产出金币
  - 材料由战斗产出，部分材料也可在商人处用金币购买。
  - 装备、药品、食物等主要在商人处用材料和金币兑换，中后期也可直接用更多金币购买低级部分，少量由战斗、任务获取。
  - *公会点数通过完成公会任务获得。
- 资源消耗：

  - 材料大多可用于兑换，少数仅用于任务和交易。
  - 学习技能、获得天赋消耗特殊材料。
  - 兑换物品、完成任务、使用设施、学习技能、获得天赋等消耗金币。
  - 装备具有耐久，可部分维修，损失耐久上限和基础性能。
  - 在标准模式下，战败将会损失资源。
  - *公会点数在公会兑换物品。

### 角色状态

- 角色有一些状态属性需要通过药品、食物、睡眠等来维持。
- 状态属性在一定水平以上可能获得增益，一定水平以下可能获得减益。
- #### 能量

  - 能量值主要在切换地图时消耗，消耗值由目标地图决定

    - 地图的能量消耗值大致反应地图代表的距离远近，较远的路程消耗较多的能量，而近路可能意味着更高的难度
  - 通过在酒馆休息完全恢复，也可在野外安全区域扎营休息部分，影响玩家每天能过的图的上限
  - 能量值上限随着游戏进程提升
  - 能量值在20%以下获得疲惫Debuff，小幅降低全部属性
  - 能量值耗尽无法切换地图
- #### 饱食、营养与水分

  - 营养等级

    - 低级食物无法补充高等级的营养
    - 饱食度和水分的降低也会降低营养
    - 更高的营养等级会获得更高的增益
    - 营养等级为0时，会有严重Debuff，非城镇场景会逐渐流失生命
  - 饱食度和水分满时无法进食
  - 饱食度和水分低于20%时会有Debuff

    - 饥饿与口渴
  - 饱食度和水分为0时会快速降低营养
  - 可以用营养药品补充营养，不受饱食度和水分影响

    - 营养药品成本很高
- #### 异常状态

  - 受到对应类型伤害时有几率陷入异常

    - 玩家受到的异常需要消耗药品治疗
    - 怪物受到的异常有持续时间

      - 异常门槛与持续时间
  - 物理异常

    - 医疗包治疗
    - 虚弱

      - 穿刺型攻击
      - 降低伤害
    - 骨折

      - 钝器型攻击
      - 明显降低移速
    - 流血

      - 利刃型攻击
      - 持续损失百分比生命
    - 眩晕*

      - 降低视野范围
  - 元素异常

    - 灵药治疗
    - 燃烧

      - 火焰伤害
      - 持续受到固定火焰伤害
    - 冰缓

      - 冰冷伤害
      - 降低行动速度
    - 感电

      - 闪电伤害
      - 受到伤害增加

### 角色属性

- 所有数值属性都有基础值、附加值、提高值、总增值、固定值，计算方式如下

$$(基础值+\sum 附加值) \times (1 + \sum (提高值/100)) \times \prod (1+总增值/100) + \sum 固定值$$

- 消耗性属性有当前值与最大值
- 生命与魔力

  - 方案1: **生命和魔力无法自然恢复**，只能通过药品、治疗、休息

    - 魔力药剂比较便宜
    - 药剂同样要占用背包空间，且堆叠数量有限，有使用冷却
  - 方案2: 生命魔力可以自然恢复，但有上限的损失，必须使用药品、治疗与休息
- 护盾

  - 消耗魔力抵挡部分生命损失
  - 转换比率
  - 抵挡比例

### 词条

- 属性修改类词条
- 能力赋予类词条
- 状态施加类词条
- #### 标签

  - 词条都有标签或关键字，用于在生效时筛选有效词条
  - 筛选方式

    - 含有
    - 都有
  - 标签可有层级

### 状态

- 词条的组合
- 可控制持续时间
- 可有堆叠层数

### 伤害

- #### 伤害来源

  - 攻击、法术、持续、次要
  - 单个伤害只有一个伤害来源
  - 不同伤害来源使用不同的伤害计算机制
- #### 伤害类型

  - 物理、火焰、冰霜、闪电
  - 单个伤害只有一种伤害类型
- #### 伤害词条关键字

  - 来源、类型、攻击武器、形态

### 背包与仓库

- 背包与仓库可升级增加容量
- 物品有尺寸大小，可旋转，可随意摆放
- 物品有重量

### 装备

### 天赋

### 技能

### 城镇与经济

## 额外内容

- ### 公会
- ### 地下城

  - 有更多战斗，获得更多材料
  - 更危险，无法跑商
- ### 家园
- ### 自定义角色

## 开发进度

- ### 基础架构

  - [X] 背包与仓库
  - [X] 词条系统
  - [X] 属性系统
  - [X] 状态系统
  - [ ] 敌人系统

    - [X] 状态机
    - [ ] 寻路
  - [ ] 伤害系统
  - [ ] 技能系统
  - [ ] 天赋系统
  - [ ] 任务系统
  - [ ] 对话与交易
  - [ ] NPC与公会
  - [ ] 物品制作
  - [ ] 动态经济
- ### 内容填充

  - [ ] 城镇与环境
  - [ ] 物品

    - 材料
    - 装备
    - 药品
    - 食物
  - [ ] 词条
  - [ ] 状态
  - [ ] 技能
  - [ ] 天赋
  - [ ] 任务
  - [ ] 敌人与Boss
  - [ ] NPC
  - [ ] 剧情
