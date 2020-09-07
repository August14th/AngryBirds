# AngryBirds

愤怒的小鸟是Rovio拳头公司2009年开发的一款2d游戏。这是一款练习作品，所有的美术资源来自网络。

1. 所有的资源都会打包成AssetBundles，如果资源有修改，玩家可以在线增量更新。
2. 如果网络异常导致不能下载到最新的资源，会使用本地资源。
3. 支持将部分资源放到StreamingAssets，这样可以控制安装包的大小。
4. 支持基于引用计数的AssetBundle的卸载，避免内存泄漏。
5. 支持xlua,支持VS Code调试lua代码。
6. 支持在lua代码中给GameObject挂载lua组件。
7. 通过扩展GameObject的__index和__newindex，支持在lua代码中直接访问GameObject上挂载的C#组件，例如go.Image就可以访问Image组件。button.onclick = function()...end可以注册点击函数。
简化lua代码。

# 截图

1. 下载资源 
![下载资源](https://raw.githubusercontent.com/August14th/AngryBirds/master/Pictures/download.jpg)
2. 关卡选择
![关卡选择](https://raw.githubusercontent.com/August14th/AngryBirds/master/Pictures/level.jpg)
3. 飞行中的小鸟
![飞行中的小鸟](https://raw.githubusercontent.com/August14th/AngryBirds/master/Pictures/bird.jpg)
4. 胜利
![胜利](https://raw.githubusercontent.com/August14th/AngryBirds/master/Pictures/settle.jpg)

# 参考资源

[siki学院-愤怒的小鸟](http://www.sikiedu.com/my/course/134)
[Angry Birds style game in Unity 3D](https://dgkanatsios.com/2014/07/28/angry-birds-clone-in-unity-3d-source-code-included-3/)
