### Lycoris.Common —— .NET 通用扩展库

一套全面的 .NET 扩展方法与工具类库，涵盖字符串、时间、集合、加解密、文件、网络、配置管理、敏感词过滤、SEO 推送等常用功能。

### 安装方式

```shell
// .NET CLI
dotnet add package Lycoris.Common
// Package Manager
Install-Package Lycoris.Common
```

---

## 一、扩展方法 (Extensions)

### 1.1 StringExtensions —— 字符串扩展

#### 类型转换

| 方法 | 说明 |
|------|------|
| `ToInt()` | 字符串转 int |
| `ToTryInt()` | 字符串转 int? (安全转换) |
| `ToLong()` | 字符串转 long |
| `ToTryLong()` | 字符串转 long? (安全转换) |
| `ToFloat()` | 字符串转 float |
| `ToTryFloat()` | 字符串转 float? (安全转换) |
| `ToDouble()` | 字符串转 double |
| `ToTryDouble()` | 字符串转 double? (安全转换) |
| `ToDecimal()` | 字符串转 decimal，支持指定小数位数 |
| `ToTryDecimal()` | 字符串转 decimal?，支持指定小数位数 |
| `ToDateTime()` | 字符串转 DateTime |
| `ToTryDateTime()` | 字符串转 DateTime? (安全转换) |
| `ToBool()` | 字符串转 bool (数值0为false，其他为true) |
| `ToTryBool()` | 字符串转 bool? (安全转换) |
| `ToGuid()` | 字符串转 Guid |
| `ToTryGuid()` | 字符串转 Guid? (安全转换) |
| `ToByte()` | 字符串转 byte |
| `ToTryByte()` | 字符串转 byte? (安全转换) |
| `ToBytes()` | 字符串转 byte[]，支持指定编码 |
| `ToTryBytes()` | 字符串转 byte[]? (安全转换)，支持指定编码 |
| `ToEnum<T>()` | 字符串转枚举 |
| `ToTryEnum<T>()` | 字符串转枚举? (安全转换) |
| `ToEnumByDescription<T>()` | 通过枚举文字描述获取枚举项 |

#### 字符串判断

| 方法 | 说明 |
|------|------|
| `IsNullOrEmpty()` | 字符串是否为空 |
| `IsNullOrWhiteSpace()` | 字符串是否为空或空格 |
| `IsJson()` | 判断字符串是否为有效 JSON 格式 |
| `HasAnyChar(params char[])` | 是否包含数组中任意字符 |
| `HasAllChar(params char[])` | 是否包含数组中所有字符 |
| `HasAnyString(params string[])` | 是否包含数组中任意字符串 |
| `HasAllString(params string[])` | 是否包含数组中所有字符串 |

#### 字符串操作

| 方法 | 说明 |
|------|------|
| `ToUrlEncode()` / `ToUrlDecode()` | URL 编码/解码 |
| `ReplaceWhitespace(replacement)` | 替换所有空白字符 |
| `ReplaceFirst(search, replace)` | 只替换第一个匹配项 |
| `GetStartsWith(length)` | 获取字符串前 N 个字符 |
| `ToParamToJson()` | URL 键值对转 JSON 字符串 |
| `ToPascalCase()` | 转换为大驼峰命名 (PascalCase) |
| `ToCamelCase()` | 转换为小驼峰命名 (camelCase) |
| `RemoveUrlScheme()` | 去除 URL 中的 http:// 或 https:// 前缀 |

---

### 1.2 DateTimeExtensions —— 时间扩展

| 方法 | 说明 |
|------|------|
| `ToLocalTimeKind()` | 将 DateTime 的 Kind 设为 Local |
| `ToTimeStamp(length)` | DateTime 转时间戳（支持10位秒/13位毫秒） |
| `ToChinese()` | DateTime 转中文格式 `2000年01月01日 00时00分00秒` |
| `TimeStampEquals(second, dateTime)` | 时间戳比较（DateTime 扩展，判断两时间差是否在指定秒数内） |
| `TimeStampEquals(dateTime, second)` | 时间戳比较（long 扩展，判断时间戳差值是否在指定秒内） |
| `Yesterday()` | 获取昨天的日期 |
| `Tomorrow()` | 获取明天的日期 |
| `MonthFirstDay()` | 获取所属月份第一天 |
| `MonthLastDay()` | 获取所属月份最后一天 |
| `DateEndTime()` | 获取当天 23:59:59 |
| `ToChineseRunTime()` | TimeSpan 转为中文运行时长 `00 天 00 小时 00 分 00 秒` |

