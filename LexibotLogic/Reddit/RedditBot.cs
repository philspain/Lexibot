using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Globalization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Data.Entity;
using System.Data;
using System.Reflection;
using System.Diagnostics;
using System.Threading;

using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using LexibotData.Models;
using LexibotLogic.Parsers;
using LexibotData.DataAccess;
using LexibotLogic.DIContainer;


namespace LexibotLogic.Reddit
{
    public class RedditBot : IRedditBot
    {
        IWordParser _wordParser;
        IEntityDbManager _dbManager;
        CookieContainer _cookieJar;
        WebClient _redditClient;
        HttpWebRequest _request;
        Logger _logger;
        JObject _me;

        //reddit link strings
        readonly string _redditAPI = "http://www.reddit.com/api/";
        readonly string _redditAll = "http://www.reddit.com/r/all";
        readonly string _redditComments = "http://www.reddit.com/comments/";
        readonly string _redditTest = "http://www.reddit.com/r/bot_test";
        readonly string _user = "";
        readonly string _pass = "";

        public RedditBot()
            : this(DependencyFactory.Resolve<IWordParser>(),
                   DependencyFactory.Resolve<IEntityDbManager>())
        {

        }               

        public RedditBot(IWordParser wordParser, IEntityDbManager dbManager)
        {
            _cookieJar = new CookieContainer();
            _redditClient = new WebClient();
            _wordParser = wordParser;
            _dbManager = dbManager;

            Login();
            _me = GetMe();            
        }

