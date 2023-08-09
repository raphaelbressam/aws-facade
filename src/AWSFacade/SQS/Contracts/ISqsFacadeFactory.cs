using System;
using System.Collections.Generic;
using System.Text;

namespace AWSFacade.SQS.Contracts
{
    public interface ISqsFacadeFactory
    {
        ISqsFacade Create();
        ISqsFacade Create(string name);
    }
}
