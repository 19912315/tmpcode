using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Aliyun.OSS;
using Aliyun.OSS.Common;

namespace iHawkOssLibrary
{
    /// <summary>
    /// oss object 上传类
    /// </summary>
    public class PutObjectManager
    {
        #region constructor

        /// <summary>
        /// oss object 上传类构造函数
        /// </summary>
        /// <param name="ossConfigIndex">0-字由官方, 1-阿里内部</param>
        /// <param name="internalEndPoint">使用经典网络内网endpoint标识，true为限制内网上传，false允许外网上传</param>
        public PutObjectManager(int ossConfigIndex, bool internalEndPoint)
        {
            var ossConfigItem = Config.OssConfigList[ossConfigIndex];
            _client = new OssClient(internalEndPoint ? ossConfigItem.InternalEndPoint : ossConfigItem.Endpoint, ossConfigItem.AccessKeyId, ossConfigItem.AccessKeySecret);
        }

        #endregion

        #region property

        private readonly OssClient _client;
        //private static readonly OssClient client = new OssClient(Config.Endpoint, Config.AccessKeyId, Config.AccessKeySecret);

        private static readonly AutoResetEvent _event = new AutoResetEvent(false);

        #endregion

        #region 同步上传

        /// <summary>
        /// 同步上传object(通过文件名)
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="key">上传名（path/filename or path/）带反斜线的默认上传为目录，否则上传为文件</param>
        /// <param name="fileName">本地上传文件名</param>
        /// <returns>上传结果信息，成功包含succeeded，失败以Failed开头</returns>
        public string PutObjectFromFile(string bucketName, string key, string fileName)
        {
            try
            {
                _client.PutObject(bucketName, key, fileName);
                Console.WriteLine("Put object:{0} succeeded", key);
                return $"Put object:{key} succeeded";
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

        /// <summary>
        /// 同步上传object(通过base64字符串)
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="key">上传名（path/filename or path/）带反斜线的默认上传为目录，否则上传为文件</param>
        /// <param name="base64String">上传文件base64字符串</param>
        /// <returns>上传结果信息，成功包含succeeded，失败以Failed开头</returns>
        public string PutObjectFromBase64String(string bucketName, string key, string base64String)
        {
            try
            {
                var binaryData = Convert.FromBase64String(base64String);
                var stream = new MemoryStream(binaryData);

                _client.PutObject(bucketName, key, stream);
                Console.WriteLine("Put object:{0} succeeded", key);
                return $"Put object:{key} succeeded";
            }
            catch (OssException ex)
            {
                Console.WriteLine("Failed with error code: {0}; Error info: {1}. \nRequestID:{2}\tJpstID:{3}", ex.ErrorCode, ex.Message, ex.RequestId, ex.HostId);
                return $"Failed with error code: {ex.ErrorCode}; Error info: {ex.Message}. \nRequestID:{ex.RequestId}\tHostID:{ex.HostId}";
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed with error info: {0}", ex.Message);
                return $"Failed with error info: {ex.Message}";
            }
        }

        /// <summary>
        /// 同步上传object(通过byte数组)
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="key">上传名（path/filename or path/）带反斜线的默认上传为目录，否则上传为文件</param>
        /// <param name="buffer">上传文件byte数组</param>
        /// <returns>上传结果信息，成功包含succeeded，失败以Failed开头</returns>
        public string PutObjectFromBuffer(string bucketName, string key, byte[] buffer)
        {
            try
            {
                var stream = new MemoryStream(buffer);

                _client.PutObject(bucketName, key, stream);
                Console.WriteLine("Put object:{0} succeeded", key);
                return $"Put object:{key} succeeded";
            }
            catch (OssException ex)
            {
                Console.WriteLine("Failed with error code: {0}; Error info: {1}. \nRequestID:{2}\tJpstID:{3}", ex.ErrorCode, ex.Message, ex.RequestId, ex.HostId);
                return $"Failed with error code: {ex.ErrorCode}; Error info: {ex.Message}. \nRequestID:{ex.RequestId}\tHostID:{ex.HostId}";
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed with error info: {0}", ex.Message);
                return $"Failed with error info: {ex.Message}";
            }
        }

        /// <summary>
        /// 同步上传object(通过Stream)
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="key">上传名（path/filename or path/）带反斜线的默认上传为目录，否则上传为文件</param>
        /// <param name="stream">上传文件stream</param>
        /// <returns>上传结果信息，成功包含succeeded，失败以Failed开头</returns>
        public string PutObjectFromStream(string bucketName, string key, Stream stream)
        {
            try
            {
                _client.PutObject(bucketName, key, stream);
                Console.WriteLine("Put object:{0} succeeded", key);
                return $"Put object:{key} succeeded";
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

        #region 异步上传

        /// <summary>
        /// 异步上传object
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="key">上传名（path/filename or path/）带反斜线的默认上传为目录，否则上传为文件</param>
        /// <param name="fileName">本地上传文件名</param>
        public void AsyncPutObject(string bucketName, string key, string fileName)
        {
            try
            {
                using (var fs = File.Open(fileName, FileMode.Open))
                {
                    var metadata = new ObjectMetadata();
                    metadata.UserMetadata.Add("mykey1", "myval1");
                    metadata.UserMetadata.Add("mykey2", "myval2");
                    metadata.CacheControl = "No-Cache";
                    metadata.ContentType = "text/html";

                    var result = "Notice user: put object finish";
                    _client.BeginPutObject(bucketName, key, fs, metadata, PutObjectCallback, result.ToCharArray());

                    _event.WaitOne();
                }
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
        }

        private void PutObjectCallback(IAsyncResult ar)
        {
            try
            {
                _client.EndPutObject(ar);

                Console.WriteLine(ar.AsyncState as string);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                _event.Set();
            }
        }

        #endregion
    }
}
