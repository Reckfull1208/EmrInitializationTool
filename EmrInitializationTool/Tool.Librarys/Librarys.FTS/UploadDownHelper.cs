using HWKJ.Common.Models;
using HWKJ.Fts.Sdk;
using HWKJ.Fts.Sdk.Uploaders;
using Librarys.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Librarys.FTS
{
    public class UploadDownHelper
    {
        /// <summary>
        /// 初始化  只需要在client启动时调用一次
        /// </summary>
        /// <param name="smallerHost"></param>
        public static void Init(string smallerHost)
        {
            FtsClientInitializer.Setup(new BasicOption()
            {
                FTSSmallFileHost = smallerHost
            });
        }

        /// <summary>
        /// 病历文件下载
        /// </summary>
        /// <param name="fileId">文件ID</param>
        /// <param name="fileDirectory">文件目录</param>
        /// <returns></returns>
        public static MsgModel DownLoad(string json)
        {
            FTSModel fm = JsonConvert.DeserializeObject<FTSModel>(json);
            var msgModel = new MsgModel();
            var url = "1";
            try
            {
                var fileInfo = SmallerUploader.Instance().GetInfo(fm.FileId);
                if (fileInfo == null)
                {
                    msgModel.Result = false;
                    msgModel.Content = "获取文件信息失败！";
                    return msgModel;
                }
                url = fileInfo.Url; 
                //文件名称(不包含扩展名)
                var fileName = Path.GetFileNameWithoutExtension(fileInfo.FileName);
                //新文件目录
                if (fileName == null)
                    return msgModel;
                var newFileDirectory = Path.Combine(fm.FileDirectory, "BaseFile");
                var directoryInfo = new DirectoryInfo(newFileDirectory);
                if (!directoryInfo.Exists) //创建根目录
                {
                    directoryInfo.Create();
                } 
                //新文件全路径
                var newFileFullPath = Path.Combine(newFileDirectory, fileInfo.FileName);
                using (var client = new WebClient())
                {
                    client.DownloadFile(fileInfo.Url, newFileFullPath);
                    msgModel.Result = true;
                    msgModel.Content = newFileFullPath;
                }
                 
                if(fm.IsNeedToClassification) //若有第二路径，说明需要2次备份 ex路径不包括第一路径内容
                {
                    //组合路径
                    var directionPah = Path.Combine(fm.FileDirectory, fm.ClassifyName);
                    var directoryInfoEx = new DirectoryInfo(directionPah);
                    if (!directoryInfoEx.Exists) 
                    {
                        directoryInfoEx.Create();
                    } 
                    System.IO.File.Copy(newFileFullPath, Path.Combine(directionPah, fm.Name));
                }

                return msgModel;

            }
            catch (Exception ex)
            { 
                msgModel.Result = false;
                msgModel.Content = ex.Message;
                return msgModel;
            }
        }

    }
}
