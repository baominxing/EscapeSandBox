using EscapeSandBox.Api.Core;
using System.ComponentModel;

namespace EscapeSandBox.Api.Dto
{
    public class R<T>
    {
        /// <summary>
        /// 请求状态
        /// </summary>
        public EnumStatus Status { get; set; } = EnumStatus.Success;

        private string? _msg;

        /// <summary>
        /// 消息描述
        /// </summary>
        public string Message
        {
            get { return !string.IsNullOrEmpty(_msg) ? _msg : EnumHelper.GetEnumDescription(Status); }
            set { _msg = value; }
        }

        /// <summary>
        /// 返回结果
        /// </summary>
        public T? Data { get; set; }
    }

    public enum EnumStatus
    {
        [Description("请求成功")]
        Success = 1,
        [Description("请求失败")]
        Failure = 0,
        [Description("请求异常")]
        Unknown = -1,

    }
}
