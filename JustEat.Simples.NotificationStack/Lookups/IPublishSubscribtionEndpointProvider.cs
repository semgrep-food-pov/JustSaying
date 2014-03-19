using System;
using System.Globalization;
using JustEat.Simples.NotificationStack.AwsTools.QueueCreation;
using SimpleMessageMule.Lookups;

namespace JustEat.Simples.NotificationStack.Stack.Lookups
{

    /// <summary>
    /// Provides endpoint locations for SQS queues subscribed to topics
    /// </summary>
    public class SqsSubscribtionEndpointProvider : IPublishSubscribtionEndpointProvider
    {
        private readonly SqsConfiguration _subscriptionConfig;
        private readonly IMessagingConfig _publishingConfiguration;

        public SqsSubscribtionEndpointProvider(SqsConfiguration subscriptionConfig, IMessagingConfig publishingConfiguration)
        {
            _subscriptionConfig = subscriptionConfig;
            _publishingConfiguration = publishingConfiguration;
        }

        public string GetLocationName()
        {
            if (_subscriptionConfig.InstancePosition.HasValue && _subscriptionConfig.InstancePosition.Value <= 0)
                throw new Exception("Cannot have an instance position less than 1. Check your configuration.");

            var instancePositionValue = _subscriptionConfig.InstancePosition.HasValue
                                            ? _subscriptionConfig.InstancePosition.Value.ToString(CultureInfo.InvariantCulture)
                                            : string.Empty;

            return String.Join("-", new[] { _publishingConfiguration.Tenant, _publishingConfiguration.Environment, _publishingConfiguration.Component, instancePositionValue, _subscriptionConfig.Topic }).ToLower().Replace("--", "-");
        }
    }
}