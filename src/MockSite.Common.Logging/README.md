# INFRASTRUCTURE Logging #

## Initialize Logger ##

1. 加入serilogsettings.json (可從Gaming那邊參考)
2. 程式碼加入

       ```csharp
            LoggerHelper.Instance.SetLogProvider(SerilogProvider.Instance);
        ```

## Logger 架構 ##

- Logger Sturcture
  -  LoggerHelper
  -  LogProvider
     -  Serilog
  -  LoggerHelper.Instance 提供四種類型的Log
     -  Debug
     -  Info
     -  Error
     -  Performance

## 目前定義Log的種類 ##

- LogDetail 
  - DebugDetail
  - InfoDetail
  - ErrorDetail
  - PerformanceDetail

定義這四種格式的Log, 每種格式裡面有不同的欄位, 利用定義好的欄位來使Log變成結構化的Log 格式

```csharp
2018-12-11 15:46:31.241 [Perf]{"Target":"MapDataToMatch","Duration":"18101","Message":null,"Parameter":{"PreStartCount":143,"InplayCount":10},"Remark":null,"$type":"PerformanceDetail"} 
```

可以呼叫 LoggerHelp.Instance 裡面的多載方法, 範例如下

```csharp
        public void Info(string message)
        {
            if (_service != null)
            {
                var detail = GetInfoDetail(message);
                _service.Info(detail);
            }
        }
        
        public void Info(InfoDetail detail)
        {
            if (_service != null)
            {
                _service.Info(detail);
            }
        }
```

