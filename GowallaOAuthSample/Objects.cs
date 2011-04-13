using System;
using MonoTouch.Foundation;
using System.Collections.Generic;

namespace GowallaOAuthSample
{
	[MonoTouch.Foundation.Preserve]
	public class OAuthToken
	{
		
		/*
		 *"{\"scope\":\"read-write\",\"expires_at\":\"Wed, 16 Mar 2011 21:27:46 -0000\",\"username\":\"martinbowling\",\"expires_in\":1209599,\"refresh_token\":\"8aeb8c03467382cf925666c008d52cdf\",\"access_token\":\"6a176418807eacdd0d1757c3a9668684\"}"
		 */ 
		public string scope;
		public DateTime expires_at;
		public string username;
		public string expires_in;
		public string refresh_token;
		public string access_token;
	}
}

