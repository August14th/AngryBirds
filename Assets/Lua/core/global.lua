
engine = CS.UnityEngine.GameObject.FindWithTag("GameEngine")
assert(engine, "engine is missing")

assets = engine:GetComponent("LocalAsset") 
if not assets then
    assets = engine:GetComponent("Bundles") 
end
assert(assets, "assets is missing")

scenes = engine:GetComponent("Scenes") 
assert(scenes, "scenes is missing")

function canvas()
    return CS.UnityEngine.GameObject.Find("Canvas")
end