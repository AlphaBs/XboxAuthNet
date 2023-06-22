using NUnit.Framework;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using XboxAuthNet.XboxLive;
using XboxAuthNet.XboxLive.Responses;

namespace XboxAuthNet.Test
{
    [TestFixture]
    public class TestXboxResponseHandler
    {
        private XboxAuthResponseHandler responseHandler = null!;

        [SetUp]
        public void SetUp()
        {
            responseHandler = new XboxAuthResponseHandler();
        }

        static object[] TestExceptionCases =
        {
            new object?[]
            {
                "{\"XErr\":2148916233, \"Message\":\"Message\", \"Redirect\":\"Redirect\"}",
                HttpStatusCode.BadRequest,
                "8015dc09, Message, Redirect", "8015dc09", "Message", "Redirect"
            },
            new object?[]
            {
                "{\"XErr\":\"XErr\", \"Redirect\":\"Redirect\"}",
                HttpStatusCode.BadRequest,
                "XErr, Redirect", "XErr", null, "Redirect"
            },
            new object?[]
            {
                "{\"Message\":\"Message\", \"Redirect\":\"Redirect\"}",
                HttpStatusCode.BadRequest,
                "Message, Redirect", null, "Message", "Redirect"
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
        };

        [Test, Combinatorial]
        [TestCaseSource(nameof(TestExceptionCases))]
        public void TestException(string resBody, HttpStatusCode statusCode, 
            string expectedMessage, string expectedError, string expectedErrorMessage, string expectedRedirect)
        {
            var res = new HttpResponseMessage();
            res.Content = new StringContent(resBody);
            res.StatusCode = statusCode;

            var ex = Assert.ThrowsAsync<XboxAuthException>(async () => await responseHandler.HandleResponse<XboxAuthResponse>(res));
            Assert.NotNull(ex);
            Assert.AreEqual(expectedMessage, ex!.Message);
            Assert.AreEqual(expectedError, ex!.Error);
            Assert.AreEqual(expectedErrorMessage, ex!.ErrorMessage);
            Assert.AreEqual(expectedRedirect, ex!.Redirect);
        }

        [Test]
        [TestCase("2148916236", "XSTS error=\"account_age_verification_required\"")]
        [TestCase(null, "XSTS error=\"account_age_verification_required\"")]
        [TestCase("2148916236", null)]
        public void TestHeaderError(string? xerr, string? wwwAuth)
        {
            var msg = new HttpResponseMessage();
            if (xerr != null)
                msg.Headers.Add("X-Err", xerr);
            if (wwwAuth != null)
                msg.Headers.Add("WWW-Authenticate", wwwAuth);
            msg.StatusCode = HttpStatusCode.Unauthorized;

            var ex = Assert.ThrowsAsync<XboxAuthException>(async () => await responseHandler.HandleResponse<XboxAuthResponse>(msg));
            Assert.NotNull(ex);
            Assert.AreEqual(ErrorHelper.TryConvertToHexErrorCode(xerr), ex!.Error);
            Assert.AreEqual(wwwAuth, ex!.ErrorMessage);
            Assert.AreEqual(null, ex!.Redirect);
        }

        [Test]
        public async Task TestSuccessResponse()
        {
            var res = new HttpResponseMessage();
            res.Content = new StringContent("{\"IssueInstant\":\"2022-07-29T08:25:28.392348Z\",\"NotAfter\":\"2022-07-30T00:25:28.392348Z\",\"Token\":\"jwt\",\"DisplayClaims\":{\"xui\":[{\"gtg\":\"gtg\",\"xid\":\"xid\",\"uhs\":\"uhs\",\"usr\":\"usr\",\"utr\":\"utr\",\"prv\":\"prv\",\"agg\":\"Adult\"}]}}");
            res.StatusCode = HttpStatusCode.OK;

            var result = await responseHandler.HandleResponse<XboxAuthResponse>(res);
            Assert.AreEqual("jwt", result.Token);
            Assert.AreEqual("gtg", result.XuiClaims?.Gamertag);
            Assert.AreEqual("xid", result.XuiClaims?.XboxUserId);
            Assert.AreEqual("uhs", result.XuiClaims?.UserHash);
            Assert.AreEqual("usr", result.XuiClaims?.UserSettingsRestrictions);
            Assert.AreEqual("utr", result.XuiClaims?.UserTitleRestrictions);
            Assert.AreEqual("prv", result.XuiClaims?.Privileges);
            Assert.AreEqual("Adult", result.XuiClaims?.AgeGroup);
        }

        [Test]
        [TestCase("{\"IssueInstant\":\"2022-07-29T08:25:28.392348Z\",\"NotAfter\":\"2022-07-30T00:25:28.392348Z\",\"Token\":\"jwt\"}")]
        [TestCase("{\"IssueInstant\":\"2022-07-29T08:25:28.392348Z\",\"NotAfter\":\"2022-07-30T00:25:28.392348Z\",\"Token\":\"jwt\",\"DisplayClaims\":{\"xui\":[]}}")]
        [TestCase("{\"IssueInstant\":\"2022-07-29T08:25:28.392348Z\",\"NotAfter\":\"2022-07-30T00:25:28.392348Z\",\"Token\":\"jwt\",\"DisplayClaims\":{\"xui\":[null]}}")]
        [TestCase("{\"IssueInstant\":\"2022-07-29T08:25:28.392348Z\",\"NotAfter\":\"2022-07-30T00:25:28.392348Z\",\"Token\":\"jwt\",\"DisplayClaims\":{\"xui\":null}}")]
        [TestCase("{\"IssueInstant\":\"2022-07-29T08:25:28.392348Z\",\"NotAfter\":\"2022-07-30T00:25:28.392348Z\",\"Token\":\"jwt\",\"DisplayClaims\":{\"x23\":null}}")]
        [TestCase("{\"IssueInstant\":\"2022-07-29T08:25:28.392348Z\",\"NotAfter\":\"2022-07-30T00:25:28.392348Z\",\"Token\":\"jwt\",\"DisplayClaims\":null}")]
        public async Task TestSuccessResponseWithoutClaims(string tc)
        {
            var res = new HttpResponseMessage();
            res.Content = new StringContent(tc);
            res.StatusCode = HttpStatusCode.OK;

            var result = await responseHandler.HandleResponse<XboxAuthResponse>(res);
            Assert.AreEqual("jwt", result.Token);
            Assert.IsNull(result.XuiClaims);
        }
    }
}