**TimeStampLength 枚举:** `Long` (13位毫秒), `Short` (10位秒)

---

### 1.3 ObjectExtensions —— 对象/集合扩展

| 方法 | 说明 |
|------|------|
| `HasValue<T>()` | 判断 IEnumerable / 数组是否有元素，支持 predicate 过滤 |
| `ForEach<T>(action)` | IEnumerable / 数组的 ForEach 遍历 |
| `ForEachAsync<T>(func)` | IEnumerable / 数组的异步 ForEach 遍历 |
| `CloneObject<T>()` | 对象深拷贝（通过 JSON 序列化实现） |
| `RemoveWhere<T>(predicate)` | 从数组/IList/Dictionary 中移除满足条件的元素 |
| `ToAsciiSortParams()` | Dictionary 或对象属性按 ASCII 排序生成 URL 参数字符串 |
| `IsSubclassFrom(type)` | 判断一个类型是否继承自另一个类（支持泛型基类） |
| `IsSubclassFrom<T>()` | 判断一个类型是否继承自 T |
| `IsInterfaceFrom(type)` | 判断一个类型是否实现了指定接口（支持泛型接口） |
| `IsInterfaceFrom<T>()` | 判断一个类型是否实现了 T 接口 |
| `GetValue(key)` | 从 Dictionary 安全获取值 |
| `AddOrUpdate(key, value)` | Dictionary 添加或更新键值 |
| `RemoveValue(key)` | Dictionary 移除指定键 |

---

### 1.4 ByteExtensions —— 字节数组扩展

| 方法 | 说明 |
|------|------|
| `SaveAs(path)` / `SaveAs(path, fileName)` | 保存字节数组到本地文件 |
| `SaveAsAsync(path)` / `SaveAsAsync(path, fileName)` | 异步保存字节数组到本地文件 |
| `ToMemoryStream()` | 字节数组转 MemoryStream |
| `ToMemoryStreamAsync()` | 异步字节数组转 MemoryStream |

---

### 1.5 StreamExtensions —— 流扩展

| 方法 | 说明 |
|------|------|
| `ToBytes()` | Stream/FileStream 转 byte[] |
| `ToBytesAsync()` | 异步 Stream/FileStream 转 byte[] |
| `SaveAs(path)` / `SaveAs(path, fileName)` | 保存流到本地文件 |
| `SaveAsAsync(path)` / `SaveAsAsync(path, fileName)` | 异步保存流到本地文件 |

---

### 1.6 DataTableExtensions —— DataTable 扩展

| 方法 | 说明 |
|------|------|
| `FirstOrDefault()` | 获取 DataTable 第一行 / 第一行的某列值 |
| `LastOrDefault()` | 获取 DataTable 最后一行 / 最后一行的某列值 |
| `ToList<T>()` | DataTable 转 List\<T\>（通过反射映射，支持 ColumnAttribute） |

---

### 1.7 EnumExtensions —— 枚举扩展

| 方法 | 说明 |
|------|------|
| `GetAttribute<TAttribute>()` | 获取枚举值的自定义特性 |
| `GetEnumDescription<T>()` | 获取枚举值的 Description 文字描述 |

---

### 1.8 ExceptionExtension —— 异常扩展

| 方法 | 说明 |
|------|------|
| `GetStackTraceService<T>()` | 获取触发异常的类名 |
| `GetStackTraceMethod<T>()` | 获取触发异常的方法名 |
| `GetStackTraceServiceMethod<T>()` | 获取触发异常的 (类名, 方法名) 元组 |
| `GetStackTraceMessage<T>()` | 获取精简后的异常堆栈信息 |

---

### 1.9 LinqExtensions —— LINQ 扩展

