using System.Net.Http;
using XboxAuthNet.XboxLive;

var httpClient = new HttpClient();
var xboxAuthClient = new XboxAuthClient(httpClient);