using System;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;

namespace ias.Rebens.Integration
{
    public class AWSHelper
    {
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.SAEast1;
        private static IAmazonS3 s3Client;

        public async Task ChangeBucketPolicy(string bucketName, bool enable)
        {
            s3Client = new AmazonS3Client("AKIA5FPFSHGM4RPFLYMW", "nalA46tc1YGK5ztTvXiuOCg3Xl+02mbgr7pfSt2s", bucketRegion);
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
