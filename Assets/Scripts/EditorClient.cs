using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

public class EditorClient : PlatformClient {

	public override void Login()
	{
		var baseInfo = new JsonData();
		baseInfo["nickname"] = "风从海上来";
		baseInfo["figure"] = "https://ss0.bdstatic.com/70cFvHSh_Q1YnxGkpoWK1HF6hhy/it/u=2043575092,1720307024&fm=26&gp=0.jpg";
		Servlet.SetBaseInfo(baseInfo.ToJson());
	}
}
