using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Web;
using System.Net;
using System.Text.RegularExpressions;
using System.Net.Sockets;
using System.IO;
using System.Collections;
using System.Threading; 

namespace _20210621
{
     public class Email
    {
        /// <summary>
        /// 无参构造器
        /// </summary>
        public Email() { }

        #region 属性
        /// <summary>
        /// 发送到邮箱地址 如果是多个 用","分开 如：jacky@zhuovi.com:jacky,service@zhuovi.com:service
        /// </summary>
        private string _ToMail = "收件人邮箱(如:your@qq.com)";
        /// <summary>
        /// 发送到邮箱地址 如果是多个 用","分开 如：jacky@zhuovi.com:jacky,service@zhuovi.com:service
        /// </summary>
        public string ToMail { get { return _ToMail; } set { _ToMail = value; } }
        /// <summary>
        /// 密送邮箱地址 如果是多个 用","分开 如：jacky@zhuovi.com:jacky,service@zhuovi.com:service
        /// </summary>
        private string _BCCMail = "";
        /// <summary>
        /// 密送到邮箱地址 如果是多个 用","分开 如：jacky@zhuovi.com:jacky,service@zhuovi.com:service
        /// </summary>
        public string BCCMail { get { return _BCCMail; } set { _BCCMail = value; } }
        /// <summary>
        /// 抄送邮箱地址 如果是多个 用","分开 如：jacky@zhuovi.com:jacky,service@zhuovi.com:service
        /// </summary>
        private string _CCMail = "";
        /// <summary>
        /// 抄送到邮箱地址 如果是多个 用","分开 如：jacky@zhuovi.com:jacky,service@zhuovi.comt:service
        /// </summary>
        public string CCMail { get { return _CCMail; } set { _CCMail = value; } }
        /// <summary>
        /// 发送邮箱地址
        /// </summary>
        private string _FromMail = "发件人邮箱(如:your@qq.com)";
        /// <summary>
        /// 发送邮箱地址
        /// </summary>
        public string FromMail { get { return _FromMail; } set { _FromMail = value; } }
        /// <summary>
        ///  发送人姓名
        /// </summary>
        private string _FromName = "默认测试标题";
        /// <summary>
        /// 发送人姓名
        /// </summary>
        public string FromName { get { return _FromName; } set { _FromName = value; } }
        /// <summary>
        /// 标题
        /// </summary>
        private string _Subject = "默认测试标题";
        /// <summary>
        /// 标题
        /// </summary>
        public string Subject { get { return _Subject; } set { _Subject = value; } }
        /// <summary>
        /// 标题编码
        /// </summary>
        private string _SubjectEncoding = "";
        /// <summary>
        /// 标题编码
        /// </summary>
        public string SubjectEncoding { get { return _SubjectEncoding; } set { _SubjectEncoding = value; } }
        /// <summary>
        /// 内容
        /// </summary>
        private string _Body = "默认测试标题";
        /// <summary>
        /// 内容
        /// </summary>
        public string Body { get { return _Body; } set { _Body = value; } }
        /// <summary>
        /// 内容编码
        /// </summary>
        private string _BodyEncoding = "";
        /// <summary>
        /// 内容编码
        /// </summary>
        public string BodyEncoding { get { return _BodyEncoding; } set { _BodyEncoding = value; } }
        /// <summary>
        /// 用户帐号
        /// </summary>
        private string _UserName = "邮箱用户名(如:your)";
        /// <summary>
        /// 用户帐号
        /// </summary>
        public string UserName { get { return _UserName; } set { _UserName = value; } }
        /// <summary>
        /// 用户密码
        /// </summary>
        private string _UserPwd = "SMTP授权码";
        /// <summary>
        /// 用户密码
        /// </summary>
        public string UserPwd { get { return _UserPwd; } set { _UserPwd = value; } }
        /// <summary>
        /// 邮件服务器
        /// </summary>
        private string _SmtpHost = "smtp.qq.com";
        /// <summary>
        /// 邮件服务器
        /// </summary>
        public string SmtpHost { get { return _SmtpHost; } set { _SmtpHost = value; } }
        /// <summary>
        /// 邮件服务器端口
        /// </summary>
        private int _SmtpPort = 25;
        /// <summary>
        /// 邮件服务器端口
        /// </summary>
        public int SmtpPort { get { return _SmtpPort; } set { _SmtpPort = value; } }
        /// <summary>
        /// 附件
        /// </summary>
        private string[] _Attachments = null;
        /// <summary>
        /// 附件
        /// </summary>
        public string[] Attachments { get { return _Attachments; } set { _Attachments = value; } }
        /// <summary>
        /// 邮件优先级
        /// </summary>
        private System.Net.Mail.MailPriority _Priority = System.Net.Mail.MailPriority.Normal;
        /// <summary>
        /// 邮件优先级
        /// </summary>
        public System.Net.Mail.MailPriority Priority { get { return _Priority; } set { _Priority = value; } }
        /// <summary>
        /// 发送错误信息
        /// </summary>
        private string _ErrorMessage = "";
        /// <summary>
        /// 发送错误信息
        /// </summary>
        public string ErrorMessage { get { return _ErrorMessage; } set { _ErrorMessage = value; } }
        #endregion

