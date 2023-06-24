using NUnit.Framework;
using System;
using System.Net;
using XboxAuthNet.OAuth;

namespace XboxAuthNet.Test
{
    [TestFixture]
    public class TestMicrosoftOAuthCodeFlow
    {
        // testcase from https://learn.microsoft.com/en-us/azure/active-directory/develop/v2-oauth2-auth-code-flow#successful-response-2

        [Test]
        [TestCase("{\"access_token\":\"eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Ik5HVEZ2ZEstZnl0aEV1Q...\",\"token_type\":\"Bearer\",\"expires_in\":3599,\"scope\":\"https%3A%2F%2Fgraph.microsoft.com%2Fmail.read\",\"refresh_token\":\"AwABAAAAvPM1KaPlrEqdFSBzjqfTGAMxZGUTdM0t4B4...\",\"id_token\":\"eyJ0eXAiOiJKV1QiLCJhbGciOiJub25lIn0.eyJhdWQiOiIyZDRkMTFhMi1mODE0LTQ2YTctOD...\"}", 
                  "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Ik5HVEZ2ZEstZnl0aEV1Q...", 
                  "AwABAAAAvPM1KaPlrEqdFSBzjqfTGAMxZGUTdM0t4B4...")]
        public void TestSuccessResponse(string resBody, string expectedAccessToken, string expectedRefreshToken)
        {
            var result = MicrosoftOAuthResponse.FromHttpResponse(resBody, 234, null);
            Assert.AreEqual(expectedAccessToken, result.AccessToken);
            Assert.AreEqual(expectedRefreshToken, result.RawRefreshToken);
        }

        [Test]
        [TestCase("{\"access_token\":\"eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Ik5HVEZ2ZEstZnl0aEV1Q...\",\"token_type\":\"Bearer\",\"expires_in\":3599,\"scope\":\"https%3A%2F%2Fgraph.microsoft.com%2Fmail.read\",\"refresh_token\":\"AwABAAAAvPM1KaPlrEqdFSBzjqfTGAMxZGUTdM0t4B4...\",\"id_token\":\"eyJ0eXAiOiJKV1QiLCJhbGciOiJub25lIn0.eyJhdWQiOiIyZDRkMTFhMi1mODE0LTQ2YTctOD...\"}",
                  3599)]
        public void TestExpiresOn(string resBody, int expiresIn)
        {
            var result = MicrosoftOAuthResponse.FromHttpResponse(resBody, 200, null);
            var expectedExpiresOn = DateTime.UtcNow.AddSeconds(expiresIn);
            var delta = expectedExpiresOn - result.ExpiresOn;
            Assert.True(delta < TimeSpan.FromMilliseconds(100));
        }

        [Test]
        [TestCase("{\"access_token\":\"eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Ik5HVEZ2ZEstZnl0aEV1Q...\",\"token_type\":\"Bearer\",\"expires_in\":3599,\"scope\":\"https%3A%2F%2Fgraph.microsoft.com%2Fmail.read\",\"refresh_token\":\"AwABAAAAvPM1KaPlrEqdFSBzjqfTGAMxZGUTdM0t4B4...\",\"id_token\":\"eyJ0eXAiOiJKV1QiLCJhbGciOiJub25lIn0.eyJhdWQiOiIyZDRkMTFhMi1mODE0LTQ2YTctOD...\"}")]
        public void TestValidateResponse(string resBody)
        {
            var result = MicrosoftOAuthResponse.FromHttpResponse(resBody, 200, null);
            Assert.True(result.Validate());
        }

        [Test]
        [TestCase("{\"access_token\":\"\",\"token_type\":\"Bearer\",\"expires_in\":3599,\"scope\":\"https%3A%2F%2Fgraph.microsoft.com%2Fmail.read\",\"refresh_token\":\"AwABAAAAvPM1KaPlrEqdFSBzjqfTGAMxZGUTdM0t4B4...\",\"id_token\":\"eyJ0eXAiOiJKV1QiLCJhbGciOiJub25lIn0.eyJhdWQiOiIyZDRkMTFhMi1mODE0LTQ2YTctOD...\"}")]
        [TestCase("{\"access_token\":\"something\",\"token_type\":\"Bearer\",\"expires_in\":0,\"scope\":\"https%3A%2F%2Fgraph.microsoft.com%2Fmail.read\",\"refresh_token\":\"AwABAAAAvPM1KaPlrEqdFSBzjqfTGAMxZGUTdM0t4B4...\",\"id_token\":\"eyJ0eXAiOiJKV1QiLCJhbGciOiJub25lIn0.eyJhdWQiOiIyZDRkMTFhMi1mODE0LTQ2YTctOD...\"}")]
        public void TestInvalidateResponse(string resBody)
        {
            var result = MicrosoftOAuthResponse.FromHttpResponse(resBody, 200, null);
            System.Threading.Thread.Sleep(1000); // to expire token
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
                "400", null, null
            },
            new object?[]
            {
                "{}",
                HttpStatusCode.BadRequest,
                "400", null, null
            },
            new object?[]
            {
                "asidjfoawiejf",
                HttpStatusCode.BadRequest,
                "400", null, null
            },
            new object?[]
            {
                "",
                HttpStatusCode.BadRequest,
                "400", null, null
            },
        };

        [Test, Combinatorial]
        [TestCaseSource(nameof(TestExceptionCases))]
        public void TestException(string resBody, HttpStatusCode statusCode, 
            string expectedMessage, string expectedError, string expectedErrorDescription)
        {
            var ex = Assert.Throws<MicrosoftOAuthException>(() => 
                MicrosoftOAuthResponse.FromHttpResponse(resBody, (int)statusCode, null));
            Assert.NotNull(ex);
            Assert.AreEqual(expectedMessage, ex!.Message);
            Assert.AreEqual(expectedError, ex!.Error);
            Assert.AreEqual(expectedErrorDescription, ex!.ErrorDescription);
        }
    }
}
