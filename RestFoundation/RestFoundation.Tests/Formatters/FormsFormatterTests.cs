﻿using System;
using System.IO;
using System.Text;
using System.Web;
using NUnit.Framework;
using RestFoundation.Formatters;
using RestFoundation.Runtime;
using RestFoundation.Tests.Implementation.Models;
using RestFoundation.UnitTesting;

namespace RestFoundation.Tests.Formatters
{
    public class FormsFormatterTests
    {
        private IServiceContext m_context;

        [SetUp]
        public void Initialize()
        {
            m_context = MockContextManager.GenerateContext();
        }

        [TearDown]
        public void ShutDown()
        {
            MockContextManager.DestroyContext();
        }

        [Test]
        public void FormatRequest()
        {
            Model model = CreateModel();

            WriteBodyAsFormsEncodedString(model);

            var formatter = new FormsFormatter();
            var resource = formatter.FormatRequest(m_context, typeof(Model)) as Model;

            Assert.That(resource, Is.Not.Null);
            Assert.That(resource.Id, Is.EqualTo(model.Id));
            Assert.That(resource.Name, Is.EqualTo(model.Name));
            Assert.That(resource.Items, Is.Not.Null);
            CollectionAssert.AreEqual(model.Items, resource.Items);
        }

        [Test]
        public void FormatResponse()
        {
            Model model = CreateModel();

            var formatter = new FormsFormatter();
            Assert.That(formatter.CanFormatResponse, Is.False);

            // "application/x-www-form-urlencoded" be used as an accepted media type
            Assert.Throws(typeof(NotSupportedException), () => formatter.FormatResponse(m_context, typeof(Model), model, null));
        }

        private static Model CreateModel()
        {
            var model = new Model
            {
                Id = 1,
                Name = "John Doe",
                Items = new[] { "A", "B", "C" }
            };

            return model;
        }

        private void WriteBodyAsFormsEncodedString(Model model)
        {
            var nameValueBuilder = new StringBuilder();
            nameValueBuilder.Append("ID=").Append(HttpUtility.UrlEncode(model.Id.ToString()));
            nameValueBuilder.Append("&Name=").Append(HttpUtility.UrlEncode(model.Name));

            foreach (string item in model.Items)
            {
                nameValueBuilder.Append("&Items=").Append(HttpUtility.UrlEncode(item));
            }

            var writer = new StreamWriter(m_context.Request.Body, Encoding.UTF8);
            writer.Write(nameValueBuilder.ToString());
            writer.Flush();
        }
    }
}