| 方法 | 说明 |
|------|------|
| `WhereIf(condition, predicate)` | 条件成立时才执行 Where，支持 else 分支，支持 IEnumerable/IQueryable |
| `Distinct(keySelector)` | 按属性去重（IEnumerable，已标记 Obsolete，建议用系统 DistinctBy） |
| `Distinct(propertyName)` | 按属性名去重（IQueryable，已标记 Obsolete） |
| `PageBy(pageIndex, pageSize)` | 分页（页码从1开始），支持 IEnumerable/IQueryable |
| `PageSum(expression)` | 合计查询 |
| `UpdatePropertyIf(condition, action)` | 条件成立时更新对象属性 |
| `Between(keySelector, low, high)` | Between 区间查询操作符 |
| `WhereOr(firstCondition)` | OR 条件构建器入口（返回 IOrWhereBuilder） |
| `SortBy(sortFields)` | 动态多字段排序（Dictionary<string, string>，支持嵌套属性 a.b.c） |
| `WhereContainsAny(selector, values)` | 查询字段包含集合中任意值的记录 |

**IOrWhereBuilder\<T\> 接口:**
- `Or(condition)` — 添加 OR 条件
- `Where(predicate)` — 附加 AND 条件
- `WhereIf(condition, predicate)` — 条件式 AND 条件
- `AsQueryable()` — 输出最终 IQueryable

---

### 1.10 MethodExtensions —— 方法扩展

| 方法 | 说明 |
|------|------|
| `IsAsync()` | 判断 MethodInfo 是否为异步方法 |
| `RunSync()` | 异步 Task 同步执行 |
| `RunSync<TResult>()` | 异步 Task\<T\> 同步执行并返回结果 |
| `RunAsync(func)` | 同步方法转 Task 异步执行 |
| `IgnoreException<TException>()` | 忽略指定类型的异常（同步/异步） |
| `HandleException<TException>(handler)` | 捕获指定类型的异常并处理（同步/异步） |
| `Catch(handler)` | 捕获所有异常并处理（同步/异步） |
| `ExecuteWithTimeoutAsync(timeout)` | 带超时的异步操作执行 |

---

### 1.11 NumberExtensions —— 数值扩展

| 方法 | 说明 |
|------|------|
| `ToRmbChinese()` | 转换人民币大小金额（支持 double/decimal） |
| `Ceiling(n)` | decimal 向上取整（默认保留2位小数） |
| `Floor(n)` | decimal 向下取整（默认保留2位小数） |
| `Cut(n)` | decimal 截取保留 N 位小数（不四舍五入） |
| `ToIntCeiling()` | double 向上取整为 int |
| `ToIntFloor()` | double 向下取整为 int |
| `ToIntRound()` | double 四舍五入取整为 int |
| `ToInt()` / `ToTryInt()` | long/double 转 int |
| `ToLong()` | double 转 long |
| `ToDateTime()` | long 时间戳转 DateTime |

---

### 1.12 NewtonsoftJsonExtensions —— JSON 扩展

#### 全局配置

| 方法 | 说明 |
|------|------|
| `SetGlobalJsonSerializerSetting(action)` | 设置全局 JsonSerializerSettings |
| `RestoreDefaultGlobalJsonSerializerSetting()` | 恢复默认配置 |
| `RemoveGlobalJsonSerializerSetting()` | 移除所有全局配置 |

#### 序列化

| 方法 | 说明 |
|------|------|
| `ToJson()` | 对象序列化为 JSON 字符串（支持全局配置/自定义 Setting/命名策略） |

#### 反序列化

| 方法 | 说明 |
|------|------|
| `ToObject<T>()` | JSON 字符串反序列化为实体（支持全局配置/自定义 Setting/Type） |
| `ToTryObject<T>()` | 安全反序列化（失败返回 default） |
| `ToJObject()` | JSON 字符串转 JObject |
| `ToTryJObject()` | 安全转 JObject |
| `ParamsToJson()` | key=value 参数字符串转 JSON |
| `ParamToObject<T>()` | key=value 参数字符串转实体 |
| `ToJsonString()` | 去除 JSON 中的换行、空格、制表符 |

**PropertyNamesContract 枚举:** `Default`, `CamelCase`, `PreserveDictionaryKeys`

---

## 二、工具类 (Helpers)

### 2.1 ChineseHelper —— 中文处理

| 方法 | 说明 |
|------|------|
| `GetChinesePinYinString(str)` | 字符串转拼音首字母 |
| `GetChinesePinYin(c)` | 取单个汉字拼音首字母 |
| `ExistsChinese(str)` | 判断字符串中是否存在汉字 |
| `DeleteChinese(str)` | 删除字符串开头的汉字 |

---

### 2.2 DateTimeHelper —— 时间戳工具

