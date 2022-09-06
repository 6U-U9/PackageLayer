# 软件包分层算法

*现在项目已经迁移到Refactor下，Layer项目仅供存档，不再使用*

**运行环境**: `.net6.0`

**代码入口**：`Program.cs`

**项目结构**：

	- core ：包装了算法运行中一些基础的数据结构，以及定义了输入输出的处理
	- resources ：算法的输入放在该文件夹下，由 Visual Studio 生成的资源文件`Resources.resx`进行管理

- steps : 定义了算法一步的处理逻辑，继承自 `core.step` 泛型类，泛型定义了主处理函数 `Process()` 的输入类型和返回类型
- procedures：定义了算法一次的处理流程，继承自 `core.procedure` 类，算法的一次运行即为对 `Execute()`的一次调用。

**如何使用**：

`Program.cs` 的结构如下

```c#
List<string> envs = new List<string> {"mysql" , "nginx", "redis"};
foreach (string e in envs)
{
    Procedure origin = new Origin(e,e);
    ...

    origin.Execute();
    ...
}
```

算法一次运行即为对于 `Procedure` 类 `Execute()` 方法的一次调用。目前数据总共有三个场景`mysql`,`nginx`,`redis`；`Procedure` 类	的构造函数第一个参数即为场景名，第二个参数为输出的 Excel 文件名。

**如何修改**：

1. 添加继承自 `Step` 的类，Override 主处理函数 `Process()` ，完成算法一步的处理逻辑，并添加相应描述。
2. 添加继承自 `Producer` 的类，实例化每一步所使用的 `Step` 类，Override 主处理函数 `Execute()` ，并整合相应描述。
3. 在`Program.cs`中创建 `Producer` 的类实例，并调用 `Execute()` 运行。



