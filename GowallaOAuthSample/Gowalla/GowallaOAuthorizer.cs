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
			
	public class GowallaOAuthConfig {
		// keys, callbacks
		public string APIKey, SecretKey, Callback;

		// Urls
		public string OAuthUrl, RequestTokenUrl;
	}
	
	public static class OAuth {

		// 
		// This url encoder is different than regular Url encoding found in .NET 
		// as it is used to compute the signature based on a url.   Every document
		// on the web omits this little detail leading to wasting everyone's time.
		//
		// This has got to be one of the lamest specs and requirements ever produced
		//
		public static string PercentEncode (string s)
		{
			var sb = new StringBuilder ();

			foreach (byte c in Encoding.UTF8.GetBytes (s)){
				if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9') || c == '-' || c == '_' || c == '.' || c == '~')
					sb.Append ((char) c);
				else {
					sb.AppendFormat ("%{0:X2}", c);
				}
			}
			return sb.ToString ();
		}
	}
	
	
	public class GowallaOAuthorizer
	{
		GowallaOAuthConfig config;
		
		public string RefreshToken;
		public string AccessToken, AccessScreenname;
		public string AccessCode;
		public long AccessId;
		public string ExpiresIn;
		public DateTime ExpiresAt;
		public string APIKey;

		public static GowallaOAuthConfig OAuthConfig = new GowallaOAuthConfig () {
			APIKey = "d2d4d68e0fb045be8b6a707dd2ee8c8c",
			SecretKey = "6f96ed3664b44a008c4888a80964ab23",
			Callback = "http://martinbowling.com/Gowalla/OAuth",
			OAuthUrl = "https://gowalla.com/api/oauth/new",
			RequestTokenUrl = "https://api.gowalla.com/api/oauth/token"
		};
		
		public GowallaOAuthorizer()
		{
			config = OAuthConfig;
		}
		
		public GowallaOAuthorizer (GowallaOAuthConfig Config)
		{
			config = Config;
		}
		
		static string PreparePostData (Dictionary<string,string> postData)
		{
			return String.Join ("&", (from x in postData.Keys select String.Format ("{0}={1}", x, postData [x])).ToArray ());
		}
		
		public bool AcquireAccessToken()
		{
 
			var postData = new Dictionary<string,string> () {
				{ "grant_type", "authorization_code" },
				{ "client_id", config.APIKey },
				{ "client_secret", config.SecretKey },
				{ "code", AccessCode },
				{ "redirect_uri", config.Callback },
				{ "scope", "read-write"}};
			var wc = new WebClient ();
			var pd = PreparePostData(postData);
			try {
				
				var s = wc.UploadString(config.RequestTokenUrl + "?", pd );
				//var b =  //wc.UploadString (new Uri (config.AccessTokenUrl), content));
				//var s = Encoding.UTF8.GetString(b);
				if(!string.IsNullOrEmpty(s))
				{
					try{
						var c = JsonConvert.DeserializeObject<OAuthToken>( s );
						if (c != null)
						{
							AccessToken = c.access_token;
							RefreshToken = c.refresh_token;
							AccessScreenname = c.username;
							ExpiresIn = c.expires_in;
							ExpiresAt = c.expires_at;
							APIKey = config.APIKey;
							
							//ExpiresIn = c.expires_in;
						}
						else { return false;}
					return true;
					} catch (Exception je) {
						return false;
					}
				}
			} catch (WebException e) {
				var x = e.Response.GetResponseStream ();
				var j = new System.IO.StreamReader (x);
				Console.WriteLine (j.ReadToEnd ());
				Console.WriteLine (e);
				// fallthrough for errors
			}
			return false;

		}
		
		class GowallaAuthorizationViewController : WebViewController {
			NSAction callback;
			GowallaOAuthorizer container;
			string url;
			
			public GowallaAuthorizationViewController (GowallaOAuthorizer oauth, string url, NSAction callback)
			{
				this.url = url;
				this.container = oauth;
				this.callback = callback;

				SetupWeb (url);
			}

			protected override string UpdateTitle ()
			{
				return "Authorization";
			}

			public override void ViewWillAppear (bool animated)
			{
				SetupWeb ("Authorization");
				WebView.ShouldStartLoad = LoadHook;
				WebView.LoadRequest (new NSUrlRequest (new NSUrl (url)));
				base.ViewWillAppear (animated);
			}

			bool LoadHook (UIWebView sender, NSUrlRequest request, UIWebViewNavigationType navType)
			{
				var requestString = request.Url.AbsoluteString;
				if (requestString.StartsWith (container.config.Callback)){
					var results = HttpUtility.ParseQueryString (requestString.Substring (container.config.Callback.Length+1));
					container.AccessCode = results["code"];
					DismissModalViewControllerAnimated (false);

					container.AcquireAccessToken ();
					callback ();
				}
				return true;
			}
		}

		public void AuthorizeUser (DialogViewController parent, NSAction callback)
		{
			//NSString * OAuthURLString = [kGowallaOAuthURL stringByAppendingFormat:@"?redirect_uri=%@&client_id=%@&scope=%@", kGowallaRedirectURI, kGowallaAPIKey, @"read-write"];
			var authweb = new GowallaAuthorizationViewController (this, config.OAuthUrl + "?redirect_uri=" + config.Callback + "&client_id=" + config.APIKey + "&scope=read-write", callback);

			parent.PresentModalViewController (authweb, true);
		}

	}
}

