using UnityEngine;
using System.IO;
using System.Net;
using FullSerializer;

namespace UnityToRails {

    public struct Id
    {

        public int id;
    }

    public struct Status
    {

        public bool success;
        public string status;
        public string errors;

        public override string ToString ()
        {
            return (success ? "Successful" : "Failed") + " response. " +
                "Received status: " + status;
        }
    }

    /**
     * Typical Rails HTTP Verbs to use to with requests to a Rails website.
     */
    public static class HttpVerb
    {

        public const string GET = "GET";
        public const string POST = "POST";
        public const string DELETE = "DELETE";
        public const string PATCH = "PATCH";
        public const string PUT = "PUT";
    }

    /**
     * The root DNS domain name, or IP address of a Rails website.
     *
     * For example:
     * www.example.com
     * 127.0.0.1
     *
     * NOT with the protocol or any trailing parts after the domain.
     * So not:
     * http://example.com
     * or
     * example.com/stuff
     */
    public class Domain
    {
        private string domain;

        /**
         * Create the domain of a Rails website.
         *
         * rootDomain: e.g. example.com, 127.0.0.1
         *     this should be the root domain URL or IP address of the website,
         *     with the leading protocol stripped, and any trailing
         *     sub-directories also stripped.
         */
        public Domain (string rootDomain)
        {
            this.domain = rootDomain;
        }

        public RailsURI At (string subURI)
        {
            return new RailsURI(domain, subURI);
        }
    }

    /**
     * The full URI of a specific webpage on a given Domain.
     */
    public class RailsURI
    {
        private string domain;
        private string subURI;

        public RailsURI (string domain, string subURI)
        {
            this.domain = domain;
            this.subURI = subURI;
        }

        /**
         * Make a RailsRequest for this RailsURI, attaching the given parameters.
         */
        public RailsRequest MakeRequest (CookieContainer container, string token,
                                         string verb = HttpVerb.GET,
                                         bool withHttps = false)
        {
            HttpWebRequest request =
                (HttpWebRequest)WebRequest.Create (ConstructURI (withHttps));
            request.Method = verb;
            request.CookieContainer = container;
            if (token != "")
                request.Headers.Add("X-CSRF-Token", token);
            return new RailsRequest (request);
        }

        // Private helper for constructing the URI
        private string ConstructURI (bool withHttps)
        {
            return withHttps ? "https" : "http" + "://" +
                domain + "/" + subURI;
        }
    }

    /**
     * Represents a HttpWebRequest to a Rails website.
     */
    public class RailsRequest
    {
        private HttpWebRequest request;

        public RailsRequest (HttpWebRequest request)
        {
            this.request = request;
        }

        /**
         * Get response from request, and Deserialize returned json as T.
         *
         * Does not do any exception catching for failed web requests.
         */
        public T GetResponse<T> ()
        {
            using (WebResponse response = request.GetResponse ())
            {
                using (StreamReader reader =
                       new StreamReader (response.GetResponseStream ()))
                {
                    string jsonResponse = reader.ReadToEnd ();
                    T deserializedObj =
                        StringSerialization.Deserialize<T>(jsonResponse);
                    return deserializedObj;
                }
            }
        }

        /**
         * Serialize the given data as json, and add to the request.
         *
         * You still need to call DeserializeGetResponse before this is sent.
         */
        public RailsRequest WriteData<T> (T dataToSend)
        {
            string serializedString =
                StringSerialization.Serialize<T> (dataToSend);
            byte[] encodedData =
                System.Text.Encoding.UTF8.GetBytes (serializedString);

            request.ContentLength = encodedData.Length;
            request.ContentType = "application/json";

            using (Stream stream = request.GetRequestStream ())
            {
                stream.Write (encodedData, 0, encodedData.Length);
            }

            return this;
        }
    }

    /**
     * Represents a HTTP request to log into the website.
     */
    public class Login
    {
        private const string CSRF_TOKEN_SUBURI = "game/csrf_token";
        private const string LOGIN_SUBURI = "login.json";

        private Login () {}

        public static Login To (Domain domain, LoginData data,
                                bool withHttps = false)
        {
            CookieContainer container = new CookieContainer ();

            // Get the csrf authentication token
            string token = domain.At(CSRF_TOKEN_SUBURI)
                .MakeRequest (container, null, verb: HttpVerb.GET,
                              withHttps: withHttps)
                .GetResponse<AuthToken> ()
                .token;

            // Try to login, getting our session id.
            int id = domain.At(LOGIN_SUBURI)
                .MakeRequest (container, token, verb: HttpVerb.POST,
                              withHttps: withHttps)
                .WriteData (data).GetResponse<Id> ()
                .id;

            // Try to edit the user's page, using the id as the credential.
            // Should remove this later.
            Status status =
                domain.At (EditUserSubURI (id))
                .MakeRequest (container, token, verb: HttpVerb.GET,
                              withHttps: withHttps)
                .GetResponse<Status> ();
            Debug.Log(status);

            return new Login();
        }

        private static string EditUserSubURI (int id)
        {
            return "users/" + id + "/edit.json";
        }
    }

    /* Methods for debugging this namespace. */
    public static class Debugging
    {
        public static void printCookies(CookieCollection collection)
        {
            foreach (Cookie cook in collection)
            {
                Debug.LogFormat("Cookie:");
                Debug.LogFormat("{0} = {1}", cook.Name, cook.Value);
                Debug.LogFormat("Domain: {0}", cook.Domain);
                Debug.LogFormat("Path: {0}", cook.Path);
                Debug.LogFormat("Port: {0}", cook.Port);
                Debug.LogFormat("Secure: {0}", cook.Secure);

                Debug.LogFormat("When issued: {0}", cook.TimeStamp);
                Debug.LogFormat("Expires: {0} (expired? {1})",
                                cook.Expires, cook.Expired);
                Debug.LogFormat("Don't save: {0}", cook.Discard);
                Debug.LogFormat("Comment: {0}", cook.Comment);
                Debug.LogFormat("Uri for comments: {0}", cook.CommentUri);
                Debug.LogFormat("Version: RFC {0}" , cook.Version == 1 ? "2109" : "2965");

                // Show the string representation of the cookie.
                Debug.LogFormat ("String: {0}", cook.ToString());
            }
        }
    }
}
