# gRPC protobuf 文件檔案 #

![landing.svg](/images/landing.png)

DomainService 與 Application 的通訊橋樑，開發流程：

1. 先寫 .proto 檔案
2. 實作 DomainService 服務器端程式
3. 實作 Application 客戶端程式
4. 整合測試
5. 建立部署腳本 .yaml 與.sh 檔案

## Execute Step ##

Execute command on root directory:

    /Users/ct.tsai/.nuget/packages/grpc.tools/1.16.0/tools/macosx_x64/protoc -I ./protos/ --csharp_out src/MockSite.Message --grpc_out src/MockSite.Message ./protos/UserMessage.proto ./protos/UserService.proto --plugin=protoc-gen-grpc=/Users/ct.tsai/.nuget/packages/grpc.tools/1.16.0/tools/macosx_x64/grpc_csharp_plugin

`--plugin=protoc-gen-grpc` should be absolutly path.

**Could use [brew install gRPC](https://github.com/grpc/homebrew-grpc) to avoid gRPC tool path issue**. But plugin still need full path to activate.

## Reference ##

- [gRPC](https://grpc.io/)
- [Protocol Buffers](https://developers.google.com/protocol-buffers/)