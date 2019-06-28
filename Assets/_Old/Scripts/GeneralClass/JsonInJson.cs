using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonInJson
{

	public string InJson(Dictionary<string,string> dic){
		return WWW.EscapeURL(new JSONObject(dic).ToString());
	}

	public Dictionary<string,string> PicJson(string json){
		var jsonObj = new JSONObject(WWW.UnEscapeURL(json));
		return jsonObj.ToDictionary ();
	}
}
