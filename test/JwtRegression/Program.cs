using System.Text;
using XboxAuthNet.Jwt;

// Synthetic payloads only: this suite makes no authentication or HTTP requests.
var count = 0;
foreach (var name in new[] { "ASCII", "한글", "࠾", "࠿", "a", "ab", "abc" })
{
    var json = "{\"name\":\"" + name + "\"}";
    var payload = Convert.ToBase64String(Encoding.UTF8.GetBytes(json)).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    var decoded = JwtDecoder.DecodePayloadString("header." + payload + ".signature");
    if (decoded != json) throw new Exception("UTF-8 payload was not preserved");
    var model = JwtDecoder.DecodePayload<Dictionary<string, string>>("header." + payload + ".signature");
    if (model?["name"] != name) throw new Exception("Deserialized payload was not preserved");
    count++;
}
foreach (var invalid in new[] { "missing-segments", "a.b.c.d", "a.x.c", "a.!.c" })
{
    try { JwtDecoder.DecodePayloadString(invalid); throw new Exception("Malformed JWT accepted"); }
    catch (FormatException) { count++; }
}
Console.WriteLine($"PASS: {count} JWT decoding cases (offline)");
