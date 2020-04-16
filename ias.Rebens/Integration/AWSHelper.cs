using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.Route53;
using Amazon.Route53.Model;
using Amazon.S3;
using Amazon.S3.Transfer;
using System.IO;

namespace ias.Rebens.Integration
{
    public class AWSHelper
    {
        private const string API_KEY = "AKIA5FPFSHGM4RPFLYMW";
        private const string API_TOKEN = "nalA46tc1YGK5ztTvXiuOCg3Xl+02mbgr7pfSt2s";

        public async void AddDomainToRoute53(string subdomain, int idOperation, IOperationRepository operationRepo)
        {
            string domainName = "sistemarebens.com.br.";
            subdomain += "." + domainName;
            subdomain = subdomain.TrimEnd('.');
            var route53Client = new AmazonRoute53Client(API_KEY, API_TOKEN, RegionEndpoint.USEast1);

            var zoneRequest = new CreateHostedZoneRequest() { Name = domainName, CallerReference = "my_change_request" };

            var recordSet = new ResourceRecordSet()
            {
                Name = subdomain,
                TTL = 60,
                Type = RRType.CNAME,
                ResourceRecords = new List<ResourceRecord> { new ResourceRecord { Value = "s3-website-sa-east-1.amazonaws.com" } }
            };

            var change = new Change() { Action = ChangeAction.CREATE, ResourceRecordSet = recordSet };

            var listZoneRequest = new ListHostedZonesRequest() { MaxItems = "1000" };
            var zones = await route53Client.ListHostedZonesAsync(listZoneRequest);
            var zone = zones.HostedZones.SingleOrDefault(z => z.Name == domainName);
            if (zone != null)
            {
                var batch = new ChangeBatch() { Changes = new List<Change>() };
                batch.Changes.Add(change);

                try
                {
                    var recordsetResponse = await route53Client.ChangeResourceRecordSetsAsync(new ChangeResourceRecordSetsRequest()
                    {
                        ChangeBatch = batch,
                        HostedZoneId = zone.Id
                    });
                    var changeRequest = new GetChangeRequest() { Id = recordsetResponse.ChangeInfo.Id };

                    var changeStatus = await route53Client.GetChangeAsync(changeRequest);
                    while (ChangeStatus.PENDING == changeStatus.ChangeInfo.Status)
                    {
                        Thread.Sleep(15000);
                        changeStatus = await route53Client.GetChangeAsync(changeRequest);
                    }

                    operationRepo.SetSubdomainCreated(idOperation, out string error);
                }
                catch (InvalidChangeBatchException ex)
                {
                    if (ex.Message.Contains("already exists"))
                        operationRepo.SetSubdomainCreated(idOperation, out string error);
                }
            }
        }

        public async Task<bool> DisableBucketAsync(string bucketName)
        {
            bool ret = false;
            try
            {
                IAmazonS3 s3Client = new AmazonS3Client(API_KEY, API_TOKEN, RegionEndpoint.SAEast1);

                var objects = await s3Client.ListObjectsAsync(bucketName);
                if(objects.S3Objects.Count > 0)
                    await s3Client.DeletesAsync(bucketName, objects.S3Objects.Select(s => s.Key), null);

                var constant = new Constant();
                var transferUtility = new TransferUtility(s3Client);
                transferUtility.Upload(Path.Combine(constant.AppSettings.App.MediaServerPath, "EmptyBucket", "index.html"), bucketName);

                ret = true;
            }
            catch { }

            return ret;
        }
    }
}
