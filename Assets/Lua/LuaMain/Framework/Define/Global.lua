require "Framework.Common.BaseClass"
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
LayerMask	= require "Framework.Common.Tools.UnityEngine.LayerMask"
Plane		= require "Framework.Common.Tools.UnityEngine.Plane"
Time		= require "Framework.Common.Tools.UnityEngine.Time"
Object		= require "Framework.Common.Tools.UnityEngine.Object"

-- util
GoUtil = require "Framework.Util.GoUtil"
LuaUtil = require "Framework.Util.LuaUtil"
StringUtil = require "Framework.Util.StringUtil"
TableUtil = require "Framework.Util.TableUtil"
TimeStrUtil = require "Framework.Util.TimeStrUtil"
UIUtil = require "Framework.Util.UIUtil"
TypeConvertUtil = require "Framework.Util.TypeConvertUtil"

list = require "Framework.Common.Tools.list"
require "Framework.Common.Tools.event"
Config = require "Framework.Define.Config"
Singleton = require "Framework.Common.Singleton"
UIManager = require "Framework.UI.UIManager"
SceneManager = require "Framework.Scene.SceneManager"
UIBase = require "Framework.UI.UIBase"
Logger = require "Framework.Common.Logger"

-- update & time
require "Framework.Updater.Coroutine"
Timer = require "Framework.Updater.Timer"
TimerManager = require "Framework.Updater.TimerManager"
UpdateManager = require "Framework.Updater.UpdateManager"

UIManager:GetInstance()
UpdateManager:GetInstance()