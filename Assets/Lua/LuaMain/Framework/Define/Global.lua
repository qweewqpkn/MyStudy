require "Framework.Common.Tools.strict"
require "Framework.Common.BaseClass"
require "Framework.Define.Rename"
require "Framework.Define.Consts"

-- unityengine
Mathf		= require "Framework.Common.Tools.UnityEngine.Mathf"
Vector2		= require "Framework.Common.Tools.UnityEngine.Vector2"
Vector3 	= require "Framework.Common.Tools.UnityEngine.Vector3"
Vector4		= require "Framework.Common.Tools.UnityEngine.Vector4"
Quaternion	= require "Framework.Common.Tools.UnityEngine.Quaternion"
Color		= require "Framework.Common.Tools.UnityEngine.Color"
Ray			= require "Framework.Common.Tools.UnityEngine.Ray"
Bounds		= require "Framework.Common.Tools.UnityEngine.Bounds"
RaycastHit	= require "Framework.Common.Tools.UnityEngine.RaycastHit"
Touch		= require "Framework.Common.Tools.UnityEngine.Touch"
--LayerMask	= require "Framework.Common.Tools.UnityEngine.LayerMask"
Plane		= require "Framework.Common.Tools.UnityEngine.Plane"
Time		= require "Framework.Common.Tools.UnityEngine.Time"
Object		= require "Framework.Common.Tools.UnityEngine.Object"

-- util
GoUtil = require "Framework.Util.GoUtil"
LuaUtil = require "Framework.Util.LuaUtil"
StringUtil = require "Framework.Util.StringUtil"
MathUtil = require "Framework.Util.MathUtil"
TableUtil = require "Framework.Util.TableUtil"
TimeStrUtil = require "Framework.Util.TimeStrUtil"
UIUtil = require "Framework.Util.UIUtil"
TypeConvertUtil = require "Framework.Util.TypeConvertUtil"
Updatable = require "Framework.Common.Updatable"
json = require "Framework.Common.Tools.Json4Lua"

Singleton = require "Framework.Common.Singleton"
SingletonManager = require "Framework.Common.SingletonManager"
list = require "Framework.Common.Tools.list"
require "Framework.Common.Tools.event"
Messenger = require "Framework.Common.Messenger"
Config = require "Framework.Define.Config"
UIBase = require "Framework.UI.UIBase"
UIConfig = require "Framework.UI.UIConfig"
UIManager = require "Framework.UI.UIManager"
SceneBase = require "Framework.Scene.SceneBase"
SceneConfig = require "Framework.Scene.SceneConfig"
SceneManager = require "Framework.Scene.SceneManager"
Logger = require "Framework.Common.Logger"
PoolManager = require "Framework.Pool.PoolManager"
Updatable = require "Framework.Common.Updatable"
SmartGOManager = require "Framework.Common.SmartGOManager"

-- update & time
require "Framework.Updater.Coroutine"
Timer = require "Framework.Updater.Timer"
TimerManager = require "Framework.Updater.TimerManager"
UpdateManager = require "Framework.Updater.UpdateManager"

SmartGOManager = require "Framework.Common.SmartGOManager"


UIManager:GetInstance()
UpdateManager:GetInstance()