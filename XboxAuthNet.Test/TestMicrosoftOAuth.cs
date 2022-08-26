using NUnit.Framework;
using System.Net;
using System.Net.Http;
using XboxAuthNet.OAuth;

namespace XboxAuthNet.Test
{
    [TestFixture]
    public class TestMicrosoftOAuth
    {
        private MicrosoftOAuth? microsoft;

        [SetUp]
        public void SetUp()
        {
            microsoft = new MicrosoftOAuth("", "", new HttpClient());
        }

        static object[] TestExceptionCases =
        {
            new object?[]
            {
                "{\"error\":\"error\", \"error_description\":\"errorDescription\", \"error_codes\":[1,2,3]}",
                HttpStatusCode.BadRequest,
                "error, errorDescription", "error", "errorDescription"
            },
            new object?[]
            {
                "{\"error\":\"error\"}",
                HttpStatusCode.BadRequest,
                "error", "error", null
            },
            new object?[]
            {
                "{\"error_description\":\"errorDescription\", \"error_codes\":[1,2,3]}",
                HttpStatusCode.BadRequest,
                "errorDescription", null, "errorDescription"
            },
            new object?[]
            {
                "{\"error_codes\":[1,2,3]}",
                HttpStatusCode.BadRequest,
                "400: Bad Request", null, null
            },
            new object?[]
            {
                "{}",
                HttpStatusCode.BadRequest,
                "400: Bad Request", null, null
            },
            new object?[]
            {
                "asidjfoawiejf",
                HttpStatusCode.BadRequest,
                "400: Bad Request", null, null
            },
            new object?[]
            {
                "",
                HttpStatusCode.BadRequest,
                "400: Bad Request", null, null
            },
        };

        [Test, Combinatorial]
        [TestCaseSource(nameof(TestExceptionCases))]
        public void TestException(string resBody, HttpStatusCode statusCode, 
            string expectedMessage, string expectedError, string expectedErrorDescription)
        {
            var res = new HttpResponseMessage();
            res.Content = new StringContent(resBody);
            res.StatusCode = statusCode;

            var ex = Assert.ThrowsAsync<MicrosoftOAuthException>(async () => await microsoft!.handleMicrosoftOAuthResponse(res));
            Assert.NotNull(ex);
            Assert.AreEqual(expectedMessage, ex!.Message);
            Assert.AreEqual(expectedError, ex!.Error);
            Assert.AreEqual(expectedErrorDescription, ex!.ErrorDescription);
        }

    }
}
