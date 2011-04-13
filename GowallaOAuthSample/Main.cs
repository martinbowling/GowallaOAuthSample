
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Threading;
using MonoTouch.Dialog;


namespace GowallaOAuthSample
{
	public class Application
	{
		static void Main (string[] args)
		{
			UIApplication.Main (args);
		}
	}

	// The name AppDelegate is referenced in the MainWindow.xib file.
	public partial class AppDelegate : UIApplicationDelegate
	{
		UINavigationController nav = new UINavigationController();
		
		DialogViewController dvc;
		
		// This method is invoked when the application has loaded its UI and its ready to run
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			// If you have defined a view, add it here:
			// window.AddSubview (navigationController.View);
			
			var root = new RootElement ("Login") { 
				new Section() { 
					new StringElement("OAuth Me", delegate{ StartLogin(); }) 
				}
			};
			
			dvc = new DialogViewController(root);
			nav.PushViewController(dvc, true);
			
			window.AddSubview(nav.View);
			window.MakeKeyAndVisible ();
			
			return true;
		}
		
		public void StartLogin()
		{
			ThreadPool.QueueUserWorkItem (delegate {
				var oauth = new GowallaOAuthorizer();

				try {
	
						BeginInvokeOnMainThread (delegate {
							oauth.AuthorizeUser (dvc, delegate {
								StartupAfterAuthorization (oauth);
							});
						});
						return;
					
				} catch (Exception e){
					Console.WriteLine (e);
				}

				BeginInvokeOnMainThread (delegate { Console.WriteLine("FE"); });
			});
			
		}
		
		public void StartupAfterAuthorization(GowallaOAuthorizer OAuth)
		{
			Console.WriteLine(OAuth.AccessToken + " " + OAuth.AccessCode + " " + OAuth.RefreshToken);	
			GowallaAPI api = new GowallaAPI(OAuth);
			Console.WriteLine(api.getPassportByUsername("martinbowling"));
			Console.WriteLine(api.getStampsForUsername("martinbowling"));
		}
		
		// This method is required in iPhoneOS 3.0
		public override void OnActivated (UIApplication application)
		{
		}
	}
}

