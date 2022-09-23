using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agent.Core
{
    public class EnumHelper
    {
        #region 获取枚举
        public static List<EnumValue> GetEnumList(Type enumType)
        {
            var list = new List<EnumValue>();
            string[] strArray = Enum.GetNames(enumType);
            foreach (string item in strArray)
            {
                int enumValue = (int)Enum.Parse(enumType, item, true);
                string text = GetEnumDescription(enumType, item);

                list.Add(new EnumValue { Text = text, Value = enumValue });

            }
            return list;
        }

        public static List<EnumberEntity> EnumToList<T>()
        {
            var list = new List<EnumberEntity>();

            foreach (var e in Enum.GetValues(typeof(T)))
            {
                var m = new EnumberEntity();
                var objArr = e.GetType().GetField(e.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (objArr.Length > 0)
                {
                    var da = objArr[0] as DescriptionAttribute;
                    if (da != null) m.Description = da.Description;
                }
                m.Value = Convert.ToInt32(e);
                m.Name = e.ToString();
                list.Add(m);
            }
            return list;
        }

        /// <summary>
        /// 获得描述
        /// </summary>
        /// <param name="value">枚举</param>
        /// <returns>描述内容</returns>
        public static string GetEnumDescription(Enum value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            string description = value.ToString();

            Type eunmtype = value.GetType();

            return GetEnumDescription(eunmtype, description);
        }

        public static string GetEnumDescription(Type enumType, string name)
        {
            try
            {
                DescriptionAttribute[] attributes = (DescriptionAttribute[])enumType.GetField(name).GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attributes != null && attributes.Length > 0)
                {
                    name = attributes[0].Description;
                }
                else
                {
                    name = name.ToString();
                }
                return name;
            }
            catch
            {
                return string.Empty;
            }
        }
        #endregion
    }

    public class EnumValue
    {
        public int Value { get; set; }
        public string Text { get; set; }
    }

    public class EnumberEntity
    {
        /// <summary>
        /// 枚举的描述
        /// </summary>
        public string Description { set; get; }

        /// <summary>
        /// 枚举名称
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// 枚举对象的值
        /// </summary>
        public int Value { set; get; }
    }
}
