using System.Collections.Generic;
using NUnit.Framework;
using XboxAuthNet.OAuth.CodeFlow.Parameters;

namespace XboxAuthNet.Test;

[TestFixture]
public class TestCodeFlowParameters
{
    [Test]
    public void AuthorizationIncludesCommonAndSpecificQueries()
    {
        var parameter = new CodeFlowAuthorizationParameter
        {
            Tenant = "tenant",
            ClientId = "client",
            Scope = "scope",
            ResponseType = "code",
            RedirectUri = "https://localhost/callback",
            Nonce = "nonce",
            State = ""
        };

        var query = parameter.ToQueryDictionary();

        Assert.That(query["client_id"], Is.EqualTo("client"));
        Assert.That(query["scope"], Is.EqualTo("scope"));
        Assert.That(query["response_type"], Is.EqualTo("code"));
        Assert.That(query["redirect_uri"], Is.EqualTo("https://localhost/callback"));
        Assert.That(query["nonce"], Is.EqualTo("nonce"));
        Assert.That(query.ContainsKey("Tenant"), Is.False);
        Assert.That(query.ContainsKey("state"), Is.False);
    }

    [Test]
    public void AccessTokenIncludesItsOwnQueries()
    {
        var parameter = new CodeFlowAccessTokenParameter
        {
            ClientId = "client",
            Code = "code",
            RedirectUrl = "https://localhost/callback",
            CodeVerifier = "verifier",
            CilentAssertion = "assertion"
        };

        var query = parameter.ToQueryDictionary();

        Assert.That(query["client_id"], Is.EqualTo("client"));
        Assert.That(query["code"], Is.EqualTo("code"));
        Assert.That(query["redirect_uri"], Is.EqualTo("https://localhost/callback"));
        Assert.That(query["code_verifier"], Is.EqualTo("verifier"));
        Assert.That(query["client_assertion"], Is.EqualTo("assertion"));
        Assert.That(query.ContainsKey("refresh_token"), Is.False);
    }

    [Test]
    public void ExtraQueriesOverrideRefreshTokenQueries()
    {
        var parameter = new CodeFlowRefreshTokenParameter
        {
            ClientId = "client",
            RefreshToken = "refresh",
            GrantType = "refresh_token"
        };
        parameter.ExtraQueries["client_id"] = "override";
        parameter.ExtraQueries["custom"] = "extra";

        var query = parameter.ToQueryDictionary();

        Assert.That(query["client_id"], Is.EqualTo("override"));
        Assert.That(query["refresh_token"], Is.EqualTo("refresh"));
        Assert.That(query["grant_type"], Is.EqualTo("refresh_token"));
        Assert.That(query["custom"], Is.EqualTo("extra"));
    }

    [Test]
    public void SubclassExtendsItsParentWithoutReflectionAttributes()
    {
        var parameter = new ExtendedAuthorizationParameter
        {
            ClientId = "client",
            ResponseType = "code",
            CustomValue = "custom"
        };

        var query = parameter.ToQueryDictionary();

        Assert.That(query["client_id"], Is.EqualTo("client"));
        Assert.That(query["response_type"], Is.EqualTo("code"));
        Assert.That(query["custom"], Is.EqualTo("custom"));
    }

    private sealed class ExtendedAuthorizationParameter : CodeFlowAuthorizationParameter
    {
        public string? CustomValue { get; set; }

        protected override void AddQueries(Dictionary<string, string?> query)
        {
            base.AddQueries(query);
            AddQuery(query, "custom", CustomValue);
        }
    }
}
