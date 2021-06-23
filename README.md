# ZabbixReport - Zabbix 月报生成器 🗓️

## 项目概述

ZabbixReport 是一个 **Zabbix 告警月报生成工具**，与 [ZabbixAlert](https://github.com/shencifang/ZabbixAlert)（告警实时推送）配套使用。

它从 Zabbix 监控系统的 MySQL 数据库中读取告警事件数据，自动生成 **上个月的告警统计报表**（HTML 格式），并通过 **邮件** 发送给指定收件人。

### 与 ZabbixAlert 的关系

| 项目 | 功能 | 关系 |
|------|------|------|
| **ZabbixAlert** | 实时推送告警到桌面通知 | 告警发生时触发 |
| **ZabbixReport** | 每月生成告警汇总报表 | 告警发生后统计分析 |

两个项目共用同一套数据库（`zabbixmonitor`），ZabbixReport 做的是事后统计汇总。

---

## 技术架构

| 层次 | 技术 |
|------|------|
| 编程语言 | C# |
| UI 框架 | Windows Forms (.NET Framework 4.6.1) |
| 数据库 | MySQL (MySql.Data) |
| 数据库工具层 | Maticsoft.DBUtility |
| 邮件发送 | System.Net.Mail (SMTP) |
| IDE | Visual Studio 2012+ |

## 项目文件结构

```
ZabbixReport/
├── App.config                  # 应用配置文件
├── Program.cs                  # 程序入口
├── Form1.cs                    # 主窗体逻辑（生成报告 + 发送邮件）
├── Form1.Designer.cs           # 窗体设计器代码
├── Form1.resx                  # 窗体资源
├── Event.cs                    # 事件实体模型
├── EventDAL.cs                 # 数据访问层（数据库查询）
├── EventBLL.cs                 # 业务逻辑层
├── DbHelperMySQL.cs            # MySQL 数据库帮助类
├── Email.cs                    # 邮件发送组件
├── Log.cs                      # 日志组件
├── Properties/
│   ├── AssemblyInfo.cs         # 程序集信息
│   ├── Resources.resx          # 资源文件
│   └── Settings.settings       # 设置文件
├── tem/                        # HTML 模板目录（需手动创建）
│   ├── head.html               # 报告头部 HTML 模板
│   ├── foot.html               # 报告尾部 HTML 模板
│   └── index.html              # 生成的报告（程序自动输出）
└── README.md                   # 本文档
```

---

## 核心功能

### 1️⃣ 生成月报

点击 **"开始生成"** 按钮，程序会：

1. 自动计算 **上个月** 的时间范围（如当前是 6 月，则查询 5 月 1 日 ~ 5 月 31 日）
2. 连接 MySQL 数据库，从 `event` 表中查询时间段内的所有告警事件
3. 读取 `tem/head.html` 作为报告开头，`tem/foot.html` 作为报告结尾
4. 将告警数据逐行插入 HTML 表格，生成完整报告文件 `tem/index.html`
5. 在界面文本框中显示执行日志

### 2️⃣ 发送邮件

点击 **"发送邮件"** 按钮，程序会：

1. 读取刚生成的 `tem/index.html` 作为邮件正文
2. 通过 QQ 邮箱 SMTP 服务发送报告
3. 邮件主题可自定义（如 "XX月份告警报告"）
4. 支持收件人、抄送、密送等配置

### 3️⃣ 日志记录

程序运行时会在 `log/` 目录下生成日志文件 `Log.txt`，记录所有操作过程。

---

## 数据库要求

### 数据库信息

| 项目 | 说明 |
|------|------|
| 数据库类型 | MySQL |
| 数据库名 | `zabbixmonitor` |
| 字符集 | utf8 |
| 数据来源 | Zabbix 前端写入的告警事件 |

### 数据表结构

**表名：`event`**

| 字段 | 类型 | 说明 |
|------|------|------|
| `id` | varchar(255) | 事件 ID（主键） |
| `triggerid` | varchar(255) | 触发器 ID |
| `time` | datetime | 事件发生时间 |
| `ip` | varchar(255) | 服务器 IP |
| `content` | varchar(255) | 事件内容描述 |
| `gm` | varchar(255) | 工程师/处理人 |
| `recetime` | datetime | 恢复时间 |
| `close` | decimal | 是否关闭（0/1） |
| `closetime` | datetime | 关闭时间 |
| `cause` | varchar(255) | 故障原因 |

> ⚠️ **注意**：该表和数据库与 ZabbixAlert 项目共用。数据由其他程序或手动写入，ZabbixReport 只读不写。

---

## 部署要求

### 系统要求

- **操作系统**：Windows 7 / Windows 10 / Windows Server 2008+
- **.NET 运行时**：.NET Framework 4.6.1 或更高版本
- **数据库**：MySQL 5.6+（可远程访问）
- **邮箱**：QQ 邮箱（或其他支持 SMTP 的邮箱）

### 部署步骤

1. **编译项目**
   - 用 Visual Studio 2012+ 打开 `20210621.sln`
   - 解决方案配置选 `Release`，目标平台 `Any CPU`
   - 生成解决方案（Build → Build Solution）

2. **创建 HTML 模板目录**
   ```
   ZabbixReport\bin\Release\tem\
   ├── head.html    # 报告头部，需自行编写 HTML 开头
   ├── foot.html    # 报告尾部，需自行编写 HTML 结尾
   └── index.html   # 自动生成，无需手动创建
   ```

3. **配置数据库连接**
   - 打开 `DbHelperMySQL.cs`，找到 `connectionString` 变量
   - 修改为实际的数据库 IP、用户名、密码
   ```csharp
   // 格式示例
   public static string connectionString = "server=数据库IP;database=zabbixmonitor;uid=用户名;pwd=密码;CharSet=utf8;";
   ```

4. **配置邮件发送**
   - 打开 `Form1.cs`，找到 `button1_Click` 方法
   - 修改发件人邮箱、SMTP 授权码、收件人地址
   - 修改报告主题

5. **运行程序**
   - 运行 `20210621.exe`
   - 点击 **"开始生成"** → 查看日志 → 点击 **"发送邮件"**

### HTML 模板示例

**tem/head.html** 参考：
```html
<!DOCTYPE html>
<html>
<head><meta charset="utf-8"><title>告警月报</title></head>
<body>
<h2>XX月份告警统计报告</h2>
<table border="1" cellpadding="5" cellspacing="0">
<tr>
  <th>事件ID</th><th>触发器ID</th><th>时间</th><th>服务器IP</th>
  <th>事件内容</th><th>工程师</th><th>是否关闭</th><th>原因</th>
</tr>
```

**tem/foot.html** 参考：
```html
</table>
</body>
</html>
```

---

## 使用流程

```
1. 双击运行程序
    └─ 窗体标题: "月报生成器"
    
2. 点击 [开始生成]
    ├─ 计算上个月日期范围
    ├─ 连接数据库查询 event 表
    ├─ 读取 head.html / foot.html 模板
    ├─ 生成 tem/index.html 报告文件
    └─ 文本框输出执行日志
    
3. 检查生成的报告
    └─ 打开 tem/index.html 查看效果
    
4. 点击 [发送邮件]
    ├─ 读取 tem/index.html 作为邮件正文
    ├─ 通过 QQ SMTP 发送
    └─ 提示发送成功/失败
```

---

## 常见问题

**Q：生成的报告没有数据？**
- 检查数据库连接是否正常
- 检查 `event` 表中是否有上个月的数据
- 检查日期范围计算是否正确

**Q：邮件发送失败？**
- 确认 QQ 邮箱 SMTP 服务已开启
- 确认使用的是 **SMTP 授权码**（非 QQ 密码）
- 检查端口 25 是否被防火墙拦截，可尝试 465 端口（SSL）

**Q：找不到 `tem/` 目录？**
- 手动创建 `tem/` 目录，并放入 `head.html` 和 `foot.html`

---

## 开发计划

- [ ] 支持自定义日期范围（不只是上个月）
- [ ] 支持多数据库来源
- [ ] 支持导出 PDF / Excel
- [ ] 支持定时自动生成和发送
- [ ] 统计图表展示

---

## 许可证

本项目仅用于学习和内部使用。
