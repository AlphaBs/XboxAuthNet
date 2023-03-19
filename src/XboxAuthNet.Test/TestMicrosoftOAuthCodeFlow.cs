using NUnit.Framework;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using XboxAuthNet.OAuth;
using XboxAuthNet.OAuth.Models;

namespace XboxAuthNet.Test
{
    [TestFixture]
    public class TestMicrosoftOAuthCodeFlow
    {
        private MicrosoftOAuthCodeApiClient? microsoft;

        [SetUp]
        public void SetUp()
        {
            microsoft = new MicrosoftOAuthCodeApiClient("", "", new HttpClient());
        }

        // testcase from https://learn.microsoft.com/en-us/azure/active-directory/develop/v2-oauth2-auth-code-flow#successful-response-2

        [Test]
        [TestCase("{\"access_token\":\"eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Ik5HVEZ2ZEstZnl0aEV1Q...\",\"token_type\":\"Bearer\",\"expires_in\":3599,\"scope\":\"https%3A%2F%2Fgraph.microsoft.com%2Fmail.read\",\"refresh_token\":\"AwABAAAAvPM1KaPlrEqdFSBzjqfTGAMxZGUTdM0t4B4...\",\"id_token\":\"eyJ0eXAiOiJKV1QiLCJhbGciOiJub25lIn0.eyJhdWQiOiIyZDRkMTFhMi1mODE0LTQ2YTctOD...\"}", 
                  "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Ik5HVEZ2ZEstZnl0aEV1Q...", 
                  "AwABAAAAvPM1KaPlrEqdFSBzjqfTGAMxZGUTdM0t4B4...")]
        public async Task TestSuccessResponse(string resBody, string expectedAccessToken, string expectedRefreshToken)
        {
            var res = createMockHttpResponse(resBody, HttpStatusCode.OK);
            var result = await microsoft!.handleMicrosoftOAuthResponse(res);
            Assert.AreEqual(expectedAccessToken, result.AccessToken);
            Assert.AreEqual(expectedRefreshToken, result.RawRefreshToken);
        }

        [Test]
        [TestCase("{\"access_token\":\"eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Ik5HVEZ2ZEstZnl0aEV1Q...\",\"token_type\":\"Bearer\",\"expires_in\":3599,\"scope\":\"https%3A%2F%2Fgraph.microsoft.com%2Fmail.read\",\"refresh_token\":\"AwABAAAAvPM1KaPlrEqdFSBzjqfTGAMxZGUTdM0t4B4...\",\"id_token\":\"eyJ0eXAiOiJKV1QiLCJhbGciOiJub25lIn0.eyJhdWQiOiIyZDRkMTFhMi1mODE0LTQ2YTctOD...\"}",
                  3599)]
        public async Task TestExpiresOn(string resBody, int expiresIn)
        {
            var res = createMockHttpResponse(resBody, HttpStatusCode.OK);
            var result = await microsoft!.handleMicrosoftOAuthResponse(res);

            var expectedExpiresOn = DateTime.UtcNow.AddSeconds(expiresIn);
            var delta = expectedExpiresOn - result.ExpiresOn;
            Assert.True(delta < TimeSpan.FromMilliseconds(100));
        }

        [Test]
        [TestCase("{\"access_token\":\"eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Ik5HVEZ2ZEstZnl0aEV1Q...\",\"token_type\":\"Bearer\",\"expires_in\":3599,\"scope\":\"https%3A%2F%2Fgraph.microsoft.com%2Fmail.read\",\"refresh_token\":\"AwABAAAAvPM1KaPlrEqdFSBzjqfTGAMxZGUTdM0t4B4...\",\"id_token\":\"eyJ0eXAiOiJKV1QiLCJhbGciOiJub25lIn0.eyJhdWQiOiIyZDRkMTFhMi1mODE0LTQ2YTctOD...\"}")]
        public async Task TestValidateResponse(string resBody)
        {
            var res = createMockHttpResponse(resBody, HttpStatusCode.OK);
            var result = await microsoft!.handleMicrosoftOAuthResponse(res);
            Assert.True(result.Validate());
        }

        [Test]
        [TestCase("{\"access_token\":\"\",\"token_type\":\"Bearer\",\"expires_in\":3599,\"scope\":\"https%3A%2F%2Fgraph.microsoft.com%2Fmail.read\",\"refresh_token\":\"AwABAAAAvPM1KaPlrEqdFSBzjqfTGAMxZGUTdM0t4B4...\",\"id_token\":\"eyJ0eXAiOiJKV1QiLCJhbGciOiJub25lIn0.eyJhdWQiOiIyZDRkMTFhMi1mODE0LTQ2YTctOD...\"}")]
        [TestCase("{\"access_token\":\"something\",\"token_type\":\"Bearer\",\"expires_in\":0,\"scope\":\"https%3A%2F%2Fgraph.microsoft.com%2Fmail.read\",\"refresh_token\":\"AwABAAAAvPM1KaPlrEqdFSBzjqfTGAMxZGUTdM0t4B4...\",\"id_token\":\"eyJ0eXAiOiJKV1QiLCJhbGciOiJub25lIn0.eyJhdWQiOiIyZDRkMTFhMi1mODE0LTQ2YTctOD...\"}")]
        public async Task TestInvalidateResponse(string resBody)
        {
            var res = createMockHttpResponse(resBody, HttpStatusCode.OK);
            var result = await microsoft!.handleMicrosoftOAuthResponse(res);
            
            await Task.Delay(1000); // to expire token
            Assert.False(result.Validate());
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
            var res = createMockHttpResponse(resBody, statusCode);

            var ex = Assert.ThrowsAsync<MicrosoftOAuthException>(async () => await microsoft!.handleMicrosoftOAuthResponse(res));
            Assert.NotNull(ex);
            Assert.AreEqual(expectedMessage, ex!.Message);
            Assert.AreEqual(expectedError, ex!.Error);
            Assert.AreEqual(expectedErrorDescription, ex!.ErrorDescription);
        }

        private HttpResponseMessage createMockHttpResponse(string body, HttpStatusCode status)
        {
            var res = new HttpResponseMessage();
            res.Content = new StringContent(body);
            res.StatusCode = status;
            return res;
        }
    }
}
