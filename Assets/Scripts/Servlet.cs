using LitJson;

public class Servlet : GameBehaviour {

	public void SetBaseInfo(string json)
	{
		var info = JsonMapper.ToObject(json);
		Player.Nickname = (string)info["nickname"];
		Player.Figure = (string)info["figure"];
		
		Scenes.GotoScene("Main");
	}
	
}
