using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Mail;
namespace _20210621
{
    public partial class Form1 : Form
    {
        Log log = new Log(AppDomain.CurrentDomain.BaseDirectory + @"/log/Log.txt");
        public _20210621.Event eventModule = new _20210621.Event();
        public _20210621.EventBLL eventBLL = new _20210621.EventBLL();
        //SendEmail SendEmail = new _20210621.SendEmail();
        //Email email = new _20210621.Email();
        public Form1()
        {
            InitializeComponent();
            //日志记录程序开始时间
            log.log("----------------------------------");
            log.log("程序开始");
            //email.Send();
        }


        private void start_Click(object sender, EventArgs e)
        {
            
            ///////////////////////////////////
            log.log("开始生成报告。");
            textBox1.Text += "[" + DateTime.Now.ToString() + "]开始生成报告。\r\n";
            log.log("当前为[" + DateTime.Now.Month.ToString() + "]月份。");
            textBox1.Text += "[" + DateTime.Now.ToString() + "]当前为[" + DateTime.Now.Month.ToString() + "]月份。\r\n";
            //if (DateTime.Now.Month.ToString() == "1"||DateTime.Now.Month.ToString() == "3"||DateTime.Now.Month.ToString() == "5"||DateTime.Now.Month.ToString() == "7"||DateTime.Now.Month.ToString() == "8"||DateTime.Now.Month.ToString() == "10"||DateTime.Now.Month.ToString() == "12") {
            //}
            //else {}
            DateTime d1 = new DateTime(DateTime.Now.Year, DateTime.Now.Month-1, 1);
            DateTime d2 = d1.AddMonths(1).AddDays(0);
            DateTime d3 = DateTime.Now.AddDays(1 - DateTime.Now.Day).AddMonths(-1);
            d3 = new DateTime(d3.Year, d3.Month, d3.Day);
            d2 = new DateTime(d2.Year, d2.Month, d2.Day);
            textBox1.Text += d2.ToString();

            log.log("进入数据库查询数据。");
            textBox1.Text += "[" + DateTime.Now.ToString() + "]进入数据库查询数据。\r\n";
            string msg = System.IO.File.ReadAllText("tem/head.html");
            //查询数据
            textBox1.Text += "[" + DateTime.Now.ToString() + "][当前时间]事件ID|事件时间|服务器信息|事件信息|工程师|恢复时间|是否关闭|关闭时间" + "\r\n";
            log.log("[当前时间]事件ID|事件时间|服务器信息|事件信息|工程师|恢复时间|是否关闭|关闭时间");
            List<_20210621.Event> todayeventlist = eventBLL.GetModelList("time>'" + d3.ToString("yyyy-MM-dd") + "' and " + "time<'" + d2.ToString("yyyy-MM-dd")+ "'");
            int rowCount = 0;
            foreach (_20210621.Event item in todayeventlist)
            {
                string temp = "[" + DateTime.Now.ToString() + "]" + item.id + "|" + item.triggerid + "|" + item.time + "|" + item.ip + "|" + item.content + "|" + item.gm + "|" + item.recetime + "|" + item.close + "|" + item.closetime + "|" + item.cause;
                textBox1.Text += temp + "\r\n";
                log.log(temp);
                msg += "<tr><td>" + item.id + "</td><td>" + item.triggerid + "</td><td>" + item.time + "</td><td>" + item.ip + "</td><td>" + item.content + "</td><td>" + item.gm + "</td><td>" + item.close + "</td><td>" + item.cause + "</td>";
                rowCount++;
            }

            msg += "<p>此表格生成于" + DateTime.Now.ToString() + "。共计" + rowCount.ToString() + "个事件</p>";
            msg += System.IO.File.ReadAllText("tem/foot.html");
            System.IO.File.WriteAllText("tem/index.html", msg, Encoding.UTF8);
            textBox1.SelectionStart = textBox1.Text.Length;
            textBox1.ScrollToCaret();
        }

        private void button1_Click(object sender, EventArgs e)
        {
	        System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();
	        msg.To.Add("发件人邮箱(如:your@qq.com)");
            //msg.To.Add("zhangyue@smartncs.com");

            //msg.To.Add("hehx@smartncs.com");
            //msg.To.Add("b@b.com");可以发送给多人 
            //msg.CC.Add("c@c.com");可以抄送给多人

            msg.From = new MailAddress("发件人邮箱(如:your@qq.com)", "发件人邮箱(如:your@qq.com)", System.Text.Encoding.UTF8);
	        /* 上面3个参数分别是发件人地址（可以随便写），发件人姓名，编码*/
            msg.Subject = "报告主题(如:XX月份告警报告)";
	        //邮件标题
	        msg.SubjectEncoding = System.Text.Encoding.UTF8;
	        //邮件标题编码
            msg.Body = System.IO.File.ReadAllText("tem/index.html");
	        //邮件内容
	        msg.BodyEncoding = System.Text.Encoding.UTF8;
	        //邮件内容编码
	        msg.IsBodyHtml = true;
	        //是否是HTML邮件
	        msg.Priority = MailPriority.High;
	        //邮件优先级
	        SmtpClient client = new SmtpClient();
            client.Credentials = new System.Net.NetworkCredential("发件人邮箱(如:your@qq.com)", "SMTP授权码");
	        //上述写你的GMail邮箱和密码
            client.Port = 25;
	        //Gmail使用的端口
            client.Host = "smtp.qq.com";
	        client.EnableSsl = true;
	        //经过ssl加密
	        object userState = msg;
	        try 
	        {
		        client.SendAsync(msg, userState);
		        //简单一点儿可以client.Send(msg);
		        MessageBox.Show("发送成功");
	        }
	        catch (System.Net.Mail.SmtpException ex) 
	        {
		        MessageBox.Show(ex.Message, "发送邮件出错");
	        }
        }

        
    }
}
