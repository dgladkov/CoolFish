using System;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.SQS;
using Amazon.SQS.Model;
using CoolFishNS.RemoteNotification.Payloads;
using NLog;

namespace CoolFishNS.Utilities
{
    public class SQSManager
    {
        private const string AccountId = "257743725717";
        private const string IdentityPoolId = "us-east-1:8df25573-daf2-4a7e-a380-bb56d8b6dbf0";
        private const string UnAuthorizedRoleId = "arn:aws:iam::257743725717:role/Cognito_CoolFishUnauth_DefaultRole";
        private const string AnalyticsQueueUri = "https://queue.amazonaws.com/257743725717/AnalyticsQueue";
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly AmazonSQSClient _client;

        public SQSManager()
        {
            var credentials = new CognitoAWSCredentials(
                AccountId,
                IdentityPoolId,
                UnAuthorizedRoleId,
                null,
                RegionEndpoint.USEast1
                );

            _client = new AmazonSQSClient(credentials, RegionEndpoint.USEast1);
        }

        public void SendAnalyticsPayload(AnalyticsPayload analyticspayload)
        {
#if DEBUG
            return;
#endif
            try
            {
                string payload = Serializer.SerializeToJSON(analyticspayload);
                var message = new SendMessageRequest(AnalyticsQueueUri, payload);

                _client.SendMessageAsync(message);
            }
            catch (Exception ex)
            {
                Logger.Error("Problem posting analytics", ex);
            }
        }

        public void SendLoggingPayload(LoggingPayload loggingPayload)
        {
#if DEBUG
            return;
#endif

            try
            {
                string payload = Serializer.SerializeToJSON(loggingPayload);
                var message = new SendMessageRequest(AnalyticsQueueUri, payload);

                _client.SendMessageAsync(message);
            }
            catch (Exception ex)
            {
                Logger.Warn("Problem posting logging", ex);
            }
        }
    }
}