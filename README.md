# WPFLauncher

> **不要开我喵**：侵犯权益联系删喵
---

## 目录结构

```
../
└── ../                # 源码，72 个 .cs
    ├── WPFLauncher/Function.cs                    # ★ 密钥解包 + 模组处理
    ├── Mark/x19Crypt.cs                           # ★ AES-CFB 模组解密核心
    ├── WPFLauncher.Pages/ItemDownloadProgress.cs  # ★ 模组下载全流程
    ├── WPFLauncher.Network.Http/X19Crypto.cs      # HTTP 传输加密
    ├── WPFLauncher.Modules.Login/NeteaseLogin.cs  # 登录协议链
    ├── Noya.LocalServer.Common.Cryptography/AESHelper.cs  # 加密原语
    └── ...
```

### 模组下载与解密

流程在 `ItemDownloadProgress.ItemDownloadProgress_OnLoaded`，三步 HTTP + 本地处理：

#### 第 1 步 — 取下载链接（**不加密**）

```
POST https://x19mclobt.nie.netease.com/pe-item/query/search-lobby-by-id-list
{"item_ids":[...]}
```
`ProtocolOption.Normal`。返回每个模组的 `lobby_res_url`、`res_name`、`iid`。

#### 第 2 步 — 取解密密钥（加密）

```
POST https://x19obtcore.nie.netease.com:8443/pe-item/get-encryption-key-list-for-guests
{"device_id":"", "item_ids":[...]}
```
`ProtocolOption.CommonEncrypt`。注意 **`device_id` 传空字符串**，走 guest 路径。

#### 第 3 步 — 从 JWT 取 key（**不验签**）

```csharp
string[] array = jsonItem["jwt"].ToString().Split('.');
string text = array[1];                  // 直接切 payload 段
...base64 padding + decode...
JObject jObject = JObject.Parse(...);
string decryptionKey = Function.GetDecryptionKey(
    device_id, jObject["contentKey"].ToString(), Var.CurrentAccount.UserID);
```

#### 第 4 步 — 下载 → 解压 → 解密

```csharp
await Function.DownloadFile(url, ..., ...);          // 下到启动器当前目录
Function.ExtractZip(zip, DownloadPath, ...);         // 解压
Function.UnzipModJsonPath(DownloadPath, Key, UUID);  // 原地解密
Function.DeleteFile(zip);                            // 删 zip
```

#### 解密算法：两层

**第一层 — `contentKey` 解包（`Function.GetDecryptionKey`）**

```csharp
string text = "TG8hVJD3Lt1r86Cv" + user_id + device_id;
byte[] bytes = Encoding.UTF8.GetBytes(text);
byte[] array = Convert.FromBase64String(TextContentKey);
for (int num = length - 1; num != -1; num--)
    array[num % 16] ^= bytes[num];        // 倒着走，下标模 16
return Encoding.ASCII.GetString(array);
```

服务端下发的 `contentKey` **本身就是明文密钥**，只是 base64 编码后再跟
`固定盐 + user_id + device_id` 逐字节 XOR。不是密钥派生，纯混淆。

**第二层 — 文件解密（`x19Crypt.DecryptModJson`）**

文件结构：

```
偏移 0..4    4 字节      忽略
偏移 4..40   36 字节     UUID（ASCII 明文，用于校验密钥是否正确）
偏移 40..64  24 字节     忽略
偏移 64..    AES-128-CFB 密文
```

```csharp
if (Encoding.ASCII.GetString(array.Skip(4).Take(36).ToArray()) != UUID)
    return null;                          // 头部校验
array = array.Skip(64).ToArray();         // 跳过 64 字节头

if (array.Length % 16 == 0)  return AES_CFB_Decrypt(key, array, key);
if (array.Length <  16)      return AES_CFB_Decrypt(key, pad16(array),    key).Take(array.Length).ToArray();
return                              AES_CFB_Decrypt(key, padNext(array), key).Take(array.Length).ToArray();
```

- **算法**：AES-128-**CFB**（`CipherMode.CFB` + `PaddingMode.Zeros`）
- **IV 就是密钥本身**（`AES_CFB_Decrypt(key, array, key)`）
- CFB 是流模式，所以补零纯粹是为满足 `CryptoStream` 的块要求，
  补出的字节最后由 `Take(原长)` 丢弃

> **实现要点**：分支 2、3 的 `.Take(n)` 用的是**原始密文长度**。
> 写错了会导致非块对齐的模组尾部多出垃圾数据。

**验证**（`mod_decrypt.py`，含负向测试）：

```
content-key unwrap OK  (key='TG8hVJD3Lt1r86Cv')
mod decrypt OK        payload=114B  blob=178B     ← 非对齐
mod decrypt OK        payload= 32B  blob= 96B     ← 整块对齐
mod decrypt OK        payload=  7B  blob= 71B     ← 不足一块
UUID header check OK  (wrong uuid rejected)
```

#### 解压后处理（`Function.UnzipModJsonPath`）

- 遍历所有文件做解密（`GetAllFiless`）
- `*.mczip` → `ZipFile.ExtractToDirectory` 后删除
- `*.mergedmcs` → 同上，`Substring(0, len-10)` 去掉后缀当目录名

---
