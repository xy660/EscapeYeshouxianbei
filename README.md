# 逃离野兽先辈

逃离野兽先辈游戏，使用GDI+直接绘制，迷宫类游戏

**本作品是本人高中时期的业余作品，代码写的不算规整，比较乱**

**游戏贴图素材来自网络和表情包，未进行商业使用**

## Features

- 直接使用GDI+绘制，无游戏引擎参与
- 实现了双缓冲技术防止闪烁
- 猎手AI自动寻路追杀玩家
- 完整游戏玩法

## 游戏玩法

玩家从左上角出生，右下角为出口，需要在躲避野兽先辈追杀的情况下拾取5个钥匙
右下角出口在拾取5个钥匙后自动打开，从那里逃出去就算赢
（详情可以查看游戏内提示）

## 游戏截图

![主菜单](https://raw.githubusercontent.com/xy660/EscapeYeshouxianbei/main/imgs/1.png)
![地图预览](https://raw.githubusercontent.com/xy660/EscapeYeshouxianbei/main/imgs/2.png)
![胜利](https://raw.githubusercontent.com/xy660/EscapeYeshouxianbei/main/imgs/3.png)

## 一些不足

- GDI+渲染效率不高，可能会出现渲染延迟，撕裂，可以尝试建立配置文件锁60帧
- 偶尔AI会出现左右脑互博，不过作为特性保留了（给玩家留下绕路时间）
