# gRPC protobuf 文件檔案 #

![landing.svg](/images/landing.png)

DomainService 與 Application 的通訊橋樑，開發流程：

1. 先依需求撰寫 .proto 檔案
2. 手動建置 src\MockSite.Message ，以產生最新的 .cs 檔案提供給 `MockSite.Application` 與 `MockSite.Client` 參考
3. 實作 DomainService 服務器端程式
4. 實作 Application 客戶端程式
5. 整合測試
6. 建立部署腳本 .yaml 與.sh 檔案

## Reference ##

- [gRPC](https://grpc.io/)
- [Protocol Buffers](https://developers.google.com/protocol-buffers/)