| 方法 | 说明 |
|------|------|
| `DateTimeToTimeStamp()` | 当前时间转10位时间戳 |
| `DateTimeToLongTimeStamp()` | 当前时间转13位时间戳 |
| `DateTimeToTimeStamp(dateTime)` | 指定 DateTime 转10位时间戳 |
| `DateTimeToLongTimeStamp(dateTime)` | 指定 DateTime 转13位时间戳 |
| `TimeStampToDateTime(timeStamp)` | 10位时间戳转 DateTime |
| `LongTimeStampToDateTime(timeStamp)` | 13位时间戳转 DateTime |
| `DateTimeToLocalTimeStamp()` | 当前本地时间转10位时间戳 |
| `DateTimeToLocalLongTimeStamp()` | 当前本地时间转13位时间戳 |

---

### 2.3 ConfigHelper —— INI 配置文件操作

| 方法 | 说明 |
|------|------|
| `SetConfigPath(path)` | 设置配置文件路径（默认 config.ini） |
| `UseConfigPath(path)` | 使用指定路径的配置文件，返回 ConfigUser 实例 |
| `GetValue(key)` | 获取字符串/int/bool 配置值 |
| `GetValue<T>(key)` | 获取泛型配置值（JSON 反序列化） |
| `SetValue(key, value)` | 设置配置值（支持 int/bool/string/泛型） |
| `Reload()` | 重新加载配置文件 |

**ConfigUser 类:** 独立配置文件实例，方法与 ConfigHelper 静态方法一致。

---

### 2.4 ComputerHelper —— 系统信息

| 方法 | 说明 |
|------|------|
| `GetComputerInfo()` | 获取计算机信息（CPU使用率、总内存、内存使用率、运行时间） |
| `GetCPURate()` | 获取 CPU 使用率（跨平台 Windows/Linux） |
| `GetRunTime()` | 获取系统启动时间 |

**ComputerInfo 模型:** `CPURate`, `TotalRAM`, `RAMRate`, `BeginRunTime`

---

### 2.5 FileHelper —— 文件操作

| 方法 | 说明 |
|------|------|
| `EnsurePathExists(path)` | 逐级检查并创建不存在的目录 |
| `MoveTo(sourceDir, destDir)` | 移动文件夹（含回滚） |
| `CopyTo(sourceDir, destDir)` | 复制文件夹（含子目录） |
| `DeleteDirectoryAndContents(targetDir)` | 递归删除目录及内容 |

---

### 2.6 IPAddressHelper —— IP 地址工具

| 方法 | 说明 |
|------|------|
| `Ipv4ToUInt32(ipAddress)` | IPv4 地址转 uint |
| `UInt32ToIpv4(uint)` | uint 转 IPv4 地址 |
| `IsPrivateNetwork(ip)` | 判断是否为局域网 IP |
| `Search(ipAddress)` | 查询 IP 归属地（国家\|省份\|城市\|运营商） |
| `ChangeAddress(attribution)` | IP归属地格式化输出（自动处理直辖市） |
| `GetLocalPublicIpAddressAsync()` | 异步获取本机公网 IP（多 API 源容错） |
| `LoadIP2RegionDb(path)` | 加载自定义 ip2region 数据库 |
| `LoadMaxMindDb(path)` | 加载自定义 MaxMind 数据库 |

**IpAttribution 模型:** `Country`, `Area`, `Province`, `City`, `Operator`, `IsPrivate`

---

### 2.7 ImageHelper —— 图片处理

| 方法 | 说明 |
|------|------|
| `ImageToBase64String(filePath)` | 图片文件转 Base64 字符串 |
| `Base64StringToImage(base64, filePath)` | Base64 字符串保存为图片 |

---

### 2.8 RandomHelper —— 随机数生成

| 方法 | 说明 |
|------|------|
| `GetRandomNumber(length)` | 生成纯数字随机数 |
| `GetRandomLetterUpper(length)` | 生成大写字母随机数 |
| `GetRandomLetterStringUpper(length)` | 生成大写字母+数字随机数 |
| `GetRandomLetterLower(length)` | 生成小写字母随机数 |
| `GetRandomLetterStringLower(length)` | 生成小写字母+数字随机数 |
| `GetRandomLetter(length)` | 生成大小写字母随机数 |
| `GetRandomString(length)` | 生成大小写字母+数字随机数 |
| `GetSMSCodeString()` | 生成6位短信验证码 |
| `GetRandomNickHeader()` | 获取随机用户昵称前缀（形容词） |
| `GetRandomNickFoot()` | 获取随机用户昵称后缀（名词） |
| `GetRandomNickName()` | 生成随机用户昵称 |

