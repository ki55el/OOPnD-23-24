using Hwdtech;

namespace SpaceBattle.Service;

[ServiceBehavior(IncludeExceptionDetailInFaults = true)]
public class Endpoint : IEndpoint
{
    public void POST(MessageContract param)
    {
        try
        {
            var thread_id = IoC.Resolve<string>("Get Thread ID by Game ID", param.GameID);
            var cmd = IoC.Resolve<Lib.ICommand>("Create Command", param);
            IoC.Resolve<Lib.ICommand>("Send Command", thread_id, cmd).Execute();
        }
        catch (Exception ex)
        {
            IoC.Resolve<Lib.ICommand>("HttpController.ExceptionHandler", param, ex).Execute();
        }
    }
}
