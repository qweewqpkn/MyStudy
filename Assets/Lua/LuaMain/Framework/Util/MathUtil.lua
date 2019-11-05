-- 最小数值和最大数值指定返回值的范围
local function clamp(v, minValue, maxValue)  
    if v < minValue then
        return minValue
    end

    if v > maxValue then
        return maxValue
    end

    return v 
end

--四舍五入
local function round(v)  
	return math.floor(v + 0.5)
end

local function RandomRange(min, max)
  return math.random(math.floor(min * 100000.0), math.floor(max * 100000.0)) * 0.00001
end

math.clamp = clamp
math.round = round
math.random_range = RandomRange