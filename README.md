# ZabbixReport - Zabbix 告警报告生成工具 📊

## 项目概述

ZabbixReport 是一个 **Zabbix 告警报告生成工具**，支持生成 **日报** 和 **月报**，与 [ZabbixAlert](https://github.com/shencifang/ZabbixAlert)（告警实时推送）配套使用。

### 功能对比

| 功能 | 数据来源 | 是否需要连接 Zabbix |
|------|----------|:---:|
| **日报**（生成日报） | Zabbix API 查询当天未关闭触发器 + MySQL 历史数据比对 | ✅ 是 |
| **月报（查询未关闭事件）** | Zabbix API 查询上月未关闭触发器 + MySQL 历史数据比对 | ✅ 是 |
| **月报（生成月报）** | MySQL `event` 表 | ❌ 否 |

### 与 ZabbixAlert 的关系

| 项目 | 功能 | 关系 |
|------|------|------|
| **ZabbixAlert** | 实时推送告警到桌面通知 | 告警发生时触发 |
| **ZabbixReport** | 日报/月报生成与邮件发送 | 告警发生后统计分析 |

两个项目共用同一套数据库（`zabbixmonitor`），ZabbixReport 做的是事后统计汇总与报告推送。

---

## 技术架构

| 层次 | 技术 |
|------|------|
| 编程语言 | C# |
| UI 框架 | Windows Forms (.NET Framework 4.6.1) |
| 数据库 | MySQL (MySql.Data) |
| 数据库工具层 | Maticsoft.DBUtility |
| Zabbix API | Zabbix JSON-RPC API（Newtonsoft.Json） |
| 邮件发送 | System.Net.Mail (SMTP) |
| IDE | Visual Studio 2012+ |

---

## 项目文件结构

```
ZabbixReport/
├── App.config                     # 应用配置文件（Zabbix连接、邮箱等）
├── Program.cs                     # 程序入口
├── Form1.cs                       # 主窗体逻辑（日报/月报生成 + 发送邮件）
├── Form1.Designer.cs              # 窗体设计器代码
├── Form1.resx                     # 窗体资源
├── Event.cs                       # 事件实体模型
├── EventDAL.cs                    # 数据访问层（数据库查询）
├── EventBLL.cs                    # 业务逻辑层
├── DbHelperMySQL.cs               # MySQL 数据库帮助类
├── Email.cs                       # 邮件发送组件（备用独立邮件发送类）
├── Zabbix.cs                      # Zabbix API 调用封装（JSON-RPC）
├── Request.cs                     # Zabbix JSON-RPC 请求对象
├── Response.cs                    # Zabbix JSON-RPC 响应对象
├── DateTimeUtil.cs                # 时间戳与 DateTime 互转工具
├── Log.cs                         # 日志组件
├── Properties/
│   ├── AssemblyInfo.cs            # 程序集信息
│   ├── Resources.resx             # 资源文件
│   └── Settings.settings          # 设置文件
├── tem/                           # HTML 模板目录（已纳入版本管理）
│   ├── head1.html                 # 日报/查询未关闭事件头部模板（CSS+标题）
│   ├── head2.html                 # 日报/查询未关闭事件段落模板（文档信息行）
│   ├── head3.html                 # 日报/查询未关闭事件表格标题模板
│   ├── medium.html                # 日报/查询未关闭事件所有事件过渡段模板
│   ├── foot.html                  # 尾部 HTML 模板
│   └── index.html                 # 生成的报告（程序自动输出）
└── README.md                      # 本文档
```

---

## 核心功能

### 1️⃣ 日报（button3 — 生成日报）

> ⚡ **需要连接 Zabbix API**

点击 **"生成日报"** 按钮，程序会：

1. 计算昨天到当前时间的时间范围
2. 通过 Zabbix API（`user.login`）登录 Zabbix
3. 调用 `trigger.get` 查询昨天至今严重等级 ≥ 3 的未关闭触发器
4. 将 Zabbix 返回的实时数据与本地 MySQL `event` 表比对，识别已知/未知事件
5. 在左侧文本框输出实时日志
6. 生成日报 HTML 文件（`tem/index.html`）
7. 设置邮件标题为：`"X月X日告警报告(日报)"`

### 2️⃣ 月报（button2 — 查询未关闭事件）

> ⚡ **需要连接 Zabbix API**

点击 **"查询未关闭事件"** 按钮，程序会：

1. Ping 检测 Zabbix 服务器连通性
2. 通过 Zabbix API 登录 Zabbix
3. 调用 `trigger.get` 查询上个月严重等级 ≥ 3 的未关闭触发器
4. 将 Zabbix 返回的实时数据与本地 MySQL `event` 表比对
5. 在左侧文本框输出实时日志
6. 拼接 HTML 模板（head1 → head2 → head3 + 表格 + medium），但 **不写入文件**
7. 设置邮件标题为：`"X月份告警报告"`
8. 启用"开始生成"按钮（start）

### 3️⃣ 月报（start — 开始生成）

> 💾 **仅使用 MySQL 数据，不需要连接 Zabbix**  
> ⚠️ **必须先点击 [查询未关闭事件]（button2）获取头部模板**，start 不读取 head 模板，而是复用 button2 预留在内存中的 `msg` 变量内容

点击 **"开始生成"** 按钮，程序会：

1. 自动计算 **上个月** 的时间范围（例如当前是 6 月，则查询 5 月 1 日 ~ 5 月 31 日）
2. 连接 MySQL 数据库，从 `event` 表中查询该时间段内的所有历史告警事件
3. 在已有头部内容（button2 构建的表格结构）基础上追加数据库数据行
4. 读取 `tem/foot.html` 尾部模板，组合生成完整报告
5. 将完整 HTML 写入 `tem/index.html` 文件
6. 在界面文本框中显示执行日志

### 4️⃣ 发送邮件（button1 — 发送邮件）

点击 **"发送邮件"** 按钮，程序会：

1. 读取刚生成的 `tem/index.html` 作为邮件正文
2. 将 `tem/index.html` 作为邮件附件一并发送
3. 通过配置的 SMTP 服务发送报告
4. 支持收件人、抄送人配置（多收件人以逗号分隔）


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
| `ip` | varchar(255) | 服务器名称 / IP |
| `content` | varchar(255) | 事件内容描述 |
| `gm` | varchar(255) | 工程师 / 处理人 |
| `recetime` | datetime | 恢复时间 |
| `close` | decimal | 是否关闭（0/1） |
| `closetime` | datetime | 关闭时间 |
| `cause` | varchar(255) | 故障原因 |

> ⚠️ **注意**：该表和数据库与 ZabbixAlert 项目共用。数据由 Zabbix 或其他程序写入，ZabbixReport **只读不写**。

---

## 部署要求

### 系统要求

- **操作系统**：Windows 7 / Windows 10 / Windows Server 2008+
- **.NET 运行时**：.NET Framework 4.6.1 或更高版本
  - [微软官网下载地址](https://www.microsoft.com/zh-CN/download/details.aspx?id=49981)
- **MySQL 数据库**：5.6+（可远程访问，需开放网络连接）
- **Zabbix 服务器**：日报/查询未关闭事件功能需要
- **邮箱**：支持 SMTP 的邮箱服务

### 部署步骤

1. **编译项目**
   - 用 Visual Studio 2012+ 打开 `20210621.sln`
   - 解决方案配置选 `Release`，目标平台 `Any CPU`
   - 生成解决方案（Build → Build Solution）

2. **HTML 模板目录**（已纳入版本管理，无需手动创建）
   ```
   ZabbixReport\tem\
   ├── head1.html    # 日报/查询未关闭事件头部（CSS+标题）
   ├── head2.html    # 日报/查询未关闭事件段落（文档信息行）
   ├── head3.html    # 日报/查询未关闭事件表格标题
   ├── medium.html   # 日报/查询未关闭事件所有事件过渡段
   ├── foot.html     # 尾部
   └── index.html    # 自动生成，无需手动创建
   ```

3. **修改配置文件 `20210621.exe.config`（App.config）**

   ```xml
   <appSettings>
     <!-- Zabbix 服务器配置 -->
     <add key="zabbixip" value="Zabbix服务器IP地址"/>
     <add key="zabbixurl" value="Zabbix API URL(如:http://your-zabbix/zabbix/api_jsonrpc.php)"/>
     <add key="zabbixuesr" value="Zabbix用户名"/>
     <add key="zabbixpass" value="Zabbix密码"/>
     
     <!-- 邮件配置 -->
     <add key="touser" value="收件人邮箱(多个以逗号分隔)"/>
     <add key="ccuser" value="抄送人邮箱(多个以逗号分隔)"/>
   </appSettings>
   ```

4. **修改数据库连接串**
   - 打开 `DbHelperMySQL.cs`，找到 `connectionString` 变量
   - 修改为实际的数据库 IP、用户名、密码
   ```csharp
   public static string connectionString = "server=数据库IP;database=zabbixmonitor;uid=用户名;pwd=密码;CharSet=utf8;";
   ```

5. **运行程序**
   - 执行 `20210621.exe`
   - 可按流程手动生成日报/月报并发送

---

## 使用流程

### 日报生成流程

```
1. 双击运行程序
2. 点击 [生成日报]（在"日报"分组框内）
    ├─ 调用 Zabbix API 登录
    ├─ 获取昨天至今未关闭的严重告警（实时数据）
    ├─ 与 MySQL 数据库比对
    └─ 生成 tem/index.html
3. 点击 [发送邮件]
    ├─ 读取生成的日报 HTML
    ├─ 通过 SMTP 发送至收件人/抄送人
    └─ 提示发送成功/失败
```

### 月报（查询未关闭事件）流程

```
1. 双击运行程序
2. 点击 [查询未关闭事件]（在"月报"分组框内）
    ├─ Ping Zabbix 服务器（验证连通性）
    ├─ 调用 Zabbix API 登录
    ├─ 获取上个月未关闭的严重告警（实时数据）
    ├─ 与 MySQL 数据库比对，输出实时日志
    └─ 拼接 HTML 报告（仅预览，不写入文件）
3. 再点击 [开始生成] 生成月报历史统计
4. 点击 [发送邮件] 发送报告
```

### 月报（历史统计）流程

```
1. 点击 [开始生成]（在"月报"分组框内）
    ├─ 自动计算上个月日期范围
    ├─ 连接 MySQL 数据库，查询 event 表
    ├─ 读取 HTML 模板
    ├─ 生成 tem/index.html 报告文件
    └─ 文本框输出执行日志
2. 检查生成的报告
    └─ 打开 tem/index.html 查看效果
3. 点击 [发送邮件]
    └─ 发送月报（正文 + 附件）
```

## 完整操作步骤

### 日报流程（button3 独立完成）
1. 点击 **[生成日报]**
2. 程序自动从 Zabbix API 获取未关闭触发器 + 数据库比对，生成 `tem/index.html`
3. 查看日志确认无误后，点击 **[发送邮件]**

### 月报流程（需要先查后生成）
1. **先**点击 **[查询未关闭事件]** → 从 Zabbix API 获取未关闭触发器并拼接 HTML 头部，启用 **[开始生成]**
2. **再**点击 **[开始生成]** → 从 MySQL 读取上月历史数据，追加到已有头部后写入 `tem/index.html`
3. 查看日志确认无误后，点击 **[发送邮件]**

> 注意：生成日报和开始生成都会覆盖 `tem/index.html`，如需保留请先备份。

## 常见问题

**Q：月报没有数据？**
- 检查 MySQL 数据库连接是否正常
- 检查 `event` 表中是否有上个月的数据
- 月报仅查 MySQL，不依赖 Zabbix 服务器

**Q：日报/查询未关闭事件查不到数据？**
- 检查 Zabbix 服务器 IP 是否能 Ping 通
- 检查 Zabbix API URL 是否正确
- 检查 Zabbix 用户名和密码是否正确
- 日报/查询未关闭事件需要 Zabbix API + MySQL 两者都正常

**Q：邮件发送失败？**
- 确认 SMTP 服务器地址和端口正确
- 确认使用的是 **SMTP 授权码**（非邮箱密码）
- 检查端口 25 是否被防火墙拦截

**Q：找不到 `tem/` 目录？**
- `tem/` 目录已纳入版本管理，执行 `git clone` 即可获得所有模板文件
- 如果是从压缩包或其他方式部署，请手动创建 `tem/` 目录并放入 `head1.html`、`head2.html`、`head3.html`、`medium.html`、`foot.html` 五个模板文件（内容见上方各文件说明）