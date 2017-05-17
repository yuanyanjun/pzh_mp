using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;
namespace MyTest_Console
{
    public static class EmployeeImportHelper
    {
        private static Dictionary<string, string> cache;

        private const int _maxImprotCount = 500;
        static EmployeeImportHelper()
        {

            cache = new Dictionary<string, string>();

            var properties = typeof(ImportAndExprotFiledInfo).GetProperties();
            foreach (var pinfo in properties)
            {
                var attribute = pinfo.GetCustomAttribute<ImportAndExprotFiledAttribute>();

                cache.Add(pinfo.Name, attribute.Name);
            }

        }

        public static void Test()
        {

        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ImportAndExprotFiledAttribute : Attribute
    {
        public string Name { get; set; }

        public bool IsRequired { get; set; }

        public int SortOrder { get; set; }

        public string DefultValue { get; set; }
    }

    public class ImportAndExprotFiledInfo
    {
        [ImportAndExprotFiled(Name = "员工编号", SortOrder = 1)]
        public string SerialNumber { get; set; }

        [ImportAndExprotFiled(Name = "姓名", IsRequired = true, SortOrder = 2)]
        public string Name { get; set; }

        [ImportAndExprotFiled(Name = "手机", SortOrder = 3)]
        public string MobileNo { get; set; }

        [ImportAndExprotFiled(Name = "部门", SortOrder = 3)]
        public string DepartmentId { get; set; }

        [ImportAndExprotFiled(Name = "性别", SortOrder = 4)]
        public string Sex { get; set; }

        //标识信息
        [ImportAndExprotFiled(Name = "微信", SortOrder = 5)]
        public string WeiXin { get; set; }

        [ImportAndExprotFiled(Name = "QQ", SortOrder = 6)]
        public string QQ { get; set; }

        [ImportAndExprotFiled(Name = "身份证号", SortOrder = 7)]
        public string IDCard { get; set; }

        [ImportAndExprotFiled(Name = "阳历生日", SortOrder = 8)]
        public string BirthDay1 { get; set; }
        [ImportAndExprotFiled(Name = "阴历生日", SortOrder = 9)]
        public string BirthDay2 { get; set; }

        [ImportAndExprotFiled(Name = "语言", SortOrder = 10)]
        public string Language { get; set; }

        [ImportAndExprotFiled(Name = "血型", SortOrder = 11)]
        public string BloodType { get; set; }

        [ImportAndExprotFiled(Name = "生肖", SortOrder = 12)]
        public string ShengXiao { get; set; }

        [ImportAndExprotFiled(Name = "星座", SortOrder = 13)]
        public string XingZuo { get; set; }

        [ImportAndExprotFiled(Name = "职务", SortOrder = 14)]
        public string Duty { get; set; }


        [ImportAndExprotFiled(Name = "学校", SortOrder = 15)]
        public string SchoolName { get; set; }

        [ImportAndExprotFiled(Name = "主页", SortOrder = 16)]
        public string HomePage { get; set; }

        [ImportAndExprotFiled(Name = "微博", SortOrder = 17)]
        public string Weibo { get; set; }

        [ImportAndExprotFiled(Name = "学历", SortOrder = 18)]
        public string EducationalBackground { get; set; }
        //联系信息
        [ImportAndExprotFiled(Name = "办公座机", SortOrder = 19)]
        public string OfficePhone { get; set; }

        [ImportAndExprotFiled(Name = "私人座机", SortOrder = 20)]
        public string HomePhone { get; set; }

        [ImportAndExprotFiled(Name = "办公邮箱", SortOrder = 21)]
        public string OfficeEmail { get; set; }

        [ImportAndExprotFiled(Name = "私人邮箱", SortOrder = 22)]
        public string HomeEmail { get; set; }

        [ImportAndExprotFiled(Name = "现住地地址", SortOrder = 23)]
        public string ResideAddress { get; set; }

        [ImportAndExprotFiled(Name = "家庭地址", SortOrder = 24)]
        public string HomeAddress { get; set; }

        [ImportAndExprotFiled(Name = "紧急联系人", SortOrder = 25)]
        public string EmergencyContactName { get; set; }

        [ImportAndExprotFiled(Name = "紧急联系电话", SortOrder = 26)]
        public string EmergencyContactPhone { get; set; }

        [ImportAndExprotFiled(Name = "入职日期", SortOrder = 27)]
        public string EntryDate { get; set; }

        [ImportAndExprotFiled(Name = "个人说明", SortOrder = 28)]
        public string Summary { get; set; }

        [ImportAndExprotFiled(Name = "失败原因", SortOrder = 29)]
        public string FaildReason { get; set; }
    }
}
