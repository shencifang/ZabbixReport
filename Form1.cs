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
using System.Net.NetworkInformation;
using System.Web;
using System.Configuration;
namespace _20210621
{
    public partial class Form1 : Form
    {
        Log log = new Log(AppDomain.CurrentDomain.BaseDirectory + @"/log/Log.txt");
        public _20210621.Event eventModule = new _20210621.Event();
        public _20210621.EventBLL eventBLL = new _20210621.EventBLL();
        Zabbix zabbix = new Zabbix(ConfigurationManager.AppSettings["zabbixuesr"], ConfigurationManager.AppSettings["zabbixpass"], ConfigurationManager.AppSettings["zabbixurl"]);
        //Zabbix zabbix = new Zabbix(ConfigurationManager.AppSettings["zabbixuesr"], ConfigurationManager.AppSettings["zabbixpass"], "Zabbix API URL(如:http://your-zabbix/zabbix/api_jsonrpc.php)");
        string msg = "";
        string title = "";
        public Form1()
        {
            InitializeComponent();
            //日志记录程序开始时间
            log.log("----------------------------------");
            log.log("程序开始");
            //button1.Enabled = false;
            start.Enabled = false;
            log.log("开始生成报告。");
            textBox1.Text += "[" + DateTime.Now.ToString() + "]开始生成报告。\r\n";
            log.log("当前为[" + DateTime.Now.Month.ToString() + "]月份。");
            textBox1.Text += "[" + DateTime.Now.ToString() + "]当前为[" + DateTime.Now.Month.ToString() + "]月份。\r\n";
        }
        
        

        private void start_Click(object sender, EventArgs e)
        {
            DateTime d1 = new DateTime(DateTime.Now.Year, DateTime.Now.Month-1, 1);
            DateTime d2 = d1.AddMonths(1).AddDays(0);
            DateTime d3 = DateTime.Now.AddDays(1 - DateTime.Now.Day).AddMonths(-1);
            d3 = new DateTime(d3.Year, d3.Month, d3.Day);
            d2 = new DateTime(d2.Year, d2.Month, d2.Day);

            log.log("进入数据库查询数据。");
            textBox1.Text += "[" + DateTime.Now.ToString() + "]进入数据库查询数据。\r\n";
            
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
                msg += "<tr><td>" + item.id + "</td><td>" + item.triggerid + "</td><td>" + item.time + "</td><td>" + item.ip + "</td><td>" + item.content + "</td><td>" + item.gm + "</td><td>" + item.close + "</td><td>" + item.cause + "</td></tr>\r\n";//</table>
                rowCount++;
            }

            msg += "</table><p>此表格生成于" + DateTime.Now.ToString() + "。共计" + rowCount.ToString() + "个事件</p>";
            msg += System.IO.File.ReadAllText("tem/foot.html");
            System.IO.File.WriteAllText("tem/index.html", msg, Encoding.UTF8);
            textBox1.SelectionStart = textBox1.Text.Length;
            textBox1.ScrollToCaret();
            //button1.Enabled = true;
            start.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            DateTime d3 = DateTime.Now.AddDays(1 - DateTime.Now.Day).AddMonths(-1);
            d3 = new DateTime(d3.Year, d3.Month, d3.Day);

	        System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();
            if (ConfigurationManager.AppSettings["touser"] == "") { MessageBox.Show("发件人为空"); return; }
            else msg.To.Add(ConfigurationManager.AppSettings["touser"]);

            if (ConfigurationManager.AppSettings["ccuser"] == "") {  }
            else msg.CC.Add(ConfigurationManager.AppSettings["ccuser"]);
            //msg.To.Add("收件人邮箱(如:user@example.com)");
            //msg.To.Add("b@b.com");可以发送给多人 
            //msg.CC.Add("c@c.com");可以抄送给多人

            msg.From = new MailAddress("发件人邮箱(如:your@qq.com)", "发件人显示名称", System.Text.Encoding.UTF8);
	        /* 上面3个参数分别是发件人地址（可以随便写），发件人姓名，编码*/
            msg.Subject = title;
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
            
