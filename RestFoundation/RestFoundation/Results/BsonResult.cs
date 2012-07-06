﻿using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using RestFoundation.Runtime;

namespace RestFoundation.Results
{
    public class BsonResult : IResult
    {
        public BsonResult()
        {
            ContentType = "application/bson";
        }

        public object Content { get; set; }
        public string ContentType { get; set; }

        public virtual void Execute(IServiceContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            if (Content == null)
            {
                return;
            }

            context.Response.Output.Clear();
            context.Response.SetHeader("Content-Type", ContentType);
            context.Response.SetCharsetEncoding(context.Request.Headers.AcceptCharsetEncoding);

            OutputCompressionManager.FilterResponse(context);

            var serializer = new JsonSerializer();
            var bsonWriter = new BsonWriter(context.Response.Output.Stream);
            serializer.Serialize(bsonWriter, Content);
        }
    }
}