---

### 2.9 SecretHelper —— 加解密

#### 基础加密

| 方法 | 说明 |
|------|------|
| `CommonEncrypt(text)` / `CommonEncrypt(text, key)` | DES 加密（默认密钥 lycoris） |
| `CommonDecrypt(text)` / `CommonDecrypt(text, key)` | DES 解密 |

#### MD5

| 方法 | 说明 |
|------|------|
| `Md5Encrypt(str, lower)` | MD5 加密（32位） |
| `Md5Encrypt(stream)` | MD5 对文件流加密 |
| `Md5Encrypt16(input, encode)` | MD5 16位加密 |

#### AES

| 方法 | 说明 |
|------|------|
| `AesEncrypt(data, key, iv)` | AES 加密（返回 Base64） |
| `AesEncryptToByte(bytes, key, iv)` | AES 加密（返回 byte[]） |
| `AesDecrypt(data, key, iv)` | AES 解密 |
| `AesDecryptToByte(data, key, iv)` | AES 解密（返回 byte[]） |

#### DES

| 方法 | 说明 |
|------|------|
| `DESEncrypt(data, key, iv)` | DES 加密 |
| `DESDecrypt(data, key, iv)` | DES 解密 |

#### 3DES

| 方法 | 说明 |
|------|------|
| `TripleDESEncrypt(data, key)` | 3DES 加密（ECB模式） |
| `TripleDESEncrypt(data, key, iv, encoding, mode)` | 3DES 加密（完整参数） |
| `TripleDESDecrypt(data, key)` | 3DES 解密 |

#### Base64

| 方法 | 说明 |
|------|------|
| `Base64Encrypt(input)` | Base64 编码 |
| `Base64Decrypt(input)` | Base64 解码 |

#### RSA

| 方法 | 说明 |
|------|------|
| `GenerateRSAKey()` | 生成 RSA 密钥对 (RSAParameters) |
| `GenerateRSAXMLKey()` | 生成 RSA XML 格式密钥对 |
| `RSAEncrypt(publicKey, content)` | RSA 公钥加密 |
| `RSADecrypt(privateKey, content)` | RSA 私钥解密 |

#### SHA

| 方法 | 说明 |
|------|------|
| `SHA1Encrypt(input)` | SHA1 加密 |
| `SHA256Encrypt(input)` | SHA256 加密 |

---

### 2.10 ShellHelper —— Shell 命令执行

#### Bash 类 (Linux/macOS)

| 方法 | 说明 |
|------|------|
| `Run(command)` | 执行 Bash 命令 |
| `Echo(input)` | echo 输出 |
| `Grep(pattern, location)` | grep 搜索 |
| `Ls(flags)` | 列出文件 |
| `Mv(source, directory)` | 移动文件 |
| `Cp(source, directory)` | 复制文件 |
| `Rm(file)` | 删除文件 |
| `Cat(file)` | 查看文件内容 |

#### Cmd 类 (Windows)

| 方法 | 说明 |
|------|------|
| `Run(cmdText)` | 执行 CMD 命令 |
| `RunApplication(fileName, args)` | 启动应用程序并获取输出 |

**ShellBashResult:** `Output`, `ErrorMsg`, `ExitCode`, `Lines`

---

### 2.11 SqlCheckHelper —— SQL 注入检测

| 方法 | 说明 |
|------|------|
| `CheckKeyWord(word)` | 检查字符串是否包含 SQL 注入关键字或特殊符号 |

---

### 2.12 TaskHelper —— 任务调度

| 方法 | 说明 |
|------|------|
| `DelayAsync(beginSecond, endSecond)` | 随机延迟（区间秒数） |
| `SetTimeout(action, seconds)` | 延时执行（同步触发） |
| `SetTimeoutAsync(action, seconds)` | 延时执行（异步） |
| `WaitIfAsync(condition, second, ttl)` | 条件等待（按过期时间） |
| `WaitIfAsync(condition, second, maxCount)` | 条件等待（按最大次数） |
| `WaitForTimeSpanAsync(condition, second, method, ttl)` | 条件等待并循环执行方法（按过期时间，支持 Action/Func/异步） |
| `WaitForCountAsync(condition, second, method, count)` | 条件等待并循环执行方法（按次数，支持 Action/Func/异步） |

