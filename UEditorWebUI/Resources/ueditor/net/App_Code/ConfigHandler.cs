using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Config 的摘要说明
/// </summary>
public class ConfigHandler : Handler
{
    public ConfigHandler(HttpContext context) : base(context) { }

    public override void Process()
    {
        var prefix = Point.Common.AppSetting.Default.GetItem("CurrentRootUrl");
        if (!string.IsNullOrWhiteSpace(prefix))
        {
           
        }
        WriteJson(Config.Items);
    }
}