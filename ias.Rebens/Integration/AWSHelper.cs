using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.Route53;
using Amazon.Route53.Model;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;


namespace ias.Rebens.Integration
{
    public class AWSHelper
    {
        private const string API_KEY = "AKIA5FPFSHGM4RPFLYMW";
        private const string API_TOKEN = "nalA46tc1YGK5ztTvXiuOCg3Xl+02mbgr7pfSt2s";

        public async void AddDomainToRoute53(string subdomain)
        {
            string domainName = "sistemarebens.com.br.";
            var route53Client = new AmazonRoute53Client(API_KEY, API_TOKEN, RegionEndpoint.USEast1);

            var zoneRequest = new CreateHostedZoneRequest() { Name = domainName, CallerReference = "my_change_request" };

            var recordSet = new ResourceRecordSet()
            {
                Name = subdomain,
                TTL = 60,
                Type = RRType.CNAME,
                ResourceRecords = new List<ResourceRecord> { new ResourceRecord { Value = "s3-website-sa-east-1.amazonaws.com" } }
            };

            var change = new Change()
            {
                Action = ChangeAction.CREATE,
                ResourceRecordSet = recordSet
            };

            var zones = await route53Client.ListHostedZonesAsync();
            var zone = zones.HostedZones.SingleOrDefault(z => z.Name == domainName);
            if (zone != null)
            {
                var batch = new ChangeBatch()
                {
                    Changes = new List<Change>()
                };
                batch.Changes.Add(change);

                var recordsetResponse = await route53Client.ChangeResourceRecordSetsAsync(new ChangeResourceRecordSetsRequest()
                {
                    ChangeBatch = batch,
                    HostedZoneId = zone.Id
                });
            }
        }

        public async Task ChangeBucketPolicy(string bucketName, bool enable)
        {
            IAmazonS3 s3Client = new AmazonS3Client(API_KEY, API_TOKEN, RegionEndpoint.SAEast1);
            string policy = $@"{{
    ""Version"": ""2012-10-17"",
    ""Statement"": [ 
        {{
            ""Effect"": ""{(enable ? "Allow" : "Deny")}"",
            ""Principal"": ""*"",
            ""Action"": ""s3:GetObject"",
            ""Resource"": ""arn:aws:s3:::{bucketName}/*""
        }} 
    ] 
}}";

            PutBucketPolicyRequest request = new PutBucketPolicyRequest()
            {
                BucketName = bucketName,
                Policy = policy
            };
            PutBucketPolicyResponse response = await s3Client.PutBucketPolicyAsync(request);
        }
    }
}