**WaitStatusEnum:** `BREAKE` (中断循环), `CONTINUE` (继续循环)

---

### 2.13 ToolHelper —— 工具

| 方法 | 说明 |
|------|------|
| `GetNetworkTimeAsync(ntpServer)` | 从 NTP 服务器获取网络时间（默认 ntp.ntsc.ac.cn） |

---

### 2.14 UrlHelper —— URL 工具

| 方法 | 说明 |
|------|------|
| `GetUrlPrefix(url)` | 提取 URL 中的协议和域名部分 |

---

### 2.15 UserAgentHelper —— UserAgent 解析

| 方法 | 说明 |
|------|------|
| `GetUserAgent(ua)` | 根据 UA 字符串识别浏览器/客户端类型 |
| `AddUserAgentData(data)` | 添加自定义 UA 识别规则 |

**内置支持识别:** Edge, Chrome, Safari, UC浏览器, 百度浏览器, QQ浏览器, 猎豹浏览器, 火狐, 360安全/极速浏览器, Opera, 小米浏览器, 微信, 手机QQ, IE, 安卓浏览器

---

### 2.16 XmlHelper —— XML 工具

| 方法 | 说明 |
|------|------|
| `Serializer(type, obj)` | 对象序列化为 XML 字符串 |
| `Deserialize<T>(xml)` | XML 反序列化为对象 |
| `Deserialize(type, xml)` | XML 反序列化为对象（指定类型） |
| `XmlAnalysis(stringRoot, xml)` | 获取 XML 指定节点的值 |

---

### 2.17 EnumHelper —— 枚举工具

| 方法 | 说明 |
|------|------|
| `GetEnumValues<T>()` | 获取枚举所有 int 值 |
| `CheckEnumValueExists<T>(enumValue)` | 验证枚举值是否存在 |
| `GetEnumsDescription<T, TResult>(selector)` | 遍历枚举项并获取自定义结果 |

---

## 三、配置管理 (ConfigurationManager)

### SettingManager

| 方法 | 说明 |
|------|------|
| `JsonConfigurationInitialization(path)` | 使用 JSON 文件初始化配置 |
| `JsonConfigurationInitialization(builder)` | 使用 Builder 初始化配置 |
| `GetConfig(key)` / `GetConfig<T>(key)` | 获取配置值（支持泛型） |
| `TryGetConfig(key)` / `TryGetConfig<T>(key)` | 安全获取配置值 |
| `GetSection(key)` / `GetSection<T>(key)` | 获取配置节点 |
| `TryGetSection(key)` / `TryGetSection<T>(key)` | 安全获取配置节点 |

### SettingManagerBuilder

| 方法 | 说明 |
|------|------|
| `AddJsonConfiguration(path)` | 添加 JSON 配置文件 |
| `AddJsonConfigurationWithEnvironment(func)` | 根据环境变量添加配置（默认 ASPNETCORE_ENVIRONMENT） |

---

## 四、工具类 (Utils)

### 4.1 DebouncerUtils —— 防抖

| 方法 | 说明 |
|------|------|
| `DebouncerUtils(action, delayMs)` | 构造函数，传入要防抖的动作和延迟毫秒 |
| `Trigger()` | 触发（若在延迟时间内重复调用会重置计时器） |

---

### 4.2 ThrottlerUtils —— 节流

| 方法 | 说明 |
|------|------|
| `ThrottlerUtils(action, intervalMs)` | 构造函数，传入要节流的动作和间隔毫秒 |
| `Trigger()` | 触发（在间隔时间内重复调用会被忽略） |

---

### 4.3 SensitiveWordUtils —— 敏感词过滤

| 方法 | 说明 |
|------|------|
| `LoadDefault()` / `LoadDefaultAsync()` | 加载内置默认敏感词库 |
| `LoadJsonFile(path)` / `LoadJsonFileAsync(path)` | 从 JSON 文件加载敏感词 |
| `LoadTxtFile(path)` / `LoadTxtFileAsync(path)` | 从 TXT 文件加载敏感词（每行一个） |
| `AddFilterWords(words)` | 动态添加敏感词 |
| `GetAllSensitiveWords(input)` | 获取输入中所有敏感词 |
| `CheckSensitiveWords(input)` | 检测是否包含敏感词 |
| `SensitiveWordsReplace(input, replaceChar)` | 将敏感词替换为指定字符（默认 *） |
| `HideSensitiveInfo(info, left, right)` | 隐藏敏感信息（保留头尾指定字符数） |
| `HideSensitiveInfo(info, sublen)` | 隐藏敏感信息（按比例保留头尾） |
| `AsMemoryStore()` | 将词库存入全局内存（配合 SensitiveWordMemoryStore 使用） |
| `GetWordsFilterLibrary()` | 获取过滤词库 Hashtable |

