local CameraControl = BaseClass("CameraControl", Singleton)
local Input = CS.UnityEngine.Input
local PhysicsExtention = CS.PhysicsExtention

function CameraControl:Init(camera, bounds, layerName)
    self.mCamera = camera
    self.mBounds = bounds
    self.mLayerName = layerName
    self.mSingleFingerEnter = false
    self.mOldPosition1 = Vector3.zero
    self.mOldPosition2 = Vector3.zero
    self.mLayer = 2 ^ CS.UnityEngine.LayerMask.NameToLayer(layerName)
    self.mDistance = nil
    self.mIsStop = false
    self.mLastMousePos = Vector3.zero
    self.mMinDistance = 20
end

function CameraControl:Stop(isStop)
    self.mIsStop = isStop
end

function CameraControl:Update()
    if(self.mIsStop) then
        return
    end

    if(IsNull(self.mCamera)) then
        Logger.E("CameraControl mCamera is null")
        return
    end

    if(not GuideMgr:GetInstance():IsFinishAll())then
        return
    end

    if(self.mDistance == nil) then
        if(IsNull(self.mCamera:GetComponent(typeof(CS.UnityEngine.Animator))) and self.mDistance == nil) then
            --等待动画播放完成,获取初始距离
            local isHit, hitInfo = self:GetHitInfo(self.mCamera.transform.position)
            if(hitInfo ~= nil) then
                self.mDistance = hitInfo.distance
                self.mMaxDistance = self.mDistance + 20
                if(self.mMaxDistance < self.mMinDistance) then
                    Logger.E("CameraControl self.mMaxDistance < self.mMinDistance")
                end
                --Logger.W("self.mMaxDistance" .. self.mMaxDistance)
            end
        end
        return
    end

    --处理缩放
    local scaleValue = 0
    if(Input.touchCount > 1) then
        if(self.mSingleFingerEnter) then
            self.mOldPosition1 = Input.GetTouch(0).position
            self.mOldPosition2 = Input.GetTouch(1).position
            self.mSingleFingerEnter = false
        end

        if(Input.GetTouch(0).phase == TouchPhase.unity_touchphase.Moved or Input.GetTouch(1).phase == TouchPhase.unity_touchphase.Moved) then
            local tempPosition1 = Input.GetTouch(0).position
            local tempPosition2 = Input.GetTouch(1).position
            scaleValue = self:GetScale(self.mOldPosition1, self.mOldPosition2, tempPosition1, tempPosition2)
            self.mOldPosition1 = tempPosition1
            self.mOldPosition2 = tempPosition2
            --Logger.L("asdffs@kkkkk ：" .. scaleValue)
        end
    else
        self.mSingleFingerEnter = true

        if( (Input.touchCount == 1 and Input.touches[0].phase == TouchPhase.unity_touchphase.Began) or Input.GetMouseButtonDown(0)) then
            if CS.UnityEngine.Application.isMobilePlatform then
                self.mLastMousePos = Input.touches[0].position
            else
                self.mLastMousePos = Input.mousePosition
            end
        end

        if((Input.touchCount == 1 and Input.touches[0].phase == TouchPhase.unity_touchphase.Moved) or Input.GetMouseButton(0)) then
            if CS.UnityEngine.Application.isMobilePlatform then
                self.mCurMousePos = Input.touches[0].position
            else
                self.mCurMousePos = Input.mousePosition
            end

            local deltaPos = self.mCurMousePos - self.mLastMousePos
            local moveDis = self.mCamera.transform.right * deltaPos.x + self.mCamera.transform.up * deltaPos.y
            local moveScale = Mathf.Lerp(-0.01, -0.05, (self.mDistance - self.mMinDistance) / (self.mMaxDistance - self.mMinDistance))
            local tempPos = self.mCamera.transform.position + moveDis * moveScale
            self.mCamera.transform.position = self:RestrictViewPointInBounds(tempPos)
            self.mLastMousePos = self.mCurMousePos
        end

        if((Input.touchCount == 1 and Input.touches[0].phase == TouchPhase.unity_touchphase.Ended) or Input.GetMouseButtonUp(0)) then
            if(not UIUtil.IsTouchedUI())then
                UserTrackMgr.Track("BATTLE_CLICK_NO_UI_AREA")
            end
        end

        scaleValue = Input.mouseScrollDelta.y
    end

    if(scaleValue ~= 0) then
        self.mDistance = self.mDistance + scaleValue * 2.5 * -1
        --Logger.W("self.mDistance" .. self.mDistance)
        if(self.mDistance > self.mMaxDistance) then
            self.mDistance = self.mMaxDistance
        elseif(self.mDistance < self.mMinDistance) then
            self.mDistance = self.mMinDistance
        end
        self.mCamera.transform.position = self:RestrictViewPointInBounds(self.mCamera.transform.position)
    end
end

function CameraControl:GetScale(oP1, oP2, nP1, nP2)
    --函数传入上一次触摸两点的位置与本次触摸两点的位置计算出用户的手势
    local lastDistance = Vector2.Distance(oP1, oP2)
    local curDistance = Vector2.Distance(nP1, nP2)
    local distance = (curDistance - lastDistance) * 0.4 * Time.deltaTime
    return distance
end

--获取摄像机的点击点信息
function CameraControl:GetHitInfo(tempPos)
    local ray = Ray.unity_ray(tempPos, self.mCamera.transform.forward)
    local isHit, hitInfo = PhysicsExtention.Raycast(ray, 800, self.mLayer)
    return isHit, hitInfo
end

--限制视点在指定区域内
function CameraControl:RestrictViewPointInBounds(tempPos)
    local isHit, hitInfo = self:GetHitInfo(tempPos)
    local min = self.mBounds.min
    local max = self.mBounds.max
    local hitPoint = Vector3(hitInfo.point.x, hitInfo.point.y, hitInfo.point.z)
    if(hitPoint.x < min.x) then
        hitPoint.x = min.x
    end

    if(hitPoint.z < min.z) then
        hitPoint.z = min.z
    end

    if(hitPoint.x > max.x) then
        hitPoint.x = max.x
    end

    if(hitPoint.z > max.z) then
        hitPoint.z = max.z
    end

    local pos = hitPoint - self.mDistance * self.mCamera.transform.forward
    return pos
end

function CameraControl:MoveTo(pos, offset, duration)
    if(IsNull(self.mCamera)) then
        return
    end

    self.mCamera.transform:DOMove(pos + offset, duration)
end

function CameraControl:LookAt(dir, duration)
    if(IsNull(self.mCamera)) then
        return
    end

    self.mCamera.transform:DOLookAt(dir, 1)
end

--晃动摄像头
function CameraControl:Shake(duration, strength, vibrato, randomness, snaapping, fadeOut)
    if(IsNull(self.mCamera)) then
        return
    end

    self.mCamera.transform:DOShakePosition(duration, strength, vibrato, randomness, snaapping, fadeOut)
end

return CameraControl