using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using XboxAuthNet.OAuth;
using XboxAuthNet.XboxLive;

namespace XboxAuthNet.Test
{
    [TestFixture]
    public class TestXboxAuth
    {
        private XboxAuth? xbox;

        [SetUp]
        public void SetUp()
        {
            xbox = new XboxAuth(new HttpClient());
        }

        static object[] TestExceptionCases =
        {
            new object?[]
            {
                "{\"XErr\":\"2148916233\", \"Message\":\"Message\", \"Redirect\":\"Redirect\"}",
                HttpStatusCode.BadRequest,
                "8015dc09, Message", "8015dc09", "Message", "Redirect"
            },
            new object?[]
            {
                "{\"XErr\":\"XErr\", \"Redirect\":\"Redirect\"}",
                HttpStatusCode.BadRequest,
                "XErr", "XErr", null, "Redirect"
            },
            new object?[]
            {
                "{\"Message\":\"Message\", \"Redirect\":\"Redirect\"}",
                HttpStatusCode.BadRequest,
                "Message", null, "Message", "Redirect"
            },
            new object?[]
            {
                "\"Redirect\":\"Redirect\"}",
                HttpStatusCode.BadRequest,
                "400: Bad Request", null, null, null
            },
            new object?[]
            {
                "{}",
                HttpStatusCode.BadRequest,
                "400: Bad Request", null, null, null
            },
            new object?[]
            {
                "asidjfoawiejf",
                HttpStatusCode.BadRequest,
                "400: Bad Request", null, null, null
            },
            new object?[]
            {
                "",
                HttpStatusCode.BadRequest,
                "400: Bad Request", null, null, null
            },
        };

        [Test, Combinatorial]
        [TestCaseSource(nameof(TestExceptionCases))]
        public void TestException(string resBody, HttpStatusCode statusCode, 
            string expectedMessage, string expectedError, string expectedErrorMessage, string expectedRedirect)
        {
            var res = new HttpResponseMessage();
            res.Content = new StringContent(resBody);
            res.StatusCode = statusCode;

            var ex = Assert.ThrowsAsync<XboxAuthException>(async () => await xbox!.handleXboxResponse(res));
            Assert.NotNull(ex);
            Assert.AreEqual(expectedMessage, ex!.Message);
            Assert.AreEqual(expectedError, ex!.Error);
            Assert.AreEqual(expectedErrorMessage, ex!.ErrorMessage);
            Assert.AreEqual(expectedRedirect, ex!.Redirect);
        }

    }
}
