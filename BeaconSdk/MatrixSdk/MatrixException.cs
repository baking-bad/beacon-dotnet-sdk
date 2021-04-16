// ReSharper disable MemberCanBePrivate.Global

namespace MatrixSdk
{
    using System;
    using System.Net;

    public class MatrixException : Exception
    {
        public MatrixException(Uri uri, string requestContent, string responseContent, HttpStatusCode statusCode)
            : base($"Matrix API error. Status: {statusCode}, json: {responseContent}")
        {
            Uri = uri;
            RequestContent = requestContent;
            ResponseContent = responseContent;
            StatusCode = statusCode;
        }

        public MatrixException(Uri uri, string responseContent, HttpStatusCode statusCode)
            : this(uri, "", responseContent, statusCode)
        {
        }

        public Uri Uri { get; }

        public string RequestContent { get; }

        public string ResponseContent { get; }

        public HttpStatusCode StatusCode { get; }
    }
}