using AWSFacade.SQS.Contracts;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace AWSFacade.SQS
{
    public class SqsFacadeFactory : ISqsFacadeFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public SqsFacadeFactory(IServiceProvider serviceProvider)
        {
            if (serviceProvider is null) throw new ArgumentNullException(nameof(serviceProvider));

            _serviceProvider = serviceProvider;
        }

        public ISqsFacade Create() => Create(string.Empty);
        public ISqsFacade Create(string name)
        {
            var sqsFacade = _serviceProvider.GetServices<ISqsFacade>().Where(w => w.Name == name).FirstOrDefault();
            return sqsFacade;
        }
    }
}