        #region 发送邮件
        /// <summary>
        /// 发送邮件
        /// </summary>
        public Boolean Send()
        {
            System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
            message.From = new System.Net.Mail.MailAddress(this.FromMail, (this.FromName == "" ? this.UserName : this.FromName));
            message.To.Clear();
            foreach (string destemail in this.ToMail.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                if (destemail.Contains(":"))
                {
                    string[] mailInfo = destemail.Split(':');
                    message.To.Add(new System.Net.Mail.MailAddress(mailInfo[0].Trim(), mailInfo[1].Trim()));
                }
                else
                {
                    message.To.Add(destemail);
                }
            }
            if (this.CCMail != "")
            {
                message.CC.Clear();
                foreach (string destemail in this.CCMail.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                {
                    if (destemail.Contains(":"))
                    {
                        string[] mailInfo = destemail.Split(':');
                        message.To.Add(new System.Net.Mail.MailAddress(mailInfo[0].Trim(), mailInfo[1].Trim()));
                    }
                    else
                    {
                        message.CC.Add(destemail);
                    }
                }
            }
            if (this.BCCMail != "")
            {
                message.Bcc.Clear();
                foreach (string destemail in this.BCCMail.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                {
                    if (destemail.Contains(":"))
                    {
                        string[] mailInfo = destemail.Split(':');
                        message.To.Add(new System.Net.Mail.MailAddress(mailInfo[0].Trim(), mailInfo[1].Trim()));
                    }
                    else
                    {
                        message.Bcc.Add(destemail);
                    }
                }
            }
            message.Subject = this.Subject;//设置邮件主题 
            if (this.BodyEncoding != "") message.SubjectEncoding = Encoding.GetEncoding(this.SubjectEncoding);
            message.IsBodyHtml = true;//设置邮件正文为html格式 
            message.Body = this.Body;//设置邮件内容 

            if (this.BodyEncoding != "") message.BodyEncoding = Encoding.GetEncoding(this.BodyEncoding);
            //添加附件
            if (this.Attachments != null)
            {
                foreach (string path in this.Attachments)
                {
                    if (!string.IsNullOrEmpty(path))
                        message.Attachments.Add(new System.Net.Mail.Attachment(path));
                }
            }
            //邮件的优先级
            message.Priority = this.Priority;
            //邮件发送人地址
            message.Sender = message.From;
            System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient(this.SmtpHost, this.SmtpPort);
            //设置发送邮件身份验证方式 
            //注意如果发件人地址是jacky@zhuovi.com，则用户名是jacky而不是jacky@zhuovi.com 
            client.Credentials = new System.Net.NetworkCredential(this.UserName, this.UserPwd);
            try
            {
                client.Send(message);
                return true;
            }
            catch (Exception e)
            {
                this.ErrorMessage = e.Message;
                return false;
            }
        }
        #endregion
}
 }