        /// <summary>
        /// Login to reddit
        /// </summary>
        void Login()
        {
            // initialise a web request
            _request = WebRequest.Create(String.Format("{0}login/{1}", _redditAPI, _user)) as HttpWebRequest;
            _request.Method = "POST";
            _request.ContentType = "application/x-www-form-urlencoded";
            _request.CookieContainer = _cookieJar;

            // encode data for POST
            string postData = String.Format("api_type=json&passwd={0}&user={1}", _pass, _user);
            byte[] encodedData = ASCIIEncoding.ASCII.GetBytes(postData);
            _request.ContentLength = encodedData.Length;

            // initialise stream to send as request
            Stream requestStream = _request.GetRequestStream();
            requestStream.Write(encodedData, 0, encodedData.Length);
            requestStream.Close();

            string responseText;

            // attempt to send request and receive response
            using (HttpWebResponse response = _request.GetResponse() as HttpWebResponse)
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    responseText = reader.ReadToEnd();
                }
                // reader is disposed here
            }
        }


        JObject GetMe()
        {
            _redditClient.Headers["COOKIE"] =
                _cookieJar.GetCookieHeader(new Uri(String.Format("{0}login/{1}", _redditAPI, _user)));

            JObject myData = null;

            try
            {
                using (Stream myStream = _redditClient.OpenRead(new Uri(String.Format("{0}me.json", _redditAPI))))
                {
                    string response;

                    using (StreamReader reader = new StreamReader(myStream))
                    {
                        response = reader.ReadToEnd();
                    }

                    Config cookieConfig = _dbManager.GetCookieConfig();

                    if (cookieConfig == null)
                    {
                        _dbManager.AddConfig("CookieValue", "Cookie:" + response);
                    }

                    myData = JObject.Parse(response);
                }
            }
            catch(WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    WebResponse resp = e.Response;

                    using(StreamReader sr = new StreamReader(resp.GetResponseStream()))
                    {
                         _logger.LogError(sr.ReadToEnd());
                    }
                }
            }

            return (JObject) myData["data"];
        }

        Dictionary<string, object> GetLinks(string link)
        {
            string linkUrl = _redditAll;

            // Reduce the speed of hits to site, in accordance with reddit's wishes.
            Thread.Sleep(3000);

            Stream linkStream = _redditClient.OpenRead(new Uri(linkUrl));
            Dictionary<string, object> pageDetails = new Dictionary<string, object>();
            string responseText;

            using (StreamReader reader = new StreamReader(linkStream))
            {
                responseText = reader.ReadToEnd();
            }

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(responseText);

            HtmlNodeCollection linkElems = htmlDoc.DocumentNode.SelectNodes("//a");

            List<string> links = new List<string>();
            string nextLink = String.Empty;

            foreach (HtmlNode elem in linkElems)
            {
                string linkClass = elem.GetAttributeValue("class", String.Empty);
                string innerText = elem.InnerText;

                if (linkClass != null && linkClass != String.Empty)
                {
                    if (linkClass.Contains("comments"))
                    {
                        links.Add(elem.GetAttributeValue("href", String.Empty));
                    }
                }

                if (innerText == "next")
                {
                    nextLink = elem.GetAttributeValue("href", String.Empty);
                }
            }

            pageDetails.Add("NextLink", nextLink);
            pageDetails.Add("Links", links);
            return pageDetails;
        }

        Dictionary<string, List<Comment>> GetCommentCache(Dictionary<string, object> links)
        {
            Dictionary<string, List<Comment>> commentCache = new Dictionary<string, List<Comment>>();

            foreach (string link in links["Links"] as List<string>)
            {
                string linkId = StripLinkId(link);

                if (!_dbManager.IsThreadParsed(linkId))
                {
                    string commentLink = String.Format("{0}/{1}.json", _redditComments, linkId);
                    string response;

                    using (Stream commentStream = _redditClient.OpenRead(commentLink))
                    {
                        using (StreamReader reader = new StreamReader(commentStream))
                        {
                            response = reader.ReadToEnd();
                            Debug.WriteLine(response);
                        }
                    }

                    JArray responseData = JArray.Parse(response);
                    JObject commentData = (JObject) responseData[1];

                    JObject data = (JObject) commentData["data"];
                    JArray children = (JArray) data["children"];

                    GetComments(link, children, commentCache);
                }

                // Reduce the speed of hits to site, in accordance with reddit's wishes.
                Thread.Sleep(3000);
            }

            return commentCache;
        }

        void GetComments(string link, JArray children, Dictionary<string, List<Comment>> commentCache)
        {
            if (!commentCache.Keys.Contains(link))
            {
                commentCache.Add(link, new List<Comment>());
            }

            foreach (JObject child in children)
            {
                JObject childData = (JObject) child["data"];

                if (((string) childData["author"]) != _user)
                {
                    string id = (string) childData["name"];
                    string permalink = link + ((string) childData["id"]);
                    string text = (string) childData["body"];
                    string subreddit = (string)childData["subreddit"];

                    decimal tempCreatedUTC = 0;

                    if (childData["created"] != null)
                    {
                        tempCreatedUTC = (decimal) childData["created"];
                    }

                    DateTime createdUTC = new DateTime(1970, 1, 1).AddSeconds((double)tempCreatedUTC);

                    // Remove minutes and seconds as any graphs created on comment data
                    // will only be resolved, at a maximum, to the precision of the
                    // hour of the day the comment was posted.
                    createdUTC = createdUTC.Subtract(new TimeSpan(0, createdUTC.Minute, createdUTC.Second));

                    if (id != null && text != null)
                    {
                        commentCache[link].Add(
                            new Comment
                            {
                                ThingId = id,
                                PermaLink = permalink,
                                Text = text,
                                CreatedUTC = createdUTC,
                                SubReddit = subreddit
                            });
                    }

                    if (childData["replies"] != null && childData["replies"].HasValues)
                    {
                        JObject replies = (JObject) childData["replies"];

                        JArray newBorns = (JArray) replies["data"]["children"];
                        GetComments(link, newBorns, commentCache);
                    }
                }
            }
        }

        void ParseComments(Dictionary<string, List<Comment>> commentCache)
        {
            foreach (string link in commentCache.Keys)
            {
                int threadOcurrences = _dbManager.ThreadOccurrences(link);

                if (threadOcurrences <= 0)
                {
                    foreach (Comment comment in commentCache[link])
                    {
                        List<string> wordsInComment = _wordParser.GetWordsInComment(comment.Text);
                        Dictionary<string, int> wordOccurrences = _wordParser.GetWordOccurrences(wordsInComment);

                        _dbManager.AddWordOccurrences(wordOccurrences, comment.SubReddit, comment.CreatedUTC);  
                    }

                    _dbManager.AddParsedThread(link);
                }
            }
        }

        /// <summary>
        /// Strip link id from a given string.
        /// </summary>
        /// <param name="link">Link to a comment thread</param>
        /// <returns>string</returns>
        string StripLinkId(string link)
        {
            int i = link.IndexOf("comments");
            int j = link.IndexOf("/", i) + 1;
            int k = link.IndexOf("/", j);

            return link.Substring(j, k - j);
        }

        public void Run()
        {
            string nextLink = null;
            int i = 0;

            // Get links for comment threads
            Dictionary<string, object> links = GetLinks(nextLink);

            nextLink = links["NextLink"] as string;

            // Get a cache of comments
            Dictionary<string, List<Comment>> commentCache = GetCommentCache(links);
            ParseComments(commentCache);
            i++;
        }
    }
}
