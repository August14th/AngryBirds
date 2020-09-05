# AngryBirds

愤怒的小鸟是Rovio拳头公司2009年开发的一款2d游戏。这是一款练习作品，所有的美术资源来自网络。

1. 所有的资源都会打包成AssetBunles，如果资源有修改，玩家可以在线增量更新。
2. 如果网络异常导致不能下载到最新的资源，会使用本地资源。
3. 支持将部分资源放到StreamingAssets，这样可以控制安装包的大小。
4. 支持xlua,支持VS Code调试lua代码。
5. 支持在lua代码中给GameObject挂载lua组件。
6. 通过扩展GameObject的__index和__newindex，支持在lua代码中直接访问GameObject上挂载的C#组件，例如go.Image就可以访问Image组件。button.onclick = function()...end可以注册点击函数。
简化lua代码。

# 参考资源

[siki学院-愤怒的小鸟](http://www.sikiedu.com/my/course/134)
