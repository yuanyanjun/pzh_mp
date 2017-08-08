<%@ WebHandler Language="C#" Class="imageUp" %>

using System;
using System.Web;
using System.IO;
using System.Collections;
using Point.WebUI.AppCode;

public class imageUp : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {

        context.Response.ContentType = "text/plain";

        //上传配置
        int size = 2; //文件大小限制,单位MB
        //var fileSize = FaceHand.Common.AppSetting.Default.GetItem("PictureSize");
        //if (!string.IsNullOrWhiteSpace(fileSize))
        //{
        //    size = Convert.ToInt32(fileSize);
        //}

        string[] filetype = { ".gif", ".png", ".jpg", ".jpeg", ".bmp" }; //文件允许格式

        //上传图片
        Hashtable info = new Hashtable();
        Uploader up = new Uploader();

        string rootpath = "~/UploadPictures/";

        string pathbase = String.Format("{0}{1}/{2}/", rootpath, up.getOtherInfo(context, "cid"), up.getOtherInfo(context, "uid"));

        //int path = Convert.ToInt32(up.getOtherInfo(context, "dir"));
        //if (path == 1)
        //{
        //    pathbase = "~/upload/";
        //}
        //else
        //{
        //    pathbase = "~/upload1/";
        //}

        info = up.upFile(context, pathbase, filetype, size);                   //获取上传状态

        string title = up.getOtherInfo(context, "pictitle");                   //获取图片描述
        string oriName = up.getOtherInfo(context, "fileName");                //获取原始文件名

        var sourcePath = info["url"].ToString().Replace(rootpath, "/");
        //if (!FaceHand.Common.Developer.IsSelfUse)
        //{
        //    UtilHelper.FileCopy(sourcePath);
        //}
        HttpContext.Current.Response.Write("{'url':'" + sourcePath + "','title':'" + title + "','original':'" + oriName + "','state':'" + info["state"] + "'}");
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}