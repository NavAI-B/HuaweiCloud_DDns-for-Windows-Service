## 使用方式

* 打开 `HuaweiCloud DDns.sln`

* 还原丢失的 `nuget` 包

* 生成项目

* 到 `HuaweiCloud DDns.exe` 同级目录创建 `config.json` 内容如下

  ```json
  {
      "LOG_PATH":"ServiceLog.log",
      "HUAWEICLOUD_SDK_AK":"",
      "HUAWEICLOUD_SDK_SK":"",
      "HUAWEICLOUD_DNS_REGION":"cn-north-4",
      "HUAWEICLOUD_DNS_PUBLIC_ZONE":"",
      "HUAWEICLOUD_DNS_RECORDERSET":"",
      "HUAWEICLOUD_DNS_NSTYPE":""
  }
  ```

  * `HUAWEICLOUD_SDK_AK` 和 `HUAWEICLOUD_SDK_SK` 指 `IAM统一认证ID` 和其 `私钥`
  * `HUAWEICLOUD_DNS_REGION` 指 `地区和终端节点`，`cn-north-4` 指 `中国-北京-线路4`
  * `HUAWEICLOUD_DNS_PUBLIC_ZONE` 指 `根域名`，最后要加一个 `.` 如：`baidu.com.`
  * `HUAWEICLOUD_DNS_RECORDERSET` 指 `解析记录集` 的 `域名` 部分, 最后要加一个 `.` 例：`www.baidu.com.`
  * `HUAWEICLOUD_DNS_NSTYPE` 指 `记录类型`, 如：`A` 指 `解析到IPv4`, 而 `AAAA` 指 `解析到IPv6`
  * 先确保已经在 `云解析控制台` 创建好对应的解析记录

* 以管理员身份执行 [InstallUtil.exe](https://learn.microsoft.com/zh-cn/dotnet/framework/tools/installutil-exe-installer-tool) 安装 `HuaweiCloud DDns.exe` 为服务，然后启动服务

* 代码极其简单，要啥功能真的可以随便改写 (lll￢ω￢)

## 相关链接

[API概览_云解析服务 DNS_华为云 (huaweicloud.com)](https://support.huaweicloud.com/api-dns/zh-cn_topic_0132421999.html)

[地区和终端节点 云解析服务 (huaweicloud.com)](https://console.huaweicloud.com/apiexplorer/#/endpoint/DNS)

[IAM统一身份认证服务 控制台](https://console.huaweicloud.com/iam)

[命令行(curl)获取IPv4和IPv6地址 | IP查询(ipw.cn)](https://ipw.cn/doc/ipv6/user/cmd_getip.html)