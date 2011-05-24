using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OAuth
{
    public class OAuthConfig : OAuthBase
    { 
    	// public string SiteUrl { get; set; }
		public string ConsumerKey { get; set; }
		public string ConsumerSecret { get; set; }
		public string OauthToken { get; set; }
		public string OauthTokenSecret { get; set; }
		public string OauthTokenTtl { get; set; }
		public string RequestTokenUrl { get; set; }
		public string AccessTokenUrl { get; set; }
		public string UserAuthorizationUrl { get; set; }
		public string OauthVersion { get; set; }
		public string OauthSignatureMethod { get; set; }

        /**
         * Constructor
         */
        public OAuthConfig(string debugType) : base(debugType)
        {            
            // nothing to do
        }

    }
}
