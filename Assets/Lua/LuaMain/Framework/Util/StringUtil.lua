--[[
-- added by wsh @ 2017-12-18
-- string扩展工具类，对string不支持的功能执行扩展
--]]

local unpack = table.unpack

-- 字符串分割
-- @split_string：被分割的字符串
-- @pattern：分隔符，可以为模式匹配
-- @init：起始位置
-- @plain：为true禁用pattern模式匹配；为false则开启模式匹配
local function split(split_string, pattern, search_pos_begin, plain)
	assert(type(split_string) == "string")
	assert(type(pattern) == "string" and #pattern > 0)
	search_pos_begin = search_pos_begin or 1
	plain = plain or true
	local split_result = {}

	while true do
		local find_pos_begin, find_pos_end = string.find(split_string, pattern, search_pos_begin, plain)
		if not find_pos_begin then
			break
		end
		local cur_str = ""
		if find_pos_begin > search_pos_begin then
			cur_str = string.sub(split_string, search_pos_begin, find_pos_begin - 1)
		end
		split_result[#split_result + 1] = cur_str
		search_pos_begin = find_pos_end + 1
	end

	--if search_pos_begin < string.len(split_string) then
	split_result[#split_result + 1] = string.sub(split_string, search_pos_begin)
	--end

	return split_result
end

-- 字符串连接
function join(join_table, joiner)
	if #join_table == 0 then
		return ""
	end

	local fmt = "%s"
	for i = 2, #join_table do
		fmt = fmt .. joiner .. "%s"
	end

	return string.format(fmt, unpack(join_table))
end

-- 是否包含
-- 注意：plain为true时，关闭模式匹配机制，此时函数仅做直接的 “查找子串”的操作
function contains(target_string, pattern, plain)
	plain = plain or true
	local find_pos_begin, find_pos_end = string.find(target_string, pattern, 1, plain)
	return find_pos_begin ~= nil
end

-- 以某个字符串开始
function startswith(target_string, start_pattern, plain)
	plain = plain or true
	local find_pos_begin, find_pos_end = string.find(target_string, start_pattern, 1, plain)
	return find_pos_begin == 1, find_pos_begin, find_pos_end
end

-- 以某个字符串结尾
function endswith(target_string, start_pattern, plain)
	plain = plain or true
	local find_pos_begin, find_pos_end = string.find(target_string, start_pattern, -#start_pattern, plain)
	return find_pos_end == #target_string
end

function IsNullOrEmpty(str)
	return str == "" or str == nil
end

local function CharCount(inputstr)
	local lenInByte = #inputstr
	local width = 0
	local i = 1
	while (i<=lenInByte) 
	do
		local curByte = string.byte(inputstr, i)
		local byteCount = 1
		if curByte > 0 and curByte <= 127 then
		    byteCount = 1                                           --1字节字符
		elseif curByte >= 192 and curByte < 223 then
		    byteCount = 2                                           --双字节字符
		elseif curByte >= 224 and curByte < 239 then
		    byteCount = 3                                           --汉字
		elseif curByte >= 240 and curByte <= 247 then
		    byteCount = 4                                           --4字节字符
		end

		local char = string.sub(inputstr, i, i + byteCount-1)

		i = i + byteCount                                 -- 重置下一字节的索引
		width = width + 1                                 -- 字符的个数（长度）
	end
	return width
end 

string.split = split
string.join = join
string.contains = contains
string.startswith = startswith
string.endswith = endswith
string.IsNullOrEmpty = IsNullOrEmpty
string.CharCount = CharCount
