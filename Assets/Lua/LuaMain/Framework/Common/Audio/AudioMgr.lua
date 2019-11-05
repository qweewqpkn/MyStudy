local AudioMgr = BaseClass("AudioMgr", Singleton)
local AudioManager = CS.AudioManager

function AudioMgr:__init()
    AudioMgr.AudioType = {
        BG = "BG",
        COMMON2D = "2D",
        COMMON3D = "3D",
    }
end

function AudioMgr:PlayBG(name, isLoop, delayTime)
    isLoop = isLoop or true
    delayTime = delayTime or 0
    return AudioManager.instance:PlayAudioBG(AudioMgr.AudioType.BG, name, isLoop, delayTime)
end

function AudioMgr:Play2D(name, isLoop, delayTime)
    isLoop = isLoop or false
    delayTime = delayTime or 0
    return AudioManager.instance:PlayAudio2D(AudioMgr.AudioType.COMMON2D, name, isLoop, delayTime)
end

function AudioMgr:Play3D(name, obj, maxRange, isLoop, delayTime)
    maxRange = maxRange or 10
    isLoop = isLoop or true
    delayTime = delayTime or 0
    return AudioManager.instance:PlayAudio3D(AudioMgr.AudioType.COMMON3D, name, obj, maxRange, isLoop, delayTime)
end

function AudioMgr:Play(type, name, isLoop, delayTime, createType, spaceType, releaseType, obj, maxRange)
    isLoop = isLoop or false
    delayTime = delayTime or 0
    createType = createType or AudioManager.AudioCreateType.eNew
    spaceType = spaceType or AudioManager.AudioSpaceType.e2D
    releaseType = releaseType or AudioManager.AudioReleaseType.eAuto
    maxRange = maxRange or 0

    return AudioManager.instance:PlayAudio(type, name, isLoop, delayTime, createType, spaceType, releaseType, obj, maxRange)
end

function AudioMgr:StopAndDisposeAudioByType(type)
    AudioManager.instance:StopAndDisposeAudioByType(type)
end

function AudioMgr:StopAudioByType(type)
    AudioManager.instance:StopAudioByType(type)
end

function AudioMgr:StopAndDisposeAll()
    AudioManager.instance:StopAndDisposeAll()
end

function AudioMgr:StopAndDisposeAudioByID(id)
    AudioManager.instance:StopAndDisposeAudioByID(id)
end

function AudioMgr:StopAudioByID(id)
    AudioManager.instance:StopAudioByID(id)
end

function AudioMgr:MuteAudioByID(id, isMute)
    AudioManager.instance:MuteAudioByID(id, isMute)
end

function AudioMgr:MuteAudioAll(isMute)
    AudioManager.instance:MuteAudioAll(isMute)
end

function AudioMgr:MuteAudioByType(type, isMute)
    AudioManager.instance:MuteAudioByType(type, isMute)
end

function AudioMgr:SetAudioVolumeByType(type, volume)
    AudioManager.instance:SetAudioVolumeByType(type, volume)
end

function AudioMgr:SetAudioVolumeByID(id, volume)
    AudioManager.instance:SetAudioVolumeByID(id, volume)
end

function AudioMgr:GetAudioClipLength(name, callback)
    AudioManager.instance:GetAudioClipLength(name, callback)
end

return AudioMgr