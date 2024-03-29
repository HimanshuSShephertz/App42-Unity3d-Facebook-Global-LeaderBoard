﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.shephertz.app42.paas.sdk.csharp;
using System;
using com.shephertz.app42.paas.sdk.csharp.game;

public class LeaderBoardCallBack : App42CallBack {

	public static string fbUserName = "";
	public static string fbUserProfilePic = "";
	public static string fbUserId = "";
	public static bool isConnected = false;
	public static bool tokenRecieved = false;

	public static bool fromSaveScore = false;
	public static bool fromLeaderBoard = false;
	public static bool fromFriends = false;

	public static IList<object> fList = new List<object> ();

	public static string fbAccessToken = "";

	public void OnSuccess (object response)
	{
		if (response is string){
			LoadingMessage.newMessage  = "Connected With Facebook";
			LeaderBoardCallBack.fbAccessToken = response.ToString();
			LeaderBoardCallBack.isConnected = true;
			LeaderBoardCallBack.tokenRecieved = true;
		}

		if (response is Game){
			Game gameObj = (Game)response;
			Debug.Log("Game ::: "+gameObj.ToString());
			if (fromLeaderBoard){
				if(gameObj.GetScoreList().Count > 0){
					for (int i=0; i<gameObj.GetScoreList().Count; i++){
						int currentDetails = gameObj.GetScoreList()[i].GetJsonDocList().Count-1;
						string score = gameObj.GetScoreList()[i].GetValue().ToString();
						string jsonDoc = gameObj.GetScoreList()[i].GetJsonDocList()[currentDetails].GetJsonDoc();
						var parser = SimpleJSON.JSON.Parse(jsonDoc);
						string profilePic = parser["profilePic"];
						string userId = parser["userId"];
						string name =  parser["name"];
						Texture.GetInstance().ExecuteShow(userId, profilePic);
						Debug.Log("userId ::: " + name);
						IList<string> slist1 = new List<string>();
						slist1.Add(userId);
						slist1.Add(name);
						slist1.Add(profilePic);
						slist1.Add(score);
						fList.Add(slist1);
					}
				}
				fromLeaderBoard = false;
				if(!fromFriends){
					Application.LoadLevel("LeaderBoardScene");
				}
				LeaderBoardCallBack.fromFriends = false;
			}
			if (fromSaveScore){
				LoadingMessage.SetMessage("Score SuccessFully Saved.");
			}

		}
		if (response is com.shephertz.app42.paas.sdk.csharp.social.Social){
			com.shephertz.app42.paas.sdk.csharp.social.Social socialObj = (com.shephertz.app42.paas.sdk.csharp.social.Social)response;
			Debug.Log("Social :: "+socialObj.ToString());
			LeaderBoardCallBack.fbUserName = socialObj.GetFacebookProfile().GetName();
			LeaderBoardCallBack.fbUserProfilePic = socialObj.GetFacebookProfile().GetPicture();
			LeaderBoardCallBack.fbUserId = socialObj.GetFacebookProfile().GetId();
			Application.LoadLevel("GameScene");
		}
	}
	
	public void OnException (Exception e)
	{
		Debug.Log("Exception :: "+e.ToString());
	}

	public static IList<object> GetList(){
		return fList;
	}
}
