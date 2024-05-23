using System.Net;
using CoreWCF.OpenApi.Attributes;
using CoreWCF.Web;

namespace SpaceBattle.Service;

[ServiceContract]
[OpenApiBasePath("/api")]
public interface IEndpoint
{
    [OperationContract]
    [WebInvoke(Method = "POST", UriTemplate = "/body")]
    [OpenApiTag("Tag")]
    [OpenApiResponse(ContentTypes = new[] { "application/json" }, Description = "Success", StatusCode = HttpStatusCode.Accepted, Type = typeof(MessageContract))]

    void POST(
        [OpenApiParameter(ContentTypes = new[] { "application/json" })] MessageContract param);
}
