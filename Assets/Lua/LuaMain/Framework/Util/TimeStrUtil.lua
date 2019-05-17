
local TimeStrUtil = BaseClass("TimeStrUtil")

function TimeStrUtil.TimeToString1(time)
	if(time < 0) then
		time = 0
	end

    local hour = time / 3600
    local minute = time % 3600 / 60
    local second = time % 60
    if(time > 3600) then
        return string.format("%d:%d:%d", hour, minute, second)
    elseif (time > 60) then
        return string.format("%d:%d", minute, second)
    end

    return string.format("%d", second)
end

function TimeStrUtil.TimeToString2(time)
	if(time < 0) then
		time = 0
	end

	local hour = math.floor(time / 3600)
    local minute = math.floor(time % 3600 / 60)
    local second = time % 60

    return string.format("%02d:%02d:%02d", hour, minute, second)
end

function TimeStrUtil.TimeToString3(time)
    local day = time / 86400
    local hour = time % 86400 / 3600
    local minute = time % 3600 / 60
    local second = time % 60
    if(time > 86400)then
        return string.format("%d天%d小时", math.floor(day), math.floor(hour))
    elseif(time > 3600) then
        return string.format("%d小时%d分", math.floor(hour), math.floor(minute))
    elseif (time > 60) then
        return string.format("%d分%d秒", math.floor(minute), math.floor(second))
    end

    return string.format("%d秒", math.floor(second))
end

function TimeStrUtil.TimeToString4(time)
    local day = time / 86400
    local hour = time % 86400 / 3600
    local minute = time % 3600 / 60
    local second = time % 60

    if(time > 86400)then
        return string.format("%d天%d小时%d分", math.floor(day), math.floor(hour), math.floor(time))
    elseif(time > 3600) then
        return string.format("%d小时%d分%d秒", math.floor(hour), math.floor(minute), math.floor(second))
    elseif (time > 60) then
        return string.format("%d分%d秒", math.floor(minute), math.floor(second))
    end

    return string.format("%d秒", math.floor(second))
end

function TimeStrUtil.TimeToString5(time)
    local day = math.floor(time / 86400)
    local leave = time % 86400
    if(leave > 0) then
        return day + 1 .. "天"
    end
end

return TimeStrUtil