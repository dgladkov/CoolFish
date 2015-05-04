using System;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.SQS;
using Amazon.SQS.Model;
using CoolFishNS.RemoteNotification.Payloads;
using NLog;

namespace CoolFishNS.Utilities
{
    internal class SQSManager
    {
        private readonly AmazonSQSClient _client;

        internal SQSManager()
        {

        }

        internal void SendAnalyticsPayload(AnalyticsPayload analyticspayload)
        {

        }

        internal void SendLoggingPayload(LoggingPayload loggingPayload)
        {

        }
    }
}