### 4.4 SensitiveWordMemoryStore —— 全局敏感词记忆库

| 方法 | 说明 |
|------|------|
| `CheckSensitiveWords(input)` | 检测是否包含敏感词 |
| `SensitiveWordsReplace(input, replaceChar)` | 敏感词替换 |
| `AddFilterWords(words)` | 动态添加敏感词 |

---

### 4.5 FileCertUtils —— 加密文件存储

| 方法 | 说明 |
|------|------|
| `FileCertUtils(key, iv, fileHeader)` | 构造函数（AES-128, 16字节 Key/IV, 4字节文件签名） |
| `SaveAsync(stream, outputPath, fileName, contentType)` | 加密并保存文件（含元数据和 SHA256 校验） |
| `ReadAsync(filePath)` | 解密并读取文件（返回 EncryptedFileModel） |

**EncryptedFileModel:** `FileName`, `ContentType`, `CreatedAt`, `Content`, `ContentAsString`, `IsSuccess`, `ErrorMessage`

---

### 4.6 SeoUtils —— SEO 推送

| 方法 | 说明 |
|------|------|
| `BaiduApiPushAsync(token, urls)` | 百度链接推送（自动分页，每次最多50条） |
| `BingApiPushAsync(apiKey, urls)` | 必应链接推送 |
| `GoogleApiPushAsync(apiKey, urls)` | 谷歌链接推送 |
| `NotifyBaiduUrlInvalidAsync(token, urls)` | 通知百度失效链接 |
| `NotifyGoogleUrlInvalidAsync(apiKey, url)` | 通知谷歌失效链接 |
| `GenerateSitemapFiles(urls, savePath)` | 生成 Sitemap XML 文件（每文件最多500条） |
| `PushSitemapToBaiduAsync(token, sitemapUrls)` | 推送 Sitemap 到百度 |
| `PushSitemapToGoogleAsync(sitemapUrls)` | 推送 Sitemap 到谷歌 |

---

## 五、模型类 (Models)

| 类 | 命名空间 | 说明 |
|------|------|------|
| `AsciiCompareStrings` | `Lycoris.Common.Extensions.Models` | ASCII 码排序比较器（升序） |
| `PropertyComparer<T>` | `Lycoris.Common.Extensions.Models` | 按属性名去重的相等比较器 |
| `OrWhereBuilder<T>` | `Lycoris.Common.Extensions.Builder.LinqBuilder` | LINQ OR 条件动态构建器 |
| `ComputerInfo` | `Lycoris.Common.Helper.Models` | 计算机信息（CPU/内存/运行时间） |
| `UserAgentData` | `Lycoris.Common.Helper.Models` | UserAgent 识别数据模型 |
| `IpAttribution` | `Lycoris.Common.Helper` | IP 归属地信息 |
| `EncryptedFileModel` | `Lycoris.Common.Utils.Cert` | 加密文件解密结果 |
| `ShellBashResult` | `Lycoris.Common.Helper` | Shell 命令执行结果 |
| `ConfigUser` | `Lycoris.Common.Helper` | 独立 INI 配置文件操作实例 |
| `SnowflakeIdInfo` | `Lycoris.Common.Snowflakes` | 雪花Id解析结果 |
| `SnowflakeOption` | `Lycoris.Common.Snowflakes.Options` | 单机雪花Id配置 |
| `DistributedSnowflakeOption` | `Lycoris.Common.Snowflakes.Options` | 分布式雪花Id配置 |

---

## 六、雪花Id (Snowflakes)

基于 Twitter Snowflake 算法的分布式 Id 生成器，支持单机和分布式（Redis）两种部署模式。

### 6.1 单机模式

#### 注册服务

