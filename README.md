# xQuant 日志分析工具

[^author]: Leon
---


- ## 1.项目介绍

  xQuant 企业日志分析工具

  // TODO ...

- ## 2.项目架构

  | 路径                                        | 角色             | 说明                                   |
  | :------------------------------------------ | ---------------- | -------------------------------------- |
  | *..\BIZ*\\**Analysiser**                    | 日志分析器       | 分析日志解析结果                       |
  | *..\BIZ\Analysiser*\\**DirectedAnalysiser** | 定向分析器       | 深入提取解析结果内日志内容包含的数据   |
  | *..\BIZ\Analysiser*\\**GroupAnalysiser**    | 组分析器         | 将开始和结束解析结果匹配为分析结果     |
  | *..\BIZ*\\**Exporter**                      | 导出器           | 日志分析结果导出器                     |
  | *..\BIZ*\\**FileFinder**                    | 文件查找器       | 查找指定目录内与任务相关的文件         |
  | *..\BIZ*\\**Parser**                        | 日志解析器       | 使用监视规则命中日志文件内容为解析结果 |
  | *..\Model*\\**EqualityComparer**            | 实体比较器       | 使用自定义规则分析实体是否指向相同数据 |
  | *..\Model*\\**Extensions**                  | 实体扩展         | 实体静态扩展                           |
  | *..\Model*\\**Factory**                     | 实体工厂         | 实体工厂                               |
  | *..\Model*\\**Fixed**                       | 固定数据         | 枚举或常量                             |
  | *..\Model*\\**Monitor**                     | 监视规则实体     | 监视规则或容器                         |
  | *..\Model*\\**Report**                      | 报告相关实体     | 导出报告相关实体                       |
  | *..\Model*\\**Result**                      | 日志处理结果实体 | 日志解析结果、分析结果、中间件结果     |
  | *..\\***Monitor**                                 | 监视规则配置文件 | 预设监视规则配置XML文件                |
  | *..\\***ReportTemplate**                          | 导出报告模板     | 预设导出报告模板                       |
  | *..\\***Utils**                                   | 工具集合         | 静态工具、助手                         |

- ## 3.业务流程

- ### 3.1. 创建任务

  ​	xQuant日志分析工具（下称分析工具）公开多个参数以允许使用者配置精细的分析任务，任务参数说明及配置要求如下。

  ##### 	任务参数说明：

  | 参数名称 | 说明               | 备注     | 格式         | 示例                              |
  | -------- | ------------------ | -------- | ------------ | --------------------------------- |
  | logdir   | 日志文件存放目录   | **必选** | 绝对路径     | C:\TEST_DIR 或 "C:\TEST DIR"      |
  | monitor  | 监视规则文件名称   | **必选** | 相对路径     | monitor.xml                       |
  | start    | 日志开始时间       | **选填*  | 24小时格式   | "2018-10-01 17:30:00"             |
  | finish   | 日志结束时间       | **选填*  | 24小时格式   | "2018-11-11 08:40:00"             |
  | sysinfo  | 是否记录系统信息   | **选填*  | 可忽略大小写 | true 或 false                     |
  | cltinfo  | 是否记录客户端信息 | **选填*  | 可忽略大小写 | true 或 false                     |
  | report   | 导出报告格式       | **选填*  | 默认:excel   | excel 或 html 或 word             |
  | level    | 日志文件的等级     | **选填*  | 默认:debug   | debug 或 trace 或 info 或 pref 等 |

  ​	**注意：参数数据内存在空格时，需要在参数外加英文双引号嵌套起来；*

  ​	**命令行参数传入格式：**

    - 单个参数格式：参数名称=参数数据

    - 多个参数格式：各个参数之间使用空格分隔

      **示例：**

      ```css
      logdir=D:\Desktop\LogDir "finish = 2018-11-11 18:30:00" monitor=client.xml report=excel level=debug
      ```

  ​	当未传入参数或参数传入出现错误时，会在控制台输出具体的参数错误原因，并弹出GUI任务创建窗口，供使用者使用窗口配置任务参数，详细使用方法见<span id="taskFactory"> 任务工厂 </span>章节； 

