local NumberStrUtil = BaseClass("NumberStrUtil")

local BASE_K = 1000
local BASE_M = BASE_K * BASE_K
local BASE_B = BASE_K * BASE_M

local function FormatNumber(inNum, separator, minimumSeparateLen, decPoint, decimalsLen, showZero)
	local pow = math.pow(10, decimalsLen)
	local num = math.abs(inNum) 
	local integer = math.floor(num)

	num = num * pow

	local fNumber = integer * pow
	local dNumber = math.floor(num - fNumber)

	local decimals = dNumber / pow
	local lPart = string.format("%d", integer)
	local rPart = string.sub(tostring(decimals), 3)

	if showZero then
		rPart = rPart or "0"
	end

	local outStr = ""
	local len = #lPart
	local fLen = len % 3

	if minimumSeparateLen ~= nil and len <= minimumSeparateLen then
		outStr = lPart
	else
		while len > 3 do
			outStr = separator..string.sub(lPart, len - 2, len)..outStr
			len = len - 3
		end
		outStr = string.sub(lPart, 1, (fLen == 0) and 3 or fLen)..outStr
	end

	
	if #rPart > 0 and decimalsLen > 0 then
		if #rPart < decimalsLen then
			outStr = outStr..(decPoint..rPart)
		else
			outStr = outStr..(decPoint..string.sub(rPart, 1, decimalsLen))
		end
	end

	return ((inNum < 0) and "-" or "")..outStr
end

function NumberStrUtil.FormatNumberToIntgeterDisString(inNum)
    local absNum = math.abs(inNum)
    local unit
    if absNum >= BASE_B then
        absNum = absNum / BASE_B
        unit = "B"
    elseif absNum >= BASE_M then
        absNum = absNum / BASE_M
        unit = "M"
    elseif absNum >= BASE_K then
        absNum = absNum / BASE_K
        unit = "K"
    else
        absNum = absNum
        unit = ""
    end

    -- At this point, $num is smaller than 1000. Keep 2 decimal.
    absNum = math.floor((absNum * 100 + 0.000001)) / 100
    if unit == "" then
    	absNum = string.format("%d", math.floor(absNum))
    	return inNum < 0 and "-" or "" .. absNum .. unit
    end
    return inNum < 0 and "-" or "" .. string.format("%.2f", absNum) .. unit
end

function NumberStrUtil.FormatNumberToIntgeterDisString1(inNum)
	local absNum = math.abs(inNum)
	local unit
	if absNum >= BASE_B then
		absNum = absNum / BASE_B
		unit = "B"
	elseif absNum >= BASE_M then
		absNum = absNum / BASE_M
		unit = "M"
	elseif absNum >= BASE_K then
		absNum = absNum / BASE_K
		unit = "K"
	else
		absNum = math.floor(absNum)
		unit = ""
	end

	return inNum < 0 and "-" or "" .. absNum .. unit
end

--不同国家显示不一样,目前默认使用英文显示方式
function NumberStrUtil.FormatLocalNumber(inNum, decimalsLen, showZero)
	return FormatNumber(inNum, ",", 3, ".", decimalsLen or 0, showZero or false)
end

return NumberStrUtil