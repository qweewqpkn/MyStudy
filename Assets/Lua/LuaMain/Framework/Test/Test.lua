local Test = BaseClass("Test")

function Test:Start()
    --测试计时器
    --local testTimer = require "Framework.Test.TestTimer"
    --testTimer:Test4()

    --测试缓存池
    --local testPoolManager = require "Framework.Test.TestPoolManager"
    --testPoolManager:Test1()

    --测试模拟unity的更新周期
    --local testUpdatable = require "Framework.Test.TestUpdatable"
    --local t = testUpdatable.New()

    --测试类的继承构造函数和析构函数
    --local testSubClass = require "Framework.Test.TestClass.TestSubClass"
    --local t = testSubClass.New(123, "测试lua")
    --t:Test()
    --t:Delete()

    --测试Messenger框架内的消息通信
    --local testMessenger = require "Framework.Test.TestMessenger"
    --testMessenger:Test3()

    --测试协程
    --local testCoroutine = require "Framework.Test.TestCoroutine"
    --testCoroutine:Test1()
    --testCoroutine:Test2()

    --测试ui
    --local testUI = require "Framework.Test.TestUI"
    --testUI:Test1()
    --testUI:Test2()
    --testUI:Test3()
    --testUI:Test4()
    --testUI:Test5()
    --testUI:Test6()
    --testUI:Test7()

    --测试场景
    --local testScene = require "Framework.Test.TestScene"
    --testScene:Test1()
end

return Test