- ### 3.2.解析日志文件

  ​	分析工具将使用系统内实现的多个解析器按照监视规则的配置分别解析客户端日志文件、服务端日志文件、中间件日志文件，并将解析结果统一维护到解析结果池中；

  ​	具体解析流程：

  ```flow
  parse=>start: 解析开始
  readLog=>operation: 读取日志行
  generalRegex=>condition: 使用概要正则匹配日志
  checkTime=>condition: 验证日志时间范围
  giveUp=>operation: 放弃数据
  particularRegex=>operation: 使用详细正则匹配日志
  generalError=>subroutine: 记录未解析结果
  checkMonitor=>condition: 应用所有监视规则匹配日志内容
  monitSuccess=>subroutine: 记录所有解析结果
  readToEnd=>condition: 日志文件读取结束
  end=>end
  
  parse->readLog->generalRegex
  generalRegex(yes)->checkTime
  generalRegex(no)->generalError(right)->readLog
  checkTime(yes)->particularRegex->checkMonitor
  checkTime(no)->giveUp
  checkMonitor(yes)->monitSuccess->readToEnd
  checkMonitor(no)->giveUp
  readToEnd(yes)->end
  readToEnd(no)->readLog
  ```

- ### 3.3.分析日志解析结果

  ​	

- ### 3.4.导出分析结果

- ## 4.主要数据实体

  ### 4.1.TaskArgument

  ​	任务参数实体，作为任务所有配置、数据的管理者，以此集中存储任务相关相关的所有配置参数、日志文件、监视规则、监视结果、分析结果、系统信息等数据；

  ​	是一次完整任务的直接体现

  ### 4.2.LogFile

  ​	日志文件实体，记录任务处理的日志文件信息，如：文件路径、相对路径*(相对于任务配置的日志文件目录)*、日志文件类型、文件创建时间、上次写入时间、关联的监视结果、关联的分析结果、关联的未解析结果、中间件日志结果、关联的分析结果总耗时等；

  ​	是任务所有延伸结果的数据源；

  ### 4.3.MonitorContainer

  ​	监视结果容器实体，每个监视规则配置文件(*.xml)对应一个监视规则容器对象，以此容器实现对监视规则树的管理，包括初始化监视规则树关系、自动填充父子或兄弟监视规则配置、自动继承父级监视规则配置等功能；

  ​	是各个监视规则的奶妈；

  ### 4.4.MonitorItem

  ​	监视规则实体，允许使用者通过配置严谨的监视规则实现丰富的日志分析需求；

  ​	可定制属性：开始条件、结束条件、组分析器、定向分析器、是否监视内存消耗、结果输出表名等；

  ​	可计算属性：所有分析结果总耗时、分析结果平均耗时、监视结果匹配率、监视结果所在树级层深等；

  ​	可管理数据：关联的解析结果、关联的分析结果等；

  ​	是分析工具可定制化的最大体现；

  ​	详细配置教程见<span id="monitorTutorial"> 监视规则配置教程 </span>章节；

  ### 4.5.MonitorResult

  ### 4.6.UnparsedResult

  ### 4.7.GroupAnalysisResultContainer

  ### 4.8.GroupAnalysisResult

  ### 4.9.MiddlewareResult

- ## 5.功能介绍

- ### 5.1.[任务工厂](#taskFactory)

- #### 5.1.1.参数工厂

- #### 5.1.2.GUI工厂

- ### 5.2.解析器

- #### 5.2.1.客户端日志解析器

- #### 5.2.2.服务端日志解析器

- #### 5.2.3中间件日志解析器

- ### 5.3.分析器

- #### 5.3.1.分析器Host

- #### 5.3.2组分析器

- #### 5.3.3定向分析器

- ### 5.4.导出器

- ## 6.流程间数据过滤条件

- ## 7.[监视规则配置教程](#monitorTutorial)

- ### 7.1.精简自动填充开始条件

- ### 7.2.组分析器使用场景

- ### 7.3.定向分析器使用场景