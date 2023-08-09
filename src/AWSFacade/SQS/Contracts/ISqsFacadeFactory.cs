namespace AWSFacade.SQS.Contracts
{
    public interface ISqsFacadeFactory
    {
        ISqsFacade Create();
        ISqsFacade Create(string name);
    }
}
