﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Web;
using System.Xml;

namespace OAuth
{
    public class OAuthConsumer : OAuthBase
    {
        private OAuthConfig _oauthConfig;

        public OAuthConfig OauthConfig
        {
            get { return _oauthConfig; }
            set { _oauthConfig = value; }
        } 

        public OAuthConsumer(OAuthConfig oauthConfig, string debugType) : base (debugType)
        {
            this._oauthConfig = oauthConfig;            
        }

        private void _saveTokenDataInConfiguration(string tokens)
        {
            if (tokens.Length == 0) return;
            string[] tokenData = tokens.Split('&');
            string[] tokenValue;
            foreach (string data in tokenData)
            {
                tokenValue = data.Split('=');
                switch (tokenValue[0])
                {
                    case "oauth_token":
                        this._oauthConfig.OauthToken = tokenValue[1];                        
                        break;
                    case "oauth_token_secret":
                        this._oauthConfig.OauthTokenSecret = tokenValue[1];
                        break;
                    case "oauth_token_access":
                        this._oauthConfig.OauthTokenSecret = tokenValue[1];
                        break;
                    case "xoauth_token_ttl":
                        this._oauthConfig.OauthTokenTtl = tokenValue[1];
                        break;
                }                
            }
        }

        private void _openAuthorizationPage()
        {
            string url = this._oauthConfig.UserAuthorizationUrl;
            string oauth_token = this._oauthConfig.OauthToken;
            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("oauth_token", oauth_token));
            url += "?oauth_token=" + oauth_token;
				System.Diagnostics.Process.Start(url);
        }


		private string _getAuthorizationPageUrl()
		{
			string url = this._oauthConfig.UserAuthorizationUrl;
			string oauth_token = this._oauthConfig.OauthToken;
			List<QueryParameter> parameters = new List<QueryParameter>();
			parameters.Add(new QueryParameter("oauth_token", oauth_token));
			url += "?oauth_token=" + oauth_token;
			return url;
		}

		public string getRequestToken()
		{
			return getRequestToken(null, "");
		}

    	public string getRequestToken(List<QueryParameter> extraparameters, string postextra)
        {
            string oauth_token = this._oauthConfig.OauthToken;
            OAuthRequest request = new OAuthRequest(this, base._debugType);
			string tokens = request.request(new Uri(this._oauthConfig.RequestTokenUrl), "GET", oauth_token, "", extraparameters, postextra	).ToString();
            if (tokens == String.Empty || tokens.Length == 0) return null;

            // Save Tokens in Configuration            
            this._saveTokenDataInConfiguration(tokens);
            
            // Open Authorization Page
            this._openAuthorizationPage();
            return tokens;
        }
		
		public string getRequestTokenRedirectUrl()
		{
			return getRequestTokenRedirectUrl(null,"");
		}

    	public string getRequestTokenRedirectUrl(List<QueryParameter> extraparameters, string extrapost)
		{
			string oauth_token = this._oauthConfig.OauthToken;
			OAuthRequest request = new OAuthRequest(this, base._debugType);
			string tokens = request.request(new Uri(this._oauthConfig.RequestTokenUrl), "GET", oauth_token, "", extraparameters, extrapost).ToString();
			if (tokens == String.Empty || tokens.Length == 0) return null;

			// Save Tokens in Configuration            
			this._saveTokenDataInConfiguration(tokens);

			// Open Authorization Page
			return this._getAuthorizationPageUrl();
		}

        public string getAccessToken(string oauth_verifier)
        {
            string oauth_token = this._oauthConfig.OauthToken;
            string oauth_token_secret = this._oauthConfig.OauthTokenSecret;            
            List<QueryParameter> parameters = new List<QueryParameter>();
            //parameters.Add(new QueryParameter("oauth_token", oauth_token));
            parameters.Add(new QueryParameter("oauth_verifier", oauth_verifier));
            OAuthRequest request = new OAuthRequest(this, base._debugType);
            string tokens = request.request(new Uri(this._oauthConfig.AccessTokenUrl), "GET", oauth_token, oauth_token_secret, parameters, "").ToString();
            if (tokens == String.Empty || tokens.Length == 0) return null;

            // Save Tokens in Configuration
            this._saveTokenDataInConfiguration(tokens);

            return tokens;
        }

        public Object request(string url, string httpMethod, List<QueryParameter> parameters, string responseFormat)
        {            
            string oauth_token = this._oauthConfig.OauthToken;
            string oauth_token_secret = this._oauthConfig.OauthTokenSecret;
            OAuthRequest request = new OAuthRequest(this, base._debugType);
            string response = request.request(new Uri(url), httpMethod, oauth_token, oauth_token_secret, parameters, "").ToString();
            if (response == String.Empty || response.Length == 0)
            {
                base._debug("The Request Response was empty");
                return null;
            }
            Object result = null;
                        
            switch (responseFormat)
            {
                case "DataSet":
                    System.IO.StringReader strreader = new System.IO.StringReader(response);
                    DataSet ds = new DataSet();
                    ds.ReadXml(strreader);
                    result = ds;
                    break;
                case "XML": 
                    System.IO.StringReader strxmlreader = new System.IO.StringReader(response);
                    XmlTextReader xmlReader = new XmlTextReader(strxmlreader);
                    xmlReader.Read();
                    result = xmlReader;
                    break;
                case "PLAIN":
                default:
                    result = response;
                    break;
            }
            return result;
        }       
        
    }
}
