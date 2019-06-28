#region License
/*
 * TestSocketIO.cs
 *
* The MIT License
*
* Copyright (c) 2014 Fabio Panettieri
*
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
	*
	* The above copyright notice and this permission notice shall be included in
	* all copies or substantial portions of the Software.
	*
	* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
	* THE SOFTWARE.
	*/
	#endregion

using System.Collections;
using UnityEngine;
using SocketIO;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class SocketObjectONE : SingletonMonoBehavior<SocketObjectONE>
{
	private SocketIOComponent socket;

	string url = "ws://52.194.134.160:1337/socket.io/?EIO=4&transport=websocket";

    public string id;
    string name = "CUBOTEMPLOYEEONLY";

	public bool connecting = false;

	void Start() 
	{
        Connect();
	}

	public void joinRoom(string roomName){
		var data = new Dictionary<string,string>();
		data["name"] = name;
		data["room"] = roomName;
		socket.Emit ("updateRoom",new JSONObject(data));
	}

	public void leaveRoom(){
		var data = new Dictionary<string,string>();
		data["name"] = name;
		data["room"] = "";
		socket.Emit ("updateRoom",new JSONObject(data));
	}

	public void EmitMessage(string s,Dictionary<string,string> d){
		if (connecting) {
			socket.Emit (s, new JSONObject (d));
		} else {
			Debug.Log ("[ERROR]オンライン状態ではありません");
		}
	}

	public void Connect(){

		if (!connecting) {
			
			try {
			
				GetComponent<SocketIOComponent> ().url = url;
				GetComponent<SocketIOComponent> ().Standby ();

				socket = GetComponent<SocketIOComponent> ();

				socket.On ("open", SocketOpen);
				socket.On ("ID", ReceiveID);
				socket.On ("error", ReceiveError);
				socket.On ("close", SocketClose);

				socket.Connect ();

			} catch (Exception e) {
				connecting = false;
			}
		}
	}

	public void SocketOpen(SocketIOEvent e)
	{
		if (!connecting) {
			connecting = true;
			var data = new Dictionary<string,string> ();
			data ["name"] = WWW.EscapeURL (name);
			Debug.Log (data ["name"]);
			socket.Emit ("ID", new JSONObject (data));
		}

		Debug.Log("[SocketIO] Open received: " + e.name + " " + e.data);
	}
	public void ReceiveID(SocketIOEvent e)
	{
		Dictionary<string,string> d = new JSONObject (e.data.ToString()).ToDictionary();
		if (id.Equals("")) {
			id = d ["id"];
			Debug.Log ("[SocketIO] ID received: " + e.name + " " + e.data);
		}
	}
    public void ReceiveError(SocketIOEvent e)
    {
        connecting = false;
        Debug.Log("[SocketIO] Error received: " + e.name + " " + e.data);
    }
    public void SocketClose(SocketIOEvent e)
    {
        connecting = false;
        Debug.Log("[SocketIO] Close received: " + e.name + " " + e.data);
    }
    public void Disconnection(){
		socket.Close ();
		connecting = false;
		name = "";
		id = "";
	}

}