```csharp
// 注册为单例服务
builder.Services.AddSnowflake().AsService();

// 注册为静态实例
builder.Services.AddSnowflake().AsHelper();

// 详细配置
builder.Services.AddSnowflake(opt =>
{
    opt.WorkId = 1;                                    // 工作机器Id，默认从1开始
    opt.WorkIdLength = 10;                             // 工作机器Id所占用的长度，最大10，默认10
    opt.StartTimeStamp = new DateTime(2022, 1, 1);     // 起始时间，设置为固定时间，不要设为 DateTime.Now
}).AsService();
```

#### 使用方式

```csharp
// 单例服务注入
public class Demo
{
    private readonly ISnowflakeMaker _snowflakeMaker;
    public Demo(ISnowflakeMaker snowflakeMaker) => _snowflakeMaker = snowflakeMaker;

    public long GetId() => _snowflakeMaker.GetNextId();
    public async Task<long> GetIdAsync() => await _snowflakeMaker.GetNextIdAsync();
}

// 静态实例
public class Demo
{
    public long GetId() => SnowflakeHelper.GetNextId();
    public async Task<long> GetIdAsync() => await SnowflakeHelper.GetNextIdAsync();

    // 批量获取
    public long[] GetBatch() => SnowflakeHelper.GetNextIds(100);

    // 解析Id
    public SnowflakeIdInfo Parse(long id) => SnowflakeHelper.Parse(id);
}
```

### 6.2 分布式模式（Redis）

分布式模式需要 Redis 辅助分配机器Id，需实现 `IDistributedSnowflakesRedis` 接口。

#### 实现 Redis 辅助服务

```csharp
// 以 CSRedisCore 为例
public class DistributedSnowflakesRedis : IDistributedSnowflakesRedis
{
    private readonly CSRedisClient client;

    public DistributedSnowflakesRedis()
    {
        client = new CSRedisClient("host:port,password=password,defaultDatabase=0");
    }

    public Task<bool> ExpireAsync(string key, TimeSpan expire) => client.ExpireAsync(key, expire);
    public Task<long> IncrByAsync(string key, long value) => client.IncrByAsync(key, value);
    public Task<long> ZAddAsync(string key, params (decimal, object)[] scoreMembers) => client.ZAddAsync(key, scoreMembers);
    public Task<long> ZRemAsync<T>(string key, params T[] member) => client.ZRemAsync(key, member);
    public Task<(string member, decimal score)[]> ZRangeByScoreWithScoresAsync(string key, decimal min, decimal max, long? count = null, long offset = 0)
        => client.ZRangeByScoreWithScoresAsync(key, min, max, count, offset);
}
```

#### 单例服务模式

```csharp
// 注册
builder.Services.AddDistributedSnowflake(opt =>
{
    opt.WorkId = 1;
    opt.WorkIdLength = 10;
    opt.StartTimeStamp = new DateTime(2022, 1, 1);
    opt.RedisPrefix = "MyService";                     // 集群标识前缀
    opt.RefreshAliveInterval = TimeSpan.FromHours(1);  // 心跳间隔，默认1小时
})
.AddSnowflakesRedisService<DistributedSnowflakesRedis>()
.AsService();

// 使用：同单机模式的单例服务
```

#### 静态实例模式

```csharp
// 无参构造函数
builder.Services.AddDistributedSnowflake()
    .AddSnowflakesRedisHelper<DistributedSnowflakesRedis>()
    .AsHelper();

// 有参构造函数
var redisHelper = new DistributedSnowflakesRedis(redis, ...);
builder.Services.AddDistributedSnowflake()
    .AddSnowflakesRedisHelper(redisHelper)
    .AsHelper();

// 使用
var id = DistributedSnowflakeHelper.GetNextId();
var batch = DistributedSnowflakeHelper.GetNextIds(100);
var info = DistributedSnowflakeHelper.Parse(id);
```

### 6.3 接口一览

| 类/接口 | 说明 |
|------|------|
| `ISnowflakeMaker` | 雪花Id生成器接口 |
| `SnowflakeHelper` | 单机静态帮助类 |
| `SnowflakesMakerService` | 单机DI服务 |
| `DistributedSnowflakeHelper` | 分布式静态帮助类 |
| `DistributedSnowflakeService` | 分布式DI服务 |
| `IDistributedSnowflakesRedis` | 分布式Redis操作接口（需自行实现） |
| `IDistributedSnowflakesSupport` | 分布式支持接口 |
| `SnowflakeIdInfo` | Id解析结果（Timestamp, WorkId, Sequence） |

---

## 开源协议

MIT License

Copyright (c) 2023 lycoris

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
