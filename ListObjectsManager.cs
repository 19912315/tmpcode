using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aliyun.OSS;
using Aliyun.OSS.Common;

namespace iHawkOssLibrary
{
    public class ListObjectsManager
    {
        #region constructor

        public ListObjectsManager(int ossConfigIndex, bool internalEndPoint)
        {
            var ossConfigItem = Config.OssConfigList[ossConfigIndex];
            _client = new OssClient(internalEndPoint ? ossConfigItem.InternalEndPoint : ossConfigItem.Endpoint, ossConfigItem.AccessKeyId, ossConfigItem.AccessKeySecret);
        }

        #endregion

        #region property

        private readonly OssClient _client;
        //private static readonly OssClient client = new OssClient(Config.Endpoint, Config.AccessKeyId, Config.AccessKeySecret);

        #endregion

        #region 同步获取object列表

        /// <summary>
        /// 同步获取object列表
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="prefix">搜索前缀</param>
        /// <returns>object名称列表</returns>
        public List<string> ListObjectsWithRequest(string bucketName, string prefix)
        {
            try
            {
                var keys = new List<string>();
                ObjectListing result;
                string nextMarker = string.Empty;
                do
                {
                    var listObjectsRequest = new ListObjectsRequest(bucketName) {Marker = nextMarker, Prefix = prefix, MaxKeys = 100};
                    result = _client.ListObjects(listObjectsRequest);

                    foreach (var summary in result.ObjectSummaries)
                    {
                        Console.WriteLine(summary.Key);
                        keys.Add(summary.Key);
                    }

                    nextMarker = result.NextMarker;
                } while (result.IsTruncated);

                Console.WriteLine("List objects of bucket:{0} succeeded ", bucketName);
                return keys;
            }
            catch (OssException ex)
            {
                Console.WriteLine("Failed with error code: {0}; Error info: {1}. \nRequestID:{2}\tHostID:{3}",
                    ex.ErrorCode, ex.Message, ex.RequestId, ex.HostId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed with error info: {0}", ex.Message);
            }

            return null;
        }

        #endregion
    }
}