            Attachment data = new Attachment("tem/index.html");
            msg.Attachments.Add(data);
            //附件
	        SmtpClient client = new SmtpClient();
            client.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["mail"], ConfigurationManager.AppSettings["pass"]);
	        //上述写你的GMail邮箱和密码
            client.Port = 25;
	        //Gmail使用的端口
            client.Host = "smtp.qq.com";
	        client.EnableSsl = false;
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

        private void button2_Click(object sender, EventArgs e)
        {
            button3.Enabled = false;
            DateTime d1 = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 1);
            DateTime d2 = d1.AddMonths(1).AddDays(0);
            DateTime d3 = DateTime.Now.AddDays(1 - DateTime.Now.Day).AddMonths(-1);
            d3 = new DateTime(d3.Year, d3.Month, d3.Day);
            d2 = new DateTime(d2.Year, d2.Month, d2.Day);
            long lastChangeSinceTime = DateTimeUtil.DateTimeToTimeStamp(d1);
            long lastChangeTillTime = DateTimeUtil.DateTimeToTimeStamp(d2);
            msg = "";
            msg = System.IO.File.ReadAllText("tem/head1.html");
            msg += d1 + "-" + d2 + System.IO.File.ReadAllText("tem/head2.html") + DateTime.Now.ToString() + System.IO.File.ReadAllText("tem/head3.html");
            Ping p1 = new Ping();
            string host = ConfigurationManager.AppSettings["zabbixip"];
            PingReply reply = p1.Send(host);
            textBox1.Text += "ZABBIX服务器地址：" + host + "\r\n";
            log.log("ZABBIX服务器地址：" + host);
                textBox1.Text += "连接成功\r\n";
                log.log("连接成功");
                //登陆
                zabbix.login();
                if (zabbix.loggedOn) { 
                    textBox1.Text += "登录成功\r\n";
                    log.log("登录成功");
                    int rowCount = 0;
                    Response responseObj = zabbix.objectResponse("trigger.get", new
                    {
                        output = new string[] { "hostname", "description", "lastchange", "priority", "value", "status", "triggerid" },
                        min_severity = 3,
                        expandData = true,
                        expandDescription = true,
                        expandExpression = true,
                        selectHosts = "extend",
                        selectGroups = "extend",
                        monitored = true,
                        sortfield = "hostname",
                        skipDependent = true,
                        lastChangeSince = lastChangeSinceTime,
                        lastChangeTill = lastChangeTillTime,
                        filter = new { value = 1 }
                    });
                    foreach (dynamic data in responseObj.result)
                    {
                        textBox1.Text += data.triggerid + "-" + data.hostname + "-" + data.description + "-" + data.lastchange + "\r\n";
                        log.log(data.triggerid + "-" + data.hostname + "-" + data.description + "-" + data.lastchange);
                        List<_20210621.Event> eventlist = eventBLL.GetModelList("id='" + data.lastchange + data.triggerid + "'");
                        if (eventlist.Count == 0) { 
                            textBox1.Text += "数据异常\r\n";
                            log.log("获取到的事件在数据库中未查询到。");
                            msg += "<tr><td>" + data.lastchange + data.triggerid + "</td><td>" + data.triggerid + "</td><td>" + DateTimeUtil.TimeStampToDateTime(long.Parse(data.lastchange)) + "</td><td>" + data.hostname + "</td><td>" + data.description + "</td><td>" + "-" + "</td><td>0</td><td>" + " " + "</td>";//</tr></table>
                            rowCount++;
                        }else {
                            foreach (_20210621.Event item in eventlist)
                            {
                                string temp = "[" + DateTime.Now.ToString() + "]" + item.id + "|" + item.triggerid + "|" + item.time + "|" + item.ip + "|" + item.content + "|" + item.gm + "|" + item.recetime + "|" + item.close + "|" + item.closetime + "|" + item.cause;
                                textBox1.Text += temp + "\r\n";
                                log.log(temp);
                                msg += "<tr><td>" + item.id + "</td><td>" + item.triggerid + "</td><td>" + item.time + "</td><td>" + item.ip + "</td><td>" + item.content + "</td><td>" + item.gm + "</td><td>0</td><td>" + item.cause + "</td>";//</tr></table>
                                rowCount++;
                            }
                        }
                    }
                        msg += "</tr></table><p>此表格生成于" + DateTime.Now.ToString() + "。共计" + rowCount.ToString() + "个事件至今未关闭</p>";
                        msg += System.IO.File.ReadAllText("tem/medium.html"); 
                    zabbix.logout();
                    button2.Enabled = false;
                    start.Enabled = true;
                    title = d3.Month + "月份告警报告";
                }
                else { textBox1.Text += "登录失败\r\n"; }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            start.Enabled = false;
            DateTime d1 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day - 1);
            DateTime d2 = DateTime.Now;
            //d2 = new DateTime(d2.Year, d2.Month, d2.Day);
            long lastChangeSinceTime = DateTimeUtil.DateTimeToTimeStamp(d1);
            long lastChangeTillTime = DateTimeUtil.DateTimeToTimeStamp(d2);

            msg = "";
            msg = System.IO.File.ReadAllText("tem/head1.html");
            msg += d1 + "-" + d2 + System.IO.File.ReadAllText("tem/head2.html") + DateTime.Now.ToString() + System.IO.File.ReadAllText("tem/head3.html");
                //登陆
                zabbix.login();
                if (zabbix.loggedOn)
                {
                    textBox1.Text += "登录成功\r\n";
                    log.log("登录成功");
                    int rowCount = 0;
                    Response responseObj = zabbix.objectResponse("trigger.get", new
                    {
                        output = new string[] { "hostname", "description", "lastchange", "priority", "value", "status", "triggerid" },
                        min_severity = 3,
                        expandData = true,
                        expandDescription = true,
                        expandExpression = true,
                        selectHosts = "extend",
                        selectGroups = "extend",
                        monitored = true,
                        sortfield = "hostname",
                        skipDependent = true,
                        lastChangeSince = lastChangeSinceTime,
                        lastChangeTill = lastChangeTillTime,
                        filter = new { value = 1 }
                    });
                    foreach (dynamic data in responseObj.result)
                    {
                        textBox1.Text += data.triggerid + "-" + data.hostname + "-" + data.description + "-" + data.lastchange + "\r\n";
                        log.log(data.triggerid + "-" + data.hostname + "-" + data.description + "-" + data.lastchange);
                        List<_20210621.Event> eventlist = eventBLL.GetModelList("id='" + data.lastchange + data.triggerid + "'");
                        if (eventlist.Count == 0)
                        {
                            textBox1.Text += "数据异常\r\n";
                            log.log("获取到的事件在数据库中未查询到。");
                            msg += "<tr><td>" + data.lastchange + data.triggerid + "</td><td>" + data.triggerid + "</td><td>" + DateTimeUtil.TimeStampToDateTime(long.Parse(data.lastchange)) + "</td><td>" + data.hostname + "</td><td>" + data.description + "</td><td>" + "-" + "</td><td>0</td><td>" + " " + "</td>";//</tr></table>
                            rowCount++;
                        }
                        else
                        {
                            foreach (_20210621.Event item in eventlist)
                            {
                                string temp = "[" + DateTime.Now.ToString() + "]" + item.id + "|" + item.triggerid + "|" + item.time + "|" + item.ip + "|" + item.content + "|" + item.gm + "|" + item.recetime + "|" + item.close + "|" + item.closetime + "|" + item.cause;
                                textBox1.Text += temp + "\r\n";
                                log.log(temp);
                                msg += "<tr><td>" + item.id + "</td><td>" + item.triggerid + "</td><td>" + item.time + "</td><td>" + item.ip + "</td><td>" + item.content + "</td><td>" + item.gm + "</td><td>0</td><td>" + item.cause + "</td>";//</tr></table>
                                rowCount++;
                            }
                        }
                    }
                    msg += "</tr></table><p>此表格生成于" + DateTime.Now.ToString() + "。共计" + rowCount.ToString() + "个事件至今未关闭</p>";
                    //msg += System.IO.File.ReadAllText("tem/medium.html");
                    msg += System.IO.File.ReadAllText("tem/foot.html");
                    System.IO.File.WriteAllText("tem/index.html", msg, Encoding.UTF8);
                    zabbix.logout();
                    title = DateTime.Now.Month + "月" + DateTime.Now.Day + "日告警报告(日报)"; ;
                }
                else { textBox1.Text += "登录失败\r\n"; }
        }
    }
}
