using System;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using MonoTouch.Foundation;
using System.Text;
using System.Net;
using System.Web.Services;
using System.Web;
using System.Collections.Generic;
using  Newtonsoft.Json;
using System.Xml.Linq;
using System.Linq;
namespace GowallaOAuthSample
{
	public class GowallaAPI
	{
		GowallaOAuthorizer OAuth;
		static string baseUrl = "https://api.gowalla.com";
		public GowallaAPI (GowallaOAuthorizer OAuthorizer)
		{
			OAuth = OAuthorizer;
		}
		
		static string PreparePostData (Dictionary<string,string> postData)
		{
			return String.Join ("&", (from x in postData.Keys select String.Format ("{0}={1}", x, postData [x])).ToArray ());
		}
		
		public string getPassportByUsername(string username)
		{
			try{
				var s = fetchGowallaData("/users/" + username);
				return s;
			}catch (Exception e){
				Console.WriteLine(e);
			}
			return "";
		}
		
		public string getStampsForUsername (string username)
		{
    		var s = getStampsForUsername(username, 20);
			return s;
		}
		
		public string getStampsForUsername (string username, int limit)
		{
    		
			try{
				var s = fetchGowallaData("/users/" + username + "/stamps?limit=" + limit);
				return s;
			}catch (Exception e){
				Console.WriteLine(e);
			}
			return "";
		}
		
		public string getTopSpotsforUser(string username)
		{
			
			try{
				var s = fetchGowallaData("'/users/" + username + "/top_spots");
				return s;
			}catch (Exception e){
				Console.WriteLine(e);
			}
			return "";
		}
		
		public string getNearBySpotsFromLatitudeAndLongitude(double lat, double lng, double radius)
		{
			try{
				var s = fetchGowallaData("/spots?lat=" + lat + "&lng=" + lng + "&radius=" + radius);
				return s;
			}catch (Exception e){
				Console.WriteLine(e);
			}
			return "";
		}
		
		public string getSpotInfoById(string id)
		{
			try{
				var s = fetchGowallaData("/spots/" + id);
				return s;
			}catch(Exception e){
				Console.WriteLine(e);
			}
			return "";
		}
		
		public string getSpotEventsById(string id)
		{
			try{
				var s = fetchGowallaData ("/spots/" + id + "/events");
				return s;
			}catch (Exception e){
				Console.WriteLine(e);
			}
			
			return "";
		}
		
		public string getSpotItemsById(string id)
		{
			try{
				var s = fetchGowallaData ("/spots/" + id + "/items");
				return s;
			}catch (Exception e){
				Console.WriteLine(e);
			}
			
			return "";
		}
		
		public string getCategories()
		{
			try{
				var s = fetchGowallaData ("/categories");
				return s;
			}catch (Exception e){
				Console.WriteLine(e);
			}
			
			return "";
		}
		
		public string getCategoryInfoById(string id)
		{
			try{
				var s = fetchGowallaData ("/categories/" + id);
				return s;
			}catch (Exception e){
				Console.WriteLine(e);
			}
			
			return "";
		}
		
		public string getItemInfoById(string id)
		{
			try{
				var s = fetchGowallaData ("/items/" + id);
				return s;
			}catch (Exception e){
				Console.WriteLine(e);
			}
			
			return "";
		}
		
		public string getTrips()
		{
			try{
				var s = fetchGowallaData ("/trips");
				return s;
			}catch (Exception e){
				Console.WriteLine(e);
			}
			
			return "";
		}
		
		public string getTripInfoById(string id)
		{
			try{
				var s = fetchGowallaData ("/trips/" + id);
				return s;
			}catch (Exception e){
				Console.WriteLine(e);
			}
			
			return "";
		}
		
		
		public string fetchGowallaData(string path)
		{ 
			var postData = new Dictionary<string,string> () {
				{ "access_token", OAuth.AccessToken }};
			var wc = new WebClient ();
			wc.Headers["Accept"] = "application/json";
			wc.Headers.Add("X-Gowalla-API-Key", OAuth.APIKey);
		
			var pd = PreparePostData(postData);
			try { 
				foreach(var h in wc.Headers)
				{
					Console.WriteLine(h.ToString() + ": " + wc.Headers[h.ToString()].ToString());
				}
				Console.WriteLine(baseUrl + path); //+ "?" + pd );
				//var s = wc.UploadString(baseUrl + path, "");//"?" + pd );
				var b = wc.DownloadData (baseUrl + path + "?" + pd );
				var s = Encoding.UTF8.GetString(b);
			
				if(!string.IsNullOrEmpty(s.ToString()))
				{
					return s.ToString();
				}
			} catch (WebException e) {
				var x = e.Response.GetResponseStream ();
				var j = new System.IO.StreamReader (x);
				Console.WriteLine (j.ReadToEnd ());
				Console.WriteLine (e);
				// fallthrough for errors
			}
			return "";
			
		}
	}
}

