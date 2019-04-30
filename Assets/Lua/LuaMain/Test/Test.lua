local Test = BaseClass("Test")

function Test:Start()
    --测试计时器
    --local testTimer = require "Test.TestTimer"
    --testTimer:Test3()

    --测试缓存池
    --local testPoolManager = require "Test.TestPoolManager"
    --testPoolManager:Test1()

    --测试模拟unity的更新周期
    --local testUpdatable = require "Test.TestUpdatable"
    --local t = testUpdatable.New()

    --测试类的继承构造函数和析构函数
    --local testSubClass = require "Test.TestClass.TestSubClass"
    --local t = testSubClass.New(123, "测试lua")
    --t:Delete()

    --测试Messenger框架内的消息通信
    --local testMessenger = require "Test.TestMessenger"
    --testMessenger:Test3()

    --测试协程
    local testCoroutine = require "Test.TestCoroutine"
    testCoroutine:Test1()
end

return Test