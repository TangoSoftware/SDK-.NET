﻿using System.Collections.Generic;
using System.Net;
using System.Xml;
using System.IO;
using System;
using Newtonsoft.Json;
using TodoPagoConnector.Model;
using TodoPagoConnector.Exceptions;
using TodoPagoConnector.Utils;
using TodoPagoConnector.Operations;

namespace TodoPagoConnector {

    public class RestConnector {

        protected string endpoint = "";
        protected Dictionary<string, string> headders;
        protected WebClient client;

        protected const string CONTENT_TYPE_APP_JSON = "application/json";
        protected const string METHOD_POST = "POST";
        protected const string METHOD_GET = "GET";

        public RestConnector(string endpoint, Dictionary<string, string> headders){

            this.endpoint = endpoint;
            client = new WebClient();

            IWebProxy theProxy = client.Proxy;
            if (theProxy != null){
                theProxy.Credentials = CredentialCache.DefaultCredentials;
            }
            if (headders!= null) { 
                 this.headders = headders;
                 foreach (var key in this.headders.Keys){
                     client.Headers.Add(key, this.headders[key]);
                 } 
            }
        }

        protected HttpWebRequest generateHttpWebRequest(String URL, String contentType, String method, Boolean withApiKey) {

             var httpWebRequest = (HttpWebRequest)WebRequest.Create(URL);

             if (contentType != null && !contentType.Equals("")){
                 httpWebRequest.ContentType = contentType;
             } else {
                 httpWebRequest.ContentType = CONTENT_TYPE_APP_JSON;
             }

             if (method != null && !method.Equals("")){
                 httpWebRequest.Method = method;
             }else{
                 httpWebRequest.Method = METHOD_POST;
             }

             IWebProxy theProxy = httpWebRequest.Proxy;

             if (theProxy != null) {
                 theProxy.Credentials = CredentialCache.DefaultCredentials;
             }

             if (withApiKey != null && withApiKey != false){
                 foreach (var key in this.headders.Keys) {
                     httpWebRequest.Headers.Add(key, this.headders[key]);
                 }
            }           
             return httpWebRequest;
        }

        protected Dictionary<string, object> toDictionary(XmlDocument xd){

            Dictionary<string, object> rv = new Dictionary<string, object>();
            Dictionary<string, object> ret = new Dictionary<string, object>();
            foreach (XmlNode child in xd.ChildNodes){
                rv = toDictionary(child);

                if (!rv.ContainsKey(" ")){
                    foreach (string k in rv.Keys){
                        //Failsafe, exception thrown on adding a key that already exists
                        if (!ret.ContainsKey(k)){
                            ret.Add(k, rv[k]);
                        }
                    }
                }else{
                    ret.Add(child.Name, rv[" "]);
                }
            }
            return ret;
        }

        protected Dictionary<string, object> toDictionary(XmlNode xd){

            Dictionary<string, object> ret = new Dictionary<string, object>();
            string keyVal = " ";
            foreach (XmlNode child in xd.ChildNodes){
                if (child.HasChildNodes && child.FirstChild.HasChildNodes){
                    Dictionary<string, object> rv = toDictionary(child);
                    if (!rv.ContainsKey(" ")) {
                        foreach (string k in rv.Keys) {
                            //Failsafe, exception thrown on adding a key that already exists
                            if (!ret.ContainsKey(k)) {
                                ret.Add(k, rv[k]);
                            }
                        }
                    }else{
                        ret.Add(child.Name, rv[" "]);
                    }
                } else{
                    if (child.Name.Contains("Id")) {
                        keyVal = child.InnerText;
                    }
                    ret.Add(child.Name, child.InnerText);
                }
            }
            Dictionary<string, object> aux = new Dictionary<string, object>();
            aux.Add(keyVal, ret);
            return aux;
        }




    }
}
