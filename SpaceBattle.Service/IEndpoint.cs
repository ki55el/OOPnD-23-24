using System.Net;
using CoreWCF.OpenApi.Attributes;
using CoreWCF.Web;

namespace SpaceBattle.Service;

[ServiceContract]
[OpenApiBasePath("/api")]
internal interface IEndpoint
{
    [OperationContract]
    [WebInvoke(Method = "POST", UriTemplate = "/body")]
    [OpenApiTag("Tag")]
    [OpenApiResponse(ContentTypes = new[] { "application/json" }, Description = "Success", StatusCode = HttpStatusCode.Accepted, Type = typeof(MessageContract))]

    int POST(
        [OpenApiParameter(ContentTypes = new[] { "application/json" })] MessageContract param);
}
