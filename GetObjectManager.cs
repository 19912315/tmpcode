using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aliyun.OSS;
using Aliyun.OSS.Common;

namespace iHawkOssLibrary
{
    /// <summary>
    /// oss object 下载类
    /// </summary>
    public class GetObjectManager
    {
        #region constructor

        /// <summary>
        /// oss object 下载类构造函数
        /// </summary>
        /// <param name="ossConfigIndex">0-字由官方, 1-阿里内部, 2-汉仪oss读写专用</param>
        /// <param name="internalEndPoint">使用经典网络内网endpoint标识，true为限制内网上传，false允许外网上传</param>
        public GetObjectManager(int ossConfigIndex, bool internalEndPoint)
        {
            var ossConfigItem = Config.OssConfigList[ossConfigIndex];
            _client = new OssClient(internalEndPoint ? ossConfigItem.InternalEndPoint : ossConfigItem.Endpoint, ossConfigItem.AccessKeyId, ossConfigItem.AccessKeySecret);
        }

        #endregion

        #region property

        private readonly OssClient _client;

        #endregion

        #region 同步下载

        /// <summary>
        /// 同步下载object
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="key"></param>
        /// <param name="fileName">本地保存文件名</param>
        /// <returns>下载结果信息，成功包含succeeded，失败以Failed开头</returns>
        public string GetObject(string bucketName, string key, string fileName)
        {
            try
            {
                var result = _client.GetObject(bucketName, key);
                using (var requestStream = result.Content)
                {
                    using (var fs = File.Open(fileName, FileMode.OpenOrCreate))
                    {
                        var length = 4 * 1024;
                        var buf = new byte[length];
                        do
                        {
                            length = requestStream.Read(buf, 0, length);
                            fs.Write(buf, 0, length);
                        } while (length != 0);
                    }
                }

                Console.WriteLine("Get object:{0} succeeded", key);
                return $"Get object:{key} succeeded";
            }
            catch (OssException ex)
            {
                Console.WriteLine("Failed with error code: {0}; Error info: {1}. \nRequestID:{2}\tHostID:{3}", ex.ErrorCode, ex.Message, ex.RequestId, ex.HostId);
                return $"Failed with error code: {ex.ErrorCode}; Error info: {ex.Message}. \nRequestID:{ex.RequestId}\tHostID:{ex.HostId}";
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed with error info: {0}", ex.Message);
                return $"Failed with error info: {ex.Message}";
            }
        }

        #endregion
    }
}
